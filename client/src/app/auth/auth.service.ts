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
import { CHAVE_ARMAZENAMENTO, URL_API } from './auth.tokens';

interface SessaoPersistidaAnteriormente {
  readonly token?: TokenDto;
  readonly usuario?: UsuarioDto;
}

@Service()
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly urlApi = inject(URL_API);
  private readonly chaveArmazenamento = inject(CHAVE_ARMAZENAMENTO);

  private readonly sessaoState = signal<SessaoAutenticada | null>(this.lerSessao());
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
    localStorage.removeItem(this.chaveArmazenamento);
    this.sessaoState.set(null);

    void this.router.navigateByUrl('/login');
  }

  private salvarSessao(token: TokenDto, email: string): void {
    const usuario: UsuarioDto = { id: token.usuarioId, email: email };

    const sessao: SessaoAutenticada = {
      token,
      usuario,
    };

    localStorage.setItem(this.chaveArmazenamento, JSON.stringify(sessao));
    this.sessaoState.set(sessao);
  }

  private lerSessao(): SessaoAutenticada | null {
    try {
      const dadosArmazenados = localStorage.getItem(this.chaveArmazenamento);

      if (!dadosArmazenados) return null;

      const sessaoAnterior = JSON.parse(dadosArmazenados) as SessaoPersistidaAnteriormente;

      const token = sessaoAnterior.token;
      const usuario = sessaoAnterior.usuario;

      if (!token || !usuario) {
        localStorage.removeItem(this.chaveArmazenamento);
        return null;
      }

      const expiracao = new Date(token.dataExpiracaoEmUtc).getTime();

      if (
        !token.accessToken ||
        !usuario.email ||
        !Number.isFinite(expiracao) ||
        expiracao <= Date.now()
      ) {
        localStorage.removeItem(this.chaveArmazenamento);
        return null;
      }

      const sessao: SessaoAutenticada = { token, usuario };

      if (!sessaoAnterior.usuario) {
        localStorage.setItem(this.chaveArmazenamento, JSON.stringify(sessao));
      }

      return sessao;
    } catch {
      localStorage.removeItem(this.chaveArmazenamento);
      return null;
    }
  }
}
