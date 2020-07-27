import { Component, OnInit, ViewChild } from '@angular/core';
import { RYDMantenimientoService, RYDMantenimientoCommoditiesService } from './../ryd-mantenimiento.service';
import { RYDMantenimientoBaseComponent } from './../ryd-mantenimiento.component';
import { Balanza, Commodity } from './../ryd-mantenimiento';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { Subscription } from 'rxjs';
import { DropdownComponent, DropdownOption } from './../../common/view-child/dropdown/dropdown.component';
import { ModalService } from './../../common/services/ModalService';



@Component({
    selector: 'app-ryd-mantenimiento-commodities',
    templateUrl: `./app/ryd-mantenimiento/commodities/commodities.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: RYDMantenimientoService, useClass: RYDMantenimientoCommoditiesService }]
})
export class RYDMantenimientoCommoditiesComponent extends RYDMantenimientoBaseComponent {

    @ViewChild(DropdownComponent)
    protected InputCommoditiesComponent: DropdownComponent;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: RYDMantenimientoCommoditiesService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, securityService, floatMsgService, modalService);
        this.InputCommoditiesComponent = new DropdownComponent();
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    filtroCommoditie = Array<any>();
    commoditieSelected = "";
    data: any;

    public balanzas: Balanza = new Balanza();
    descripcion: string = "";
    materialSAP: string = "";
    almacenOrigen: string = "";
    guardarNuevo: Commodity = new Commodity();

    checkPermisos() {
        this.securityService.tienePermisoRedirect("ABM COMMODITIES");
    }

    setTabs() {
        this.setMenuSeccionTab("ryd-mantenimiento", "Commodities");
    }

    getFiltros() {
        this.mensajeComponent.setMsgsEmpty();
        this.subscriptionDropDowns = this.service.getFiltros().subscribe(
            result => {
                //this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.filtroCommoditie = result.data;
                }
            },
            error => {
                //this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    setFiltroCommoditie(commoditie: string) {
        this.commoditieSelected = commoditie;
        this.getData();
    }

    getDataInputs() {
        this.subscriptionDropDowns = this.service.getInputCommodities().subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {

                    this.filtroCommoditie = result.data.commodities;
               }
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
    }


    getData() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getCommodities(this.commoditieSelected).subscribe(
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
                    this.descripcion = result.data.label;
                    this.materialSAP = result.data.MaterialSap;
                    this.almacenOrigen = result.data.AlmacenOrigen;
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    }

    guardarCommodity() {
        this.initRequestBotones();
        try {
            this.unsubscribe();
            this.subscription = this.service.guardarCommodity(this.materialSAP, this.almacenOrigen, this.descripcion).subscribe(
                result => {
                    this.visibleButton = true;
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.endRequestBotones();
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.endRequestBotones();
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.vaciarInputs();
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.getDataInputs();
                        this.commoditieSelected = null;
                        this.endRequestBotones();
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


    actualizarCommodity() {
        this.initRequestBotones();
        try {
            this.unsubscribe();
            this.subscription = this.service.actualizarCommodity(this.materialSAP, this.almacenOrigen, this.descripcion, this.commoditieSelected).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.endRequestBotones();
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.endRequestBotones();
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.vaciarInputs();
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.getDataInputs();
                        this.endRequestBotones();
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

    borrarCommodity() {
        this.initRequestBotones();
        try {
            this.unsubscribe();
            this.subscription = this.service.borrarCommodity(this.commoditieSelected).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.endRequestBotones();
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.endRequestBotones();
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.vaciarInputs();
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.getDataInputs();
                        this.endRequestBotones();
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


    setInputCommodity(value: string) {
        this.balanzas.commodityId = value;
    }

    vaciarInputs() {
        this.InputCommoditiesComponent.setSelectItem("");
        this.descripcion = "";
        this.materialSAP = "";
        this.almacenOrigen = "";
    }

}