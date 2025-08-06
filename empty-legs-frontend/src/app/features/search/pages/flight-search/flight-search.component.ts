import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-flight-search',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page-container">
      <h1>Recherche de vols</h1>
      <p>Page en cours de développement...</p>
    </div>
  `,
  styles: [`
    .page-container {
      padding: 2rem;
      text-align: center;
    }
  `]
})
export class FlightSearchComponent {}