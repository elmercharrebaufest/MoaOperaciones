import { Component } from '@angular/core';
import { AduanaBaseComponent } from './../aduana.component';
import { AduanaService } from './../aduana.service';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { ModalService } from './../../common/services/ModalService';

@Component({
    selector: 'app-aduana-informacion-meteorologica',
    templateUrl: `./app/aduana/informacion-meteorologica/informacion-meteorologica.component.html?v=${new Date().getTime()}`,
    //providers: []
})

export class InformacionMeteorologicaComponent extends AduanaBaseComponent {

    constructor(protected navService: NavService, protected service: AduanaService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, service, securityService, floatMsgService, modalService);
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR INFORMACION METEOROL"); }

    setTabs() {
        this.setMenuSeccionTab("aduana", "Inf. Meteorológica");
    }

    ngOnInit() {
        super.ngOnInit();
        if (this.securityService.tienePermiso("CONSULTAR INFORMACION METEOROL")) {
            this.abrirInfoMet();
        }
    }

    abrirInfoMet() {
        window.open("Template/mb1.htm");
        return false;
    }

}