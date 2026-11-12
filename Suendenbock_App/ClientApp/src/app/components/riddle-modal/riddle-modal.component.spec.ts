import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RiddleModalComponent } from './riddle-modal.component';

describe('RiddleModalComponent', () => {
  let component: RiddleModalComponent;
  let fixture: ComponentFixture<RiddleModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RiddleModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RiddleModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
