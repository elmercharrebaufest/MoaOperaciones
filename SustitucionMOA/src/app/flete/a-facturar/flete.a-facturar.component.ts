import { Component, OnInit, ViewChild } from '@angular/core';
import { FleteBaseComponent } from './../flete.component';
import { FleteService, FleteAFacturarService } from './../flete.service';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { TipoPeriodo } from '../../common/enums/TipoPeriodo';
declare var $: any;

@Component({
    selector: 'app-flete-a-facturar',
    templateUrl: `flete.a-facturar.component.html`,
    providers: [{ provide: FleteService, useClass: FleteAFacturarService }]
})
export class FleteAFacturarComponent extends FleteBaseComponent {

    tituloArchivo = "FletesAFacturar.xls";
    viajeGuardar: any;
    filtroFechaPeriodoDefault: TipoPeriodo = TipoPeriodo.UltimoMes;
    filtroFechaKey: string = 'NGFletAFact_Periodo';

    constructor(protected service: FleteService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    setTabs() {
        this.setMenuSeccionTab("flete", "Viajes A Facturar");
    }

    actualizarTarifa(proforma: any, viaje: any, event: any) {
        this.floatMsgService.setMsgsEmpty();
        var valor = 0;
        var valorString = "0";  
        if (event) {
            //valorString = this.formatearValor(event);
            //valor = parseFloat(valorString); 
            valorString = event.replace(/\./g, "").replace(',','.');
            valor = parseFloat(valorString);
            if (isNaN(valor)) {
                this.floatMsgService.setErrorMsg("El valor de tarifa no es válido");
                viaje.tarifa = "0";
                valor = 0;
                return;
            }
            viaje.tarifa = valor.toLocaleString("de-DE");          
        } else {
            viaje.tarifa = "0";
            valor = 0;
        }
        viaje.importe = ((valor / 1000) * viaje.kg) + viaje.peaje + viaje.playa;
        viaje.importeString = viaje.importe.toLocaleString("de-DE");
        proforma.totalImporte = proforma.viajeItem.map(this.importe).reduce(this.suma);
        proforma.totalImporteString = "$" + proforma.totalImporte.toLocaleString("de-DE");
    }

    importe(item: any) { return item.importe }

    suma(prev: any, next: any) { return prev + next }

    /*formatearValor(value: string) {

        value = value.replace(",", "--coma--");
        value = value.replace(".", ",");
        value = value.replace("--coma--", ".");

        return value;
    }*/

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

    /*guardarTarifas(viajeProforma: any) {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        let file = viajeProforma.pdf;
        viajeProforma.pdf = null;
        this.subscription = this.service.guardarTarifas(viajeProforma, file).subscribe(
            (result:any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {

                }
            },
            error => {
                this.floatMsgService.setErrorMsg(error.message);
            }
        );
        return false;
    }*/

    validarImporte(proforma: any) {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.validarImporte(proforma.totalImporte.toString(), proforma.proforma).subscribe(
            (result:any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    this.showModalCargaDatos(proforma.proforma);
                }
            },
            error => {
                this.floatMsgService.setErrorMsg(error.message);
            }
        );
        return false;
    }

    showModalTableResponsive(Viaje: any) {
        this.modalService.openModalTableResponsive("Viaje A Facturar", [
            { etiqueta: "CCPP", valor: Viaje.ccpp },
            { etiqueta: "Fecha", valor: Viaje.fechaCCPP },
            { etiqueta: "Patente", valor: Viaje.patente },
            { etiqueta: "KG CCPP", valor: Viaje.kgString },
            { etiqueta: "Descripción", valor: Viaje.descMat },
            { etiqueta: "Origen", valor: Viaje.origen },
            { etiqueta: "Destino", valor: Viaje.destino },
            { etiqueta: "Tarifa", valor: Viaje.tarifa },
            { etiqueta: "Peaje", valor: Viaje.peajeString },
            { etiqueta: "Playas", valor: Viaje.peajeString },
            { etiqueta: "Importe", valor: Viaje.importeString }
        ]);
        return false;
    }
}