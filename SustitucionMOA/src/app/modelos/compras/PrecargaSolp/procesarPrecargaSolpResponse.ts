import { SolpPosicionPrecargada } from "./solpPosicionPrecargada";

export interface ProcesarPrecargaSolpResponse {
    ErroresValidacion: string[];
    Posiciones: SolpPosicionPrecargada[];
}