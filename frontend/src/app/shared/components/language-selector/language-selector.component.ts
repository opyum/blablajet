import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { TranslocoService } from '@ngneat/transloco';

@Component({
  selector: 'app-language-selector',
  standalone: true,
  imports: [CommonModule, MatSelectModule, MatFormFieldModule],
  template: `
    <mat-form-field appearance="outline" class="language-selector">
      <mat-select [value]="currentLang" (selectionChange)="changeLanguage($event.value)">
        <mat-option value="fr">
          <span class="flag">🇫🇷</span> Français
        </mat-option>
        <mat-option value="en">
          <span class="flag">🇬🇧</span> English
        </mat-option>
      </mat-select>
    </mat-form-field>
  `,
  styles: [`
    .language-selector {
      width: 120px;
    }
    
    :host ::ng-deep .language-selector .mat-mdc-form-field-wrapper {
      padding: 0;
    }
    
    :host ::ng-deep .language-selector .mat-mdc-form-field-infix {
      padding: 8px 12px;
      min-height: auto;
      border: none;
    }
    
    :host ::ng-deep .language-selector .mat-mdc-form-field-subscript-wrapper {
      display: none;
    }
    
    :host ::ng-deep .language-selector .mat-mdc-select-value {
      color: white;
    }
    
    :host ::ng-deep .language-selector .mat-mdc-select-arrow {
      color: white;
    }
    
    :host ::ng-deep .language-selector .mdc-notched-outline__leading,
    :host ::ng-deep .language-selector .mdc-notched-outline__notch,
    :host ::ng-deep .language-selector .mdc-notched-outline__trailing {
      border-color: rgba(255, 255, 255, 0.3) !important;
    }
    
    :host ::ng-deep .language-selector:hover .mdc-notched-outline__leading,
    :host ::ng-deep .language-selector:hover .mdc-notched-outline__notch,
    :host ::ng-deep .language-selector:hover .mdc-notched-outline__trailing {
      border-color: rgba(255, 255, 255, 0.5) !important;
    }
    
    .flag {
      margin-right: 8px;
      font-size: 1.2em;
    }
  `]
})
export class LanguageSelectorComponent {
  private translocoService = inject(TranslocoService);

  get currentLang(): string {
    return this.translocoService.getActiveLang();
  }

  changeLanguage(lang: string): void {
    this.translocoService.setActiveLang(lang);
    localStorage.setItem('preferredLanguage', lang);
  }
}