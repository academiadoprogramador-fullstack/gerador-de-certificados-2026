import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { form, FormField, maxLength, min, required } from '@angular/forms/signals';
import { Router, RouterLink } from '@angular/router';
import { LucideCirclePlus } from '@lucide/angular';

import { ApiValidationProblemDetails } from '../../auth/auth.models';
import { CriarCursoRequest } from '../cursos.models';
import { CursosService } from '../cursos.service';

export interface CursoFormModel {
  readonly nome: string;
  readonly cargaHoraria: number | null;
  readonly dataConclusao: string;
}

@Component({
  imports: [LucideCirclePlus, FormField, RouterLink],
  selector: 'app-cadastrar-curso',
  templateUrl: './cadastrar-curso.html',
})
export class CadastrarCurso {
  private readonly cursosService = inject(CursosService);
  private readonly router = inject(Router);

  protected readonly enviandoFormulario = signal(false);
  protected readonly mensagemErro = signal('');

  protected readonly modeloCurso = signal<CursoFormModel>({
    nome: '',
    cargaHoraria: null,
    dataConclusao: '',
  });
  protected readonly formularioCurso = form(this.modeloCurso, (modelo) => {
    required(modelo.nome, { message: 'Informe o nome do curso.' });
    maxLength(modelo.nome, 200, { message: 'O nome pode ter no máximo 200 caracteres.' });
    required(modelo.cargaHoraria, { message: 'Informe a carga horária.' });
    min(modelo.cargaHoraria, 1, { message: 'A carga horária deve ser maior que zero.' });
    required(modelo.dataConclusao, { message: 'Informe a data de conclusão.' });
  });

  protected cadastrarCurso(event: SubmitEvent): void {
    event.preventDefault();

    const formulario = this.formularioCurso();

    if (formulario.invalid()) {
      formulario.markAsTouched();
      return;
    }

    const modelo = this.modeloCurso();

    const requisicao: CriarCursoRequest = {
      nome: modelo.nome.trim(),
      cargaHoraria: modelo.cargaHoraria!,
      dataConclusao: modelo.dataConclusao,
    };

    this.enviandoFormulario.set(true);
    this.mensagemErro.set('');

    this.cursosService.cadastrar(requisicao).subscribe({
      next: () => {
        this.enviandoFormulario.set(false);

        void this.router.navigateByUrl('/cursos/listar');
      },
      error: (resposta: HttpErrorResponse) => {
        this.mensagemErro.set(this.obterMensagemErroApi(resposta));
        this.enviandoFormulario.set(false);
      },
    });
  }

  private obterMensagemErroApi(resposta: HttpErrorResponse): string {
    const problem = resposta.error as ApiValidationProblemDetails | null;

    if (problem?.errors) {
      const mensagens = Object.values(problem.errors).flat();

      if (mensagens.length > 0) return mensagens.join('\n');
    }

    return problem?.detail ?? 'Não foi possível cadastrar o curso. Tente novamente em instantes.';
  }
}
