import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ApiService } from '../../services/api.service';

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
  private readonly api = inject(ApiService);

  login(): void {
    // remove leading and trailing whitespace from the user ID
    const trimmedUserId = this.userId.trim();

    // if no user ID, provided, set an error message and return
    if (!trimmedUserId) {
      this.errorMessage.set('User ID is required.');
      return;
    }
    
    // if user ID is not a positive integer, set an error message and return
    const id = Number(trimmedUserId);
    if (!Number.isInteger(id) || id <= 0) {
      this.errorMessage.set('User ID must be a positive number.');
      return;
    }

    this.errorMessage.set('');
    this.api.getUser(id).subscribe({
      next: (user) => {
        sessionStorage.setItem('nexusUserId', String(user.id));
        void this.router.navigate(['/home'], { queryParams: { userId: user.id } });
      },
      error: (err) => {
        this.errorMessage.set(err.status === 404 ? 'User not found.' : 'Failed to reach API.');
      },
    });
  }
}
