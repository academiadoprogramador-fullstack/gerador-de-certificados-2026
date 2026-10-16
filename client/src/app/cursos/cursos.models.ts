export interface CursoDto {
  readonly id: string;
  readonly nome: string;
  readonly cargaHoraria: number;
  readonly dataConclusao: string;
}

export interface CriarCursoRequest {
  readonly nome: string;
  readonly cargaHoraria: number;
  readonly dataConclusao: string;
}
