import { Material } from "../material";

export interface ObtenerContratosDisponiblesResponse {

    Contratos: ContratoOrdenFas[];

    Info: string;
    Error: string;
    Logout: boolean;
}

export class ContratoOrdenFas {
    NumeroContrato: string;
    Producto: Material;
    KgDisponiblesTn: number;
    NombreProducto: string;
    Label: string;
    
    constructor(
        numeroContrato?: string,
        producto?: Material,
        kgDisponiblesTn?: number) {
        
            this.NumeroContrato = numeroContrato ? numeroContrato : this.NumeroContrato;
            this.KgDisponiblesTn = kgDisponiblesTn || this.KgDisponiblesTn;
            this.Producto = producto ? producto : this.Producto;
            const prod = this.Producto.Abreviacion ? this.Producto.Abreviacion : this.Producto.Descripcion;
            const kgs = this.KgDisponiblesTn ? " " + Math.trunc(this.KgDisponiblesTn) + " kg. Disp." : "";
            this.Label = this.NumeroContrato + " - " + prod + " " + kgs;
    }
}