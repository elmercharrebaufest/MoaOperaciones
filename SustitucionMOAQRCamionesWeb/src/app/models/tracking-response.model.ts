import { TrackingData } from './tracking-data.model';

export interface TrackingResponse {
  resultado: boolean;
  mensaje: string;
  data?: TrackingData;
}