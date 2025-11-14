export interface Etapa {
  nombre: string;
  fecha: Date;
  tiempoEstimado: string;
  estado: 'completado' | 'en-proceso' | 'pendiente';
}

export interface DatosAdicionales {
  preCaladoFila: string | null;      
  postCaladoFila: string | null;     
  caladoEstado: string | null;       
  pesadaBruto: number | null;        
  pesadaTara: number | null;         
  pesadaDescargado: number | null;   
}

export interface EstadoEtapas {
  datosAdicionales: DatosAdicionales;
  etapas: Etapa[];
}