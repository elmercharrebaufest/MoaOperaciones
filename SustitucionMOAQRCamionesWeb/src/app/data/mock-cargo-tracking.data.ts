import { CargoTrackingState } from '../models/cargo-tracking.model';

export const MOCK_CARGO_TRACKING_DATA: CargoTrackingState = {
  ctg: "098765123456",
  titularCartaPorte: "EL NORTE CORDOBES SRL",
  remitenteComercial: "SYNGENTA AGRO SOCIEDAD ANONIMA",
  remitenteComercialVtaPrim: "FYO ACOPIO S.A.",
  entregador: "MARTINO Y CIA SA",
  transportista: "BORLETTO LOG Y SERV SRL",
  chofer: {
    cuil: "20-23456789-6",
    tipoDocumento: "DNI",
    numeroDocumento: "23456789",
    extranjero: false,
    nombreApellido: "JAVIER MONTENEGRO"
  },
  camion: {
    patente: "EJE977",
    patenteAcoplado: "DWG633"
  },
  granos: {
    material: "Poroto de Soja"
  },
  etapas: [
    "Ingreso",
    "Pre Calado",
    "Calado",
    "Post Calado",
    "Pesaje Bruto",
    "Descarga",
    "Cierre"
  ],
  currentStageIndex: 0,
  stageDate: new Date('2025-09-19T12:30:00'),
  estimatedTime: 'aprox. 10 min'
};