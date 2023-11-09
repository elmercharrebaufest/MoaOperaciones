import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { AplicacionCcppBaseComponent } from '../aplicacion-ccpp.base.component';
import { SeccionAplicacionCCPP } from '../aplicacion-ccpp.model';
import { AplicacionCcppService } from '../aplicacion-ccpp.service';

@Component({
    selector: 'app-masiva',
    templateUrl: './masiva.component.html',
    styleUrls: ['./masiva.component.css']
})
export class MasivaComponent extends AplicacionCcppBaseComponent implements OnInit, OnDestroy {
    @BlockUI() blockUI: NgBlockUI;
    
    constructor(
        protected service: AplicacionCcppService,
        protected navService: NavService,
        protected sessionDataService: SessionDataService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService
    ) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    archivo: File | null = null;

    ngOnInit() {
        this.crearSecciones();
        this.setMenuSeccionTab(SeccionAplicacionCCPP, 'Carga masiva');
    }

    descargarTemplateCsv() {
        alert("req desc template");
    }

    cargarArchivo(event: any) {
        let archivos: FileList = event.target.files;
        if (archivos.length > 0) {
            this.archivo = archivos[0];
        }
    }
}