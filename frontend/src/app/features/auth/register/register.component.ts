import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatRadioModule } from '@angular/material/radio';
import { MatStepperModule } from '@angular/material/stepper';
import { AuthService } from '@core/services/auth.service';
import { NotificationService } from '@core/services/notification.service';
import { finalize } from 'rxjs/operators';

// Custom validator for password match
export function passwordMatchValidator(control: AbstractControl): {[key: string]: boolean} | null {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');
  
  if (!password || !confirmPassword) {
    return null;
  }
  
  return password.value === confirmPassword.value ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    TranslocoModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatRadioModule,
    MatStepperModule
  ],
  template: `
    <div class="text-center mb-8">
      <h1 class="text-3xl font-heading font-bold text-primary-navy mb-2">
        {{ 'auth.register.title' | transloco }}
      </h1>
      <p class="text-gray-600">
        {{ 'auth.register.subtitle' | transloco }}
      </p>
    </div>

    <mat-stepper linear #stepper>
      <!-- Step 1: Account Type -->
      <mat-step [stepControl]="accountTypeForm">
        <ng-template matStepLabel>{{ 'auth.register.userType' | transloco }}</ng-template>
        
        <form [formGroup]="accountTypeForm">
          <div class="py-4">
            <mat-radio-group formControlName="userType" class="flex flex-col gap-4">
              <mat-radio-button value="Customer" class="border rounded-lg p-4 hover:bg-gray-50">
                <div class="flex items-center gap-4">
                  <mat-icon class="text-primary-gold">person</mat-icon>
                  <div>
                    <p class="font-semibold">{{ 'auth.register.customer' | transloco }}</p>
                    <p class="text-sm text-gray-600">Réservez des vols empty leg</p>
                  </div>
                </div>
              </mat-radio-button>
              
              <mat-radio-button value="Company" class="border rounded-lg p-4 hover:bg-gray-50">
                <div class="flex items-center gap-4">
                  <mat-icon class="text-primary-gold">business</mat-icon>
                  <div>
                    <p class="font-semibold">{{ 'auth.register.company' | transloco }}</p>
                    <p class="text-sm text-gray-600">Proposez vos vols empty leg</p>
                  </div>
                </div>
              </mat-radio-button>
            </mat-radio-group>
          </div>
          
          <div class="flex justify-end mt-6">
            <button mat-raised-button matStepperNext class="btn-premium" [disabled]="!accountTypeForm.valid">
              {{ 'common.next' | transloco }}
            </button>
          </div>
        </form>
      </mat-step>

      <!-- Step 2: Personal Information -->
      <mat-step [stepControl]="personalInfoForm">
        <ng-template matStepLabel>Informations personnelles</ng-template>
        
        <form [formGroup]="personalInfoForm">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <!-- First Name -->
            <mat-form-field appearance="outline" class="w-full">
              <mat-label>{{ 'auth.register.firstName' | transloco }}</mat-label>
              <input matInput formControlName="firstName">
              <mat-icon matPrefix>person</mat-icon>
              @if (personalInfoForm.get('firstName')?.hasError('required') && personalInfoForm.get('firstName')?.touched) {
                <mat-error>{{ 'validation.required' | transloco }}</mat-error>
              }
            </mat-form-field>

            <!-- Last Name -->
            <mat-form-field appearance="outline" class="w-full">
              <mat-label>{{ 'auth.register.lastName' | transloco }}</mat-label>
              <input matInput formControlName="lastName">
              <mat-icon matPrefix>person</mat-icon>
              @if (personalInfoForm.get('lastName')?.hasError('required') && personalInfoForm.get('lastName')?.touched) {
                <mat-error>{{ 'validation.required' | transloco }}</mat-error>
              }
            </mat-form-field>
          </div>

          <!-- Company Name (if company) -->
          @if (accountTypeForm.get('userType')?.value === 'Company') {
            <mat-form-field appearance="outline" class="w-full">
              <mat-label>{{ 'auth.register.companyName' | transloco }}</mat-label>
              <input matInput formControlName="companyName">
              <mat-icon matPrefix>business</mat-icon>
              @if (personalInfoForm.get('companyName')?.hasError('required') && personalInfoForm.get('companyName')?.touched) {
                <mat-error>{{ 'validation.required' | transloco }}</mat-error>
              }
            </mat-form-field>
          }

          <!-- Email -->
          <mat-form-field appearance="outline" class="w-full">
            <mat-label>{{ 'auth.register.email' | transloco }}</mat-label>
            <input matInput type="email" formControlName="email">
            <mat-icon matPrefix>email</mat-icon>
            @if (personalInfoForm.get('email')?.hasError('required') && personalInfoForm.get('email')?.touched) {
              <mat-error>{{ 'validation.required' | transloco }}</mat-error>
            }
            @if (personalInfoForm.get('email')?.hasError('email') && personalInfoForm.get('email')?.touched) {
              <mat-error>{{ 'validation.email' | transloco }}</mat-error>
            }
          </mat-form-field>

          <!-- Phone -->
          <mat-form-field appearance="outline" class="w-full">
            <mat-label>{{ 'auth.register.phone' | transloco }}</mat-label>
            <input matInput formControlName="phoneNumber">
            <mat-icon matPrefix>phone</mat-icon>
          </mat-form-field>

          <div class="flex justify-between mt-6">
            <button mat-button matStepperPrevious>
              {{ 'common.back' | transloco }}
            </button>
            <button mat-raised-button matStepperNext class="btn-premium" [disabled]="!personalInfoForm.valid">
              {{ 'common.next' | transloco }}
            </button>
          </div>
        </form>
      </mat-step>

      <!-- Step 3: Password -->
      <mat-step [stepControl]="passwordForm">
        <ng-template matStepLabel>Mot de passe</ng-template>
        
        <form [formGroup]="passwordForm">
          <!-- Password -->
          <mat-form-field appearance="outline" class="w-full">
            <mat-label>{{ 'auth.register.password' | transloco }}</mat-label>
            <input 
              matInput 
              [type]="showPassword ? 'text' : 'password'" 
              formControlName="password">
            <mat-icon matPrefix>lock</mat-icon>
            <button 
              mat-icon-button 
              matSuffix 
              type="button"
              (click)="showPassword = !showPassword">
              <mat-icon>{{ showPassword ? 'visibility_off' : 'visibility' }}</mat-icon>
            </button>
            @if (passwordForm.get('password')?.hasError('required') && passwordForm.get('password')?.touched) {
              <mat-error>{{ 'validation.required' | transloco }}</mat-error>
            }
            @if (passwordForm.get('password')?.hasError('minlength') && passwordForm.get('password')?.touched) {
              <mat-error>{{ 'validation.minLength' | transloco : { min: 8 } }}</mat-error>
            }
          </mat-form-field>

          <!-- Confirm Password -->
          <mat-form-field appearance="outline" class="w-full">
            <mat-label>{{ 'auth.register.confirmPassword' | transloco }}</mat-label>
            <input 
              matInput 
              [type]="showPassword ? 'text' : 'password'" 
              formControlName="confirmPassword">
            <mat-icon matPrefix>lock</mat-icon>
            @if (passwordForm.get('confirmPassword')?.hasError('required') && passwordForm.get('confirmPassword')?.touched) {
              <mat-error>{{ 'validation.required' | transloco }}</mat-error>
            }
            @if (passwordForm.hasError('passwordMismatch') && passwordForm.get('confirmPassword')?.touched) {
              <mat-error>{{ 'validation.passwordMatch' | transloco }}</mat-error>
            }
          </mat-form-field>

          <div class="flex justify-between mt-6">
            <button mat-button matStepperPrevious>
              {{ 'common.back' | transloco }}
            </button>
            <button 
              mat-raised-button 
              class="btn-premium" 
              [disabled]="!isFormValid || isLoading"
              (click)="onSubmit()">
              @if (isLoading) {
                <ng-container>
                  <mat-icon class="animate-spin mr-2">refresh</mat-icon>
                  Inscription en cours...
                </ng-container>
              } @else {
                {{ 'auth.register.submit' | transloco }}
              }
            </button>
          </div>
        </form>
      </mat-step>
    </mat-stepper>

    <!-- Sign In Link -->
    <p class="text-center text-sm text-gray-600 mt-6">
      {{ 'auth.register.hasAccount' | transloco }}
      <a 
        routerLink="/auth/login" 
        class="text-primary-gold font-semibold hover:underline">
        {{ 'auth.register.signIn' | transloco }}
      </a>
    </p>
  `,
  styles: [`
    @keyframes spin {
      to { transform: rotate(360deg); }
    }
    
    .animate-spin {
      animation: spin 1s linear infinite;
    }
  `]
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);
  
  isLoading = false;
  showPassword = false;
  
  accountTypeForm = this.fb.group({
    userType: ['Customer', Validators.required]
  });
  
  personalInfoForm = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    companyName: [''],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['']
  });

  constructor() {
    // Update company name validation based on user type
    this.accountTypeForm.get('userType')?.valueChanges.subscribe(userType => {
      const companyNameControl = this.personalInfoForm.get('companyName');
      if (userType === 'Company') {
        companyNameControl?.setValidators(Validators.required);
      } else {
        companyNameControl?.clearValidators();
      }
      companyNameControl?.updateValueAndValidity();
    });
  }
  
  passwordForm = this.fb.group({
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', Validators.required]
  }, { validators: passwordMatchValidator });

  get isFormValid(): boolean {
    return this.accountTypeForm.valid && this.personalInfoForm.valid && this.passwordForm.valid;
  }

  onSubmit(): void {
    if (this.isFormValid && !this.isLoading) {
      this.isLoading = true;
      
      const registerData = {
        ...this.personalInfoForm.value,
        ...this.passwordForm.value,
        userType: this.accountTypeForm.value.userType
      };
      
      // Remove confirmPassword from the data
      delete (registerData as any).confirmPassword;
      
      this.authService.register(registerData as any)
        .pipe(finalize(() => this.isLoading = false))
        .subscribe({
          next: () => {
            this.notificationService.successTranslated('auth.register.registerSuccess');
            this.router.navigate(['/']);
          },
          error: (error) => {
            console.error('Registration error:', error);
            this.notificationService.errorTranslated('auth.register.registerError');
          }
        });
    }
  }
}