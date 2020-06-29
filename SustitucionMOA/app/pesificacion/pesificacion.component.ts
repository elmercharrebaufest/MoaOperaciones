import { Component, OnInit, ViewChild } from '@angular/core';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { PesificacionService } from './pesificacion.service';
import { Router, ActivatedRoute } from '@angular/router';
import { NavService } from './../common/services/NavService';
import { SecurityService } from './../common/services/SecurityService';
import { SessionDataService } from './../common/services/SessionDataService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
declare var $: any;


@Component({
    selector: 'app-pesificacion',
    templateUrl: './app/pesificacion/pesificacion.component.html',
    providers: [PesificacionService]
})

export class PesificacionComponent extends ListBaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild(SpinnerSmallComponent)
    protected SpinnerSmallComponent: SpinnerSmallComponent;

    constructor(protected service: PesificacionService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    fecha: any = null;
    contrato: string = "";
    fijacion: string = "";
    cantidad: number = 0;
    file: any = null;

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.initForm();
    }

    setTabs() {
        this.navService.setSeccionList([]);
        this.setMenuSeccionTab("pesificacion", "");
    }

    checkPermisos() {
        this.securityService.tienePermisoRedirect("PESIFICACION");
    }

    initForm() {
        this.fecha = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getData().subscribe(
            result => {
                this.fecha = null;
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.fecha = result;
                    this.contrato = "";
                    this.fijacion = "";
                    this.cantidad = 0;
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
                return false;
            }

        );
        return false;
    }

    isVisible(): boolean {
        if (this.fecha && this.fecha != null)
            return true;
        else
            return false;
    }


    guardarPesificaciones() {
        this.spinnerSmallComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();

        if (this.contrato == null || this.contrato == "") {
            this.spinnerSmallComponent.hideIt();
            this.mensajeComponent.setErrorMsg("Debe ingresar un contrato");
            return false;
        }

        if (this.cantidad == null || this.cantidad <= 0) {
            this.spinnerSmallComponent.hideIt();
            this.mensajeComponent.setErrorMsg("Debe ingresar una cantidad mayor a 0");
            return false;
        }

        this.unsubscribe();
        this.subscription = this.service.setData(this.contrato, this.fijacion, this.cantidad).subscribe(
            result => {
                this.spinnerSmallComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.mensajeComponent.setSuccessMsg("Operacion realizada exitosamente");
                }
            },
            error => {
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
                return false;
            }
        );
        return false;
    }

    cargarArchivo(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            this.file = fileList[0];
        }
    }

    cargaMasiva() {
        this.spinnerSmallComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();

        if (this.file == null || !this.esCSV(this.file.name)) {
            this.spinnerSmallComponent.hideIt();
            this.mensajeComponent.setErrorMsg("Debe seleccionar un archivo .csv valido");
            return false;
        }

        this.unsubscribe();
        this.subscription = this.service.setMassiveData(this.file).subscribe(
            result => {
                this.spinnerSmallComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.mensajeComponent.setSuccessMsg("Operacion realizada exitosamente");
                }
            },
            error => {
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
                return false;
            }
        );
        return false;
    }

    esCSV(name: String): boolean {
        let ext = name.substring(name.lastIndexOf('.') + 1);
        if (ext.toLowerCase() == 'csv')
            return true;
        else
            return false;
    }
}