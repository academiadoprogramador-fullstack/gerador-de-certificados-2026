import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideFileText, LucideLogIn, LucideUserRoundArrowLeft } from '@lucide/angular';

import { AuthService } from '../../auth/auth.service';

@Component({
  imports: [LucideUserRoundArrowLeft, LucideLogIn, LucideFileText, RouterLink],
  selector: 'app-navbar',
  templateUrl: './navbar.html',
})
export class Navbar {
  protected readonly authService = inject(AuthService);
  protected readonly sessaoAutenticada = this.authService.sessao;
}
