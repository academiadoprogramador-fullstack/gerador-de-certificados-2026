import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import {
  LucideArrowRight,
  LucideAward,
  LucideLibraryBig,
  LucideLogIn,
  LucideSparkles,
  LucideUserRoundPlus,
} from '@lucide/angular';

import { AuthService } from '../auth/auth.service';
import { Navbar } from '../shared/navbar/navbar';

@Component({
  imports: [
    LucideLogIn,
    LucideUserRoundPlus,
    LucideAward,
    LucideSparkles,
    LucideLibraryBig,
    LucideArrowRight,
    Navbar,
    RouterLink,
  ],
  selector: 'app-home',
  templateUrl: './home.html',
})
export class Home {
  protected readonly authService = inject(AuthService);
  protected readonly sessaoAutenticada = this.authService.sessao;
}
