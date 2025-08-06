import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { TranslocoService } from '@ngneat/transloco';

export interface NotificationConfig extends MatSnackBarConfig {
  type?: 'success' | 'error' | 'warning' | 'info';
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private defaultConfig: MatSnackBarConfig = {
    duration: 4000,
    horizontalPosition: 'end',
    verticalPosition: 'top',
  };

  constructor(
    private snackBar: MatSnackBar,
    private transloco: TranslocoService
  ) {}

  success(message: string, config?: NotificationConfig): void {
    this.show(message, {
      ...config,
      type: 'success',
      panelClass: ['notification-success']
    });
  }

  error(message: string, config?: NotificationConfig): void {
    this.show(message, {
      ...config,
      type: 'error',
      duration: 6000, // Errors stay longer
      panelClass: ['notification-error']
    });
  }

  warning(message: string, config?: NotificationConfig): void {
    this.show(message, {
      ...config,
      type: 'warning',
      panelClass: ['notification-warning']
    });
  }

  info(message: string, config?: NotificationConfig): void {
    this.show(message, {
      ...config,
      type: 'info',
      panelClass: ['notification-info']
    });
  }

  showTranslated(key: string, params?: any, config?: NotificationConfig): void {
    const message = this.transloco.translate(key, params);
    this.show(message, config);
  }

  successTranslated(key: string, params?: any, config?: NotificationConfig): void {
    const message = this.transloco.translate(key, params);
    this.success(message, config);
  }

  errorTranslated(key: string, params?: any, config?: NotificationConfig): void {
    const message = this.transloco.translate(key, params);
    this.error(message, config);
  }

  private show(message: string, config?: NotificationConfig): void {
    const finalConfig = { ...this.defaultConfig, ...config };
    const action = config?.duration === 0 ? this.transloco.translate('common.close') : undefined;
    
    this.snackBar.open(message, action, finalConfig);
  }
}