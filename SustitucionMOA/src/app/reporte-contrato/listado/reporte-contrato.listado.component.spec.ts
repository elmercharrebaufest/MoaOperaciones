import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReporteContratoListado } from './reporte-contrato.listado.component';

describe('ContratosComponent', () => {
    let component: ReporteContratoListado;
    let fixture: ComponentFixture<ReporteContratoListado>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
        declarations: [ReporteContratoListado ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
      fixture = TestBed.createComponent(ReporteContratoListado);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
