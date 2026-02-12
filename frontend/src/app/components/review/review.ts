import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { firstValueFrom } from 'rxjs';
import { ApiService, WordDto } from '../../services/api.service';
import { Flashcard } from '../flashcard/flashcard';

@Component({
  selector: 'app-review',
  imports: [ButtonModule, Flashcard],
  templateUrl: './review.html',
  styleUrl: './review.scss',
})
export class Review implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly api = inject(ApiService);
  readonly userId = signal(
    this.route.snapshot.queryParamMap.get('userId') ?? sessionStorage.getItem('nexusUserId') ?? ''
  );
  readonly words = signal<WordDto[]>([]);
  readonly currentIndex = signal(0);
  readonly isLoading = signal(false);
  readonly errorMessage = signal('');
  readonly currentWord = computed(() => this.words()[this.currentIndex()] ?? null);
  readonly isFirstWord = computed(() => this.currentIndex() === 0);
  readonly isLastWord = computed(
    () => this.words().length === 0 || this.currentIndex() === this.words().length - 1
  );

  ngOnInit(): void {
    void this.loadWords();
  }

  previousWord(): void {
    if (!this.isFirstWord()) {
      this.currentIndex.update(index => index - 1);
    }
  }

  nextWord(): void {
    if (!this.isLastWord()) {
      this.currentIndex.update(index => index + 1);
    }
  }

  restartReview(): void {
    this.currentIndex.set(0);
  }

  goHome(): void {
    void this.router.navigate(['/home'], {
      queryParams: { userId: this.userId() },
    });
  }

  private async loadWords(): Promise<void> {
    const userId = Number(this.userId());
    if (!Number.isInteger(userId) || userId <= 0) {
      this.errorMessage.set('A valid user ID is required.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    try {
      const words = await firstValueFrom(this.api.getUserWords(userId));
      this.words.set(words);
      this.currentIndex.set(0);
    } catch {
      this.errorMessage.set('Failed to load words. Please try again.');
    } finally {
      this.isLoading.set(false);
    }
  }
}
