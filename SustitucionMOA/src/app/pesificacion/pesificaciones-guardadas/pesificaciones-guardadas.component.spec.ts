import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PesificacionesGuardadasComponent } from './pesificaciones-guardadas.component';

describe('PesificacionesGuardadasComponent', () => {
  let component: PesificacionesGuardadasComponent;
  let fixture: ComponentFixture<PesificacionesGuardadasComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PesificacionesGuardadasComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PesificacionesGuardadasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
