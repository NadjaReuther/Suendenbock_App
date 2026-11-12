import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdventCalendarService } from '../../services/advent-calendar.service';
import { GetContentResponse } from '../../models/advent-calendar.models';

@Component({
  selector: 'app-door-detail',
  templateUrl: './door-detail.component.html',
  styleUrl: './door-detail.component.css'
})
export class DoorDetailComponent implements OnInit {
  dayNumber: number = 0;
  isLoading: boolean = true;
  errorMessage: string = '';
  content: GetContentResponse | null = null;

  // Choice-Türchen State
  selectedChoice: number | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private adventService: AdventCalendarService
  ) {}

  ngOnInit(): void {
    // Tag-Nummer aus Route-Parameter holen
    this.route.params.subscribe(params => {
      this.dayNumber = +params['day']; // '+' konvertiert zu Number
      this.loadContent();
    });
  }

  /**
   * Lädt den Inhalt des Türchens
   */
  loadContent(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.adventService.getContent(this.dayNumber).subscribe({
      next: (response) => {
        this.content = response;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Fehler beim Laden des Türchens';
        this.isLoading = false;
        console.error('Fehler beim Laden:', error);
      }
    });
  }

  /**
   * Behandelt die Auswahl bei Choice-Türchen
   */
  onChoiceSelected(choiceIndex: number): void {
    if (this.selectedChoice !== null) {
      return; // Bereits gewählt
    }

    this.isLoading = true;
    this.adventService.saveChoice({ day: this.dayNumber, choiceIndex }).subscribe({
      next: (response) => {
        this.selectedChoice = choiceIndex;
        // Content aktualisieren mit neuem Audio-Pfad
        if (this.content) {
          this.content.alreadyChosen = true;
          this.content.choiceIndex = choiceIndex;
          this.content.audioPath = response.audioPath;
          this.content.choiceName = choiceIndex === 0 ? 'Emma' : 'Kasimir';
        }
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Fehler beim Speichern der Auswahl';
        this.isLoading = false;
        console.error('Fehler beim Speichern:', error);
      }
    });
  }

  /**
   * Zurück zum Kalender
   */
  goBack(): void {
    this.router.navigate(['/kalender']);
  }
}
