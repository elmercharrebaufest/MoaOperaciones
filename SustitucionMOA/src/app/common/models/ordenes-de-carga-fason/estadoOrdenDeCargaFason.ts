import { EstadoOrdenDeCarga } from '../ordenes-de-carga/estadoOrdenDeCarga';
export enum EstadoOrdenDeCargaFason {
    Generada = 0,
    Pendiente = 1,
    Vencida = 2,
    Entregada = 3,
    SinEstado = 4,
    PendienteCompensacion = 5,
    PendienteContabilizacion = 6,
    // EdicionSolicitada = 7,
    // EdicionRechazada = 8,
    // AnulacionSolicitada = 9,
    Anulada = 10,
}
