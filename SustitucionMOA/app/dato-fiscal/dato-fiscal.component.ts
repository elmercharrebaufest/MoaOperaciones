import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { DatoFiscalService } from './dato-fiscal.service';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { Seccion } from './../common/models/Seccion';
import { BaseComponent } from './../common/base-components/base-component';
import { SessionDataService } from './../common/services/SessionDataService';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import { ModalService } from './../common/services/ModalService';

@Component({
    selector: 'app-dato-fiscal',
    templateUrl: `./app/dato-fiscal/dato-fiscal.component.html?v=${new Date().getTime()}`,
    providers: [DatoFiscalService]
})
export class DatoFiscalBaseComponent extends BaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(
        protected service: DatoFiscalService,
        protected navService: NavService,
        protected securityService: SecurityService,
        protected sessionDataService: SessionDataService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService,
        private route: ActivatedRoute,
        private router: Router ) {
            super(navService, securityService, floatMsgService, modalService);
            this.spinnerComponent = new SpinnerComponent();
            this.mensajeComponent = new MensajeComponent();

            this.nombre = sessionStorage.getItem("nombre");
            this.proveedor = sessionStorage.getItem("proveedor");

            sessionDataService.nombre$.subscribe(
                nombre => {
                    this.nombre = nombre;
                });
            sessionDataService.proveedor$.subscribe(
                proveedor => {
                    this.proveedor = proveedor;
                });

    }

    titulo = "";
    nombre = "";
    proveedor = "";
    tipoUsuario: string;
    vendedorId: string;
    itemsPerPage = sessionStorage.getItem("itemsPerPage") ? sessionStorage.getItem("itemsPerPage") : "10";
    dropdownType: string = 'numberItems';
    datosFiscales: any;

    setTabs() {
        this.setMenuSeccionTab("dato-fiscal", "Mi Situacion Fiscal");
    }

    ngOnInit() {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR DATOS FISCALES");
        //this.service.getTitulo().subscribe(titulo => this.titulo = titulo);
        var secciones = [];
        if (this.isAuthorized("CONSULTAR DATOS FISCALES"))
            secciones.push(new Seccion('/dato-fiscal/situacion-fiscal', 'dato-fiscal', 'Mi Situacion Fiscal'));

        if (this.isAuthorized("CONSULTAR VENDEDORES") && this.isCorredor())
            secciones.push(new Seccion('/dato-fiscal/vendedor', 'dato-fiscal', 'Mis Vendedores'));

        if (this.isAuthorized("CONSULTAR DOCUMENTACION"))
            secciones.push(new Seccion('/dato-fiscal/documentacion', 'dato-fiscal', 'Documentacion'));
        
        this.navService.setSeccionList(secciones);

        this.getData();
    }

    setItemsPerPage(numberOfItems: string) {
        this.itemsPerPage = numberOfItems;
    }

    getData() {
        this.datosFiscales = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.route.params.forEach((params: Params) => {
            this.vendedorId = params['id'];
            if (params['id2'] != undefined && params['id2'] != "" && params['id2'] != null) {
                this.nombre = params['id2'];
                this.proveedor = params['id'];
            }
            this.subscription = this.service.getDatosFiscales(this.vendedorId).subscribe(
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
                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        });
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

    showModalTableCuentasHabilitadasResponsive(Cuenta: any) {
        this.modalService.openModalTableResponsive("Cuenta Habilitada", [
            { etiqueta: "Banco", valor: Cuenta.banco },
            { etiqueta: "Cuenta Nº", valor: Cuenta.cuenta },
            { etiqueta: "CBU", valor: Cuenta.cbu },
            { etiqueta: "Tipo de cuenta", valor: Cuenta.tipoCta },
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