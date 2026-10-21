import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { LucideFileText } from '@lucide/angular';

import { ApiValidationProblemDetails } from '../../auth/auth.models';
import { CursoDto } from '../cursos.models';
import { CursosService } from '../cursos.service';

@Component({
  imports: [LucideFileText, RouterLink],
  selector: 'app-detalhes-curso',
  templateUrl: './detalhes-curso.html',
})
export class DetalhesCurso {
  private readonly route = inject(ActivatedRoute);
  private readonly cursosService = inject(CursosService);

  protected readonly curso = signal<CursoDto | null>(null);
  protected readonly carregando = signal(true);
  protected readonly mensagemErro = signal('');

  constructor() {
    const cursoId = this.route.snapshot.paramMap.get('cursoId');

    if (!cursoId) {
      this.mensagemErro.set('Identificador do curso inválido.');
      this.carregando.set(false);
      return;
    }

    this.cursosService.obterPorId(cursoId).subscribe({
      next: (curso) => {
        this.curso.set(curso);
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

  private obterMensagemErro(resposta: HttpErrorResponse): string {
    const problem = resposta.error as ApiValidationProblemDetails | null;

    return problem?.detail ?? 'Não foi possível carregar o curso. Tente novamente em instantes.';
  }
}
