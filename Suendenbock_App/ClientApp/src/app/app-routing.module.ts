import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChristmasOverviewComponent } from './components/christmas-overview/christmas-overview.component';
import { InstructionsComponent } from './components/instructions/instructions.component';
import { AdventCalendarComponent } from './components/advent-calendar/advent-calendar.component';
import { DoorDetailComponent } from './components/door-detail/door-detail.component';
import { RiddleCalendarComponent } from './components/riddle-calendar/riddle-calendar.component';

const routes: Routes = [
  { path: '', redirectTo: '/weihnachten', pathMatch: 'full' },
  { path: 'weihnachten', component: ChristmasOverviewComponent },
  { path: 'anleitung', component: InstructionsComponent },
  { path: 'kalender', component: AdventCalendarComponent },
  { path: 'tuer/:day', component: DoorDetailComponent },
  { path: 'raetsel', component: RiddleCalendarComponent },
  // Wildcard Route - fängt alle unbekannten Routen ab
  { path: '**', redirectTo: '/weihnachten' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
