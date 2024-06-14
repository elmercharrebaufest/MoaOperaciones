import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { certificacionES } from './modalAprobacion.interface';
type Column = {
    name: string;
    visible: boolean;
};

@Component({
    selector: 'modal-aprobacion',
    templateUrl: 'modal-aprobacion.component.html',
    styleUrls: ['modal-aprobacion.component.css']
})

export class ModalAprobacionComponent implements OnInit {
    @Input() showModal: boolean = false;
    @Input() entradaServicioSeleccionada: certificacionES[] = [];
    @Output() aprobarES = new EventEmitter<string>();
    @Output() closeModal = new EventEmitter<void>();
    colConfigName: string = 'columnasAprobaciones';
    fechaDocumento: Date = new Date();
    fechaDocMin: Date = new Date();
    fechaDocMax: Date = new Date();
    fechaContabilizacion: Date = new Date();
    fechaContabilizacionMin: Date = new Date();
    fechaContabilizacionMax: Date = new Date();
    referencia: string = '';
    es: any = {
        firstDayOfWeek: 0,
        dayNames: ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"],
        dayNamesShort: ["Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb"],
        dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sá"],
        monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
        monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
        today: 'Hoy',
        clear: 'Limpiar',
        dateFormat: 'yyyy-mm-dd',
        weekHeader: 'Sem'
    };
    monthNavStatus: boolean = false;
    totalMontoCertificar: number = 0;

    constructor() { }

    ngOnDestroy(): void {
    }

    ngOnInit() {
        this.setRangoFechaDocumento();
        this.setRangoFechaContabilizacion();
        this.calcularTotalMontoCertificar();
        this.entradaServicioSeleccionada = this.orderBy(this.entradaServicioSeleccionada, 'NroPosicion');
    }

    /**
     * Ordena el array por el Nro de posicion.
     * @param array 
     * @param field 
     * @returns 
     */
    orderBy(array: any[], field: string): any[] {
        return array.sort((a, b) => a[field] - b[field]);
    }

    /**
     * Calcula el total del monto a certificar
     */
    calcularTotalMontoCertificar() {
        let total = 0;
        this.entradaServicioSeleccionada.forEach((es: any) => {
            es.Items.forEach( (item: any) => {
                total += item.MontoACertificar;
            });
        });

        this.totalMontoCertificar = total;
    }

    /**
     * Setear rango de fecha documento.
     */
    setRangoFechaDocumento() {
        // Fecha mínima: Fecha Actual 5 años hacia atrás
        this.fechaDocMin = new Date(new Date().setFullYear(new Date().getFullYear() - 5));
    }

    /**
     * Setear rango de fecha contabilizada.
     */
    setRangoFechaContabilizacion() {
        // Fecha máxima: Fecha actual
        this.fechaContabilizacionMax = new Date();
        // Fecha mínima: Primero del mes corriente
        if (sessionStorage.permisos !== undefined && sessionStorage.permisos.includes('ADMIN CONTABILIZACION MES ANTERIOR')) {
            this.fechaContabilizacionMin = new Date(new Date().getFullYear(), new Date().getMonth() - 1, 1);
            this.monthNavStatus = true;
        }
        else {
            this.fechaContabilizacionMin = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
        }        
    }

    /**
     * Calcula el porcentaje acumulado.
     */
    calcularPorcentajeAcumulado(rowData: any) : number {
        let totalPorcentaje = (rowData.Porcentaje * 1) + (rowData.PorcentajeACertificar * 1);
        return Math.min(totalPorcentaje, 100);
    }

    /**
     * Calcula el monto certificado anteriormente
     * a la certificacion actual.
     */
    calcularMontoAnterior(item: any): number {
        item.MontoAnterior = (item.CantidadReal * item.Importe);
        return item.MontoAnterior;
    }

    /**
     * Calcula el monto total general.
     * @returns 
     */
    calculateGeneralTotalAmount(): string {
        let montoTotalGeneral = 0;
        let moneda: string = ''
        moneda = this.entradaServicioSeleccionada[0].Items[0].Moneda;

        this.entradaServicioSeleccionada.forEach(position => {
            montoTotalGeneral += position.MontoTotalACertificar;
        });

        if (moneda === 'ARP') {
            return `$ ${montoTotalGeneral.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
        } else {
            return `${montoTotalGeneral.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
        }

    }

    /**
     * Validacion de numero de referencia.
     * @param event 
     */
    validarReferenciaRemito(event) {
        let element = document.getElementById("ref");

        if (element.classList.contains('ng-dirty') && element.classList.contains('ng-invalid')) {
            element.classList.add('error');
        } else {
            element.classList.remove('error');
        }
    }

    /**
     * Mostrar modal de resumen.
     */
    showIt(): void {
        this.showModal = true;
    }

    /**
     * Ocultar modal de resumen.
     */
    hideIt(): void {
        this.showModal = false;
    }
}