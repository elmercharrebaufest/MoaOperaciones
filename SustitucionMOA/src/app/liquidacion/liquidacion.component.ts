import { Component, OnInit, ViewChild } from '@angular/core';
import { LiquidacionService } from './liquidacion.service';
import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
import { ListBaseComponent } from './../common/base-components/list-base-component'
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SessionDataService } from './../common/services/SessionDataService';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { Seccion } from './../common/models/seccion';
import { ModalService } from './../common/services/ModalService';



@Component({
    selector: 'app-liquidacion',
    template: ``,
    providers: [LiquidacionService]
})
export class LiquidacionBaseComponent extends ListBaseComponent {

    constructor(protected service: LiquidacionService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    filtroProducto: any = null;
    productoSelected: string = "";
    filtroObservacion: any = null;
    filtroVendedor: any = null;
    observacionSelected: string = "";
    filtroComprobanteOContrato: string = "";
    tituloArchivoModal: string = "";
    tituloArchivoPDF = "Documento"
    CodigoProveedorSAP: string = sessionStorage.getItem("proveedor");


    checkPermisos() {
        this.securityService.tienePermisoRedirect("CONSULTAR LIQUIDACIONES");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        sessionStorage.getItem("proveedor");
        
        this.navService.setSeccionList([
            new Seccion('/liquidacion/aprobada', 'liquidacion', 'Aprobadas'),
        ]);
        
        //this.getData();
    }

 
}
