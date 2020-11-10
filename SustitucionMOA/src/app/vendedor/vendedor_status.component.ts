import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { VendedorStatusService } from './vendedor_status.service';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { Seccion } from './../common/models/Seccion';
import { BaseComponent } from './../common/base-components/base-component';
import { SessionDataService } from './../common/services/SessionDataService';


import { ModalService } from './../common/services/ModalService';

@Component({
    selector: 'app-vendedor-status',
    templateUrl: `vendedor_status.component.html`,
    styleUrls: ['vendedor_status.component.css'],
    providers: [VendedorStatusService]
})

export class VendedorStatusComponent extends BaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(
        protected service: VendedorStatusService,
        protected navService: NavService,
        protected securityService: SecurityService,
        protected sessionDataService: SessionDataService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService,
        private route: ActivatedRoute,
        private router: Router) {
        super(navService, securityService, floatMsgService, modalService);
        this.spinnerComponent = new SpinnerComponent();
        this.mensajeComponent = new MensajeComponent();
    }

    cuit:string = "";
    datosFiscales: any;
    listaVendedores: any;
    statusHabilitado: boolean;
    statusInhabilitado: boolean;
    statusObservado: boolean;
    consultaMultiple: boolean = false;

    setTabs() {
        this.setMenuSeccionTab("vendedor", "Vendedor Estado");
    }

    ngOnInit() {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR VENDEDOR STATUS");
        //this.service.getTitulo().subscribe(titulo => this.titulo = titulo);
        var secciones = [];
        secciones.push(new Seccion('/vendedor/status', 'vendedor', 'Vendedor Estado'));
        this.navService.setSeccionList(secciones);
    }

    getData() {
        this.consultaMultiple = false;

        if (this.cuit.includes(",")) {
            this.consultaMultiple = true;
            this.getDataMultiple();
        }
        else {
            this.getDataUnico();
        }

        return false;
    }

    getDataMultiple() {
        this.datosFiscales = null;
        this.mensajeComponent.setMsgsEmpty();
  
        if ((this.cuit.match(/,/g) || []).length > 15)
        {
            this.mensajeComponent.setErrorMsg("Solo se pueden hacer busquedas de hasta 15 CUITs");
            return false;
        }
        
        this.spinnerComponent.showIt();
        this.unsubscribe();

        this.subscription = this.service.getVariosVendedoresStatus(this.cuit).subscribe(
            result => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.listaVendedores = result.data;

                    this.listaVendedores = this.listaVendedores.map(x => {
                        return {
                        ...x,
                        cssClass: this.getCssClass(x.Estado)
                        };
                    });
                 
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
        return false;
    }
    
    getCssClass(Estado:string) {
        if (Estado.toLowerCase().indexOf("inhabilitado") >= 0) {
            return "bg-danger"
        } else if (Estado.toLowerCase().indexOf("habilitado") >= 0) {
            return "bg-success"
        } else {
            return "bg-warning"
        }
    }

    getDataUnico()
    {
        this.datosFiscales = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();

        this.statusHabilitado = false;
        this.statusInhabilitado = false;
        this.statusObservado = false;

        this.subscription = this.service.getVendedorStatus(this.cuit).subscribe(
            result => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.datosFiscales = result.data;

                    if (this.datosFiscales.status.toLowerCase().indexOf("inhabilitado") >= 0) {
                        this.statusInhabilitado = true;
                    } else if (this.datosFiscales.status.toLowerCase().indexOf("habilitado") >= 0) {
                        this.statusHabilitado = true;
                    } else {
                        this.statusObservado = true;
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
    isVencida(fechaHasta: string): boolean {
        if (fechaHasta != undefined) {
            return new Date(fechaHasta) < new Date();
        }
        return false;
    }

    showModalTableExencionesResponsive(Exencion: any) {
        this.modalService.openModalTableResponsive("Exención", [
            { etiqueta: "Descripción", valor: Exencion.descripcion },
            { etiqueta: "% Exención", valor: Exencion.exencion },
            { etiqueta: "Fecha Desde", valor: Exencion.fechaDesde },
            { etiqueta: "Fecha Hasta", valor: Exencion.fechaHasta },
        ]);
        return false;
    }

    showModalTableConveniosMultilateralesResponsive(Convenio: any) {
        this.modalService.openModalTableResponsive("Convenio Multilateral", [
            { etiqueta: "Provincia", valor: Convenio.provincia },
            { etiqueta: "Coeficiente", valor: Convenio.coeficiente },
            { etiqueta: "Descripcion", valor: Convenio.descripcion },
        ]);
        return false;
    }

}