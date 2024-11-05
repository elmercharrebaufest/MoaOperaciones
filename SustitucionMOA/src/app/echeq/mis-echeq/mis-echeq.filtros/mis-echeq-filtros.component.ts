import { DatePipe } from '@angular/common';
import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { DropdownComponent } from '../../../common/view-child/dropdown/dropdown.component';
import { FiltroFechaComponent } from '../../../common/view-child/filtro-fecha/filtro-fecha.component';
import { OrdenesDeCargaFasonService } from '../../../ordenes-de-carga-fason/ordenes-de-carga-fason.service';
import { MisEcheqFilter } from './mis-echeq-filter.model';
import { TipoPeriodo } from '../../../common/enums/TipoPeriodo';

@Component({
    selector: 'app-mis-echeq-filtros',
    templateUrl: './mis-echeq-filtros.component.html',
    styleUrls: ['./mis-echeq-filtros.component.css']
})
export class MisEcheqFiltrosComponent extends ListBaseComponent implements OnInit {

    //Con esto paso el evento a distintos componentes
    @Output() applyFilterEmitter = new EventEmitter<MisEcheqFilter>();

    @Output() searchDataEmitter = new EventEmitter<MisEcheqFilter>();

    @ViewChild(DropdownComponent)
    protected itemsPerPageComponent: DropdownComponent;

    @ViewChild(FiltroFechaComponent)
    protected filtroFechaComponent: FiltroFechaComponent;

    public echeqFilterModel: MisEcheqFilter;

    esUsuario: boolean = this.isAuthorized('VER ECHEQ');
    esAdmin: boolean = this.isAuthorized('VER ECHEQ ADMIN');

    constructor(private route: ActivatedRoute, protected service: OrdenesDeCargaFasonService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        public datepipe: DatePipe,
        private confirmationService: ConfirmationService) {
        super(service, navService, sessionDataService, securytiService, floatMsgService, modalService);

        this.echeqFilterModel = new MisEcheqFilter();
    }

    filtroFechaPeriodoDefault: TipoPeriodo = TipoPeriodo.UltimaSemana;
    filtroFechaKey: string = 'GEchqMisEch_Periodo';

    ngOnInit() {
        this.onChangeFecha();
    }

    public onApplyFilter() {
        this.applyFilterEmitter.next(this.echeqFilterModel);
    }

    public onChangeFecha() {
        if (this.filtroFechaComponent.getFechaIncio() == undefined || this.filtroFechaComponent.getFechaFin() == undefined) {
            this.filtroFechaComponent.setPeriodo(this.filtroFechaPeriodoDefault);
        }
        this.echeqFilterModel.periodo = this.filtroFechaComponent.periodo;
        this.echeqFilterModel.fechaInicio = this.filtroFechaComponent.getFechaIncio();
        this.echeqFilterModel.fechaFin = this.filtroFechaComponent.getFechaFin();

        this.searchDataEmitter.next(this.echeqFilterModel);
    }

    public onChangeFilter() {
        this.applyFilterEmitter.next(this.echeqFilterModel);
    }


    public buscarBoton() {
        this.onChangeFecha();
    }
}
