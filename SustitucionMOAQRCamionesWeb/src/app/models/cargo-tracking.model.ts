export interface Etapa {
  nombre: string;
  fecha: string;
  tiempoEstimado: string;
  estado: 'completado' | 'en-proceso' | 'pendiente';
  icono: string;
}

export interface CargoTrackingData {
  ctg: string;
  fechaHoraIngreso: string;
  titularCartaPorte: string;
  remitenteComercial: string;
  remitenteComercialVtaPrim: string;
  entregador: string;
  transportista: string;
  camion: {
    patente: string;
    patenteAcoplado: string;
  };
  chofer: {
    cuil: string;
    tipoDocumento: string;
    numeroDocumento: string;
    extranjero: string;
    nombreApellido: string;
  };
  granos: {
    material: string;
  };
  etapas: Etapa[];
}