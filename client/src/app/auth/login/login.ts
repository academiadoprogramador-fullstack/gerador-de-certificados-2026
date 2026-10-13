import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideArrowRight } from '@lucide/angular';

import { PaginaAutenticacao } from '../pagina-autenticacao';

@Component({
  imports: [LucideArrowRight, PaginaAutenticacao, RouterLink],
  selector: 'app-login',
  templateUrl: './login.html',
})
export class Login {}
