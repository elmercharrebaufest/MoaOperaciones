import { Component, OnInit, ViewChild } from '@angular/core';
import { ContratoBaseComponent } from './../contrato.component';
import { ContratoService, ContratoAmpliacionService } from './../contrato.service';

@Component({
    selector: 'app-contrato-ampliacion',
    templateUrl: `./app/contrato/ampliacion/contrato.ampliacion.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: ContratoService, useClass: ContratoAmpliacionService }]
})
export class ContratoAmpliacionComponent extends ContratoBaseComponent {

    tituloArchivo = "ReporteAnulaciones.xls";

    setTabs() {
        this.setMenuSeccionTab("contrato", "Ampliaciones");
    }

    showModalTableResponsive(contratoInfo: any) {
        this.modalService.openModalTableResponsive("Contrato", [
            { etiqueta: "Fecha de Anulación", valor: contratoInfo.fecha },
            { etiqueta: "Contrato Molinos", valor: contratoInfo.nroContrato },
            { etiqueta: "Contrato Proveedor", valor: contratoInfo.contrvend },
            { etiqueta: "Producto", valor: contratoInfo.material },
            { etiqueta: "Pactado", valor: contratoInfo.cantKilosString },
            { etiqueta: "Ampliado", valor: contratoInfo.ampliadoString },
            { etiqueta: "Total", valor: contratoInfo.totalString },
            { etiqueta: "Vendedor", valor: contratoInfo.vendedor }
        ]);
        return false;
    }
}