export interface Etapa {
  nombre: string;
  fecha: string;
  tiempoEstimado: string;
  estado: 'completado' | 'en-proceso' | 'pendiente';
}

export interface DatosAdicionales {
  pre_calado_fila: string;
  post_calado_fila: string;
  calado_estado: string;
  pesada_bruto: number;
  pesada_tara: number;
  pesada_descargado: number;
}

export interface EstadoEtapas {
  datosAdicionales: DatosAdicionales;
  etapas: Etapa[];
}