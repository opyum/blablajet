# 🛫 Empty Legs Platform

Une plateforme complète de gestion et réservation de vols à vide pour l'aviation privée, permettant aux compagnies aériennes de valoriser leurs trajets retour et aux clients d'accéder à l'aviation privée à prix réduits.

## 🎯 Vue d'ensemble

### Fonctionnalités principales

**Pour les Compagnies :**
- 📅 Gestion des vols à vide avec tarification dynamique
- 📊 Dashboard de performance et analytics
- 💬 Communication temps réel avec les clients
- 🎯 Yield management automatisé

**Pour les Clients :**
- 🔍 Recherche avancée de vols avec carte interactive
- 💳 Réservation et paiement sécurisé (Stripe)
- 📱 Alertes personnalisées
- 📄 Gestion des documents de voyage

**Pour les Administrateurs :**
- 🛡️ Modération et validation des compagnies
- 📈 Analytics globales de la plateforme
- ⚖️ Gestion des litiges et remboursements

## 🏗️ Architecture

### Stack Technologique

| Composant | Technologie | Version |
|-----------|-------------|---------|
| **Backend API** | C# .NET | 8.0 |
| **Base de données** | PostgreSQL + Redis | 16 + 7 |
| **Frontend Web** | Next.js + TypeScript | 14.1 |
| **Mobile** | React Native + Expo | 50.0 |
| **Authentification** | JWT + Azure AD B2C | - |
| **Paiements** | Stripe | v43 |
| **Maps** | Google Maps API | - |
| **Cloud** | Azure | - |

### Architecture Clean Architecture

```
Backend/
├── EmptyLegs.API/              # Controllers, Middleware, SignalR
├── EmptyLegs.Application/      # Services, DTOs, CQRS Commands
├── EmptyLegs.Core/            # Entités, Interfaces, Business Rules
└── EmptyLegs.Infrastructure/   # EF Core, Repositories, External Services
```

## 🚀 Démarrage rapide

### Prérequis
- .NET 8 SDK
- Node.js 18+
- Docker & Docker Compose
- PostgreSQL (ou via Docker)

### Installation

```bash
# 1. Cloner le repository
git clone https://github.com/your-org/empty-legs-platform.git
cd empty-legs-platform

# 2. Démarrer les services de base
docker-compose up -d postgres redis

# 3. Configuration backend
cd backend
dotnet restore
dotnet ef database update -p src/EmptyLegs.Infrastructure -s src/EmptyLegs.API

# 4. Configuration frontend
cd ../frontend
npm install

# 5. Variables d'environnement
cp .env.example .env.local
# Configurer les clés API (Stripe, Google Maps, etc.)
```

### Démarrage des services

```bash
# Terminal 1 - Backend API
cd backend && dotnet run --project src/EmptyLegs.API

# Terminal 2 - Frontend Web  
cd frontend && npm run dev

# Terminal 3 - Mobile (optionnel)
cd mobile && npm start
```

**URLs d'accès :**
- Backend API : http://localhost:5000
- Frontend Web : http://localhost:3000
- API Documentation : http://localhost:5000/swagger

## 📊 Fonctionnalités clés

### Yield Management Intelligent
```csharp
Prix final = Prix base × Multiplicateur temps × Multiplicateur demande × Multiplicateur saisonnier
```

- Réduction temporelle automatique à l'approche du départ
- Ajustement selon le taux d'occupation
- Analyse des tendances historiques

### Notifications Temps Réel
- SignalR pour les mises à jour instantanées
- Push notifications mobiles
- Alertes email et SMS
- Système d'alertes personnalisées

### Intégration Paiements
- Paiements sécurisés avec Stripe
- Support multi-devises
- Gestion des remboursements
- Paiement en plusieurs fois

## 🔐 Sécurité

- **Authentification JWT** avec refresh tokens
- **Autorisation basée sur les rôles** (Customer, Company, Admin)
- **Chiffrement des données sensibles**
- **Validation stricte** de toutes les entrées
- **Audit trail** complet
- **Conformité GDPR**

## 📱 Applications

### Web (Next.js 14)
- Server-Side Rendering pour SEO optimal
- Interface responsive avec Tailwind CSS
- Optimisations de performance avancées
- PWA ready

### Mobile (React Native + Expo)
- Applications natives iOS et Android
- Notifications push
- Mode hors ligne
- Authentification biométrique
- Géolocalisation

## 🛠️ Développement

### Structure du projet
```
/
├── backend/                    # API C# .NET 8
├── frontend/                   # Web Next.js 14
├── mobile/                     # React Native + Expo
├── shared/                     # Types et utilitaires partagés
├── docs/                      # Documentation
└── docker-compose.yml         # Configuration Docker
```

### Standards de code
- **Backend** : Clean Architecture, SOLID principles, DDD
- **Frontend** : App Router, Server Components, React Query
- **Mobile** : Expo managed workflow, TypeScript strict
- **Tests** : 80%+ de couverture de code

### Workflow Git
1. Feature branches depuis `develop`
2. Pull requests avec review obligatoire
3. Tests automatisés en CI/CD
4. Déploiement automatique vers Staging
5. Déploiement manuel vers Production

## 📈 Roadmap

### Phase 1 - MVP (Q1 2024)
- [x] Architecture backend complète
- [x] Frontend web de base
- [ ] API authentication/authorization
- [ ] Gestion des vols et réservations
- [ ] Intégration Stripe

### Phase 2 - Fonctionnalités avancées (Q2 2024)
- [ ] Application mobile complète
- [ ] Notifications temps réel
- [ ] Yield management automatique
- [ ] Analytics et reporting

### Phase 3 - Optimisation (Q3 2024)
- [ ] IA pour recommendations prix
- [ ] Intégrations calendriers
- [ ] Performance optimizations
- [ ] Tests de charge

## 🤝 Contributing

1. Fork le repository
2. Créer une feature branch (`git checkout -b feature/amazing-feature`)
3. Respecter les [Cursor Rules](.cursorrules) définies
4. Commiter les changements (`git commit -m 'Add amazing feature'`)
5. Push vers la branch (`git push origin feature/amazing-feature`)
6. Ouvrir une Pull Request

Consultez le [Guide de Développement](DEVELOPMENT_GUIDE.md) pour plus de détails.

## 📋 Documentation

- [🏗️ Architecture Technique](TECHNICAL_ARCHITECTURE.md)
- [📋 Spécifications Fonctionnelles](FUNCTIONAL_SPECIFICATIONS.md)
- [👨‍💻 Guide de Développement](DEVELOPMENT_GUIDE.md)
- [⚙️ Cursor Rules](.cursorrules)

## 📄 License

Ce projet est sous licence MIT. Voir le fichier [LICENSE](LICENSE) pour plus de détails.

## 📞 Contact

- **Email** : dev@empty-legs.com
- **Slack** : #empty-legs-dev
- **Issues** : [GitHub Issues](https://github.com/your-org/empty-legs-platform/issues)

---

**Développé avec ❤️ pour révolutionner l'aviation privée** ✈️