import { ListadoNovedadesModule } from './listado-novedades.module';

describe('ListadoNovedadesModule', () => {
  let listadoNovedadesModule: ListadoNovedadesModule;

  beforeEach(() => {
    listadoNovedadesModule = new ListadoNovedadesModule();
  });

  it('should create an instance', () => {
    expect(listadoNovedadesModule).toBeTruthy();
  });
});
