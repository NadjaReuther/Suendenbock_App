import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AdventCalendarService } from '../../services/advent-calendar.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-advent-calendar',
  templateUrl: './advent-calendar.component.html',
  styleUrl: './advent-calendar.component.css'
})
export class AdventCalendarComponent implements OnInit {
  // Observable Streams
  isGod$: Observable<boolean>;
  openedDoors$: Observable<number[]>;

  // Array für die 24 Türchen
  doors: number[] = Array.from({ length: 24 }, (_, i) => i + 1);

  constructor(
    private adventService: AdventCalendarService,
    private router: Router
  ) {
    this.isGod$ = this.adventService.isGod$;
    this.openedDoors$ = this.adventService.openedDoors$;
  }

  ngOnInit(): void {
    // Zufällige Reihenfolge der Türchen (wie in Razor-Version)
    this.shuffleDoors();
  }

  /**
   * Mischt die Türchen-Reihenfolge zufällig
   */
  private shuffleDoors(): void {
    for (let i = this.doors.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [this.doors[i], this.doors[j]] = [this.doors[j], this.doors[i]];
    }
  }

  /**
   * Behandelt den Klick auf ein Türchen
   */
  onDoorClick(dayNumber: number): void {
    // Prüfen ob Tag verfügbar ist
    if (!this.adventService.isDayAvailable(dayNumber)) {
      this.showLockedMessage(dayNumber);
      return;
    }

    // Zur Türchen-Detail-Seite navigieren (innerhalb Angular)
    this.router.navigate(['/tuer', dayNumber]);
  }

  /**
   * Zeigt Nachricht für gesperrte Türchen
   */
  private showLockedMessage(dayNumber: number): void {
    // TODO: Modal/Dialog implementieren
    alert(`🔒 Dieses Türchen öffnet sich erst am ${dayNumber}. Dezember!`);
  }

  /**
   * Prüft ob ein Türchen bereits geöffnet wurde
   */
  isDoorOpened(dayNumber: number, openedDoors: number[] | null): boolean {
    return openedDoors ? openedDoors.includes(dayNumber) : false;
  }

  /**
   * Gibt den SVG-Pfad für ein Türchen zurück (Kalenderzahlen wie in Razor-Version)
   */
  getDoorImagePath(dayNumber: number): string {
    return `/assets/spieler/Weihnachten/Kalenderzahlen/${dayNumber}.svg`;
  }

  /**
   * Navigiert zurück zur Weihnachten-Übersicht
   */
  goToOverview(): void {
    this.router.navigate(['/weihnachten']);
  }
}
