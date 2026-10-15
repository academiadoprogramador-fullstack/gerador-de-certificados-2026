import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideArrowRight, LucideAward, LucideSparkles } from '@lucide/angular';

import { AuthService } from '../auth/auth.service';
import { Navbar } from '../shared/navbar/navbar';

@Component({
  imports: [LucideAward, LucideSparkles, LucideArrowRight, Navbar, RouterLink],
  selector: 'app-home',
  templateUrl: './home.html',
})
export class Home {
  protected readonly authService = inject(AuthService);
  protected readonly sessaoAutenticada = this.authService.sessao;
}
