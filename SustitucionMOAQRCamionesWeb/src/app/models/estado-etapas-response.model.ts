import { EstadoEtapas } from './estado-etapas.model';

export interface EstadoEtapasResponse {
  resultado: boolean;
  mensaje: string;
  data?: EstadoEtapas;
}