import { Component, OnInit, ViewChild } from '@angular/core';
import { CabeceraCM05, DetalleCM05, MovimientoCM05 } from './gestionCM05';
import { Table, TableModule } from 'primeng/table';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { GestionCM05Service } from './gestionCM05.service';
import { NavService } from '../common/services/NavService';
import { SessionDataService } from '../common/services/SessionDataService';
import { SecurityService } from '../common/services/SecurityService';
import { ActivatedRoute, Router } from '@angular/router';
import { ModalService } from '../common/services/ModalService';
import { FloatMsgService } from '../common/services/FloatMsgService';
import { ListBaseComponent } from '../common/base-components/list-base-component';
import { SelectItem } from 'primeng/api';
import { MessageService } from 'primeng/api';
import { CommonResponse } from '../common/models/common-response';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { MenuItem } from 'primeng/api';

declare var $: any;

@Component({
    templateUrl: './gestionCM05.component.html',
    styleUrls: ['./gestionCM05.component.css'],
    providers: [{ provide: GestionCM05Service, useClass: GestionCM05Service }, MessageService]
})
export class GestionCM05Component extends ListBaseComponent {
    @BlockUI() blockUI: NgBlockUI;

    @ViewChild("dt")
    protected table: Table;

    @ViewChild('fileUploadCargaCM05')
    protected fileUploadCargaCM05: any;

    displayDialogDetallesCM05: boolean;

    cabeceraCols: any[];
    cabeceras: CabeceraCM05[];

    selectedCabecera: CabeceraCM05;

    detalleCols: any[];
    detalles: DetalleCM05[];
    frozenCols: any[];

    movimientoCols: any[];
    movimientos: MovimientoCM05[];

    estados: SelectItem[];
    secuencias: SelectItem[];
    tiposMovimiento: SelectItem[];
    origenesMovimiento: SelectItem[];

    detallesEditando: DetalleCM05[];

    cuitFiltro: string;
    idConsultaFiltro: string;
    idFiltro: string;

    fechaDesde: Date;
    fechaHasta: Date;

    editandoCabecera: boolean;
    cabeceraEditando: CabeceraCM05;
    files: FileList = null;
    listaArchivos: Array<File> = new Array<File>();

    tabsPopup: MenuItem[];
    tabPopupActiva: MenuItem;

    displayDialogCargaCM05: boolean = false;
    uploadedFiles: any[] = [];

    constructor(protected service: GestionCM05Service,
        protected navService: NavService,
        protected sessionDataService: SessionDataService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService,
        protected route: ActivatedRoute,
        protected router: Router,
        private messageService: MessageService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    ngOnInit() {
        this.detallesEditando = [];

        this.navService.setSeccionList([]);

        this.estados = [
            { label: 'Pendiente', value: 1, },
            { label: 'Autorizado', value: 2, },
            { label: 'Completado', value: 3, },
            { label: 'Rechazado por usuario', value: 4, },
            { label: 'Rechazado por sistema', value: 5, },
        ];

        this.secuencias = [
            { label: 'Original', value: 1, },
            { label: 'Rectificativa', value: 2, },
        ];

        this.tiposMovimiento = [
            { label: 'Creación', value: 1 },
            { label: 'Autorización', value: 2 },
            { label: 'Autorización revertida', value: 3 },
            { label: 'Exportación exitosa', value: 4 },
            { label: 'Error', value: 5 }
        ];

        this.origenesMovimiento = [
            { label: 'WEB', value: 1 },
            { label: 'SAP', value: 2 },
        ];

        this.listarCabeceras();

        this.cabeceraCols = [
            { field: 'Id', header: 'Id' },
            { field: 'IdConsulta', header: 'Id consulta' },
            { field: 'RazonSocial', header: 'Razón Social' },
            { field: 'Estado', header: 'Estado' },
            { field: 'CUIT', header: 'CUIT' },
            { field: 'Anticipo', header: 'Anticipo' },
            { field: 'Sede', header: 'Sede' },
            { field: 'FechaCarga', header: 'Fecha carga' },
            { field: 'FechaUltimaModificacion', header: 'Última modificación' },
            { field: 'SecuienciaId', header: 'SecuenciaId' },
            { field: 'MalCargada', header: 'MalCargada' },
        ];

        this.detalleCols = [
            { field: 'Jurisdiccion', header: 'Jurisdicción', },
            { field: 'FechaInicio', header: 'Fecha inicio', },
            { field: 'FechaCese', header: 'Fecha cese', },
            //{ field: 'CoeficienteIngresos', header: 'Coef. ingresos', },
            //{ field: 'CoeficienteGastos', header: 'Coef. gastos', },
            { field: 'CoeficienteUnificado', header: 'Coef. unificado', },
            { field: 'FechaUltimaModificacion', header: 'Última modificación', },
        ];
        this.frozenCols = this.detalleCols;

        this.movimientoCols = [
            { field: 'Fecha', header: 'Fecha', },
            { field: 'Observaciones', header: 'Observaciones', },
            { field: 'Tipo', header: 'Tipo', },
            { field: 'Origen', header: 'Origen', },
            { field: 'EstadoAnterior', header: 'Estado Anterior', },
            { field: 'EstadoPosterior', header: 'Estado Posterior', },
        ];

        this.editandoCabecera = false;
        this.cabeceraEditando = null;

        $(".adjuntarArchivo").click(function () {
            $(".adjuntarArchivo1").click();
        });

        this.tabsPopup = [
            {
                label: 'Detalles',
                icon: 'fa fa-fw fa-info',
                command: (event) => { this.tabPopupActiva = this.tabsPopup[0] }
            },
            {
                label: 'Movimientos',
                icon: 'fa fa-fw fa-history',
                command: (event) => { this.tabPopupActiva = this.tabsPopup[1] }
            },
        ];

    }

    onCabeceraClick(data) {
        this.selectedCabecera = {
            Id: data.Id,
            Estado: data.Estado,
            EstadoId: data.EstadoId,
            Anticipo: data.Anticipo,
            CUIT: data.CUIT,
            Sede: data.Sede,
            FechaCarga: data.FechaCarga,
            FechaUltimaModificacion: data.FechaUltimaModificacion,
            MalCargada: data.MalCargada,
            Secuencia: data.Secuencia,
            SecuenciaId: data.SecuenciaId,
            ConsultaId: data.ConsultaId,
            RazonSocial: data.RazonSocial,
        };

        this.service.listarDetalles(this.selectedCabecera.Id).subscribe(result => {
            this.detalles = result;
            this.detalles.forEach(x => {
                x.FechaCese = x.FechaCese == undefined ? null : new Date(this.getDateFromAspNetFormat(x.FechaCese));
                x.FechaInicio = x.FechaInicio == undefined ? null : new Date(this.getDateFromAspNetFormat(x.FechaInicio));
                x.FechaUltimaModificacion = new Date(this.getDateFromAspNetFormat(x.FechaUltimaModificacion));
            });
        });

        this.service.listarMovimientos(this.selectedCabecera.Id).subscribe(result => {
            this.movimientos = result;
            this.movimientos.forEach(x => {
                x.Fecha = x.Fecha == undefined ? null : new Date(this.getDateFromAspNetFormat(x.Fecha));
                x.Origen = this.origenesMovimiento.find(origen => origen.value == x.OrigenId).label;
                x.Tipo = this.tiposMovimiento.find(tipo => tipo.value == x.TipoId).label;
                x.EstadoAnterior = this.estados.find(estado => estado.value == x.EstadoAnteriorId).label;
                x.EstadoPosterior = this.estados.find(estado => estado.value == x.EstadoPosteriorId).label;
            });
        });

        this.tabPopupActiva = this.tabsPopup[0];

        setTimeout(() => {
            this.displayDialogDetallesCM05 = true;
        }, 700);
    }

    closeDialogDetalles() {
        this.selectedCabecera = null;
        this.detalles = null;
        this.editandoCabecera = false;
        this.cabeceraEditando = null;
        this.displayDialogDetallesCM05 = false;
    }

    editarRow(rowData) {
        rowData.Editar = true;
        this.detallesEditando.push({ ...rowData });
    }

    guardarRow(rowData) {
        this.messageService.clear();
        try {
            if (this.validarRow(rowData)) {
                return
            }

            this.subscription = this.service.editarRow(rowData).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.messageService.add({ severity: 'error', summary: 'No se pudo editar', detail: result.error });
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.messageService.add({ severity: 'info', summary: 'No se pudo editar', detail: result.info });
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.messageService.add({ severity: 'success', summary: 'Detalle actualizado', detail: result.Mensaje });
                        this.floatMsgService.setSuccessMsg(result.Mensaje);

                        rowData.FechaUltimaModificacion = new Date(this.getDateFromAspNetFormat(result.FechaUltimaModificacion));
                        rowData.Editar = false;

                        this.eliminarDetalleDe(rowData, this.detallesEditando);
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    cancelarGuardarRow(rowData) {
        var backupDetalle = this.detallesEditando.filter(x => x.Id == rowData.Id)[0];

        rowData.Id = backupDetalle.Id;
        rowData.Jurisdiccion = backupDetalle.Jurisdiccion;
        rowData.NumeroJurisdiccion = backupDetalle.NumeroJurisdiccion;
        rowData.FechaInicio = backupDetalle.FechaInicio;
        rowData.FechaCese = backupDetalle.FechaCese;
        rowData.CoeficienteUnificado = backupDetalle.CoeficienteUnificado;
        rowData.FechaUltimaModificacion = backupDetalle.FechaUltimaModificacion;

        rowData.Editar = false;
        this.eliminarDetalleDe(rowData, this.detallesEditando);
    }

    validarRow(rowData) {
        this.messageService.clear();
        var regexNumerosEnteros = /^[0-9]*$/
        var regexNumerosDecimales = /^[0-9,.]*$/

        if (rowData.NumeroJurisdiccion == null || rowData.NumeroJurisdiccion == "") {
            this.messageService.add({ severity: 'error', summary: 'Nro. Jurisdicción', detail: 'Esta vacio.' });
            return
        }
        if (!(regexNumerosEnteros.test(rowData.NumeroJurisdiccion))) {
            this.messageService.add({ severity: 'error', summary: 'Nro. Jurisdicción', detail: 'Debe ser un numero entero.' });
            return
        }

        /*if(!(regexNumerosDecimales.test(rowData.CoeficienteIngresos))){
            this.messageService.add({severity:'error', summary:'Coef. Ingresos', detail:'Debe ser un numero entero o decimal.'});
            return
        }
        if(rowData.CoeficienteIngresos == null || rowData.CoeficienteIngresos == ""){
            this.messageService.add({severity:'error', summary:'Coef. Ingresos', detail:'Esta vacio.'});
            return
        }

        if(!(regexNumerosDecimales.test(rowData.CoeficienteGastos))){
            this.messageService.add({severity:'error', summary:'Coef. Gastos', detail:'Debe ser un numero entero o decimal.'});
            return
        }
        if(rowData.CoeficienteGastos == null || rowData.CoeficienteGastos == ""){
            this.messageService.add({severity:'error', summary:'Coef. Gastos', detail:'Esta vacio.'});
            return
        }*/

        if (!(regexNumerosDecimales.test(rowData.CoeficienteUnificado))) {
            this.messageService.add({ severity: 'error', summary: 'Coef. Unificado', detail: 'Debe ser un numero entero o decimal.' });
            return
        }
        /*
        if(rowData.CoeficienteUnificado == null || rowData.CoeficienteUnificado == ""){
            this.messageService.add({severity:'error', summary:'Coef. Unificado', detail:'Esta vacio.'});
            return
        }*/

        return false
    }

    autorizarCabecera() {
        try {
            this.service.autorizarCabecera(this.selectedCabecera.Id).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.selectedCabecera.Estado = 'Autorizado';
                        this.listarCabeceras();
                        this.floatMsgService.setSuccessMsg("Registro autorizado correctamente");
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false;
    }

    descargarFormularioCM05() {
        this.service.DescargarArchivoFormularioCM05(this.selectedCabecera.Id).subscribe(
            (result) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                }
                else {
                    var byteArray = new Uint8Array(result.FileContents);
                    var blob = new Blob([byteArray], {
                        type: "application/octet-stream",
                    });

                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(
                            blob,
                            result.FileDownloadName
                        );
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = result.FileDownloadName;
                        link.click();
                        setTimeout(function () {
                            window.URL.revokeObjectURL(url);
                        }, 0);
                        return false;
                    }
                }
            },
            (error) => {
                //this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        )
    }

    eliminarDetalleDe(elemento: DetalleCM05, array: DetalleCM05[]) {
        var indiceDelElemento = array.findIndex(x => x.Id == elemento.Id);
        if (indiceDelElemento > -1) {
            array.splice(indiceDelElemento, 1);
        }
    }

    listarCabeceras() {
        this.unsubscribe();
        try {
            this.subscription = this.service.listarCabeceras().subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.cabeceras = result;
                        this.cabeceras.forEach(x => {
                            x.Estado = this.estados.find(e => e.value == x.EstadoId).label;
                            x.Secuencia = this.secuencias.find(s => s.value == x.SecuenciaId).label;
                            x.FechaCarga = x.FechaCarga == undefined ? null : new Date(this.getDateFromAspNetFormat(x.FechaCarga));
                            x.FechaUltimaModificacion = new Date(this.getDateFromAspNetFormat(x.FechaUltimaModificacion));
                        });
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    editarCabecera() {
        this.editandoCabecera = true;
        this.cabeceraEditando = { ...this.selectedCabecera };
    }

    guardarCabeceraEditada() {
        this.messageService.clear();
        try {
            if (!this.validarCabeceraEditada()) {
                this.subscription = this.service.editarCabecera(this.selectedCabecera).subscribe(
                    (result: any) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.messageService.add({ key: 'toastPopupDetalles', severity: 'error', summary: 'No se pudo editar', detail: result.error });
                        } else if (result.info != undefined) {
                            this.messageService.add({ key: 'toastPopupDetalles', severity: 'info', summary: 'No se pudo editar', detail: result.info });
                        } else {
                            this.messageService.add({ key:'toastPopupDetalles', severity: 'success', summary: 'Cabecera actualizada', detail: result.Mensaje });

                            this.selectedCabecera.Secuencia = this.selectedCabecera.SecuenciaId ? this.secuencias.find(s => s.value == this.selectedCabecera.SecuenciaId).label : '';
                            this.selectedCabecera.FechaUltimaModificacion = new Date(this.getDateFromAspNetFormat(result.FechaUltimaModificacion));

                            this.editandoCabecera = false;
                            this.cabeceraEditando = null;

                            this.listarCabeceras();
                        }
                    },
                    error => {
                        this.messageService.add({ key: 'toastPopupDetalles', severity: 'error', summary: 'No se pudo editar', detail: error.message });
                    }
                );
            }
        } catch (e) {
            this.messageService.add({ key: 'toastPopupDetalles', severity: 'error', detail: e });
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    validarCabeceraEditada() {
        this.messageService.clear();
        var regexNumerosEnteros = /^[0-9]*$/

        if (this.selectedCabecera.Anticipo == null || this.selectedCabecera.Anticipo == "") {
            this.messageService.add({ severity: 'error', summary: 'Anticipo', detail: 'Esta vacio.' });
            return true;
        }

        if (!(regexNumerosEnteros.test(this.selectedCabecera.Anticipo))) {
            this.messageService.add({ severity: 'error', summary: 'Anticipo', detail: 'Debe ser un número entero.' });
            return true;
        }

        if (this.selectedCabecera.Sede == null || this.selectedCabecera.Sede == "") {
            this.messageService.add({ severity: 'error', summary: 'Sede', detail: 'Esta vacio.' });
            return true;
        }

        if (!(regexNumerosEnteros.test(this.selectedCabecera.Sede))) {
            this.messageService.add({ severity: 'error', summary: 'Sede', detail: 'Debe ser un número entero.' });
            return true;
        }

        if (!(regexNumerosEnteros.test(this.selectedCabecera.CUIT))) {
            this.messageService.add({ severity: 'error', summary: 'Cuit', detail: 'Debe ser un número.' });
            return true;
        }

        return false;
    }

    cancelarEditarCabecera() {
        var backupCabecera = this.cabeceraEditando;

        this.selectedCabecera.CUIT = backupCabecera.CUIT;
        this.selectedCabecera.Anticipo = backupCabecera.Anticipo;
        this.selectedCabecera.Sede = backupCabecera.Sede;
        this.selectedCabecera.SecuenciaId = backupCabecera.SecuenciaId;
        this.selectedCabecera.RazonSocial = backupCabecera.RazonSocial;

        this.editandoCabecera = false;
        this.cabeceraEditando = null;
    }

    mostrarDialogCargaCM05() {
        this.displayDialogCargaCM05 = true;
    }

    closeDialogCargaCM05() {
        this.displayDialogCargaCM05 = false;
        this.fileUploadCargaCM05.clear();
        this.blockUI.stop();
    }

    cargarCM05(event: any) {
        this.blockUI.start()
        try {
            this.service.cargarCM05(event.files).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout()
                        this.blockUI.stop();
                    } else if (result.error != undefined && result.error != "") {
                        this.messageService.add({ key: 'toastPopupCargaCM05', severity: 'error', detail: result.error });
                        this.blockUI.stop();
                    } else if (result.info != undefined) {
                        this.messageService.add({ key: 'toastPopupCargaCM05', severity: 'info', detail: result.info });
                        this.blockUI.stop()
                        return false;
                    } else {
                        this.messageService.add({ key: 'toastPopupCargaCM05', severity: 'success', detail: result.Mensaje });
                        this.listarCabeceras()
                        this.closeDialogCargaCM05()
                        this.blockUI.stop();
                        return false;
                    }
                },
                error => {
                    this.messageService.add({ key: 'toastPopupCargaCM05', severity: 'error', detail: error.message });

                    this.closeDialogCargaCM05()
                    this.blockUI.stop();
                    return false;
                }
            );
        } catch (e) {
            this.messageService.add({ key: 'toastPopupCargaCM05', severity: 'error', detail: e });
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
    }


}