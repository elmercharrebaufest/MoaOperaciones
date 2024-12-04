import { Component } from "@angular/core";
import { BaseComponent } from "./base-component";
import { Planta } from "../models/ordenes-de-carga/planta";
import { NavService } from "../services/NavService";
import { SecurityService } from "../services/SecurityService";
import { FloatMsgService } from "../services/FloatMsgService";
import { ModalService } from "../services/ModalService";
import { BaseService } from "../services/BaseService";
import { Observable, Subject, Subscription } from "rxjs";
import { ApiResponse } from "../models/response";
import { Domicilio } from "../models/ordenes-de-carga/domicilio";
import { ValidarIntermediarioFleteResponse } from "../models/ordenes-de-carga/ValidarIntermediarioFleteResponse";
import { ValidarCuitExisteScatoResponse } from "../models/ordenes-de-carga-common/ValidarCuitExisteScatoResponse";
import { debounceTime, filter } from "rxjs/operators";


export abstract class OrdenesBaseService extends BaseService {
    abstract obtenerPlantasDestino(destinoCuit: string): Observable<ApiResponse<Planta[]>>
    abstract obtenerDomiciliosDestino(destinoCuit: string): Observable<ApiResponse<Domicilio[]>>
    abstract validarIntermediarioFlete(cuit: string): Observable<ApiResponse<ValidarIntermediarioFleteResponse>>
    abstract validarExisteCuitScato(cuit: string): Observable<ApiResponse<ValidarCuitExisteScatoResponse>>
    abstract validarSisaCuit(cuitDestinatario: string, cuitDestino: string, codigoMaterial: string): Observable<ApiResponse<boolean>>
    abstract validarCuitRuca(cuit: string): Observable<ApiResponse<boolean>>
    abstract validarCuilChofer(cuil: string): Observable<ApiResponse<boolean>>;
    abstract validarCuitTransporte(cuil: string): Observable<ApiResponse<boolean>>
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
    CUITDestino?: string;
    CUITDestinatario?: string;
    Escalable: boolean;
    CUITChofer: string;
    CUILChofer: string;
    CUITTransporte: string;
    NecesitaVerificarCuitsTerceros: boolean;
    DestinoMercaderia: string;
}

export interface IOrdenesBaseComponent {
    onPlantaSeleccionadaChanged(): void;
    onDomicilioSeleccionadoChanged(): void;
    setearDefaultEnCPEDG(): void;
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
    mensajesOrdenDeCargaFason: Partial<Record<keyof OrdenesBase, string>> = {};
    mensajesGestionCuit: Partial<Record<keyof Pick<OrdenesBase, 'CUITDestinatario' | 'CUITDestino' | 'CUITIntermediarioFlete'>, string>> = {};
    validando: Partial<Record<keyof OrdenesBase, boolean>> = {};
    displayModal: keyof Pick<OrdenesBase, 'CUITDestinatario' | 'CUITDestino' | 'CUITIntermediarioFlete'> | null;

    descripcionIntermediarioFlete = "Llenar en caso que el transporte lo haga un tercero"
    descripcionTransporte = "CUIT transportista MOA"
    ttCopiarCuit = "Copiar CUIT del Cliente"

    revisarCUITFormatoValido(cuit: string): boolean {
        return !!(cuit && cuit.length == 11 && !Number.isNaN(cuit as unknown as number))
    }

    focusRazonSocialParaGestion = true;
    razonSocialParaGestion = "";

    localSubscriptions = new Subscription();

    validarCNRTSubject = new Subject();
    validarCNRTSubscription?: Subscription;

    validacionExistenciaPatente$ = new Subject<void>();
    validacionExistenciaPatenteSub?: Subscription;

    constructor(
        protected service: OrdenesBaseService,
        protected navService: NavService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService) {

        super(navService, securityService, floatMsgService, modalService);
        this.localSubscriptions.add(
            this.validarCNRTSubject.pipe(debounceTime(500)).subscribe(_ =>
                this.validarCNRTRequest()
            ))
        this.localSubscriptions.add(
            this.validacionExistenciaPatente$
                .pipe(
                    debounceTime(500),
                    filter(_ => this.puedeValidarExistenciaPatentes()))
                .subscribe(() => {
                    this.validarExistenciaPatentes()
                })
        )
    }

    listaPlantas: Planta[];
    plantaSeleccionada?: Planta;

    listaDomicilios?: Domicilio[];
    domicilioSeleccionado?: Domicilio;

    esPatenteValida(patente: string): boolean {
        const exprReg = /^[A-Z]{3}[\d]{3}$|^[A-Z]{2}[\d]{3}[A-Z]{2}$/;
        const esValida = exprReg.test(patente);
        return esValida;
    }

    validarCNRTRequest(): void {
        throw new Error("Method not implemented.");
    }
    validarExistenciaPatentes(): void {
        throw new Error("Method not implemented.");
    }

    puedeValidarExistenciaPatentes(): boolean {
        throw new Error("Method not implemented.");
    }
}