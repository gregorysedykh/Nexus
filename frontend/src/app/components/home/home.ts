import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {
  private readonly route = inject(ActivatedRoute);
  readonly userId = signal(
    this.route.snapshot.queryParamMap.get('userId') ?? sessionStorage.getItem('nexusUserId') ?? ''
  );
}
