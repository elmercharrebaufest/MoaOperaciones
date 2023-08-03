import { Component } from "@angular/core";
import { BaseComponent } from "./base-component";
import { OrdenDeCarga } from '../models/ordenes-de-carga/ordenDeCarga';
import { OrdenDeCargaFasonDto } from "../models/ordenes-de-carga-fason/ordenDeCargaFasonDto";
import { Planta } from "../models/ordenes-de-carga/planta";
import { NavService } from "../services/NavService";
import { SecurityService } from "../services/SecurityService";
import { FloatMsgService } from "../services/FloatMsgService";
import { ModalService } from "../services/ModalService";
import { BaseService } from "../services/BaseService";
import { Observable } from "rxjs";
import { ApiResponse } from "../models/response";
import { Domicilio } from "../models/ordenes-de-carga/domicilio";
import { ValidarIntermediarioFleteResponse } from "../models/ordenes-de-carga/ValidarIntermediarioFleteResponse";
import { ValidarCuitExisteScatoResponse } from "../models/ordenes-de-carga-common/ValidarCuitExisteScatoResponse";

export type Ordenes = OrdenDeCarga | OrdenDeCargaFasonDto

export abstract class OrdenesBaseService extends BaseService {
    abstract enviarMailGestionarAltaCuit(cuit: string, razonSocial: string, esIntermediarioFlete: boolean): Observable<ApiResponse<boolean>>
    abstract obtenerPlantasDestino(destinoCuit: string): Observable<ApiResponse<Planta[]>>
    abstract obtenerDomiciliosDestino(destinoCuit: string): Observable<ApiResponse<Domicilio[]>>
    abstract validarIntermediarioFlete(cuit: string): Observable<ApiResponse<ValidarIntermediarioFleteResponse>>
    abstract validarExisteCuitScato(cuit: string): Observable<ApiResponse<ValidarCuitExisteScatoResponse>>
    abstract validarSisaCuit(cuitDestinatario: string, cuitDestino: string, codigoMaterial: string): Observable<ApiResponse<boolean>>
    abstract validarCuitRuca(cuit: string): Observable<ApiResponse<boolean>>
}
export abstract class OrdenesBase {
    Reventa: boolean;
    CUITIntermediarioFlete?: string;
    RazonSocialIntermediarioFlete?: string;
    PlantaCodigo: string;
    DomicilioTipo: string;
    DomicilioOrden: number;
    DomicilioDescr: string;
    FleteMOA: boolean;
    RazonSocialDestinatario?: string;
    RazonSocialDestino?: string;
    CUITDestino: string;
    CUITDestinatario: string;
}

export interface IOrdenesBaseComponent {
    onPlantaSeleccionadaChanged(): void;
    onDomicilioSeleccionadoChanged(): void;
    setearDefaultEnCPEDG(): void;
    gestionarAltaCUIT(): void;
    validarIntermediarioFlete(): void;
    manejarErroresApiResponse<T>(response: ApiResponse<T>): T | null;
    manejarRespuestaDomicilio(resp: ApiResponse<Domicilio[]>, cuitDestino: string): void;
    manejarRespuestaPlanta(resp: ApiResponse<Planta[]>, cuitDestino: string): void;
    revisarCUITFormatoValido(cuit: string): boolean;
    definirValorPlanta(): void;
    definirValorDomicilio(): void;
    resetearPlantasDomicilios(): void;
}
@Component({
    template: ''
})
export class OrdenesBaseComponent extends BaseComponent {
    mensajesOrdenDeCargaFason: Partial<Record<keyof Ordenes, string>> = {};
    mensajesGestionCuit: Partial<Record<keyof Pick<Ordenes, 'CUITDestinatario' | 'CUITDestino' | 'CUITIntermediarioFlete'>, string>> = {};
    validando: Partial<Record<keyof Ordenes, boolean>> = {};
    displayModal: keyof Pick<Ordenes, 'CUITDestinatario' | 'CUITDestino' | 'CUITIntermediarioFlete'> | null;

    revisarCUITFormatoValido(cuit: string): boolean {
        return !!(cuit && cuit.length == 11 && !Number.isNaN(cuit as unknown as number))
    }

    focusRazonSocialParaGestion = true;
    razonSocialParaGestion = "";
    descripcionIntermediarioFlete = "Llenar en caso que el transporte lo haga un tercero"

    constructor(
        protected service: OrdenesBaseService,
        protected navService: NavService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService) {

        super(navService, securityService, floatMsgService, modalService);
    }

    listaPlantas: Planta[];
    plantaSeleccionada?: Planta;

    listaDomicilios?: Domicilio[];
    domicilioSeleccionado?: Domicilio;

}