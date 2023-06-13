import { Material } from "../material";

export interface ObtenerContratosDisponiblesResponse {
    Contratos: ContratoOrdenFas[];
    Info: string;
    Error: string;
    Logout: boolean;
}

export enum TipoContrato {
    Normal = "NORMAL",
    FacturaAnticipada = "ANTICIPADO",
}

export interface ContratoOrdenFas {
    NumeroContrato: string;
    Producto: Material;
    TipoContrato: TipoContrato;
}