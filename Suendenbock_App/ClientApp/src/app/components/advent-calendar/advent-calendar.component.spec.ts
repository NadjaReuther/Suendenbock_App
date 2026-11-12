import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdventCalendarComponent } from './advent-calendar.component';

describe('AdventCalendarComponent', () => {
  let component: AdventCalendarComponent;
  let fixture: ComponentFixture<AdventCalendarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AdventCalendarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdventCalendarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
