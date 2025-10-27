import { DatosAdicionales, Etapa } from "./estado-etapas.model";

export interface TrackingData {
  workflow: string;
  ctg: string;
  fechaHoraIngreso: string;
  titularCartaPorte: string;
  remitenteComercial: string;
  remitenteComercialVtaPrim: string;
  entregador: string;
  transportista: string; 
  material: string;
  rechazado: string;
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
  datosAdicionales: DatosAdicionales;
  etapas: Etapa[];
}