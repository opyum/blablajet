# Empty Legs Platform - Spécifications Fonctionnelles

## 🎯 Vue d'ensemble fonctionnelle

### Problématique business
Les compagnies d'aviation privée effectuent régulièrement des vols à vide (repositionnement d'aéronefs), représentant des coûts importants. La plateforme Empty Legs permet de valoriser ces trajets en les proposant aux clients à tarifs réduits.

### Proposition de valeur
- **Pour les compagnies** : Monétisation des vols retour, optimisation des coûts
- **Pour les clients** : Accès à l'aviation privée à prix attractifs
- **Pour la plateforme** : Commission sur chaque réservation

## 👥 Personas et cas d'usage

### 🏢 Compagnie aérienne (Loueur)
**Profil type** : Gestionnaire de flotte, Commercial aviation d'affaires
**Objectifs** :
- Minimiser les coûts de repositionnement
- Optimiser la rentabilité de la flotte
- Simplifier la gestion des réservations

**Cas d'usage principaux** :
1. Publier un vol à vide rapidement
2. Gérer la tarification dynamique
3. Valider/refuser les demandes de réservation
4. Suivre les performances et revenus

### 🧳 Client voyageur
**Profil type** : Entrepreneur, cadre dirigeant, voyageur occasionnel haut de gamme
**Objectifs** :
- Voyager en jet privé à prix accessible
- Flexibilité et confort de voyage
- Gain de temps vs aviation commerciale

**Cas d'usage principaux** :
1. Rechercher des vols selon critères
2. Comparer les options disponibles
3. Réserver et payer rapidement
4. Gérer ses voyages et documents

### 👨‍💼 Administrateur plateforme
**Profil type** : Équipe technique/business de la plateforme
**Objectifs** :
- Assurer la qualité du service
- Modérer les contenus et litiges
- Optimiser les performances business

## 🛫 Fonctionnalités Compagnies - Spécifications détaillées

### 1. Gestion des vols à vide

#### 1.1 Création rapide d'un vol
**Entrées requises** :
- Aéroport de départ (recherche avec autocomplétion IATA)
- Aéroport d'arrivée (recherche avec autocomplétion IATA)
- Date et heure de départ (calendrier + sélecteur heure)
- Aéronef disponible (sélection depuis la flotte)
- Nombre de places disponibles (max = capacité aéronef)
- Prix de base (€)

**Entrées optionnelles** :
- Heure d'arrivée estimée (auto-calculée si vide)
- Services inclus (repas, wifi, etc.)
- Restrictions particulières
- Photos supplémentaires

**Règles business** :
- Départ minimum 2h dans le futur
- Prix minimum = coût carburant estimé
- Validation automatique des codes aéroports IATA

**Interface** :
```
┌─────────────────────────────────┐
│ 🛫 Nouveau vol à vide           │
├─────────────────────────────────┤
│ Départ: [CDG - Paris      ▼]   │
│ Arrivée: [NCE - Nice      ▼]   │
│ Date: [📅 15/03/2024]          │
│ Heure: [⏰ 14:30]              │
│ Aéronef: [Citation XLS+   ▼]   │
│ Places: [🪑 6 / 8]             │
│ Prix: [💰 2500 €]              │
│                                 │
│ ⚙️ Options avancées            │
│ └─ Services inclus              │
│ └─ Restrictions                 │
│                                 │
│ [Annuler] [👁️ Aperçu] [✅ Publier] │
└─────────────────────────────────┘
```

#### 1.2 Gestion dynamique des tarifs

**Algorithme de pricing automatique** :
```
Prix final = Prix base × Multiplicateur temps × Multiplicateur demande × Multiplicateur saisonnier

Multiplicateur temps :
- > 7 jours : 1.0
- 3-7 jours : 0.9
- 1-3 jours : 0.8
- < 24h : 0.7

Multiplicateur demande :
- 0% occupation : 0.8
- 1-50% : 0.9
- 51-80% : 1.0
- > 80% : 1.1

Multiplicateur saisonnier :
- Haute saison : 1.2
- Moyenne saison : 1.0
- Basse saison : 0.9
```

**Interface de contrôle prix** :
```
┌─────────────────────────────────┐
│ 💰 Gestion des prix            │
├─────────────────────────────────┤
│ Prix actuel: 2100€ ⚡ (-400€)  │
│                                 │
│ 📊 Réductions automatiques:    │
│ └─ Temporelle: -10% (3j restant)│
│ └─ Demande: -5% (20% occupé)   │
│ └─ Saisonnière: 0%             │
│                                 │
│ ⚙️ Paramètres:                 │
│ □ Prix automatique             │
│ □ Réduction de dernière minute │
│ □ Prix minimum: [1800€]        │
│                                 │
│ [💾 Sauvegarder]               │
└─────────────────────────────────┘
```

### 2. Planning et calendrier

#### 2.1 Vue calendrier des vols
**Fonctionnalités** :
- Vue mensuelle/hebdomadaire/journalière
- Codes couleur par statut (Disponible, Réservé, Complet, Annulé)
- Drag & drop pour modification rapide
- Synchronisation calendriers externes (Google/Outlook)

**Interface calendrier** :
```
┌─────────────────────────────────────────────────────────┐
│ 📅 Mars 2024                           [Mois ▼] [+ Vol] │
├─────────────────────────────────────────────────────────┤
│ Lun  Mar  Mer  Jeu  Ven  Sam  Dim                      │
│                 1    2    3    4                        │
│  5    6    7    8    9   10   11                        │
│ 12   13   14   15   16   17   18                        │
│                     🟢              🔴                  │
│                CDG→NCE          LYS→CDG                │
│                14:30            16:00                   │
│                6/8 places       COMPLET                 │
│                                                         │
│ 🟢 Disponible  🟡 Réservé  🔴 Complet  ⚫ Annulé       │
└─────────────────────────────────────────────────────────┘
```

### 3. Gestion des réservations

#### 3.1 Validation des demandes
**Processus de validation** :
1. Réception notification temps réel
2. Vérification documents passagers
3. Validation/refus avec commentaire
4. Notification automatique client

**Interface de validation** :
```
┌─────────────────────────────────┐
│ 🔔 Nouvelle réservation         │
├─────────────────────────────────┤
│ Vol: CDG → NCE (15/03 14:30)   │
│ Client: Jean Dupont             │
│ Passagers: 2 adultes           │
│ Montant: 4200€                 │
│                                 │
│ 📋 Documents reçus:             │
│ ✅ Pièces d'identité           │
│ ✅ Informations passagers      │
│ ❌ Autorisation sortie territoire│
│                                 │
│ 💬 Commentaire:                │
│ [                               ]│
│                                 │
│ [❌ Refuser] [✅ Accepter]      │
└─────────────────────────────────┘
```

#### 3.2 Communication avec clients
**Messagerie intégrée** :
- Chat temps réel par réservation
- Templates de messages prédéfinis
- Notifications push/email automatiques
- Historique des échanges

### 4. Analytics et reporting

#### 4.1 Dashboard performance
**KPIs affichés** :
- Revenus mensuel/annuel
- Taux d'occupation moyen
- Nombre de vols publiés/réservés
- Note satisfaction moyenne
- Top destinations

**Graphiques** :
- Évolution revenus (courbe temporelle)
- Répartition vols par statut (camembert)
- Performance par aéronef (barres)
- Saisonnalité des réservations

## 🛬 Fonctionnalités Clients - Spécifications détaillées

### 1. Recherche et découverte

#### 1.1 Moteur de recherche avancé
**Critères de recherche** :
- Départ (ville/aéroport + rayon km)
- Arrivée (ville/aéroport + rayon km)
- Dates (date fixe ou ±3 jours)
- Nombre de passagers
- Type d'aéronef (turboprop, jet léger, moyen, lourd)
- Services souhaités (wifi, repas, etc.)
- Budget maximum

**Interface de recherche** :
```
┌─────────────────────────────────────────────────────────┐
│ 🔍 Rechercher un vol à vide                            │
├─────────────────────────────────────────────────────────┤
│ 🛫 Départ    🛬 Arrivée     📅 Date      👥 Passagers  │
│ [Paris  ▼]   [Nice    ▼]   [15/03/24]   [2 ▼]         │
│                                                         │
│ 🔧 Filtres avancés                                     │
│ ├─ Type aéronef: [Tous ▼]                             │
│ ├─ Budget max: [5000€]                                │
│ ├─ Services: □ Wifi □ Repas □ Bar                     │
│ └─ Horaires: [Matin] [AM] [Soir]                      │
│                                                         │
│ [🔍 Rechercher]                    [📍 Voir sur carte] │
└─────────────────────────────────────────────────────────┘
```

#### 1.2 Affichage des résultats
**Format des résultats** :
- Liste ou grille de cartes vol
- Tri par prix, heure, durée, note compagnie
- Filtres en temps réel
- Vue carte interactive

**Carte de vol** :
```
┌─────────────────────────────────────────────────────────┐
│ 🛫 CDG (15:30) ————————————————————— NCE (17:15) 🛬    │
│ Citation XLS+ • Air Prestige ⭐⭐⭐⭐⭐ (4.8)          │
│                                                         │
│ 👥 6 places • 1h45min • ✈️ Jet léger                   │
│ 🛜 Wifi • 🍽️ Collation • 🥂 Bar                       │
│                                                         │
│ 2400€ (-400€) 💰 Prix normal: 2800€                   │
│                                                         │
│ [📋 Détails] [❤️ Favoris] [📱 Partager] [✈️ Réserver] │
└─────────────────────────────────────────────────────────┘
```

#### 1.3 Alertes personnalisées
**Configuration d'alertes** :
- Critères de recherche sauvegardés
- Fréquence de notification (immédiate, quotidienne, hebdomadaire)
- Canaux (push, email, SMS)
- Prix maximum accepté

### 2. Processus de réservation

#### 2.1 Tunnel de réservation
**Étapes** :
1. **Sélection vol** : Validation disponibilité temps réel
2. **Informations passagers** : Identité, documents, besoins spéciaux
3. **Services additionnels** : Transfert, restauration premium, etc.
4. **Paiement** : CB, PayPal, Apple Pay, Google Pay
5. **Confirmation** : Envoi voucher et instructions

**Étape 2 - Informations passagers** :
```
┌─────────────────────────────────────────────────────────┐
│ 👥 Informations passagers (2/5)                        │
├─────────────────────────────────────────────────────────┤
│ Passager 1 (Réservant)                                 │
│ Prénom*: [Jean        ] Nom*: [Dupont       ]         │
│ Email*:  [jean.dupont@email.com            ]          │
│ Tél*:    [+33 6 12 34 56 78                ]          │
│ Naissance*: [📅 15/05/1975]                           │
│                                                         │
│ Passager 2                                             │
│ Prénom*: [Marie       ] Nom*: [Dupont       ]         │
│ Naissance*: [📅 20/08/1978]                           │
│                                                         │
│ 🆔 Documents (à fournir avant vol)                     │
│ □ Pièce d'identité valide                             │
│ □ Passeport (vols internationaux)                     │
│                                                         │
│ [⬅️ Retour] [Sauvegarder] [Continuer ➡️]               │
└─────────────────────────────────────────────────────────┘
```

#### 2.2 Paiement sécurisé
**Moyens de paiement** :
- Carte bancaire (Visa, Mastercard, Amex)
- PayPal
- Apple Pay / Google Pay
- Virement bancaire (vols > 10k€)
- Paiement en plusieurs fois (partenariat Alma/Klarna)

**Interface paiement** :
```
┌─────────────────────────────────────────────────────────┐
│ 💳 Paiement sécurisé (4/5)                             │
├─────────────────────────────────────────────────────────┤
│ Récapitulatif                                           │
│ Vol CDG → NCE (15/03 15:30)      2400€                │
│ Services additionnels              150€                │
│ Frais de service (3%)               76€                │
│ ————————————————————————————————————————                │
│ Total                             2626€                │
│                                                         │
│ Mode de paiement                                        │
│ ○ 💳 Carte bancaire                                    │
│ ○ 📱 PayPal        ○ 🍎 Apple Pay                     │
│                                                         │
│ [••••] [••••] [••••] [1234]                            │
│ MM/AA [12/26] CVC [•••]                                │
│                                                         │
│ 🔒 Paiement sécurisé par Stripe                       │
│                                                         │
│ [⬅️ Retour] [💳 Payer 2626€]                          │
└─────────────────────────────────────────────────────────┘
```

### 3. Gestion du compte

#### 3.1 Profil utilisateur
**Informations** :
- Données personnelles
- Préférences de voyage
- Documents numérisés
- Historique des vols
- Programme de fidélité

#### 3.2 Historique des réservations
**Statuts possibles** :
- ⏳ En attente de validation
- ✅ Confirmé
- ✈️ Effectué  
- ❌ Annulé
- 🔄 Remboursé

**Actions possibles** :
- Télécharger voucher/facture
- Modifier informations passagers
- Annuler (selon conditions)
- Évaluer l'expérience
- Réserver à nouveau

## ⚙️ Fonctionnalités Administrateur

### 1. Modération et validation

#### 1.1 Validation des compagnies
**Critères de validation** :
- Licence d'exploitation valide
- Assurance responsabilité civile
- Certification ISO/IOSA (bonus)
- Vérification des aéronefs
- Références et historique

#### 1.2 Gestion des litiges
**Types de litiges** :
- Annulation de vol
- Retard important
- Service non conforme
- Problème de facturation
- Réclamation qualité

**Processus de résolution** :
1. Ouverture ticket automatique
2. Investigation (logs, témoignages)
3. Médiation entre parties
4. Décision et compensation
5. Suivi satisfaction

### 2. Analytics globales

#### 2.1 Dashboard business
**Métriques globales** :
- GMV (Gross Merchandise Value)
- Commission perçue
- Nombre utilisateurs actifs
- Taux de conversion global
- NPS (Net Promoter Score)

#### 2.2 Monitoring technique
**Métriques techniques** :
- Uptime des services
- Temps de réponse API
- Taux d'erreur
- Utilisation ressources
- Incidents de sécurité

## 🔄 Workflows métier critiques

### 1. Publication d'un vol
```
Compagnie → Création vol → Validation automatique → Publication → Indexation recherche → Notifications alertes clients
```

### 2. Réservation client
```
Client → Recherche → Sélection → Réservation → Paiement → Notification compagnie → Validation → Confirmation → Préparation vol
```

### 3. Annulation
```
Initiateur → Demande annulation → Vérification conditions → Calcul remboursement → Traitement paiement → Notifications → Libération places
```

## 📱 Spécificités mobile

### 1. Fonctionnalités natives
- **Notifications push** : Alertes personnalisées, confirmations, rappels
- **Géolocalisation** : Recherche par proximité, navigation vers aéroport
- **Mode hors ligne** : Cache des vols favoris et réservations
- **Partage** : Partage de vols via réseaux sociaux, messages
- **Widget** : Prochains vols sur écran d'accueil

### 2. Optimisations UX mobile
- **Recherche vocale** : "Trouver un vol Paris-Nice demain"
- **Scanner documents** : OCR pour saisie automatique papiers d'identité
- **Touch ID/Face ID** : Authentification biométrique
- **Quick actions** : Raccourcis 3D Touch vers actions fréquentes

Cette spécification fonctionnelle garantit une expérience utilisateur optimale et un modèle business viable pour tous les acteurs de la plateforme Empty Legs.