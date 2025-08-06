import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '@core/services/auth.service';

export const guestGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    const user = authService.getCurrentUser();
    
    // Redirect based on role
    switch (user?.role) {
      case 'Admin':
        router.navigate(['/admin/dashboard']);
        break;
      case 'Company':
        router.navigate(['/company/dashboard']);
        break;
      case 'Customer':
        router.navigate(['/customer/dashboard']);
        break;
      default:
        router.navigate(['/']);
    }
    
    return false;
  }

  return true;
};