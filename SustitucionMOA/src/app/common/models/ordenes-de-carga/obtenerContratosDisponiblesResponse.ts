import { Material } from "../material";

export interface ObtenerContratosDisponiblesResponse {
    Contratos: ContratoOrdenFas[];
    Info: string;
    Error: string;
    Logout: boolean;
}

export enum TipoContrato {
    Normal = 0,
    FacturaAnticipada = 1,
}

export interface ContratoOrdenFas {
    NumeroContrato: string;
    Producto: Material;
    TipoContrato: TipoContrato;
}