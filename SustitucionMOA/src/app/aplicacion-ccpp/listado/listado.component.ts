import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ApiResponse } from '../../common/models/response';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { AplicacionCcppBaseComponent } from '../aplicacion-ccpp.base.component';
import { AplicacionCCPP, SeccionAplicacionCCPP, AplicacionCCPPFiltro, EstadoAplicacionCCPP } from '../aplicacion-ccpp.model';
import { AplicacionCcppService, ListadoRequest } from '../aplicacion-ccpp.service';
import { DropdownComponent, DropdownOption } from '../../common/view-child/dropdown/dropdown.component';

import * as XLSX from 'xlsx';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Permiso } from '../../common/enums/Permisos';

@Component({
    selector: 'app-listado',
    templateUrl: './listado.component.html',
    styleUrls: ['./listado.component.css']
})
export class ListadoComponent extends AplicacionCcppBaseComponent implements OnDestroy {
    @BlockUI() blockUI: NgBlockUI;
    @ViewChild("FiltroEstado") filtroEstadoComponent: DropdownComponent;
    aplicaciones?: AplicacionCCPP[];
    opcionesClientes?: DropdownOption[];
    opcionesEstadoAplicacionCCPP?: DropdownOption[];

    contratoFiltro = new FormControl();
    ccppFiltro = new FormControl();
    estadoFiltro = new FormControl();
    clienteFiltro = new FormControl();
    esAdmin = this.isAuthorized(Permiso.AdminAppCCPP);
    disabled = false;
    show = false;
    develop = true;
    seleccionaTodos: boolean;
    aplicacionesSeleccionadas: AplicacionCCPP[] = [];

    mostrarModalAprobarRechazar = false;
    mostrarModalAprobarRechazarMasivo = false;
    aprobarRechazarAplicacionId: number;
    opcionesAprobarRechazar: any[] = [{ label: 'Aprobar', value: 'aprobar' }, { label: 'Rechazar', value: 'rechazar' }];
    decisionAprobarRechazar = { label: '', value: '' };
    motivoRechazo: string = "";

    constructor(
        protected service: AplicacionCcppService,
        protected navService: NavService,
        protected sessionDataService: SessionDataService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }
    setTabs(): void {
        this.setMenuSeccionTab(SeccionAplicacionCCPP, 'Estado de cargas');
    }
    extraOnInit() {
        //this.getListado()
    }
    get isVisible(): boolean {
        return this.aplicaciones && !!this.aplicaciones.length && this.show
    }
    get verUsuario(): boolean {
        return true;
    }
    get verRazonSocial(): boolean {
        return true;
    }
    get filtraClientes(): boolean {
        return this.esAdmin;
    }
    getListadoFechas() {
        this.getListado();
        this.setMenuSeccionTab(SeccionAplicacionCCPP, "Estado de cargas");
    }
    getListado() {
        this.unsubscribe();
        this.blockUI.start('');
        this.mensajeComponent.setMsgsEmpty();
        this.limpiarListado();
        this.disabled = true;
        this.seleccionaTodos = false;
        this.subscription = this.service.getListado(this.getRequest()).subscribe(
            res => {
                let data = this.manejarErroresApiResponse(res);
                if (data) {
                    this.show = true
                    this.aplicaciones = data;
                    if (res.filtros) {
                        this.setOpciones(res.filtros);
                    }
                }
                this.blockUI.stop();
            },
            err => {
                console.error(err);
                this.blockUI.stop();
            }
        );
    }

    limpiarListado() {
        this.show = false
        this.aplicaciones = []
    }

    exportExcelAplicacionesCCPP() {
        this.mensajeComponent.setMsgsEmpty();
        let informacionExportar: any;

        informacionExportar = this.aplicaciones.map(info => {
            return {
                "Fecha": info.FechaAlta || "-",
                "Usuario": info.MailUsuario || "-",
                "Razon Social": info.RazonSocial || "-",
                "Contrato": info.Contrato,
                "Carta Porte": info.CartaPorte || "-",
                "KGS": info.Kilogramos || "-",
                "Estado": info.LabelEstado || "-",
                "Observaciones": info.Error,
            }
        });

        if (informacionExportar.length == 0) {
            this.mensajeComponent.setInfoMsg("No existen datos para exportar.");
            return
        }

        this.DownloadJsonData(informacionExportar, "Aplicación CCPP");
    }

    DownloadJsonData(JSONData: any, FileTitle: string) {
        //crea la estructura inicial del archivo
        let worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(JSONData);
        let workbook: XLSX.WorkBook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, FileTitle);
        //escribe el file para ser descargado
        XLSX.writeFile(workbook, FileTitle + '.xlsx');
    }

    getRequest(): ListadoRequest {
        return {
            fechaInicio: this.filtroFechaComponent.fecha_inicio,
            fechaFin: this.filtroFechaComponent.fecha_fin
        }
    }

    setOpciones({ FiltroEstados, FiltroClientes }: AplicacionCCPPFiltro) {
        this.opcionesEstadoAplicacionCCPP = FiltroEstados;
        this.opcionesClientes = FiltroClientes;
    }

    eliminarAplicacion(aplicacion: AplicacionCCPP) {
        this.blockUI.start("Eliminando ...");
        try {
            this.service.eliminarAplicacion(aplicacion.Id).subscribe({
                next: ({ info, error, logout }) => {
                    if (logout)
                        this.sessionDataService.logout()
                    else if (info || error)
                        this.mensajeComponent.setInfoMsg(info || error);
                    this.blockUI.stop();
                    this.getListado();
                },
                error: (err) => this.blockUI.stop()
                ,
            })
        } catch (error) {
            console.error(error)
            this.blockUI.stop()
        }
    }

    aprobarORechazarAplicacion(aplicacion: AplicacionCCPP) {
        this.aprobarRechazarAplicacionId = aplicacion.Id;
        this.mostrarModalAprobarRechazarMasivo = false;
        this.mostrarModalAprobarRechazar = true;
    }

    cancelarDecisionSobreAplicacionPendienteAprobacion() {
        this.mostrarModalAprobarRechazar = false;
        this.mostrarModalAprobarRechazarMasivo = false;
    }

    limpiarModalAprobarRechazar() {
        this.aprobarRechazarAplicacionId = -1;
        this.decisionAprobarRechazar = { label: '', value: '' };
        this.motivoRechazo = '';
    }

    grabarDecisionSobreAplicacionPendienteAprobacion() {
        let idsAplicaciones: number[] = [];
        if (this.mostrarModalAprobarRechazarMasivo) {
            idsAplicaciones = this.aplicacionesSeleccionadas.map(a => { return a.Id });
        }
        else if (this.mostrarModalAprobarRechazar) {
            idsAplicaciones = [this.aprobarRechazarAplicacionId];
        }
        this.mostrarModalAprobarRechazar = false;
        this.mostrarModalAprobarRechazarMasivo = false;

        if (this.decisionAprobarRechazar.value == 'aprobar') {
            this.aprobarAplicacionesPendientes(idsAplicaciones);
        }
        if (this.decisionAprobarRechazar.value == 'rechazar' && this.motivoRechazo.length > 2) {
            this.rechazarAplicacionesPendientes(idsAplicaciones);
        }
        this.limpiarModalAprobarRechazar();
    }

    aprobarAplicacionesPendientes(idsAplicaciones: number[]) {
        this.blockUI.start();
        try {
            this.subscription = this.service.aprobarAplicacionesPendientes(idsAplicaciones).subscribe(
                (resp) => {
                    let data = this.manejarErroresApiResponse(resp);
                    if (data) {
                        this.getListado();
                    }
                    this.blockUI.stop();
                },
                (err) => {
                    this.blockUI.stop();
                    console.error(err);
                }
            );
        } catch (error) {
            console.error(error);
            this.blockUI.stop();
        }
    }

    rechazarAplicacionesPendientes(idsAplicaciones: number[]) {
        this.blockUI.start();
        try {
            this.subscription = this.service.rechazarAplicacionesPendientes(idsAplicaciones, this.motivoRechazo).subscribe(
                (resp) => {
                    let data = this.manejarErroresApiResponse(resp);
                    if (data) {
                        this.getListado();
                    }
                    this.blockUI.stop();
                },
                (err) => {
                    this.blockUI.stop();
                    console.error(err);
                }
            );
        } catch (error) {
            console.error(error);
            this.blockUI.stop();
        }
    }

    puedeSeleccionarAplicacion(aplicacion: AplicacionCCPP): boolean {
        return aplicacion.Estado == EstadoAplicacionCCPP.PendienteAprobacion;
    }

    seleccionarTodos() {
        this.seleccionaTodos = !this.seleccionaTodos;
        if (this.aplicaciones && this.seleccionaTodos) {
            this.aplicaciones.forEach((aplicacion) => {
                if (this.puedeSeleccionarAplicacion(aplicacion) && !this.aplicacionEstaSeleccionada(aplicacion)) {
                    this.seleccionarAplicacion(aplicacion);
                }
            });
        }
        else if (!this.seleccionaTodos) {
            this.aplicacionesSeleccionadas = [];
        }
    }

    seleccionarAplicacion(aplicacion: AplicacionCCPP) {
        if (this.aplicacionEstaSeleccionada(aplicacion)) {
            this.aplicacionesSeleccionadas.splice(this.aplicacionesSeleccionadas.indexOf(aplicacion), 1);
            this.seleccionaTodos = false;
        }
        else {
            this.aplicacionesSeleccionadas.push(aplicacion);
            if (this.aplicaciones && this.aplicaciones.every(x => !this.puedeSeleccionarAplicacion(x) || this.aplicacionEstaSeleccionada(x))) {
                this.seleccionaTodos = true;
            }
        }
    }
    
    aplicacionEstaSeleccionada(aplicacion: AplicacionCCPP) {
        return this.aplicacionesSeleccionadas.includes(aplicacion);
    }

    resolverAprobacionMasiva() {
        this.mostrarModalAprobarRechazar = false;
        this.mostrarModalAprobarRechazarMasivo = true;
    }

    manejarErroresApiResponse<T>(response: ApiResponse<T>): T | null {
        this.mensajeComponent.setMsgsEmpty();
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
