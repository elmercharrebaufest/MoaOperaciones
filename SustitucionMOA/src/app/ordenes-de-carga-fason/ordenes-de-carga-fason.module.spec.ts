import { OrdenesDeCargaFasonModule } from './ordenes-de-carga-fason.module';

describe('OrdenesDeCargaFasonModule', () => {
  let ordenesDeCargaFasonModule: OrdenesDeCargaFasonModule;

  beforeEach(() => {
    ordenesDeCargaFasonModule = new OrdenesDeCargaFasonModule();
  });

  it('should create an instance', () => {
    expect(ordenesDeCargaFasonModule).toBeTruthy();
  });
});
