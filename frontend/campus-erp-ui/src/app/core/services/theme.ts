import { Injectable, signal } from '@angular/core';

export type AppTheme = 'light' | 'dark';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly themeKey = 'theme';

  readonly theme = signal<AppTheme>('light');

  initializeTheme(): void {
    const savedTheme = localStorage.getItem(this.themeKey) as AppTheme | null;

    if (savedTheme === 'light' || savedTheme === 'dark') {
      this.applyTheme(savedTheme);

      return;
    }

    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;

    this.applyTheme(prefersDark ? 'dark' : 'light');
  }

  toggleTheme(): void {
    this.applyTheme(this.theme() === 'dark' ? 'light' : 'dark');
  }

  setTheme(theme: AppTheme): void {
    this.applyTheme(theme);
  }

  isDarkMode(): boolean {
    return this.theme() === 'dark';
  }

  private applyTheme(theme: AppTheme): void {
    this.theme.set(theme);

    document.body.setAttribute('data-theme', theme);

    localStorage.setItem(this.themeKey, theme);
  }
}
