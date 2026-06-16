import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly themeKey = 'theme';

  initializeTheme(): void {
    const savedTheme = localStorage.getItem(this.themeKey);

    if (savedTheme) {
      document.body.setAttribute('data-theme', savedTheme);

      return;
    }

    document.body.setAttribute('data-theme', 'light');
  }

  toggleTheme(): void {
    const currentTheme = document.body.getAttribute('data-theme');

    const nextTheme = currentTheme === 'dark' ? 'light' : 'dark';

    document.body.setAttribute('data-theme', nextTheme);

    localStorage.setItem(this.themeKey, nextTheme);
  }

  isDarkMode(): boolean {
    return document.body.getAttribute('data-theme') === 'dark';
  }
}
