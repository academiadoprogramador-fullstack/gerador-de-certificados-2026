import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';

import { URL_API } from '../../auth/auth.tokens';
import { SolicitacaoCertificadoDto, SolicitarCertificadosRequest } from './certificados.models';

@Service()
export class CertificadosService {
  private readonly http = inject(HttpClient);
  private readonly urlApi = inject(URL_API);

  solicitar(
    cursoId: string,
    req: SolicitarCertificadosRequest,
  ): Observable<SolicitacaoCertificadoDto> {
    return this.http.post<SolicitacaoCertificadoDto>(
      `${this.urlApi}/cursos/${cursoId}/certificados`,
      req,
    );
  }
}
