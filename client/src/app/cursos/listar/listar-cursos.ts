import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { LucideBookOpen } from '@lucide/angular';

import { ApiValidationProblemDetails } from '../../auth/auth.models';
import { CursosService } from '../cursos.service';

@Component({
  imports: [LucideBookOpen, RouterLink],
  selector: 'app-listar-cursos',
  templateUrl: './listar-cursos.html',
})
export class ListarCursos {
  private readonly cursosService = inject(CursosService);

  protected readonly cursos = toSignal(this.cursosService.listar());
  protected readonly mensagemErro = signal('');

  private obterMensagemErro(response: HttpErrorResponse): string {
    const problem = response.error as ApiValidationProblemDetails | null;

    return problem?.detail ?? 'Não foi possível carregar os cursos. Tente novamente em instantes.';
  }
}
