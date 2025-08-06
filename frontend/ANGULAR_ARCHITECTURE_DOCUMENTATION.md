# Architecture Frontend Angular - Plateforme Empty Legs

## 📋 Table des Matières
1. [Vue d'ensemble du projet](#vue-densemble-du-projet)
2. [Tâches de création du projet Angular](#tâches-de-création-du-projet-angular)
3. [Architecture technique](#architecture-technique)
4. [Librairies et compatibilité](#librairies-et-compatibilité)
5. [Design System Enterprise Luxe](#design-system-enterprise-luxe)
6. [Intégration API C#](#intégration-api-c)
7. [Gestion des traductions (i18n)](#gestion-des-traductions-i18n)
8. [Découpage écran par écran](#découpage-écran-par-écran)

---

## 🎯 Vue d'ensemble du projet

### Contexte métier
Plateforme de réservation de vols "empty legs" pour l'aviation privée de luxe, permettant aux compagnies aériennes de valoriser leurs vols retour et aux clients d'accéder à l'aviation privée à prix réduits.

### Objectifs techniques
- **Performance** : Application rapide et fluide
- **Scalabilité** : Architecture modulaire et maintenable  
- **UX Premium** : Interface moderne adaptée au secteur du luxe
- **Multilingue** : Support français et anglais
- **Responsive** : Expérience optimale sur tous devices

---

## 🚀 Tâches de création du projet Angular

### Phase 1: Initialisation du projet (Priorité: ÉLEVÉE)

#### 1.1 Setup environnement
```bash
# Vérifier les prérequis
node --version  # Minimum v18.19.1
npm --version   # Minimum v10.2.4

# Installation Angular CLI global
npm install -g @angular/cli@18

# Création du projet
ng new empty-legs-frontend --routing --style=scss --strict --package-manager=npm

# Configuration TypeScript stricte
ng config projects.empty-legs-frontend.architect.build.options.strict true
```

#### 1.2 Configuration initiale
- ✅ Routing activé
- ✅ SCSS pour le styling
- ✅ Mode strict TypeScript
- ✅ Configuration ESLint + Prettier
- ✅ Husky pour pre-commit hooks

#### 1.3 Structure des dossiers
```
src/
├── app/
│   ├── core/                    # Services singleton, guards, interceptors
│   ├── shared/                  # Composants, pipes, directives partagés
│   ├── features/               # Modules métier par fonctionnalité
│   │   ├── auth/              # Authentification
│   │   ├── search/            # Recherche de vols
│   │   ├── booking/           # Réservation
│   │   ├── dashboard/         # Tableaux de bord
│   │   └── admin/             # Administration
│   ├── layout/                # Layouts de l'application
│   └── app.component.*
├── assets/                    # Images, fonts, icons
├── environments/             # Configuration environnements
└── styles/                   # Styles globaux SCSS
```

### Phase 2: Configuration avancée

#### 2.1 Configuration de build
```typescript
// angular.json - Optimisations de build
"build": {
  "builder": "@angular-devkit/build-angular:browser",
  "options": {
    "outputPath": "dist/empty-legs-frontend",
    "index": "src/index.html",
    "main": "src/main.ts",
    "polyfills": "src/polyfills.ts",
    "tsConfig": "tsconfig.app.json",
    "inlineStyleLanguage": "scss",
    "assets": ["src/favicon.ico", "src/assets"],
    "styles": ["src/styles.scss"],
    "scripts": [],
    "budgets": [
      {
        "type": "initial",
        "maximumWarning": "2mb",
        "maximumError": "5mb"
      }
    ]
  }
}
```

#### 2.2 Configuration ESLint + Prettier
```json
// .eslintrc.json
{
  "root": true,
  "ignorePatterns": ["projects/**/*"],
  "overrides": [
    {
      "files": ["*.ts"],
      "extends": [
        "@angular-eslint/recommended",
        "@angular-eslint/template/process-inline-templates"
      ],
      "rules": {
        "@angular-eslint/directive-selector": [
          "error",
          { "type": "attribute", "prefix": "app", "style": "camelCase" }
        ],
        "@angular-eslint/component-selector": [
          "error",
          { "type": "element", "prefix": "app", "style": "kebab-case" }
        ]
      }
    }
  ]
}
```

---

## 🏗️ Architecture technique

### Principes architecturaux

#### 1. Architecture modulaire
- **Feature modules** : Un module par fonctionnalité métier
- **Shared module** : Composants/services réutilisables
- **Core module** : Services singleton (importé une seule fois)
- **Lazy loading** : Chargement différé des modules

#### 2. Patterns utilisés
- **Smart/Dumb Components** : Séparation logique métier / présentation
- **Container/Presentational** : Composants conteneurs + composants présentation
- **Service-Repository** : Couche d'abstraction pour les données
- **State Management** : NgRx pour la gestion d'état complexe

#### 3. Structure des composants
```typescript
// Structure type d'un composant
@Component({
  selector: 'app-flight-search',
  templateUrl: './flight-search.component.html',
  styleUrls: ['./flight-search.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FlightSearchComponent implements OnInit, OnDestroy {
  // Properties
  @Input() searchCriteria: SearchCriteria;
  @Output() searchRequested = new EventEmitter<SearchRequest>();
  
  // Lifecycle
  ngOnInit(): void {}
  ngOnDestroy(): void {}
  
  // Methods
  onSearch(): void {}
}
```

### Gestion d'état avec NgRx

#### Structure NgRx
```
src/app/store/
├── actions/
├── reducers/
├── effects/
├── selectors/
└── index.ts
```

#### Configuration du store
```typescript
// app.module.ts
@NgModule({
  imports: [
    StoreModule.forRoot(reducers),
    EffectsModule.forRoot([AppEffects]),
    StoreDevtoolsModule.instrument({
      maxAge: 25,
      logOnly: environment.production
    })
  ]
})
export class AppModule {}
```

---

## 📦 Librairies et compatibilité

### Librairies principales (Angular 18 compatible)

#### 1. UI/Design System
```json
{
  "@angular/material": "^18.0.0",
  "@angular/cdk": "^18.0.0",
  "@angular/flex-layout": "^15.0.0-beta.42",
  "primeng": "^18.0.0",
  "ng-bootstrap": "^17.0.0"
}
```

#### 2. État et données
```json
{
  "@ngrx/store": "^18.0.0",
  "@ngrx/effects": "^18.0.0",
  "@ngrx/router-store": "^18.0.0",
  "@ngrx/store-devtools": "^18.0.0",
  "rxjs": "^7.8.1"
}
```

#### 3. Internationalisation
```json
{
  "@angular/localize": "^18.0.0",
  "@ngx-translate/core": "^15.0.0",
  "@ngx-translate/http-loader": "^8.0.0"
}
```

#### 4. Formulaires et validation
```json
{
  "@angular/forms": "^18.0.0",
  "ngx-mask": "^18.0.0",
  "ng2-validation": "^4.2.0"
}
```

#### 5. Cartes et géolocalisation
```json
{
  "@angular/google-maps": "^18.0.0",
  "leaflet": "^1.9.4",
  "@asymmetrik/ngx-leaflet": "^17.0.0"
}
```

#### 6. Utilitaires et helpers
```json
{
  "date-fns": "^3.6.0",
  "lodash-es": "^4.17.21",
  "@types/lodash-es": "^4.17.12",
  "uuid": "^10.0.0",
  "@types/uuid": "^10.0.0"
}
```

#### 7. Tests
```json
{
  "@angular/testing": "^18.0.0",
  "jasmine": "^5.1.0",
  "karma": "^6.4.3",
  "@types/jasmine": "^5.1.4"
}
```

### Matrice de compatibilité validée

| Librairie | Version | Angular 18 | Status | Notes |
|-----------|---------|------------|--------|-------|
| Angular Material | 18.x | ✅ | Stable | UI components officiels |
| PrimeNG | 18.x | ✅ | Stable | Rich UI components |
| NgRx | 18.x | ✅ | Stable | State management |
| NGX-Translate | 15.x | ✅ | Stable | i18n solution |
| Angular Google Maps | 18.x | ✅ | Stable | Google Maps integration |
| Angular Flex Layout | 15.x | ⚠️ | Deprecated | Migrer vers CSS Grid/Flexbox |

---

## 🎨 Design System Enterprise Luxe

### Palette de couleurs

#### Couleurs primaires (Luxe et élégance)
```scss
// _colors.scss
$primary-colors: (
  'deep-navy': #1a1a2e,        // Bleu marine profond
  'luxury-gold': #d4af37,      // Or luxueux
  'platinum': #e5e4e2,         // Platine
  'charcoal': #36454f,         // Gris charbon
  'ivory': #fffff0             // Ivoire
);

$secondary-colors: (
  'midnight-blue': #191970,    // Bleu nuit
  'champagne': #f7e7ce,        // Champagne
  'silver': #c0c0c0,           // Argent
  'pearl': #eae0c8,            // Perle
  'obsidian': #0f0f0f          // Obsidienne
);
```

#### Couleurs fonctionnelles
```scss
$functional-colors: (
  'success': #2e7d32,          // Vert succès
  'warning': #f57c00,          // Orange attention
  'error': #c62828,            // Rouge erreur
  'info': #1565c0              // Bleu information
);
```

### Typographie

#### Fonts principales
```scss
// _typography.scss
@import url('https://fonts.googleapis.com/css2?family=Playfair+Display:wght@400;500;600;700&display=swap');
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap');

$font-families: (
  'heading': ('Playfair Display', serif),    // Titres élégants
  'body': ('Inter', sans-serif),             // Corps de texte moderne
  'code': ('Consolas', 'Monaco', monospace)  // Code/données
);

$font-sizes: (
  'xs': 0.75rem,   // 12px
  'sm': 0.875rem,  // 14px
  'base': 1rem,    // 16px
  'lg': 1.125rem,  // 18px
  'xl': 1.25rem,   // 20px
  '2xl': 1.5rem,   // 24px
  '3xl': 1.875rem, // 30px
  '4xl': 2.25rem,  // 36px
  '5xl': 3rem      // 48px
);
```

#### Hiérarchie typographique
```scss
.text-hierarchy {
  h1 { @include font-style('heading', '4xl', 700); }
  h2 { @include font-style('heading', '3xl', 600); }
  h3 { @include font-style('heading', '2xl', 600); }
  h4 { @include font-style('heading', 'xl', 500); }
  h5 { @include font-style('heading', 'lg', 500); }
  h6 { @include font-style('heading', 'base', 500); }
  
  body { @include font-style('body', 'base', 400); }
  .caption { @include font-style('body', 'sm', 400); }
  .overline { @include font-style('body', 'xs', 500); }
}
```

### Composants de base

#### Boutons premium
```scss
// _buttons.scss
.btn {
  &--primary {
    background: linear-gradient(135deg, map-get($primary-colors, 'luxury-gold'), #b8860b);
    color: map-get($primary-colors, 'deep-navy');
    box-shadow: 0 4px 15px rgba(212, 175, 55, 0.3);
    
    &:hover {
      transform: translateY(-2px);
      box-shadow: 0 6px 20px rgba(212, 175, 55, 0.4);
    }
  }
  
  &--secondary {
    background: transparent;
    border: 2px solid map-get($primary-colors, 'platinum');
    color: map-get($primary-colors, 'charcoal');
    
    &:hover {
      background: map-get($primary-colors, 'platinum');
      transform: translateY(-1px);
    }
  }
}
```

#### Cards élégantes
```scss
// _cards.scss
.card {
  &--luxury {
    background: linear-gradient(145deg, #ffffff, #f8f9fa);
    border-radius: 16px;
    box-shadow: 
      0 10px 30px rgba(0, 0, 0, 0.1),
      0 1px 8px rgba(0, 0, 0, 0.06);
    border: 1px solid rgba(map-get($primary-colors, 'platinum'), 0.3);
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    
    &:hover {
      transform: translateY(-8px);
      box-shadow: 
        0 20px 40px rgba(0, 0, 0, 0.15),
        0 1px 12px rgba(0, 0, 0, 0.1);
    }
  }
}
```

### Système de spacing

```scss
// _spacing.scss
$spacing: (
  'xs': 0.25rem,   // 4px
  'sm': 0.5rem,    // 8px
  'md': 1rem,      // 16px
  'lg': 1.5rem,    // 24px
  'xl': 2rem,      // 32px
  '2xl': 3rem,     // 48px
  '3xl': 4rem,     // 64px
  '4xl': 6rem,     // 96px
  '5xl': 8rem      // 128px
);
```

### Animations et transitions

```scss
// _animations.scss
$transitions: (
  'fast': 0.15s ease-out,
  'normal': 0.3s cubic-bezier(0.4, 0, 0.2, 1),
  'slow': 0.5s cubic-bezier(0.23, 1, 0.32, 1)
);

@keyframes slideInFromBottom {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes fadeInScale {
  from {
    opacity: 0;
    transform: scale(0.95);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}
```

---

## 🔌 Intégration API C#

### Configuration de l'API Client

#### Service de base pour HTTP
```typescript
// core/services/api.service.ts
@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl = environment.apiUrl;
  
  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}
  
  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': token ? `Bearer ${token}` : ''
    });
  }
  
  get<T>(endpoint: string, params?: HttpParams): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}/${endpoint}`, {
      headers: this.getHeaders(),
      params
    }).pipe(
      catchError(this.handleError)
    );
  }
  
  post<T>(endpoint: string, data: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}/${endpoint}`, data, {
      headers: this.getHeaders()
    }).pipe(
      catchError(this.handleError)
    );
  }
  
  private handleError(error: HttpErrorResponse): Observable<never> {
    // Gestion centralisée des erreurs
    return throwError(() => error);
  }
}
```

#### Services métier spécialisés
```typescript
// features/flights/services/flight.service.ts
@Injectable({
  providedIn: 'root'
})
export class FlightService {
  constructor(private apiService: ApiService) {}
  
  searchFlights(criteria: SearchCriteria): Observable<Flight[]> {
    const params = new HttpParams()
      .set('departure', criteria.departure)
      .set('arrival', criteria.arrival)
      .set('date', criteria.date.toISOString())
      .set('passengers', criteria.passengers.toString());
      
    return this.apiService.get<Flight[]>('flights/search', params);
  }
  
  getFlightDetails(id: string): Observable<FlightDetails> {
    return this.apiService.get<FlightDetails>(`flights/${id}`);
  }
  
  createBooking(booking: CreateBookingRequest): Observable<Booking> {
    return this.apiService.post<Booking>('bookings', booking);
  }
}
```

### Modèles TypeScript (Types C# mappés)

```typescript
// shared/models/flight.models.ts
export interface Flight {
  id: string;
  departure: Airport;
  arrival: Airport;
  departureDateTime: Date;
  arrivalDateTime: Date;
  aircraft: Aircraft;
  availableSeats: number;
  pricePerSeat: number;
  currency: string;
  company: Company;
  services: FlightService[];
  status: FlightStatus;
}

export interface Airport {
  iataCode: string;
  name: string;
  city: string;
  country: string;
  timezone: string;
  coordinates: Coordinates;
}

export interface Aircraft {
  id: string;
  model: string;
  manufacturer: string;
  capacity: number;
  yearManufactured: number;
  images: string[];
  amenities: string[];
}

export enum FlightStatus {
  Available = 'Available',
  Reserved = 'Reserved',
  Cancelled = 'Cancelled',
  Completed = 'Completed'
}
```

### Intercepteurs HTTP

#### Intercepteur d'authentification
```typescript
// core/interceptors/auth.interceptor.ts
@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}
  
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.authService.getToken();
    
    if (token && !req.url.includes('/auth/')) {
      const authReq = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token}`)
      });
      return next.handle(authReq);
    }
    
    return next.handle(req);
  }
}
```

#### Intercepteur de gestion d'erreur
```typescript
// core/interceptors/error.interceptor.ts
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private toastr: ToastrService,
    private router: Router
  ) {}
  
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        switch (error.status) {
          case 401:
            this.router.navigate(['/auth/login']);
            break;
          case 403:
            this.toastr.error('Accès non autorisé');
            break;
          case 500:
            this.toastr.error('Erreur serveur, veuillez réessayer');
            break;
          default:
            this.toastr.error('Une erreur est survenue');
        }
        return throwError(() => error);
      })
    );
  }
}
```

### Configuration des environnements

```typescript
// environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api',
  auth: {
    clientId: 'empty-legs-frontend',
    authority: 'https://localhost:5001',
    responseType: 'code',
    scope: 'openid profile email api1'
  },
  maps: {
    googleMapsApiKey: 'YOUR_GOOGLE_MAPS_API_KEY'
  }
};

// environments/environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://api.emptylegs.com/api',
  auth: {
    clientId: 'empty-legs-frontend',
    authority: 'https://auth.emptylegs.com',
    responseType: 'code',
    scope: 'openid profile email api1'
  },
  maps: {
    googleMapsApiKey: 'YOUR_PRODUCTION_GOOGLE_MAPS_API_KEY'
  }
};
```

---

## 🌍 Gestion des traductions (i18n)

### Configuration NGX-Translate

#### Installation et setup
```typescript
// app.module.ts
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpClient } from '@angular/common/http';

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

@NgModule({
  imports: [
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      },
      defaultLanguage: 'fr'
    })
  ]
})
export class AppModule {}
```

#### Service de langue
```typescript
// core/services/language.service.ts
@Injectable({
  providedIn: 'root'
})
export class LanguageService {
  private currentLang = new BehaviorSubject<string>('fr');
  public currentLang$ = this.currentLang.asObservable();
  
  private readonly supportedLanguages = ['fr', 'en'];
  private readonly storageKey = 'preferred-language';
  
  constructor(private translate: TranslateService) {
    this.initializeLanguage();
  }
  
  private initializeLanguage(): void {
    const savedLang = localStorage.getItem(this.storageKey);
    const browserLang = this.translate.getBrowserLang();
    const defaultLang = savedLang || browserLang || 'fr';
    
    const language = this.supportedLanguages.includes(defaultLang) 
      ? defaultLang 
      : 'fr';
      
    this.setLanguage(language);
  }
  
  setLanguage(lang: string): void {
    if (this.supportedLanguages.includes(lang)) {
      this.translate.use(lang);
      this.currentLang.next(lang);
      localStorage.setItem(this.storageKey, lang);
      
      // Mise à jour de l'attribut lang du HTML
      document.documentElement.lang = lang;
    }
  }
  
  getCurrentLanguage(): string {
    return this.currentLang.value;
  }
  
  getSupportedLanguages(): string[] {
    return [...this.supportedLanguages];
  }
  
  translate(key: string, params?: any): Observable<string> {
    return this.translate.get(key, params);
  }
  
  instant(key: string, params?: any): string {
    return this.translate.instant(key, params);
  }
}
```

### Structure des fichiers de traduction

#### Français (assets/i18n/fr.json)
```json
{
  "common": {
    "buttons": {
      "search": "Rechercher",
      "book": "Réserver",
      "cancel": "Annuler",
      "confirm": "Confirmer",
      "back": "Retour",
      "next": "Suivant",
      "save": "Enregistrer",
      "edit": "Modifier",
      "delete": "Supprimer"
    },
    "labels": {
      "departure": "Départ",
      "arrival": "Arrivée",
      "date": "Date",
      "time": "Heure",
      "passengers": "Passagers",
      "price": "Prix",
      "duration": "Durée",
      "aircraft": "Aéronef"
    },
    "messages": {
      "loading": "Chargement en cours...",
      "noResults": "Aucun résultat trouvé",
      "error": "Une erreur est survenue",
      "success": "Opération réussie"
    }
  },
  "navigation": {
    "home": "Accueil",
    "search": "Rechercher",
    "bookings": "Mes Réservations",
    "profile": "Mon Profil",
    "dashboard": "Tableau de Bord",
    "admin": "Administration"
  },
  "flight": {
    "search": {
      "title": "Rechercher un vol Empty Legs",
      "subtitle": "Trouvez votre vol privé au meilleur prix",
      "form": {
        "departureAirport": "Aéroport de départ",
        "arrivalAirport": "Aéroport d'arrivée",
        "departureDate": "Date de départ",
        "passengerCount": "Nombre de passagers",
        "searchButton": "Rechercher les vols"
      }
    },
    "results": {
      "title": "Vols disponibles",
      "noFlights": "Aucun vol disponible pour ces critères",
      "priceFrom": "À partir de",
      "seatsAvailable": "{{count}} place(s) disponible(s)",
      "bookNow": "Réserver maintenant"
    },
    "details": {
      "flightInfo": "Informations du vol",
      "aircraftInfo": "Informations de l'aéronef",
      "services": "Services inclus",
      "terms": "Conditions"
    }
  },
  "booking": {
    "steps": {
      "flightSelection": "Sélection du vol",
      "passengerInfo": "Informations passagers",
      "payment": "Paiement",
      "confirmation": "Confirmation"
    },
    "passenger": {
      "title": "Informations des passagers",
      "firstName": "Prénom",
      "lastName": "Nom",
      "email": "Email",
      "phone": "Téléphone",
      "dateOfBirth": "Date de naissance",
      "nationality": "Nationalité"
    },
    "payment": {
      "title": "Paiement",
      "cardNumber": "Numéro de carte",
      "expiryDate": "Date d'expiration",
      "cvv": "CVV",
      "cardholderName": "Nom du titulaire",
      "billingAddress": "Adresse de facturation"
    }
  }
}
```

#### Anglais (assets/i18n/en.json)
```json
{
  "common": {
    "buttons": {
      "search": "Search",
      "book": "Book",
      "cancel": "Cancel",
      "confirm": "Confirm",
      "back": "Back",
      "next": "Next",
      "save": "Save",
      "edit": "Edit",
      "delete": "Delete"
    },
    "labels": {
      "departure": "Departure",
      "arrival": "Arrival",
      "date": "Date",
      "time": "Time",
      "passengers": "Passengers",
      "price": "Price",
      "duration": "Duration",
      "aircraft": "Aircraft"
    },
    "messages": {
      "loading": "Loading...",
      "noResults": "No results found",
      "error": "An error occurred",
      "success": "Operation successful"
    }
  },
  "navigation": {
    "home": "Home",
    "search": "Search",
    "bookings": "My Bookings",
    "profile": "My Profile",
    "dashboard": "Dashboard",
    "admin": "Administration"
  },
  "flight": {
    "search": {
      "title": "Search Empty Legs Flights",
      "subtitle": "Find your private flight at the best price",
      "form": {
        "departureAirport": "Departure airport",
        "arrivalAirport": "Arrival airport",
        "departureDate": "Departure date",
        "passengerCount": "Number of passengers",
        "searchButton": "Search flights"
      }
    },
    "results": {
      "title": "Available flights",
      "noFlights": "No flights available for these criteria",
      "priceFrom": "From",
      "seatsAvailable": "{{count}} seat(s) available",
      "bookNow": "Book now"
    },
    "details": {
      "flightInfo": "Flight information",
      "aircraftInfo": "Aircraft information",
      "services": "Included services",
      "terms": "Terms & conditions"
    }
  },
  "booking": {
    "steps": {
      "flightSelection": "Flight selection",
      "passengerInfo": "Passenger information",
      "payment": "Payment",
      "confirmation": "Confirmation"
    },
    "passenger": {
      "title": "Passenger information",
      "firstName": "First name",
      "lastName": "Last name",
      "email": "Email",
      "phone": "Phone",
      "dateOfBirth": "Date of birth",
      "nationality": "Nationality"
    },
    "payment": {
      "title": "Payment",
      "cardNumber": "Card number",
      "expiryDate": "Expiry date",
      "cvv": "CVV",
      "cardholderName": "Cardholder name",
      "billingAddress": "Billing address"
    }
  }
}
```

### Composant sélecteur de langue

```typescript
// shared/components/language-selector/language-selector.component.ts
@Component({
  selector: 'app-language-selector',
  template: `
    <div class="language-selector">
      <button 
        class="language-btn"
        [class.active]="currentLang === 'fr'"
        (click)="setLanguage('fr')"
        [attr.aria-label]="'common.languages.french' | translate">
        🇫🇷 FR
      </button>
      <button 
        class="language-btn"
        [class.active]="currentLang === 'en'"
        (click)="setLanguage('en')"
        [attr.aria-label]="'common.languages.english' | translate">
        🇬🇧 EN
      </button>
    </div>
  `,
  styleUrls: ['./language-selector.component.scss']
})
export class LanguageSelectorComponent implements OnInit {
  currentLang: string = 'fr';
  
  constructor(private languageService: LanguageService) {}
  
  ngOnInit(): void {
    this.languageService.currentLang$.subscribe(lang => {
      this.currentLang = lang;
    });
  }
  
  setLanguage(lang: string): void {
    this.languageService.setLanguage(lang);
  }
}
```

### Pipe personnalisé pour les devises

```typescript
// shared/pipes/currency-localized.pipe.ts
@Pipe({
  name: 'currencyLocalized'
})
export class CurrencyLocalizedPipe implements PipeTransform {
  constructor(private languageService: LanguageService) {}
  
  transform(value: number, currency: string = 'EUR'): string {
    const currentLang = this.languageService.getCurrentLanguage();
    const locale = currentLang === 'fr' ? 'fr-FR' : 'en-US';
    
    return new Intl.NumberFormat(locale, {
      style: 'currency',
      currency: currency
    }).format(value);
  }
}
```

---

## 🖥️ Découpage écran par écran

### Architecture de navigation

```
Application Root
├── Public Layout
│   ├── Homepage (/)
│   ├── Flight Search (/search)
│   ├── Flight Details (/flights/:id)
│   └── Auth Pages (/auth/*)
├── Customer Layout
│   ├── Dashboard (/dashboard)
│   ├── Bookings (/bookings)
│   ├── Profile (/profile)
│   └── Loyalty (/loyalty)
├── Company Layout
│   ├── Company Dashboard (/company)
│   ├── Fleet Management (/company/fleet)
│   ├── Flight Management (/company/flights)
│   └── Analytics (/company/analytics)
└── Admin Layout
    ├── Admin Dashboard (/admin)
    ├── User Management (/admin/users)
    ├── Content Management (/admin/content)
    └── System Settings (/admin/settings)
```

### 1. Pages Publiques

#### 1.1 Homepage (/)
**Objectif** : Présenter la plateforme et inciter à la recherche
**Composants principaux** :
- Hero section avec recherche rapide
- Destinations populaires
- Témoignages clients
- Comment ça marche
- CTA inscription

```typescript
// features/public/pages/homepage/homepage.component.ts
@Component({
  selector: 'app-homepage',
  templateUrl: './homepage.component.html',
  styleUrls: ['./homepage.component.scss']
})
export class HomepageComponent implements OnInit {
  popularDestinations: Destination[] = [];
  testimonials: Testimonial[] = [];
  
  constructor(
    private flightService: FlightService,
    private router: Router
  ) {}
  
  ngOnInit(): void {
    this.loadPopularDestinations();
    this.loadTestimonials();
  }
  
  onQuickSearch(criteria: SearchCriteria): void {
    this.router.navigate(['/search'], { 
      queryParams: { 
        departure: criteria.departure,
        arrival: criteria.arrival,
        date: criteria.date.toISOString(),
        passengers: criteria.passengers 
      }
    });
  }
}
```

#### 1.2 Flight Search (/search)
**Objectif** : Recherche et filtrage des vols
**Composants principaux** :
- Formulaire de recherche avancé
- Filtres (prix, horaires, compagnies)
- Liste des résultats avec pagination
- Carte interactive
- Tri et comparaison

```typescript
// features/search/pages/flight-search/flight-search.component.ts
@Component({
  selector: 'app-flight-search',
  templateUrl: './flight-search.component.html',
  styleUrls: ['./flight-search.component.scss']
})
export class FlightSearchComponent implements OnInit, OnDestroy {
  searchForm: FormGroup;
  flights$ = new BehaviorSubject<Flight[]>([]);
  loading$ = new BehaviorSubject<boolean>(false);
  filters: SearchFilters = new SearchFilters();
  
  private destroy$ = new Subject<void>();
  
  constructor(
    private fb: FormBuilder,
    private flightService: FlightService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.initializeForm();
  }
  
  ngOnInit(): void {
    this.loadFiltersFromUrl();
    this.searchFlights();
  }
  
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  
  onSearch(): void {
    if (this.searchForm.valid) {
      this.updateUrlParams();
      this.searchFlights();
    }
  }
  
  onFilterChange(filters: SearchFilters): void {
    this.filters = filters;
    this.applyFilters();
  }
  
  private searchFlights(): void {
    this.loading$.next(true);
    const criteria = this.searchForm.value;
    
    this.flightService.searchFlights(criteria)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (flights) => {
          this.flights$.next(flights);
          this.loading$.next(false);
        },
        error: (error) => {
          console.error('Search error:', error);
          this.loading$.next(false);
        }
      });
  }
}
```

#### 1.3 Flight Details (/flights/:id)
**Objectif** : Détails complets d'un vol et démarrage réservation
**Composants principaux** :
- Informations détaillées du vol
- Galerie photos de l'aéronef
- Services inclus
- Conditions d'annulation
- Bouton de réservation

### 2. Authentification

#### 2.1 Login (/auth/login)
#### 2.2 Register (/auth/register)
#### 2.3 Forgot Password (/auth/forgot-password)
#### 2.4 Reset Password (/auth/reset-password)

### 3. Dashboard Client

#### 3.1 Customer Dashboard (/dashboard)
**Objectif** : Vue d'ensemble des activités client
**Composants principaux** :
- Prochains vols
- Réservations récentes
- Points de fidélité
- Recommandations personnalisées

```typescript
// features/customer/pages/dashboard/customer-dashboard.component.ts
@Component({
  selector: 'app-customer-dashboard',
  templateUrl: './customer-dashboard.component.html',
  styleUrls: ['./customer-dashboard.component.scss']
})
export class CustomerDashboardComponent implements OnInit {
  upcomingFlights$ = new BehaviorSubject<Booking[]>([]);
  recentBookings$ = new BehaviorSubject<Booking[]>([]);
  loyaltyPoints$ = new BehaviorSubject<number>(0);
  recommendations$ = new BehaviorSubject<Flight[]>([]);
  
  constructor(
    private bookingService: BookingService,
    private loyaltyService: LoyaltyService,
    private recommendationService: RecommendationService
  ) {}
  
  ngOnInit(): void {
    this.loadDashboardData();
  }
  
  private loadDashboardData(): void {
    // Chargement parallèle des données
    forkJoin({
      upcomingFlights: this.bookingService.getUpcomingFlights(),
      recentBookings: this.bookingService.getRecentBookings(5),
      loyaltyPoints: this.loyaltyService.getLoyaltyPoints(),
      recommendations: this.recommendationService.getPersonalizedRecommendations()
    }).subscribe({
      next: (data) => {
        this.upcomingFlights$.next(data.upcomingFlights);
        this.recentBookings$.next(data.recentBookings);
        this.loyaltyPoints$.next(data.loyaltyPoints);
        this.recommendations$.next(data.recommendations);
      }
    });
  }
}
```

#### 3.2 My Bookings (/bookings)
**Objectif** : Gestion des réservations
**Composants** :
- Liste des réservations (passées, futures, annulées)
- Détails de réservation
- Actions (modifier, annuler, télécharger documents)

#### 3.3 Profile (/profile)
**Objectif** : Gestion du profil utilisateur
**Composants** :
- Informations personnelles
- Préférences de voyage
- Documents d'identité
- Paramètres de notification

#### 3.4 Loyalty Program (/loyalty)
**Objectif** : Programme de fidélité
**Composants** :
- Solde de points
- Historique des gains/dépenses
- Avantages disponibles
- Statut membre

### 4. Dashboard Compagnie

#### 4.1 Company Dashboard (/company)
**Objectif** : Vue d'ensemble business compagnie
**Composants** :
- KPIs (revenus, vols, taux de remplissage)
- Graphiques de performance
- Vols récents
- Demandes de réservation en attente

#### 4.2 Fleet Management (/company/fleet)
**Objectif** : Gestion de la flotte d'aéronefs
**Composants** :
- Liste des aéronefs
- Statuts (disponible, en maintenance, en vol)
- Planning de maintenance
- Ajout/modification d'aéronef

#### 4.3 Flight Management (/company/flights)
**Objectif** : Gestion des vols empty legs
**Composants** :
- Création de nouveau vol
- Liste des vols publiés
- Demandes de réservation
- Modification/annulation de vol

#### 4.4 Analytics (/company/analytics)
**Objectif** : Analyses et rapports détaillés
**Composants** :
- Revenus par période
- Performance par route
- Analyse de la demande
- Rapports d'utilisation

### 5. Administration

#### 5.1 Admin Dashboard (/admin)
**Objectif** : Vue d'ensemble plateforme
**Composants** :
- Métriques globales
- Activité récente
- Alertes système
- Statistiques utilisateurs

#### 5.2 User Management (/admin/users)
**Objectif** : Gestion des utilisateurs
**Composants** :
- Liste des utilisateurs
- Filtres et recherche
- Actions administratives
- Historique des actions

#### 5.3 Content Management (/admin/content)
**Objectif** : Gestion du contenu
**Composants** :
- Pages CMS
- Gestion des médias
- Templates d'emails
- Traductions

#### 5.4 System Settings (/admin/settings)
**Objectif** : Configuration système
**Composants** :
- Paramètres généraux
- Configuration API
- Gestion des tarifs
- Logs système

---

## 🚀 Plan de développement

### Phase 1: Foundation (Semaines 1-2)
- ✅ Setup projet Angular 18
- ✅ Configuration architecture
- ✅ Design system de base
- ✅ Authentification
- ✅ Routing principal

### Phase 2: Core Features (Semaines 3-6)
- 🔄 Pages publiques (Homepage, Search)
- 🔄 Intégration API C#
- 🔄 Système de réservation
- 🔄 Dashboard client

### Phase 3: Advanced Features (Semaines 7-10)
- ⏳ Dashboard compagnie
- ⏳ Système de paiement
- ⏳ Programme de fidélité
- ⏳ Notifications temps réel

### Phase 4: Administration & Polish (Semaines 11-12)
- ⏳ Dashboard admin
- ⏳ Tests e2e
- ⏳ Optimisations performance
- ⏳ Déploiement production

---

Cette documentation constitue la base complète pour le développement du frontend Angular de la plateforme Empty Legs. Elle respecte les meilleures pratiques et assure une architecture solide et maintenable.