# Empty Legs Platform - Architecture Technique

## 🎯 Vue d'ensemble du projet

La plateforme Empty Legs est une solution complète de gestion et réservation de vols à vide, permettant aux compagnies aériennes de valoriser leurs trajets retour et aux clients de bénéficier de tarifs avantageux.

### Acteurs principaux
- **Compagnies (Loueurs)** : Créent et gèrent les vols à vide
- **Clients** : Recherchent et réservent des vols
- **Administrateurs** : Gèrent la plateforme globalement

## 🏗️ Architecture système

### Stack technologique retenu

| Composant | Technologie | Justification |
|-----------|-------------|---------------|
| **API Backend** | C# .NET 8 + ASP.NET Core | Performance, écosystème riche, facilité de maintenance |
| **Base de données** | PostgreSQL + Redis | Robustesse, scalabilité, cache performant |
| **Frontend Web** | Next.js 14 + TypeScript | SSR/SSG, performance, SEO optimisé |
| **Mobile** | React Native + Expo | Code partagé iOS/Android, développement rapide |
| **Authentification** | JWT + Azure AD B2C | Sécurité enterprise, gestion utilisateurs |
| **Paiements** | Stripe | Intégration simple, sécurité PCI-DSS |
| **Notifications** | SignalR + Twilio + SendGrid | Temps réel + SMS + Email |
| **Maps** | Google Maps API | Précision, écosystème complet |
| **Cloud** | Azure | Intégration native .NET, services managés |

## 📊 Modèle de données principal

### Entités core

```csharp
// Utilisateurs et authentification
public class User : BaseEntity
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserRole Role { get; set; } // Admin, Company, Customer
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

// Compagnies
public class Company : BaseEntity
{
    public string Name { get; set; }
    public string License { get; set; }
    public string Description { get; set; }
    public string LogoUrl { get; set; }
    public ContactInfo ContactInfo { get; set; }
    public List<Aircraft> Aircraft { get; set; }
    public List<Flight> Flights { get; set; }
}

// Aéronefs
public class Aircraft : BaseEntity
{
    public string Model { get; set; }
    public string Registration { get; set; }
    public int Capacity { get; set; }
    public AircraftType Type { get; set; }
    public List<string> Amenities { get; set; }
    public List<string> PhotoUrls { get; set; }
    public Guid CompanyId { get; set; }
}

// Vols à vide
public class Flight : BaseEntity
{
    public string FlightNumber { get; set; }
    public Airport DepartureAirport { get; set; }
    public Airport ArrivalAirport { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal BasePrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public int AvailableSeats { get; set; }
    public FlightStatus Status { get; set; }
    public Guid AircraftId { get; set; }
    public Guid CompanyId { get; set; }
    public List<Booking> Bookings { get; set; }
}

// Réservations
public class Booking : BaseEntity
{
    public string BookingReference { get; set; }
    public BookingStatus Status { get; set; }
    public int PassengerCount { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime BookingDate { get; set; }
    public Guid FlightId { get; set; }
    public Guid UserId { get; set; }
    public List<Passenger> Passengers { get; set; }
    public Payment Payment { get; set; }
}

// Passagers
public class Passenger : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string PassportNumber { get; set; }
    public string Nationality { get; set; }
    public List<Document> Documents { get; set; }
}
```

## 🔧 Architecture API (Backend)

### Structure Clean Architecture

```
EmptyLegs.API/
├── Controllers/           # Points d'entrée API
├── Middleware/           # Authentification, erreurs, logs
├── Filters/             # Validation, autorisation
└── Program.cs           # Configuration startup

EmptyLegs.Application/
├── Services/            # Logique métier
├── DTOs/               # Data Transfer Objects
├── Mappings/           # AutoMapper profiles
├── Validators/         # FluentValidation
└── Interfaces/         # Contrats de service

EmptyLegs.Core/
├── Entities/           # Modèles domaine
├── Enums/             # Énumérations
├── Exceptions/        # Exceptions métier
└── Interfaces/        # Contrats repository

EmptyLegs.Infrastructure/
├── Data/              # Context EF, migrations
├── Repositories/      # Implémentations repository
├── Services/          # Services externes (email, SMS)
└── Configurations/    # Configuration EF
```

### Endpoints API principaux

```
Authentication:
POST   /api/v1/auth/login
POST   /api/v1/auth/register
POST   /api/v1/auth/refresh
POST   /api/v1/auth/logout

Flights:
GET    /api/v1/flights/search
GET    /api/v1/flights/{id}
POST   /api/v1/flights
PUT    /api/v1/flights/{id}
DELETE /api/v1/flights/{id}

Bookings:
GET    /api/v1/bookings
GET    /api/v1/bookings/{id}
POST   /api/v1/bookings
PUT    /api/v1/bookings/{id}/status

Companies:
GET    /api/v1/companies
GET    /api/v1/companies/{id}
POST   /api/v1/companies
PUT    /api/v1/companies/{id}

Users:
GET    /api/v1/users/profile
PUT    /api/v1/users/profile
GET    /api/v1/users/bookings

Payments:
POST   /api/v1/payments/intent
POST   /api/v1/payments/confirm
POST   /api/v1/payments/webhook
```

## 🌐 Frontend Web (Next.js)

### Structure de l'application

```
src/
├── app/                    # App Router
│   ├── (auth)/            # Groupe d'authentification
│   ├── (dashboard)/       # Tableau de bord compagnies
│   ├── search/            # Recherche de vols
│   ├── booking/           # Processus de réservation
│   └── profile/           # Profil utilisateur
├── components/
│   ├── ui/               # Composants shadcn/ui
│   ├── forms/            # Formulaires réutilisables
│   ├── layout/           # Header, Footer, Navigation
│   └── features/         # Composants métier
├── lib/
│   ├── api.ts           # Client API
│   ├── auth.ts          # Gestion authentification
│   ├── utils.ts         # Utilitaires
│   └── validations.ts   # Schémas Zod
└── types/
    └── api.ts           # Types TypeScript API
```

### Pages principales

1. **Page d'accueil** : Recherche de vols, mise en avant
2. **Recherche** : Filtres avancés, carte interactive
3. **Détail vol** : Informations complètes, réservation
4. **Processus de réservation** : Formulaire multi-étapes
5. **Dashboard compagnie** : Gestion vols, statistiques
6. **Profil utilisateur** : Historique, préférences

## 📱 Application Mobile (React Native)

### Structure navigation

```
src/
├── navigation/
│   ├── AppNavigator.tsx      # Navigation racine
│   ├── AuthNavigator.tsx     # Stack authentification
│   ├── TabNavigator.tsx      # Navigation onglets
│   └── StackNavigator.tsx    # Navigation empilée
├── screens/
│   ├── auth/                 # Écrans connexion/inscription
│   ├── search/               # Recherche et filtres
│   ├── booking/              # Processus réservation
│   ├── profile/              # Profil et paramètres
│   └── company/              # Interface compagnie
├── components/
│   ├── common/               # Composants réutilisables
│   ├── forms/                # Formulaires
│   └── cards/                # Cartes d'affichage
└── services/
    ├── api.ts               # Client API
    ├── storage.ts           # Stockage local sécurisé
    └── notifications.ts     # Push notifications
```

### Fonctionnalités mobiles spécifiques

- **Notifications push** : Alertes temps réel
- **Mode hors ligne** : Cache des données critiques
- **Géolocalisation** : Recherche par proximité
- **Authentification biométrique** : Touch/Face ID
- **Partage social** : Partage de vols trouvés

## 🔐 Sécurité et authentification

### Architecture JWT

```csharp
// Token structure
{
  "sub": "user-id",
  "email": "user@example.com",
  "role": "Customer|Company|Admin",
  "company_id": "company-id", // Si applicable
  "exp": timestamp,
  "iat": timestamp
}

// Refresh token flow
1. Login → Access Token (15min) + Refresh Token (30 jours)
2. API calls avec Access Token
3. Token expiré → Refresh automatique
4. Logout → Invalidation des tokens
```

### Niveaux d'autorisation

- **Public** : Recherche de vols, informations générales
- **Customer** : Réservations, profil, historique
- **Company** : Gestion vols, passagers, statistiques
- **Admin** : Modération, analytics globales

## 💳 Intégration paiements Stripe

### Flux de paiement

1. **Création Payment Intent** côté backend
2. **Confirmation paiement** côté client
3. **Webhook de confirmation** pour mise à jour booking
4. **Gestion des échecs** et tentatives

```typescript
// Frontend - Processus paiement
const handlePayment = async (bookingData: BookingRequest) => {
  // 1. Créer payment intent
  const { clientSecret } = await createPaymentIntent(bookingData);
  
  // 2. Confirmer paiement avec Stripe
  const result = await stripe.confirmCardPayment(clientSecret, {
    payment_method: paymentMethodId
  });
  
  // 3. Mettre à jour booking si succès
  if (result.paymentIntent?.status === 'succeeded') {
    await confirmBooking(result.paymentIntent.id);
  }
};
```

## 📡 Notifications temps réel

### Architecture SignalR

```csharp
// Hub de notifications
public class NotificationHub : Hub
{
    public async Task JoinCompanyGroup(string companyId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"company_{companyId}");
    }
    
    public async Task JoinUserGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
    }
}

// Service de notifications
public class NotificationService
{
    public async Task NotifyNewBooking(Guid companyId, BookingDto booking)
    {
        await _hubContext.Clients.Group($"company_{companyId}")
            .SendAsync("NewBooking", booking);
    }
    
    public async Task NotifyBookingStatusChange(Guid userId, BookingStatusDto status)
    {
        await _hubContext.Clients.Group($"user_{userId}")
            .SendAsync("BookingStatusChanged", status);
    }
}
```

## 📊 Yield Management et tarification dynamique

### Algorithme de pricing

```csharp
public class PricingEngine
{
    public decimal CalculateDynamicPrice(Flight flight)
    {
        var basePrice = flight.BasePrice;
        var timeToFlight = flight.DepartureTime - DateTime.UtcNow;
        var occupancyRate = CalculateOccupancyRate(flight);
        
        // Réduction temporelle (plus proche = moins cher)
        var timeMultiplier = CalculateTimeMultiplier(timeToFlight);
        
        // Ajustement selon occupation
        var demandMultiplier = CalculateDemandMultiplier(occupancyRate);
        
        // Tendances historiques
        var historicalMultiplier = GetHistoricalTrends(
            flight.DepartureAirport.Code, 
            flight.ArrivalAirport.Code,
            flight.DepartureTime
        );
        
        return basePrice * timeMultiplier * demandMultiplier * historicalMultiplier;
    }
}
```

## 🗺️ Intégration cartographique

### Google Maps implementation

```typescript
// Composant carte de recherche
function FlightMap({ flights }: { flights: Flight[] }) {
  return (
    <GoogleMap
      mapContainerStyle={{ width: '100%', height: '400px' }}
      center={{ lat: 46.603354, lng: 1.888334 }} // Centre France
      zoom={6}
    >
      {flights.map(flight => (
        <FlightPath
          key={flight.id}
          departure={flight.departureAirport.coordinates}
          arrival={flight.arrivalAirport.coordinates}
          onClick={() => selectFlight(flight)}
        />
      ))}
    </GoogleMap>
  );
}
```

## 📈 Analytics et monitoring

### Métriques clés à tracker

**Business metrics:**
- Taux de conversion recherche → réservation
- Revenus par vol, par compagnie
- Taux d'occupation moyen
- Temps moyen entre publication et réservation

**Technical metrics:**
- Performance API (latence, erreurs)
- Disponibilité des services
- Utilisation cache Redis
- Taux d'erreur payments

### Dashboards Power BI

1. **Dashboard Compagnie** : Performance vols, revenus, tendances
2. **Dashboard Admin** : Vue globale plateforme, KPIs
3. **Dashboard Technique** : Monitoring infrastructure

## 🚀 Stratégie de déploiement

### Environnements

- **Development** : Développement local + Azure Dev
- **Staging** : Tests avant production
- **Production** : Environnement live

### Pipeline CI/CD Azure DevOps

```yaml
# azure-pipelines.yml
stages:
- stage: Build
  jobs:
  - job: BuildAPI
    steps:
    - task: DotNetCoreCLI@2
      inputs:
        command: 'build'
        projects: '**/*.csproj'
        
  - job: BuildFrontend
    steps:
    - task: NodeTool@0
      inputs:
        versionSpec: '18.x'
    - script: npm ci && npm run build

- stage: Test
  jobs:
  - job: UnitTests
  - job: IntegrationTests
  - job: E2ETests

- stage: Deploy
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  jobs:
  - deployment: DeployProduction
```

## 🔄 Plan de développement par phases

### Phase 1 - MVP (2-3 mois)
- API de base (vols, compagnies, utilisateurs)
- Frontend web recherche et réservation
- Authentification JWT
- Paiements Stripe basiques

### Phase 2 - Fonctionnalités avancées (2 mois)
- App mobile React Native
- Notifications temps réel
- Yield management basique
- Dashboard compagnies

### Phase 3 - Optimisation (1-2 mois)
- Analytics avancées
- IA pour recommandations prix
- Intégrations calendriers
- Tests de charge et optimisations

Cette architecture garantit une scalabilité, une maintenabilité et une sécurité optimales pour votre plateforme Empty Legs.