import { afterNextRender, Component, ElementRef, inject, signal, viewChild } from '@angular/core';
import { email, form, FormField, minLength, pattern, required } from '@angular/forms/signals';
import { Router, RouterLink } from '@angular/router';
import { LucideArrowRight } from '@lucide/angular';

import { CadastroConcluidoNavigationState, CadastroRequest } from '../auth.models';
import { AuthService } from '../auth.service';
import { AuthShell } from '../shell/auth-shell';

interface CadastroFormModel {
  readonly email: string;
  readonly senha: string;
}

@Component({
  imports: [LucideArrowRight, AuthShell, FormField, RouterLink],
  selector: 'app-cadastro',
  templateUrl: './cadastro.html',
})
export class Cadastro {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  private readonly tituloPagina = viewChild<ElementRef<HTMLHeadingElement>>('tituloPagina');

  protected readonly modeloFormulario = signal<CadastroFormModel>({ email: '', senha: '' });
  protected readonly formularioCadastro = form(this.modeloFormulario, (modelo) => {
    required(modelo.email, { message: 'Informe seu email.' });
    email(modelo.email, { message: 'Informe um email válido.' });
    required(modelo.senha, { message: 'Informe sua senha.' });
    minLength(modelo.senha, 8, { message: 'A senha precisa ter pelo menos 8 caracteres.' });
    pattern(modelo.senha, /(?=.*\d)(?=.*[^a-zA-Z0-9]).+/, {
      message: 'Inclua pelo menos um número e um caractere especial.',
    });
  });

  constructor() {
    afterNextRender(() => this.tituloPagina()?.nativeElement.focus());
  }

  protected enviarFormulario(event: SubmitEvent): void {
    event.preventDefault();

    if (this.formularioCadastro().invalid()) {
      this.formularioCadastro().markAsTouched();
      return;
    }

    const request: CadastroRequest = {
      email: this.modeloFormulario().email,
      senha: this.modeloFormulario().senha,
    };

    this.authService.cadastrar(request).subscribe((usuario) => {
      const estadoNavegacao: CadastroConcluidoNavigationState = {
        emailCadastrado: usuario.email,
      };

      void this.router.navigateByUrl('/login', { state: estadoNavegacao });
    });
  }
}
