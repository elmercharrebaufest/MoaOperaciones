import { Component, OnInit, ViewChild } from '@angular/core';
import { SlicePipe } from '@angular/common';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { CuentaCorrientePartidasAbiertasService } from './../cuenta-corriente.service';
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
import { TipoPeriodo } from '../../common/enums/TipoPeriodo';



@Component({
    selector: 'app-cuenta-corriente-partidas-abiertas',
    templateUrl: `cuenta-corriente.partidas-abiertas.component.html`,
    providers: [CuentaCorrientePartidasAbiertasService]

})
    
export class CuentaCorrientePartidasAbiertasComponent extends CuentaCorrienteBaseComponent {

    constructor(protected service: CuentaCorrientePartidasAbiertasService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService, route, router);

        this.filtroFechaPeriodoDefault = this.isGranos() ? TipoPeriodo.UltimosDosDias : TipoPeriodo.UltimaSemana;
        this.filtroFechaKey = this.isGranos() ? 'GCCPartAb_Periodo' : (this.isNoGranos() ? 'NGCCPartAb_Periodo' : '');
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList(
            [
                new Seccion('/cuenta-corriente/simple', 'cuenta-corriente', 'Cuenta Corriente'),
                new Seccion('/cuenta-corriente/agrupada', 'cuenta-corriente', 'Detalle de Pagos'),
                new Seccion('/cuenta-corriente/partidas-abiertas', 'cuenta-corriente', 'Partidas Abiertas'),
            ]
        );
        this.getData();
    }

    tituloArchivo = "ReporteCuentasCorrientesAgrupadas.xls";
    itemsEnPantalla = 5;
    showMostrarMas = false;
    filtroNroLegal = "";

    setTabs() {
        this.setMenuSeccionTab("cuenta-corriente", "Partidas Abiertas");
    }

    isVisible(): boolean {
        if (this.data) {
            if (this.isSinAgruparVisible() || this.isAgrupadasVisible()) {
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
        this.showMostrarMas = false;
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
}