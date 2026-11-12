import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import {
  GetContentResponse,
  SaveChoiceRequest,
  SaveChoiceResponse,
  IsGodResponse,
  OpenedDoorsResponse
} from '../models/advent-calendar.models';

@Injectable({
  providedIn: 'root'
})
export class AdventCalendarService {
  private readonly baseUrl = '/AdventCalendar';

  // State management
  private isGodSubject = new BehaviorSubject<boolean>(false);
  private openedDoorsSubject = new BehaviorSubject<number[]>([]);

  public isGod$ = this.isGodSubject.asObservable();
  public openedDoors$ = this.openedDoorsSubject.asObservable();

  constructor(private http: HttpClient) {
    // Automatisches Laden beim Service-Start
    this.loadIsGod();
    this.loadOpenedDoors();
  }

  /**
   * Prüft ob der aktuelle User die Gott-Rolle hat
   */
  loadIsGod(): void {
    this.http.get<IsGodResponse>(`${this.baseUrl}/IsGod`)
      .subscribe({
        next: (response) => this.isGodSubject.next(response.isGod),
        error: (error) => {
          console.error('Fehler beim Laden des God-Status:', error);
          this.isGodSubject.next(false);
        }
      });
  }

  /**
   * Lädt alle geöffneten Türchen für den aktuellen User
   */
  loadOpenedDoors(): void {
    this.http.get<OpenedDoorsResponse>(`${this.baseUrl}/GetOpenedDoors`)
      .subscribe({
        next: (response) => this.openedDoorsSubject.next(response.openedDoors),
        error: (error) => {
          console.error('Fehler beim Laden der geöffneten Türchen:', error);
          this.openedDoorsSubject.next([]);
        }
      });
  }

  /**
   * Lädt den Inhalt eines Türchens
   */
  getContent(day: number): Observable<GetContentResponse> {
    return this.http.get<GetContentResponse>(`${this.baseUrl}/GetContent?day=${day}`)
      .pipe(
        tap(() => {
          // Nach erfolgreichem Laden: Geöffnete Türchen aktualisieren
          const currentDoors = this.openedDoorsSubject.value;
          if (!currentDoors.includes(day)) {
            this.openedDoorsSubject.next([...currentDoors, day]);
          }
        })
      );
  }

  /**
   * Speichert die User-Auswahl für Choice-Türchen
   */
  saveChoice(request: SaveChoiceRequest): Observable<SaveChoiceResponse> {
    // Anti-Forgery Token holen
    const token = this.getAntiForgeryToken();

    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'RequestVerificationToken': token
    });

    return this.http.post<SaveChoiceResponse>(
      `${this.baseUrl}/SaveChoice`,
      request,
      { headers }
    );
  }

  /**
   * Hilfsmethode: Holt das Anti-Forgery Token aus dem DOM
   */
  private getAntiForgeryToken(): string {
    const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement;
    return tokenElement ? tokenElement.value : '';
  }

  /**
   * Prüft ob ein Tag verfügbar ist (für UI-Logik)
   */
  isDayAvailable(dayNumber: number): boolean {
    // God-User können alle Türchen öffnen
    if (this.isGodSubject.value) {
      return true;
    }

    // TESTING: Alle Türchen immer verfügbar
    return true;

    /* PRODUKTIONS-CODE - SPÄTER AKTIVIEREN:
    const today = new Date();
    const currentDay = today.getDate();
    const currentMonth = today.getMonth() + 1;

    // Nur im Dezember
    if (currentMonth !== 12) {
      return false;
    }

    // Tag muss erreicht oder überschritten sein
    return currentDay >= dayNumber;
    */
  }

  /**
   * Getter für aktuelle Werte (synchron)
   */
  get isGod(): boolean {
    return this.isGodSubject.value;
  }

  get openedDoors(): number[] {
    return this.openedDoorsSubject.value;
  }
}
