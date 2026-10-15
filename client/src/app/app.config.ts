import { provideHttpClient } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, Routes } from '@angular/router';

import { environment } from '../environments/environment';
import { CHAVE_ARMAZENAMENTO, URL_API } from './auth/auth.tokens';

const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./home/home').then((c) => c.Home),
  },
  {
    path: 'cadastro',
    loadComponent: () => import('./auth/cadastro/cadastro').then((c) => c.Cadastro),
  },
  {
    path: 'login',
    loadComponent: () => import('./auth/login/login').then((c) => c.Login),
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
    provideHttpClient(),
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
