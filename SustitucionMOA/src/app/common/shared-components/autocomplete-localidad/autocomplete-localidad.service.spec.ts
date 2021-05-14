import { TestBed } from '@angular/core/testing';

import { AutocompleteLocalidadService } from './autocomplete-localidad.service';

describe('AutocompleteLocalidadService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AutocompleteLocalidadService = TestBed.get(AutocompleteLocalidadService);
    expect(service).toBeTruthy();
  });
});
