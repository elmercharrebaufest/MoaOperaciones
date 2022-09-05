import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FinalizarSolpComponent } from './finalizar-solp.component';

describe('FinalizarSolpComponent', () => {
  let component: FinalizarSolpComponent;
  let fixture: ComponentFixture<FinalizarSolpComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FinalizarSolpComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FinalizarSolpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
