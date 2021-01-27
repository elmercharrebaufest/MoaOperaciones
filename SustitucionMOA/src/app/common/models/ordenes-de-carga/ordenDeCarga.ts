import { EmpresaTransporte } from "./EmpresaTransporte";
import { Persona } from "./Persona";

export type OrdenDeCarga = {
    CUITTercero: number,
    CUITCliente: number,
    Chofer: Persona,
    PatenteAcomplado: string,
    ChasisAcoplado: string,
    Transporte: EmpresaTransporte,
    Producto: string,
    Cantidad: number,
    Observacion: string
}

