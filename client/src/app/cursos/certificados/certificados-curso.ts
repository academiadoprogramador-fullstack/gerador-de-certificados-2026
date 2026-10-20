import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { ApiValidationProblemDetails } from '../../auth/auth.models';
import { CursoDto } from '../cursos.models';
import { CursosService } from '../cursos.service';

@Component({
  imports: [RouterLink],
  selector: 'app-certificados-curso',
  templateUrl: './certificados-curso.html',
})
export class CertificadosCurso {
  private readonly route = inject(ActivatedRoute);
  private readonly cursosService = inject(CursosService);

  protected readonly cursoId = this.route.snapshot.paramMap.get('cursoId') ?? '';

  protected readonly curso = signal<CursoDto | null>(null);
  protected readonly mensagemSucesso = signal('');
  protected readonly mensagemErro = signal('');
  protected readonly carregando = signal(true);

  constructor() {
    this.cursosService.obterPorId(this.cursoId).subscribe({
      next: (curso: CursoDto) => {
        this.curso.set(curso);
        this.carregando.set(false);
      },
      error: (resposta: HttpErrorResponse) => {
        this.mensagemErro.set(
          this.obterMensagemErroApi(resposta, 'Não foi possível carregar os dados do curso.'),
        );
        this.carregando.set(false);
      },
    });
  }

  private obterMensagemErroApi(resposta: HttpErrorResponse, mensagemPadrao: string): string {
    const problem = resposta.error as ApiValidationProblemDetails | null;

    if (problem?.errors) {
      const mensagens = Object.values(problem.errors).flat();

      if (mensagens.length > 0) return mensagens.join('\n');
    }

    return problem?.detail ?? mensagemPadrao;
  }
}
