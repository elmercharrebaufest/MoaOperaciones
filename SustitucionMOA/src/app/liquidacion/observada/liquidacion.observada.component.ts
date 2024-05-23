import { Component, OnInit, ViewChild } from '@angular/core';
import { LiquidacionService, LiquidacionObservadaService } from './../liquidacion.service';
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
import { SendDataService } from '../../consulta/send-data.service';



@Component({
    selector: 'app-liquidacion-observada',
    templateUrl: `liquidacion.observada.component.html`,
    providers: [{ provide: LiquidacionService, useClass: LiquidacionObservadaService }]
})
export class LiquidacionObservadaComponent extends LiquidacionBaseComponent {

    constructor(
        protected service: LiquidacionObservadaService,
        protected navService: NavService,
        protected sessionDataService: SessionDataService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService,
        private sendDataService: SendDataService
    ) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    setTabs() {
        this.setMenuSeccionTab("liquidacion", "Observadas");
    }

    acortar(value: string) {
        return value.slice(0, 8);
    }

    tituloArchivo = "ReporteLiquidacionesObservadas.xls";

    showModalTableResponsive(liquidacion: any) {
        this.modalService.openModalTableResponsive("Liquidación", [
            { etiqueta: "Vencimiento", valor: liquidacion.emitido },
            { etiqueta: "Tipo", valor: liquidacion.tipo },
            { etiqueta: "Comprobante", valor: liquidacion.comprobante },
            { etiqueta: "Producto", valor: liquidacion.producto },
            { etiqueta: "Liquidacion", valor: liquidacion.liquidadoString },
            { etiqueta: "Total", valor: liquidacion.importeString },
            { etiqueta: "Contrato", valor: liquidacion.contrato },
            { etiqueta: "Falta", valor: liquidacion.observaciones }
        ]);
        return false;
    }

    navegarAConsulta(liquidacion: any) {
        if (liquidacion.linkConsulta) {
            this.sendDataService.setDatosLiquidacionObservada({
                Tipo: liquidacion.tipo,
                NroComprobante: liquidacion.comprobante
            })
            return this.goToSeccionParam("consulta", "crear-consulta")
        }
    }
}