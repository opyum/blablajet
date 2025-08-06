import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-loyalty',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page-container">
      <h1>Programme de Fidélité</h1>
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
export class LoyaltyComponent {}
