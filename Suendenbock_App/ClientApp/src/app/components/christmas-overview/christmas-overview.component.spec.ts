import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChristmasOverviewComponent } from './christmas-overview.component';

describe('ChristmasOverviewComponent', () => {
  let component: ChristmasOverviewComponent;
  let fixture: ComponentFixture<ChristmasOverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ChristmasOverviewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChristmasOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
