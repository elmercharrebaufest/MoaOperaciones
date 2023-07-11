import { Component,Input,OnInit, Output, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { animate, style, transition, trigger } from '@angular/animations';

import { SelectItem } from 'primeng/api';
import { ConfirmationService } from 'primeng/components/common/api';
import { MessageService } from 'primeng/components/common/messageservice';

import { BlockUI, NgBlockUI } from 'ng-block-ui';
import * as uuid from 'uuid';

import { BaseComponent } from '../../common/base-components/base-component';
import { Paso } from '../../common/models/paso';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';

import { ComprasService } from '../compras.service';
import { CabeceraComponent } from './steps/posicion/cabecera.component';
import { EspecificacionesViewModel } from './steps/especificaciones/especificacionesViewModel';
import { SubPosicionViewModel } from './steps/posicion/tab-subposicion/sub-posicion-view-model';
import { DashboardComponent } from '../dashboard/dashboard.component';
import { ArchivoModel } from './steps/archivo.model';
import { EnumPasoSolp } from '../enum-paso-solp';
import { EnumTipoSolpSap } from '../enum-tipo-solp-sap';
import { ComponentMode, setupDaysAndMonths, setupJornadaLaboralDias, setupSolpPasos } from './solp.utils';
import { Solp } from './solp';
import { SolpPosicion } from './solp-posicion';
import { EmailComposeModel } from '../../common/email-compose/email-compose.model';
import { EmailComposeService } from '../../common/email-compose/email-compose.service';
import { CotizacionComponent } from './steps/cotizacion/cotizacion.component';

@Component({
    selector: 'app-solp',
    templateUrl: './solp.component.html',
    styleUrls: ['../compras.component.css'],
    animations: [
        trigger('fadeInOut', [
            transition(
                ':enter',
                [
                    style({ opacity: 0 }),
                    animate('1s ease-out',
                        style({ opacity: 1 }))
                ]
            ),
            transition(
                ':leave',
                [
                    style({ opacity: 1 }),
                    animate('0s ease-in',
                        style({ opacity: 0 }))
                ]
            )
        ]),
    ],
    providers: [ComprasService, MessageService]
})

export class SolpComponent extends BaseComponent implements OnInit {

    @BlockUI() blockUI: NgBlockUI;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild(CabeceraComponent)
    protected cabecera: CabeceraComponent;

    @ViewChild(DashboardComponent)
    protected dashboard: DashboardComponent;

    cambiosGuardados: boolean = false;
    mostrarPreview: boolean = false;
    pdfPreview: any;
    urlPdf: any;
    solpActual: Solp;
    _pasoActual: Paso;
    es: any;
    enumSolp: typeof EnumPasoSolp = EnumPasoSolp;
    combos: any;
    solpId: number = 0;
    visitasFinalizar: SelectItem[];
    finalizarOk: boolean;
    displayFinalizar: boolean = false;
    displaySAP: boolean;
    displayErrorSAP: boolean;
    displaySAPVincularPliego: boolean;
    disabledSave = false;

    disabled: boolean = false;

    listadoErrores: string[] = new Array<string>();
    displaySAPEditar: boolean;
    flagSolpFinalizada: boolean = false;

    tieneContratoMarco: boolean = false;


    set pasoActual(value: Paso) {
        this.actualizarPasoCompleto(this._pasoActual);
        this._pasoActual = value;
    }

    get pasoActual(): Paso {
        return this._pasoActual;
    }

    pasos: Paso[];

    selectUsuarioCompras: any;
    usuarioComprasList: any[] = [];

    titulo: string = "";
    tituloNroSolp: string = "";
    camposObligatorios: any[] = [
        { campo: 'revisadoPor', esObligatorio: true}
    ];

    public solpMode: ComponentMode;

    constructor(protected service: ComprasService, 
        protected navService: NavService, 
        protected sessionDataService: SessionDataService, 
        protected securytiService: SecurityService, 
        protected floatMsgService: FloatMsgService, 
        protected modalService: ModalService,
        private messageService: MessageService, 
        private route: ActivatedRoute, 
        private confirmationService: ConfirmationService,
        private emailComposeService: EmailComposeService) {
        super(navService, securytiService, floatMsgService, modalService);
        this.pasos = setupSolpPasos();
        this.solpActual = new Solp();
    }

    ngOnInit() {
        if (this.pasos && this.pasos.length > 0) {
            this.getCombos();
            this.es = setupDaysAndMonths();            

            this.pasos[0].Activo = true;
            this.pasos[0].Iniciado = true;
            this.pasoActual = this.pasos[0];

            if (this.route.params) {
                this.route.params.forEach((params: Params) => {
                    let numeroSolp = "";
                    // if (params["id"] > 0) this.solpId = params["id"];
                    if (parseInt(params["id"].split(',')[0]) > 0) this.solpId = parseInt(params["id"].split(',')[0]);
                    if (params["tipoSolp"]) this.solpActual.tipoSolp = params["tipoSolp"];
                    if (params["id"].split(',')[1] == undefined) {
                        numeroSolp = "";
                    } else {
                        numeroSolp = params["id"].split(',')[1];
                    }

                    if (numeroSolp != "") this.flagSolpFinalizada = true;
                    this.tituloSolp();
                });

                if (this.solpId > 0) {
                    this.setComponentMode(ComponentMode.Edition);
                    this.traerSolpId(this.solpId);
                } else {
                    this.setComponentMode(ComponentMode.Creation);
                    this.setearPasos();
                }
            }
        }
    }

    public setComponentMode(value: ComponentMode) {
        this.solpMode = value;
    }

    public getComponentMode(): ComponentMode {
      return this.solpMode;
    }

    public get esCreacionSolp(): boolean {
      return this.getComponentMode() === ComponentMode.Creation;
    }

    public get esEdicionSolp(): boolean {
      return this.getComponentMode() === ComponentMode.Edition;
    }

    private setupCentroPorDefecto(): void {
        let centroPorDefecto = this.combos.Centro.find(x => x.Codigo == "1029");
        if (centroPorDefecto != null) {
            this.solpActual.centroPorDefecto = centroPorDefecto;
        }
    }

    private setupDireccionCentroPorDefecto(): void {
        if (this.solpActual.centroPorDefecto) {
            let direccionCentro = this.combos.CentrosDireccion.find(x => x.CodigoSap == this.solpActual.centroPorDefecto.CodigoSap);
            if (direccionCentro != null){
                this.solpActual.direccionCentroPorDefecto = direccionCentro;
            }
        }
    }
    
    private setupMonedaPorDefecto(): void {
        let monedaPorDefecto = this.combos.Moneda.find(x => x.Codigo == "ARP");
        if (monedaPorDefecto) {
            this.solpActual.monedaPorDefecto = monedaPorDefecto;
        }
    }

    tituloSolp() {
        switch (this.solpActual.tipoSolp) {
            case "CON_PLIEGO":
                this.titulo = "Generación de SOLP con documento de pliego"
                break;
            case "SIN_PLIEGO":
                this.titulo = "Generación de SOLP sin documento de pliego"
                break;
            default:
        }
    }

    setearPasos() {
        switch (this.solpActual.tipoSolp) {
            case "CON_PLIEGO":
                break;
            case "SIN_PLIEGO":
                this.pasos[0].Deshabilitado = true;
                this.pasos[2].Deshabilitado = true;
                this.pasos[3].Deshabilitado = false;
                this.pasos[0].Iniciado = true;
                this.pasos[2].Iniciado = true;
                this.pasos[3].Iniciado = true;
                this.pasos[0].Completo = true;
                this.pasos[2].Completo = true;
                this.pasos[3].Completo = true;
                this.pasoActual = this.pasos[1];
                break;
            default:
        }
    }

    sumarDias(fecha, dias) {
        fecha.setDate(fecha.getDate() + dias);
        return fecha;
    }

    traerSolpId(idSolp) {
        try {
            this.blockUI.start('Cargando...');
            this.spinnerComponent.showIt();

            this.subscription = this.service.traerSolpId(idSolp).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.solpActual = new Solp(result.data)
                        let estadosPasos = this.solpActual.estadoPasos.split(',');
                        
                        let count = 0;
                        estadosPasos.forEach(item => {
                            if (this.pasos[count]) {
                                this.pasos[count].Iniciado = item == '1' || item == '2';
                                this.pasos[count].Completo = item == '2';
                                count += 1;
                            }
                        });

                        this._pasoActual = this.pasos.find(x => x.Numero == 1) as Paso;
                        this.cambioPaso(this.pasos[0]);
                        this.setearPasos();
                        this.blockUI.stop();
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.spinnerComponent.hideIt();
                    this.blockUI.stop();
                });
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    private getSelectedTipoPosicion(posiciones: any) {
        let tipoPosicion = undefined;
        if (posiciones != undefined && posiciones.length > 0) {
            let posicion = posiciones.filter(p => p.TipoPosicion.Codigo != undefined);
            if (posicion != undefined) {
                tipoPosicion = posicion[0].TipoPosicion.Codigo;
            }
        }
        return tipoPosicion;
    }

    validatePasos(): any {
        let estadosPasos = this.solpActual.estadoPasos.split(',');
        this.solpActual.estadoPasos = '';

        this.pasos.forEach((p, i) => {
            estadosPasos[p.Numero - 1] = !p.Iniciado ? '0' : p.Completo ? '2' : '1';
            this.solpActual.estadoPasos += `${estadosPasos[p.Numero - 1]},`;

        });

        this.solpActual.estadoPasos = this.solpActual.estadoPasos.substring(0, this.solpActual.estadoPasos.length - 1);

        return {
            completo: estadosPasos.every(x => x === '2'),
            primerPasoIncompleto: 1 + estadosPasos.findIndex(x => x != '2')
        }
    }

    cambioPaso(paso) {
        let estadosPasos = this.solpActual.estadoPasos.split(',');
        this.pasos.forEach((p, i) => {
            if (p.Codigo == this.pasoActual.Codigo) {
                p.Activo = false;
                p.Iniciado = true;
                //p.Completo = true;
            } else if (p.Codigo == paso.Codigo) {
                p.Iniciado = true;
                p.Activo = true;
            } else if (p.Codigo == "PliegoCotizacion" && paso.Numero == 5) {
                p.Iniciado = true;
                p.Activo = true;
            } else if (p.Codigo == "PliegoEspecificacion" && paso.Numero == 5) {
                p.Iniciado = true;
                p.Activo = true;
            }

            estadosPasos[p.Numero - 1] = p.Completo ? '2' : '1';
        });

        this.solpActual.estadoPasos = '';


        estadosPasos.forEach(x => {
            this.solpActual.estadoPasos += `${x},`;
        });

        this.solpActual.estadoPasos = this.solpActual.estadoPasos.substring(0, this.solpActual.estadoPasos.length - 1);

        this.pasoActual = paso; //aca esta el error

        this.solpActual.pasoCompletado = this.solpActual.pasoCompletado > this.pasoActual.Numero
            ? this.solpActual.pasoCompletado
            : this.pasoActual.Numero;

        this.actualizarPasoCompleto(this.pasoActual);

        // this.guardarCambios(false, false, true);
    }

    navegar(paso) {
        if (paso.paso) {
            this.pasoActual = paso.paso
        } else if (paso.descripcion == 'VOLVER') {
            this.navService.navegarSeccion('/compras');
        } else if (paso.descripcion == 'FINALIZAR') {
            //guardar - finalizar
        }
    }

    mostrarValidacion(campoAValidar, vacio){
        let camposVacios = this.camposObligatorios.find(x => x.campo == campoAValidar && x.esObligatorio);
        return (camposVacios != null && vacio == 0);
    }

    guardarCambios({mostrarPreview = false, enviarSap = false, guardarPorPaso = false}) {
        this.messageService.clear();
        try {
            if(!this.solpActual.posiciones){
                this.solpActual.posiciones = [];
            }
            
            this.actualizarPasoCompleto(this.pasoActual);
            

            this.disabledSave = true;
            if (guardarPorPaso == false) {
                this.blockUI.start('Guardando...');
            }

            let validatePasos = this.validatePasos();

            if (enviarSap) {
                if (!validatePasos.completo) {
                    this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `Falta completar campos en el paso #${validatePasos.primerPasoIncompleto}` });

                    if (guardarPorPaso == false) {
                        this.blockUI.stop();
                    }
                    this.disabledSave = false;
                    return;
                }

                if (!this.solpActual.revisadoPor) {
                    this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `Falta completar campo Revisado por` });

                    if (guardarPorPaso == false) {
                        this.blockUI.stop();
                    }
                    this.disabledSave = false;
                    return;
                }

                if (this.selectUsuarioCompras.Id == null) {
                    this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `Falta completar campo Usuario compras` });

                    if (guardarPorPaso == false) {
                        this.blockUI.stop();
                    }
                    this.disabledSave = false;
                    return;
                }
            }
            this.solpActual.Finalizar = enviarSap;
            this.solpActual.usuarioComprasId = this.selectUsuarioCompras != null ? this.selectUsuarioCompras.Id : null;

            this.subscription = this.service.GuardarSolp(this.solpActual).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                        if (guardarPorPaso == false) {
                            this.blockUI.stop();
                        }
                    } else if (result.error != undefined && result.error != "") {
                        this.messageService.add({ severity: 'error', summary: 'No se pudo guardar la solp', detail: result.error });
                        if (guardarPorPaso == false) {
                            this.blockUI.stop();
                        }
                    } else if (result.info != undefined) {
                        this.messageService.add({ severity: 'info', summary: 'No se pudo guardar la solp', detail: result.info });
                        if (guardarPorPaso == false) {
                            this.blockUI.stop();
                        }
                    } else {

                        if (guardarPorPaso == false) {
                            this.blockUI.stop();
                        }

                        if (!mostrarPreview && !guardarPorPaso) {
                            this.messageService.add({ severity: 'success', detail: 'Los datos se guardaron correctamente' });
                        }
                        // this.floatMsgService.setSuccessMsg("Los datos se guardaron correctamente");
                        this.solpActual.id = result.Solp.Id;
                        this.solpActual.NroSolp = result.Solp.NroSolp;
                        this.solpActual.especificacionesViewModel.archivosEspecificacionesNuevos.splice(0, this.solpActual.especificacionesViewModel.archivosEspecificacionesNuevos.length);
                        this.solpActual.especificacionesViewModel.archivosEspecificaciones = result.Solp.Adjuntos.filter(x => x.FileKey == 'adjuntoSolp' || x.FileKey == 'especificacionesTecnicasPliego').map(x => {
                            return {
                                id: x.Id,
                                nombreArchivo: x.Nombre,
                                rutaDeAcceso: ''
                            }
                        });
                        this.solpActual.archivosCotizacionesNuevos.splice(0, this.solpActual.archivosCotizacionesNuevos.length);
                        this.solpActual.archivosCotizaciones = result.Solp.Adjuntos.filter(x => x.FileKey == 'adjuntoCotizacionesSolp').map(x => {
                            return {
                                id: x.Id,
                                nombreArchivo: x.Nombre,
                                rutaDeAcceso: ''
                            }
                        });

                        this.cambiosGuardados = true;

                        if (mostrarPreview) {
                            if (result.Solp.Pdf) {
                                this.pdfPreview = "data:application/pdf;base64," + result.Solp.Pdf;
                                this.mostrarPreview = true;
                            } else {
                                this.messageService.add({ severity: 'error', detail: 'Hubo un error al generar el preview. Por favor contacte con el administrador de sistemas.' });
                            }
                        }

                        if (enviarSap) {

                            if (result.Mensaje == "OK") {
                                this.finalizarOk = true;

                                if (this.solpActual.vincularAPliego) {
                                    this.displaySAPVincularPliego = true;
                                }
                                else if (this.solpActual.nroSolp) {
                                    this.displaySAPEditar = true;
                                } else {
                                    this.displaySAP = true;
                                }
                            }
                            else {
                                if (result.Solp.NroSolp != "" && result.Solp.NroSolp != null) {
                                    this.solpActual = new Solp(result.Solp)
                                    let esto = this;
                                    setTimeout(function () {
                                        esto.cambioPaso(esto.pasos[5]);
                                    }, 500);
                                }
                                this.listadoErrores = result.Errores;
                                this.displayErrorSAP = true;
                            }
                            // if (this.solpActual.nroSolp) {
                            //     this.displaySAPEditar = true;
                            // }
                        }
                    }
                    this.disabledSave = false;
                },
                error => {
                    this.messageService.add({ severity: 'error', summary: 'Error al intentar guardar la solp.', detail: error.message });
                    if (guardarPorPaso == false) {
                        this.blockUI.stop();
                    }
                    this.disabledSave = false;
                }
            );
        } catch (e) {  
            this.disabledSave = false;
            this.messageService.add({ severity: 'error', summary: 'Error al intentar guardar la solp', detail: e });
            if (guardarPorPaso == false) {
                this.blockUI.stop();
            }
            return false; //<-- Prevent Refresh
        }
    }

    actualizarPasoCompleto(paso: Paso) {
        if (paso) {
            switch (paso.Codigo) {
                case EnumPasoSolp.PliegoGeneracion1:
                    paso.Completo = true;
                    if (!paso.Deshabilitado) {
                        if (!this.listaStringCompleta([
                            this.solpActual.nombreDePedido,
                            this.solpActual.fiscalContrato,
                            this.solpActual.mail,
                            this.solpActual.fechaEntrega,
                            this.solpActual.horaEntrega
                        ])) {
                            paso.Completo = false;
                        }
                    }

                    break;

                case EnumPasoSolp.PliegoGeneracion2:
                    paso.Completo = true;
                    if (!paso.Deshabilitado) {
                        if (!this.listaStringCompleta([
                            this.solpActual.supervisorSector,
                            this.solpActual.supervisorTrabajo
                        ])) {
                            paso.Completo = false;
                        }
                    }
                    break;
                case EnumPasoSolp.PliegoEspecificacion:
                    paso.Completo = true;
                    if (!paso.Deshabilitado) {
                        if (!this.listaStringCompleta([
                            this.solpActual.especificacionesViewModel.observaciones
                        ])) 
                        {
                            return paso.Completo = false;
                        }
                    }
                    break;
                case EnumPasoSolp.PliegoCotizacion:
                    paso.Completo = true;
                    if (!this.listaStringCompleta([
                        //this.solpActual.ejecucion,
                        this.solpActual.jornadaLaboralDias,
                        this.solpActual.comienzoJornadaLaboral,
                        this.solpActual.terminoJornadaLaboral, 
                        this.solpActual.validarTrabajoHecho
                    ])) {
                        return paso.Completo = false;
                    } else {
                        return this.solpActual.mensajeCotizacion = "";
                    }
                    
                    break;
                case EnumPasoSolp.SolpCabecera:
                    paso.Completo = true;
                    if (!paso.Deshabilitado) {
                        if(!this.solpActual.posiciones){
                            this.solpActual.posiciones = [];
                        }
                        this.solpActual.posiciones.forEach(pos => {
                            pos.doValidatePosicion(this.solpActual.tipoSolpSap);
                            if (!pos.tabsPosicionValidos.tabDireccionEntrega ||
                                (pos.esTipoPosicionMaterial && !pos.tabsPosicionValidos.tabImputacion) ||
                                !pos.tabsPosicionValidos.tabProveedor ||
                                !pos.tabsPosicionValidos.tabDatosPosicion ||
                                !pos.tabsPosicionValidos.tabFechas ||
                                (pos.esTipoPosicionServicio && !pos.tabsPosicionValidos.tabSubposiciones) ||
                                !pos.tabsPosicionValidos.tabPosiciones || this.validarContratoMarco() || this.validarAdicional() || this.validarCondicionesEspeciales()) {                                     
                                    return paso.Completo = false;
                            } else {
                                return pos.mensaje = "";
                            }
                        });
                    }
                    break;
            }
        }
    }

    validarContratoMarco(){
        var validacionContratoTrabajo = false;

        if(this.solpActual.posiciones.some(x => x.numeroContratoSuperior != "") && this.solpActual.trabajoHecho == true){
            return validacionContratoTrabajo = true;
        }
        return validacionContratoTrabajo;
    }

    validarAdicional(){
        var validacionAdicional = false;

        if(this.solpActual.posiciones.some(x => x.numeroContratoSuperior != "") && this.solpActual.adicional == true){
            return validacionAdicional = true;
        }
        return validacionAdicional;
    }
    
    validarCondicionesEspeciales(){
        var validacionCheck = false;

        if(this.solpActual.trabajoHecho == true && this.solpActual.adicional == true){
            return validacionCheck = true;
        }
        return validacionCheck;
    }

    mostrarMensajeCampos(){
        this.solpActual.posiciones.forEach(pos => {
            if (pos.mensaje != "") {
                this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `${pos.mensaje}` });
            }
        })

        if(this.validarContratoMarco()){
            this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: "Las solp con contrato marco cargado no pueden tener el tilde en el check de trabajo hecho en el paso #4" });
        }

        if(this.validarAdicional()){
            this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: "Las solp con contrato marco cargado no pueden tener el tilde en el check de adicional en el paso #4" });
        }

        if(this.validarCondicionesEspeciales()){
            this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: "Las solp no pueden tener el tilde en el check de adicional y el check de trabajo hecho en el paso #4" });
        }
    }

    mostrarMensajeCotizacion(){
            if (this.solpActual.mensajeCotizacion != "" && this.solpActual.mensajeCotizacion != undefined) {
                this.messageService.add({ severity: 'error', summary: 'No se pudo finalizar', detail: `${this.solpActual.mensajeCotizacion}` });
                console.log("mensaje", this.solpActual.mensajeCotizacion)
            } 

    }

    diasJornadaLaboral(lista: any[]) {
        return lista.filter(x => x.selected).length >= 1;
    }

    listaStringCompleta(lista: any[]) {
        return lista.filter(x => !x || x.length == 0).length == 0;
    }

    listaStringVacia(lista: any[]) {
        return lista.every(x => !x || x.length == 0);
    }

    //evento que se activa cuando se deja uno de los paso de la solp
    //infomacionSolp : { codigo : string , esPasoInvalido : bool}
    actualizarEstadoSolp(infomacionSolp: any) {
        var pasoSolp = this.pasos.find(x => x.Codigo == infomacionSolp.codigo);
        if (pasoSolp == undefined) return;
        pasoSolp.Completo = !infomacionSolp.esPasoInvalido;

        let estadosPasos = this.solpActual.estadoPasos.split(',');
        estadosPasos[pasoSolp.Numero - 1] = pasoSolp.Completo ? '2' : estadosPasos[pasoSolp.Numero - 1];

        this.solpActual.estadoPasos = '';

        estadosPasos.forEach(x => {
            this.solpActual.estadoPasos += `${x},`;
        });

        this.solpActual.estadoPasos = this.solpActual.estadoPasos.substring(0, this.solpActual.estadoPasos.length - 1);
    }

    // funcion para que cuando agregues una posicion, vuelva a la altura posiciones
    scrollTo(el: HTMLElement) {
        el.scrollIntoView();
    }

    duplicarPosicion(el: HTMLElement) {
        this.cabecera.validarPosicionActual();
        this.solpActual.agregarNuevaPosicion(null as SolpPosicion);
        el.scrollIntoView();
    }

    getCombos() {
        try {
            this.subscription = this.service.getCombos().subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.combos = result;
                        this.combos.flagSolpFinalizada = this.flagSolpFinalizada;
                        this.obtenerUsuarioCompras();
                        this.setupCentroPorDefecto();
                        this.setupDireccionCentroPorDefecto();
                        this.setupMonedaPorDefecto();
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    obtenerUsuarioCompras() {
        try {
            this.subscription = this.service.obtenerUsuarioCompras().subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.usuarioComprasList = [];
                        result.data.forEach(element => {
                            this.usuarioComprasList.push({
                                Id: element.UsuarioCompras.Id,
                                CodigoDescripcion: element.UsuarioCompras.Mail
                            });
                        });
                        this.usuarioComprasList = [{ Id: null, CodigoDescripcion: "Seleccione un usuario" }, ...this.usuarioComprasList];
                        this.selectUsuarioCompras = this.solpActual.usuarioComprasId > 0
                            ? this.usuarioComprasList.find(x => x.Id === this.solpActual.usuarioComprasId)
                            : this.usuarioComprasList[0];
                        this.spinnerComponent.hideIt();
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
        }
    }

    preview() {
        this.guardarCambios({mostrarPreview: true});
    }

    salir() {
        this.navService.navegarSeccion('/compras');
    }

    generarZipPliego(idSolp) {
        this.blockUI.start('Generando ')
        this.service.descargarZipPliego(idSolp)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "application/octet-stream",
                        });

                        if (window.navigator.msSaveOrOpenBlob) {
                            // IE11
                            window.navigator.msSaveOrOpenBlob(
                                blob,
                                result.FileDownloadName
                            );
                        } else {
                            var url = window.URL.createObjectURL(blob);
                            var link = document.createElement("a");
                            document.body.appendChild(link);
                            link.href = url;
                            link.download = result.FileDownloadName;
                            link.click();
                            setTimeout(function () {
                                window.URL.revokeObjectURL(url);
                            }, 0);
                            this.blockUI.stop();
                            return false;
                        }
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    // this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
    }

    // Todos los Modal
    finalizar({selectUsuarioCompras, solpActual}) {
        this.selectUsuarioCompras = selectUsuarioCompras;
        this.solpActual = solpActual;

        this.cabecera.validarTabCompleto();
        this.guardarCambios({mostrarPreview: false, enviarSap: true, guardarPorPaso: false});
        this.displayFinalizar = false;
        this.mostrarMensajeCampos();
        this.mostrarMensajeCotizacion();
    }

    // Abre el modal del boton finalizar
    showFinalizarDialog() {
        this.displayFinalizar = true;
    }

    cancelarFinalizar() {
        this.displayFinalizar = false;
        this.displayErrorSAP = false;
    }

    cancelarSolp() {
        this.confirmationService.confirm({
            key: 'cancelarSolp',
            message: '¿Está seguro que desea volver a la pantalla principal? No se conservaran los cambios no guardados.',
            accept: () => {
                this.salir();
            },
            reject: () => {
            }
        });
    }

    solpFinalizadaVolverHome() {
        this.salir();
    }

    ultimoPasoSolp() {
        this.confirmationService.confirm({
            key: 'ultimoPasoSolp',
            message: 'Está a punto de enviar a SOLP sin documento a xxxx ¿Desea continuar?',
            accept: () => {

            },
            reject: () => {
                this.salir();
            }
        });
    }

    modalErrorSAP() {
        this.confirmationService.confirm({
            key: 'displayErrorSAP',
            message: '',
            accept: () => {
                this.salir();
            },
            reject: () => {

            }
        });
    }

    showEmailPopup(esPrimeraFinalizacion: boolean, esPosteriorFinalizacion: boolean) {
        this.displaySAP = false;
        this.displaySAPEditar = false;
        const emailModel = new EmailComposeModel();
        emailModel.from = this.fromEmail;
        emailModel.to = this.emailTo;
        emailModel.subject = this.getEmailSubject(esPrimeraFinalizacion, esPosteriorFinalizacion);
        emailModel.body = this.emailBody;
        emailModel.downloadLinkUrl = this.downloadLinkUrl;
        this.emailComposeService.show(emailModel);
    }

    sendEmail(emailModel: EmailComposeModel) {
        this.blockUI.start('Enviando email...');
        this.service.enviarEmail(emailModel).subscribe(result => {
            this.blockUI.stop();
            this.emailComposeService.close();
            if (result.logout == true) {
                this.sessionDataService.logout();
            } else if (result.error != undefined && result.error != "") {
                this.floatMsgService.setErrorMsg(result.error);
            } else {
                this.floatMsgService.setSuccessMsg("Email se envio correctamente.");
            }
        },
        error => {
            this.emailComposeService.close();
            this.floatMsgService.setErrorMsg(error.message);
            this.blockUI.stop();
        });
    }

    private get fromEmail(): string {
        return this.solpActual.mail != undefined && this.solpActual.mail != null ? this.solpActual.mail : sessionStorage.getItem("username");
    }

    private get nombreDePedido(): string {
        return this.solpActual.nombreDePedido != undefined && this.solpActual.nombreDePedido != null ? this.solpActual.nombreDePedido : "";
    }

    private getEmailSubject(esPrimeraFinalizacion: boolean, esPosteriorFinalizacion: boolean): string {
        let subject = "";
        
        if (esPrimeraFinalizacion) {
            //SOLP N°” + *nnnnn* + “PLIEGO:” + *detalle del pliego*
            // subject = `SOLP N° ${this.solpActual.NroSolp || ""} PLIEGO: ${this.nombreDePedido}`;
            subject = `SOLP N° ${this.solpActual.NroSolp || ""} ${this.solpActual.tipoSolp === "SIN_PLIEGO" ? "" : " PLIEGO: " + this.nombreDePedido}`;

        }
        if (esPosteriorFinalizacion) {
            //ACTUALIZACIÓN SOLP N°” + *nnnnn* + “PLIEGO:” + *detalle del pliego*
            subject = `ACTUALIZACIÓN SOLP N° ${this.solpActual.NroSolp || ""} ${this.solpActual.tipoSolp === "SIN_PLIEGO" ? "" : " PLIEGO: " + this.nombreDePedido}`;
        }
        return subject;
    }

    private get emailTo(): string {
        let emailTo = "";
        if (this.solpActual.usuarioComprasId != null) {
            let usuarioCompras = this.usuarioComprasList.find(x => x.Id === this.solpActual.usuarioComprasId);
            if (usuarioCompras != null) {
                emailTo = usuarioCompras.CodigoDescripcion;
            }
        }
        return emailTo;
    }

    private get emailBody(): string {
        return `SOLP N° ${this.solpActual.NroSolp || ""} ${this.solpActual.tipoSolp === "SIN_PLIEGO" ? "" : " PLIEGO: " + this.nombreDePedido}`;
    }

    private get downloadLinkUrl(): string {
        const solpId = this.solpActual.id;
        const token = this.solpActual.emailLinkToken;
        return `${window.location.origin}/api/compras/DescargarPliegoDesdeLink?solpId=${solpId}&token=${token}`;
    }

}