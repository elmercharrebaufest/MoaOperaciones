import { Rol } from './../app/common/models/rol';

export const notificacionMock = {
  Id: 1,
  Nombre: 'Notificación de ejemplo',
  FechaInicio: new Date('2023-08-15'),
  FechaFin: new Date('2023-08-31'),
  HoraInicio: 10,
  Habilitada: true,
  Borrada: false,
  LinkAdjunto: 'https://ejemplo.com/adjunto',
  Mensaje: 'Este es un mensaje de ejemplo',
  FiltroRoles: [
    { Id: '1', Nombre: 'Rol 1', checked: true } as Rol,
    { Id: '2', Nombre: 'Rol 2', checked: false } as Rol
  ],
  Prioridad: 1
};