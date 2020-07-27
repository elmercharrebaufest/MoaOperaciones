import { Component, OnInit, ViewChild } from '@angular/core';
import { ContratoBaseComponent } from './../contrato.component';
import { ContratoService, ContratoAnulacionService } from './../contrato.service';

@Component({
    selector: 'app-contrato-anulacion',
    templateUrl: `./app/contrato/anulacion/contrato.anulacion.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: ContratoService, useClass: ContratoAnulacionService }]
})
export class ContratoAnulacionComponent extends ContratoBaseComponent {

    tituloArchivo = "ReporteContratosAnulaciones.xls";

    setTabs() {
        this.setMenuSeccionTab("contrato", "Anulaciones");
    }

    showModalTableResponsive(contratoInfo: any) {
        this.modalService.openModalTableResponsive("Contrato", [
            { etiqueta: "Fecha de Anulación", valor: contratoInfo.fecha },
            { etiqueta: "Contrato Molinos", valor: contratoInfo.nroContrato },
            { etiqueta: "Contrato Proveedor", valor: contratoInfo.contrvend },
            { etiqueta: "Producto", valor: contratoInfo.material },
            { etiqueta: "Pactado", valor: contratoInfo.cantKilosString },
            { etiqueta: "Anulado", valor: contratoInfo.anuladoString },
            { etiqueta: "Total", valor: contratoInfo.totalString },
            { etiqueta: "Vendedor", valor: contratoInfo.vendedor }
        ]);
        return false;
    }
}