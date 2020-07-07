import { Component, OnInit, ViewChild } from '@angular/core';
import { ContratoBaseComponent } from './../contrato.component';
import { ContratoService, ContratoFijacionService } from './../contrato.service';

@Component({
    selector: 'app-contrato-fijacion',
    templateUrl: `./app/contrato/fijacion/contrato.fijacion.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: ContratoService, useClass: ContratoFijacionService }]
})
export class ContratoFijacionComponent extends ContratoBaseComponent {

    tituloArchivo = "ReporteContratosFijaciones.xls";

    setTabs() {
        this.setMenuSeccionTab("contrato", "Fijaciones");
    }

    showModalTableResponsive(contratoInfo: any) {
        this.modalService.openModalTableResponsive("Contrato", [
            { etiqueta: "Fecha de Anulación", valor: contratoInfo.fecha },
            { etiqueta: "Contrato Molinos", valor: contratoInfo.nroContrato },
            { etiqueta: "Contrato Proveedor", valor: contratoInfo.contrvend },
            { etiqueta: "Producto", valor: contratoInfo.material },
            { etiqueta: "Fijado", valor: contratoInfo.kilosFijadosString },
            { etiqueta: "Precio/TN", valor: contratoInfo.importeString },
            { etiqueta: "Vendedor", valor: contratoInfo.vendedor }
        ]);
        return false;
    }

}