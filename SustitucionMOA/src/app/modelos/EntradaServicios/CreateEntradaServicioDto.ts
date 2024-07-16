import { EntrySheet } from './EntrySheet';
import { Report } from './Report';
export interface CreateEntradaServicioDto {

    parametros: EntrySheet[];
    report: Report[];
    IdAdjuntos: string[];
}