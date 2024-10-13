import { Component, OnInit, ViewChild } from '@angular/core';
import { FleteBaseComponent } from './../flete.component';
import { FleteService, FleteFacturadoService } from './../flete.service';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { TipoPeriodo } from '../../common/enums/TipoPeriodo';
declare var $: any;

@Component({
    selector: 'app-flete-facturado',
    templateUrl: `flete.facturado.component.html`,
    providers: [{ provide: FleteService, useClass: FleteFacturadoService }]
})
export class FleteFacturadoComponent extends FleteBaseComponent {

    constructor(protected service: FleteService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    tituloArchivo = "FletesFacturados.xls";
    filtroFechaPeriodoDefault: TipoPeriodo = TipoPeriodo.UltimoMes;
    filtroFechaKey: string = 'NGFletFact_Periodo';

    setTabs() {
        this.setMenuSeccionTab("flete", "Viajes Facturados");
    }

    ngAfterViewInit(): void {
        $(document).on("mouseover", '.form_datetime', function () {
            $(".form_datetime").datetimepicker({
                format: 'yyyy-mm-dd',
                language: 'es',
                weekStart: 1,
                todayBtn: 1,
                autoclose: 1,
                todayHighlight: 1,
                startView: 2,
                forceParse: 0,
                showMeridian: 1,
                pickTime: false,
                minView: 2,
                maxView: 4
            });
        });
    }

    showModalTableResponsive(Viaje: any) {
        this.modalService.openModalTableResponsive("Viaje Factuado", [
            { etiqueta: "CCPP", valor: Viaje.ccpp },
            { etiqueta: "Fecha", valor: Viaje.fechaCCPP },
            { etiqueta: "Patente", valor: Viaje.patente },
            { etiqueta: "KG CCPP", valor: Viaje.kgString },
            { etiqueta: "Descripción", valor: Viaje.descMat },
            { etiqueta: "Origen", valor: Viaje.origen },
            { etiqueta: "Destino", valor: Viaje.destino },
            { etiqueta: "Factura Legal Nº", valor: Viaje.factura },
            { etiqueta: "Factura Fecha", valor: Viaje.fechaEmision },
            { etiqueta: "Tarifa", valor: Viaje.tarifaString },
            { etiqueta: "Peaje", valor: Viaje.peajeString },
            { etiqueta: "Playas", valor: Viaje.peajeString },
            { etiqueta: "Importe", valor: Viaje.importeString }
        ]);
        return false;
    }

}