import { Component, OnInit, ViewChild } from '@angular/core';
import { ContratoBaseComponent } from './../contrato.component';
import { ContratoService, ContratoVigenteService } from './../contrato.service';
import { TipoPeriodo } from '../../common/enums/TipoPeriodo';

@Component({
    selector: 'app-contrato-vigente',
    templateUrl: `contrato.vigente.component.html`,
    providers: [{ provide: ContratoService, useClass: ContratoVigenteService }]
})
export class ContratoVigenteComponent extends ContratoBaseComponent {

    tituloArchivo = "ReporteContratoVigentes.xls";
    filtroFechaKey: string = 'GcontrVig_Periodo'
    filtroFechaPeriodoDefault: TipoPeriodo = TipoPeriodo.UltimaSemana;

    setTabs() {
        this.setMenuSeccionTab("contrato", "Vigentes");
    }

    showModalTableResponsive(Contrato: any) {
        this.modalService.openModalTableResponsive("Contrato Vigente", [
            { etiqueta: "Último Movimiento", valor: Contrato.fecha },
            { etiqueta: "Contrato Molinos", valor: Contrato.nroContrato },
            { etiqueta: "Contrato Proveedor", valor: Contrato.contrvend },
            { etiqueta: "Estado Boleto", valor: Contrato.estado },
            { etiqueta: "Producto", valor: Contrato.material },
            { etiqueta: "TipoContrato", valor: Contrato.tipoContrato },
            { etiqueta: "Pactado", valor: Contrato.cantKilosString },
            { etiqueta: "Entregado", valor: Contrato.aplicacionesString },
            { etiqueta: "Liquidado", valor: Contrato.liquidadoString }
        ]);
        return false;
    }
}