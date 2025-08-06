import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    TranslateModule
  ],
  template: `
    <div class="app-container">
      <main class="app-content">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Empty Legs - Plateforme de Vol Privé Luxe';

  constructor(private translate: TranslateService) {
    // Configuration de la langue par défaut
    this.translate.setDefaultLang('fr');
    
    // Récupération de la langue préférée depuis le localStorage
    const savedLanguage = localStorage.getItem('preferred-language');
    const browserLanguage = this.translate.getBrowserLang();
    const language = savedLanguage || browserLanguage || 'fr';
    
    // Utilisation de la langue appropriée
    this.translate.use(['fr', 'en'].includes(language) ? language : 'fr');
    
    // Mise à jour de l'attribut lang du document
    document.documentElement.lang = language;
  }
}