import { EstadoEtapas } from '../models/estado-etapas.model';

const randomFila = () => Math.floor(Math.random() * 20) + 1;
const randomCaladoEstado = () => (Math.random() > 0.5 ? 'Grano con Humedad' : 'En Analisis');

export const MOCK_ESTADO_ETAPAS_1: EstadoEtapas = {
  datosAdicionales: {
    pre_calado_fila: String(randomFila()),
    post_calado_fila: '',
    calado_estado: '',
    pesada_bruto: 0,
    pesada_tara: 0,
    pesada_descargado: 0
  },
  etapas: [
    { nombre: 'Ingreso', fecha: new Date('2025-10-27T13:00:00.000Z'), tiempoEstimado: '30 minutos', estado: 'completado' },
    { nombre: 'Pre Calado', fecha: new Date('2025-10-27T13:30:00.000Z'), tiempoEstimado: '45 minutos', estado: 'en-proceso' },
    { nombre: 'Calado', fecha: new Date('2025-10-27T14:15:00.000Z'), tiempoEstimado: '120 minutos', estado: 'pendiente' },
    { nombre: 'Post Calado', fecha: new Date('2025-10-27T16:15:00.000Z'), tiempoEstimado: '60 minutos', estado: 'pendiente' },
    { nombre: 'Pesaje Bruto', fecha: new Date('2025-10-27T17:15:00.000Z'), tiempoEstimado: '120 minutos', estado: 'pendiente' },
    { nombre: 'Descarga', fecha: new Date('2025-10-27T19:15:00.000Z'), tiempoEstimado: '45 minutos', estado: 'pendiente' },
    { nombre: 'Cierre', fecha: new Date('2025-10-27T20:00:00.000Z'), tiempoEstimado: '30 minutos', estado: 'pendiente' }
  ]
};

export const MOCK_ESTADO_ETAPAS_2: EstadoEtapas = {
  datosAdicionales: {
    pre_calado_fila: MOCK_ESTADO_ETAPAS_1.datosAdicionales.pre_calado_fila,
    post_calado_fila: '',
    calado_estado: '',
    pesada_bruto: 0,
    pesada_tara: 0,
    pesada_descargado: 0
  },
  etapas: [
    { nombre: 'Ingreso', fecha: new Date('2025-10-27T13:00:00.000Z'), tiempoEstimado: '30 minutos', estado: 'completado' },
    { nombre: 'Pre Calado', fecha: new Date('2025-10-27T13:30:00.000Z'), tiempoEstimado: '45 minutos', estado: 'completado' },
    { nombre: 'Calado', fecha: new Date('2025-10-27T14:15:00.000Z'), tiempoEstimado: '120 minutos', estado: 'en-proceso' },
    { nombre: 'Post Calado', fecha: new Date('2025-10-27T16:15:00.000Z'), tiempoEstimado: '60 minutos', estado: 'pendiente' },
    { nombre: 'Pesaje Bruto', fecha: new Date('2025-10-27T17:15:00.000Z'), tiempoEstimado: '120 minutos', estado: 'pendiente' },
    { nombre: 'Descarga', fecha: new Date('2025-10-27T19:15:00.000Z'), tiempoEstimado: '45 minutos', estado: 'pendiente' },
    { nombre: 'Cierre', fecha: new Date('2025-10-27T20:00:00.000Z'), tiempoEstimado: '30 minutos', estado: 'pendiente' }
  ]
};

export const MOCK_ESTADO_ETAPAS_3: EstadoEtapas = {
  datosAdicionales: {
    pre_calado_fila: MOCK_ESTADO_ETAPAS_1.datosAdicionales.pre_calado_fila,
    post_calado_fila: String(randomFila()),
    calado_estado: randomCaladoEstado(),
    pesada_bruto: 0,
    pesada_tara: 0,
    pesada_descargado: 0
  },
  etapas: [
    { nombre: 'Ingreso', fecha: new Date('2025-10-27T13:00:00.000Z'), tiempoEstimado: '30 minutos', estado: 'completado' },
    { nombre: 'Pre Calado', fecha: new Date('2025-10-27T13:30:00.000Z'), tiempoEstimado: '45 minutos', estado: 'completado' },
    { nombre: 'Calado', fecha: new Date('2025-10-27T14:15:00.000Z'), tiempoEstimado: '120 minutos', estado: 'completado' },
    { nombre: 'Post Calado', fecha: new Date('2025-10-27T16:15:00.000Z'), tiempoEstimado: '60 minutos', estado: 'en-proceso' },
    { nombre: 'Pesaje Bruto', fecha: new Date('2025-10-27T17:15:00.000Z'), tiempoEstimado: '120 minutos', estado: 'pendiente' },
    { nombre: 'Descarga', fecha: new Date('2025-10-27T19:15:00.000Z'), tiempoEstimado: '45 minutos', estado: 'pendiente' },
    { nombre: 'Cierre', fecha: new Date('2025-10-27T20:00:00.000Z'), tiempoEstimado: '30 minutos', estado: 'pendiente' }
  ]
};

export const MOCK_ESTADO_ETAPAS_4: EstadoEtapas = {
  datosAdicionales: {
    pre_calado_fila: MOCK_ESTADO_ETAPAS_3.datosAdicionales.pre_calado_fila,
    post_calado_fila: MOCK_ESTADO_ETAPAS_3.datosAdicionales.post_calado_fila,
    calado_estado: MOCK_ESTADO_ETAPAS_3.datosAdicionales.calado_estado,
    pesada_bruto: 15000,
    pesada_tara: 0,
    pesada_descargado: 0
  },
  etapas: [
    { nombre: 'Ingreso', fecha: new Date('2025-10-27T13:00:00.000Z'), tiempoEstimado: '30 minutos', estado: 'completado' },
    { nombre: 'Pre Calado', fecha: new Date('2025-10-27T13:30:00.000Z'), tiempoEstimado: '45 minutos', estado: 'completado' },
    { nombre: 'Calado', fecha: new Date('2025-10-27T14:15:00.000Z'), tiempoEstimado: '120 minutos', estado: 'completado' },
    { nombre: 'Post Calado', fecha: new Date('2025-10-27T16:15:00.000Z'), tiempoEstimado: '60 minutos', estado: 'completado' },
    { nombre: 'Pesaje Bruto', fecha: new Date('2025-10-27T17:15:00.000Z'), tiempoEstimado: '120 minutos', estado: 'en-proceso' },
    { nombre: 'Descarga', fecha: new Date('2025-10-27T19:15:00.000Z'), tiempoEstimado: '45 minutos', estado: 'pendiente' },
    { nombre: 'Cierre', fecha: new Date('2025-10-27T20:00:00.000Z'), tiempoEstimado: '30 minutos', estado: 'pendiente' }
  ]
};

export const MOCK_ESTADO_ETAPAS_5: EstadoEtapas = {
  datosAdicionales: {
    pre_calado_fila: MOCK_ESTADO_ETAPAS_4.datosAdicionales.pre_calado_fila,
    post_calado_fila: MOCK_ESTADO_ETAPAS_4.datosAdicionales.post_calado_fila,
    calado_estado: MOCK_ESTADO_ETAPAS_4.datosAdicionales.calado_estado,
    pesada_bruto: 15000,
    pesada_tara: 9000,
    pesada_descargado: 6000
  },
  etapas: [
    { nombre: 'Ingreso', fecha: new Date('2025-10-27T13:00:00.000Z'), tiempoEstimado: '30 minutos', estado: 'completado' },
    { nombre: 'Pre Calado', fecha: new Date('2025-10-27T13:30:00.000Z'), tiempoEstimado: '45 minutos', estado: 'completado' },
    { nombre: 'Calado', fecha: new Date('2025-10-27T14:15:00.000Z'), tiempoEstimado: '120 minutos', estado: 'completado' },
    { nombre: 'Post Calado', fecha: new Date('2025-10-27T16:15:00.000Z'), tiempoEstimado: '60 minutos', estado: 'completado' },
    { nombre: 'Pesaje Bruto', fecha: new Date('2025-10-27T17:15:00.000Z'), tiempoEstimado: '120 minutos', estado: 'completado' },
    { nombre: 'Descarga', fecha: new Date('2025-10-27T19:15:00.000Z'), tiempoEstimado: '45 minutos', estado: 'en-proceso' },
    { nombre: 'Cierre', fecha: new Date('2025-10-27T20:00:00.000Z'), tiempoEstimado: '30 minutos', estado: 'pendiente' }
  ]
};

export const MOCK_ESTADO_ETAPAS_6: EstadoEtapas = {
  datosAdicionales: MOCK_ESTADO_ETAPAS_5.datosAdicionales,
  etapas: [
    { nombre: 'Ingreso', fecha: new Date('2025-10-27T13:00:00.000Z'), tiempoEstimado: '30 minutos', estado: 'completado' },
    { nombre: 'Pre Calado', fecha: new Date('2025-10-27T13:30:00.000Z'), tiempoEstimado: '45 minutos', estado: 'completado' },
    { nombre: 'Calado', fecha: new Date('2025-10-27T14:15:00.000Z'), tiempoEstimado: '120 minutos', estado: 'completado' },
    { nombre: 'Post Calado', fecha: new Date('2025-10-27T16:15:00.000Z'), tiempoEstimado: '60 minutos', estado: 'completado' },
    { nombre: 'Pesaje Bruto', fecha: new Date('2025-10-27T17:15:00.000Z'), tiempoEstimado: '120 minutos', estado: 'completado' },
    { nombre: 'Descarga', fecha: new Date('2025-10-27T19:15:00.000Z'), tiempoEstimado: '45 minutos', estado: 'completado' },
    { nombre: 'Cierre', fecha: new Date('2025-10-27T20:00:00.000Z'), tiempoEstimado: '30 minutos', estado: 'en-proceso' }
  ]
};

export const MOCK_ESTADO_ETAPAS_7: EstadoEtapas = {
  datosAdicionales: MOCK_ESTADO_ETAPAS_6.datosAdicionales,
  etapas: MOCK_ESTADO_ETAPAS_6.etapas.map(e => ({ ...e, estado: 'completado' }))
};
