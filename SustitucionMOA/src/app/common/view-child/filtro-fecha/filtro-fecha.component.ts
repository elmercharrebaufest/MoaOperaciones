import { Component, Output, EventEmitter, ViewChild, OnInit, Input, ElementRef } from '@angular/core';
import { Formatter } from './../../formatter/Formatter';
import { DropdownComponent, DropdownOption } from './../dropdown/dropdown.component';
import { TipoPeriodo } from '../../enums/TipoPeriodo';
declare var $: any;

@Component({
    selector: 'filtro-fecha',
    templateUrl: `filtro-fecha.component.html`
})

export class FiltroFechaComponent implements OnInit {

    @Output() ClickEvent = new EventEmitter();
    @Input() leyenda?: string;

    @ViewChild(DropdownComponent)
    private dropdownComponent: DropdownComponent;

    @ViewChild('dtp_input1') dtpInput1?: ElementRef;
    @ViewChild('dtp_input2') dtpInput2?: ElementRef;

    @Input() key: string;
    @Input() periodoDefault: TipoPeriodo;

    periodo: string;
    fecha_inicio: string;
    fecha_fin: string;
    nombrePedido: string;
    constructor() {
        this.dropdownComponent = new DropdownComponent();
    }

    ngOnInit() {
        let periodoGeneral = sessionStorage.getItem("periodoGeneral");
        console.log("periodoGeneral", periodoGeneral);
        this.setPeriodoInitial(periodoGeneral || this.periodoDefault || this.obtenerPeriodo());
        this.dropdownComponent.setSelectItem(this.obtenerPeriodo());
    }

    ngAfterViewInit(): void {

        $(document).on("mouseover", '.form_datetime1', function () {
            $(".form_datetime1").datetimepicker({
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

        $(document).on("mouseover", '.form_datetime2', function () {
            $(".form_datetime2").datetimepicker({
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

    /**
     * Obtiene valor de periodo guardado en el sessionStorage. 
     * Si al instanciar el componente se definió un atributo 'key'
     * se utilizará el valor de ese atributo para buscar en el 
     * sessionStorage sino se buscará por la propiedad default.
     * 
     * @returns string Id de la opcíon guardada en la última selección.
     */
    obtenerPeriodo(): string {
        let periodoInicial = sessionStorage.getItem("periodo") ? sessionStorage.getItem("periodo") : "1";

        if (this.key != undefined && sessionStorage.getItem(this.key)) {
            periodoInicial = sessionStorage.getItem(this.key);
            this.periodo = periodoInicial;
        }

        return periodoInicial;
    }

    setDropdownOptions(options: Array<DropdownOption>) {
        this.dropdownComponent.options = options;
    }

    filterByDate(fechaHoraInicio: string, fechaHoraFin: string) {
        var fechaInicio = fechaHoraInicio;
        var fechaFin = fechaHoraFin;

        if (fechaHoraInicio != undefined) {
            var fechaHoraInicioArray = fechaHoraInicio.split(" ");
            if (fechaHoraInicioArray.length > 0)
                fechaInicio = fechaHoraInicioArray[0];
        }

        if (fechaHoraInicio != undefined) {
            var fechaHoraFinArray = fechaHoraFin.split(" ");
            if (fechaHoraFinArray.length > 0)
                fechaFin = fechaHoraFinArray[0];
        }

        this.setFechaIncio(fechaInicio);
        this.setFechaFin(fechaFin);
        sessionStorage.setItem("fechaInicio", fechaInicio);
        sessionStorage.setItem("fechaFin", fechaFin);
        this.ClickEvent.emit();
        return false;
    }

    setPeriodoInitial(periodo: string) {
        if (periodo == "4" || periodo == "2") {
            this.setFechaIncio(sessionStorage.getItem("fechaInicio") ? sessionStorage.getItem("fechaInicio") : Formatter.DateToSting(new Date(new Date().setDate(new Date().getDate() - 1))));
            this.setFechaFin(sessionStorage.getItem("fechaFin") ? sessionStorage.getItem("fechaFin") : Formatter.DateToSting(new Date()));
            this.guardarFechasYSetarPeriodo(periodo);
        } else {
            this.setPeriodo(periodo);
        }
    }

    setPeriodoGeneral(periodo: string) {
        sessionStorage.setItem("periodoGeneral", periodo);
        this.setPeriodo(periodo);
    }

    setPeriodo(periodo: string) {

        switch (periodo) {

            case "2":
                this.setFechaIncio(Formatter.DateToSting(new Date(new Date().setDate(new Date().getDate() - 7))));
                this.setFechaFin(Formatter.DateToSting(new Date()));
                this.guardarFechasYSetarPeriodo(periodo);
                this.ClickEvent.emit();
                break;

            case "3":
                this.setFechaIncio(Formatter.DateToSting(new Date(new Date().setDate(new Date().getDate() - 30))));
                this.setFechaFin(Formatter.DateToSting(new Date()));
                this.guardarFechasYSetarPeriodo(periodo);
                this.ClickEvent.emit();
                break;

            case "5":
                this.setFechaIncio(Formatter.DateToSting(new Date(new Date().setDate(new Date().getDate() - 60))));
                this.setFechaFin(Formatter.DateToSting(new Date()));
                this.guardarFechasYSetarPeriodo(periodo);
                this.ClickEvent.emit();
                break;

            case "4":
                this.setFechaIncio(Formatter.DateToSting(new Date(new Date().setDate(new Date().getDate() - 1))));
                this.setFechaFin(Formatter.DateToSting(new Date()));
                this.guardarFechasYSetarPeriodo(periodo);
                break;

            case "6":
                this.setFechaIncio(Formatter.DateToSting(new Date(new Date().setFullYear(new Date().getFullYear() - 1))));
                this.setFechaFin(Formatter.DateToSting(new Date()));
                this.guardarFechasYSetarPeriodo(periodo);
                this.ClickEvent.emit();
                break;

            case "7":
                this.setFechaIncio(Formatter.DateToSting(new Date(2020, 0, 1)));
                this.setFechaFin(Formatter.DateToSting(new Date()));
                this.guardarFechasYSetarPeriodo(periodo);
                this.ClickEvent.emit();
                break;
            case "1":
            default:
                this.setFechaIncio(Formatter.DateToSting(new Date(new Date().setDate(new Date().getDate() - 1))));
                this.setFechaFin(Formatter.DateToSting(new Date()));
                this.guardarFechasYSetarPeriodo(periodo);
                this.ClickEvent.emit();
                break;
        }

    }

    guardarFechasYSetarPeriodo(periodo: string) {
        sessionStorage.setItem("fechaInicio", this.fecha_inicio);
        sessionStorage.setItem("fechaFin", this.fecha_fin);
        this.dropdownComponent.setSelectItem(periodo);
        // Si no se definió una clave cuando se llamó al componente filtro-fecha
        // entonces usar default "periodo"
        const keyName = this.key != undefined ? this.key : "periodo"
        sessionStorage.setItem(keyName, periodo);
        this.periodo = periodo;
    }

    updateFechaFin(event: any) {
        this.fecha_inicio = "a";
    }

    setFechaIncio(fecha: string) {
        this.fecha_inicio = fecha;
    }

    setFechaFin(fecha: string) {
        this.fecha_fin = fecha;
    }

    getPeriodo() {
        return this.periodo;
    }

    getFechaIncio() {
        return this.fecha_inicio;
    }

    getFechaFin() {
        return this.fecha_fin;
    }

}