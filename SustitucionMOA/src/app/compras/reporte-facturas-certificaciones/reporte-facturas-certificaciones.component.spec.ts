import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReporteFacturasCertificacionesComponent } from './reporte-facturas-certificaciones.component';

describe('ReporteFacturasCertificacionesComponent', () => {
  let component: ReporteFacturasCertificacionesComponent;
  let fixture: ComponentFixture<ReporteFacturasCertificacionesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReporteFacturasCertificacionesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReporteFacturasCertificacionesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
