import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-flashcard',
  imports: [CardModule, ButtonModule],
  templateUrl: './flashcard.html',
  styleUrl: './flashcard.scss',
})
export class Flashcard implements OnChanges {
  @Input({ required: true }) term = '';
  @Output() gotIt = new EventEmitter<void>();
  @Output() forgotIt = new EventEmitter<void>();
  isFlipped = false;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['term']) {
      this.isFlipped = false;
    }
  }

  flipToBack(): void {
    this.isFlipped = true;
  }

  flipToFront(): void {
    this.isFlipped = false;
  }

  onGotIt(): void {
    this.isFlipped = false;
    this.gotIt.emit();
  }

  onForgotIt(): void {
    this.isFlipped = false;
    this.forgotIt.emit();
  }
}
