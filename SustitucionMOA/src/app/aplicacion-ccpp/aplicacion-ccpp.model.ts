import { DropdownOption } from '../common/view-child/dropdown/dropdown.component';
export enum EstadoAplicacionCCPP {
    Pendiente,
    Aplicado,
    Error
}
export type ColorEstado = 'red' | 'green' | 'orange'

export interface AplicacionCCPPForm {
    Id: number;
    Contrato: string;
    CartaPorteSeleccionada: CartaPorteParaAplicacionCartaPorte;
    Kilogramos: number;
}
export interface AplicacionCCPP extends AplicacionCCPPForm {
    Proveedor_Id?: number;
    Proveedor?: any;
    CartaPorte: string;
    Estado: EstadoAplicacionCCPP
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

export type ContratoParaAplicacionCartaPorte = {
    NumeroContrato: string;
}
export type ContratoParaAplicacionCartaPorteResponse = {
    Contratos: ContratoParaAplicacionCartaPorte[]
}
export type CartaPorteParaAplicacionCartaPorte = {
    NumeroCartaPorte: string;
    KgPendientes: number;
}
export type CartaPorteParaAplicacionCartaPorteResponse = {
    CartasPorte: CartaPorteParaAplicacionCartaPorte[]
}