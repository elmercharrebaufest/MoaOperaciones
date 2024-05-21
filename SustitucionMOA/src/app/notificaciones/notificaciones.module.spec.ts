import { NotificacionesModule } from './notificaciones.module';

describe('NotificacionesModule', () => {
  let notificacionesModule: NotificacionesModule;

  beforeEach(() => {
    notificacionesModule = new NotificacionesModule();
  });

  it('should create an instance', () => {
    expect(notificacionesModule).toBeTruthy();
  });
});
