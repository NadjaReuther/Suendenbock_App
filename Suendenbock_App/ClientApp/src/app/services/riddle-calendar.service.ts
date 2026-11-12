import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import {
  RiddleDoor,
  UserRiddleProgress,
  HintResponse,
  CheckSolutionResponse
} from '../models/riddle-calendar.models';

@Injectable({
  providedIn: 'root'
})
export class RiddleCalendarService {
  private readonly baseUrl = '/api/RiddleCalendar';

  // State Management
  private doorsSubject = new BehaviorSubject<RiddleDoor[]>([]);
  private progressSubject = new BehaviorSubject<Map<number, UserRiddleProgress>>(new Map());

  public doors$ = this.doorsSubject.asObservable();
  public progress$ = this.progressSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadDoors();
    this.loadProgress();
  }

  /**
   * Lädt alle Rätsel-Türchen
   */
  loadDoors(): void {
    this.http.get<RiddleDoor[]>(`${this.baseUrl}/doors`)
      .subscribe({
        next: (doors) => this.doorsSubject.next(doors),
        error: (error) => console.error('Fehler beim Laden der Türchen:', error)
      });
  }

  /**
   * Lädt den User-Fortschritt
   */
  loadProgress(): void {
    this.http.get<UserRiddleProgress[]>(`${this.baseUrl}/progress`)
      .subscribe({
        next: (progress) => {
          const progressMap = new Map<number, UserRiddleProgress>();
          progress.forEach(p => progressMap.set(p.riddleDoorId, p));
          this.progressSubject.next(progressMap);
        },
        error: (error) => console.error('Fehler beim Laden des Fortschritts:', error)
      });
  }

  /**
   * Holt einen Hinweis für ein Türchen
   */
  getHint(doorId: number): Observable<HintResponse> {
    return this.http.post<HintResponse>(`${this.baseUrl}/hint`, { doorId })
      .pipe(
        tap(() => this.loadProgress()) // Aktualisiere Fortschritt
      );
  }

  /**
   * Prüft eine Lösungseingabe
   */
  checkSolution(doorId: number, answer: string): Observable<CheckSolutionResponse> {
    return this.http.post<CheckSolutionResponse>(`${this.baseUrl}/check`, { doorId, answer })
      .pipe(
        tap(() => this.loadProgress()) // Aktualisiere Fortschritt
      );
  }

  /**
   * Hilfsmethode: Holt Fortschritt für ein Türchen
   */
  getProgressForDoor(doorId: number): UserRiddleProgress | undefined {
    return this.progressSubject.value.get(doorId);
  }
}
