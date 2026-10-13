import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideAward, LucideFileText } from '@lucide/angular';

@Component({
  imports: [LucideFileText, LucideAward, RouterLink],
  selector: 'app-pagina-autenticacao',
  templateUrl: './pagina-autenticacao.html',
})
export class PaginAutenticacao {}
