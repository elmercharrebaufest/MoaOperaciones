"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
var ryd_service_1 = require("./../ryd.service");
var ryd_component_1 = require("./../ryd.component");
var ryd_1 = require("./../ryd");
var mensaje_component_1 = require("./../../common/view-child/mensaje/mensaje.component");
var spinner_component_1 = require("./../../common/view-child/spinner/spinner.component");
var dropdown_component_1 = require("./../../common/view-child/dropdown/dropdown.component");
var NavService_1 = require("./../../common/services/NavService");
var FloatMsgService_1 = require("./../../common/services/FloatMsgService");
var SecurityService_1 = require("./../../common/services/SecurityService");
var SessionDataService_1 = require("./../../common/services/SessionDataService");
var Formatter_1 = require("./../../common/formatter/Formatter");
var ModalService_1 = require("./../../common/services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
require("rxjs/add/operator/takeUntil");
var CargaPesadasComponent = /** @class */ (function (_super) {
    __extends(CargaPesadasComponent, _super);
    function CargaPesadasComponent(service, navService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.habilitarCargaPesadas = false;
        _this.numeroPesada = 1;
        _this.fechaPesada = "";
        _this.pesoTara = 0;
        _this.pesoBruto = 0;
        _this.pesoNeto = 0;
        _this.pesadas = [];
        _this.itemsPerPage = 10;
        _this.InputBalanzaComponent = new dropdown_component_1.DropdownComponent();
        _this.InputCommoditiesComponent = new dropdown_component_1.DropdownComponent();
        _this.InputExportadoresComponent = new dropdown_component_1.DropdownComponent();
        _this.mensajeTopComponent = new mensaje_component_1.MensajeComponent();
        _this.mensajeBottomComponent = new mensaje_component_1.MensajeComponent();
        _this.spinnerTopComponent = new spinner_component_1.SpinnerComponent();
        _this.spinnerBottomComponent = new spinner_component_1.SpinnerComponent();
        return _this;
    }
    ;
    CargaPesadasComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("REGISTRAR PESADA"); };
    CargaPesadasComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("ryd", "Carga de Pesadas");
    };
    CargaPesadasComponent.prototype.ngOnInit = function () {
        _super.prototype.ngOnInit.call(this);
        this.initCargaPesada();
        this.getDataInputs();
    };
    CargaPesadasComponent.prototype.ngAfterViewInit = function () {
        $('.form_datetime').datetimepicker({
            format: "dd/mm/yyyy HH:ii",
            language: 'es',
            weekStart: 1,
            todayBtn: 1,
            autoclose: 1,
            todayHighlight: 1,
            startView: 2,
            forceParse: 0,
            showMeridian: 1,
            minuteStep: 1,
            useCurrent: true
        });
        $(document).on("focus", ".form_datetimePesadas", function () {
            $(this).datetimepicker({
                format: "dd/mm/yyyy HH:ii",
                language: 'es',
                weekStart: 1,
                todayBtn: 1,
                autoclose: 1,
                todayHighlight: 1,
                startView: 2,
                forceParse: 0,
                showMeridian: 1,
                minuteStep: 1,
                useCurrent: true
            });
        });
    };
    CargaPesadasComponent.prototype.getDataInputs = function () {
        var _this = this;
        this.setAllMsgsEmpty();
        this.subscriptionDropDowns = this.service.getInputsCargaPesadas().subscribe(function (result) {
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.mensajeTopComponent.setErrorMsg(result.error);
            }
            else {
                _this.balanzaOptions = result.data.balanzas;
                _this.bodegaOptions = result.data.bodegas;
                _this.commodityOptions = result.data.commodities;
                _this.destinoOptions = result.data.destinos;
                _this.exportadorOptions = result.data.exportadores;
                _this.vaporOptions = result.data.vapores;
            }
        }, function (error) {
            _this.mensajeTopComponent.setErrorMsg(error.message);
        });
    };
    CargaPesadasComponent.prototype.validarEncabezado = function (fecha) {
        try {
            this.setAllMsgsEmpty();
            this.validateInput(this.cargaPesadas.balanza, "Balanza");
            this.cargaPesadas.bodega = this.validateInput(this.bodegaInput, "Bodega");
            this.cargaPesadas.commodity = this.validateInput(this.cargaPesadas.commodity, "Commodity");
            this.cargaPesadas.destino = this.validateInput(this.destinoInput, "Destino");
            this.cargaPesadas.exportador = this.validateInput(this.cargaPesadas.exportador, "Exportador");
            this.cargaPesadas.vapor = this.validateInput(this.vaporInput, "Vapor");
            this.cargaPesadas.pesoAcumulado = 0;
            this.cargaPesadas.fecha = Formatter_1.Formatter.parseFecha(fecha);
            this.habilitarCargaPesadas = !this.habilitarCargaPesadas;
        }
        catch (err) {
            this.mensajeTopComponent.setErrorMsg(err.message);
        }
        return false; // <-- Prevent refresh.
    };
    CargaPesadasComponent.prototype.agregarPesada = function (fecha) {
        var _this = this;
        try {
            this.setAllMsgsEmpty();
            this.showAllSpinners();
            var pesada = new ryd_1.Pesada();
            pesada.numeroPesada = this.numeroPesada;
            pesada.fecha = Formatter_1.Formatter.parseFecha(fecha);
            pesada.pesoBruto = this.pesoBruto;
            pesada.pesoNeto = this.pesoNeto;
            pesada.pesoTara = this.pesoTara;
            this.cargaPesadas.pesadas = [pesada];
            this.unsubscribe();
            this.subscription = this.service.postRegistrarPesadas(this.cargaPesadas.balanza, this.cargaPesadas.fecha, this.cargaPesadas.bodega, this.cargaPesadas.commodity, this.cargaPesadas.destino, this.cargaPesadas.exportador, this.cargaPesadas.vapor, this.cargaPesadas.pesoProgramado, this.cargaPesadas.pesoAcumulado, this.numeroPesada, pesada.fecha, this.pesoTara, this.pesoBruto).subscribe(function (result) {
                _this.hideAllSpinners();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != null) {
                    _this.setAllErrorMsgs(result.error);
                }
                else {
                    _this.pesadas.push(pesada);
                    _this.numeroPesada++;
                    _this.cargaPesadas.pesoAcumulado = +_this.cargaPesadas.pesoAcumulado + +pesada.pesoNeto;
                    _this.vaciarPesada();
                }
            }, function (error) {
                _this.hideAllSpinners();
                _this.setAllErrorMsgs(error.message);
            });
        }
        catch (_a) {
        }
        return false; // <-- Prevent refresh.
    };
    CargaPesadasComponent.prototype.finalizarPesadas = function () {
        var _this = this;
        this.setAllMsgsEmpty();
        this.showAllSpinners();
        this.unsubscribe();
        this.subscription = this.service.postFinalizarCargaPesadas(this.cargaPesadas.balanza, this.cargaPesadas.fecha).subscribe(function (result) {
            _this.hideAllSpinners();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.setAllErrorMsgs(result.error);
            }
            else if (result.info != undefined) {
                _this.setAllInfoMsgs(result.info);
            }
            else {
                _this.mensajeTopComponent.setSuccessMsg(result.data);
                _this.emptyAll();
                _this.InputBalanzaComponent.setSelectItem("");
                _this.cargaPesadas.balanza = "";
            }
        }, function (error) {
            _this.hideAllSpinners();
            _this.setAllErrorMsgs(error.message);
        });
        return false; // <-- Prevent refresh.
    };
    CargaPesadasComponent.prototype.verificarBalanzaEnProceso = function () {
        var _this = this;
        this.spinnerTopComponent.showIt();
        this.setAllMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.verificarBalanzaEnProceso(this.cargaPesadas.balanza).subscribe(function (result) {
            _this.spinnerTopComponent.hideIt();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.mensajeTopComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.mensajeTopComponent.setInfoMsg(result.info);
            }
            else {
                _this.setDatosCarga(result.data);
            }
        }, function (error) {
            _this.spinnerTopComponent.hideIt();
            _this.mensajeTopComponent.setErrorMsg(error.message);
        });
        return false;
    };
    CargaPesadasComponent.prototype.initCargaPesada = function () {
        this.cargaPesadas = new ryd_1.CargaPesadas();
        this.cargaPesadas.pesoProgramado = 0;
        this.cargaPesadas.pesoAcumulado = 0;
        this.cargaPesadas.fecha = "";
        this.cargaPesadas.pesadas = [];
    };
    CargaPesadasComponent.prototype.emptyAll = function () {
        this.vaciarPesada();
        this.setDatosCarga(null);
    };
    CargaPesadasComponent.prototype.vaciarPesada = function () {
        this.pesoBruto = 0;
        this.pesoNeto = 0;
        this.pesoTara = 0;
    };
    CargaPesadasComponent.prototype.validateInput = function (value, nombreInput) {
        if (value == "" || value == undefined) {
            throw Error("El valor del campo " + nombreInput + " no es válido.");
        }
        else if (value.value != undefined) {
            return value.value;
        }
        else {
            return value;
        }
    };
    CargaPesadasComponent.prototype.showAllSpinners = function () {
        this.spinnerTopComponent.showIt();
        this.spinnerBottomComponent.showIt();
    };
    CargaPesadasComponent.prototype.hideAllSpinners = function () {
        this.spinnerTopComponent.hideIt();
        this.spinnerBottomComponent.hideIt();
    };
    CargaPesadasComponent.prototype.setAllMsgsEmpty = function () {
        this.mensajeTopComponent.setMsgsEmpty();
        this.mensajeBottomComponent.setMsgsEmpty();
    };
    CargaPesadasComponent.prototype.setAllErrorMsgs = function (msg) {
        this.mensajeTopComponent.setErrorMsg(msg);
        this.mensajeBottomComponent.setErrorMsg(msg);
    };
    CargaPesadasComponent.prototype.setAllInfoMsgs = function (msg) {
        this.mensajeTopComponent.setInfoMsg(msg);
        this.mensajeBottomComponent.setInfoMsg(msg);
    };
    CargaPesadasComponent.prototype.setAllSuccessMsgs = function (msg) {
        this.mensajeTopComponent.setSuccessMsg(msg);
        this.mensajeBottomComponent.setSuccessMsg(msg);
    };
    CargaPesadasComponent.prototype.setInputBalanza = function (value) {
        this.cargaPesadas.balanza = value;
        this.verificarBalanzaEnProceso();
    };
    CargaPesadasComponent.prototype.setInputCommodity = function (value) {
        this.cargaPesadas.commodity = value;
    };
    CargaPesadasComponent.prototype.setInputExportador = function (value) {
        this.cargaPesadas.exportador = value;
    };
    CargaPesadasComponent.prototype.generalFormatter = function (data) {
        return "" + data['label'];
    };
    CargaPesadasComponent.prototype.calcularPesoNeto = function () {
        this.pesoNeto = this.pesoBruto - this.pesoTara;
    };
    CargaPesadasComponent.prototype.setDatosCarga = function (carga) {
        if (carga != undefined) {
            this.cargaPesadas.pesadas = carga.pesadas;
            this.cargaPesadas.fecha = carga.fecha;
            this.cargaPesadas.hora = carga.hora;
            this.cargaPesadas.commodity = carga.commodity;
            this.cargaPesadas.exportador = carga.exportador;
            this.cargaPesadas.pesoProgramado = carga.pesoProgramado;
            this.cargaPesadas.pesoAcumulado = carga.pesoAcumulado;
            this.bodegaInput = this.setSelectItem(carga.bodega, this.bodegaOptions);
            this.cargaPesadas.bodega = carga.bodega;
            this.InputCommoditiesComponent.setSelectItem(carga.commodity);
            this.destinoInput = this.setSelectItem(carga.destino, this.destinoOptions);
            this.cargaPesadas.destino = carga.destino;
            this.InputExportadoresComponent.setSelectItem(carga.exportador);
            this.vaporInput = this.setSelectItem(carga.vapor, this.vaporOptions);
            this.cargaPesadas.vapor = carga.vapor;
            this.pesadas = carga.pesadas;
            this.numeroPesada = carga.pesadas.length + 1;
            this.habilitarCargaPesadas = true;
        }
        else {
            this.bodegaInput = null;
            this.cargaPesadas.bodega = "";
            this.InputCommoditiesComponent.setSelectItem("");
            this.destinoInput = null;
            this.cargaPesadas.destino = "";
            this.InputExportadoresComponent.setSelectItem("");
            this.vaporInput = null;
            this.cargaPesadas.vapor = "";
            this.cargaPesadas.commodity = "";
            this.cargaPesadas.exportador = "";
            this.cargaPesadas.fecha = "";
            this.cargaPesadas.pesoAcumulado = 0;
            this.cargaPesadas.pesoProgramado = 0;
            this.pesadas = [];
            this.numeroPesada = 1;
            this.habilitarCargaPesadas = false;
        }
    };
    CargaPesadasComponent.prototype.setSelectItem = function (value, options) {
        if (options != null) {
            for (var _i = 0, options_1 = options; _i < options_1.length; _i++) {
                var option = options_1[_i];
                if (option.value == value) {
                    return option;
                }
            }
        }
    };
    __decorate([
        core_1.ViewChild("dropdown_balanza"),
        __metadata("design:type", dropdown_component_1.DropdownComponent)
    ], CargaPesadasComponent.prototype, "InputBalanzaComponent", void 0);
    __decorate([
        core_1.ViewChild("dropdown_commodity"),
        __metadata("design:type", dropdown_component_1.DropdownComponent)
    ], CargaPesadasComponent.prototype, "InputCommoditiesComponent", void 0);
    __decorate([
        core_1.ViewChild("dropdown_exportador"),
        __metadata("design:type", dropdown_component_1.DropdownComponent)
    ], CargaPesadasComponent.prototype, "InputExportadoresComponent", void 0);
    __decorate([
        core_1.ViewChild("topMensaje"),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], CargaPesadasComponent.prototype, "mensajeTopComponent", void 0);
    __decorate([
        core_1.ViewChild("bottomMensaje"),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], CargaPesadasComponent.prototype, "mensajeBottomComponent", void 0);
    __decorate([
        core_1.ViewChild("topSpinner"),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], CargaPesadasComponent.prototype, "spinnerTopComponent", void 0);
    __decorate([
        core_1.ViewChild("bottomSpinner"),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], CargaPesadasComponent.prototype, "spinnerBottomComponent", void 0);
    CargaPesadasComponent = __decorate([
        core_1.Component({
            selector: 'app-ryd-cargas-pesadas',
            templateUrl: "./app/ryd/carga-pesadas/carga-pesadas.component.html?v=" + new Date().getTime(),
            providers: [ryd_service_1.RYDService]
        }),
        __metadata("design:paramtypes", [ryd_service_1.RYDService, NavService_1.NavService, SecurityService_1.SecurityService, SessionDataService_1.SessionDataService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], CargaPesadasComponent);
    return CargaPesadasComponent;
}(ryd_component_1.RYDBaseComponent));
exports.CargaPesadasComponent = CargaPesadasComponent;
//# sourceMappingURL=carga-pesadas.component.js.map