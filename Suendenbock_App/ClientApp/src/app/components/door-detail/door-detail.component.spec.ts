import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DoorDetailComponent } from './door-detail.component';

describe('DoorDetailComponent', () => {
  let component: DoorDetailComponent;
  let fixture: ComponentFixture<DoorDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DoorDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DoorDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
