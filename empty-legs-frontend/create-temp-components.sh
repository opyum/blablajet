#!/bin/bash

# Function to create a temporary component
create_component() {
    local path=$1
    local name=$2
    local title=$3
    
    cat > "$path" << EOC
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-${name}',
  standalone: true,
  imports: [CommonModule],
  template: \`
    <div class="page-container">
      <h1>${title}</h1>
      <p>Page en cours de développement...</p>
    </div>
  \`,
  styles: [\`
    .page-container {
      padding: 2rem;
      text-align: center;
    }
  \`]
})
export class ${name^}Component {}
EOC
}

# Create all temporary components
create_component "src/app/features/auth/pages/login/login.component.ts" "login" "Connexion"
create_component "src/app/features/auth/pages/register/register.component.ts" "register" "Inscription"
create_component "src/app/features/auth/pages/forgot-password/forgot-password.component.ts" "forgot-password" "Mot de passe oublié"
create_component "src/app/features/customer/pages/dashboard/customer-dashboard.component.ts" "customer-dashboard" "Dashboard Client"
create_component "src/app/features/customer/pages/bookings/my-bookings.component.ts" "my-bookings" "Mes Réservations"
create_component "src/app/features/customer/pages/profile/profile.component.ts" "profile" "Mon Profil"
create_component "src/app/features/customer/pages/loyalty/loyalty.component.ts" "loyalty" "Programme de Fidélité"
create_component "src/app/features/company/pages/dashboard/company-dashboard.component.ts" "company-dashboard" "Dashboard Compagnie"
create_component "src/app/features/company/pages/fleet/fleet-management.component.ts" "fleet-management" "Gestion de Flotte"
create_component "src/app/features/company/pages/flights/flight-management.component.ts" "flight-management" "Gestion des Vols"
create_component "src/app/features/company/pages/analytics/analytics.component.ts" "analytics" "Analyses"
create_component "src/app/features/admin/pages/dashboard/admin-dashboard.component.ts" "admin-dashboard" "Dashboard Admin"
create_component "src/app/features/admin/pages/users/user-management.component.ts" "user-management" "Gestion Utilisateurs"
create_component "src/app/features/admin/pages/content/content-management.component.ts" "content-management" "Gestion de Contenu"
create_component "src/app/features/admin/pages/settings/system-settings.component.ts" "system-settings" "Paramètres Système"

echo "Tous les composants temporaires ont été créés!"
