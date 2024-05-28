import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListadoDashboardCertificacionDeServiciosComponent } from './listado-dashboard-certificacion-de-servicios.component';

describe('ListadoDashboardCertificacionDeServiciosComponent', () => {
  let component: ListadoDashboardCertificacionDeServiciosComponent;
  let fixture: ComponentFixture<ListadoDashboardCertificacionDeServiciosComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListadoDashboardCertificacionDeServiciosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListadoDashboardCertificacionDeServiciosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
