import { ReporteContratoModule } from './reporte-contrato.module';

describe('ReporteContratoModule', () => {
  let reporteContratoModule: ReporteContratoModule;

  beforeEach(() => {
    reporteContratoModule = new ReporteContratoModule();
  });

  it('should create an instance', () => {
    expect(reporteContratoModule).toBeTruthy();
  });
});
