import { Routes } from '@angular/router';

export const routes: Routes = [
  // Redirection par défaut vers la page d'accueil
  {
    path: '',
    redirectTo: '/home',
    pathMatch: 'full'
  },
  
  // Page d'accueil publique
  {
    path: 'home',
    loadComponent: () => 
      import('./features/public/pages/homepage/homepage.component').then(m => m.HomepageComponent),
    title: 'Empty Legs - Accueil'
  },
  
  // Pages publiques - Recherche de vols
  {
    path: 'search',
    loadComponent: () =>
      import('./features/search/pages/flight-search/flight-search.component').then(m => m.FlightSearchComponent),
    title: 'Rechercher un vol - Empty Legs'
  },
  
  // Détails d'un vol
  {
    path: 'flights/:id',
    loadComponent: () =>
      import('./features/search/pages/flight-details/flight-details.component').then(m => m.FlightDetailsComponent),
    title: 'Détails du vol - Empty Legs'
  },
  
  // Pages d'authentification
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () =>
          import('./features/auth/pages/login/login.component').then(m => m.LoginComponent),
        title: 'Connexion - Empty Legs'
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./features/auth/pages/register/register.component').then(m => m.RegisterComponent),
        title: 'Inscription - Empty Legs'
      },
      {
        path: 'forgot-password',
        loadComponent: () =>
          import('./features/auth/pages/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent),
        title: 'Mot de passe oublié - Empty Legs'
      }
    ]
  },
  
  // Dashboard client (protégé)
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/customer/pages/dashboard/customer-dashboard.component').then(m => m.CustomerDashboardComponent),
    title: 'Mon tableau de bord - Empty Legs'
    // canActivate: [AuthGuard] // À ajouter plus tard
  },
  
  // Réservations client
  {
    path: 'bookings',
    loadComponent: () =>
      import('./features/customer/pages/bookings/my-bookings.component').then(m => m.MyBookingsComponent),
    title: 'Mes réservations - Empty Legs'
    // canActivate: [AuthGuard]
  },
  
  // Profil client
  {
    path: 'profile',
    loadComponent: () =>
      import('./features/customer/pages/profile/profile.component').then(m => m.ProfileComponent),
    title: 'Mon profil - Empty Legs'
    // canActivate: [AuthGuard]
  },
  
  // Programme de fidélité
  {
    path: 'loyalty',
    loadComponent: () =>
      import('./features/customer/pages/loyalty/loyalty.component').then(m => m.LoyaltyComponent),
    title: 'Programme de fidélité - Empty Legs'
    // canActivate: [AuthGuard]
  },
  
  // Dashboard compagnie (protégé)
  {
    path: 'company',
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/company/pages/dashboard/company-dashboard.component').then(m => m.CompanyDashboardComponent),
        title: 'Tableau de bord compagnie - Empty Legs'
      },
      {
        path: 'fleet',
        loadComponent: () =>
          import('./features/company/pages/fleet/fleet-management.component').then(m => m.FleetManagementComponent),
        title: 'Gestion de flotte - Empty Legs'
      },
      {
        path: 'flights',
        loadComponent: () =>
          import('./features/company/pages/flights/flight-management.component').then(m => m.FlightManagementComponent),
        title: 'Gestion des vols - Empty Legs'
      },
      {
        path: 'analytics',
        loadComponent: () =>
          import('./features/company/pages/analytics/analytics.component').then(m => m.AnalyticsComponent),
        title: 'Analyses - Empty Legs'
      }
    ]
    // canActivate: [AuthGuard, CompanyGuard]
  },
  
  // Dashboard admin (protégé)
  {
    path: 'admin',
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/admin/pages/dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent),
        title: 'Administration - Empty Legs'
      },
      {
        path: 'users',
        loadComponent: () =>
          import('./features/admin/pages/users/user-management.component').then(m => m.UserManagementComponent),
        title: 'Gestion utilisateurs - Empty Legs'
      },
      {
        path: 'content',
        loadComponent: () =>
          import('./features/admin/pages/content/content-management.component').then(m => m.ContentManagementComponent),
        title: 'Gestion de contenu - Empty Legs'
      },
      {
        path: 'settings',
        loadComponent: () =>
          import('./features/admin/pages/settings/system-settings.component').then(m => m.SystemSettingsComponent),
        title: 'Paramètres système - Empty Legs'
      }
    ]
    // canActivate: [AuthGuard, AdminGuard]
  },
  
  // Page 404 - À la fin
  {
    path: '**',
    loadComponent: () =>
      import('./shared/components/not-found/not-found.component').then(m => m.NotFoundComponent),
    title: 'Page non trouvée - Empty Legs'
  }
];