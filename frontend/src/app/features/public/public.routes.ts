import { Routes } from '@angular/router';
import { PublicLayoutComponent } from '@app/layout/public-layout/public-layout.component';

export const PUBLIC_ROUTES: Routes = [
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      {
        path: '',
        loadComponent: () => import('./home/home.component').then(m => m.HomeComponent)
      },
      {
        path: 'flights',
        loadComponent: () => import('./flight-search/flight-search.component').then(m => m.FlightSearchComponent)
      },
      {
        path: 'flights/:id',
        loadComponent: () => import('./flight-details/flight-details.component').then(m => m.FlightDetailsComponent)
      },
      {
        path: 'about',
        loadComponent: () => import('./about/about.component').then(m => m.AboutComponent)
      },
      {
        path: 'contact',
        loadComponent: () => import('./contact/contact.component').then(m => m.ContactComponent)
      }
    ]
  }
];