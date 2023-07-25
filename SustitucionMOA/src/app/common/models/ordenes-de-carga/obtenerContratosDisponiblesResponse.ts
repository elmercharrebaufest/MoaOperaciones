import { Formatter } from "../../formatter/Formatter";
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

export class ContratoOrdenFas {
    NumeroContrato: string;
    Producto: Material;
    TipoContrato: TipoContrato;
    KgDisponibles: number;
    NombreProducto: string;
    Label: string;

    constructor(
        numeroContrato?: string,
        producto?: Material,
        kgDisponibles?: number) {

        this.NumeroContrato = numeroContrato || this.NumeroContrato;
        this.KgDisponibles = kgDisponibles === null || kgDisponibles === undefined ? undefined : kgDisponibles;
        this.Producto = producto || this.Producto;
        const prod = this.Producto.Abreviacion || this.Producto.Descripcion;
        const kgs = this.KgDisponibles <= 0 ? "sin kg. Disp." :
            !this.KgDisponibles ? '' :
                `${Formatter.formatNumberWithPoint(this.KgDisponibles.toString())} kg. Disp.`;
        this.Label = this.NumeroContrato + " - " + prod + " " + kgs;
    }
}