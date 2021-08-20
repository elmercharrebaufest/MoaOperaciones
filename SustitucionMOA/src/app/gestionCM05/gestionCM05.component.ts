import { Component, OnInit, ViewChild } from '@angular/core';
import { CabeceraCM05, DetalleCM05 } from './gestionCM05';
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

@Component({
    templateUrl: './gestionCM05.component.html',
    styleUrls: ['./gestionCM05.component.css'],
    providers: [{ provide: GestionCM05Service, useClass: GestionCM05Service }, MessageService]
})
export class GestionCM05Component extends ListBaseComponent {

    @ViewChild("dt")
    protected table: Table;

    displayDialog: boolean;

    cabeceraCols: any[];
    cabeceras: CabeceraCM05[];

    selectedCabecera: CabeceraCM05;

    detalleCols: any[];
    detalles: DetalleCM05[];

    estados: SelectItem[];

    detallesEditando: DetalleCM05[];

    cuitFiltro: string;

    fechaDesde: Date;
    fechaHasta: Date;

    editandoCabecera: boolean;
    cabeceraEditando: CabeceraCM05;

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
            { label: 'Pendiente',  value: 'Pendiente',   },
            { label: 'Autorizado', value: 'Autorizado', },
            { label: 'Completado', value: 'Completado', },
        ];

        this.listarCabeceras();

        this.cabeceraCols = [
            { field: 'Id', header: 'Id' },
            { field: 'Estado', header: 'Estado' },
            { field: 'Estado_Id', header: 'Estado_Id' },
            { field: 'CUIT', header: 'CUIT' },
            { field: 'Anticipo', header: 'Anticipo' },
            { field: 'Sede', header: 'Sede' },
            { field: 'FechaCarga', header: 'Fecha carga' },
            { field: 'FechaUltimaModificacion', header: 'Última modificación' },
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

        this.editandoCabecera = false;
        this.cabeceraEditando = null;
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
            MalCargada: data.MalCargada
        };
        this.service.listarDetalles(this.selectedCabecera.Id).subscribe(result => {
            this.detalles = result;
            this.detalles.forEach(x => {
                x.FechaCese = x.FechaCese == undefined ? null : new Date(this.getDateFromAspNetFormat(x.FechaCese));
                x.FechaInicio = x.FechaInicio == undefined ? null : new Date(this.getDateFromAspNetFormat(x.FechaInicio));
                x.FechaUltimaModificacion = new Date(this.getDateFromAspNetFormat(x.FechaUltimaModificacion));
            });
        });
        setTimeout(() => {
            this.displayDialog = true;
        }, 600);
    }
     
    close() {
        this.selectedCabecera = null;
        this.detalles = null;
        this.displayDialog = false;
    }

    editarRow(rowData) {
        rowData.Editar = true;
        this.detallesEditando.push({ ...rowData });
    }

    guardarRow(rowData){
        this.messageService.clear();
        try {
            if(this.validarRow(rowData)){
                return
            }

            this.subscription = this.service.editarRow(rowData).subscribe(
                result => {
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

    validarRow(rowData){
        this.messageService.clear();
        var regexNumerosEnteros = /^[0-9]*$/
        var regexNumerosDecimales = /^[0-9,.]*$/

        if(rowData.NumeroJurisdiccion == null || rowData.NumeroJurisdiccion == ""){
            this.messageService.add({severity:'error', summary:'Nro. Jurisdicción', detail:'Esta vacio.'});
            return
        }
        if(!(regexNumerosEnteros.test(rowData.NumeroJurisdiccion))){
            this.messageService.add({severity:'error', summary:'Nro. Jurisdicción', detail:'Debe ser un numero entero.'});
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

        if(!(regexNumerosDecimales.test(rowData.CoeficienteUnificado))){
            this.messageService.add({severity:'error', summary:'Coef. Unificado', detail:'Debe ser un numero entero o decimal.'});
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
                            x.Estado = x.EstadoId == 1 ? 'Pendiente' : x.EstadoId == 2 ? 'Autorizado' : x.EstadoId == 3 ? 'Completado' : '';
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
            if (this.validarCabeceraEditada()) {
                return
            }

            this.subscription = this.service.editarCabecera(this.selectedCabecera).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.messageService.add({ severity: 'error', summary: 'No se pudo editar', detail: result.error });
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.messageService.add({ severity: 'info', summary: 'No se pudo editar', detail: result.info });
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.messageService.add({ severity: 'success', summary: 'Cabecera actualizada', detail: result.Mensaje });
                        this.floatMsgService.setSuccessMsg(result.Mensaje);

                        this.selectedCabecera.FechaUltimaModificacion = new Date(this.getDateFromAspNetFormat(result.FechaUltimaModificacion));
                        this.editandoCabecera = false;
                        this.cabeceraEditando = null;

                        this.listarCabeceras();
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

    validarCabeceraEditada() {
        this.messageService.clear();
        var regexNumerosEnteros = /^[0-9]*$/

        if (this.selectedCabecera.Anticipo == null || this.selectedCabecera.Anticipo == "") {
            this.messageService.add({ severity: 'error', summary: 'Anticipo', detail: 'Esta vacio.' });
            return
        }

        if (!(regexNumerosEnteros.test(this.selectedCabecera.Anticipo))) {
            this.messageService.add({ severity: 'error', summary: 'Anticipo', detail: 'Debe ser un número entero.' });
            return
        }

        if (this.selectedCabecera.Sede == null || this.selectedCabecera.Sede == "") {
            this.messageService.add({ severity: 'error', summary: 'Sede', detail: 'Esta vacio.' });
            return
        }

        if (!(regexNumerosEnteros.test(this.selectedCabecera.Sede))) {
            this.messageService.add({ severity: 'error', summary: 'Sede', detail: 'Debe ser un número entero.' });
            return
        }

        if (!(regexNumerosEnteros.test(this.selectedCabecera.CUIT))) {
            this.messageService.add({ severity: 'error', summary: 'Cuit', detail: 'Debe ser un número.' });
            return
        }

        return false;
    }

    cancelarEditarCabecera() {
        var backupCabecera = this.cabeceraEditando;

        this.selectedCabecera.CUIT = backupCabecera.CUIT;
        this.selectedCabecera.Anticipo = backupCabecera.Anticipo;
        this.selectedCabecera.Sede = backupCabecera.Sede;

        this.editandoCabecera = false;
        this.cabeceraEditando = null;
    }
}