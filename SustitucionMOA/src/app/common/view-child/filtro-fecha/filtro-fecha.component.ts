import { Component, Output, EventEmitter, ViewChild, OnInit, Input } from '@angular/core';
import { Formatter } from './../../formatter/Formatter';
import { DropdownComponent, DropdownOption } from './../dropdown/dropdown.component';
declare var $: any;

@Component({
    selector: 'filtro-fecha',
    templateUrl: `filtro-fecha.component.html`
})

export class FiltroFechaComponent implements OnInit {

    @Output() ClickEvent = new EventEmitter();
    @Input()leyenda?:string;

    @ViewChild(DropdownComponent)
    private dropdownComponent: DropdownComponent;

    periodo: string;
    fecha_inicio: string;
    fecha_fin: string;

    constructor() {
        this.dropdownComponent = new DropdownComponent();
        this.setPeriodoInitial(sessionStorage.getItem("periodo") ? sessionStorage.getItem("periodo") : "1");
    }

    ngOnInit() {
        this.dropdownComponent.setSelectItem(sessionStorage.getItem("periodo") ? sessionStorage.getItem("periodo") : "1");
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
        if (periodo == "4") {
            this.setFechaIncio(sessionStorage.getItem("fechaInicio") ? sessionStorage.getItem("fechaInicio") : Formatter.DateToSting(new Date(new Date().setDate(new Date().getDate() - 1))));
            this.setFechaFin(sessionStorage.getItem("fechaFin") ? sessionStorage.getItem("fechaFin") : Formatter.DateToSting(new Date()));
            sessionStorage.setItem("fechaInicio", this.fecha_inicio);
            sessionStorage.setItem("fechaFin", this.fecha_fin);
            this.periodo = periodo;
        } else {
            this.setPeriodo(periodo);
        }
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
        sessionStorage.setItem("periodo", periodo);
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