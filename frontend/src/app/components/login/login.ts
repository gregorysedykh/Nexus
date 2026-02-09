import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-login',
  imports: [FormsModule, ButtonModule, InputTextModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  userId = '';
  readonly errorMessage = signal('');
  private readonly router = inject(Router);

  login(): void {
    const trimmedUserId = this.userId.trim();

    if (!trimmedUserId) {
      this.errorMessage.set('User ID is required.');
      return;
    }

    this.errorMessage.set('');
    sessionStorage.setItem('nexusUserId', trimmedUserId);
    void this.router.navigate(['/home'], { queryParams: { userId: trimmedUserId } });
  }
}
