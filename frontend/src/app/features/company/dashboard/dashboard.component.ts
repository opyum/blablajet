import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-company-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container mx-auto px-4 py-8">
      <h1 class="text-3xl font-bold mb-6">Company Dashboard</h1>
      <p>Welcome to your company dashboard!</p>
    </div>
  `
})
export class DashboardComponent {}