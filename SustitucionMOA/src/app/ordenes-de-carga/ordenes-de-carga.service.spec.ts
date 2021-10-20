import { TestBed } from '@angular/core/testing';

import { OrdenesDeCargaService } from './ordenes-de-carga.service';

describe('OrdenesDeCargaService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: OrdenesDeCargaService = TestBed.get(OrdenesDeCargaService);
    expect(service).toBeTruthy();
  });
});
