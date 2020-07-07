import { Component, OnInit, ViewChild, Renderer, ElementRef, AfterViewInit } from '@angular/core';
import { LiquidacionService, LiquidacionAprobadaService } from './../liquidacion.service';
import { LiquidacionBaseComponent } from './../liquidacion.component';
import { SessionDataService } from './../../common/services/SessionDataService';
//import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
//import { ListBaseComponent } from './../common/base-components/list-base-component'
//import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
//import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { SecurityService } from './../../common/services/SecurityService';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

@Component({
    selector: 'app-liquidacion-aprobada',
    templateUrl: `./app/liquidacion/aprobada/liquidacion.aprobada.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: LiquidacionService, useClass: LiquidacionAprobadaService }]
})
export class LiquidacionAprobadaComponent extends LiquidacionBaseComponent {


    constructor(protected service: LiquidacionAprobadaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected elementRef: ElementRef, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    tituloArchivo = "ReporteLiquidacionesAprobadas.xls";
    tituloArchivoModal = "ReporteVinculacion";

    setTabs() {
        this.setMenuSeccionTab("liquidacion", "Aprobadas");
    }

    showModalTableResponsive(liquidacion: any) {
        this.modalService.openModalTableResponsive("Liquidación", [
            { etiqueta: "Vencimiento", valor: liquidacion.emitido },
            { etiqueta: "Tipo", valor: liquidacion.tipo },
            { etiqueta: "Comprobante", valor: liquidacion.comprobante },
            { etiqueta: "Producto", valor: liquidacion.producto },
            { etiqueta: "Liquidacion", valor: liquidacion.liquidadoString },
            { etiqueta: "Total", valor: liquidacion.importeString },
            { etiqueta: "Contrato", valor: liquidacion.contrato }
        ]);
        return false;
    }
}