import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslocoModule } from '@ngneat/transloco';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { NotificationService } from '@core/services/notification.service';

@Component({
  selector: 'app-newsletter',
  standalone: true,
  imports: [
    CommonModule,
    TranslocoModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatIconModule
  ],
  template: `
    <section class="py-16 bg-gradient-premium text-white">
      <div class="container mx-auto px-4">
        <div class="max-w-2xl mx-auto text-center">
          <mat-icon class="text-6xl mb-6 text-primary-gold">mail_outline</mat-icon>
          
          <h2 class="text-3xl md:text-4xl font-heading font-bold mb-4">
            {{ 'home.newsletter.title' | transloco }}
          </h2>
          
          <p class="text-lg mb-8 opacity-90">
            {{ 'home.newsletter.subtitle' | transloco }}
          </p>
          
          <form [formGroup]="newsletterForm" (ngSubmit)="onSubmit()" class="flex flex-col md:flex-row gap-4 max-w-md mx-auto">
            <mat-form-field appearance="outline" class="flex-1">
              <mat-label>{{ 'home.newsletter.placeholder' | transloco }}</mat-label>
              <input 
                matInput 
                type="email" 
                formControlName="email"
                [placeholder]="'home.newsletter.placeholder' | transloco">
              <mat-icon matSuffix>email</mat-icon>
              @if (newsletterForm.get('email')?.hasError('email') && newsletterForm.get('email')?.touched) {
                <mat-error>{{ 'validation.email' | transloco }}</mat-error>
              }
            </mat-form-field>
            
            <button 
              type="submit" 
              mat-raised-button 
              class="btn-premium h-14"
              [disabled]="!newsletterForm.valid || isSubmitting">
              @if (isSubmitting) {
                <mat-icon class="animate-spin">refresh</mat-icon>
              } @else {
                {{ 'home.newsletter.button' | transloco }}
              }
            </button>
          </form>
          
          <p class="text-sm mt-6 opacity-75">
            En vous inscrivant, vous acceptez de recevoir nos communications marketing.
          </p>
        </div>
      </div>
    </section>
  `,
  styles: [`
    :host ::ng-deep .mat-mdc-form-field-wrapper {
      margin-bottom: 0;
    }
    
    :host ::ng-deep .mat-mdc-text-field-wrapper {
      background-color: rgba(255, 255, 255, 0.1);
    }
    
    :host ::ng-deep .mat-mdc-form-field-flex {
      background-color: transparent;
    }
    
    :host ::ng-deep .mdc-notched-outline__leading,
    :host ::ng-deep .mdc-notched-outline__notch,
    :host ::ng-deep .mdc-notched-outline__trailing {
      border-color: rgba(255, 255, 255, 0.3) !important;
    }
    
    :host ::ng-deep .mat-mdc-form-field-focus-overlay {
      background-color: transparent;
    }
    
    :host ::ng-deep input,
    :host ::ng-deep .mat-mdc-form-field-label,
    :host ::ng-deep .mat-mdc-form-field-icon-suffix {
      color: white !important;
    }
    
    @keyframes spin {
      to { transform: rotate(360deg); }
    }
    
    .animate-spin {
      animation: spin 1s linear infinite;
    }
  `]
})
export class NewsletterComponent {
  private fb = inject(FormBuilder);
  private notificationService = inject(NotificationService);
  
  isSubmitting = false;
  
  newsletterForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  onSubmit(): void {
    if (this.newsletterForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;
      
      // Simulate API call
      setTimeout(() => {
        this.notificationService.successTranslated('home.newsletter.success');
        this.newsletterForm.reset();
        this.isSubmitting = false;
      }, 1500);
    }
  }
}