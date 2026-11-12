import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { RiddleCalendarService } from '../../services/riddle-calendar.service';
import { RiddleDoor, UserRiddleProgress } from '../../models/riddle-calendar.models';

@Component({
  selector: 'app-riddle-calendar',
  templateUrl: './riddle-calendar.component.html',
  styleUrl: './riddle-calendar.component.css'
})
export class RiddleCalendarComponent implements OnInit {
  doors$: Observable<RiddleDoor[]>;
  progress$: Observable<Map<number, UserRiddleProgress>>;

  selectedDoor: RiddleDoor | null = null;
  showModal = false;

  constructor(private riddleService: RiddleCalendarService) {
    this.doors$ = this.riddleService.doors$;
    this.progress$ = this.riddleService.progress$;
  }

  ngOnInit(): void {
    this.riddleService.loadDoors();
    this.riddleService.loadProgress();
  }

  /**
   * Öffnet ein Türchen
   */
  openDoor(door: RiddleDoor): void {
    // Prüfen ob Tag verfügbar ist
    if (!this.isDayAvailable(door.dayNumber)) {
      alert(`🔒 Dieses Türchen öffnet sich erst am ${door.dayNumber}. Dezember 2026!`);
      return;
    }

    this.selectedDoor = door;
    this.showModal = true;
  }

  /**
   * Prüft ob ein Tag verfügbar ist
   */
  isDayAvailable(dayNumber: number): boolean {
    // TESTING: Alle Türchen immer verfügbar
    return true;

    /* PRODUKTIONS-CODE - SPÄTER AKTIVIEREN:
    const today = new Date();
    const currentDay = today.getDate();
    const currentMonth = today.getMonth() + 1;
    const currentYear = today.getFullYear();

    // Nur im Dezember 2026
    if (currentMonth !== 12 || currentYear !== 2026) {
      return false;
    }

    // Tag muss erreicht oder überschritten sein
    return currentDay >= dayNumber;
    */
  }

  /**
   * Schließt das Modal
   */
  closeModal(): void {
    this.showModal = false;
    this.selectedDoor = null;
  }

  /**
   * Prüft ob ein Türchen gelöst wurde
   */
  isSolved(doorId: number, progressMap: Map<number, UserRiddleProgress> | null): boolean {
    if (!progressMap) return false;
    const progress = progressMap.get(doorId);
    return progress?.isSolved ?? false;
  }

  /**
   * Holt die Anzahl der verwendeten Hinweise
   */
  getHintsUsed(doorId: number, progressMap: Map<number, UserRiddleProgress> | null): number {
    if (!progressMap) return 0;
    const progress = progressMap.get(doorId);
    return progress?.hintsUsed ?? 0;
  }
}
