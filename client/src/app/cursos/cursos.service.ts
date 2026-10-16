import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';

import { URL_API } from '../auth/auth.tokens';
import { CriarCursoRequest, CursoDto } from './cursos.models';

@Service()
export class CursosService {
  private readonly http = inject(HttpClient);
  private readonly urlApi = inject(URL_API);

  listar(): Observable<readonly CursoDto[]> {
    return this.http.get<readonly CursoDto[]>(`${this.urlApi}/cursos`);
  }

  cadastrar(requisicao: CriarCursoRequest): Observable<CursoDto> {
    return this.http.post<CursoDto>(`${this.urlApi}/cursos`, requisicao);
  }
}
