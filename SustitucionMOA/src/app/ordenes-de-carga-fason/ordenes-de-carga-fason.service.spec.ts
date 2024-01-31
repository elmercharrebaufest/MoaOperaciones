import { TestBed } from '@angular/core/testing';

import { OrdenesDeCargaFasonService } from './ordenes-de-carga-fason.service';

describe('OrdenesDeCargaFasonService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: OrdenesDeCargaFasonService = TestBed.get(OrdenesDeCargaFasonService);
    expect(service).toBeTruthy();
  });
});
