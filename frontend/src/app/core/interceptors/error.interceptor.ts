import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { NotificationService } from '@core/services/notification.service';
import { AuthService } from '@core/services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> => {
  const router = inject(Router);
  const notificationService = inject(NotificationService);
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      console.error('HTTP Error:', error);
      
      if (error.status === 401) {
        // Unauthorized - redirect to login
        authService.logout();
        notificationService.errorTranslated('auth.login.loginError');
      } else if (error.status === 403) {
        // Forbidden
        notificationService.errorTranslated('common.error');
        router.navigate(['/']);
      } else if (error.status === 404) {
        // Not found
        notificationService.errorTranslated('common.error');
      } else if (error.status === 422) {
        // Validation error
        if (error.error && error.error.errors) {
          const firstError = Object.values(error.error.errors)[0];
          if (Array.isArray(firstError) && firstError.length > 0) {
            notificationService.error(firstError[0]);
          } else {
            notificationService.errorTranslated('validation.error');
          }
        }
      } else if (error.status >= 500) {
        // Server error
        notificationService.errorTranslated('common.error');
      } else if (error.status === 0) {
        // Network error
        notificationService.error('Network error. Please check your connection.');
      }
      
      return throwError(() => error);
    })
  );
};