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
    ContratoSeleccionado: ContratoParaAplicacionCartaPorte;
    Kilogramos: number;
    CartaPorte: string;
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
    CodigoProveedor: string;
    Material: string;
}
export type CartaPorteParaAplicacionCartaPorte = {
    NumeroCartaPorte: string;
    KgPendientes: number;
    Material: string;
}
export type ComboAppContratosCCPPResponse = {
    Contratos: ContratoParaAplicacionCartaPorte[]
    CartasPorte: CartaPorteParaAplicacionCartaPorte[]
}

export type EnviarCargaMasivaResponse = {
    HayErroresValidacion: boolean;
    ErroresValidacion: ErrorValidacionCargaMasivaCCPP[];
    AplicacionesGuardadas: AplicacionGuardadaCargaMasivaCCPP[];
}

export interface AplicacionGuardadaCargaMasivaCCPP {
    ContratoNumero: string;
    CartaDePorte: string;
    Kilos: string;
}

export interface ErrorValidacionCargaMasivaCCPP extends AplicacionGuardadaCargaMasivaCCPP {
    Fila: number;
    Error: string;
}
