import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideArrowRight } from '@lucide/angular';

import { PaginAutenticacao } from '../pagina-autenticacao';

@Component({
  imports: [LucideArrowRight, PaginAutenticacao, RouterLink],
  selector: 'app-login',
  templateUrl: './login.html',
})
export class Login {}
