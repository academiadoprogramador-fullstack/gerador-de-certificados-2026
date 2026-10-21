import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, Routes } from '@angular/router';

import { environment } from '../environments/environment';
import { isAnonymousGuard, isAuthenticatedGuard } from './auth/auth.guards';
import { authInterceptor } from './auth/auth.interceptor';
import { CHAVE_ARMAZENAMENTO, URL_API } from './auth/auth.tokens';

const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./home/home').then((c) => c.Home),
  },
  {
    path: 'auth',
    canActivate: [isAnonymousGuard],
    canActivateChild: [isAnonymousGuard],
    loadComponent: () => import('./auth/shell/auth-shell').then((c) => c.AuthShell),
    children: [
      {
        path: 'cadastro',
        loadComponent: () => import('./auth/cadastro/cadastro').then((c) => c.Cadastro),
      },
      {
        path: 'login',
        loadComponent: () => import('./auth/login/login').then((c) => c.Login),
      },
    ],
  },
  {
    path: 'cursos',
    canActivate: [isAuthenticatedGuard],
    canActivateChild: [isAuthenticatedGuard],
    loadComponent: () => import('./cursos/shell/cursos-shell').then((c) => c.CursosShell),
    children: [
      { path: '', redirectTo: 'listar', pathMatch: 'full' },
      {
        path: 'listar',
        loadComponent: () => import('./cursos/listar/listar-cursos').then((c) => c.ListarCursos),
      },
      {
        path: ':cursoId/detalhes',
        loadComponent: () =>
          import('./cursos/detalhes/detalhes-curso').then((c) => c.DetalhesCurso),
      },
      {
        path: 'cadastrar',
        loadComponent: () =>
          import('./cursos/cadastrar/cadastrar-curso').then((c) => c.CadastrarCurso),
      },
      {
        path: ':cursoId/certificados',
        loadComponent: () =>
          import('./cursos/certificados/certificados-curso').then((c) => c.CertificadosCurso),
      },
    ],
  },
  {
    path: '**',
    redirectTo: '',
  },
];

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    {
      provide: URL_API,
      useValue: environment.urlApi,
    },
    {
      provide: CHAVE_ARMAZENAMENTO,
      useValue: 'certifica+.sessao',
    },
  ],
};
