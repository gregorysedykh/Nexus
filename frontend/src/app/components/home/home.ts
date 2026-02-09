import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TableModule } from 'primeng/table';
import { ApiService, WordDto } from '../../services/api.service';

@Component({
  selector: 'app-home',
  imports: [TableModule],
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

  /**
   * On init, the component checks for a userId in the query parameters or sessionStorage and loads the user's words if a userId is found.
   */
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
}
