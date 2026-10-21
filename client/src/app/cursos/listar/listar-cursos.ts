import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideBookOpen, LucideCirclePlus } from '@lucide/angular';

import { ApiValidationProblemDetails } from '../../auth/auth.models';
import { CursoDto } from '../cursos.models';
import { CursosService } from '../cursos.service';

@Component({
  imports: [LucideCirclePlus, LucideBookOpen, RouterLink],
  selector: 'app-listar-cursos',
  templateUrl: './listar-cursos.html',
})
export class ListarCursos {
  private readonly cursosService = inject(CursosService);

  protected readonly cursos = signal<readonly CursoDto[]>([]);
  protected readonly mensagemErro = signal('');
  protected readonly carregando = signal(true);

  constructor() {
    this.carregarCursos();
  }

  private carregarCursos(): void {
    this.carregando.set(true);
    this.mensagemErro.set('');

    this.cursosService.listar().subscribe({
      next: (cursos) => {
        this.cursos.set(cursos);
        this.carregando.set(false);
      },
      error: (resposta: HttpErrorResponse) => {
        this.mensagemErro.set(this.obterMensagemErro(resposta));
        this.carregando.set(false);
      },
    });
  }

  protected formatarData(dataConclusao: string): string {
    return new Date(dataConclusao).toLocaleDateString();
  }

  private obterMensagemErro(response: HttpErrorResponse): string {
    const problem = response.error as ApiValidationProblemDetails | null;

    return problem?.detail ?? 'Não foi possível carregar os cursos. Tente novamente em instantes.';
  }
}
