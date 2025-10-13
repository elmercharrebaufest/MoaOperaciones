export interface Chofer {
  cuil: string;
  tipoDocumento: string;
  numeroDocumento: string;
  extranjero: boolean;
  nombreApellido: string;
}

export interface Camion {
  patente: string;
  patenteAcoplado: string;
}

export interface Granos {
  material: string;
}

export interface Etapa {
  nombre: string;
  fecha: Date;
  tiempoEstimado: number;
}

export interface CargoTrackingInfo {
  ctg: string;
  titularCartaPorte: string;
  remitenteComercial: string;
  remitenteComercialVtaPrim: string;
  entregador: string;
  transportista: string;
  chofer: Chofer;
  camion: Camion;
  granos: Granos;
  etapas: string[];
}

// Extended model for UI state
export interface CargoTrackingState extends CargoTrackingInfo {
  currentStageIndex: number;
  stageDate: Date;
  estimatedTime: string;
}