import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslocoModule } from '@ngneat/transloco';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { NotificationService } from '@core/services/notification.service';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslocoModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule
  ],
  template: `
    <div class="min-h-screen bg-gray-50">
      <!-- Hero Section -->
      <section class="bg-gradient-premium text-white py-16">
        <div class="container mx-auto px-4 text-center">
          <h1 class="text-4xl md:text-5xl font-heading font-bold mb-4">
            {{ 'navigation.contact' | transloco }}
          </h1>
          <p class="text-xl opacity-90 max-w-2xl mx-auto">
            Notre équipe est à votre disposition pour répondre à toutes vos questions
          </p>
        </div>
      </section>

      <!-- Contact Content -->
      <section class="py-16">
        <div class="container mx-auto px-4">
          <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
            <!-- Contact Info -->
            <div class="lg:col-span-1">
              <!-- Phone -->
              <mat-card class="mb-6">
                <mat-card-content class="text-center py-6">
                  <mat-icon class="text-4xl text-primary-gold mb-4">phone</mat-icon>
                  <h3 class="text-xl font-semibold mb-2">Téléphone</h3>
                  <p class="text-gray-600">+33 1 23 45 67 89</p>
                  <p class="text-sm text-gray-500 mt-2">Lun-Ven: 9h-18h</p>
                </mat-card-content>
              </mat-card>

              <!-- Email -->
              <mat-card class="mb-6">
                <mat-card-content class="text-center py-6">
                  <mat-icon class="text-4xl text-primary-gold mb-4">email</mat-icon>
                  <h3 class="text-xl font-semibold mb-2">Email</h3>
                  <p class="text-gray-600">contact@empty-legs-luxury.com</p>
                  <p class="text-sm text-gray-500 mt-2">Réponse sous 24h</p>
                </mat-card-content>
              </mat-card>

              <!-- Address -->
              <mat-card>
                <mat-card-content class="text-center py-6">
                  <mat-icon class="text-4xl text-primary-gold mb-4">location_on</mat-icon>
                  <h3 class="text-xl font-semibold mb-2">Adresse</h3>
                  <p class="text-gray-600">
                    123 Avenue des Champs-Élysées<br>
                    75008 Paris, France
                  </p>
                </mat-card-content>
              </mat-card>
            </div>

            <!-- Contact Form -->
            <div class="lg:col-span-2">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Envoyez-nous un message</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <form [formGroup]="contactForm" (ngSubmit)="onSubmit()" class="mt-6">
                    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <!-- First Name -->
                      <mat-form-field appearance="outline" class="w-full">
                        <mat-label>{{ 'auth.register.firstName' | transloco }}</mat-label>
                        <input matInput formControlName="firstName">
                        <mat-icon matPrefix>person</mat-icon>
                        @if (contactForm.get('firstName')?.hasError('required') && contactForm.get('firstName')?.touched) {
                          <mat-error>{{ 'validation.required' | transloco }}</mat-error>
                        }
                      </mat-form-field>

                      <!-- Last Name -->
                      <mat-form-field appearance="outline" class="w-full">
                        <mat-label>{{ 'auth.register.lastName' | transloco }}</mat-label>
                        <input matInput formControlName="lastName">
                        <mat-icon matPrefix>person</mat-icon>
                        @if (contactForm.get('lastName')?.hasError('required') && contactForm.get('lastName')?.touched) {
                          <mat-error>{{ 'validation.required' | transloco }}</mat-error>
                        }
                      </mat-form-field>
                    </div>

                    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <!-- Email -->
                      <mat-form-field appearance="outline" class="w-full">
                        <mat-label>{{ 'auth.login.email' | transloco }}</mat-label>
                        <input matInput type="email" formControlName="email">
                        <mat-icon matPrefix>email</mat-icon>
                        @if (contactForm.get('email')?.hasError('required') && contactForm.get('email')?.touched) {
                          <mat-error>{{ 'validation.required' | transloco }}</mat-error>
                        }
                        @if (contactForm.get('email')?.hasError('email') && contactForm.get('email')?.touched) {
                          <mat-error>{{ 'validation.email' | transloco }}</mat-error>
                        }
                      </mat-form-field>

                      <!-- Phone -->
                      <mat-form-field appearance="outline" class="w-full">
                        <mat-label>{{ 'booking.phone' | transloco }}</mat-label>
                        <input matInput formControlName="phone">
                        <mat-icon matPrefix>phone</mat-icon>
                      </mat-form-field>
                    </div>

                    <!-- Subject -->
                    <mat-form-field appearance="outline" class="w-full">
                      <mat-label>Sujet</mat-label>
                      <mat-select formControlName="subject">
                        <mat-option value="general">Question générale</mat-option>
                        <mat-option value="booking">Réservation</mat-option>
                        <mat-option value="partnership">Partenariat</mat-option>
                        <mat-option value="support">Support technique</mat-option>
                        <mat-option value="other">Autre</mat-option>
                      </mat-select>
                      <mat-icon matPrefix>subject</mat-icon>
                    </mat-form-field>

                    <!-- Message -->
                    <mat-form-field appearance="outline" class="w-full">
                      <mat-label>Message</mat-label>
                      <textarea matInput formControlName="message" rows="6"></textarea>
                      <mat-icon matPrefix>message</mat-icon>
                      @if (contactForm.get('message')?.hasError('required') && contactForm.get('message')?.touched) {
                        <mat-error>{{ 'validation.required' | transloco }}</mat-error>
                      }
                    </mat-form-field>

                    <div class="flex justify-end mt-6">
                      <button 
                        type="submit" 
                        mat-raised-button 
                        class="btn-premium"
                        [disabled]="!contactForm.valid || isSubmitting">
                        @if (isSubmitting) {
                          <mat-icon class="animate-spin mr-2">refresh</mat-icon>
                          Envoi en cours...
                        } @else {
                          <mat-icon class="mr-2">send</mat-icon>
                          Envoyer le message
                        }
                      </button>
                    </div>
                  </form>
                </mat-card-content>
              </mat-card>
            </div>
          </div>
        </div>
      </section>

      <!-- Map Section -->
      <section class="h-96">
        <iframe 
          src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d2624.2216451856!2d2.2944813!3d48.8698679!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x47e66fec70fb1d8f%3A0xd9b5676e112e643d!2sAv.%20des%20Champs-%C3%89lys%C3%A9es%2C%2075008%20Paris!5e0!3m2!1sfr!2sfr!4v1234567890"
          width="100%" 
          height="100%" 
          style="border:0;" 
          allowfullscreen="" 
          loading="lazy" 
          referrerpolicy="no-referrer-when-downgrade">
        </iframe>
      </section>
    </div>
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
export class ContactComponent {
  private fb = inject(FormBuilder);
  private notificationService = inject(NotificationService);
  
  isSubmitting = false;
  
  contactForm = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phone: [''],
    subject: ['general', Validators.required],
    message: ['', Validators.required]
  });

  onSubmit(): void {
    if (this.contactForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;
      
      // Simulate API call
      setTimeout(() => {
        this.notificationService.success('Votre message a été envoyé avec succès !');
        this.contactForm.reset({ subject: 'general' });
        this.isSubmitting = false;
      }, 2000);
    }
  }
}