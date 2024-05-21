import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FiltroDashboardCertificacionDeServiciosComponent } from './filtro-dashboard-certificacion-de-servicios.component';

describe('FiltroDashboardCertificacionDeServiciosComponent', () => {
  let component: FiltroDashboardCertificacionDeServiciosComponent;
  let fixture: ComponentFixture<FiltroDashboardCertificacionDeServiciosComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FiltroDashboardCertificacionDeServiciosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FiltroDashboardCertificacionDeServiciosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
