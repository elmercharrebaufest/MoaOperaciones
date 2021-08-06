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

    displayDialog: boolean;

    cabeceraCols: any[];
    cabeceras: CabeceraCM05[];

    selectedCabecera: CabeceraCM05;

    detalleCols: any[];
    detalles: DetalleCM05[];

    estados: SelectItem[];

    detallesEditando: DetalleCM05[];

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

        this.service.listarCabeceras().subscribe(result => {
            this.cabeceras = result;
            this.cabeceras.forEach(x => {
                x.Estado = x.EstadoId == 1 ? 'Pendiente' : x.EstadoId == 2 ? 'Autorizado' : x.EstadoId == 3 ? 'Completado' : '';
                x.FechaCarga = x.FechaCarga == undefined ? null : new Date(this.getDateFromAspNetFormat(x.FechaCarga));
                x.FechaUltimaModificacion = new Date(this.getDateFromAspNetFormat(x.FechaUltimaModificacion));
            });
        });

        this.cabeceraCols = [
            { field: 'Id', header: 'Id' },
            { field: 'Estado', header: 'Estado' },
            { field: 'Estado_Id', header: 'Estado_Id' },
            { field: 'CUIT', header: 'CUIT' },
            { field: 'Anticipo', header: 'Anticipo' },
            { field: 'Sede', header: 'Sede' },
            { field: 'FechaCarga', header: 'Fecha carga' },
            { field: 'FechaUltimaModificacion', header: 'Última modificación' },
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

                        this.eliminarDe(rowData, this.detallesEditando);
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
        this.eliminarDe(rowData, this.detallesEditando);
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
                        this.cabeceras.find(c => c.Id == this.selectedCabecera).Estado = 'Autorizado';
                        this.floatMsgService.setErrorMsg("Registro autorizado correctamente");
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

    eliminarDe(elemento: DetalleCM05, array: DetalleCM05[]) {
        var indiceDelElemento = array.findIndex(x => x.Id == elemento.Id);
        if (indiceDelElemento > -1) {
            array.splice(indiceDelElemento, 1);
        }
    }
}