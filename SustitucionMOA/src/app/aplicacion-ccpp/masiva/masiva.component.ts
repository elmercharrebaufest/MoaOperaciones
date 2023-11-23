import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { AplicacionCcppBaseComponent } from '../aplicacion-ccpp.base.component';
import { SeccionAplicacionCCPP, AplicacionGuardadaCargaMasivaCCPP, ErrorValidacionCargaMasivaCCPP } from '../aplicacion-ccpp.model';
import { AplicacionCcppService } from '../aplicacion-ccpp.service';
import { ApiResponse } from '../../common/models/response';

@Component({
    selector: 'app-masiva',
    templateUrl: './masiva.component.html',
    styleUrls: ['./masiva.component.css']
})
export class MasivaComponent extends AplicacionCcppBaseComponent implements OnInit, OnDestroy {
    @BlockUI() blockUI: NgBlockUI;

    archivo: File | null = null;
    archivoFueProcesado: boolean = false;
    hayErroresValidacion: boolean = false;
    erroresValidacionArchivo: ErrorValidacionCargaMasivaCCPP[];
    aplicacionesGuardadas: AplicacionGuardadaCargaMasivaCCPP[];
    
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
    
    ngOnInit() {
        this.setMenuSeccionTab(SeccionAplicacionCCPP, 'Carga masiva');
    }

    cargarArchivo(event: any) {
        let archivos: FileList = event.target.files;
        if (archivos.length > 0) {
            this.archivo = archivos[0];
        }
    }

    cargarMasiva() {
        this.blockUI.start();
        this.mensajeComponent.setMsgsEmpty();
        this.floatMsgService.setMsgsEmpty();

    //     if (this.file == null || !this.esCSV(this.file.name)) {
    //         this.spinnerSmallComponent.hideIt();
    //         this.visibleEnviar = true;
    //         this.mensajeComponent.setErrorMsg("Debe seleccionar un archivo .csv valido");
    //         return false;
    //     }
        if (this.archivo == null ) {
            this.mensajeComponent.setErrorMsg("Debe seleccionar un archivo .csv válido");
            this.blockUI.stop();
            return;
        }
        
        this.unsubscribe();
        this.subscription = this.service.enviarCargaMasiva(this.archivo).subscribe(
            (apiResponse) => {
                let res = this.manejarErroresApiResponse(apiResponse);
                if (res) {
                    this.archivoFueProcesado = true;
                    this.hayErroresValidacion = res.HayErroresValidacion;
                    this.erroresValidacionArchivo = res.ErroresValidacion;
                    this.aplicacionesGuardadas = res.AplicacionesGuardadas;
                    if (res.HayErroresValidacion) {
                        this.floatMsgService.setErrorMsg("No se pudo procesar. Por favor, corrija en el archivo los errores listados y vuelva a cargarlo.");
                    }
                    else {
                        this.floatMsgService.setSuccessMsg("Archivo procesado correctamente");
                    }
                }
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
            },
            () => {
                this.blockUI.stop();
            }
        );
    }

    resetearCarga() {
        this.archivo = null;
        this.archivoFueProcesado = false;
        this.hayErroresValidacion = false;
        this.erroresValidacionArchivo = [];
        this.aplicacionesGuardadas = [];
        this.mensajeComponent.setMsgsEmpty();
        this.floatMsgService.setMsgsEmpty();
    }
    
    manejarErroresApiResponse<T>(response: ApiResponse<T>): T | null {
        if (response.logout) {
            this.sessionDataService.logout();
            return null;
        }
        if (response.error) {
            this.mensajeComponent.setErrorMsg(response.error);
            return null;
        }
        if (response.info) {
            this.mensajeComponent.setInfoMsg(response.info);
        }
        return response.data || null;
    }
}