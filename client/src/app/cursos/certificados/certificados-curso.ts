import { filter } from 'rxjs';

import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { form, FormField, required } from '@angular/forms/signals';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { ApiValidationProblemDetails } from '../../auth/auth.models';
import { CursoDto } from '../cursos.models';
import { CursosService } from '../cursos.service';
import { SolicitarCertificadosRequest } from './certificados.models';
import { CertificadosService } from './certificados.service';

interface CertificadosFormModel {
  readonly nomesAlunos: string;
}

@Component({
  imports: [FormField, RouterLink],
  selector: 'app-certificados-curso',
  templateUrl: './certificados-curso.html',
})
export class CertificadosCurso {
  private readonly route = inject(ActivatedRoute);
  private readonly cursosService = inject(CursosService);
  private readonly certificadosService = inject(CertificadosService);

  protected readonly cursoId = this.route.snapshot.paramMap.get('cursoId') ?? '';
  protected readonly curso = signal<CursoDto | null>(null);

  protected readonly mensagemSucesso = signal('');
  protected readonly mensagemErro = signal('');
  protected readonly carregando = signal(true);
  protected readonly enviando = signal(false);

  protected readonly modelo = signal<CertificadosFormModel>({ nomesAlunos: '' });
  protected readonly formulario = form(this.modelo, (modelo) => {
    required(modelo.nomesAlunos, { message: 'Informe ao menos um nome de aluno.' });
  });

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

  protected solicitarGeracao(event: SubmitEvent): void {
    event.preventDefault();

    const nomes = this.modelo()
      .nomesAlunos.split(/\r?\n/)
      .map((nome) => nome.trim())
      .filter(Boolean);

    if (nomes.length === 0) {
      this.formulario.nomesAlunos().markAsTouched();
      return;
    }

    const indiceNomeGrande = nomes.findIndex((n) => n.length > 200);

    if (indiceNomeGrande !== -1) {
      this.mensagemErro.set(
        `O nome do aluno na linha ${indiceNomeGrande + 1} excede o limite de 200 caracteres`,
      );
      return;
    }

    this.enviando.set(true);
    this.mensagemErro.set('');
    this.mensagemSucesso.set('');

    const req: SolicitarCertificadosRequest = {
      alunos: nomes.map((nome) => ({
        nome,
      })),
    };

    this.certificadosService.solicitar(this.cursoId, req).subscribe({
      next: (dto) => {
        this.enviando.set(false);
        this.formulario.nomesAlunos().reset('');
      },
      error: (resposta: HttpErrorResponse) => {
        this.enviando.set(false);
        this.mensagemErro.set(
          this.obterMensagemErroApi(
            resposta,
            'Não foi possível solicitar a geração dos certificados do curso.',
          ),
        );
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
