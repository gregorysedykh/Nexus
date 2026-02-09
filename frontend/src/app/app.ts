import { DOCUMENT } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterOutlet } from '@angular/router';
import { ToggleSwitchModule } from 'primeng/toggleswitch';

type ThemeMode = 'light' | 'dark';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, FormsModule, ToggleSwitchModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  readonly theme = signal<ThemeMode>(this.getInitialTheme());
  private readonly document = inject(DOCUMENT);

  constructor() {
    this.applyTheme(this.theme());
  }

  setTheme(mode: ThemeMode): void {
    this.theme.set(mode);
    this.applyTheme(mode);
    this.persistTheme(mode);
  }

  private applyTheme(mode: ThemeMode): void {
    this.document.documentElement.classList.toggle('app-dark', mode === 'dark');
  }

  private getInitialTheme(): ThemeMode {
    try {
      return localStorage.getItem('nexus-theme') === 'dark' ? 'dark' : 'light';
    } catch {
      return 'light';
    }
  }

  private persistTheme(mode: ThemeMode): void {
    try {
      localStorage.setItem('nexus-theme', mode);
    } catch {
    }
  }
}
