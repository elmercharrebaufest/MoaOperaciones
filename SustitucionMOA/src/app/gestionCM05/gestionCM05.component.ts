import { Component, OnInit } from '@angular/core';
import { CabeceraCM05, DetalleCM05 } from './gestionCM05';
import { TableModule } from 'primeng/table';
import { GestionCM05Service } from './gestionCM05.service';
import { NavService } from '../common/services/NavService';
import { SessionDataService } from '../common/services/SessionDataService';
import { SecurityService } from '../common/services/SecurityService';
import { ActivatedRoute, Router } from '@angular/router';
import { ModalService } from '../common/services/ModalService';
import { FloatMsgService } from '../common/services/FloatMsgService';
import { ListBaseComponent } from '../common/base-components/list-base-component';
import { SelectItem } from 'primeng/api';
import { MessageService } from 'primeng/api';

@Component({
    templateUrl: './gestionCM05.component.html',
    styleUrls: ['./gestionCM05.component.css'],
    providers: [{ provide: GestionCM05Service, useClass: GestionCM05Service }, MessageService]
})
export class GestionCM05Component extends ListBaseComponent {

    displayDialog: boolean;

    cabeceraCols: any[];
    cabeceras: CabeceraCM05[];

    cabecera: CabeceraCM05;
    selectedCabecera: CabeceraCM05;

    detalleCols: any[];
    detalles: DetalleCM05[];

    constructor(protected service: GestionCM05Service,
                protected navService: NavService,
                protected sessionDataService: SessionDataService,
                protected securityService: SecurityService,
                protected floatMsgService: FloatMsgService,
                protected modalService: ModalService,
                protected route: ActivatedRoute,
                protected router: Router,
                private messageService: MessageService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    ngOnInit() {
        this.service.listarCabeceras().subscribe(result => {
            this.cabeceras = result;
            this.cabeceras.forEach(x => {
                x.FechaCarga = x.FechaCarga == undefined ? null : new Date(this.getDateFromAspNetFormat(x.FechaCarga));
                x.FechaUltimaModificacion = new Date(this.getDateFromAspNetFormat(x.FechaUltimaModificacion));
            });
        });

        this.cabeceraCols = [
            { field: 'Id', header: 'Id', filterType: 'text' },
            { field: 'CUIT', header: 'CUIT', filterType: 'text' },
            { field: 'Anticipo', header: 'Anticipo', filterType: 'text' },
            { field: 'Sede', header: 'Sede', filterType: 'text' },
            { field: 'FechaCarga', header: 'Fecha carga', filterType: 'date' },
            { field: 'FechaUltimaModificacion', header: 'Última modificación', filterType: 'date' },
        ];

        this.detalleCols = [
            { field: 'Jurisdiccion', header: 'Jurisdicción', filterType: 'text' },
            { field: 'FechaInicio', header: 'Fecha inicio', filterType: 'date' },
            { field: 'FechaCese', header: 'Fecha cese', filterType: 'date' },
            { field: 'CoeficienteIngresos', header: 'Coef. ingresos', filterType: 'text' },
            { field: 'CoeficienteGastos', header: 'Coef. gastos', filterType: 'text' },
            { field: 'CoeficienteUnificado', header: 'Coef. unificado', filterType: 'text' },
            { field: 'FechaUltimaModificacion', header: 'Última modificación', filterType: 'date' },
        ];
    }

    onCabeceraClick(data) {
        this.cabecera = {
            Id: data.Id,
            Anticipo: data.Anticipo,
            CUIT: data.CUIT,
            Sede: data.Sede,
            FechaCarga: data.FechaCarga,
            FechaUltimaModificacion: data.FechaUltimaModificacion,
        };
        this.service.listarDetalles(this.cabecera.Id).subscribe(result => {
            this.detalles = result;
            this.detalles.forEach(x => {
                x.FechaCese = x.FechaCese == undefined ? null : new Date(this.getDateFromAspNetFormat(x.FechaCese));
                x.FechaInicio = x.FechaInicio == undefined ? null : new Date(this.getDateFromAspNetFormat(x.FechaInicio));
                x.FechaUltimaModificacion = new Date(this.getDateFromAspNetFormat(x.FechaUltimaModificacion));
            });
        });
        setTimeout(() => {
            this.displayDialog = true;
        }, 400);
    }

    close() {
        this.cabecera = null;
        this.detalles = null;
        this.displayDialog = false;
    }
}