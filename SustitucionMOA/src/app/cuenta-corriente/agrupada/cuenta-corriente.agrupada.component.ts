import { Component, OnInit, ViewChild } from '@angular/core';
import { SlicePipe } from '@angular/common';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { CuentaCorrienteAgrupadaService } from './../cuenta-corriente.service';
import { CuentaCorrienteBaseComponent } from './../cuenta-corriente.component';
import { FiltroFechaComponent } from './../../common/view-child/filtro-fecha/filtro-fecha.component';
import { ListBaseComponent } from './../../common/base-components/list-base-component'
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { DropdownComponent, DropdownOption } from './../../common/view-child/dropdown/dropdown.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { Seccion } from '../../common/models/seccion';



@Component({
    selector: 'app-cuenta-corriente-agrupada',
    templateUrl: `cuenta-corriente.agrupada.component.html`,
    providers: [CuentaCorrienteAgrupadaService]

})
    
export class CuentaCorrienteAgrupadaComponent extends CuentaCorrienteBaseComponent {

    constructor(protected service: CuentaCorrienteAgrupadaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService, route, router);

        this.granosSelected = sessionStorage.getItem("granosSelected");

        sessionDataService.granosSelected$.subscribe(
            granosSelected => {
                this.granosSelected = granosSelected;
            });

    }

    tituloArchivo = "ReporteCuentasCorrientesAgrupadas.xls";
    itemsEnPantalla = 5;
    showMostrarMas = true;
    filtroNroLegal = "";
    granosSelected: string;

    setTabs() {
        this.setMenuSeccionTab("cuenta-corriente", "Detalle de pagos");
    }

    isVisible(): boolean {
        if (this.data) {
            if (this.isSinAgruparVisible() || this.isAgrupadasVisible) {
                return true;
            }else
                return false;
        }
        else
            return false;
    }

    isSinAgruparVisible() {
        return this.data.cuentasCorrientesSinAgrupar && this.data.cuentasCorrientesSinAgrupar.cuentasCorrientes.length != 0;
    }

    isAgrupadasVisible() {
        return this.data.cuentasCorrientesAgrupadas && this.data.cuentasCorrientesAgrupadas.length != 0;
    }

    verMas() {
        this.itemsEnPantalla = this.itemsEnPantalla + 5;
        if (this.data.cuentasCorrientesAgrupadas.length <= this.itemsEnPantalla) {
            this.showMostrarMas = false;
        }
        return false;
    }

    protected vaciarFiltros() {
        this.filtroNroLegal = "";
        this.itemsEnPantalla = 5;
        this.showMostrarMas = true;
    }

    isGranos() {
        return this.granosSelected == "G";
    }

    showModalTableAgrupadaResponsive(CuentaCorriente: any, agrupador: string) {
        this.modalService.openModalTableResponsive("Detalle de pagos", [
            { etiqueta: "Agrupador", valor: agrupador },
            { etiqueta: "F. Emisión", valor: CuentaCorriente.docDate },
            { etiqueta: "Descripción", valor: CuentaCorriente.descripcion },
            { etiqueta: "Nº Legal", valor: CuentaCorriente.xblnr },
            { etiqueta: "Contrato", valor: CuentaCorriente.contrato },
            { etiqueta: "Importe AR$", valor: CuentaCorriente.importeArgString }
        ]);
        return false;
    }



    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList(
            [
                new Seccion('/cuenta-corriente/simple', 'cuenta-corriente', 'Cuenta Corriente'),
                new Seccion('/cuenta-corriente/agrupada', 'cuenta-corriente', 'Detalle de pagos'),
            ]
        );
        this.getData();
    }

}