import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { PesificacionService } from './pesificacion.service';
import { Router, ActivatedRoute } from '@angular/router';
import { NavService } from './../common/services/NavService';
import { SecurityService } from './../common/services/SecurityService';
import { SessionDataService } from './../common/services/SessionDataService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { PesificacionBaseComponent } from './pesificacion-base.component';
import { DolarGirasol, DolarMaiz } from '../common/models/dolarMaterial';
declare var $: any;


@Component({
    selector: 'app-pesificacion',
    templateUrl: 'pesificacion.component.html',
    providers: [PesificacionService]
})

export class PesificacionComponent extends PesificacionBaseComponent implements OnInit, OnDestroy {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild(SpinnerSmallComponent)
    protected SpinnerSmallComponent: SpinnerSmallComponent;

    constructor(protected service: PesificacionService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    fecha: any = null;
    contratos: any = null;
    contrato: string = "";
    fijacion: string = "";
    cantidad: number = 0;
    file: any = null;
    visibleEnviar: boolean = true;
    itemsPerPage = "10";
    orderedByColumn: string = "NroContrato";
    permitirCarga: boolean = false;
    visibleSoja200: boolean = false;
    soja200: any = null;
    dolarGirasol?: DolarGirasol;
    dolarMaiz?: DolarMaiz;
    logMensajes: string[] = [];
    ngOnInit() {
        super.ngOnInit();
        this.setTabs();
        this.checkPermisos();
        this.initForm();
        this.setMenuSeccionTab("pesificacion", "Carga");
    }

    // setTabs() {
    //     // this.navService.setSeccionList([]);
    //     this.setMenuSeccionTab("pesificacion", "Pesificacion");
    // }

    checkPermisos() {
        //La diferencia entre VER PESIFICACION es que es read only, mientras que PESIFICACION te permite cargar pesificaciones masivas e individuales
        if (!this.securityService.tienePermiso("VER PESIFICACIONES")) {
            this.securityService.tienePermisoRedirect("PESIFICACION");
        }
    }

    getListaContratos() {
        this.mensajeComponent.setMsgsEmpty();
        //this.spinnerComponent.showIt();
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription = this.service.getContratos().subscribe(
                (result: any) => {
                    //this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.contratos = new Array();
                    } else if (result.info != undefined) {
                        this.contratos = new Array();
                        this.verificarPermiso();

                    } else {
                        this.contratos = result;
                        this.verificarPermiso();
                    }
                },
                error => {
                    //this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            //this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    verificarPermiso() {
        if (this.securityService.tienePermiso("PESIFICACION")) {
            this.permitirCarga = true;
        }
        else {
            this.permitirCarga = false;
            if (document.getElementById("linkPendiente") != null)
                document.getElementById("linkPendiente").click();
        }
    }

    initForm() {
        this.fecha = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();

        this.subscription = this.service.getData().subscribe(
            (result: any) => {
                this.fecha = null;
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.fecha = result;

                    this.contrato = "";
                    this.fijacion = "";
                    this.cantidad = 0;
                    this.verificarPermiso();
                    this.getListaContratos();
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
                return false;
            },

        );


        this.subscription = this.service.getDataSoja200().subscribe(
            (result: any) => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.soja200 = result;
                    let hoy = new Date();
                    hoy.setHours(0, 0, 0, 0);
                    if (new Date(parseInt(result.Desde.substr(6))) <= hoy && new Date(parseInt(result.Hasta.substr(6))) >= hoy) {
                        this.visibleSoja200 = true;
                    }
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
                return false;
            },

        );
        this.service.getDolarGirasol().subscribe(
            (result) => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.dolarGirasol = result.data;
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
                return false;
            },

        );
        this.service.getDolarMaiz().subscribe(
            (result) => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.dolarMaiz = result.data;
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
                return false;
            },

        );
        return false;
    }

    isVisible(): boolean {
        if (this.fecha && this.fecha != null)
            return true;
        else
            return false;

    }

    isVisiblePaginacion(): boolean {
        if (this.contratos != null && this.contratos.length > 0)
            return true;
        else
            return false;
    }
    orderColumnBy(column: string) {
        if (column == this.orderedByColumn) {
            this.orderDirection = -this.orderDirection;
        } else {
            this.orderDirection = 1;
            this.orderedByColumn = column;
        }
    }

    guardarPesificaciones() {
        this.spinnerSmallComponent.showIt();
        this.visibleEnviar = false;

        this.mensajeComponent.setMsgsEmpty();
        this.logMensajes = [];

        if (this.contrato == null || this.contrato == "") {
            this.spinnerSmallComponent.hideIt();
            this.visibleEnviar = true;

            this.mensajeComponent.setErrorMsg("Debe ingresar un contrato");
            return false;
        }

        if (this.cantidad == null || this.cantidad <= 0) {
            this.spinnerSmallComponent.hideIt();
            this.visibleEnviar = true;
            this.mensajeComponent.setErrorMsg("Debe ingresar una cantidad mayor a 0");
            return false;
        }

        this.unsubscribe();
        this.subscription = this.service.setData(this.contrato, this.fijacion, this.cantidad).subscribe(
            (result: any) => {
                this.setLogMensajes(result);
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.getListaContratos();
                    this.mensajeComponent.setSuccessMsg("Operacion realizada exitosamente");
                }
                $("#hidemyModal").click();
                this.spinnerSmallComponent.hideIt();
                this.visibleEnviar = true;
                return false;
            },
            error => {
                $("#myModalConfirmacion").hide();
                this.spinnerSmallComponent.hideIt();
                this.visibleEnviar = true;
                this.mensajeComponent.setErrorMsg(error.message);
                return false;
            }
        );
        return false;
    }

    cargarArchivo(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            this.file = fileList[0];
        }
    }

    cargaMasiva() {
        this.spinnerSmallComponent.showIt();
        this.visibleEnviar = false;
        this.mensajeComponent.setMsgsEmpty();
        this.logMensajes = [];

        if (this.file == null || !this.esCSV(this.file.name)) {
            this.spinnerSmallComponent.hideIt();
            this.visibleEnviar = true;
            this.mensajeComponent.setErrorMsg("Debe seleccionar un archivo .csv valido");
            return false;
        }

        this.unsubscribe();
        this.subscription = this.service.setMassiveData(this.file).subscribe(
            (result: any) => {
                this.spinnerSmallComponent.hideIt();
                this.visibleEnviar = true;
                this.setLogMensajes(result);
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.getListaContratos();
                    if(this.logMensajes.length > 0) {
                        this.mensajeComponent.setInfoMsg("Operacion realizada con errores, ver log de mensajes");   
                    } else {
                        this.mensajeComponent.setSuccessMsg("Operacion realizada exitosamente");
                    }
                }
            },
            error => {
                this.spinnerSmallComponent.hideIt();
                this.visibleEnviar = true;
                this.mensajeComponent.setErrorMsg(error.message);
                return false;
            }
        );
        return false;
    }

    esCSV(name: String): boolean {
        let ext = name.substring(name.lastIndexOf('.') + 1);
        if (ext.toLowerCase() == 'csv')
            return true;
        else
            return false;
    }

    round(num: number) {
        return Math.round(num);
    };

    changeFijacion() {
        this.fijacion = this.fijacion.replace(/^0+/, '');
    }

    changeContrato() {
        this.contrato = this.contrato.replace(/^0+/, '');
    }

    private setLogMensajes(result: any) {
        if (!result || !Array.isArray(result.Log)) {
            this.logMensajes = [];
            return;
        }

        this.logMensajes = result.Log
            .map((item: any) => item && item.Mensaje ? item.Mensaje : null)
            .filter((mensaje: string | null) => !!mensaje) as string[];
    }

}

