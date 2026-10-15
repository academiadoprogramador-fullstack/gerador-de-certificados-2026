import { Observer } from 'rxjs';

import { HttpErrorResponse } from '@angular/common/http';
import { afterNextRender, Component, ElementRef, inject, signal, viewChild } from '@angular/core';
import { email, form, FormField, minLength, required } from '@angular/forms/signals';
import { Router, RouterLink } from '@angular/router';
import { LucideArrowRight } from '@lucide/angular';

import {
  ApiValidationProblemDetails,
  CadastroConcluidoNavigationState,
  LoginRequest,
  TokenDto,
} from '../auth.models';
import { AuthService } from '../auth.service';

interface LoginFormModel {
  readonly email: string;
  readonly senha: string;
}

@Component({
  imports: [LucideArrowRight, FormField, RouterLink],
  selector: 'app-login',
  templateUrl: './login.html',
})
export class Login {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly modeloLogin = signal<LoginFormModel>({ email: '', senha: '' });
  protected readonly formularioLogin = form(this.modeloLogin, (modelo) => {
    required(modelo.email, { message: 'Informe seu email.' });
    email(modelo.email, { message: 'Informe um email válido.' });
    required(modelo.senha, { message: 'Informe sua senha.' });
    minLength(modelo.senha, 8, { message: 'A senha precisa ter pelo menos 8 caracteres.' });
  });

  protected readonly mensagemSucesso = signal('');
  protected readonly mensagemErro = signal('');

  protected readonly enviandoFormulario = signal(false);

  private readonly tituloPagina = viewChild<ElementRef<HTMLHeadingElement>>('tituloPagina');

  constructor() {
    afterNextRender(() => this.tituloPagina()?.nativeElement.focus());

    const estadoNavegacao = this.router.currentNavigation()?.extras.state as
      CadastroConcluidoNavigationState | undefined;

    if (estadoNavegacao?.emailCadastrado) {
      this.modeloLogin.update((original) => ({
        email: estadoNavegacao?.emailCadastrado ?? '',
        senha: original.senha,
      }));

      this.mensagemSucesso.set(
        'Conta criada com sucesso. Entre com suas credenciais para continuar.',
      );
    }
  }

  protected enviarFormulario(event: SubmitEvent): void {
    event.preventDefault();

    const formulario = this.formularioLogin();

    if (formulario.invalid()) {
      formulario.markAsTouched();
      return;
    }

    const modelo = this.modeloLogin();

    const requisicao: LoginRequest = {
      email: modelo.email,
      senha: modelo.senha,
    };

    this.mensagemErro.set('');
    this.enviandoFormulario.set(true);

    const observer: Partial<Observer<TokenDto>> = {
      next: () => {
        this.enviandoFormulario.set(false);

        void this.router.navigateByUrl('/');
      },
      error: (response: HttpErrorResponse) => {
        this.enviandoFormulario.set(false);

        const mensagem =
          response.status === 401
            ? 'Email ou senha incorretos. Configura suas credenciais e tente novamente.'
            : this.obterMensagemErroApi(response);

        this.mensagemErro.set(mensagem);
      },
    };

    this.authService.login(requisicao).subscribe(observer);
  }

  private obterMensagemErroApi(response: HttpErrorResponse): string {
    const problem = response.error as ApiValidationProblemDetails | null;

    return problem?.detail ?? 'Não foi possível conectar à API. Tente novamente em instantes';
  }
}
