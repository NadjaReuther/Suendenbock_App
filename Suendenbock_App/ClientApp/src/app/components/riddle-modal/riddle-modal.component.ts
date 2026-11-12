import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { RiddleDoor } from '../../models/riddle-calendar.models';
import { RiddleCalendarService } from '../../services/riddle-calendar.service';

@Component({
  selector: 'app-riddle-modal',
  templateUrl: './riddle-modal.component.html',
  styleUrl: './riddle-modal.component.css'
})
export class RiddleModalComponent implements OnInit {
  @Input() door!: RiddleDoor;
  @Output() close = new EventEmitter<void>();

  userAnswer = '';
  hints: string[] = [];
  isCorrect: boolean | null = null;
  isLoading = false;
  attemptCount = 0;
  hintsUsed = 0;
  isSolved = false;

  constructor(private riddleService: RiddleCalendarService) {}

  ngOnInit(): void {
    const progress = this.riddleService.getProgressForDoor(this.door.id);
    if (progress) {
      this.hintsUsed = progress.hintsUsed;
      this.attemptCount = progress.attemptCount;
      this.isSolved = progress.isSolved;
    }
  }

  getHint(): void {
    if (this.hintsUsed >= 3 || this.isSolved) return;

    this.isLoading = true;
    this.riddleService.getHint(this.door.id).subscribe({
      next: (response) => {
        this.hints.push(response.hint);
        this.hintsUsed = response.hintsUsed;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Fehler:', error);
        this.isLoading = false;
      }
    });
  }

  checkAnswer(): void {
    if (!this.userAnswer.trim() || this.isSolved) return;

    this.isLoading = true;
    this.riddleService.checkSolution(this.door.id, this.userAnswer).subscribe({
      next: (response) => {
        this.isCorrect = response.correct;
        this.attemptCount = response.attemptCount;
        this.isSolved = response.isSolved;
        this.isLoading = false;

        if (response.correct) {
          setTimeout(() => this.close.emit(), 2000);
        }
      },
      error: (error) => {
        console.error('Fehler:', error);
        this.isLoading = false;
      }
    });
  }

  closeModal(): void {
    this.close.emit();
  }
}
