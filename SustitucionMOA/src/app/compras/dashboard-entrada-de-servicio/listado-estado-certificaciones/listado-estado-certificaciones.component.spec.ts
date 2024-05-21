import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListadoEstadoCertificacionesComponent } from './listado-estado-certificaciones.component';

describe('ListadoEstadoCertificacionesComponent', () => {
  let component: ListadoEstadoCertificacionesComponent;
  let fixture: ComponentFixture<ListadoEstadoCertificacionesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListadoEstadoCertificacionesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListadoEstadoCertificacionesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
