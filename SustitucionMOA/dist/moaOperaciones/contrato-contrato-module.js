(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["contrato-contrato-module"],{

/***/ "./src/app/contrato/ampliacion/contrato.ampliacion.component.html":
/*!************************************************************************!*\
  !*** ./src/app/contrato/ampliacion/contrato.ampliacion.component.html ***!
  \************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"content-wraper\">\r\n    <!-- CONTENT-->\r\n    <section class=\"container-fluid clearfix\">\r\n        <!--Title-->\r\n        <div class=\"row titleSection\">\r\n            <div class=\"col-xs-12 col-sm-10 \">Contrato: <strong>Ampliaciones</strong> </div>\r\n            <div class=\"col-xs-12 col-sm-2\">\r\n                <spinner-small></spinner-small>\r\n                <a (click)=\"exportExcel()\" href=\"#\" style=\"text-decoration:none!important\" *ngIf=\"!this.spinnerSmallComponent.visible\">\r\n                    <div class=\"btnSection animate\">Exportar <span class=\"fa  fa-file-excel-o\"></span></div>\r\n                </a>\r\n            </div>\r\n        </div>\r\n        <!--End Title-->\r\n        <!--Search-->\r\n        <div class=\"row  myRow firstSection mySearch\">\r\n            <div class=\"col-xs-12 col-md-12 rounded shadow animate\">\r\n                <title-caps>Refinar resultados</title-caps>\r\n                <hr />\r\n                <div id=\"searchVer\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Ver:\r\n                    <filtro-fecha (ClickEvent)=\"getData()\"></filtro-fecha>\r\n                </div>\r\n\r\n                <div id=\"searchVendedor\" class=\"col-xs-12 col-sm-6 col-md-15\"> <!--*ngIf=\"isCorredor()\"-->\r\n                    Por Vendedor:\r\n                    <div class=\"styled-select \">\r\n                        <dropdown [tipoDropdown]=\"'filtroVariable'\" [options]=\"filtroVendedor\" (select)=\"setFiltroVendedor($event)\"></dropdown>\r\n                    </div>\r\n                </div>\r\n                <div id=\"searchProducto\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Por Producto\r\n                    <div class=\"styled-select \">\r\n                        <dropdown [tipoDropdown]=\"'filtroVariable'\" [options]=\"filtroProducto\" (select)=\"setFiltroProducto($event)\"></dropdown>\r\n                    </div>\r\n                </div>\r\n                <div id=\"searchContrato\" class=\"col-xs-12 col-sm-6 col-md-15 \">\r\n                    Por Contrato:\r\n                    <div class=\"styled-select \">\r\n                        <input type=\"search\" class=\"\" placeholder=\"\" [value]=\"filtroContrato\" (input)=\"filtroContrato = $event.target.value\" aria-controls=\"tableModal\">\r\n                    </div>\r\n                </div>\r\n                <div id=\"searchMostrar\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Mostrar:\r\n                    <div class=\"styled-select \">\r\n                        <dropdown [tipoDropdown]=\"'numberItems'\" (select)=\"setItemsPerPage($event)\"></dropdown>\r\n                    </div>\r\n                </div>\r\n                <div id=\"showSearch\" onClick=\"openSearch()\">\r\n                    <div style=\"\">\r\n                        <div class=\"fa fa-angle-double-right\" aria-hidden=\"true\"></div>\r\n                    </div>\r\n                    <div>VER MÁS OPCIONES DE BÚSQUEDA</div>\r\n                </div>\r\n                <div id=\"hideSearch\" onClick=\"closeSearch()\">\r\n                    <div style=\"\">\r\n                        <div class=\"fa fa-times\" aria-hidden=\"true\"></div>\r\n                    </div>\r\n                    <div>OCULTAR OPCIONES DE BÚSQUEDA</div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n        <!-- End Search-->\r\n        <!-- Tables-->\r\n\r\n        <div class=\"row myRow\">\r\n            <div class=\"clearfix myTables\">\r\n\r\n                <mensaje></mensaje>\r\n                <spinner></spinner>\r\n\r\n                <table *ngIf=\"isVisible()\" id=\"tableModal\" class=\"display nowrap dataTable no-footer dtr-inline rounded shadow animate contrato-ampliacion-table\" cellspacing=\"0\" role=\"grid\" aria-describedby=\"tableModal_info\" style=\"width: 100%; padding:10px 0;\">\r\n                    <thead>\r\n                        <tr role=\"row\">\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('fechaDate')\">Fecha de ampliaci&oacute;n</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('nroContrato')\">Contrato Molinos</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('contrvend')\">Contrato Proveedor</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('material')\">Producto</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('cantKilos')\">Pactado</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('ampliado')\">Ampliado</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('total')\">Total</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('vendedor')\" *ngIf=\"isCorredor()\">Vendedor</th>\r\n                        </tr>\r\n                    </thead>\r\n                    <tbody>\r\n                        <tr *ngFor=\"let contratoInfo of data.contratosInfo | customFilter : 'material' : productoSelected | customFilter : 'vendedor' : vendedorSelected | customFilter : 'nroContrato' : filtroContrato | orderedColumn : {property: orderedByColumn, direction: orderDirection} | paginate: { itemsPerPage:itemsPerPage, currentPage: p } ; let odd = odd\" role=\"row\" [class.odd]=\"!odd\">\r\n                            <td><a (click)=\"showModalTableResponsive(contratoInfo)\" class=\"btnShowModal\"><i class=\"fa fa-plus fa-lg\" aria-hidden=\"true\"></i></a> {{contratoInfo.fecha}}</td>\r\n                            <td><a class=\"animate\" (click)=\"goToSeccionParam('/contrato/detalle', contratoInfo.nroContrato)\" href=\"#\">{{contratoInfo.nroContrato}}</a></td>\r\n                            <td>{{contratoInfo.contrvend}}</td>\r\n                            <td>{{contratoInfo.material}}</td>\r\n                            <td>{{contratoInfo.cantKilosString}}</td>\r\n                            <td>{{contratoInfo.ampliadoString}}</td>\r\n                            <td>{{contratoInfo.totalString}}</td>\r\n                            <td *ngIf=\"isCorredor()\"><a (click)=\"goToSeccionParamDos('/dato-fiscal/situacion-fiscal', contratoInfo.idVendedor, contratoInfo.vendedor)\" href=\"#\" class=\"animate\">{{contratoInfo.vendedor}}</a></td>\r\n                        </tr>\r\n                    </tbody>\r\n                </table>\r\n\r\n                <pagination-template #pT=\"paginationApi\" (pageChange)=\"p = $event\" *ngIf=\"isVisible()\">\r\n                    <div class=\"dataTables_paginate paging_simple_numbers\">\r\n                        <a id=\"tableModal_previous\" class=\"paginate_button previous\" (click)=\"pT.previous()\" [class.disabled]=\"pT.isFirstPage()\"> Anterior </a>\r\n                        <span *ngFor=\"let page of pT.pages\" [class.current]=\"pT.getCurrent() === page.value\">\r\n                            <a class=\"paginate_button\" (click)=\"pT.setCurrent(page.value)\" *ngIf=\"pT.getCurrent() !== page.value\">{{ page.label }}</a>\r\n                            <a class=\"paginate_button current\" *ngIf=\"pT.getCurrent() === page.value\">{{ page.label }}</a>\r\n                        </span>\r\n                        <a id=\"tableModal_next\" class=\"paginate_button next\" (click)=\"pT.next()\" [class.disabled]=\"pT.isLastPage()\"> Siguiente </a>\r\n                    </div>\r\n                </pagination-template>\r\n            </div>\r\n        </div>\r\n        <!-- END tables-->\r\n    </section>\r\n    <!--End CONTENT-->\r\n</div>\r\n"

/***/ }),

/***/ "./src/app/contrato/ampliacion/contrato.ampliacion.component.ts":
/*!**********************************************************************!*\
  !*** ./src/app/contrato/ampliacion/contrato.ampliacion.component.ts ***!
  \**********************************************************************/
/*! exports provided: ContratoAmpliacionComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContratoAmpliacionComponent", function() { return ContratoAmpliacionComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _contrato_component__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./../contrato.component */ "./src/app/contrato/contrato.component.ts");
/* harmony import */ var _contrato_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./../contrato.service */ "./src/app/contrato/contrato.service.ts");
var __extends = (undefined && undefined.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};



var ContratoAmpliacionComponent = /** @class */ (function (_super) {
    __extends(ContratoAmpliacionComponent, _super);
    function ContratoAmpliacionComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.tituloArchivo = "ReporteAnulaciones.xls";
        return _this;
    }
    ContratoAmpliacionComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("contrato", "Ampliaciones");
    };
    ContratoAmpliacionComponent.prototype.showModalTableResponsive = function (contratoInfo) {
        this.modalService.openModalTableResponsive("Contrato", [
            { etiqueta: "Fecha de Anulación", valor: contratoInfo.fecha },
            { etiqueta: "Contrato Molinos", valor: contratoInfo.nroContrato },
            { etiqueta: "Contrato Proveedor", valor: contratoInfo.contrvend },
            { etiqueta: "Producto", valor: contratoInfo.material },
            { etiqueta: "Pactado", valor: contratoInfo.cantKilosString },
            { etiqueta: "Ampliado", valor: contratoInfo.ampliadoString },
            { etiqueta: "Total", valor: contratoInfo.totalString },
            { etiqueta: "Vendedor", valor: contratoInfo.vendedor }
        ]);
        return false;
    };
    ContratoAmpliacionComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-contrato-ampliacion',
            template: __webpack_require__(/*! ./contrato.ampliacion.component.html */ "./src/app/contrato/ampliacion/contrato.ampliacion.component.html"),
            providers: [{ provide: _contrato_service__WEBPACK_IMPORTED_MODULE_2__["ContratoService"], useClass: _contrato_service__WEBPACK_IMPORTED_MODULE_2__["ContratoAmpliacionService"] }]
        })
    ], ContratoAmpliacionComponent);
    return ContratoAmpliacionComponent;
}(_contrato_component__WEBPACK_IMPORTED_MODULE_1__["ContratoBaseComponent"]));



/***/ }),

/***/ "./src/app/contrato/anulacion/contrato.anulacion.component.html":
/*!**********************************************************************!*\
  !*** ./src/app/contrato/anulacion/contrato.anulacion.component.html ***!
  \**********************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"content-wraper\">\r\n    <!-- CONTENT-->\r\n    <section class=\"container-fluid clearfix\">\r\n        <!--Title-->\r\n        <div class=\"row titleSection\">\r\n            <div class=\"col-xs-12 col-sm-10 \">Contrato: <strong>Anulaciones</strong> </div>\r\n            <div class=\"col-xs-12 col-sm-2\">\r\n                <spinner-small></spinner-small>\r\n                <a (click)=\"exportExcel()\" href=\"#\" style=\"text-decoration:none!important\" *ngIf=\"!this.spinnerSmallComponent.visible\">\r\n                    <div class=\"btnSection animate\">Exportar <span class=\"fa  fa-file-excel-o\"></span></div>\r\n                </a>\r\n            </div>\r\n        </div>\r\n        <!--End Title-->\r\n        <!--Search-->\r\n        <div class=\"row  myRow firstSection mySearch\">\r\n            <div class=\"col-xs-12 col-md-12 rounded shadow animate\">\r\n                <title-caps>Refinar resultados</title-caps>\r\n                <hr />\r\n                <div id=\"searchVer\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Ver:\r\n                    <filtro-fecha (ClickEvent)=\"getData()\"></filtro-fecha>\r\n                </div>\r\n                <div id=\"searchVendedor\" class=\"col-xs-12  col-sm-6 col-md-15\"> <!--*ngIf=\"isCorredor()\"-->\r\n                    Por Vendedor:\r\n                    <div class=\"styled-select \">\r\n                        <dropdown [tipoDropdown]=\"'filtroVariable'\" [options]=\"filtroVendedor\" (select)=\"setFiltroVendedor($event)\"></dropdown>\r\n                    </div>\r\n                </div>\r\n                <div id=\"searchProducto\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Por Producto:\r\n                    <div class=\"styled-select \">\r\n                        <dropdown [tipoDropdown]=\"'filtroVariable'\" [options]=\"filtroProducto\" (select)=\"setFiltroProducto($event)\"></dropdown>\r\n                    </div>\r\n                </div>\r\n                <div id=\"searchContrato\" class=\"col-xs-12 col-sm-6 col-md-15 \">\r\n                    Por Contrato:\r\n                    <div class=\"styled-select \">\r\n                        <input type=\"search\" class=\"\" placeholder=\"\" [value]=\"filtroContrato\" (input)=\"filtroContrato = $event.target.value\" aria-controls=\"tableModal\">\r\n                    </div>\r\n                </div>\r\n                <div id=\"searchMostrar\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Mostrar:\r\n                    <div class=\"styled-select \">\r\n                        <dropdown [tipoDropdown]=\"'numberItems'\" (select)=\"setItemsPerPage($event)\"></dropdown>\r\n                    </div>\r\n                </div>\r\n                <div id=\"showSearch\" onClick=\"openSearch()\">\r\n                    <div style=\"\">\r\n                        <div class=\"fa fa-angle-double-right\" aria-hidden=\"true\"></div>\r\n                    </div>\r\n                    <div>VER MÁS OPCIONES DE BÚSQUEDA</div>\r\n                </div>\r\n                <div id=\"hideSearch\" onClick=\"closeSearch()\">\r\n                    <div style=\"\">\r\n                        <div class=\"fa fa-times\" aria-hidden=\"true\"></div>\r\n                    </div>\r\n                    <div>OCULTAR OPCIONES DE BÚSQUEDA</div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n        <!-- End Search-->\r\n        <!-- Tables-->\r\n\r\n        <div class=\"row myRow\">\r\n            <div class=\"clearfix myTables\">\r\n\r\n                <mensaje></mensaje>\r\n                <spinner></spinner>\r\n\r\n                <table *ngIf=\"isVisible()\" id=\"tableModal\" class=\"display nowrap dataTable no-footer dtr-inline rounded shadow animate contrato-anulaciones-table\" cellspacing=\"0\" role=\"grid\" aria-describedby=\"tableModal_info\" style=\"width: 100%; padding:10px 0;\">\r\n                    <thead>\r\n                        <tr role=\"row\">\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('fechaDate')\">Fecha de anulaci&oacute;n</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('nroContrato')\">Contrato Molinos</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('contrvend')\">Contrato Proveedor</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('material')\">Producto</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('cantKilos')\">Pactado</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('anulado')\">Anulado</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('total')\">Total</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('vendedor')\" *ngIf=\"isCorredor()\">Vendedor</th>\r\n                        </tr>\r\n                    </thead>\r\n                    <tbody>\r\n                        <tr *ngFor=\"let contratoInfo of data.contratosInfo | customFilter : 'material' : productoSelected | customFilter : 'vendedor' : vendedorSelected | customFilter : 'nroContrato' : filtroContrato | orderedColumn : {property: orderedByColumn, direction: orderDirection} | paginate: { itemsPerPage:itemsPerPage, currentPage: p } ; let odd = odd\" role=\"row\" [class.odd]=\"!odd\">\r\n                            <td><a (click)=\"showModalTableResponsive(contratoInfo)\" class=\"btnShowModal\"><i class=\"fa fa-plus fa-lg\" aria-hidden=\"true\"></i></a> {{contratoInfo.fecha}}</td>\r\n                            <td><a class=\"animate\" (click)=\"goToSeccionParam('/contrato/detalle', contratoInfo.nroContrato)\" href=\"#\">{{contratoInfo.nroContrato}}</a></td>\r\n                            <td>{{contratoInfo.contrvend}}</td>\r\n                            <td>{{contratoInfo.material}}</td>\r\n                            <td>{{contratoInfo.cantKilosString}}</td>\r\n                            <td>{{contratoInfo.anuladoString}}</td>\r\n                            <td>{{contratoInfo.totalString}}</td>\r\n                            <td *ngIf=\"isCorredor()\"><a (click)=\"goToSeccionParamDos('/dato-fiscal/situacion-fiscal', contratoInfo.idVendedor, contratoInfo.vendedor)\" href=\"#\" class=\"animate\">{{contratoInfo.vendedor}}</a></td>\r\n                        </tr>\r\n                    </tbody>\r\n                </table>\r\n\r\n                <pagination-template #pT=\"paginationApi\" (pageChange)=\"p = $event\" *ngIf=\"isVisible()\">\r\n                    <div class=\"dataTables_paginate paging_simple_numbers\">\r\n                        <a id=\"tableModal_previous\" class=\"paginate_button previous\" (click)=\"pT.previous()\" [class.disabled]=\"pT.isFirstPage()\"> Anterior </a>\r\n                        <span *ngFor=\"let page of pT.pages\" [class.current]=\"pT.getCurrent() === page.value\">\r\n                            <a class=\"paginate_button\" (click)=\"pT.setCurrent(page.value)\" *ngIf=\"pT.getCurrent() !== page.value\">{{ page.label }}</a>\r\n                            <a class=\"paginate_button current\" *ngIf=\"pT.getCurrent() === page.value\">{{ page.label }}</a>\r\n                        </span>\r\n                        <a id=\"tableModal_next\" class=\"paginate_button next\" (click)=\"pT.next()\" [class.disabled]=\"pT.isLastPage()\"> Siguiente </a>\r\n                    </div>\r\n                </pagination-template>\r\n            </div>\r\n        </div>\r\n        <!-- END tables-->\r\n    </section>\r\n    <!--End CONTENT-->\r\n</div>\r\n"

/***/ }),

/***/ "./src/app/contrato/anulacion/contrato.anulacion.component.ts":
/*!********************************************************************!*\
  !*** ./src/app/contrato/anulacion/contrato.anulacion.component.ts ***!
  \********************************************************************/
/*! exports provided: ContratoAnulacionComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContratoAnulacionComponent", function() { return ContratoAnulacionComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _contrato_component__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./../contrato.component */ "./src/app/contrato/contrato.component.ts");
/* harmony import */ var _contrato_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./../contrato.service */ "./src/app/contrato/contrato.service.ts");
var __extends = (undefined && undefined.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};



var ContratoAnulacionComponent = /** @class */ (function (_super) {
    __extends(ContratoAnulacionComponent, _super);
    function ContratoAnulacionComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.tituloArchivo = "ReporteContratosAnulaciones.xls";
        return _this;
    }
    ContratoAnulacionComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("contrato", "Anulaciones");
    };
    ContratoAnulacionComponent.prototype.showModalTableResponsive = function (contratoInfo) {
        this.modalService.openModalTableResponsive("Contrato", [
            { etiqueta: "Fecha de Anulación", valor: contratoInfo.fecha },
            { etiqueta: "Contrato Molinos", valor: contratoInfo.nroContrato },
            { etiqueta: "Contrato Proveedor", valor: contratoInfo.contrvend },
            { etiqueta: "Producto", valor: contratoInfo.material },
            { etiqueta: "Pactado", valor: contratoInfo.cantKilosString },
            { etiqueta: "Anulado", valor: contratoInfo.anuladoString },
            { etiqueta: "Total", valor: contratoInfo.totalString },
            { etiqueta: "Vendedor", valor: contratoInfo.vendedor }
        ]);
        return false;
    };
    ContratoAnulacionComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-contrato-anulacion',
            template: __webpack_require__(/*! ./contrato.anulacion.component.html */ "./src/app/contrato/anulacion/contrato.anulacion.component.html"),
            providers: [{ provide: _contrato_service__WEBPACK_IMPORTED_MODULE_2__["ContratoService"], useClass: _contrato_service__WEBPACK_IMPORTED_MODULE_2__["ContratoAnulacionService"] }]
        })
    ], ContratoAnulacionComponent);
    return ContratoAnulacionComponent;
}(_contrato_component__WEBPACK_IMPORTED_MODULE_1__["ContratoBaseComponent"]));



/***/ }),

/***/ "./src/app/contrato/contrato-routing.module.ts":
/*!*****************************************************!*\
  !*** ./src/app/contrato/contrato-routing.module.ts ***!
  \*****************************************************/
/*! exports provided: ContratoRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContratoRoutingModule", function() { return ContratoRoutingModule; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/router */ "./node_modules/@angular/router/fesm5/router.js");
/* harmony import */ var _vigente_contrato_vigente_component__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./vigente/contrato.vigente.component */ "./src/app/contrato/vigente/contrato.vigente.component.ts");
/* harmony import */ var _ampliacion_contrato_ampliacion_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./ampliacion/contrato.ampliacion.component */ "./src/app/contrato/ampliacion/contrato.ampliacion.component.ts");
/* harmony import */ var _fijacion_contrato_fijacion_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./fijacion/contrato.fijacion.component */ "./src/app/contrato/fijacion/contrato.fijacion.component.ts");
/* harmony import */ var _anulacion_contrato_anulacion_component__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./anulacion/contrato.anulacion.component */ "./src/app/contrato/anulacion/contrato.anulacion.component.ts");
/* harmony import */ var _detalle_contrato_detalle_component__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./detalle/contrato.detalle.component */ "./src/app/contrato/detalle/contrato.detalle.component.ts");
/* harmony import */ var _detalle_fijacion_contrato_detalle_fijacion_component__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./detalle-fijacion/contrato.detalle-fijacion.component */ "./src/app/contrato/detalle-fijacion/contrato.detalle-fijacion.component.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};








var routes = [
    { path: '', component: _vigente_contrato_vigente_component__WEBPACK_IMPORTED_MODULE_2__["ContratoVigenteComponent"] },
    { path: 'vigente', component: _vigente_contrato_vigente_component__WEBPACK_IMPORTED_MODULE_2__["ContratoVigenteComponent"] },
    { path: "fijacion", component: _fijacion_contrato_fijacion_component__WEBPACK_IMPORTED_MODULE_4__["ContratoFijacionComponent"] },
    { path: "ampliacion", component: _ampliacion_contrato_ampliacion_component__WEBPACK_IMPORTED_MODULE_3__["ContratoAmpliacionComponent"] },
    { path: "anulacion", component: _anulacion_contrato_anulacion_component__WEBPACK_IMPORTED_MODULE_5__["ContratoAnulacionComponent"] },
    { path: "detalle/:id", component: _detalle_contrato_detalle_component__WEBPACK_IMPORTED_MODULE_6__["ContratoDetalleComponent"] },
    { path: "detalle-fijacion/:id/:id2", component: _detalle_fijacion_contrato_detalle_fijacion_component__WEBPACK_IMPORTED_MODULE_7__["ContratoDetalleFijacionComponent"] },
];
var ContratoRoutingModule = /** @class */ (function () {
    function ContratoRoutingModule() {
    }
    ContratoRoutingModule = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["NgModule"])({
            imports: [_angular_router__WEBPACK_IMPORTED_MODULE_1__["RouterModule"].forChild(routes)],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_1__["RouterModule"]]
        })
    ], ContratoRoutingModule);
    return ContratoRoutingModule;
}());



/***/ }),

/***/ "./src/app/contrato/contrato.component.ts":
/*!************************************************!*\
  !*** ./src/app/contrato/contrato.component.ts ***!
  \************************************************/
/*! exports provided: ContratoBaseComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContratoBaseComponent", function() { return ContratoBaseComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _contrato_service__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./contrato.service */ "./src/app/contrato/contrato.service.ts");
/* harmony import */ var _common_base_components_list_base_component__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./../common/base-components/list-base-component */ "./src/app/common/base-components/list-base-component.ts");
/* harmony import */ var _common_services_SessionDataService__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./../common/services/SessionDataService */ "./src/app/common/services/SessionDataService.ts");
/* harmony import */ var _common_services_SecurityService__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./../common/services/SecurityService */ "./src/app/common/services/SecurityService.ts");
/* harmony import */ var _common_services_NavService__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./../common/services/NavService */ "./src/app/common/services/NavService.ts");
/* harmony import */ var _common_services_FloatMsgService__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./../common/services/FloatMsgService */ "./src/app/common/services/FloatMsgService.ts");
/* harmony import */ var _common_services_ModalService__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./../common/services/ModalService */ "./src/app/common/services/ModalService.ts");
/* harmony import */ var _common_models_Seccion__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./../common/models/Seccion */ "./src/app/common/models/Seccion.ts");
var __extends = (undefined && undefined.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};









var ContratoBaseComponent = /** @class */ (function (_super) {
    __extends(ContratoBaseComponent, _super);
    function ContratoBaseComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.filtroProducto = null;
        _this.filtroVendedor = null;
        _this.filtroContrato = "";
        _this.productoSelected = "";
        _this.vendedorSelected = "";
        return _this;
    }
    ContratoBaseComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CONSULTAR CONTRATOS"); };
    ContratoBaseComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new _common_models_Seccion__WEBPACK_IMPORTED_MODULE_8__["Seccion"]('/contrato/vigente', 'contrato', 'Vigentes'), new _common_models_Seccion__WEBPACK_IMPORTED_MODULE_8__["Seccion"]('/contrato/fijacion', 'contrato', 'Fijaciones'), new _common_models_Seccion__WEBPACK_IMPORTED_MODULE_8__["Seccion"]('/contrato/ampliacion', 'contrato', 'Ampliaciones'), new _common_models_Seccion__WEBPACK_IMPORTED_MODULE_8__["Seccion"]('/contrato/anulacion', 'contrato', 'Anulaciones')]);
        this.getData();
    };
    ContratoBaseComponent.prototype.setFiltroProducto = function (producto) {
        this.productoSelected = producto;
    };
    ContratoBaseComponent.prototype.setFiltroVendedor = function (vendedor) {
        this.vendedorSelected = vendedor;
    };
    ContratoBaseComponent.prototype.isVisible = function () {
        if (this.data && this.data.contratosInfo.length != 0)
            return true;
        else
            return false;
    };
    ContratoBaseComponent.prototype.vaciarFiltros = function () {
        this.filtroProducto = null;
        this.filtroVendedor = null;
        this.filtroContrato = "";
        this.productoSelected = "";
        this.vendedorSelected = "";
    };
    ContratoBaseComponent.prototype.cargarFiltrosVariables = function (result) {
        if (result.filtroProducto != undefined)
            this.filtroProducto = result.filtroProducto.options;
        if (result.filtroVendedor != undefined)
            this.filtroVendedor = result.filtroVendedor.options;
    };
    ContratoBaseComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-contrato',
            template: "",
            providers: [_contrato_service__WEBPACK_IMPORTED_MODULE_1__["ContratoService"]]
        }),
        __metadata("design:paramtypes", [_contrato_service__WEBPACK_IMPORTED_MODULE_1__["ContratoService"], _common_services_NavService__WEBPACK_IMPORTED_MODULE_5__["NavService"], _common_services_SessionDataService__WEBPACK_IMPORTED_MODULE_3__["SessionDataService"], _common_services_SecurityService__WEBPACK_IMPORTED_MODULE_4__["SecurityService"], _common_services_FloatMsgService__WEBPACK_IMPORTED_MODULE_6__["FloatMsgService"], _common_services_ModalService__WEBPACK_IMPORTED_MODULE_7__["ModalService"]])
    ], ContratoBaseComponent);
    return ContratoBaseComponent;
}(_common_base_components_list_base_component__WEBPACK_IMPORTED_MODULE_2__["ListBaseComponent"]));



/***/ }),

/***/ "./src/app/contrato/contrato.module.ts":
/*!*********************************************!*\
  !*** ./src/app/contrato/contrato.module.ts ***!
  \*********************************************/
/*! exports provided: ContratoModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContratoModule", function() { return ContratoModule; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/common */ "./node_modules/@angular/common/fesm5/common.js");
/* harmony import */ var _contrato_routing_module__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./contrato-routing.module */ "./src/app/contrato/contrato-routing.module.ts");
/* harmony import */ var _contrato_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./contrato.component */ "./src/app/contrato/contrato.component.ts");
/* harmony import */ var _ampliacion_contrato_ampliacion_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./ampliacion/contrato.ampliacion.component */ "./src/app/contrato/ampliacion/contrato.ampliacion.component.ts");
/* harmony import */ var _anulacion_contrato_anulacion_component__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./anulacion/contrato.anulacion.component */ "./src/app/contrato/anulacion/contrato.anulacion.component.ts");
/* harmony import */ var _fijacion_contrato_fijacion_component__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./fijacion/contrato.fijacion.component */ "./src/app/contrato/fijacion/contrato.fijacion.component.ts");
/* harmony import */ var _vigente_contrato_vigente_component__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./vigente/contrato.vigente.component */ "./src/app/contrato/vigente/contrato.vigente.component.ts");
/* harmony import */ var _detalle_contrato_detalle_component__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./detalle/contrato.detalle.component */ "./src/app/contrato/detalle/contrato.detalle.component.ts");
/* harmony import */ var _detalle_fijacion_contrato_detalle_fijacion_component__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./detalle-fijacion/contrato.detalle-fijacion.component */ "./src/app/contrato/detalle-fijacion/contrato.detalle-fijacion.component.ts");
/* harmony import */ var _contrato_service__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./contrato.service */ "./src/app/contrato/contrato.service.ts");
/* harmony import */ var _common_shared_module__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ../common/shared.module */ "./src/app/common/shared.module.ts");
/* harmony import */ var ngx_pagination__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ngx-pagination */ "./node_modules/ngx-pagination/dist/ngx-pagination.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};













var ContratoModule = /** @class */ (function () {
    function ContratoModule() {
    }
    ContratoModule = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["NgModule"])({
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_1__["CommonModule"],
                _contrato_routing_module__WEBPACK_IMPORTED_MODULE_2__["ContratoRoutingModule"],
                _common_shared_module__WEBPACK_IMPORTED_MODULE_11__["SharedModule"],
                ngx_pagination__WEBPACK_IMPORTED_MODULE_12__["NgxPaginationModule"]
            ],
            declarations: [
                _contrato_component__WEBPACK_IMPORTED_MODULE_3__["ContratoBaseComponent"],
                _ampliacion_contrato_ampliacion_component__WEBPACK_IMPORTED_MODULE_4__["ContratoAmpliacionComponent"],
                _anulacion_contrato_anulacion_component__WEBPACK_IMPORTED_MODULE_5__["ContratoAnulacionComponent"],
                _fijacion_contrato_fijacion_component__WEBPACK_IMPORTED_MODULE_6__["ContratoFijacionComponent"],
                _vigente_contrato_vigente_component__WEBPACK_IMPORTED_MODULE_7__["ContratoVigenteComponent"],
                _detalle_contrato_detalle_component__WEBPACK_IMPORTED_MODULE_8__["ContratoDetalleComponent"],
                _detalle_fijacion_contrato_detalle_fijacion_component__WEBPACK_IMPORTED_MODULE_9__["ContratoDetalleFijacionComponent"]
            ],
            providers: [
                _contrato_service__WEBPACK_IMPORTED_MODULE_10__["ContratoService"]
            ],
            schemas: [_angular_core__WEBPACK_IMPORTED_MODULE_0__["CUSTOM_ELEMENTS_SCHEMA"]],
        })
    ], ContratoModule);
    return ContratoModule;
}());



/***/ }),

/***/ "./src/app/contrato/contrato.service.ts":
/*!**********************************************!*\
  !*** ./src/app/contrato/contrato.service.ts ***!
  \**********************************************/
/*! exports provided: ContratoService, ContratoVigenteService, ContratoFijacionService, ContratoAnulacionService, ContratoAmpliacionService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContratoService", function() { return ContratoService; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContratoVigenteService", function() { return ContratoVigenteService; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContratoFijacionService", function() { return ContratoFijacionService; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContratoAnulacionService", function() { return ContratoAnulacionService; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContratoAmpliacionService", function() { return ContratoAmpliacionService; });
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_http__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/http */ "./node_modules/@angular/http/fesm5/http.js");
/* harmony import */ var _common_services_BaseService__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./../common/services/BaseService */ "./src/app/common/services/BaseService.ts");
var __extends = (undefined && undefined.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};





var ContratoService = /** @class */ (function (_super) {
    __extends(ContratoService, _super);
    function ContratoService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ContratoService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return null;
    };
    ContratoService.prototype.getDetalle = function (numero_contrato) {
        var params = new _angular_http__WEBPACK_IMPORTED_MODULE_3__["URLSearchParams"]();
        params.set('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/getDetalle', { search: params, headers: this.headers }).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(this.extractData));
    };
    ContratoService.prototype.getDetalleFijacion = function (numero_contrato, fijacion) {
        var params = new _angular_http__WEBPACK_IMPORTED_MODULE_3__["URLSearchParams"]();
        params.set('numeroContrato', numero_contrato);
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/contrato/getDetalleFijacion', { search: params, headers: this.headers }).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(this.extractData));
    };
    ContratoService.prototype.downloadBoletoFisico = function (numero_contrato) {
        var params = new _angular_http__WEBPACK_IMPORTED_MODULE_3__["URLSearchParams"]();
        params.set('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/downloadBoletoFisico', { search: params, headers: this.headers }).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(this.extractData));
    };
    ContratoService.prototype.exportExcelDetalle = function (numero_contrato) {
        var params = new _angular_http__WEBPACK_IMPORTED_MODULE_3__["URLSearchParams"]();
        params.set('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/downloadDetalle', { search: params, headers: this.headers }).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(this.extractData));
    };
    ContratoService.prototype.exportPDFCalidad = function (numero_contrato) {
        var params = new _angular_http__WEBPACK_IMPORTED_MODULE_3__["URLSearchParams"]();
        params.set('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/exportPDFCalidad', { search: params, headers: this.headers }).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(this.extractData));
    };
    ContratoService.prototype.exportExcelDetalleFijacion = function (numero_contrato, fijacion) {
        var params = new _angular_http__WEBPACK_IMPORTED_MODULE_3__["URLSearchParams"]();
        params.set('numeroContrato', numero_contrato);
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/contrato/downloadDetalleFijacion', { search: params, headers: this.headers }).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(this.extractData));
    };
    ContratoService.prototype.getContratosCommon = function (periodo, fecha_inicio, fecha_fin, method) {
        var params = new _angular_http__WEBPACK_IMPORTED_MODULE_3__["URLSearchParams"]();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/contrato/' + method, { search: params, headers: this.headers }).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(this.extractData));
    };
    ContratoService.prototype.getContratosNoCumplidosCommon = function (periodo, fecha_inicio, fecha_fin, method, contratos) {
        var params = new _angular_http__WEBPACK_IMPORTED_MODULE_3__["URLSearchParams"]();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/contrato/' + method, { search: params, headers: this.headers }).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(this.extractData));
    };
    ContratoService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return null;
    };
    ContratoService.prototype.exportExcelCommon = function (periodo, fecha_inicio, fecha_fin, method) {
        var params = new _angular_http__WEBPACK_IMPORTED_MODULE_3__["URLSearchParams"]();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/contrato/' + method, { search: params, headers: this.headers }).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["timeoutWith"])(30000, Object(rxjs__WEBPACK_IMPORTED_MODULE_0__["throwError"])(new Error("Por favor, restrinja el rango de fechas"))), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(this.extractData));
    };
    ContratoService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_2__["Injectable"])()
    ], ContratoService);
    return ContratoService;
}(_common_services_BaseService__WEBPACK_IMPORTED_MODULE_4__["BaseService"]));

var ContratoVigenteService = /** @class */ (function (_super) {
    __extends(ContratoVigenteService, _super);
    function ContratoVigenteService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ContratoVigenteService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getContratosCommon(periodo, fecha_inicio, fecha_fin, 'getVigentes');
    };
    ContratoVigenteService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadVigentes');
    };
    ContratoVigenteService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_2__["Injectable"])()
    ], ContratoVigenteService);
    return ContratoVigenteService;
}(ContratoService));

var ContratoFijacionService = /** @class */ (function (_super) {
    __extends(ContratoFijacionService, _super);
    function ContratoFijacionService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ContratoFijacionService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getContratosNoCumplidosCommon(periodo, fecha_inicio, fecha_fin, 'getFijaciones', new Array());
    };
    ContratoFijacionService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadFijaciones');
    };
    ContratoFijacionService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_2__["Injectable"])()
    ], ContratoFijacionService);
    return ContratoFijacionService;
}(ContratoService));

var ContratoAnulacionService = /** @class */ (function (_super) {
    __extends(ContratoAnulacionService, _super);
    function ContratoAnulacionService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ContratoAnulacionService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getContratosNoCumplidosCommon(periodo, fecha_inicio, fecha_fin, 'getAnulaciones', new Array());
    };
    ContratoAnulacionService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadAnulaciones');
    };
    ContratoAnulacionService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_2__["Injectable"])()
    ], ContratoAnulacionService);
    return ContratoAnulacionService;
}(ContratoService));

var ContratoAmpliacionService = /** @class */ (function (_super) {
    __extends(ContratoAmpliacionService, _super);
    function ContratoAmpliacionService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ContratoAmpliacionService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getContratosNoCumplidosCommon(periodo, fecha_inicio, fecha_fin, 'getAmpliaciones', new Array());
    };
    ContratoAmpliacionService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadAmpliaciones');
    };
    ContratoAmpliacionService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_2__["Injectable"])()
    ], ContratoAmpliacionService);
    return ContratoAmpliacionService;
}(ContratoService));



/***/ }),

/***/ "./src/app/contrato/detalle-fijacion/contrato.detalle-fijacion.component.html":
/*!************************************************************************************!*\
  !*** ./src/app/contrato/detalle-fijacion/contrato.detalle-fijacion.component.html ***!
  \************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"content-wraper\">\r\n    <mensaje></mensaje>\r\n    <spinner></spinner>\r\n    <!-- CONTENT-->\r\n    <section class=\"container-fluid clearfix\" *ngIf=\"data\">\r\n        <!--Title-->\r\n        <div class=\"row titleSection titleSectionTable\">\r\n            <div class=\"col-xs-12 col-sm-10 fixtwobuttons titleSectionTablePart animate\" style=\"vertical-align:bottom;\">\r\n                <div style=\"line-height:0.8; display: inline-block; vertical-align:top;\">Detalle Fijaci&oacute;n <strong><span>N&ordm; {{data.fijacion}}</span></strong></div>\r\n                <div class=\"titleExtraData\">\r\n                    <i class=\"fa fa-arrow-circle-right\" aria-hidden=\"true\"></i> Monto: <strong>{{data.total}}</strong><br>\r\n                </div>\r\n            </div>\r\n            <div class=\"col-xs-12 col-sm-2 titleSectionTablePart\">\r\n                <spinner-small #spinnerSmallExport></spinner-small>\r\n                <a (click)=\"exportarExcel()\" href=\"#\" style=\"text-decoration:none!important\" *ngIf=\"!this.spinnerSmallExportComponent.visible\">\r\n                    <div class=\"btnSection animate export\">Exportar <span class=\"fa  fa-file-excel-o\"></span></div>\r\n                </a>\r\n                <!--<a href=\"#\" style=\"text-decoration:none!important\">\r\n                    <div class=\"btnSection animate print\">Imprimir <span class=\"fa  fa-print\"></span></div>\r\n                </a>-->\r\n            </div>\r\n        </div>\r\n        <!--End Title-->\r\n        <!--Search-->\r\n        <div id=\"myPagos\" class=\"row myRow hideMobile\">\r\n            <div class=\"clearfix myTables\">\r\n                <table id=\"tableModal\" class=\"display nowrap dataTable no-footer dtr-inline rounded shadow animate boletosTable \" cellspacing=\"0\" role=\"grid\" aria-describedby=\"tableModal_info\" style=\"width: 100%; padding:10px 0;\">\r\n                    <thead>\r\n                        <tr role=\"row\">\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Fecha</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">N&ordm; CCPP</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Kilos Fijados</th>\r\n                        </tr>\r\n                    </thead>\r\n                    <tbody>\r\n                        <tr *ngFor=\"let item of data.detalleFijacion\" role=\"row\" class=\"odd\">\r\n                            <td tabindex=\"0\">{{item.fechaDescarga}}</td>\r\n                            <td><a class=\"animate\" (click)=\"goToSeccionParam('/carta-porte/detalle', item.cartaPorte)\" href=\"#\">{{item.cartaPorte}}</a></td>\r\n                            <td style=\"\">{{item.netoDescontado}}</td>\r\n                        </tr>\r\n                    </tbody>\r\n                </table>\r\n            </div>\r\n        </div>\r\n        <!--Fin botones mobile-->\r\n    </section>\r\n    <!--End CONTENT-->\r\n</div>\r\n\r\n\r\n"

/***/ }),

/***/ "./src/app/contrato/detalle-fijacion/contrato.detalle-fijacion.component.ts":
/*!**********************************************************************************!*\
  !*** ./src/app/contrato/detalle-fijacion/contrato.detalle-fijacion.component.ts ***!
  \**********************************************************************************/
/*! exports provided: ContratoDetalleFijacionComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContratoDetalleFijacionComponent", function() { return ContratoDetalleFijacionComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/router */ "./node_modules/@angular/router/fesm5/router.js");
/* harmony import */ var _contrato_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./../contrato.service */ "./src/app/contrato/contrato.service.ts");
/* harmony import */ var _common_view_child_mensaje_mensaje_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./../../common/view-child/mensaje/mensaje.component */ "./src/app/common/view-child/mensaje/mensaje.component.ts");
/* harmony import */ var _common_view_child_spinner_spinner_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./../../common/view-child/spinner/spinner.component */ "./src/app/common/view-child/spinner/spinner.component.ts");
/* harmony import */ var _common_view_child_spinner_small_spinner_small_component__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./../../common/view-child/spinner-small/spinner-small.component */ "./src/app/common/view-child/spinner-small/spinner-small.component.ts");
/* harmony import */ var _common_services_SecurityService__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./../../common/services/SecurityService */ "./src/app/common/services/SecurityService.ts");
/* harmony import */ var _common_services_NavService__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./../../common/services/NavService */ "./src/app/common/services/NavService.ts");
/* harmony import */ var _common_services_FloatMsgService__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./../../common/services/FloatMsgService */ "./src/app/common/services/FloatMsgService.ts");
/* harmony import */ var _common_base_components_base_component__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./../../common/base-components/base-component */ "./src/app/common/base-components/base-component.ts");
/* harmony import */ var _common_services_SessionDataService__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./../../common/services/SessionDataService */ "./src/app/common/services/SessionDataService.ts");
/* harmony import */ var _common_models_Seccion__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ./../../common/models/Seccion */ "./src/app/common/models/Seccion.ts");
/* harmony import */ var _common_services_ModalService__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ./../../common/services/ModalService */ "./src/app/common/services/ModalService.ts");
var __extends = (undefined && undefined.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};













var ContratoDetalleFijacionComponent = /** @class */ (function (_super) {
    __extends(ContratoDetalleFijacionComponent, _super);
    function ContratoDetalleFijacionComponent(route, router, service, navService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.route = route;
        _this.router = router;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.data = null;
        _this.tituloArchivoExcel = "ReporteFijacionDetalle.xls";
        _this.numeroContratoId = "";
        _this.fijacion = "";
        _this.mensajeComponent = new _common_view_child_mensaje_mensaje_component__WEBPACK_IMPORTED_MODULE_3__["MensajeComponent"]();
        _this.spinnerComponent = new _common_view_child_spinner_spinner_component__WEBPACK_IMPORTED_MODULE_4__["SpinnerComponent"]();
        return _this;
    }
    ContratoDetalleFijacionComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("contrato", "Detalle Fijaciones");
    };
    ContratoDetalleFijacionComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR CONTRATO DETALLE");
        this.navService.setSeccionList([new _common_models_Seccion__WEBPACK_IMPORTED_MODULE_11__["Seccion"]('/contrato/vigente', 'contrato', 'Vigentes'), new _common_models_Seccion__WEBPACK_IMPORTED_MODULE_11__["Seccion"]('/contrato/fijacion', 'contrato', 'Fijaciones'), new _common_models_Seccion__WEBPACK_IMPORTED_MODULE_11__["Seccion"]('/contrato/ampliacion', 'contrato', 'Ampliaciones'), new _common_models_Seccion__WEBPACK_IMPORTED_MODULE_11__["Seccion"]('/contrato/anulacion', 'contrato', 'Anulaciones')]);
        this.navService.setSeccionActive("Detalle");
        this.getData();
    };
    ContratoDetalleFijacionComponent.prototype.ngAfterViewInit = function () {
        this.spinnerSmallExportComponent = new _common_view_child_spinner_small_spinner_small_component__WEBPACK_IMPORTED_MODULE_5__["SpinnerSmallComponent"]();
        this.spinnerSmallPDFComponent = new _common_view_child_spinner_small_spinner_small_component__WEBPACK_IMPORTED_MODULE_5__["SpinnerSmallComponent"]();
    };
    ContratoDetalleFijacionComponent.prototype.getData = function () {
        var _this = this;
        this.data = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.forEach(function (params) {
            _this.numeroContratoId = params['id'];
            _this.fijacion = params['id2'];
            _this.unsubscribe();
            _this.subscription = _this.service.getDetalleFijacion(_this.numeroContratoId, _this.fijacion).subscribe(function (result) {
                _this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.data = result;
                }
            }, function (error) {
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
            //this.navService.setSeccionActive('Fijaciones');
        });
    };
    ContratoDetalleFijacionComponent.prototype.exportarExcel = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallExportComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcelDetalleFijacion(this.numeroContratoId, this.fijacion).subscribe(function (result) {
            _this.spinnerSmallExportComponent.hideIt();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                var blob = new Blob([result], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, _this.tituloArchivoExcel);
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = _this.tituloArchivoExcel;
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.spinnerSmallExportComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    ContratoDetalleFijacionComponent.prototype.showDataPlus = function (registro) {
        return this.tieneData(registro.kgNetos) || this.tieneData(registro.kgDto) || this.tieneData(registro.kgApli) || this.tieneData(registro.dto);
    };
    ContratoDetalleFijacionComponent.prototype.tieneData = function (value) {
        return value != undefined && value != 0 && value != "" && value != "0 KG" && value != "0%";
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])(_common_view_child_mensaje_mensaje_component__WEBPACK_IMPORTED_MODULE_3__["MensajeComponent"]),
        __metadata("design:type", _common_view_child_mensaje_mensaje_component__WEBPACK_IMPORTED_MODULE_3__["MensajeComponent"])
    ], ContratoDetalleFijacionComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])(_common_view_child_spinner_spinner_component__WEBPACK_IMPORTED_MODULE_4__["SpinnerComponent"]),
        __metadata("design:type", _common_view_child_spinner_spinner_component__WEBPACK_IMPORTED_MODULE_4__["SpinnerComponent"])
    ], ContratoDetalleFijacionComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])("spinnerSmallExport"),
        __metadata("design:type", _common_view_child_spinner_small_spinner_small_component__WEBPACK_IMPORTED_MODULE_5__["SpinnerSmallComponent"])
    ], ContratoDetalleFijacionComponent.prototype, "spinnerSmallExportComponent", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])("spinnerSmallPDF"),
        __metadata("design:type", _common_view_child_spinner_small_spinner_small_component__WEBPACK_IMPORTED_MODULE_5__["SpinnerSmallComponent"])
    ], ContratoDetalleFijacionComponent.prototype, "spinnerSmallPDFComponent", void 0);
    ContratoDetalleFijacionComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-contrato-detalle-fijacion',
            template: __webpack_require__(/*! ./contrato.detalle-fijacion.component.html */ "./src/app/contrato/detalle-fijacion/contrato.detalle-fijacion.component.html"),
            providers: [{ provide: _contrato_service__WEBPACK_IMPORTED_MODULE_2__["ContratoService"], useClass: _contrato_service__WEBPACK_IMPORTED_MODULE_2__["ContratoFijacionService"] }]
        }),
        __metadata("design:paramtypes", [_angular_router__WEBPACK_IMPORTED_MODULE_1__["ActivatedRoute"],
            _angular_router__WEBPACK_IMPORTED_MODULE_1__["Router"],
            _contrato_service__WEBPACK_IMPORTED_MODULE_2__["ContratoService"],
            _common_services_NavService__WEBPACK_IMPORTED_MODULE_7__["NavService"],
            _common_services_SecurityService__WEBPACK_IMPORTED_MODULE_6__["SecurityService"],
            _common_services_SessionDataService__WEBPACK_IMPORTED_MODULE_10__["SessionDataService"],
            _common_services_FloatMsgService__WEBPACK_IMPORTED_MODULE_8__["FloatMsgService"],
            _common_services_ModalService__WEBPACK_IMPORTED_MODULE_12__["ModalService"]])
    ], ContratoDetalleFijacionComponent);
    return ContratoDetalleFijacionComponent;
}(_common_base_components_base_component__WEBPACK_IMPORTED_MODULE_9__["BaseComponent"]));



/***/ }),

/***/ "./src/app/contrato/detalle/contrato.detalle.component.html":
/*!******************************************************************!*\
  !*** ./src/app/contrato/detalle/contrato.detalle.component.html ***!
  \******************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"content-wraper\">\r\n    <mensaje></mensaje>\r\n    <spinner></spinner>\r\n    <!-- CONTENT-->\r\n    <section class=\"container-fluid clearfix\" *ngIf=\"isData()\">\r\n        <!--Title-->\r\n        <div class=\"row titleSection titleSectionTable\">\r\n            <div class=\"col-xs-12 col-sm-10 fixtwobuttons titleSectionTablePart animate\" style=\"vertical-align:bottom;\">\r\n                <div style=\"line-height:0.8; display: inline-block; vertical-align:top;\"> Contrato Molinos <strong><span>Nº {{data.contrato}} </span></strong> / Corredor <strong><span>{{data.caracteristicas[0].corredor}}</span></strong> </div>\r\n                <div class=\"titleExtraData\">\r\n                    <i class=\"fa fa-arrow-circle-right\" aria-hidden=\"true\"></i> Corredor: <strong>{{data.caracteristicas[0].nomCorredor}}</strong><br>\r\n                    <i class=\"fa fa-arrow-circle-right\" aria-hidden=\"true\"></i> Vendedor: <strong><a (click)=\"goToSeccionParamDos('/dato-fiscal/situacion-fiscal', data.caracteristicas[0].vendedor, data.caracteristicas[0].nomVendedor)\" href=\"#\">{{data.caracteristicas[0].nomVendedor}}</a> </strong>({{data.caracteristicas[0].vendedor}})\r\n                </div>\r\n            </div>\r\n            <div class=\"col-xs-12 col-sm-2 titleSectionTablePart\">\r\n                <spinner-small #spinnerSmallExport></spinner-small>\r\n                <a (click)=\"exportarExcel()\" href=\"#\" style=\"text-decoration:none!important\" *ngIf=\"!this.spinnerSmallExportComponent.visible\">\r\n                    <div class=\"btnSection animate export\">Exportar <span class=\"fa  fa-file-excel-o\"></span></div>\r\n                </a> \r\n                <!--<a href=\"#\" style=\"text-decoration:none!important\">\r\n                    <div class=\"btnSection animate print\">Imprimir <span class=\"fa  fa-print\"></span></div>\r\n                </a>--> \r\n                <spinner-small #spinnerSmallPDF></spinner-small>\r\n                <a (click)=\"downloadBoletoFisico(data.contrato)\" href=\"#\" style=\"text-decoration:none!important\" *ngIf=\"data.caracteristicas[0].confirma != 'X' && !this.spinnerSmallPDFComponent.visible\">\r\n                    <div class=\"btnSection animate ticket\">Boleto Físico <span class=\"fa  fa-file-pdf-o \"></span></div>\r\n                </a>\r\n            </div>\r\n        </div>\r\n        <!--End Title-->\r\n        <!--Search-->\r\n        <div class=\"row myRow firstSection roundedInfo\">\r\n            <div class=\"col-xs-12 col-sm-3 col-md-2  rowInfoGrey rounded shadow animate rowInfoRoundedWhite\" *ngIf=\"data.resumen\">\r\n                <title-caps>Resumen</title-caps>\r\n                <hr>\r\n                <div class=\"col-xs-6 col-sm-12\">\r\n                    <p>Estado:</p>\r\n                    {{data.resumen[0].estado}}\r\n                </div>\r\n                <div class=\"col-xs-6 col-sm-12\">\r\n                    <p>Producto:</p>\r\n                    {{data.resumen[0].producto}}\r\n                </div>\r\n                <div class=\"col-xs-6 col-sm-12\">\r\n                    <p>Pactado:</p>\r\n                    {{data.caracteristicas[0].cantidadString}}\r\n                </div>\r\n                <div class=\"col-xs-6 col-sm-12\">\r\n                    <p>Precio/Tn:</p>\r\n                    {{data.resumen[0].precioString}}\r\n                </div>\r\n                <div class=\"col-xs-6 col-sm-12\">\r\n                    <p>Entregado:</p>\r\n                    {{data.resumen[0].cantEntreString}}\r\n                </div>\r\n                <div class=\"col-xs-6 col-sm-12\">\r\n                    <p>Liquidado:</p>\r\n                    {{data.resumen[0].cantLiquiString}}\r\n                </div>\r\n                <div class=\"col-xs-6 col-sm-12\">\r\n                    <p>Fijado:</p>\r\n                    {{data.resumen[0].cantFijaString}}\r\n                </div>\r\n                <div class=\"col-xs-6 col-sm-12\">\r\n                    <p>Pendiente de entrega:</p>\r\n                    {{data.resumen[0].cantPendEntreString}}\r\n                </div>\r\n            </div>\r\n            <div class=\"col-xs-12 col-sm-9 col-md-10 squareInfo animate\">\r\n                <div class=\"row myRow\">\r\n                    <div id=\"showCaracteristicas\" class=\"showBtn btnSection btnInfo animate\" onClick=\"showCaracteristicas()\">\r\n                        <div>\r\n                            <li class=\"fa fa-angle-double-right\" aria-hidden=\"true\"></li>\r\n                        </div>\r\n                        <div>Características del contrato</div>\r\n                    </div>\r\n                    <div id=\"hideCaracteristicas\" class=\"hideBtn btnSection btnInfo animate btnInfoClose\" onClick=\"closeCaracteristicas()\">\r\n                        <div>\r\n                            <li class=\"fa fa-times\" aria-hidden=\"true\"></li>\r\n                        </div>\r\n                        <div>Características del contrato</div>\r\n                    </div>\r\n                    <title-caps class=\"hideMobile\">Características del contrato</title-caps>\r\n                    <hr class=\"hideMobile\">\r\n                    <div id=\"caracteristicas\" class=\"rowInfoGrey clearfix hideMobile\" *ngIf=\"data.caracteristicas[0]\">\r\n                        <div class=\"row myRow\">\r\n                            <div class=\"col-xs-12 col-sm-3\">\r\n                                <p>Tipo de contrato:</p>\r\n                                {{data.caracteristicas[0].tipo}}\r\n                            </div>\r\n                            <div class=\"col-xs-12 col-sm-3 \">\r\n                                <p>Cantidad:</p>\r\n                                {{data.caracteristicas[0].cantidadString}}\r\n                            </div>\r\n                            <div class=\"col-xs-12 col-sm-3 \">\r\n                                <p>Calificación Proveedor:</p>\r\n                                {{data.caracteristicas[0].calificacion}}\r\n                            </div>\r\n                            <div class=\"col-xs-12 col-sm-3 \">\r\n                                <p>Tolerancia:</p>\r\n                                {{data.caracteristicas[0].toleMin}}% a {{data.caracteristicas[0].toleMax}}%\r\n                            </div>\r\n                        </div>\r\n                        <div class=\"row myRow\">\r\n                            <div class=\"col-xs-12 col-sm-3 \">\r\n                                <p>Lugar descarga/puerto:</p>\r\n                                {{data.caracteristicas[0].descarga}}\r\n                            </div>\r\n                            <div class=\"col-xs-12 col-sm-3 \">\r\n                                <p>Estado de Boleto:</p>\r\n                                {{data.caracteristicas[0].estadoBol}}\r\n                            </div>\r\n                            <div class=\"col-xs-12 col-sm-3 \">\r\n                                <p>Procedencia:</p>\r\n                                {{data.caracteristicas[0].procedencia}}\r\n                            </div>\r\n                            <div class=\"col-xs-12 col-sm-3 \">\r\n                                <p>Período de entrega:</p>\r\n                                {{data.caracteristicas[0].entregaMin}} a {{data.caracteristicas[0].entregaMax}}\r\n                            </div>\r\n                        </div>\r\n                        <div class=\"row myRow\">\r\n                            <div class=\"col-xs-12 col-sm-3 \">\r\n                                <p>Fecha de concertación:</p>\r\n                                {{data.caracteristicas[0].fechaConcerta}}\r\n                            </div>\r\n                            <div class=\"col-xs-12 col-sm-3 \">\r\n                                <p>Standard calidad:</p>\r\n                                {{data.caracteristicas[0].standardCali}}\r\n                            </div>\r\n                            <div class=\"col-xs-12 col-sm-3 \">\r\n                                <p>Cosecha:</p>\r\n                                {{data.caracteristicas[0].cosecha}}\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n                <!--segunda línea de datos-->\r\n                <div class=\"row myRow\">\r\n                    <div class=\"col-xs-12 col-sm-6 rowInfoGreyLeft\">\r\n                        <div id=\"showCondiciones\" class=\"showBtn btnSection btnInfo animate\" onClick=\"showCondiciones()\">\r\n                            <div>\r\n                                <li class=\"fa fa-angle-double-right\" aria-hidden=\"true\"></li>\r\n                            </div>\r\n                            <div>Condiciones comerciales</div>\r\n                        </div>\r\n                        <div id=\"hideCondiciones\" class=\"hideBtn btnSection btnInfo animate btnInfoClose\" onClick=\"closeCondiciones()\">\r\n                            <div>\r\n                                <li class=\"fa fa-times\" aria-hidden=\"true\"></li>\r\n                            </div>\r\n                            <div>Condiciones comerciales</div>\r\n                        </div>\r\n                        <title-caps class=\"hideMobile\">Condiciones comerciales</title-caps>\r\n                        <hr class=\"hideMobile\">\r\n                        <div id=\"condiciones\" class=\"rowInfoGrey clearfix hideMobile\" *ngIf=\"data.caracteristicas[0]\">\r\n                            <div class=\"row myRow\">\r\n                                <div class=\"col-xs-12 col-sm-6\">\r\n                                    <p>Pizarra de referencia:</p>\r\n                                    {{data.caracteristicas[0].pizarraRef}}\r\n                                </div>\r\n                                <div class=\"col-xs-12 col-sm-6 \">\r\n                                    <p>Fijación diaria:</p>\r\n                                    {{data.caracteristicas[0].fijaDiariaMin}}kg a {{data.caracteristicas[0].fijaDiariaMax}}kg\r\n                                </div>\r\n                            </div>\r\n                            <div class=\"row myRow\">\r\n                                <div class=\"col-xs-12 col-sm-6\">\r\n                                    <p>Pago parcial:</p>\r\n                                    {{data.caracteristicas[0].pagoParcial}}%\r\n                                </div>\r\n                                <div class=\"col-xs-12 col-sm-6 \">\r\n                                    <p>Descuento por acarreo:</p>\r\n                                    ${{data.caracteristicas[0].descuentoAcarreo}} / Tn\r\n                                </div>\r\n                            </div>\r\n                            <div class=\"row myRow\">\r\n                                <div class=\"col-xs-12 col-sm-6\">\r\n                                    <p>Condición de pago fijaciones:</p>\r\n                                    {{data.caracteristicas[0].condPagoFija}}\r\n                                </div>\r\n                                <div class=\"col-xs-12 col-sm-6 \">\r\n                                    <p>Condiciones especiales de pago:</p>\r\n                                    <ul>\r\n                                        <li *ngFor=\"let condicion of data.condicionesPago\">\r\n                                            {{condicion}}\r\n                                        </li>\r\n                                    </ul>\r\n                                </div>\r\n                            </div>\r\n                            <div class=\"row myRow\">\r\n                                <div class=\"col-xs-12 col-sm-6\">\r\n                                    <p>Fecha tope de fijación:</p>\r\n                                    {{data.caracteristicas[0].fechaTopeFija}}\r\n                                </div>\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n                    <div class=\"col-xs-12 col-sm-6 rowInfoGreyRight\">\r\n                        <div id=\"showBonificaciones\" class=\"showBtn btnSection btnInfo animate\" onClick=\"showBonificaciones()\">\r\n                            <div>\r\n                                <li class=\"fa fa-angle-double-right\" aria-hidden=\"true\"></li>\r\n                            </div>\r\n                            <div>Bonificaciones</div>\r\n                        </div>\r\n                        <div id=\"hideBonificaciones\" class=\"hideBtn btnSection btnInfo animate btnInfoClose\" onClick=\"closeBonificaciones()\">\r\n                            <div>\r\n                                <li class=\"fa fa-times\" aria-hidden=\"true\"></li>\r\n                            </div>\r\n                            <div>Bonificaciones</div>\r\n                        </div>\r\n                        <title-caps class=\"hideMobile\">Bonificaciones</title-caps>\r\n                        <hr class=\"hideMobile\">\r\n                        <div id=\"bonificaciones\" class=\"rowInfoGrey rowInfoGreyTable clearfix hideMobile \" *ngIf=\"data.caracteristicas[0]\">\r\n                            <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n                                <tr>\r\n                                    <td width=\"50%\">&nbsp;</td>\r\n                                    <td width=\"25%\">&nbsp;</td>\r\n                                    <td width=\"25%\">GENERAL</td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td colspan=\"3\" class=\"titleTable\">DENTRO DEL PRECIO</td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td>&nbsp;</td>\r\n                                    <td>Fijo</td>\r\n                                    <td>{{data.caracteristicas[0].importeAPrecio}}</td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td>&nbsp;</td>\r\n                                    <td>%</td>\r\n                                    <td>{{data.caracteristicas[0].porcAPrecio}}</td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td colspan=\"3\" class=\"titleTable\">FUERA DEL PRECIO</td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td>&nbsp;</td>\r\n                                    <td>Fijo</td>\r\n                                    <td>{{data.caracteristicas[0].importeSPrecio}}</td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td>&nbsp;</td>\r\n                                    <td>%</td>\r\n                                    <td>{{data.caracteristicas[0].porcSPrecio}}</td>\r\n                                </tr>\r\n                            </table>\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n        <!--BOLETOS-->\r\n        <div class=\"row myRow\" style=\" margin-top:15px;margin-bottom: 30px;\">\r\n            <!--boleto destacado-->\r\n            <div class=\"row titleSection titleSectionNoBorder\">\r\n                <div class=\"col-xs-12 col-sm-12\">Boletos</div>\r\n                <div class=\"col-xs-12 rowInfoGrey rounded shadow animate rowInfoRoundedGreen\" style=\"background:#77b794;\">\r\n                    <div class=\"rowInfoRoundedGreenLine\" *ngIf=\"data.boleto\">\r\n                        <div class=\"col-xs-12 col-sm-15\">\r\n                            <p>Estado</p>\r\n                            <p>{{data.boleto.estado}} <span *ngIf=\"data.boleto.observacion != '' && data.boleto.observacion\" class=\"fa  fa-info-circle\" toggle=\"tooltip\" title=\"{{data.boleto.observacion}}\"></span></p>\r\n                        </div>\r\n                        <div class=\"col-xs-12 col-sm-15\">\r\n                            <p>Fecha de recepción</p>\r\n                            <p>{{data.boleto.fechaRecepcion}}</p>\r\n                        </div>\r\n                        <div class=\"col-xs-12 col-sm-15\">\r\n                            <p>Tipo Boleto</p>\r\n                            <p>{{data.boleto.tipoBoleto}}</p>\r\n                        </div>\r\n                        <div class=\"col-xs-12 col-sm-15\">\r\n                            <p>Bolsa</p>\r\n                            <p>{{data.boleto.bolsa}}</p>\r\n                        </div>\r\n                        <div class=\"col-xs-12 col-sm-15\">\r\n                            <p>Devolución</p>\r\n                            <p>{{data.boleto.devAcop}}</p>\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n            <!--Boleto tabla-->\r\n            <div class=\"row myRow\">\r\n                <div class=\"clearfix myTables\">\r\n                    <table id=\"tableModal\" class=\"display nowrap dataTable no-footer dtr-inline rounded shadow animate boletosTable contrato-det-boletos-table\" cellspacing=\"0\" role=\"grid\" aria-describedby=\"tableModal_info\" style=\"width: 100%; padding:10px 0;\">\r\n                        <thead>\r\n                            <tr role=\"row\">\r\n                                <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\" background:none\">Envío bolsa</th>\r\n                                <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\" background:none\">Vuelta Bolsa</th>\r\n                                <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\" background:none\">Oblea Bolsa</th>\r\n                                <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\" background:none\">Envío Afip</th>\r\n                                <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\" background:none\">Vuelta Afip</th>\r\n                                <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\" background:none\">REG. Afip</th>\r\n                                <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\" background:none\">Oblea Prov. Plan Canje</th>\r\n                                <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\" background:none\">Envío Sellado</th>\r\n                                <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\" background:none\">Vuelta Sellado</th>\r\n                            </tr>\r\n                        </thead>\r\n                        <tbody>\r\n                            <tr role=\"row\" class=\"odd\" *ngIf=\"data.boleto.envioBolsa\">\r\n                                <td tabindex=\"0\" style=\"\"><a (click)=\"showModalTableBoletosResponsive(data.boleto)\" href=\"#\" class=\"btnShowModal\"><i class=\"fa fa-plus fa-lg\" aria-hidden=\"true\"></i></a>{{data.boleto.envioBolsa}}</td>\r\n                                <td>{{data.boleto.vueltaBolsa}}</td>\r\n                                <td style=\"\">{{data.boleto.obleaBolsa}}</td>\r\n                                <td style=\"\">{{data.boleto.envioAfip}}</td>\r\n                                <td style=\"\">{{data.boleto.vueltaAfip}}</td>\r\n                                <td style=\"\">{{data.boleto.obleaAfip}}</td>\r\n                                <td style=\"\">{{data.boleto.provPlanCanje}}</td>\r\n                                <td style=\"\">{{data.boleto.envioSellado}}</td>\r\n                                <td style=\"\">{{data.boleto.vueltaSellado}}</td>\r\n                            </tr>\r\n                        </tbody>\r\n                    </table>\r\n                </div>\r\n            </div>\r\n        </div>\r\n\r\n        <!--pagos y liquidaciones-->\r\n        <div class=\"row myRow\">\r\n\r\n            <div id=\"Pagos\" class=\"col-sm-12 col-md-12 col-lg-12\">\r\n\r\n                <div id=\"showPagos\" class=\"showBtn btnSection btnInfo animate\" onClick=\"showPagos()\">\r\n                    <div>\r\n                        <li class=\"fa fa-angle-double-right\" aria-hidden=\"true\"></li>\r\n                    </div>\r\n                    <div>Pagos</div>\r\n                </div>\r\n                <div id=\"hidePagos\" class=\"hideBtn btnSection btnInfo animate btnInfoClose\" onClick=\"closePagos()\">\r\n                    <div>\r\n                        <li class=\"fa fa-times\" aria-hidden=\"true\"></li>\r\n                    </div>\r\n                    <div>Pagos</div>\r\n                </div>\r\n\r\n                <div class=\"row titleSection titleSectionNoBorder hideMobile\">\r\n                    <div class=\"col-xs-12 col-sm-12\">Pagos</div>\r\n                </div>\r\n                <!--pagos y liquidaciones tabla-->\r\n                <div id=\"myPagos\" class=\"row myRow hideMobile\">\r\n                    <div class=\"clearfix myTables\">\r\n                        <table id=\"tableModal\" class=\"display nowrap dataTable no-footer dtr-inline rounded shadow animate boletosTable contrato-det-pagos-table\" cellspacing=\"0\" role=\"grid\" aria-describedby=\"tableModal_info\" style=\"width: 100%; padding:10px 0;\">\r\n                            <thead>\r\n                                <tr role=\"row\">\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Fecha</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">ID pago</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Bruto</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Iva</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Retenciones</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Total</th>\r\n                                </tr>\r\n                            </thead>\r\n                            <tbody>\r\n                                <tr *ngFor=\"let pago of data.pagos\" role=\"row\" class=\"odd\">\r\n                                    <td tabindex=\"0\"><a (click)=\"showModalTablePagosResponsive(pago)\" href=\"#\" class=\"btnShowModal\"><i class=\"fa fa-plus fa-lg\" aria-hidden=\"true\"></i></a>{{pago.fecha}}</td>\r\n                                    <td><a class=\"animate\" (click)=\"goToSeccionParam('/pago/detalle', pago.idPago)\" href=\"#\">{{pago.idPago}}</a></td>\r\n                                    <td style=\"\">{{pago.brutoString}}</td>\r\n                                    <td style=\"\">{{pago.ivaString}}</td>\r\n                                    <td style=\"\"><a (click)=\"goToSeccionParam('/cuenta-corriente/simple', pago.idPago)\" href=\"#\"><span class=\"fa  fa-file-pdf-o \"></span>{{pago.retencionesString}}</a></td>\r\n                                    <td style=\"\">{{pago.netoString}}</td>\r\n                                </tr>\r\n                            </tbody>\r\n                        </table>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n            <div id=\"Liquidaciones\" class=\"col-sm-12 col-md-12 col-lg-12\">\r\n                <div id=\"showLiquidaciones\" class=\"showBtn btnSection btnInfo animate\" onClick=\"showLiquidaciones()\">\r\n                    <div>\r\n                        <li class=\"fa fa-angle-double-right\" aria-hidden=\"true\"></li>\r\n                    </div>\r\n                    <div>Liquidaciones</div>\r\n                </div>\r\n                <div id=\"hideLiquidaciones\" class=\"hideBtn btnSection btnInfo animate btnInfoClose\" onClick=\"closeLiquidaciones()\">\r\n                    <div>\r\n                        <li class=\"fa fa-times\" aria-hidden=\"true\"></li>\r\n                    </div>\r\n                    <div>Liquidaciones</div>\r\n                </div>\r\n                <div class=\"row titleSection titleSectionNoBorder hideMobile\">\r\n                    <div class=\"col-xs-12 col-sm-12\">Liquidaciones</div>\r\n                </div>\r\n                <!--pagos y liquidaciones tabla-->\r\n                <div id=\"myLiquidaciones\" class=\"row myRow hideMobile\">\r\n                    <div class=\"clearfix myTables\">\r\n                        <table id=\"tableModal\" class=\"display nowrap dataTable no-footer dtr-inline rounded shadow animate boletosTable contrato-det-liquidaciones-table\" cellspacing=\"0\" role=\"grid\" aria-describedby=\"tableModal_info\" style=\"width: 100%; padding:10px 0;\">\r\n                            <thead>\r\n                                <tr role=\"row\">\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Fecha</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Tipo</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Comprobante</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">KG</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Precio/tn</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Total</th>\r\n                                </tr>\r\n                            </thead>\r\n                            <tbody>\r\n                                <tr *ngFor=\"let liquidacion of data.liquidaciones\" role=\"row\" class=\"odd\">\r\n                                    <ng-container *ngIf=\"liquidacion.pedido == ''\">\r\n                                        <td tabindex=\"0\" style=\"\"><a (click)=\"showModalTableLiquidacionesResponsive(liquidacion)\" href=\"#\" class=\"btnShowModal\"><i class=\"fa fa-plus fa-lg\" aria-hidden=\"true\"></i></a>{{liquidacion.fecha}}</td>\r\n                                        <td>{{liquidacion.tipo}}</td>\r\n                                        <td>{{liquidacion.comprobante}}</td>\r\n                                        <td style=\"\">{{liquidacion.cantidadString}}</td>\r\n                                        <td style=\"\">{{liquidacion.precioString}}</td>\r\n                                        <td style=\"\">{{liquidacion.totalString}}</td>\r\n                                    </ng-container>\r\n                                    <ng-container *ngIf=\"liquidacion.pedido != ''\">\r\n                                        <td tabindex=\"0\" style=\"\">{{liquidacion.fecha}}</td>\r\n                                        <td>{{liquidacion.tipo}}   <a (click)=\"goToSeccionParam('liquidacion/proforma', liquidacion.pedido)\" href=\"#\">Ver Proforma</a></td>\r\n                                        <td></td>\r\n                                        <td></td>\r\n                                        <td></td>\r\n                                    </ng-container>\r\n                                </tr>\r\n                            </tbody>\r\n                        </table>\r\n                    </div>\r\n                    <!--<a href=\"#\" style=\"text-decoration:none!important;\">\r\n                        <div class=\"btnSection animate\" style=\" margin-top:10px;\">FIJAC. <span style=\"color:#03753e;\">VER PROFORMA</span></div>\r\n                    </a>-->\r\n                </div>\r\n            </div>\r\n        </div>\r\n        <!--fijaciones y ampliaciones-->\r\n        <div class=\"row myRow\">\r\n            <div id=\"Fijaciones\" class=\"col-md-12 col-lg-6\">\r\n                <div id=\"showFijaciones\" class=\"showBtn btnSection btnInfo animate\" onClick=\"showFijaciones()\">\r\n                    <div>\r\n                        <li class=\"fa fa-angle-double-right\" aria-hidden=\"true\"></li>\r\n                    </div>\r\n                    <div>Fijaciones</div>\r\n                </div>\r\n                <div id=\"hideFijaciones\" class=\"hideBtn btnSection btnInfo animate btnInfoClose\" onClick=\"closeFijaciones()\">\r\n                    <div>\r\n                        <li class=\"fa fa-times\" aria-hidden=\"true\"></li>\r\n                    </div>\r\n                    <div>Fijaciones</div>\r\n                </div>\r\n                <div class=\"row titleSection titleSectionNoBorder hideMobile\">\r\n                    <div class=\"col-xs-12 col-sm-12\">Fijaciones</div>\r\n                </div>\r\n                <!--pagos y liquidaciones tabla-->\r\n                <div id=\"myFijaciones\" class=\"row myRow hideMobile\">\r\n                    <div class=\"clearfix myTables\">\r\n                        <table id=\"tableModal\" class=\"display nowrap dataTable no-footer dtr-inline rounded shadow animate boletosTable contrato-det-fijaciones-table\" cellspacing=\"0\" role=\"grid\" aria-describedby=\"tableModal_info\" style=\"width: 100%; padding:10px 0;\">\r\n                            <thead>\r\n                                <tr role=\"row\">\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Fecha</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Nº Fijación</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Kilos Fijados</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Precio/TN</th>\r\n                                </tr>\r\n                            </thead>\r\n                            <tbody>\r\n                                <tr *ngFor=\"let fijacion of data.fijaciones\" role=\"row\" class=\"odd\">\r\n                                    <td tabindex=\"0\"><a (click)=\"showModalTableLFijacionesResponsive(fijacion)\" href=\"#\" class=\"btnShowModal\"><i class=\"fa fa-plus fa-lg\" aria-hidden=\"true\"></i></a>{{fijacion.fecha}}</td>\r\n                                    <td><a class=\"animate\" (click)=\"goToSeccionParamDos('/contrato/detalle-fijacion', data.contrato, fijacion.nroFija)\" href=\"#\">{{fijacion.nroFija}}</a></td>\r\n                                    <td style=\"\">{{fijacion.kilosFijaString}}</td>\r\n                                    <td style=\"\">{{fijacion.precioString}}</td>\r\n                                </tr>\r\n                            </tbody>\r\n                        </table>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n            <div id=\"Ampliaciones\" class=\"col-sm-12 col-md-12 col-lg-6 \">\r\n                <div id=\"showAmpliaciones\" class=\"showBtn btnSection btnInfo animate\" onClick=\"showAmpliaciones()\">\r\n                    <div>\r\n                        <li class=\"fa fa-angle-double-right\" aria-hidden=\"true\"></li>\r\n                    </div>\r\n                    <div>Ampliaciones / Anulaciones</div>\r\n                </div>\r\n                <div id=\"hideAmpliaciones\" class=\"hideBtn btnSection btnInfo animate btnInfoClose\" onClick=\"closeAmpliaciones()\">\r\n                    <div>\r\n                        <li class=\"fa fa-times\" aria-hidden=\"true\"></li>\r\n                    </div>\r\n                    <div>Ampliaciones / Anulaciones</div>\r\n                </div>\r\n\r\n                <div class=\"row titleSection titleSectionNoBorder hideMobile\">\r\n                    <div class=\"col-xs-12 col-sm-12\">Ampliaciones / Anulaciones</div>\r\n                </div>\r\n                <!--pagos y liquidaciones tabla-->\r\n                <div id=\"myAmpliaciones\" class=\"row myRow hideMobile\">\r\n                    <div class=\"clearfix myTables\">\r\n                        <table id=\"tableModal\" class=\"display nowrap dataTable no-footer dtr-inline rounded shadow animate boletosTable contrato-det-ampliaciones-anulaciones-table\" cellspacing=\"0\" role=\"grid\" aria-describedby=\"tableModal_info\" style=\"width: 100%; padding:10px 0;\">\r\n                            <thead>\r\n                                <tr role=\"row\">\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Tipo</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Fecha</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Cantidad</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Precio/Multa</th>\r\n                                </tr>\r\n                            </thead>\r\n                            <tbody>\r\n                                <tr *ngFor=\"let ampliacionAnulacion of data.ampliacionesAnulaciones\" role=\"row\" class=\"odd\">\r\n                                    <td tabindex=\"0\" style=\"\"><a (click)=\"showModalTableAmpliacionesAnulacionesResponsive(ampliacionAnulacion)\" href=\"#\" class=\"btnShowModal\"><i class=\"fa fa-plus fa-lg\" aria-hidden=\"true\"></i></a>{{ampliacionAnulacion.tipo}}</td>\r\n                                    <td>{{ampliacionAnulacion.fecha}}</td>\r\n                                    <td style=\"\">{{ampliacionAnulacion.cantidadString}}</td>\r\n                                    <td style=\"\">{{ampliacionAnulacion.importeString}}</td>\r\n                                </tr>\r\n                            </tbody>\r\n                        </table>\r\n                    </div>\r\n\r\n                </div>\r\n            </div>\r\n        </div>\r\n        <!--Aplicaciones-->\r\n        <div class=\"row myRow\">\r\n            <div id=\"Aplicaciones\" class=\"col-sm-12\">\r\n\r\n                <div id=\"showAplicaciones\" class=\"showBtn btnSection btnInfo animate\" onClick=\"showAplicaciones()\">\r\n                    <div>\r\n                        <li class=\"fa fa-angle-double-right\" aria-hidden=\"true\"></li>\r\n                    </div>\r\n                    <div>Aplicaciones</div>\r\n                </div>\r\n                <div id=\"hideAplicaciones\" class=\"hideBtn btnSection btnInfo animate btnInfoClose\" onClick=\"closeAplicaciones()\">\r\n                    <div>\r\n                        <li class=\"fa fa-times\" aria-hidden=\"true\"></li>\r\n                    </div>\r\n                    <div>Aplicaciones</div>\r\n                </div>\r\n                <div class=\"row titleSection titleSectionNoBorder hideMobile\">\r\n                    <div class=\"col-xs-12 col-sm-12\">Aplicaciones</div>\r\n                </div>\r\n\r\n                <div id=\"myAplicaciones\" class=\"row myRow hideMobile\">\r\n                    <div class=\"clearfix myTables\">\r\n                        <table id=\"tableModal\" class=\"display nowrap dataTable no-footer dtr-inline rounded shadow animate boletosTable contrato-det-aplicacciones-table\" cellspacing=\"0\" role=\"grid\" aria-describedby=\"tableModal_info\" style=\"width: 100%; padding:10px 0;\">\r\n                            <thead>\r\n                                <tr role=\"row\">\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Fecha</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Carta de Porte</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Lugar de Descarga</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">KG Brutos</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">KG Aplicados</th>\r\n                                </tr>\r\n                            </thead>\r\n                            <tbody>\r\n                                <tr *ngFor=\"let aplicacion of data.aplicaciones\" role=\"row\" class=\"odd\">\r\n                                    <td tabindex=\"0\"><a (click)=\"showModalTableAplicacionesResponsive(aplicacion)\" href=\"#\" class=\"btnShowModal\"><i class=\"fa fa-plus fa-lg\" aria-hidden=\"true\"></i></a>{{aplicacion.fecha}}</td>\r\n                                    <td><a class=\"animate\" (click)=\"goToSeccionParam('/carta-porte/detalle', aplicacion.ccpp)\" href=\"#\">{{aplicacion.ccpp}}</a></td>\r\n                                    <td>{{aplicacion.descarga}}</td>\r\n                                    <td style=\"\">{{aplicacion.kgBrutosString}}</td>\r\n                                    <td style=\"\">{{aplicacion.kgNetosString}}</td>\r\n                                </tr>\r\n                            </tbody>\r\n                            <tbody>\r\n                                <tr role=\"row\" class=\"totalTable\">\r\n                                    <td tabindex=\"0\">TOTAL</td>\r\n                                    <td></td>\r\n                                    <td style=\"\"></td>\r\n                                    <td style=\"\">{{data.aplicacionesTotalBrutosString}}</td>\r\n                                    <td style=\"\">{{data.aplicacionesTotalAplicadosString}}</td>\r\n                                </tr>\r\n                            </tbody>\r\n                        </table>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n        <!--contratos hijos-->\r\n        <div class=\"row myRow\">\r\n            <div id=\"Hijos\" class=\"col-sm-12\">\r\n\r\n                <div id=\"showHijos\" class=\"showBtn btnSection btnInfo animate\" onClick=\"showHijos()\">\r\n                    <div>\r\n                        <li class=\"fa fa-angle-double-right\" aria-hidden=\"true\"></li>\r\n                    </div>\r\n                    <div>Contratos hijos</div>\r\n                </div>\r\n                <div id=\"hideHijos\" class=\"hideBtn btnSection btnInfo animate btnInfoClose\" onClick=\"closeHijos()\">\r\n                    <div>\r\n                        <li class=\"fa fa-times\" aria-hidden=\"true\"></li>\r\n                    </div>\r\n                    <div>Contratos hijos</div>\r\n                </div>\r\n                <div class=\"row titleSection titleSectionNoBorder hideMobile\">\r\n                    <div class=\"col-xs-12 col-sm-12\">Contratos hijos</div>\r\n                </div>\r\n\r\n                <div id=\"myHijos\" class=\"row myRow  hideMobile\">\r\n                    <div class=\"clearfix myTables\">\r\n                        <table id=\"tableModal\" class=\"display nowrap dataTable no-footer dtr-inline rounded shadow animate boletosTable contrato-det-hijos-table\" cellspacing=\"0\" role=\"grid\" aria-describedby=\"tableModal_info\" style=\"width: 100%; padding:10px 0;\">\r\n                            <thead>\r\n                                <tr role=\"row\">\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Fecha de Concertación</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Contrato Molinos</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Contrato Proveedor</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Pactado</th>\r\n                                    <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\">Precio/tn</th>\r\n                                </tr>\r\n                            </thead>\r\n                            <tbody>\r\n                                <tr *ngFor=\"let hijo of data.hijos\" role=\"row\" class=\"odd\">\r\n                                    <td tabindex=\"0\"><a (click)=\"showModalTableHijosResponsive(hijo)\" href=\"#\" class=\"btnShowModal\"><i class=\"fa fa-plus fa-lg\" aria-hidden=\"true\"></i></a>{{hijo.fecha}}</td>\r\n                                    <td><a class=\"animate\" (click)=\"goToSeccionParam('/contrato/detalle', hijo.contrMolinos)\" href=\"#\">{{hijo.contrMolinos}}</a></td>\r\n                                    <td style=\"\">{{hijo.contrProve}}</td>\r\n                                    <td>{{hijo.cantidadString}}</td>\r\n                                    <td style=\"\">{{hijo.precioString}}</td>\r\n                                </tr>\r\n                            </tbody>\r\n                        </table>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n\r\n        <!--Calidad-->\r\n        <div class=\"row myRow\">\r\n            <div id=\"Calidad\" class=\"col-sm-12  hideMobile\">\r\n                <div class=\"row titleSection titleSectionNoBorder\">\r\n                    <div class=\"col-xs-12 col-sm-12\">Calidad</div>\r\n                </div>\r\n                <div class=\"row  myRow firstSection mySearch\">\r\n                    <div class=\"col-xs-12 col-md-12 rounded shadow animate\">\r\n                        <title-caps>Refinar resultados de Calidad</title-caps>\r\n                        <hr />\r\n                        <div id=\"searchTexto\" class=\"col-xs-6 col-sm-4 col-md-15 \">\r\n                            Por N&ordm; CCPP:\r\n                            <div class=\"styled-select \">\r\n                                <input type=\"search\" class=\"\" placeholder=\"\" [value]=\"filtroCCPP\" (input)=\"filtroCCPP = $event.target.value\" aria-controls=\"tableModal\">\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"row myRow\">\r\n            <div id=\"Calidad\" class=\"col-sm-12  hideMobile\">\r\n                <div class=\"row myRow\">\r\n                    <div class=\"clearfix totalTable\">\r\n                        <table width=\"100%\">\r\n                            <tr>\r\n                                <td width=\"20%\">\r\n                                    resultado final TOTAL\r\n\r\n                                </td>\r\n                                <td width=\"80%\" style=\"padding-left:15px;\">\r\n\r\n                                    <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n                                        <tr>\r\n                                            <td width=\"20%\"></td>\r\n                                            <td width=\"20%\"></td>\r\n                                            <td width=\"20%\"></td>\r\n                                            <td width=\"20%\">\r\n                                                <p>Kg netos</p>\r\n                                                {{data.calidadTotalNetosString}}\r\n                                            </td>\r\n                                            <td width=\"20%\" style=\"padding:5px;\">\r\n                                                <p>Kg Aplicados</p>\r\n                                                {{data.calidadTotalAplicadosString}}\r\n                                            </td>\r\n                                        </tr>\r\n                                    </table>\r\n                                </td>\r\n\r\n\r\n                            </tr>\r\n                        </table>\r\n\r\n                    </div>\r\n                    <table class=\"CalidadCCPP\" width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n\r\n                        <tr *ngFor=\"let calidad of data.calidad | customFilter : 'ccpp' : filtroCCPP\">\r\n                            <td width=\"20%\" valign=\"top\" class=\"CalidadCCPPTitle animate\">\r\n                                <p>CCPP</p>\r\n                                <a class=\"animate\" (click)=\"goToSeccionParam('/carta-porte/detalle', calidad.ccpp)\" href=\"#\">{{calidad.ccpp}}</a>\r\n                            </td>\r\n                            <td width=\"80%\" valign=\"top\" style=\"padding-left:15px;\">\r\n                                <table class=\"CalidadCCPPData rounded shadow animate\" width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n                                    <tr>\r\n                                        <td>\r\n                                            <table class=\"CalidadCCPPDataPaddingBottom\" width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n\r\n                                                <tr class=\"CalidadCCPPDataPlus\">\r\n                                                    <td colspan=\"5\" style=\"border:none\">\r\n                                                        <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n                                                            <tr>\r\n\r\n                                                                <td width=\"100%\">\r\n                                                                    <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"totalTable CalidadDataExtra\" style=\"\">\r\n                                                                        <tr>\r\n                                                                            <td width=\"20%\">resultado final </td>\r\n                                                                            <td width=\"20%\">\r\n                                                                                <p>Kg Dto.</p>\r\n                                                                                {{calidad.kgDtoTotal}}\r\n                                                                            </td>\r\n                                                                            <td width=\"20%\">\r\n                                                                                <p>Total Dto (%)</p>\r\n                                                                                {{calidad.dtoPorcTotal}}\r\n                                                                            </td>\r\n                                                                            <td width=\"20%\">\r\n                                                                                <p>Kg netos</p>\r\n                                                                                {{calidad.kgNetosTotal}}\r\n                                                                            </td>\r\n                                                                            <td width=\"20%\">\r\n                                                                                <p>Kg Aplicados</p>\r\n                                                                                {{calidad.kgAplicadosTotal}}\r\n                                                                            </td>\r\n                                                                        </tr>\r\n                                                                    </table>\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                                <tr class=\"CalidadCCPPDataCategoria\">\r\n                                                    <th width=\"20%\" rowspan=\"2\" class=\"\">Características</th>\r\n                                                    <th width=\"40%\" colspan=\"2\" class=\"CalidadCCPPDataSub\"><div>Calado</div></th>\r\n                                                    <th width=\"40%\" colspan=\"2\" class=\"CalidadCCPPDataSub\"><div>C&aacute;mara (Cert.: <span *ngIf=\"calidad.certificado != ''\">{{calidad.certificado}}</span><span *ngIf=\"calidad.certificado == ''\">- </span>)</div></th>\r\n                                                </tr>\r\n                                                <tr class=\"CalidadCCPPDataCategoria\">\r\n                                                    <th width=\"20%\" style=\"text-align:right\">Resultado</th>\r\n                                                    <th width=\"20%\">Kg DTO.</th>\r\n                                                    <th width=\"20%\">Resultado</th>\r\n                                                    <th width=\"20%\">DTO (%)</th>\r\n                                                </tr>\r\n                                                <tr *ngFor=\"let registro of calidad.registros\">\r\n                                                    <td>{{registro.caract}}</td>\r\n                                                    <td>{{registro.calaResul}}%</td>\r\n                                                    <td>{{registro.kgDto}}</td>\r\n                                                    <td>{{registro.camaResul}}%</td>\r\n                                                    <td>{{registro.dto}}%</td>\r\n\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                </table>\r\n                            </td>\r\n                        </tr>\r\n                    </table>\r\n                </div>\r\n            </div>\r\n        </div>\r\n\r\n        <!--Botones mobile-->\r\n        <div class=\"showBtn btnSection btnInfo animate downloadPDF\">\r\n            <a (click)=\"descargarPDF()\" href=\"#\">\r\n                <div>\r\n                    <li class=\"fa fa-file-pdf-o\" aria-hidden=\"true\"></li>\r\n                </div>\r\n                <div>Calidad</div>\r\n            </a>\r\n        </div>\r\n        <!--Fin botones mobile-->\r\n    </section>\r\n    <!--End CONTENT-->\r\n</div>\r\n\r\n\r\n"

/***/ }),

/***/ "./src/app/contrato/detalle/contrato.detalle.component.ts":
/*!****************************************************************!*\
  !*** ./src/app/contrato/detalle/contrato.detalle.component.ts ***!
  \****************************************************************/
/*! exports provided: ContratoDetalleComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContratoDetalleComponent", function() { return ContratoDetalleComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/router */ "./node_modules/@angular/router/fesm5/router.js");
/* harmony import */ var _contrato_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./../contrato.service */ "./src/app/contrato/contrato.service.ts");
/* harmony import */ var _common_view_child_mensaje_mensaje_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./../../common/view-child/mensaje/mensaje.component */ "./src/app/common/view-child/mensaje/mensaje.component.ts");
/* harmony import */ var _common_view_child_spinner_spinner_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./../../common/view-child/spinner/spinner.component */ "./src/app/common/view-child/spinner/spinner.component.ts");
/* harmony import */ var _common_view_child_spinner_small_spinner_small_component__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./../../common/view-child/spinner-small/spinner-small.component */ "./src/app/common/view-child/spinner-small/spinner-small.component.ts");
/* harmony import */ var _common_services_SecurityService__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./../../common/services/SecurityService */ "./src/app/common/services/SecurityService.ts");
/* harmony import */ var _common_services_NavService__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./../../common/services/NavService */ "./src/app/common/services/NavService.ts");
/* harmony import */ var _common_services_FloatMsgService__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./../../common/services/FloatMsgService */ "./src/app/common/services/FloatMsgService.ts");
/* harmony import */ var _common_base_components_base_component__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./../../common/base-components/base-component */ "./src/app/common/base-components/base-component.ts");
/* harmony import */ var _common_services_SessionDataService__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./../../common/services/SessionDataService */ "./src/app/common/services/SessionDataService.ts");
/* harmony import */ var _common_models_Seccion__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ./../../common/models/Seccion */ "./src/app/common/models/Seccion.ts");
/* harmony import */ var _common_services_ModalService__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ./../../common/services/ModalService */ "./src/app/common/services/ModalService.ts");
var __extends = (undefined && undefined.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};













var ContratoDetalleComponent = /** @class */ (function (_super) {
    __extends(ContratoDetalleComponent, _super);
    function ContratoDetalleComponent(route, router, service, navService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.route = route;
        _this.router = router;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.data = null;
        _this.tituloArchivoPDF = "BoletoFisico";
        _this.tituloArchivoExcel = "ReporteContratoDetalle.xls";
        _this.numeroContratoId = "";
        _this.filtroCCPP = "";
        _this.mensajeComponent = new _common_view_child_mensaje_mensaje_component__WEBPACK_IMPORTED_MODULE_3__["MensajeComponent"]();
        _this.spinnerComponent = new _common_view_child_spinner_spinner_component__WEBPACK_IMPORTED_MODULE_4__["SpinnerComponent"]();
        return _this;
    }
    ContratoDetalleComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("contrato", "Detalle");
    };
    ContratoDetalleComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR CONTRATO DETALLE");
        this.navService.setSeccionList([new _common_models_Seccion__WEBPACK_IMPORTED_MODULE_11__["Seccion"]('/contrato/vigente', 'contrato', 'Vigentes'), new _common_models_Seccion__WEBPACK_IMPORTED_MODULE_11__["Seccion"]('/contrato/fijacion', 'contrato', 'Fijaciones'), new _common_models_Seccion__WEBPACK_IMPORTED_MODULE_11__["Seccion"]('/contrato/ampliacion', 'contrato', 'Ampliaciones'), new _common_models_Seccion__WEBPACK_IMPORTED_MODULE_11__["Seccion"]('/contrato/anulacion', 'contrato', 'Anulaciones')]);
        this.getData();
    };
    ContratoDetalleComponent.prototype.ngAfterViewInit = function () {
        this.spinnerSmallExportComponent = new _common_view_child_spinner_small_spinner_small_component__WEBPACK_IMPORTED_MODULE_5__["SpinnerSmallComponent"]();
        this.spinnerSmallPDFComponent = new _common_view_child_spinner_small_spinner_small_component__WEBPACK_IMPORTED_MODULE_5__["SpinnerSmallComponent"]();
    };
    ContratoDetalleComponent.prototype.getData = function () {
        var _this = this;
        this.data = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.subscribe(function (params) {
            _this.numeroContratoId = params['id'];
            _this.unsubscribe();
            _this.subscription = _this.service.getDetalle(_this.numeroContratoId).subscribe(function (result) {
                _this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.data = result;
                }
            }, function (error) {
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
            //this.navService.setSeccionActive('Fijaciones');
        });
    };
    ContratoDetalleComponent.prototype.downloadBoletoFisico = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallPDFComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.downloadBoletoFisico(this.numeroContratoId).subscribe(function (result) {
            _this.spinnerSmallPDFComponent.hideIt();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                var byteArray = new Uint8Array(result.data);
                var blob = new Blob([byteArray], { type: 'application/pdf' });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, _this.tituloArchivoPDF + _this.numeroContratoId + ".pdf");
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = _this.tituloArchivoPDF + _this.numeroContratoId + ".pdf";
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.spinnerSmallPDFComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    ContratoDetalleComponent.prototype.exportarExcel = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallExportComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcelDetalle(this.numeroContratoId).subscribe(function (result) {
            _this.spinnerSmallExportComponent.hideIt();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                var blob = new Blob([result], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, _this.tituloArchivoExcel);
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = _this.tituloArchivoExcel;
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.spinnerSmallExportComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    ContratoDetalleComponent.prototype.descargarPDF = function () {
        var _this = this;
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.exportPDFCalidad(this.numeroContratoId).subscribe(function (result) {
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
                    window.navigator.msSaveOrOpenBlob(blob, "Calidad Contrato(" + _this.numeroContratoId + ").pdf");
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = "Calidad Contrato(" + _this.numeroContratoId + ").pdf";
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.floatMsgService.setErrorMsg(error.message);
        });
        return false;
    };
    ContratoDetalleComponent.prototype.showDataPlus = function (registro) {
        return this.tieneData(registro.kgNetos) || this.tieneData(registro.kgDto) || this.tieneData(registro.kgApli) || this.tieneData(registro.dto);
    };
    ContratoDetalleComponent.prototype.tieneData = function (value) {
        return value != undefined && value != 0 && value != "" && value != "0 KG" && value != "0%";
    };
    ContratoDetalleComponent.prototype.descargaRetencionesPDF = function (pago) {
        // Descarga PDF TODO
        return false;
    };
    ContratoDetalleComponent.prototype.showModalTableBoletosResponsive = function (Boleto) {
        this.modalService.openModalTableResponsive("Oblea Boleto", [
            { etiqueta: "Envío bolsa", valor: Boleto.envioBolsa },
            { etiqueta: "Vuelta Bolsa", valor: Boleto.vueltaBolsa },
            { etiqueta: "Oblea Bolsa", valor: Boleto.obleaBolsa },
            { etiqueta: "Envío Afip", valor: Boleto.envioAfip },
            { etiqueta: "Vuelta Afip", valor: Boleto.vueltaAfip },
            { etiqueta: "REG. Afip", valor: Boleto.obleaAfip },
            { etiqueta: "Oblea Prov. Plan Canje", valor: Boleto.provPlanCanje },
            { etiqueta: "Envío Sellado", valor: Boleto.envioSellado },
            { etiqueta: "Vuelta Sellado", valor: Boleto.vueltaSellado }
        ]);
        return false;
    };
    ContratoDetalleComponent.prototype.showModalTablePagosResponsive = function (Pago) {
        this.modalService.openModalTableResponsive("Pago", [
            { etiqueta: "Fecha", valor: Pago.fecha },
            { etiqueta: "ID pago", valor: Pago.idPago },
            { etiqueta: "Brutor", valor: Pago.brutoString },
            { etiqueta: "Iva", valor: Pago.ivaString },
            { etiqueta: "Retenciones", valor: Pago.retencionesString },
            { etiqueta: "Total", valor: Pago.netoString }
        ]);
        return false;
    };
    ContratoDetalleComponent.prototype.showModalTableLiquidacionesResponsive = function (Liquidacion) {
        this.modalService.openModalTableResponsive("Liquidación", [
            { etiqueta: "Fecha", valor: Liquidacion.fecha },
            { etiqueta: "Tipo", valor: Liquidacion.tipo },
            { etiqueta: "Comprobante", valor: Liquidacion.comprobante },
            { etiqueta: "KG", valor: Liquidacion.cantidadString },
            { etiqueta: "Precio/tn", valor: Liquidacion.precioString },
            { etiqueta: "Total", valor: Liquidacion.totalString }
        ]);
        return false;
    };
    ContratoDetalleComponent.prototype.showModalTableLFijacionesResponsive = function (Fijacion) {
        this.modalService.openModalTableResponsive("Fijación", [
            { etiqueta: "Fecha", valor: Fijacion.fecha },
            { etiqueta: "Nº Fijación", valor: Fijacion.nroFija },
            { etiqueta: "Kilos Fijados", valor: Fijacion.kilosFijaString },
            { etiqueta: "Precio/TN", valor: Fijacion.precioString }
        ]);
        return false;
    };
    ContratoDetalleComponent.prototype.showModalTableAmpliacionesAnulacionesResponsive = function (AmpAnul) {
        this.modalService.openModalTableResponsive("Ampliación / Anulación", [
            { etiqueta: "Tipo", valor: AmpAnul.tipo },
            { etiqueta: "Fecha", valor: AmpAnul.fecha },
            { etiqueta: "Cantidad", valor: AmpAnul.cantidadString },
            { etiqueta: "Precio/Multa", valor: AmpAnul.importeString }
        ]);
        return false;
    };
    ContratoDetalleComponent.prototype.showModalTableAplicacionesResponsive = function (Aplicacion) {
        this.modalService.openModalTableResponsive("Aplicación", [
            { etiqueta: "Fecha", valor: Aplicacion.fecha },
            { etiqueta: "Carta de Porte", valor: Aplicacion.ccpp },
            { etiqueta: "Lugar de Descarga", valor: Aplicacion.descarga },
            { etiqueta: "KG Brutos", valor: Aplicacion.kgBrutosString },
            { etiqueta: "KG Aplicados", valor: Aplicacion.kgNetosString }
        ]);
        return false;
    };
    ContratoDetalleComponent.prototype.showModalTableHijosResponsive = function (Hijo) {
        this.modalService.openModalTableResponsive("Hijo", [
            { etiqueta: "Fecha de Concertación", valor: Hijo.fecha },
            { etiqueta: "Contrato Molinos", valor: Hijo.contrMolinos },
            { etiqueta: "Contrato Proveedor", valor: Hijo.contrProve },
            { etiqueta: "Pactado", valor: Hijo.cantidadString },
            { etiqueta: "Precio/tn", valor: Hijo.precioString }
        ]);
        return false;
    };
    ContratoDetalleComponent.prototype.isData = function () {
        return this.data != null;
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])(_common_view_child_mensaje_mensaje_component__WEBPACK_IMPORTED_MODULE_3__["MensajeComponent"]),
        __metadata("design:type", _common_view_child_mensaje_mensaje_component__WEBPACK_IMPORTED_MODULE_3__["MensajeComponent"])
    ], ContratoDetalleComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])(_common_view_child_spinner_spinner_component__WEBPACK_IMPORTED_MODULE_4__["SpinnerComponent"]),
        __metadata("design:type", _common_view_child_spinner_spinner_component__WEBPACK_IMPORTED_MODULE_4__["SpinnerComponent"])
    ], ContratoDetalleComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])("spinnerSmallExport"),
        __metadata("design:type", _common_view_child_spinner_small_spinner_small_component__WEBPACK_IMPORTED_MODULE_5__["SpinnerSmallComponent"])
    ], ContratoDetalleComponent.prototype, "spinnerSmallExportComponent", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])("spinnerSmallPDF"),
        __metadata("design:type", _common_view_child_spinner_small_spinner_small_component__WEBPACK_IMPORTED_MODULE_5__["SpinnerSmallComponent"])
    ], ContratoDetalleComponent.prototype, "spinnerSmallPDFComponent", void 0);
    ContratoDetalleComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-contrato-detalle',
            template: __webpack_require__(/*! ./contrato.detalle.component.html */ "./src/app/contrato/detalle/contrato.detalle.component.html"),
            providers: [{ provide: _contrato_service__WEBPACK_IMPORTED_MODULE_2__["ContratoService"], useClass: _contrato_service__WEBPACK_IMPORTED_MODULE_2__["ContratoFijacionService"] }]
        }),
        __metadata("design:paramtypes", [_angular_router__WEBPACK_IMPORTED_MODULE_1__["ActivatedRoute"],
            _angular_router__WEBPACK_IMPORTED_MODULE_1__["Router"],
            _contrato_service__WEBPACK_IMPORTED_MODULE_2__["ContratoService"],
            _common_services_NavService__WEBPACK_IMPORTED_MODULE_7__["NavService"],
            _common_services_SecurityService__WEBPACK_IMPORTED_MODULE_6__["SecurityService"],
            _common_services_SessionDataService__WEBPACK_IMPORTED_MODULE_10__["SessionDataService"],
            _common_services_FloatMsgService__WEBPACK_IMPORTED_MODULE_8__["FloatMsgService"],
            _common_services_ModalService__WEBPACK_IMPORTED_MODULE_12__["ModalService"]])
    ], ContratoDetalleComponent);
    return ContratoDetalleComponent;
}(_common_base_components_base_component__WEBPACK_IMPORTED_MODULE_9__["BaseComponent"]));



/***/ }),

/***/ "./src/app/contrato/fijacion/contrato.fijacion.component.html":
/*!********************************************************************!*\
  !*** ./src/app/contrato/fijacion/contrato.fijacion.component.html ***!
  \********************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"content-wraper\">\r\n    <!-- CONTENT-->\r\n    <section class=\"container-fluid clearfix\">\r\n        <!--Title-->\r\n        <div class=\"row titleSection\">\r\n            <div class=\"col-xs-12 col-sm-10 \">Contrato: <strong>Fijaciones</strong> </div>\r\n            <div class=\"col-xs-12 col-sm-2\">\r\n                <spinner-small></spinner-small>\r\n                <a (click)=\"exportExcel()\" href=\"#\" style=\"text-decoration:none!important\" *ngIf=\"!this.spinnerSmallComponent.visible\">\r\n                    <div class=\"btnSection animate\">Exportar <span class=\"fa  fa-file-excel-o\"></span></div>\r\n                </a>\r\n            </div>\r\n        </div>\r\n        <!--End Title-->\r\n        <!--Search-->\r\n        <div class=\"row  myRow firstSection mySearch\">\r\n            <div class=\"col-xs-12 col-md-12 rounded shadow animate\">\r\n                <title-caps>Refinar resultados</title-caps>\r\n                <hr>\r\n                <div id=\"searchVer\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Ver:\r\n                    <filtro-fecha (ClickEvent)=\"getData()\"></filtro-fecha>\r\n                </div>\r\n                \r\n                <div id=\"searchVendedor\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Por Vendedor:\r\n                    <div class=\"styled-select\">\r\n                        <dropdown [tipoDropdown]=\"'filtroVariable'\" [options]=\"filtroVendedor\" (select)=\"setFiltroVendedor($event)\"></dropdown>\r\n                    </div>\r\n                </div>\r\n                <div id=\"searchProducto\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Por Producto:\r\n                    <div class=\"styled-select\">\r\n                        <dropdown [tipoDropdown]=\"'filtroVariable'\" [options]=\"filtroProducto\" (select)=\"setFiltroProducto($event)\"></dropdown>\r\n                    </div>\r\n                </div>\r\n                <div id=\"searchContrato\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Por Contrato:\r\n                    <div class=\"styled-select\">\r\n                        <input type=\"search\" class=\"\" placeholder=\"\" [value]=\"filtroContrato\" (input)=\"filtroContrato = $event.target.value\" aria-controls=\"tableModal\">\r\n                    </div>\r\n                </div>\r\n                <div id=\"searchMostrar\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Mostrar:\r\n                    <div class=\"styled-select\">\r\n                        <dropdown [tipoDropdown]=\"'numberItems'\" (select)=\"setItemsPerPage($event)\"></dropdown>\r\n                    </div>\r\n                </div>\r\n                <div id=\"showSearch\" onClick=\"openSearch()\">\r\n                    <div style=\"\">\r\n                        <i class=\"fa fa-angle-double-right\" aria-hidden=\"true\"></i>\r\n                    </div>\r\n                    <div>VER MÁS OPCIONES DE BÚSQUEDA</div>\r\n                </div>\r\n                <div id=\"hideSearch\" onClick=\"closeSearch()\">\r\n                    <div style=\"\">\r\n                        <i class=\"fa fa-times\" aria-hidden=\"true\"></i>\r\n                    </div>\r\n                    <div>OCULTAR OPCIONES DE BÚSQUEDA</div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n        <!-- End Search-->\r\n\r\n        <!-- Tables-->\r\n\r\n        <div class=\"row myRow\">\r\n            <div class=\"clearfix myTables\">\r\n\r\n                <mensaje></mensaje>\r\n                <spinner></spinner>\r\n\r\n                <table *ngIf=\"isVisible()\" id=\"tableModal\" class=\"display nowrap dataTable no-footer dtr-inline rounded shadow animate contrato-fijacion-table\" cellspacing=\"0\" role=\"grid\" aria-describedby=\"tableModal_info\" style=\"width: 100%; padding:10px 0;\">\r\n                    <thead>\r\n                        <tr role=\"row\">\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('fechaDate')\">Fecha de &uacute;ltima fijaci&oacute;n</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('nroContrato')\">Contrato Molinos</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('contrvend')\">Contrato Proveedor</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('material')\">Producto</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('kilosFijados')\">Fijado</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('importe')\">Precio/TN</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('vendedor')\" *ngIf=\"isCorredor()\">Vendedor</th>\r\n                        </tr>\r\n                    </thead>\r\n                    <tbody>\r\n                        <tr *ngFor=\"let contratoInfo of data.contratosInfo | customFilter : 'material' : productoSelected | customFilter : 'vendedor' : vendedorSelected | customFilter : 'nroContrato' : filtroContrato | orderedColumn : {property: orderedByColumn, direction: orderDirection} | paginate: { itemsPerPage:itemsPerPage, currentPage: p } ; let odd = odd\" role=\"row\" [class.odd]=\"!odd\">\r\n                            <td><a (click)=\"showModalTableResponsive(contratoInfo)\" class=\"btnShowModal\"><i class=\"fa fa-plus fa-lg\" aria-hidden=\"true\"></i></a> {{contratoInfo.fecha}}</td>\r\n                            <td><a class=\"animate\" (click)=\"goToSeccionParam('/contrato/detalle', contratoInfo.nroContrato)\" href=\"#\">{{contratoInfo.nroContrato}}</a></td>\r\n                            <td>{{contratoInfo.contrvend}}</td>\r\n                            <td>{{contratoInfo.material}}</td>\r\n                            <td>{{contratoInfo.kilosFijadosString}}</td>\r\n                            <td>{{contratoInfo.importeString}}</td>\r\n                            <td *ngIf=\"isCorredor()\"><a (click)=\"goToSeccionParamDos('/dato-fiscal/situacion-fiscal', contratoInfo.idVendedor, contratoInfo.vendedor)\" href=\"#\" class=\"animate\">{{contratoInfo.vendedor}}</a></td>\r\n                        </tr>\r\n                    </tbody>\r\n                </table>\r\n\r\n                <pagination-template #pT=\"paginationApi\" (pageChange)=\"p = $event\" *ngIf=\"isVisible()\">\r\n                    <div class=\"dataTables_paginate paging_simple_numbers\">\r\n                        <a id=\"tableModal_previous\" class=\"paginate_button previous\" (click)=\"pT.previous()\" [class.disabled]=\"pT.isFirstPage()\"> Anterior </a>\r\n                        <span *ngFor=\"let page of pT.pages\" [class.current]=\"pT.getCurrent() === page.value\">\r\n                            <a class=\"paginate_button\" (click)=\"pT.setCurrent(page.value)\" *ngIf=\"pT.getCurrent() !== page.value\">{{ page.label }}</a>\r\n                            <a class=\"paginate_button current\" *ngIf=\"pT.getCurrent() === page.value\">{{ page.label }}</a>\r\n                        </span>\r\n                        <a id=\"tableModal_next\" class=\"paginate_button next\" (click)=\"pT.next()\" [class.disabled]=\"pT.isLastPage()\"> Siguiente </a>\r\n                    </div>\r\n                </pagination-template>\r\n            </div>\r\n        </div>\r\n        <!-- END tables-->\r\n    </section>\r\n</div>\r\n<!--End CONTENT-->\r\n"

/***/ }),

/***/ "./src/app/contrato/fijacion/contrato.fijacion.component.ts":
/*!******************************************************************!*\
  !*** ./src/app/contrato/fijacion/contrato.fijacion.component.ts ***!
  \******************************************************************/
/*! exports provided: ContratoFijacionComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContratoFijacionComponent", function() { return ContratoFijacionComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _contrato_component__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./../contrato.component */ "./src/app/contrato/contrato.component.ts");
/* harmony import */ var _contrato_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./../contrato.service */ "./src/app/contrato/contrato.service.ts");
var __extends = (undefined && undefined.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};



var ContratoFijacionComponent = /** @class */ (function (_super) {
    __extends(ContratoFijacionComponent, _super);
    function ContratoFijacionComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.tituloArchivo = "ReporteContratosFijaciones.xls";
        return _this;
    }
    ContratoFijacionComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("contrato", "Fijaciones");
    };
    ContratoFijacionComponent.prototype.showModalTableResponsive = function (contratoInfo) {
        this.modalService.openModalTableResponsive("Contrato", [
            { etiqueta: "Fecha de Anulación", valor: contratoInfo.fecha },
            { etiqueta: "Contrato Molinos", valor: contratoInfo.nroContrato },
            { etiqueta: "Contrato Proveedor", valor: contratoInfo.contrvend },
            { etiqueta: "Producto", valor: contratoInfo.material },
            { etiqueta: "Fijado", valor: contratoInfo.kilosFijadosString },
            { etiqueta: "Precio/TN", valor: contratoInfo.importeString },
            { etiqueta: "Vendedor", valor: contratoInfo.vendedor }
        ]);
        return false;
    };
    ContratoFijacionComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-contrato-fijacion',
            template: __webpack_require__(/*! ./contrato.fijacion.component.html */ "./src/app/contrato/fijacion/contrato.fijacion.component.html"),
            providers: [{ provide: _contrato_service__WEBPACK_IMPORTED_MODULE_2__["ContratoService"], useClass: _contrato_service__WEBPACK_IMPORTED_MODULE_2__["ContratoFijacionService"] }]
        })
    ], ContratoFijacionComponent);
    return ContratoFijacionComponent;
}(_contrato_component__WEBPACK_IMPORTED_MODULE_1__["ContratoBaseComponent"]));



/***/ }),

/***/ "./src/app/contrato/vigente/contrato.vigente.component.html":
/*!******************************************************************!*\
  !*** ./src/app/contrato/vigente/contrato.vigente.component.html ***!
  \******************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"content-wraper\">\r\n    <!-- CONTENT-->\r\n    <section class=\"container-fluid clearfix\">\r\n        <!--Title-->\r\n        <div class=\"row titleSection\">\r\n            <div class=\"col-xs-12 col-sm-10 \">Contrato: <strong>Vigentes</strong> </div>\r\n            <div class=\"col-xs-12 col-sm-2\">\r\n                <spinner-small></spinner-small>\r\n                <a (click)=\"exportExcel()\" href=\"#\" style=\"text-decoration:none!important\" *ngIf=\"!this.spinnerSmallComponent.visible\">\r\n                    <div class=\"btnSection animate\">Exportar <span class=\"fa  fa-file-excel-o\"></span></div>\r\n                </a>\r\n            </div>\r\n        </div>\r\n        <!--End Title-->\r\n        <!--Search-->\r\n        <div class=\"row myRow firstSection mySearch\">\r\n            <div class=\"col-xs-12 col-md-12 rounded shadow animate\">\r\n                <title-caps>Refinar resultados</title-caps>\r\n                <hr>\r\n                <div id=\"searchVer\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Ver:\r\n                    <filtro-fecha (ClickEvent)=\"getData()\"></filtro-fecha>\r\n                </div>\r\n\r\n                <div id=\"searchVendedor\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Por Vendedor:\r\n                    <div class=\"styled-select\">\r\n                        <dropdown [tipoDropdown]=\"'filtroVariable'\" [options]=\"filtroVendedor\" (select)=\"setFiltroVendedor($event)\"></dropdown>\r\n                    </div>\r\n                </div>\r\n                <div id=\"searchProducto\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Por Producto:\r\n                    <div class=\"styled-select\">\r\n                        <dropdown [tipoDropdown]=\"'filtroVariable'\" [options]=\"filtroProducto\" (select)=\"setFiltroProducto($event)\"></dropdown>\r\n                    </div>\r\n                </div>\r\n                <div id=\"searchContrato\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Por Contrato:\r\n                    <div class=\"styled-select\">\r\n                        <input type=\"search\" class=\"\" placeholder=\"\" [value]=\"filtroContrato\" (input)=\"filtroContrato = $event.target.value\" aria-controls=\"tableModal\">\r\n                    </div>\r\n                </div>\r\n                <div id=\"searchMostrar\" class=\"col-xs-12 col-sm-6 col-md-15\">\r\n                    Mostrar:\r\n                    <div class=\"styled-select\">\r\n                        <dropdown [tipoDropdown]=\"'numberItems'\" (select)=\"setItemsPerPage($event)\"></dropdown>\r\n                    </div>\r\n                </div>\r\n                <div id=\"showSearch\" onClick=\"openSearch()\">\r\n                    <div style=\"\">\r\n                        <i class=\"fa fa-angle-double-right\" aria-hidden=\"true\"></i>\r\n                    </div>\r\n                    <div>VER MÁS OPCIONES DE BÚSQUEDA</div>\r\n                </div>\r\n                <div id=\"hideSearch\" onClick=\"closeSearch()\">\r\n                    <div style=\"\">\r\n                        <i class=\"fa fa-times\" aria-hidden=\"true\"></i>\r\n                    </div>\r\n                    <div>OCULTAR OPCIONES DE BÚSQUEDA</div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n        <!-- End Search-->\r\n        <!-- Tables-->\r\n\r\n        <div class=\"row myRow\">\r\n            <div class=\"clearfix myTables\">\r\n\r\n                <mensaje></mensaje>\r\n                <spinner></spinner>\r\n\r\n                <table *ngIf=\"isVisible()\" id=\"tableModal\" class=\"display nowrap dataTable no-footer dtr-inline rounded shadow animate contrato-vigente-table\" cellspacing=\"0\" role=\"grid\" aria-describedby=\"tableModal_info\" style=\"width: 100%; padding:10px 0;\">\r\n                    <thead>\r\n                        <tr role=\"row\">\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('fechaDate')\">&Uacute;ltimo Movimiento</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('nroContrato')\">Contrato Molinos</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('contrvend')\">Contrato Proveedor</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('estado')\">Estado Boleto</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('material')\">Producto</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('cantKilos')\">Pactado</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('ampliado')\">Entregado</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('liquidado')\">Liquidado</th>\r\n                            <th tabindex=\"0\" rowspan=\"1\" colspan=\"1\" style=\"\" (click)=\"orderColumnBy('vendedor')\" *ngIf=\"isCorredor()\">Vendedor</th>\r\n                        </tr>\r\n                    </thead>\r\n                    <tbody>\r\n                        <tr *ngFor=\"let contratoInfo of data.contratosInfo | customFilter : 'material' : productoSelected | customFilter : 'vendedor' : vendedorSelected | customFilter : 'nroContrato' : filtroContrato | orderedColumn : {property: orderedByColumn, direction: orderDirection} | paginate: { itemsPerPage:itemsPerPage, currentPage: p } ; let odd = odd\" role=\"row\" [class.odd]=\"!odd\">\r\n                            <td><a (click)=\"showModalTableResponsive(contratoInfo)\" href=\"#\" class=\"btnShowModal\"><i class=\"fa fa-plus fa-lg\" aria-hidden=\"true\"></i></a> {{contratoInfo.fecha}}</td>\r\n                            <td><a class=\"animate\" (click)=\"goToSeccionParam('/contrato/detalle', contratoInfo.nroContrato)\" href=\"#\">{{contratoInfo.nroContrato}}</a></td>\r\n                            <td>{{contratoInfo.contrvend}}</td>\r\n                            <td>{{contratoInfo.estado}}</td>\r\n                            <td>{{contratoInfo.material}}</td>\r\n                            <td>{{contratoInfo.cantKilosString}}</td>\r\n                            <td>{{contratoInfo.aplicacionesString}}</td>\r\n                            <td>{{contratoInfo.liquidadoString}}</td>\r\n                            <td *ngIf=\"isCorredor()\"><a (click)=\"goToSeccionParamDos('/dato-fiscal/situacion-fiscal', contratoInfo.idVendedor, contratoInfo.vendedor)\" href=\"#\" class=\"animate\">{{contratoInfo.vendedor}}</a></td>\r\n                        </tr>\r\n                    </tbody>\r\n                </table>\r\n\r\n                <pagination-template #pT=\"paginationApi\" (pageChange)=\"p = $event\" *ngIf=\"isVisible()\">\r\n                    <div class=\"dataTables_paginate paging_simple_numbers\">\r\n                        <a id=\"tableModal_previous\" class=\"paginate_button previous\" (click)=\"pT.previous()\" [class.disabled]=\"pT.isFirstPage()\"> Anterior </a>\r\n                        <span *ngFor=\"let page of pT.pages\" [class.current]=\"pT.getCurrent() === page.value\">\r\n                            <a class=\"paginate_button\" (click)=\"pT.setCurrent(page.value)\" *ngIf=\"pT.getCurrent() !== page.value\">{{ page.label }}</a>\r\n                            <a class=\"paginate_button current\" *ngIf=\"pT.getCurrent() === page.value\">{{ page.label }}</a>\r\n                        </span>\r\n                        <a id=\"tableModal_next\" class=\"paginate_button next\" (click)=\"pT.next()\" [class.disabled]=\"pT.isLastPage()\"> Siguiente </a>\r\n                    </div>\r\n                </pagination-template>\r\n            </div>\r\n        </div>\r\n        <!-- END tables-->\r\n    </section>\r\n    <!--End CONTENT-->\r\n</div>"

/***/ }),

/***/ "./src/app/contrato/vigente/contrato.vigente.component.ts":
/*!****************************************************************!*\
  !*** ./src/app/contrato/vigente/contrato.vigente.component.ts ***!
  \****************************************************************/
/*! exports provided: ContratoVigenteComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContratoVigenteComponent", function() { return ContratoVigenteComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _contrato_component__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./../contrato.component */ "./src/app/contrato/contrato.component.ts");
/* harmony import */ var _contrato_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./../contrato.service */ "./src/app/contrato/contrato.service.ts");
var __extends = (undefined && undefined.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};



var ContratoVigenteComponent = /** @class */ (function (_super) {
    __extends(ContratoVigenteComponent, _super);
    function ContratoVigenteComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.tituloArchivo = "ReporteContratoVigentes.xls";
        return _this;
    }
    ContratoVigenteComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("contrato", "Vigentes");
    };
    ContratoVigenteComponent.prototype.showModalTableResponsive = function (Contrato) {
        this.modalService.openModalTableResponsive("Contrato Vigente", [
            { etiqueta: "Último Movimiento", valor: Contrato.fecha },
            { etiqueta: "Contrato Molinos", valor: Contrato.nroContrato },
            { etiqueta: "Contrato Proveedor", valor: Contrato.contrvend },
            { etiqueta: "Estado Boleto", valor: Contrato.estado },
            { etiqueta: "Producto", valor: Contrato.material },
            { etiqueta: "Pactado", valor: Contrato.cantKilosString },
            { etiqueta: "Entregado", valor: Contrato.aplicacionesString },
            { etiqueta: "Liquidado", valor: Contrato.liquidadoString }
        ]);
        return false;
    };
    ContratoVigenteComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-contrato-vigente',
            template: __webpack_require__(/*! ./contrato.vigente.component.html */ "./src/app/contrato/vigente/contrato.vigente.component.html"),
            providers: [{ provide: _contrato_service__WEBPACK_IMPORTED_MODULE_2__["ContratoService"], useClass: _contrato_service__WEBPACK_IMPORTED_MODULE_2__["ContratoVigenteService"] }]
        })
    ], ContratoVigenteComponent);
    return ContratoVigenteComponent;
}(_contrato_component__WEBPACK_IMPORTED_MODULE_1__["ContratoBaseComponent"]));



/***/ })

}]);
//# sourceMappingURL=contrato-contrato-module.js.map