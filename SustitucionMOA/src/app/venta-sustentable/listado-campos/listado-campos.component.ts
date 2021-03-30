import { Component, OnInit, ViewChild } from '@angular/core';
import { VentaSustentableService } from './../venta-sustentable.service'
import { BaseComponent } from './../../common/base-components/base-component';
import { NavService } from './../../common/services/NavService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { Seccion } from './../../common/models/seccion';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';

@Component({
    selector: 'app-listado-campos',
    templateUrl: './listado-campos.component.html',
    providers: [VentaSustentableService]
})
export class ListadoCamposComponent extends BaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: VentaSustentableService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    camposSustentables: any[];
    esInterno: boolean = this.isAuthorized('VER TODOS CAMPOS SUSTENTABLE');
    editarCampos: boolean = this.isAuthorized('EDICION CAMPOS CREADOS')

    ngOnInit() {
        this.navService.setSeccionList([new Seccion('/sustentable/alta', 'alta', 'Dar de Alta'), new Seccion('/sustentable/listado-campos', 'listado-campos', 'Listado Campos')]);
        this.getCamposSustentables();
    }

    getCamposSustentables() {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.getCamposProveedores().subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    this.camposSustentables = result;
                }
            },
            error => {
                this.floatMsgService.setErrorMsg(error.message);
            }
        );

        return false;
    }

    eliminarCampo(campoCosechaId: number, proveedorId: number) {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.campoProveedorBorrar(campoCosechaId, proveedorId).subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    this.mensajeComponent.setSuccessMsg("se elimino el campo " + campoCosechaId + " correctamente.");
                    setTimeout(() => {
                        this.getCamposSustentables();
                    }, 200);
                }
            },
            error => {
                this.floatMsgService.setErrorMsg(error.message);
            }
        );

        return false;
    }

}
