import { DropdownOption } from '../common/view-child/dropdown/dropdown.component';
export enum EstadoAplicacionCCPP {
    Pendiente,
    Aplicado,
    Error
}
export type ColorEstado = 'red' | 'green' | 'orange'

export interface AplicacionCCPP {
    Id: number;
    Proveedor_Id?: number;
    Proveedor?: any;
    Contrato: string;
    CartaPorte: string;
    Estado: EstadoAplicacionCCPP
    Kilogramos: number;
    Usuario_Id?: number;
    Usuario?: any;
    Error?: string;
    FechaAlta: string;
    FechaActualizacion: string;
    //Dto info
    MailUsuario?: string;
    RazonSocial?: string;
    ColorEstado?: ColorEstado;
    LabelEstado?: string;
}
export interface AplicacionCCPPFiltro {
    FiltroContratos: Array<DropdownOption>;
    FiltroCartasPorte: Array<DropdownOption>;
    FiltroEstados: Array<DropdownOption>;
    FiltroClientes: Array<DropdownOption>;
}
export const SeccionAplicacionCCPP = "aplicaciones-ccpp";
export const estadosAplicacionCCPP: DropdownOption[] = [
    { value: "", label: "Todos" },
    { value: EstadoAplicacionCCPP.Aplicado.toString(), label: "Aplicado" },
    { value: EstadoAplicacionCCPP.Error.toString(), label: "Error" },
    { value: EstadoAplicacionCCPP.Pendiente.toString(), label: "Pendiente" },
] 