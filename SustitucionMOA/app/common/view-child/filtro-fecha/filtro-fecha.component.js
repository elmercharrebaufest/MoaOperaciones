"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var Formatter_1 = require("./../../formatter/Formatter");
var dropdown_component_1 = require("./../dropdown/dropdown.component");
var FiltroFechaComponent = /** @class */ (function () {
    function FiltroFechaComponent() {
        this.ClickEvent = new core_1.EventEmitter();
        this.dropdownComponent = new dropdown_component_1.DropdownComponent();
        this.setPeriodoInitial(sessionStorage.getItem("periodo") ? sessionStorage.getItem("periodo") : "1");
    }
    FiltroFechaComponent.prototype.ngOnInit = function () {
        this.dropdownComponent.setSelectItem(sessionStorage.getItem("periodo") ? sessionStorage.getItem("periodo") : "1");
    };
    FiltroFechaComponent.prototype.ngAfterViewInit = function () {
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
    };
    FiltroFechaComponent.prototype.setDropdownOptions = function (options) {
        this.dropdownComponent.options = options;
    };
    FiltroFechaComponent.prototype.filterByDate = function (fechaHoraInicio, fechaHoraFin) {
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
    };
    FiltroFechaComponent.prototype.setPeriodoInitial = function (periodo) {
        if (periodo == "4") {
            this.setFechaIncio(sessionStorage.getItem("fechaInicio") ? sessionStorage.getItem("fechaInicio") : Formatter_1.Formatter.DateToSting(new Date(new Date().setDate(new Date().getDate() - 1))));
            this.setFechaFin(sessionStorage.getItem("fechaFin") ? sessionStorage.getItem("fechaFin") : Formatter_1.Formatter.DateToSting(new Date()));
            sessionStorage.setItem("fechaInicio", this.fecha_inicio);
            sessionStorage.setItem("fechaFin", this.fecha_fin);
            this.periodo = periodo;
        }
        else {
            this.setPeriodo(periodo);
        }
    };
    FiltroFechaComponent.prototype.setPeriodo = function (periodo) {
        switch (periodo) {
            case "2":
                this.setFechaIncio(Formatter_1.Formatter.DateToSting(new Date(new Date().setDate(new Date().getDate() - 7))));
                this.setFechaFin(Formatter_1.Formatter.DateToSting(new Date()));
                this.guardarFechasYSetarPeriodo(periodo);
                this.ClickEvent.emit();
                break;
            case "3":
                this.setFechaIncio(Formatter_1.Formatter.DateToSting(new Date(new Date().setDate(new Date().getDate() - 30))));
                this.setFechaFin(Formatter_1.Formatter.DateToSting(new Date()));
                this.guardarFechasYSetarPeriodo(periodo);
                this.ClickEvent.emit();
                break;
            case "5":
                this.setFechaIncio(Formatter_1.Formatter.DateToSting(new Date(new Date().setDate(new Date().getDate() - 60))));
                this.setFechaFin(Formatter_1.Formatter.DateToSting(new Date()));
                this.guardarFechasYSetarPeriodo(periodo);
                this.ClickEvent.emit();
                break;
            case "4":
                this.setFechaIncio(Formatter_1.Formatter.DateToSting(new Date(new Date().setDate(new Date().getDate() - 1))));
                this.setFechaFin(Formatter_1.Formatter.DateToSting(new Date()));
                this.guardarFechasYSetarPeriodo(periodo);
                break;
            case "1":
            default:
                this.setFechaIncio(Formatter_1.Formatter.DateToSting(new Date(new Date().setDate(new Date().getDate() - 1))));
                this.setFechaFin(Formatter_1.Formatter.DateToSting(new Date()));
                this.guardarFechasYSetarPeriodo(periodo);
                this.ClickEvent.emit();
                break;
        }
    };
    FiltroFechaComponent.prototype.guardarFechasYSetarPeriodo = function (periodo) {
        sessionStorage.setItem("fechaInicio", this.fecha_inicio);
        sessionStorage.setItem("fechaFin", this.fecha_fin);
        this.dropdownComponent.setSelectItem(periodo);
        sessionStorage.setItem("periodo", periodo);
        this.periodo = periodo;
    };
    FiltroFechaComponent.prototype.updateFechaFin = function (event) {
        this.fecha_inicio = "a";
    };
    FiltroFechaComponent.prototype.setFechaIncio = function (fecha) {
        this.fecha_inicio = fecha;
    };
    FiltroFechaComponent.prototype.setFechaFin = function (fecha) {
        this.fecha_fin = fecha;
    };
    FiltroFechaComponent.prototype.getPeriodo = function () {
        return this.periodo;
    };
    FiltroFechaComponent.prototype.getFechaIncio = function () {
        return this.fecha_inicio;
    };
    FiltroFechaComponent.prototype.getFechaFin = function () {
        return this.fecha_fin;
    };
    __decorate([
        core_1.Output(),
        __metadata("design:type", Object)
    ], FiltroFechaComponent.prototype, "ClickEvent", void 0);
    __decorate([
        core_1.ViewChild(dropdown_component_1.DropdownComponent),
        __metadata("design:type", dropdown_component_1.DropdownComponent)
    ], FiltroFechaComponent.prototype, "dropdownComponent", void 0);
    FiltroFechaComponent = __decorate([
        core_1.Component({
            selector: 'filtro-fecha',
            templateUrl: "./app/common/view-child/filtro-fecha/filtro-fecha.component.html?v=" + new Date().getTime()
        }),
        __metadata("design:paramtypes", [])
    ], FiltroFechaComponent);
    return FiltroFechaComponent;
}());
exports.FiltroFechaComponent = FiltroFechaComponent;
//# sourceMappingURL=filtro-fecha.component.js.map