import { NotificacionesRoutingModule } from './notificaciones-routing.module';

describe('NotificacionesRoutingModule', () => {
  let notificacionesRoutingModule: NotificacionesRoutingModule;

  beforeEach(() => {
    notificacionesRoutingModule = new NotificacionesRoutingModule();
  });

  it('should create an instance', () => {
    expect(notificacionesRoutingModule).toBeTruthy();
  });
});
