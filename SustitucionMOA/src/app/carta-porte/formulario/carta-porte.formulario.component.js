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
import { of as observableOf } from 'rxjs';
import { Component, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CartaPorteFormularioService } from './../carta-porte2.service';
import { ListBaseComponent } from './../../common/base-components/list-base-component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { DropdownComponent } from './../../common/view-child/dropdown/dropdown.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { Formulario } from './carta-porte.formulario';
import { Seccion } from './../../common/models/Seccion';
import { ModalService } from './../../common/services/ModalService';
var CartaPorteFormularioComponent = /** @class */ (function (_super) {
    __extends(CartaPorteFormularioComponent, _super);
    function CartaPorteFormularioComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService, route, router) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.route = route;
        _this.router = router;
        _this.formulario = new Formulario();
        _this.localidadCTG = "";
        _this.localidadDestinoCTG = "";
        _this.pageSelected = 1;
        return _this;
    }
    CartaPorteFormularioComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CREAR FORMULARIO CCPP"); };
    CartaPorteFormularioComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("carta-porte", "Formulario");
    };
    CartaPorteFormularioComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        var secciones = [new Seccion('/carta-porte/descarga', 'carta-porte', 'Descargas'), new Seccion('/carta-porte/aplicacion', 'carta-porte', 'Aplicaciones')];
        if (this.isAuthorized("CREAR FORMULARIO CCPP"))
            secciones.push(new Seccion('/carta-porte/formulario', 'carta-porte', 'Formulario'));
        this.navService.setSeccionList(secciones);
        this.formulario.cargaPesadaDestino = false;
        this.formulario.declaracionCalidad = false;
        this.formulario.conforme = false;
        this.formulario.condicional = false;
        this.getDataDropdown();
    };
    CartaPorteFormularioComponent.prototype.ngAfterViewInit = function () {
        $(document).on("mouseover", '.form_datetime_carga', function () {
            $('.form_datetime_carga').datetimepicker({
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
    CartaPorteFormularioComponent.prototype.getDataDropdown = function () {
        var _this = this;
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        try {
            this.subscription = this.service.getFormularioDropdowns().subscribe(function (result) {
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.floatMsgService.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.floatMsgService.setInfoMsg(result.info);
                }
                else {
                    _this.cosechaOptions = result.cosechas;
                    _this.destinoOptions = result.destinos;
                    _this.destinatarioOptions = result.destinatarios;
                    _this.localidadOptions = result.localidades;
                    _this.provinciaOptions = result.provincias;
                }
            }, function (error) {
                _this.floatMsgService.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    CartaPorteFormularioComponent.prototype.observableSource = function (keyword) {
        var filteredList = this.localidadOptions.filter(function (el) { return el.label.toLowerCase().indexOf(keyword.toLowerCase()) !== -1; });
        return observableOf(filteredList);
    };
    CartaPorteFormularioComponent.prototype.getDataCTG = function () {
        var _this = this;
        this.unsubscribe();
        this.floatMsgService.setMsgsEmpty();
        try {
            this.spinnerSmallCTGBuscarComponent.showIt();
            this.subscription = this.service.getDataCTG(this.formulario.nroCTG).subscribe(function (result) {
                _this.spinnerSmallCTGBuscarComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.floatMsgService.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.floatMsgService.setInfoMsg(result.info);
                }
                else {
                    _this.formulario.fechaCarga = result.data.fechaCarga;
                    _this.formulario.nroRenspa = result.data.renspa;
                    //Intermediarios
                    _this.formulario.intermediario = result.data.intervinientes.interm;
                    _this.formulario.cuitIntermediario = result.data.intervinientes.intermCuit;
                    _this.formulario.remitenteComercial = result.data.intervinientes.remCom;
                    _this.formulario.cuitRemitenteComercial = result.data.intervinientes.remComCuit;
                    _this.formulario.corredorComprador = result.data.intervinientes.corrComp;
                    _this.formulario.cuitCorredorComprador = result.data.intervinientes.corrCompCuit;
                    _this.formulario.mercadoATermino = result.data.intervinientes.mercadot;
                    _this.formulario.cuitMercadoATermino = result.data.intervinientes.mercodatCuit;
                    _this.formulario.corredorVendedor = result.data.intervinientes.corrVend;
                    _this.formulario.cuitCorredorVendedor = result.data.intervinientes.corrVendCuit;
                    _this.formulario.remitenteComercial = result.data.intervinientes.remCom;
                    _this.formulario.cuitRemitenteComercial = result.data.intervinientes.remComCuit;
                    _this.formulario.destinatario = result.data.intervinientes.destinatario;
                    _this.formulario.cuitDestinatario = result.data.intervinientes.destinatarioCuit;
                    _this.formulario.destino = result.data.intervinientes.destino;
                    _this.formulario.cuitDestino = result.data.intervinientes.destinoCuit;
                    _this.formulario.intermediarioFlete = result.data.intervinientes.interFlete;
                    _this.formulario.cuitIntermediarioFlete = result.data.intervinientes.interFleteCuit;
                    _this.formulario.transportista = result.data.intervinientes.transpor;
                    _this.formulario.cuitTransportista = result.data.intervinientes.transporCuit;
                    _this.formulario.chofer = result.data.intervinientes.chofer;
                    _this.formulario.cuitCuilChofer = result.data.intervinientes.choferCuit;
                    //Granos
                    _this.formulario.granoEspecie = result.data.granos.grano;
                    _this.formulario.cosecha = result.data.granos.cosecha;
                    _this.formulario.localidad = result.data.granos.localOrig;
                    _this.formulario.provincia = result.data.granos.provOrig;
                    _this.formulario.tipo = result.data.granos.tipo;
                    _this.formulario.nroContrato = result.data.granos.contrato;
                    _this.formulario.cargaPesadaDestino = result.data.granos.pesadaDest;
                    _this.formulario.kilosEstimados = result.data.granos.kilosEst;
                    _this.formulario.declaracionCalidad = result.data.granos.decCalidad;
                    _this.formulario.conforme = result.data.granos.conforme;
                    _this.formulario.condicional = result.data.granos.condicional;
                    _this.formulario.pesoBruto = result.data.granos.bruto;
                    _this.formulario.pesoNeto = result.data.granos.neto;
                    _this.formulario.pesoTara = result.data.granos.tara;
                    _this.formulario.observaciones = result.data.granos.observa;
                    _this.formulario.procedenciaMercaderia = result.data.granos.procedencia;
                    _this.formulario.establecimiento = result.data.granos.establecim;
                    //Destino
                    _this.formulario.direccionDestino = result.data.destino.direccion;
                    _this.formulario.localidadDestino = result.data.destino.localDest;
                    _this.formulario.provinciaDestino = result.data.destino.provDest;
                    //Transporte
                    _this.formulario.camion1 = result.data.transporte.camion;
                    _this.formulario.tarifaReferencia = result.data.transporte.tarifaRef;
                    _this.formulario.kmRecorrer = result.data.transporte.kmRecorrer;
                    _this.formulario.pagadorFlete = result.data.transporte.pagaFlete;
                    _this.formulario.fletePagado = result.data.transporte.fletePag;
                    _this.formulario.fletePagar = result.data.transporte.fleteAPag;
                    _this.formulario.acoplado = result.data.transporte.acoplado;
                    _this.formulario.tarifa = result.data.transporte.tarifa;
                }
            }, function (error) {
                _this.spinnerSmallCTGBuscarComponent.hideIt();
                _this.floatMsgService.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerSmallCTGBuscarComponent.hideIt();
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    CartaPorteFormularioComponent.prototype.generalFormatter = function (data) {
        return "" + data['label'];
    };
    CartaPorteFormularioComponent.prototype.cargarArchivo = function (event) {
        var fileList = event.target.files;
        if (fileList.length > 0) {
            this.ccppPDF = fileList[0];
        }
    };
    CartaPorteFormularioComponent.prototype.getCompletedPDFTemplate = function (fechaCarga) {
        var _this = this;
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        try {
            this.spinnerSmallImpTempComponent.showIt();
            this.getSpecialInputs(fechaCarga);
            this.subscription = this.service.getCompletedPDFTemplate(this.formulario, this.ccppPDF, this.pageSelected).subscribe(function (result) {
                _this.spinnerSmallImpTempComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.floatMsgService.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.floatMsgService.setInfoMsg(result.info);
                }
                else {
                    var byteArray = new Uint8Array(result.data);
                    var blob = new Blob([byteArray], { type: 'application/pdf' });
                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, "CCPPPrint.pdf");
                    }
                    else {
                        var objectURL = URL.createObjectURL(blob);
                        if ($('#print_page'))
                            $('#print_page').remove();
                        var iframe = document.createElement('iframe');
                        iframe.id = 'print_page';
                        iframe.className = 'sample-iframe';
                        iframe.src = objectURL;
                        document.getElementById("pdf-print").appendChild(iframe);
                        //document.body.appendChild(iframe);
                        iframe.focus();
                        iframe.contentWindow.print();
                    }
                }
            }, function (error) {
                _this.spinnerSmallImpTempComponent.hideIt();
                _this.floatMsgService.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.spinnerSmallImpTempComponent.hideIt();
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    CartaPorteFormularioComponent.prototype.getTemplate = function (fechaCarga) {
        var _this = this;
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        try {
            this.spinnerSmallImpBlancoComponent.showIt();
            this.getSpecialInputs(fechaCarga);
            this.subscription = this.service.getTemplate(this.formulario).subscribe(function (result) {
                _this.spinnerSmallImpBlancoComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.floatMsgService.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.floatMsgService.setInfoMsg(result.info);
                }
                else {
                    var bytes = new Uint8Array(result.data);
                    var blob = new Blob([bytes], {
                        type: 'application/pdf'
                    });
                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, "CCPPPrint.pdf");
                    }
                    else {
                        var objectURL = window.URL.createObjectURL(blob);
                        if ($('#print_page'))
                            $('#print_page').remove();
                        var iframe = document.createElement('iframe');
                        iframe.id = 'print_page';
                        iframe.className = 'sample-iframe';
                        iframe.src = objectURL;
                        document.getElementById("pdf-print").appendChild(iframe);
                        //document.body.appendChild(iframe);
                        iframe.focus();
                        iframe.contentWindow.print();
                    }
                }
            }, function (error) {
                _this.spinnerSmallImpBlancoComponent.hideIt();
                _this.floatMsgService.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerSmallImpBlancoComponent.hideIt();
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    CartaPorteFormularioComponent.prototype.setInputPages = function (event) {
        this.pageSelected = event;
    };
    CartaPorteFormularioComponent.prototype.destinatarioSelected = function (value) {
        this.formulario.destinatario = value.label;
    };
    CartaPorteFormularioComponent.prototype.destinoSelected = function (value) {
        this.formulario.destino = value.label;
    };
    CartaPorteFormularioComponent.prototype.cosechaSelected = function (value) {
        this.formulario.cosecha = value.label;
    };
    CartaPorteFormularioComponent.prototype.provinciaSelected = function (value) {
        this.formulario.provincia = value.label;
    };
    CartaPorteFormularioComponent.prototype.localidadSelected = function (value) {
        this.formulario.localidad = value.label;
    };
    CartaPorteFormularioComponent.prototype.provinciaDestinoSelected = function (value) {
        this.formulario.provinciaDestino = value.label;
    };
    CartaPorteFormularioComponent.prototype.localidadDestinoSelected = function (value) {
        this.formulario.localidadDestino = value.label;
    };
    CartaPorteFormularioComponent.prototype.getSpecialInputs = function (fechaCarga) {
        try {
            this.formulario.fechaCarga = fechaCarga.split(" ")[0];
        }
        catch (_a) { }
    };
    __decorate([
        ViewChild("dropdown_pages"),
        __metadata("design:type", DropdownComponent)
    ], CartaPorteFormularioComponent.prototype, "InputPagesComponent", void 0);
    __decorate([
        ViewChild("sp_imp_temp"),
        __metadata("design:type", SpinnerSmallComponent)
    ], CartaPorteFormularioComponent.prototype, "spinnerSmallImpTempComponent", void 0);
    __decorate([
        ViewChild("sp_imp_blanco"),
        __metadata("design:type", SpinnerSmallComponent)
    ], CartaPorteFormularioComponent.prototype, "spinnerSmallImpBlancoComponent", void 0);
    __decorate([
        ViewChild("sp_ctg_buscar"),
        __metadata("design:type", SpinnerSmallComponent)
    ], CartaPorteFormularioComponent.prototype, "spinnerSmallCTGBuscarComponent", void 0);
    CartaPorteFormularioComponent = __decorate([
        Component({
            selector: 'app-carta-porte-formulario',
            templateUrl: "carta-porte.formulario.component.html",
            providers: [CartaPorteFormularioService]
        }),
        __metadata("design:paramtypes", [CartaPorteFormularioService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService, ActivatedRoute, Router])
    ], CartaPorteFormularioComponent);
    return CartaPorteFormularioComponent;
}(ListBaseComponent));
export { CartaPorteFormularioComponent };
//# sourceMappingURL=carta-porte.formulario.component.js.map