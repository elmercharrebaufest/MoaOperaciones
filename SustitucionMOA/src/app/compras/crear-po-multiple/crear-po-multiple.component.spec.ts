import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CrearPoMultipleComponent } from './crear-po-multiple.component';

describe('CrearPoMultipleComponent', () => {
  let component: CrearPoMultipleComponent;
  let fixture: ComponentFixture<CrearPoMultipleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CrearPoMultipleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CrearPoMultipleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
