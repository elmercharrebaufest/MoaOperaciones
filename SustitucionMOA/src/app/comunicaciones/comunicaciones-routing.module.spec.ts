import { ComunicacionesRoutingModule } from './comunicaciones-routing.module';

describe('ComunicacionesRoutingModule', () => {
  let comunicacionesRoutingModule: ComunicacionesRoutingModule;

  beforeEach(() => {
    comunicacionesRoutingModule = new ComunicacionesRoutingModule();
  });

  it('should create an instance', () => {
    expect(comunicacionesRoutingModule).toBeTruthy();
  });
});
