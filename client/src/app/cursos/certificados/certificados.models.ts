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

export interface StatusProcessamentoDto {
  readonly cursoId: string;
  readonly status: string;
  readonly totalCertificados: number;
  readonly gerados: number;
  readonly falhas: number;
  readonly zipDisponivel: number;
  readonly iniciadoEm: string;
  readonly concluidoEm: string | null;
}

export interface CertificadoDto {
  readonly id: string;
  readonly nomeAluno: string;
  readonly statusGeracao: string;
  readonly geradoEm: string | null;
}
