import { Component, OnInit, ViewChild } from '@angular/core';
import { LiquidacionService, LiquidacionPagaService } from './../liquidacion.service';
import { LiquidacionBaseComponent } from './../liquidacion.component';
import { SessionDataService } from './../../common/services/SessionDataService';
//import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
//import { ListBaseComponent } from './../common/base-components/list-base-component'
//import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
//import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { ModalService } from './../../common/services/ModalService';
import { TipoPeriodo } from '../../common/enums/TipoPeriodo';



@Component({
    selector: 'app-liquidacion-paga',
    templateUrl: `liquidacion.paga.component.html`,
    providers: [{ provide: LiquidacionService, useClass: LiquidacionPagaService }]
})
export class LiquidacionPagaComponent extends LiquidacionBaseComponent {

    constructor(protected service: LiquidacionPagaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    filtroFechaPeriodoDefault: TipoPeriodo = TipoPeriodo.UltimosDosDias;
    filtroFechaKey: string = 'GLiqPagas_Periodo';

    setTabs() {
        this.setMenuSeccionTab("liquidacion", "Pagas");
    }

    tituloArchivo = "ReporteLiquidacionesPagas.xls";

    showModalTableResponsive(liquidacion: any) {
        this.modalService.openModalTableResponsive("Liquidación", [
            { etiqueta: "Fecha de pago", valor: liquidacion.pago },
            { etiqueta: "Tipo", valor: liquidacion.tipo },
            { etiqueta: "Comprobante", valor: liquidacion.comprobante },
            { etiqueta: "Producto", valor: liquidacion.producto },
            { etiqueta: "Liquidacion", valor: liquidacion.liquidadoString },
            { etiqueta: "Total", valor: liquidacion.importeString },
            { etiqueta: "Contrato", valor: liquidacion.contrato },
            { etiqueta: "Detalle", valor: liquidacion.detallePago}
        ]);
        return false;
    }
}