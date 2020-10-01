import { Component } from '@angular/core';
import { AduanaBaseComponent } from './../../aduana.component';
import { AduanaService } from './../../aduana.service';
import { NavService } from './../../../common/services/NavService';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { SecurityService } from './../../../common/services/SecurityService';
import { ModalService } from './../../../common/services/ModalService';

@Component({
    selector: 'app-aduana-camara-muelle',
    templateUrl: `camara.muelle.component.html`,
    //providers: []
})
export class CamaraMuelleComponent extends AduanaBaseComponent {

    constructor(protected navService: NavService, protected service: AduanaService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, service, securityService, floatMsgService, modalService);
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR CAMARAS MUELLE"); }

    setTabs() {
        this.setMenuSeccionTab("aduana", "Cámaras Muelle");
    }

    ngOnInit() {
        super.ngOnInit();
        if (this.securityService.tienePermiso("CONSULTAR CAMARAS MUELLE")) {
            this.abrirCamara();
        }
    }

    abrirCamara() {
        window.open("http://10.10.105.35:8888/view/index.shtml", null, "channelmode=1,scrollbars=1,status=0,titlebar=0,toolbar=0,resizable=1");
        return false;
    }

}