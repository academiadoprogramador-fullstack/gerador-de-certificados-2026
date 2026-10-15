import { Observer } from 'rxjs';

import { HttpErrorResponse } from '@angular/common/http';
import { afterNextRender, Component, ElementRef, inject, signal, viewChild } from '@angular/core';
import { email, form, FormField, minLength, pattern, required } from '@angular/forms/signals';
import { Router, RouterLink } from '@angular/router';
import { LucideArrowRight } from '@lucide/angular';

import {
  ApiValidationProblemDetails,
  CadastroConcluidoNavigationState,
  CadastroRequest,
  UsuarioDto,
} from '../auth.models';
import { AuthService } from '../auth.service';

interface CadastroFormModel {
  readonly email: string;
  readonly senha: string;
}

@Component({
  imports: [LucideArrowRight, FormField, RouterLink],
  selector: 'app-cadastro',
  templateUrl: './cadastro.html',
})
export class Cadastro {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  private readonly tituloPagina = viewChild<ElementRef<HTMLHeadingElement>>('tituloPagina');

  protected readonly modeloCadastro = signal<CadastroFormModel>({ email: '', senha: '' });
  protected readonly formularioCadastro = form(this.modeloCadastro, (modelo) => {
    required(modelo.email, { message: 'Informe seu email.' });
    email(modelo.email, { message: 'Informe um email válido.' });
    required(modelo.senha, { message: 'Informe sua senha.' });
    minLength(modelo.senha, 8, { message: 'A senha precisa ter pelo menos 8 caracteres.' });
    pattern(modelo.senha, /(?=.*\d)(?=.*[^a-zA-Z0-9]).+/, {
      message: 'Inclua pelo menos um número e um caractere especial.',
    });
  });

  protected readonly mensagemErro = signal('');
  protected readonly enviandoFormulario = signal(false);

  constructor() {
    afterNextRender(() => this.tituloPagina()?.nativeElement.focus());
  }

  protected enviarFormulario(event: SubmitEvent): void {
    event.preventDefault();

    const formulario = this.formularioCadastro();

    if (formulario.invalid()) {
      formulario.markAsTouched();
      return;
    }

    const modelo = this.modeloCadastro();

    const requisicao: CadastroRequest = {
      email: modelo.email,
      senha: modelo.senha,
    };

    this.mensagemErro.set('');
    this.enviandoFormulario.set(true);

    const observer: Partial<Observer<UsuarioDto>> = {
      next: (usuario) => {
        this.enviandoFormulario.set(false);

        const estadoNavegacao: CadastroConcluidoNavigationState = {
          emailCadastrado: usuario.email,
        };

        void this.router.navigateByUrl('/auth/login', { state: estadoNavegacao });
      },
      error: (response: HttpErrorResponse) => {
        this.enviandoFormulario.set(false);

        const mensagem =
          response.status === 409
            ? 'Este email já está cadastrado. Entre na sua conta ou use outro email.'
            : this.obterMensagemErroApi(response);

        this.mensagemErro.set(mensagem);
      },
    };

    this.authService.cadastrar(requisicao).subscribe(observer);
  }

  private obterMensagemErroApi(response: HttpErrorResponse): string {
    const problem = response.error as ApiValidationProblemDetails | null;

    if (problem?.errors) {
      const mensagens = Object.values(problem.errors).flat();

      if (mensagens.length > 0) return mensagens.join('\n');
    }

    return problem?.detail ?? 'Não foi possível criar sua conta. Tente novamente em instantes';
  }
}
