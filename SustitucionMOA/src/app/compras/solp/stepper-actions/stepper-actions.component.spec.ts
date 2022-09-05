import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StepperActionsComponent } from './stepper-actions.component';

describe('StepperActionsComponent', () => {
  let component: StepperActionsComponent;
  let fixture: ComponentFixture<StepperActionsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StepperActionsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StepperActionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
