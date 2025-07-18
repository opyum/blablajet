# Empty Legs Platform - Guide de Développement

## 🚀 Démarrage rapide

### Prérequis
- .NET 8 SDK
- Node.js 18+ 
- Docker & Docker Compose
- Git
- Visual Studio Code ou Visual Studio 2022

### Installation initiale

```bash
# 1. Cloner le repository
git clone <repository-url>
cd empty-legs-platform

# 2. Démarrer les services avec Docker
docker-compose up -d postgres redis

# 3. Configuration backend
cd backend
dotnet restore
dotnet ef database update -p src/EmptyLegs.Infrastructure -s src/EmptyLegs.API

# 4. Configuration frontend
cd ../frontend
npm install

# 5. Configuration mobile
cd ../mobile
npm install
```

### Démarrage des services

```bash
# Terminal 1 - Backend API
cd backend
dotnet run --project src/EmptyLegs.API

# Terminal 2 - Frontend Web
cd frontend
npm run dev

# Terminal 3 - Mobile (optionnel)
cd mobile
npm start
```

L'application sera accessible sur :
- **Backend API** : http://localhost:5000
- **Frontend Web** : http://localhost:3000
- **Swagger API** : http://localhost:5000/swagger
- **Mobile** : Expo DevTools

## 🏗️ Structure du projet

```
/
├── backend/                    # API C# .NET 8
│   ├── src/
│   │   ├── EmptyLegs.API/     # Controllers, Middleware, SignalR
│   │   ├── EmptyLegs.Core/    # Entités, Interfaces, Enums
│   │   ├── EmptyLegs.Application/ # Services, DTOs, CQRS
│   │   └── EmptyLegs.Infrastructure/ # EF Core, Repositories
│   └── tests/                 # Tests unitaires et intégration
├── frontend/                   # Web Next.js 14
│   └── src/
│       ├── app/               # App Router (pages)
│       ├── components/        # Composants réutilisables
│       ├── lib/              # Utilitaires, API client
│       └── types/            # Types TypeScript
├── mobile/                     # React Native + Expo
│   └── src/
│       ├── screens/          # Écrans de l'app
│       ├── components/       # Composants mobiles
│       ├── navigation/       # Configuration navigation
│       └── services/         # API, storage, notifications
├── shared/                     # Types et utilitaires partagés
└── docs/                      # Documentation
```

## 🛠️ Workflows de développement

### 1. Ajouter une nouvelle fonctionnalité

#### Backend (C# API)
1. **Créer l'entité** dans `EmptyLegs.Core/Entities/`
2. **Définir l'interface** dans `EmptyLegs.Core/Interfaces/`
3. **Implémenter le service** dans `EmptyLegs.Application/Services/`
4. **Créer les DTOs** dans `EmptyLegs.Application/DTOs/`
5. **Ajouter le controller** dans `EmptyLegs.API/Controllers/`
6. **Configurer EF** dans `EmptyLegs.Infrastructure/`

Exemple - Nouvelle entité "Notification" :

```csharp
// 1. EmptyLegs.Core/Entities/Notification.cs
public class Notification : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}

// 2. EmptyLegs.Core/Interfaces/INotificationService.cs
public interface INotificationService
{
    Task<NotificationDto> CreateAsync(CreateNotificationDto dto);
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(Guid userId);
}

// 3. EmptyLegs.Application/Services/NotificationService.cs
public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<NotificationDto> CreateAsync(CreateNotificationDto dto)
    {
        // Implémentation...
    }
}

// 4. EmptyLegs.API/Controllers/NotificationsController.cs
[ApiController]
[Route("api/v1/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications()
    {
        // Implémentation...
    }
}
```

#### Frontend (Next.js)
1. **Ajouter les types** dans `types/api.ts`
2. **Créer les hooks API** dans `lib/api/`
3. **Développer les composants** dans `components/`
4. **Ajouter les pages** dans `app/`

Exemple - Page notifications :

```typescript
// 1. types/api.ts
export interface Notification {
  id: string;
  title: string;
  message: string;
  isRead: boolean;
  createdAt: string;
}

// 2. lib/api/notifications.ts
export const useNotifications = () => {
  return useQuery({
    queryKey: ['notifications'],
    queryFn: () => api.get<Notification[]>('/notifications')
  });
};

// 3. components/NotificationsList.tsx
export function NotificationsList() {
  const { data: notifications, isLoading } = useNotifications();
  
  if (isLoading) return <LoadingSpinner />;
  
  return (
    <div className="space-y-4">
      {notifications?.map(notification => (
        <NotificationCard key={notification.id} notification={notification} />
      ))}
    </div>
  );
}

// 4. app/notifications/page.tsx
export default function NotificationsPage() {
  return (
    <div className="container mx-auto px-4">
      <h1 className="text-2xl font-bold mb-6">Notifications</h1>
      <NotificationsList />
    </div>
  );
}
```

### 2. Migration de base de données

```bash
# Créer une nouvelle migration
cd backend
dotnet ef migrations add AddNotificationEntity -p src/EmptyLegs.Infrastructure -s src/EmptyLegs.API

# Appliquer les migrations
dotnet ef database update -p src/EmptyLegs.Infrastructure -s src/EmptyLegs.API

# Rollback (si nécessaire)
dotnet ef database update PreviousMigration -p src/EmptyLegs.Infrastructure -s src/EmptyLegs.API
```

### 3. Tests

```bash
# Tests backend
cd backend
dotnet test

# Tests frontend
cd frontend
npm run test

# Tests E2E
npm run test:e2e
```

## 📊 Bonnes pratiques par technologie

### Backend C# - Règles importantes

1. **Toujours utiliser async/await** pour les opérations I/O
2. **Validation avec FluentValidation** pour tous les DTOs
3. **Logging structuré** avec Serilog
4. **Gestion d'erreurs** avec middleware global
5. **Tests unitaires** avec xUnit et FluentAssertions

```csharp
// ✅ Bon exemple
[HttpPost]
public async Task<ActionResult<FlightDto>> CreateFlight([FromBody] CreateFlightDto dto)
{
    try 
    {
        var result = await _flightService.CreateFlightAsync(dto);
        _logger.LogInformation("Flight created successfully with ID {FlightId}", result.Id);
        return CreatedAtAction(nameof(GetFlight), new { id = result.Id }, result);
    }
    catch (ValidationException ex)
    {
        return BadRequest(ex.Errors);
    }
}

// ❌ Mauvais exemple  
[HttpPost]
public FlightDto CreateFlight(CreateFlightDto dto)
{
    var result = _flightService.CreateFlight(dto); // Synchrone
    return result; // Pas de gestion d'erreur
}
```

### Frontend Next.js - Règles importantes

1. **Server Components par défaut**, Client Components uniquement si nécessaire
2. **React Query** pour tous les appels API
3. **Zod** pour la validation des formulaires
4. **TypeScript strict** pour tous les fichiers
5. **Responsive design** mobile-first

```typescript
// ✅ Bon exemple - Server Component
async function FlightsPage() {
  const initialFlights = await getFlights(); // Server-side
  
  return (
    <div className="container mx-auto px-4">
      <FlightsList initialData={initialFlights} />
    </div>
  );
}

// ✅ Bon exemple - Client Component avec React Query
'use client';
function FlightsList({ initialData }: { initialData: Flight[] }) {
  const { data: flights, isLoading } = useQuery({
    queryKey: ['flights'],
    queryFn: fetchFlights,
    initialData,
  });

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      {flights?.map(flight => <FlightCard key={flight.id} flight={flight} />)}
    </div>
  );
}
```

### Mobile React Native - Règles importantes

1. **Expo managed workflow** pour simplicité
2. **React Navigation** pour la navigation
3. **React Query** avec persistance offline
4. **Optimisation performance** avec FlatList
5. **Gestion d'état** avec Zustand

```typescript
// ✅ Bon exemple - Écran optimisé
function FlightSearchScreen() {
  const { data: flights, isLoading } = useFlights();

  const renderFlightItem = useCallback(({ item }: { item: Flight }) => (
    <FlightCard flight={item} />
  ), []);

  return (
    <SafeAreaView style={styles.container}>
      <FlatList
        data={flights}
        renderItem={renderFlightItem}
        keyExtractor={(item) => item.id}
        removeClippedSubviews
        maxToRenderPerBatch={10}
        windowSize={10}
      />
    </SafeAreaView>
  );
}
```

## 🔧 Outils de développement

### Extensions VS Code recommandées
- C# Dev Kit
- TypeScript Hero
- ES7+ React/Redux/React-Native snippets
- Tailwind CSS IntelliSense
- GitLens
- Thunder Client (tests API)

### Scripts utiles

```bash
# Formatage du code
npm run format                    # Frontend
dotnet format                     # Backend

# Analyse du code
npm run lint                      # Frontend
dotnet run --verbosity normal     # Backend

# Build de production
npm run build                     # Frontend
dotnet publish -c Release        # Backend
```

## 🐛 Debugging

### Backend
1. Utiliser les **breakpoints** dans Visual Studio/VS Code
2. **Logs structurés** avec Serilog
3. **Swagger** pour tester les endpoints
4. **Entity Framework logging** pour les requêtes SQL

### Frontend
1. **React DevTools** pour les composants
2. **TanStack Query DevTools** pour les requêtes
3. **Chrome DevTools** pour performance
4. **Network tab** pour les appels API

### Mobile
1. **Expo DevTools** pour debugging
2. **React Native Debugger** pour inspection
3. **Flipper** pour logging avancé
4. **Metro bundler** pour les erreurs build

## 🚀 Déploiement

### Environnements
- **Development** : localhost avec Docker
- **Staging** : Azure Container Apps
- **Production** : Azure App Service + Azure SQL

### CI/CD Pipeline
Le pipeline Azure DevOps est configuré pour :
1. **Build** automatique sur push
2. **Tests** unitaires et intégration
3. **Déploiement** automatique vers Staging
4. **Déploiement manuel** vers Production

## 📞 Support

- **Documentation** : Consultez les `.cursorrules` et `TECHNICAL_ARCHITECTURE.md`
- **Issues** : Utilisez GitHub Issues avec les labels appropriés
- **Questions** : Canal Slack #empty-legs-dev
- **Code Review** : Pull requests obligatoires avec 2 reviewers minimum

Suivez ces guidelines pour maintenir la qualité et la cohérence du code ! 🚀