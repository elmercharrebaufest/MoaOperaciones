import { ConsultaModule } from './consulta.module';

describe('ConsultaModule', () => {
  let consultaModule: ConsultaModule;

  beforeEach(() => {
    consultaModule = new ConsultaModule();
  });

  it('should create an instance', () => {
    expect(consultaModule).toBeTruthy();
  });
});
