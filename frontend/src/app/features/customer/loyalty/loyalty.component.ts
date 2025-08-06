import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-customer-loyalty',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container mx-auto px-4 py-8">
      <h1 class="text-3xl font-bold mb-6">Loyalty Program</h1>
      <p>Your loyalty program details will appear here.</p>
    </div>
  `
})
export class LoyaltyComponent {}