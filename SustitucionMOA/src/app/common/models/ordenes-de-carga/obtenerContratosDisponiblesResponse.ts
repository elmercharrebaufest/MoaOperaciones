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
    CyO = 3,
}

export enum CondicionRetiro {
    PuestoEnDestino = 0,
    RetiroEnPlanta = 1
}

export class ContratoOrdenFas {
    NumeroContrato: string;
    Producto: Material;
    TipoContrato: TipoContrato;
    KgDisponibles: number;
    CondicionRetiro: CondicionRetiro;
    NombreProducto: string;
    Label: string;

    constructor(
        numeroContrato?: string,
        tipoContrato?: TipoContrato,
        producto?: Material,
        kgDisponibles?: number,
        condicionRetiro?: CondicionRetiro) {
            
        this.NumeroContrato = numeroContrato || this.NumeroContrato;
        this.KgDisponibles = kgDisponibles === null || kgDisponibles === undefined ? this.KgDisponibles : kgDisponibles;
        this.Producto = producto || this.Producto;
        this.TipoContrato = tipoContrato;
        this.CondicionRetiro = condicionRetiro;
        const prod = this.Producto.Abreviacion || this.Producto.Descripcion;
        
        const kgs = this.KgDisponibles <= 0 ? "sin kg. Disp." :
            !this.KgDisponibles ? '' :
                `${Formatter.formatNumberWithPoint(this.KgDisponibles.toString())} kg. Disp.`;

        const marcaTipoContrato = this.TipoContrato == TipoContrato.FacturaAnticipada ? ' (A)' : '';
        const marcaCondicionRetiro = this.CondicionRetiro == CondicionRetiro.PuestoEnDestino ? ' (PD)' : '';

        this.Label = `${this.NumeroContrato} - ${prod} ${kgs}${marcaTipoContrato}${marcaCondicionRetiro}`;
    }
}