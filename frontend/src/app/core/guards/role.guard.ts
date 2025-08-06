import { inject } from '@angular/core';
import { Router, CanActivateFn, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '@core/services/auth.service';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const requiredRoles = route.data['roles'] as string[];
  
  if (!requiredRoles || requiredRoles.length === 0) {
    return true;
  }

  const user = authService.getCurrentUser();
  
  if (!user) {
    router.navigate(['/auth/login']);
    return false;
  }

  const hasRequiredRole = requiredRoles.includes(user.role);
  
  if (!hasRequiredRole) {
    router.navigate(['/']);
    return false;
  }

  return true;
};