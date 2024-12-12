import { SolpPosicion } from "../../../compras/solp/solp-posicion";


export interface ProcesarPrecargaSolpResponse {
    ErroresValidacion: string[];
    Posiciones: SolpPosicion[];
}