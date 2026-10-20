export interface AlunoCertificadoRequest {
  readonly nome: string;
}

export interface SolicitarCertificadosRequest {
  readonly alunos: readonly AlunoCertificadoRequest[];
}

export interface SolicitacaoCertificadoDto {
  readonly cursoId: string;
  readonly totalAlunos: number;
  readonly status: string;
}
