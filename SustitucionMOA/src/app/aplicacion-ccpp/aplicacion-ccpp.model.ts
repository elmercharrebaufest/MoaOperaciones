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
    RazonSocialCuit?: string;
    ColorEstado?: ColorEstado;
    LabelEstado?: string;
}
export interface AplicacionCCPPFiltro {
    FiltroEstados: Array<DropdownOption>;
    FiltroClientes: Array<DropdownOption>;
}
export const SeccionAplicacionCCPP = "aplicaciones-ccpp";
