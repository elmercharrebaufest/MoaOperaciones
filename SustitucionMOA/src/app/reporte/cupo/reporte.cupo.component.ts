import { Component, OnInit, ViewChild } from '@angular/core';
import { ReporteBaseComponent } from './../reporte.component';
import { ReporteService, ReporteCupoService } from './../reporte.service';

@Component({
    selector: 'app-reporte-cupo',
    templateUrl: `reporte.cupo.component.html`,
    providers: [{ provide: ReporteService, useClass: ReporteCupoService }]
})
export class ReporteCupoComponent extends ReporteBaseComponent {

    setTabs() {
        this.setMenuSeccionTab("reporte", "Cupos");
    }



}