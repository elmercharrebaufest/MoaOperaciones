import { TestBed } from '@angular/core/testing';

import { ComunicacionesService } from './comunicaciones.service';

describe('ComunicacionesService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ComunicacionesService = TestBed.get(ComunicacionesService);
    expect(service).toBeTruthy();
  });
});
