import { TestBed } from '@angular/core/testing';

import { ReporteContratoService } from './reporte-contrato.service';

describe('ReporteContratoService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ReporteContratoService = TestBed.get(ReporteContratoService);
    expect(service).toBeTruthy();
  });
});
