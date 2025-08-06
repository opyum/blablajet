import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-homepage',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    TranslateModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './homepage.component.html',
  styleUrls: ['./homepage.component.scss']
})
export class HomepageComponent {
  
  features = [
    {
      icon: 'flight',
      titleKey: 'homepage.features.luxury.title',
      descriptionKey: 'homepage.features.luxury.description'
    },
    {
      icon: 'star',
      titleKey: 'homepage.features.comfort.title',
      descriptionKey: 'homepage.features.comfort.description'
    },
    {
      icon: 'schedule',
      titleKey: 'homepage.features.flexibility.title',
      descriptionKey: 'homepage.features.flexibility.description'
    }
  ];

  steps = [
    {
      step: '01',
      titleKey: 'homepage.howItWorks.step1.title',
      descriptionKey: 'homepage.howItWorks.step1.description',
      icon: 'search'
    },
    {
      step: '02',
      titleKey: 'homepage.howItWorks.step2.title',
      descriptionKey: 'homepage.howItWorks.step2.description',
      icon: 'book'
    },
    {
      step: '03',
      titleKey: 'homepage.howItWorks.step3.title',
      descriptionKey: 'homepage.howItWorks.step3.description',
      icon: 'flight_takeoff'
    }
  ];

  constructor() {}

  onSearchFlights(): void {
    // Navigation vers la page de recherche
    console.log('Navigate to search page');
  }
}