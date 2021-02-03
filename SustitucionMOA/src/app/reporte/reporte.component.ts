import { Component, OnInit, ViewChild } from '@angular/core';
import { ReporteService } from './reporte.service';
import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { SessionDataService } from './../common/services/SessionDataService';
import { SecurityService } from './../common/services/SecurityService';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { Seccion } from './../common/models/Seccion';
declare var $: any;



@Component({
    selector: 'app-reporte',
    template: ``,
    providers: [ReporteService]
})
export class ReporteBaseComponent extends ListBaseComponent {

    constructor(protected service: ReporteService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }
    data: any = [];

    datosContrato: any = new Array();
    proveedores: any = new Array();
    materiales: any = [];
    monedas: any = new Array();
    destinos: any = new Array();
    campanias: any = new Array();
    zona: any = new Array();
    bolsasSelect: any = new Array();
    bolsasConfirma: any = new Array();
    bolsasFisico: any = new Array();
    bolsasCarta: any = new Array();
    condicionVendedor: any = new Array();
    condicionFijacion: any = new Array();
    datosCompraNet: any = null;
    esCorredorEnDataAgro: boolean = false;
    keyword2 = "RazonSocial";
    autocompleteNotFoundText = "No encontrado";

    corredorId: number = null;
    proveedorId: number = null;

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR CONTRATOS"); }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList(
            [
                new Seccion('/reporte/contrato', 'reporte', 'Contratos'),
                //new Seccion('/reporte/cupo', 'reporte', 'Cupos'),
            ]
        );
    }


    selectEventProveedor(item) {
        this.proveedorId = item.Id;
        console.log("prov: ", item.Id);
    }

    onChangeSearchProveedor(term: string) {
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.buscarProveedoresConCorredor(term).subscribe(
                result => {
                    var resultlist = JSON.parse(result);

                    this.proveedores = resultlist.map(prov => {
                        return { Id: prov.Id, RazonSocial: prov.RazonSocial + " (" + prov.Cuit + ")", CUIT: prov.Cuit }
                    })
                    //this.proveedores = JSON.parse(result);
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        }
    }
    isVisible(): boolean {
        if (this.data && this.data.length != 0)
            return true;
        else
            return false;
    }

    isVisibleProveedor(): boolean {
        return this.esCorredorEnDataAgro == true;
    }




}