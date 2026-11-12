import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RiddleCalendarComponent } from './riddle-calendar.component';

describe('RiddleCalendarComponent', () => {
  let component: RiddleCalendarComponent;
  let fixture: ComponentFixture<RiddleCalendarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RiddleCalendarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RiddleCalendarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
