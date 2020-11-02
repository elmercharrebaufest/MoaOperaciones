import { Component, OnInit, ViewChild } from '@angular/core';
import { AduanaService } from './../../aduana.service';
import { AduanaBaseComponent } from './../../aduana.component';
import { SessionDataService } from './../../../common/services/SessionDataService';
import { NavService } from './../../../common/services/NavService';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { SecurityService } from './../../../common/services/SecurityService';
import { MensajeComponent } from './../../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../../common/view-child/spinner/spinner.component';
import { ModalService } from './../../../common/services/ModalService';

@Component({
    selector: 'app-aduana-camara-consolidacion',
    templateUrl: `camara.consolidacion.component.html`,
    //providers: []
})
export class CamaraConsolidacionComponent extends AduanaBaseComponent {

    camaraActualNombre = "";
    camaraUrlActual = "";
    camaraImagen = "";
    cambiandoCamara = false;

    @ViewChild(MensajeComponent)
    private mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    private spinnerComponent: SpinnerComponent;

    constructor(protected navService: NavService, protected service: AduanaService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, service, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR CAMARAS CONSOLIDACIO"); }

    setTabs() {
        this.setMenuSeccionTab("aduana", "Cámaras Consolidación");
    }

    ngOnInit() {
        super.ngOnInit();
        this.cambiarCamara("http://10.10.115.21/jpg/image.jpg", "BZA-CABEZAL");
    }

    cambiarCamara(url: string, nombre: string) {
        this.camaraActualNombre = nombre;
        this.camaraUrlActual = url;
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        this.cambiandoCamara = true;
        this.unsubscribe();
        this.subscription = this.service.getImagenCamaraConsolidacion(url, nombre).subscribe(
            result => {
                if (this.camaraActualNombre == result.nombre) {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.camaraImagen = "data:image/jpg;base64," + result.img;
                        this.cambiandoCamara = false;
                        this.obtenerImagen();
                    }
                }
            },
            error => {
                this.spinnerComponent.hideIt();
            }
        );
        return false;
    }

    obtenerImagen() {
        if (!this.cambiandoCamara) {
            this.unsubscribe();
            this.subscription = this.service.getImagenCamaraConsolidacion(this.camaraUrlActual, this.camaraActualNombre).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }else if (this.camaraActualNombre == result.nombre && result.img != undefined && result.img != "") {
                        this.camaraImagen = "data:image/jpg;base64," + result.img;
                    }
                    this.obtenerImagen();
                }
            );
        }
    }
}