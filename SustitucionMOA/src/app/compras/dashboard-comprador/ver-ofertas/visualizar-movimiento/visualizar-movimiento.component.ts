import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { AdjudicacionDto } from '../../../../modelos/adjudicacion';
import { ComprasService } from '../../../compras.service';
import { NavService } from '../../../../common/services/NavService';
import { SecurityService } from '../../../../common/services/SecurityService';
import { FloatMsgService } from '../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../common/services/ModalService';
import { SessionDataService } from '../../../../common/services/SessionDataService';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { SpinnerComponent } from '../../../../common/view-child/spinner/spinner.component';
import { HistorialDeFechaDto } from '../../../../modelos/historialDeFechaDto';

@Component({
    selector: 'app-visualizar-movimiento',
    templateUrl: './visualizar-movimiento.component.html',
    styleUrls: ['./visualizar-movimiento.component.css']
})
export class VisualizarMovimientoComponent implements OnInit, OnChanges {

    @Input()
    displayVisualizarMovimientos: boolean;   
    @Input()
    peticionOferta_Id: number; 
    @BlockUI() blockUI: NgBlockUI;
    @Output() cerrarModalMovimientosEmitter = new EventEmitter();
    subscription: any;
    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;
    historialDeFechas: HistorialDeFechaDto;
    colspan: any;
    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    }   
    ngOnChanges(changes: SimpleChanges): void {
        this.listarHistorialDeFechas();
        if(this.historialDeFechas != undefined){
            this.colspan = this.historialDeFechas.Cuerpo[0].length;
        }
       
    }
    

    ngOnInit() {    
        this.listarHistorialDeFechas();
        if(this.historialDeFechas != undefined){
            this.colspan = this.historialDeFechas.Cuerpo[0].length;
        }
    }
    
    onCerrarMovimientos() {            
        this.cerrarModalMovimientosEmitter.next();      
    }        
   
    salir() {
        this.onCerrarMovimientos();       
    }

    listarHistorialDeFechas() {
        try {
            this.subscription = this.service.listarHistorialDeFechas(this.peticionOferta_Id).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {                       
                        this.historialDeFechas = result.data;
                        if( this.spinnerComponent != undefined){
                        this.spinnerComponent.hideIt();
                        }
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
        }
    }
    
// Verifica si una fila contiene "Revisión Técnica"
isRevisionTecnica(rowData: string[]): boolean {
    return rowData.some(cell => cell.includes('Revisión Técnica'));
}

// Verifica si una fila contiene "Cierre de Cotización"
isCierreCotizacion(rowData: string[]): boolean {
    return rowData.some(cell => cell.includes('Cierre de cotización'));
}

// Obtiene el dato de "Revisión Técnica" de la fila
getRevisionTecnica(rowData: string[]): string {
    const cell = rowData.find(cell => cell.includes('Revisión Técnica'));
    return cell || '';
}

// Obtiene el dato de "Cierre de Cotización" de la fila
getCierreCotizacion(rowData: string[]): string {
    const cell = rowData.find(cell => cell.includes('Cierre de cotización'));
    return cell || '';
}

// Devuelve la clase CSS para cada celda
getCellClass(cell: string): string {
    if (cell.includes('Circular Comprador')) {
        return 'circular';
    } else if (cell.includes('Orden de compra')) {
        return 'orden-compra';
    }
    return '';
}

// Devuelve la clase CSS para cada fila
getRowClass(rowData: string[]): string {
    if (this.isRevisionTecnica(rowData)) {
        return 'revision-tecnica';
    } else if (this.isCierreCotizacion(rowData)) {
        return 'cierre-cotizacion';
    }
    return '';
}

getRowColor(rowData: string[]): string {
    if (rowData.some(cell => cell.includes('Circular Comprador'))) {
        return '#FFCCCC'; // Color rojo claro para circular comprador
    }
    if (rowData.some(cell => cell.includes('Revisión Técnica'))) {
        return '#CCFFCC'; // Color verde claro para revisión técnica
    }
    if (rowData.some(cell => cell.includes('Cierre de cotización'))) {
        return '#CCCCFF'; // Color azul claro para cierre de cotización
    }
    if (rowData.some(cell => cell.includes('Orden de compra'))) {
        return '#FFFFCC'; // Color amarillo claro para orden de compra
    }
    return ''; // Sin color de fondo
}
    
}
