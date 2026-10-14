export interface CadastroRequest {
  readonly email: string;
  readonly senha: string;
}

export interface CadastroConcluidoNavigationState {
  readonly emailCadastrado?: string;
}

export interface UsuarioDto {
  readonly id: string;
  readonly email: string;
}

export interface LoginRequest {
  readonly email: string;
  readonly senha: string;
}

export interface TokenDto {
  readonly usuarioId: string;
  readonly accessToken: string;
  readonly dataExpiracaoEmUtc: string;
}

export interface SessaoAutenticada {
  readonly token: TokenDto;
  readonly usuario: UsuarioDto;
}
