import { TrackingData } from '../models/tracking-data.model';

export const MOCK_CARGO_DATA: TrackingData = {
  workflow: 'Granos',
  ctg: '098765123456',
  fechaHoraIngreso: new Date('2025-10-27T12:45:00.000Z'),
  titularCartaPorte: 'EL NORTE CORDOBÉS SRL',
  remitenteComercial: 'SYNGENTA AGRO SOCIEDAD ANONIMA',
  remitenteComercialVtaPrim: 'FYO ACOPIO S.A.',
  entregador: 'MARTINO Y CIA SA',
  transportista: 'BORLETTO LOG Y SERV SRL',
  material: 'Poroto de soja',
  rechazado: false,
  camion: {
    patente: 'EJE977',
    patenteAcoplado: 'DWG633'
  },
  chofer: {
    cuil: '20-23456789-6',
    tipoDocumento: 'DNI',
    numeroDocumento: '23.456.789',
    extranjero: 'NO',
    nombreApellido: 'JAVIER MONTENEGRO'
  },
  datosAdicionales: {
    pre_calado_fila: '',
    post_calado_fila: '',
    calado_estado: '',
    pesada_bruto: 0,
    pesada_tara: 0,
    pesada_descargado: 0
  },
  etapas: [
    {
      nombre: 'Ingreso',
      fecha: new Date('2025-10-27T00:00:00.000Z'),
      tiempoEstimado: '30 minutos',
      estado: 'pendiente'
    },
    {
      nombre: 'Pre Calado',
      fecha: new Date('2025-10-27T00:00:00.000Z'),
      tiempoEstimado: '45 minutos',
      estado: 'pendiente'
    },
    {
      nombre: 'Calado',
      fecha: new Date('2025-10-27T00:00:00.000Z'),
      tiempoEstimado: '120 minutos',
      estado: 'pendiente'
    },
    {
      nombre: 'Post Calado',
      fecha: new Date('2025-10-27T00:00:00.000Z'),
      tiempoEstimado: '60 minutos',
      estado: 'pendiente'
    },
    {
      nombre: 'Pesaje Bruto',
      fecha: new Date('2025-10-27T00:00:00.000Z'),
      tiempoEstimado: '120 minutos',
      estado: 'pendiente'
    },
    {
      nombre: 'Descarga',
      fecha: new Date('2025-10-27T00:00:00.000Z'),
      tiempoEstimado: '45 minutos',
      estado: 'pendiente'
    },
    {
      nombre: 'Cierre',
      fecha: new Date('2025-10-27T00:00:00.000Z'),
      tiempoEstimado: '30 minutos',
      estado: 'pendiente'
    }
  ]
};
