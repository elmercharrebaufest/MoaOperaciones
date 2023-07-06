import { Material } from "../material";

export interface ObtenerContratosDisponiblesResponse {

    Contratos: ContratoOrdenFas[];

    Info: string;
    Error: string;
    Logout: boolean;
}

export interface ContratoOrdenFas {
    NumeroContrato: string;
    Producto: Material;
    KgDisponiblesTn: number;
    NombreProducto: string;
    Label: string;
}