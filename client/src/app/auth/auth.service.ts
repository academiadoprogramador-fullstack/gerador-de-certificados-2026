import { Observable, tap } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { inject, Service, signal } from '@angular/core';
import { Router } from '@angular/router';

import {
  CadastroRequest,
  LoginRequest,
  SessaoAutenticada,
  TokenDto,
  UsuarioDto,
} from './auth.models';
import { URL_API } from './auth.tokens';

@Service()
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly urlApi = inject(URL_API);

  private readonly sessaoState = signal<SessaoAutenticada | null>(null);
  readonly sessao = this.sessaoState.asReadonly();

  cadastrar(req: CadastroRequest): Observable<UsuarioDto> {
    return this.http.post<UsuarioDto>(`${this.urlApi}/auth/cadastro`, req);
  }

  login(req: LoginRequest): Observable<TokenDto> {
    return this.http
      .post<TokenDto>(`${this.urlApi}/auth/login`, req)
      .pipe(tap((token) => this.salvarSessao(token, req.email))); // tap é utilizado para gerar efeitos colaterais
  }

  encerrarSessao(): void {
    this.sessaoState.set(null);

    void this.router.navigateByUrl('/login');
  }

  private salvarSessao(token: TokenDto, email: string): void {
    const usuario: UsuarioDto = { id: token.usuarioId, email: email };

    const sessao: SessaoAutenticada = {
      token,
      usuario,
    };

    this.sessaoState.set(sessao);
  }
}
