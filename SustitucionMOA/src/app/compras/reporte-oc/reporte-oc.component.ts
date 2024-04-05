import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Table } from 'primeng/table';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { Solp } from '../solp/solp';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { ComprasService } from '../compras.service';
import { Paginator } from 'primeng/components/paginator/paginator';
import { OrdenDeCompraSAPCabecera, OrdenDeCompraSap } from '../../modelos/ordenDeCompraSap';

@Component({
    selector: 'app-reporte-oc',
    templateUrl: './reporte-oc.component.html',
    styleUrls: ['./../compras.component.css', './reporte-oc.component.css']
})
export class ReporteOcComponent extends ListBaseComponent {

    protected locale: any;

    @ViewChild("tabla")
    protected tabla: Table;

    @BlockUI() blockUI: NgBlockUI;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild('myCalendar', undefined)
    private calendar: any;

    usuario: string;// = "Prueba";
    codigoProveedor: string;

    fechaDesde: string = "";
    fechaHasta: string = "";
    rangeDates: Date[];

    ordenDeCompraSap: OrdenDeCompraSap;
    Cabecera: OrdenDeCompraSAPCabecera;

    nroOc: string = "";

    ordenDeCompra: any;
    displayOrdenDeCompra: boolean;

    usuarioInterno: boolean = false;
    proveedorSeleccionado: any = null;
    proveedores: any[] = new Array();

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

        this.locale = {
            firstDayOfWeek: 0,
            dayNames: ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"],
            dayNamesShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
            dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
            monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
            monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
            today: 'Hoy',
            clear: 'Borrar'
        };
        this.codigoProveedor = sessionStorage.getItem("proveedor");

    }

    ngOnInit() {

        const permisos: string[] = JSON.parse(sessionStorage.getItem("permisos"));
        console.log(permisos);
        if (permisos) {
            if (permisos.includes("ABM SOL")
                || permisos.includes("VER TODAS SOLPS")
                || permisos.includes("VER SOLPS COMPRADOR")
            ) {
                this.usuarioInterno = true;
            }
        }
        console.log(permisos.includes("ABM SOL"));
        console.log(permisos.includes("VER TODAS SOLPS"));
        console.log(permisos.includes("VER SOLPS COMPRADOR"));
        console.log(this.usuarioInterno);
        this.obtenerReporteOrdenDeCompra();

    }

    obtenerReporteOrdenDeCompra() {
        try {
            console.log("this.codigoProveedor", this.codigoProveedor);
            let codigoProveedor = this.codigoProveedor;
            console.log("this.usuarioInterno", this.usuarioInterno);

            if (this.usuarioInterno) {
                if (!this.proveedorSeleccionado) {
                    this.floatMsgService.setInfoMsg("Debe seleccionar un proveedor.");
                    return;
                } else {
                    codigoProveedor = this.proveedorSeleccionado.CodigoProveedor;
                }
            }

            this.subscription = this.service.obtenerReporteOrdenDeCompra(this.nroOc, this.fechaDesde, this.fechaHasta, codigoProveedor).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.ordenDeCompraSap = result;
                        this.spinnerComponent.hideIt();
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);

                });
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);

            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    onBuscar() {
        this.obtenerReporteOrdenDeCompra();
    }

    returnToTodaysDate() {
        this.fechaDesde = "";
        this.fechaHasta = "";
        if (this.ordenDeCompraSap) {
            this.mensajeComponent.setMsgsEmpty();
        }
    }

    onSelect(event: any) {
        if (this.rangeDates[0] && this.rangeDates[1] == null) {
            let d = new Date(Date.parse(event));
            this.fechaDesde = `${d.getFullYear()}-${d.getMonth() + 1}-${d.getDate()}`;
        } else {
            let d = new Date(Date.parse(event));
            this.fechaHasta = `${d.getFullYear()}-${d.getMonth() + 1}-${d.getDate()}`;
            if (this.rangeDates[1]) {
                this.calendar.overlayVisible = false;
            }
        }
    }

    verDetalleOrdenDeCompra(nroOC: any) {
        this.obtenerAdjudicacion(nroOC);
        this.displayOrdenDeCompra = true;
    }

    obtenerAdjudicacion(nroOC) {
        this.blockUI.start('Cargando...');
        this.service.obtenerAdjudicacion(nroOC)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        this.ordenDeCompra = result.data;
                        this.ordenDeCompra.usuarioExterno = true;
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
    }

    cerrarOrdenDeCompra() {
        this.displayOrdenDeCompra = false;
    }

    searchProveedor(event) {
        try {
            this.subscription = this.service.listarProveedores(event.query).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.proveedores = result.data;
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                });
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }
}
