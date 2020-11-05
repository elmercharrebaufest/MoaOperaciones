import { Component, OnInit, ViewChild } from '@angular/core';
import { ReporteBaseComponent } from './../reporte.component';
import { ReporteService, ReporteContratoService } from './../reporte.service';

@Component({
    selector: 'app-reporte-contrato',
    templateUrl: `reporte.contrato.component.html`,
    providers: [{ provide: ReporteService, useClass: ReporteContratoService }]
})
export class ReporteContratoComponent extends ReporteBaseComponent {
       
    setTabs() {
        this.setMenuSeccionTab("reporte", "Contratos");
    }

   

}