import { Component, Input, OnInit, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { autoCompleteObject } from '../../listado-estado-certificaciones/listado-estado-certificaciones.component';
import { DropdownOption } from '../../../../common/view-child/dropdown/dropdown.component';
import { Formatter } from '../../../../common/formatter/Formatter';
declare var $: any;

@Component({
    selector: 'aux-pannel',
    templateUrl: './aux-pannel.component.html',
    styleUrls: ['./aux-pannel.component.css']
})

export class AuxPannelComponent implements OnInit {
    constructor() { }
    
    @Input() showOrHideAuxPanel: boolean = false;
    @Input() periodoSeleccionado: string = '1';
    @Input() proveedor: string = '';
    @Input() visible: boolean = false;
    @Output() obtenerESSap: EventEmitter<boolean> = new EventEmitter<boolean>();
    @Output() getFecha: EventEmitter<string> = new EventEmitter<string>();
    @Output() getOrdenCompra: EventEmitter<string> = new EventEmitter<string>();
    
    @Output() onFiltroFechaDesdeChanged: EventEmitter<string> = new EventEmitter<string>();
    @Output() onFiltroFechaHastaChanged: EventEmitter<string> = new EventEmitter<string>();

    @ViewChild('dtp_input1') dtpInput1?: ElementRef;
    @ViewChild('dtp_input2') dtpInput2?: ElementRef;

    ngOnInit(): void {
        this.setFechasSegunPeriodo(this.periodoSeleccionado);
        // Emitir las fechas iniciales para que el componente padre las reciba
        this.onFiltroFechaDesdeChanged.emit(this.filtroFechaDesde);
        this.onFiltroFechaHastaChanged.emit(this.filtroFechaHasta);
    }

    ngAfterViewInit(): void {
        $(document).on('mouseover', '.form_datetime1', function() {
            $('.form_datetime1').datetimepicker({
                format: 'yyyy-mm-dd',
                language: 'es',
                weekStart: 1,
                todayBtn: 1,
                autoclose: 1,
                todayHighlight: 1,
                startView: 2,
                forceParse: 0,
                showMeridian: 1,
                pickTime: false,
                minView: 2,
                maxView: 4
            });
        });

        $(document).on('mouseover', '.form_datetime2', function() {
            $('.form_datetime2').datetimepicker({
                format: 'yyyy-mm-dd',
                language: 'es',
                weekStart: 1,
                todayBtn: 1,
                autoclose: 1,
                todayHighlight: 1,
                startView: 2,
                forceParse: 0,
                showMeridian: 1,
                pickTime: false,
                minView: 2,
                maxView: 4
            });
        });
    }
    
    subscripciones: Subscription[] = [];
    proveedorSeleccionado: autoCompleteObject;
    filtroFechaDesde: string = "";
    filtroFechaHasta: string = "";
    ordenCompra: string = "";

    filtroFechas: Array<DropdownOption> = [
        new DropdownOption("1", "Últimos dos días"),
        new DropdownOption("2", "Última semana"),
        new DropdownOption("3", "Último mes"),
        new DropdownOption("4", "Entre fechas")
      ];

    /**
    * Muestra/Oculta un panel según nombre de clase
    * que lo identifica.
    * Sólo un panel puede estar activo a la vez.
    * @param className
    */
    togglePanel(className: string): void {
        let panels = document.getElementsByClassName('aux-panel') as HTMLCollectionOf<HTMLElement>;
        Array.from(panels).forEach(panel => {
            if (panel.id === className && panel.classList.contains('hidden')) {
                panel.classList.remove('hidden');
            }
            else {
                panel.classList.add('hidden');
            }
        });
    }

    setFechasSegunPeriodo(periodoSeleccionado: string) {
        let fechaDesde = new Date();
        switch (periodoSeleccionado) {
            case '1':
                fechaDesde.setDate(fechaDesde.getDate() - 2);
                break;

            case '2':
                fechaDesde.setDate(fechaDesde.getDate() - 7);
                break;

            case '3':
                fechaDesde.setDate(fechaDesde.getDate() - 30);
                break;

            case '4':
                fechaDesde.setDate(fechaDesde.getDate() - 1);
                break;
        }
        this.filtroFechaDesde = Formatter.DateToSting(fechaDesde);
        this.filtroFechaHasta = Formatter.DateToSting(new Date());
    }

    setDateByRange(event: string): void {
        this.setFechasSegunPeriodo(event);
        this.getFecha.emit(event);
        // Emitir las nuevas fechas cuando cambia el período
        this.onFiltroFechaDesdeChanged.emit(this.filtroFechaDesde);
        this.onFiltroFechaHastaChanged.emit(this.filtroFechaHasta);
    }

    setOrdenCompra() : void {
        this.getOrdenCompra.emit(this.ordenCompra);
    }
    
    onBuscar() {
        this.setearFechasDesdeHasta();
        this.onFiltroFechaDesdeChanged.emit(this.filtroFechaDesde);
        this.onFiltroFechaHastaChanged.emit(this.filtroFechaHasta);
        this.collapseExpandedRow();

        if (this.proveedorSeleccionado !== undefined) {
            this.proveedor = this.proveedorSeleccionado.CodigoProveedor;
        }
        else {
            this.proveedor = '';
        }

        this.obtenerESSap.emit(true);
        return false;
    }

    setearFechasDesdeHasta() {
        if (this.periodoSeleccionado != "4" || !this.dtpInput1 || !this.dtpInput2) {
            return; // Cuando el periodo no es 'Entre fechas', las fechas Desde y Hasta ya fueron seteadas al momento de seleccionar el tipo de periodo
        }

        let fechaHoraInicio = this.dtpInput1.nativeElement.value;
        let fechaHoraFin = this.dtpInput2.nativeElement.value;

        let fechaDesde = fechaHoraInicio;
        let fechaHasta = fechaHoraFin;

        if (fechaHoraInicio != undefined) {
            var fechaHoraInicioArray = fechaHoraInicio.split(" ");
            if (fechaHoraInicioArray.length > 0) {
                fechaDesde = fechaHoraInicioArray[0];
            }
        }

        if (fechaHoraFin != undefined) {
            var fechaHoraFinArray = fechaHoraFin.split(" ");
            if (fechaHoraFinArray.length > 0) {
                fechaHasta = fechaHoraFinArray[0];
            }
        }

        this.filtroFechaDesde = fechaDesde;
        this.filtroFechaHasta = fechaHasta;
    }
    
    collapseExpandedRow() {
        let elementExpanded = document.querySelector('.pi-chevron-down') as HTMLElement;
        if (elementExpanded != null) {
            elementExpanded.click();
        }
    }
}
