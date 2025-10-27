import { CargoTrackingData } from '../models/cargo-tracking.model';

export const MOCK_CARGO_DATA: CargoTrackingData = {
  ctg: '098765123456',
  fechaHoraIngreso: '19/09/2025 a las 12:45h',
  titularCartaPorte: 'EL NORTE CORDOBÉS SRL',
  remitenteComercial: 'SYNGENTA AGRO SOCIEDAD ANONIMA',
  remitenteComercialVtaPrim: 'FYO ACOPIO S.A.',
  entregador: 'MARTINO Y CIA SA',
  transportista: 'BORLETTO LOG Y SERV SRL',
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
  granos: {
    material: 'Poroto de soja'
  },
  etapas: [
    {
      nombre: 'Ingreso',
      fecha: '19/09/2025 12:30h',
      tiempoEstimado: '10 minutos',
      estado: 'en-proceso',
      icono: 'ingreso'
    },
    {
      nombre: 'Pre-calado',
      fecha: '',
      tiempoEstimado: '15 minutos',
      estado: 'pendiente',
      icono: 'pre-calado'
    },
    {
      nombre: 'Calado',
      fecha: '',
      tiempoEstimado: '20 minutos',
      estado: 'pendiente',
      icono: 'calado'
    },
    {
      nombre: 'Post-calado',
      fecha: '',
      tiempoEstimado: '15 minutos',
      estado: 'pendiente',
      icono: 'post-calado'
    },
    {
      nombre: 'Pesaje inicial',
      fecha: '',
      tiempoEstimado: '10 minutos',
      estado: 'pendiente',
      icono: 'pesaje'
    },
    {
      nombre: 'Descarga',
      fecha: '',
      tiempoEstimado: '30 minutos',
      estado: 'pendiente',
      icono: 'descarga'
    },
    {
      nombre: 'Cierre',
      fecha: '',
      tiempoEstimado: '5 minutos',
      estado: 'pendiente',
      icono: 'cierre'
    }
  ]
};