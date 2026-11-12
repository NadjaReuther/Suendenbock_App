import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AdventCalendarComponent } from './components/advent-calendar/advent-calendar.component';
import { DoorDetailComponent } from './components/door-detail/door-detail.component';
import { ChristmasOverviewComponent } from './components/christmas-overview/christmas-overview.component';
import { InstructionsComponent } from './components/instructions/instructions.component';
import { RiddleCalendarComponent } from './components/riddle-calendar/riddle-calendar.component';
import { RiddleModalComponent } from './components/riddle-modal/riddle-modal.component';

@NgModule({
  declarations: [
    AppComponent,
    AdventCalendarComponent,
    DoorDetailComponent,
    ChristmasOverviewComponent,
    InstructionsComponent,
    RiddleCalendarComponent,
    RiddleModalComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
