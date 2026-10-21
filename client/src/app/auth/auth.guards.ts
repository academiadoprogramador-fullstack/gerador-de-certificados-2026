import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';

import { AuthService } from './auth.service';

export const isAnonymousGuard: CanActivateFn = (): true | UrlTree => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.sessao()) return true;

  return router.createUrlTree(['/']);
};

export const isAuthenticatedGuard: CanActivateFn = (): true | UrlTree => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.sessao()) return true;

  return router.createUrlTree(['/auth/login']);
};
