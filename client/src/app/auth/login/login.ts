import { afterNextRender, Component, ElementRef, inject, signal, viewChild } from '@angular/core';
import { email, form, FormField, minLength, required } from '@angular/forms/signals';
import { Router, RouterLink } from '@angular/router';
import { LucideArrowRight } from '@lucide/angular';

import { CadastroConcluidoNavigationState } from '../auth.models';
import { AuthShell } from '../shell/auth-shell';

interface LoginFormModel {
  readonly email: string;
  readonly senha: string;
}

@Component({
  imports: [LucideArrowRight, AuthShell, FormField, RouterLink],
  selector: 'app-login',
  templateUrl: './login.html',
})
export class Login {
  private readonly router = inject(Router);

  protected readonly modeloFormulario = signal<LoginFormModel>({ email: '', senha: '' });
  protected readonly formularioLogin = form(this.modeloFormulario, (modelo) => {
    required(modelo.email, { message: 'Informe seu email.' });
    email(modelo.email, { message: 'Informe um email válido.' });
    required(modelo.senha, { message: 'Informe sua senha.' });
    minLength(modelo.senha, 8, { message: 'A senha precisa ter pelo menos 8 caracteres.' });
  });

  private readonly tituloPagina = viewChild<ElementRef<HTMLHeadingElement>>('tituloPagina');

  constructor() {
    afterNextRender(() => this.tituloPagina()?.nativeElement.focus());

    const estadoNavegacao = this.router.currentNavigation()?.extras.state as
      CadastroConcluidoNavigationState | undefined;

    if (estadoNavegacao?.emailCadastrado) {
      this.modeloFormulario.update((original) => ({
        email: estadoNavegacao?.emailCadastrado ?? '',
        senha: original.senha,
      }));
    }
  }
}
