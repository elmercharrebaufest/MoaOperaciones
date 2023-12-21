import { ComunicacionesModule } from './comunicaciones.module';

describe('ComunicacionesModule', () => {
  let comunicacionesModule: ComunicacionesModule;

  beforeEach(() => {
    comunicacionesModule = new ComunicacionesModule();
  });

  it('should create an instance', () => {
    expect(comunicacionesModule).toBeTruthy();
  });
});
