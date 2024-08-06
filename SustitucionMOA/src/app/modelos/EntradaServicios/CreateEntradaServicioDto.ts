import { EntrySheet } from './EntrySheet';
import { Report } from './Report';
export interface CreateEntradaServicioDto {

    Posiciones: EntrySheet[];
    report: Report[];
    IdAdjuntos: string[];
}