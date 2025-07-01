import { Component, OnInit, ViewChild } from '@angular/core';
import { RYDMantenimientoService, RYDMantenimientoExportadorService } from './../ryd-mantenimiento.service';
import { RYDMantenimientoBaseComponent } from './../ryd-mantenimiento.component';
import { Balanza, Exportador } from './../ryd-mantenimiento';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';
import { Subscription } from 'rxjs';
import { DropdownComponent, DropdownOption } from './../../common/view-child/dropdown/dropdown.component';




@Component({
    selector: 'app-ryd-mantenimiento-exportadores',
    templateUrl: `exportadores.component.html`,
    providers: [{ provide: RYDMantenimientoService, useClass: RYDMantenimientoExportadorService }]
})
export class RYDMantenimientoExportadorComponent extends RYDMantenimientoBaseComponent {

    @ViewChild(DropdownComponent)
    protected InputExportadoresComponent: DropdownComponent;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: RYDMantenimientoExportadorService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, securityService, floatMsgService, modalService);
        this.InputExportadoresComponent = new DropdownComponent();
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }


    filtroExportador = Array<any>();
    exportadorSelected = "";
    data: any;

    public balanzas: Balanza = new Balanza();
    descripcion: string = "";
    almacenSAP: string = "";
    guardarNuevo: Array<Exportador> = new Array<Exportador>();

    subscriptions = new Subscription();

    checkPermisos() {
        this.securityService.tienePermisoRedirect("ABM EXPORTADORES");
    }

    setTabs() {
        this.setMenuSeccionTab("ryd-mantenimiento", "Exportadores");
    }

    getFiltros() {
        this.mensajeComponent.setMsgsEmpty();
        this.service.getFiltros().subscribe(
            (result:any) => {
                //this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.filtroExportador = result.data;
                }
            },
            error => {
                //this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    setFiltroExportador(exportador: string) {
        this.exportadorSelected = exportador;
        this.getData();
    }

    getDataInputs() {
        this.mensajeComponent.setMsgsEmpty();
        this.subscriptionDropDowns = this.service.getInputExportador().subscribe(
            (result:any) => {
                //this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {

                    this.filtroExportador = result.data.exportadores;
                }
            },
            error => {
                //this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
    }

    getData() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getExportador(this.exportadorSelected).subscribe(
            (result:any) => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.data = result.data;
                    this.descripcion = result.data.label;
                    this.almacenSAP = result.data.AlmacenSap;
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    }


    guardarExportador() {
        this.initRequestBotones();
        try {
            this.unsubscribe();
            this.subscription = this.service.guardarExportador(this.almacenSAP, this.descripcion).subscribe(
                (result:any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.endRequestBotones();
                    } else if (result.info != undefined) {
                        this.endRequestBotones();
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.vaciarInputs();
                        this.getDataInputs();
                        this.endRequestBotones();
                        this.mensajeComponent.setSuccessMsg(result.data);
                    }
                },
                error => {
                    this.endRequestBotones();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.endRequestBotones();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }


    actualizarExportador() {
        this.initRequestBotones();
        try {
            this.unsubscribe();
            this.subscription = this.service.actualizarExportador(this.almacenSAP, this.descripcion, this.exportadorSelected).subscribe(
                (result:any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                        this.endRequestBotones();
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                        this.endRequestBotones();
                    } else {
                        this.vaciarInputs();
                        this.getDataInputs();
                        this.endRequestBotones();
                        this.mensajeComponent.setSuccessMsg(result.data);
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.endRequestBotones();
                }

            );
        } catch (e) {
            this.endRequestBotones();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    borrarExportador() {
        this.initRequestBotones();
        try {
            this.unsubscribe();
            this.subscription = this.service.borrarExportador(this.exportadorSelected).subscribe(
                (result:any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                        this.endRequestBotones();
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                        this.endRequestBotones();
                    } else {
                        this.vaciarInputs();
                        this.getDataInputs();
                        this.endRequestBotones();
                        this.mensajeComponent.setSuccessMsg(result.data);
                    }
                },
                error => {
                    this.endRequestBotones();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.endRequestBotones();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }


    setInputExportador(value: string) {
        this.balanzas.exportadorId = value;
    }

    vaciarInputs() {
        this.InputExportadoresComponent.setSelectItem("");
        this.descripcion = "";
        this.almacenSAP = "";
    }

}