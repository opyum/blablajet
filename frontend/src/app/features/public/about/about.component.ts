import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslocoModule } from '@ngneat/transloco';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [CommonModule, TranslocoModule, MatCardModule, MatIconModule],
  template: `
    <div class="min-h-screen bg-gray-50">
      <!-- Hero Section -->
      <section class="bg-gradient-premium text-white py-16">
        <div class="container mx-auto px-4 text-center">
          <h1 class="text-4xl md:text-5xl font-heading font-bold mb-4">
            {{ 'footer.about.company' | transloco }}
          </h1>
          <p class="text-xl opacity-90 max-w-2xl mx-auto">
            Leader dans le domaine des vols empty leg de luxe
          </p>
        </div>
      </section>

      <!-- Mission Section -->
      <section class="py-16">
        <div class="container mx-auto px-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-12 items-center">
            <div>
              <h2 class="text-3xl font-heading font-bold text-primary-navy mb-6">
                Notre Mission
              </h2>
              <p class="text-gray-600 mb-4">
                Empty Legs Luxury révolutionne l'aviation privée en rendant le luxe accessible. 
                Notre plateforme connecte les voyageurs exigeants avec des opportunités de vol 
                exceptionnelles à des prix réduits.
              </p>
              <p class="text-gray-600 mb-4">
                Nous croyons que le voyage en jet privé ne devrait pas être réservé à une élite. 
                Grâce à notre réseau de partenaires de confiance, nous offrons des vols empty leg 
                qui permettent d'économiser jusqu'à 75% sur le prix habituel.
              </p>
              <p class="text-gray-600">
                Notre engagement envers l'excellence se reflète dans chaque aspect de notre service, 
                de la sélection rigoureuse de nos partenaires à l'accompagnement personnalisé de nos clients.
              </p>
            </div>
            <div>
              <img 
                src="/assets/images/about-mission.jpg" 
                alt="Notre mission"
                class="rounded-2xl shadow-xl"
              >
            </div>
          </div>
        </div>
      </section>

      <!-- Values Section -->
      <section class="py-16 bg-white">
        <div class="container mx-auto px-4">
          <h2 class="text-3xl font-heading font-bold text-primary-navy text-center mb-12">
            Nos Valeurs
          </h2>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
            <mat-card class="text-center">
              <mat-card-content class="py-8">
                <mat-icon class="text-5xl text-primary-gold mb-4">workspace_premium</mat-icon>
                <h3 class="text-xl font-semibold mb-3">Excellence</h3>
                <p class="text-gray-600">
                  Nous sélectionnons uniquement les meilleurs opérateurs et appareils 
                  pour garantir une expérience exceptionnelle.
                </p>
              </mat-card-content>
            </mat-card>

            <mat-card class="text-center">
              <mat-card-content class="py-8">
                <mat-icon class="text-5xl text-primary-gold mb-4">handshake</mat-icon>
                <h3 class="text-xl font-semibold mb-3">Confiance</h3>
                <p class="text-gray-600">
                  La transparence et l'intégrité sont au cœur de nos relations 
                  avec nos clients et partenaires.
                </p>
              </mat-card-content>
            </mat-card>

            <mat-card class="text-center">
              <mat-card-content class="py-8">
                <mat-icon class="text-5xl text-primary-gold mb-4">eco</mat-icon>
                <h3 class="text-xl font-semibold mb-3">Durabilité</h3>
                <p class="text-gray-600">
                  En optimisant l'utilisation des jets privés, nous contribuons 
                  à réduire l'empreinte carbone de l'aviation.
                </p>
              </mat-card-content>
            </mat-card>
          </div>
        </div>
      </section>

      <!-- Team Section -->
      <section class="py-16">
        <div class="container mx-auto px-4">
          <h2 class="text-3xl font-heading font-bold text-primary-navy text-center mb-12">
            Notre Équipe
          </h2>
          <div class="grid grid-cols-1 md:grid-cols-4 gap-8">
            @for (member of teamMembers; track member.name) {
              <div class="text-center">
                <img 
                  [src]="member.photo" 
                  [alt]="member.name"
                  class="w-32 h-32 rounded-full mx-auto mb-4 object-cover"
                >
                <h3 class="font-semibold">{{ member.name }}</h3>
                <p class="text-sm text-gray-600">{{ member.role }}</p>
              </div>
            }
          </div>
        </div>
      </section>

      <!-- Stats Section -->
      <section class="py-16 bg-gradient-premium text-white">
        <div class="container mx-auto px-4">
          <div class="grid grid-cols-2 md:grid-cols-4 gap-8 text-center">
            <div>
              <p class="text-4xl font-bold text-primary-gold">500+</p>
              <p class="text-lg opacity-90">Vols disponibles</p>
            </div>
            <div>
              <p class="text-4xl font-bold text-primary-gold">10k+</p>
              <p class="text-lg opacity-90">Clients satisfaits</p>
            </div>
            <div>
              <p class="text-4xl font-bold text-primary-gold">50+</p>
              <p class="text-lg opacity-90">Compagnies partenaires</p>
            </div>
            <div>
              <p class="text-4xl font-bold text-primary-gold">75%</p>
              <p class="text-lg opacity-90">Économies moyennes</p>
            </div>
          </div>
        </div>
      </section>
    </div>
  `,
  styles: []
})
export class AboutComponent {
  teamMembers = [
    {
      name: 'Jean Dupont',
      role: 'CEO & Fondateur',
      photo: '/assets/images/team/ceo.jpg'
    },
    {
      name: 'Marie Laurent',
      role: 'Directrice des Opérations',
      photo: '/assets/images/team/coo.jpg'
    },
    {
      name: 'Pierre Martin',
      role: 'Directeur Commercial',
      photo: '/assets/images/team/cco.jpg'
    },
    {
      name: 'Sophie Bernard',
      role: 'Responsable Qualité',
      photo: '/assets/images/team/quality.jpg'
    }
  ];
}