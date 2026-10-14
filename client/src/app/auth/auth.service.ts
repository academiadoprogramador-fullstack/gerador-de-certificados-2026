import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Router } from '@angular/router';

import { CadastroRequest, UsuarioDto } from './auth.models';
import { URL_API } from './auth.tokens';

@Service()
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly urlApi = inject(URL_API);

  cadastrar(req: CadastroRequest): Observable<UsuarioDto> {
    return this.http.post<UsuarioDto>(`${this.urlApi}/auth/cadastro`, req);
  }
}
