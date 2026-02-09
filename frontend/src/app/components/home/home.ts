import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { firstValueFrom } from 'rxjs';
import { ApiService, CreateWordDto, WordDto } from '../../services/api.service';

@Component({
  selector: 'app-home',
  imports: [FormsModule, ButtonModule, InputTextModule, TableModule],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home implements OnInit {
  private readonly route = inject(ActivatedRoute);
  readonly userId = signal(
    this.route.snapshot.queryParamMap.get('userId') ?? sessionStorage.getItem('nexusUserId') ?? ''
  );
  readonly apiService = inject(ApiService);
  readonly userWords = signal<WordDto[]>([]);
  readonly isSubmitting = signal(false);
  readonly addWordMessage = signal('');
  readonly addWordError = signal('');
  newWord: CreateWordDto = {
    term: '',
    languageCode: '',
  };

  ngOnInit(): void {
    const storedUserId = sessionStorage.getItem('nexusUserId');
    if (storedUserId) {
      this.userId.set(storedUserId);
    }
    this.loadUserWords();
  }

  /**
   * Loads the user's words from the API and updates the userWords signal. It also stores the userId in sessionStorage for persistence across sessions.
   */
  loadUserWords(): void {
    if (this.userId()) {
      sessionStorage.setItem('nexusUserId', this.userId());
      this.apiService.getUserWords(Number(this.userId())).subscribe(words => this.userWords.set(words));
    }
  }

  addWord(form: NgForm): void {
    const userId = Number(this.userId());
    const term = this.newWord.term.trim();
    const languageCode = this.newWord.languageCode.trim().toLowerCase();

    if (!Number.isInteger(userId) || userId <= 0) {
      this.addWordMessage.set('');
      this.addWordError.set('A valid user ID is required.');
      return;
    }

    if (!term || !languageCode) {
      this.addWordMessage.set('');
      this.addWordError.set('Term and language code are required.');
      return;
    }

    this.isSubmitting.set(true);
    this.addWordMessage.set('');
    this.addWordError.set('');

    void this.addWordForUser(userId, term, languageCode, form);
  }

  private async addWordForUser(
    userId: number,
    term: string,
    languageCode: string,
    form: NgForm
  ): Promise<void> {
    try {
      const createdWord = await firstValueFrom(this.apiService.createWord({ term, languageCode }));
      const wordId = createdWord.id;

      await firstValueFrom(this.apiService.addWordToUser(userId, wordId));
      const words = await firstValueFrom(this.apiService.getUserWords(userId));

      this.userWords.set(words);
      this.newWord = { term: '', languageCode: '' };
      form.resetForm(this.newWord);
      this.addWordError.set('');
      this.addWordMessage.set('Word added successfully.');

    } catch (error) {
      this.addWordMessage.set('');
      if (this.isConflictError(error)) {
        this.addWordError.set('This word is already assigned to your account.');
        return;
      }
      this.addWordError.set('Failed to add word. Please try again.');
    } finally {
      this.isSubmitting.set(false);
    }
  }

  private isConflictError(error: unknown): boolean {
    if (!(error instanceof HttpErrorResponse)) {
      return false;
    }

    if (error.status === 409) {
      return true;
    }

    if (typeof error.error !== 'object' || error.error === null) {
      return false;
    }

    const problemStatus = (error.error as { status?: unknown }).status;
    return problemStatus === 409;
  }
}
