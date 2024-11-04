import { Component, OnInit, ViewChild } from '@angular/core';
import { ContratoBaseComponent } from './../contrato.component';
import { ContratoService, ContratoFijacionService } from './../contrato.service';
import { TipoPeriodo } from '../../common/enums/TipoPeriodo';

@Component({
    selector: 'app-contrato-fijacion',
    templateUrl: `contrato.fijacion.component.html`,
    providers: [{ provide: ContratoService, useClass: ContratoFijacionService }]
})
export class ContratoFijacionComponent extends ContratoBaseComponent {

    tituloArchivo = "ReporteContratosFijaciones.xls";
    filtroFechaKey: string = 'GContrFij_Periodo'
    filtroFechaPeriodoDefault: TipoPeriodo = TipoPeriodo.UltimaSemana;

    setTabs() {
        this.setMenuSeccionTab("contrato", "Fijaciones");
    }

    showModalTableResponsive(contratoInfo: any) {
        this.modalService.openModalTableResponsive("Contrato", [
            { etiqueta: "Fecha de Anulación", valor: contratoInfo.fecha },
            { etiqueta: "Contrato Molinos", valor: contratoInfo.nroContrato },
            { etiqueta: "Contrato Proveedor", valor: contratoInfo.contrvend },
            { etiqueta: "Producto", valor: contratoInfo.material },
            { etiqueta: "TipoContrato", valor: contratoInfo.tipoContrato },
            { etiqueta: "Fijado", valor: contratoInfo.kilosFijadosString },
            { etiqueta: "Precio/TN", valor: contratoInfo.importeString },
            { etiqueta: "Vendedor", valor: contratoInfo.vendedor }
        ]);
        return false;
    }

}