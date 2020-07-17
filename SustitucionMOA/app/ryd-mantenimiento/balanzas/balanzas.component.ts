import { Component, OnInit, ViewChild } from '@angular/core';
import { RYDMantenimientoService, RYDMantenimientoBalanzaService } from './../ryd-mantenimiento.service';
import { RYDMantenimientoBaseComponent } from './../ryd-mantenimiento.component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { DropdownComponent, DropdownOption } from './../../common/view-child/dropdown/dropdown.component';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';



@Component({
    selector: 'app-ryd-mantenimiento-balanzas',
    templateUrl: `./app/ryd-mantenimiento/balanzas/balanzas.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: RYDMantenimientoService, useClass: RYDMantenimientoBalanzaService }]
})
export class RYDMantenimientoBalanzaComponent extends RYDMantenimientoBaseComponent {

    @ViewChild("dropdown_tipo")
    protected InputTipoComponent: DropdownComponent;

    @ViewChild("dropdown_codCabezal")
    protected InputCodigoCabezalComponent: DropdownComponent;

    @ViewChild("dropdown_itc")
    protected InputITCComponent: DropdownComponent;

    @ViewChild("dropdown_nroPuesto")
    protected InputNroPuestoComponent: DropdownComponent;

    @ViewChild("dropdown_tipoAcceso")
    protected InputTipoAccesoComponent: DropdownComponent;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: RYDMantenimientoBalanzaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService ) {
        super(service, navService, securityService, floatMsgService, modalService);
        this.InputTipoComponent = new DropdownComponent();
        this.InputCodigoCabezalComponent = new DropdownComponent();
        this.InputITCComponent = new DropdownComponent();
        this.InputNroPuestoComponent = new DropdownComponent();
        this.InputTipoAccesoComponent = new DropdownComponent();
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    filtroTipo = Array<any>();
    filtroCodigoCabezal = Array<any>();
    filtroITC = Array<any>();
    filtroNroPuesto = Array<any>();
    filtroTipoAcceso = Array<any>();

    tipoSelected = "";
    codigoCabezalSelected = "";
    itcSelected = "";
    nroPuestoSelected = "";
    tipoAccesoSelected = "";

    data: any;

    codigo: string = "";

    descripcionBusqueda: string = "";
    tipoBusqueda: string = "";
    cabezalBusqueda: string = "";
    numeroSAPBusqueda: string = "";

    descripcion: string;
    automatico: string;
    toleria: string;
    centroEmisor: string;
    tolerX: string;
    pesoMaximo: string;
    codigoSAP: string;

    modelBusquedaVisible = false;
    codigoInputDisable = false;

    itemsPerPage = 5;

    balanzas: any;

    checkPermisos() {
        this.securityService.tienePermisoRedirect("ABM BALANZAS");
    }

    setTabs() {
        this.setMenuSeccionTab("ryd-mantenimiento", "Balanzas");
    }

    getDataInputs() {
        this.mensajeComponent.setMsgsEmpty();
        this.subscriptionDropDowns = this.service.getInputDropDown().subscribe(
            result => {
                //this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {

                    this.filtroTipo = result.data.tipo;
                    this.filtroCodigoCabezal = result.data.codigoCabezal;
                    this.filtroITC = result.data.itc;
                    this.filtroNroPuesto = result.data.nroPuesto;
                    this.filtroTipoAcceso = result.data.tipoAcceso;
                }
            },
            error => {
                //this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
    }

    getNroPuesto() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.filtroNroPuesto = null;
        this.service.getNroPuesto(this.itcSelected).subscribe(
            result => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.data = result.data;
                    this.filtroNroPuesto = result.data.nroPuesto;
                    if (this.nroPuestoSelected != undefined && this.nroPuestoSelected  != "") {
                        this.InputNroPuestoComponent.setSelectItem(this.nroPuestoSelected );
                    }
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    }

    aplicarTabla(codigo: string) {
        this.vaciarInputs();
        this.codigo = codigo
        this.aplicar();
        return false;
    }

    aplicar() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.aplicar(this.codigo).subscribe(
            result => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.codigoInputDisable = true;
                    this.data = result.data;
                    if (result.data.length > 0) {
                        this.descripcion = result.data[0].descripcion;
                        this.automatico = result.data[0].automatico;
                        this.toleria = result.data[0].toleria;
                        this.centroEmisor = result.data[0].centroEmisor;
                        this.tolerX = result.data[0].tolerX;
                        this.pesoMaximo = result.data[0].pesoMaximo;
                        this.codigoSAP = result.data[0].codigoSAP;
                        this.setFiltroTipo(result.data[0].tipo);
                        this.setFiltroCodigoCabezal(result.data[0].codigoCabezal);
                        this.setFiltroNroPuesto(result.data[0].nroPuesto);
                        this.setFiltroTipoAcceso(result.data[0].tipoAcceso);
                        this.setFiltroITC(result.data[0].itc);
                    }
                    

                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    }

    modalBusquedaShow() {
        this.modelBusquedaVisible = true;
        this.codigoInputDisable = true;
        return false;
    }

    modalBusquedaHide() {
        this.modelBusquedaVisible = false;
        this.codigoInputDisable = false;
        this.vaciarInputsBusqueda();
        return false;
    }

    isVisibleSearch() {
        return this.balanzas != undefined && this.balanzas.length != 0;
    }

    buscarBalanza() {
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service.buscarBalanza(this.descripcionBusqueda, this.tipoBusqueda, this.cabezalBusqueda, this.numeroSAPBusqueda).subscribe(
                result => {
                    this.visibleButton = true;
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.balanzas = result.data;
                    }
                },
                error => {
                    this.visibleButton = true;
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.visibleButton = true;
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    guardarBalanza() {
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service.guardarBalanza(this.codigo, this.descripcion, this.automatico, this.toleria, this.centroEmisor, this.tolerX, this.tipoSelected, this.pesoMaximo, this.codigoSAP, this.codigoCabezalSelected, this.itcSelected, this.nroPuestoSelected, this.tipoAccesoSelected).subscribe(
                result => {
                    this.visibleButton = true;
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.vaciarInputs();
                        this.mensajeComponent.setSuccessMsg(result.data);
                    }
                },
                error => {
                    this.visibleButton = true;
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.visibleButton = true;
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    actualizarBalanza() {
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service.actualizarBalanza(this.codigo, this.descripcion, this.automatico, this.toleria, this.centroEmisor, this.tolerX, this.tipoSelected, this.pesoMaximo, this.codigoSAP, this.codigoCabezalSelected, this.itcSelected, this.nroPuestoSelected, this.tipoAccesoSelected).subscribe(
                result => {
                    this.visibleButton = true;
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.vaciarInputs();
                        this.mensajeComponent.setSuccessMsg(result.data);
                    }
                },
                error => {
                    this.visibleButton = true;
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.visibleButton = true;
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    borrarBalanza() {
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service.borrarBalanza(this.codigo).subscribe(
                result => {
                    this.visibleButton = true;
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.vaciarInputs();
                        this.mensajeComponent.setSuccessMsg(result.data);
                    }
                },
                error => {
                    this.visibleButton = true;
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.visibleButton = true;
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    cancelarBalanza() {
        this.codigoInputDisable = false;
        this.codigo = "";
        this.vaciarInputs();
        return false;
    }

    setFiltroTipo(tipo: string) {
        this.tipoSelected = tipo;
        this.InputTipoComponent.setSelectItem(tipo);
    }

    setFiltroCodigoCabezal(codigoCabezal: string) {
        this.codigoCabezalSelected = codigoCabezal;
        this.InputCodigoCabezalComponent.setSelectItem(codigoCabezal);
    }

    setFiltroITC(itc: string) {
        this.itcSelected = itc;
        this.InputITCComponent.setSelectItem(itc);
        this.getNroPuesto();
    }

    setFiltroNroPuesto(nroPuesto: string) {
        this.nroPuestoSelected = nroPuesto;
    }

    setFiltroTipoAcceso(tipoAcceso: string) {
        this.tipoAccesoSelected = tipoAcceso;
        this.InputTipoAccesoComponent.setSelectItem(tipoAcceso);
    }

    vaciarInputs() {
        this.codigo = "";
        this.codigoInputDisable = false;
        this.setFiltroTipo("");
        this.setFiltroCodigoCabezal("");
        this.setFiltroITC("");
        this.setFiltroNroPuesto("");
        this.setFiltroTipoAcceso("");
        this.data = null;
        this.descripcion = "";
        this.automatico = "";
        this.toleria = "";
        this.centroEmisor = "";
        this.tolerX = "";
        this.pesoMaximo = "";
        this.codigoSAP = "";
        this.vaciarInputsBusqueda();
        this.modelBusquedaVisible = false;
    };
    
    vaciarInputsBusqueda() {
        this.descripcionBusqueda = "";
        this.tipoBusqueda = "";
        this.cabezalBusqueda = "";
        this.numeroSAPBusqueda = "";
        this.balanzas = null;
    }
}