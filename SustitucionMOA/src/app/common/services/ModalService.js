var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
var ModalService = /** @class */ (function () {
    function ModalService() {
        this.modalHeader = new Subject();
        this.modalInfoLiquidacion = new Subject();
        this.modalLiquidacionContrato = new Subject();
        this.modalLiquidacionSecuencia = new Subject();
        this.modalLiquidacionComprobante = new Subject();
        this.modalShowLiquidacion = new Subject();
        this.modalInfoComprobante = new Subject();
        this.modalShowComprobante = new Subject();
        this.modalInfoDetalle = new Subject();
        this.modalShowDetalle = new Subject();
        this.modalInfoFlete = new Subject();
        this.modalShowFlete = new Subject();
        this.modalContratoFleteProcedencia = new Subject();
        this.modalInfoFleteProcedencia = new Subject();
        this.modalShowFleteProcedencia = new Subject();
        this.modalShowEmpresaCambiarEstado = new Subject();
        this.modalShowNoticia = new Subject();
        this.modalFooter = new Subject();
        this.dataGuardarFlete = new Subject();
        this.modalMsjError = new Subject();
        this.modalMsjSuccess = new Subject();
        this.modalHeader$ = this.modalHeader.asObservable();
        this.modalInfoLiquidacion$ = this.modalInfoLiquidacion.asObservable();
        this.modalLiquidacionContrato$ = this.modalLiquidacionContrato.asObservable();
        this.modalLiquidacionSecuencia$ = this.modalLiquidacionSecuencia.asObservable();
        this.modalLiquidacionComprobante$ = this.modalLiquidacionComprobante.asObservable();
        this.modalShowLiquidacion$ = this.modalShowLiquidacion.asObservable();
        this.modalInfoComprobante$ = this.modalInfoComprobante.asObservable();
        this.modalShowComprobante$ = this.modalShowComprobante.asObservable();
        this.modalInfoDetalle$ = this.modalInfoDetalle.asObservable();
        this.modalShowDetalle$ = this.modalShowDetalle.asObservable();
        this.modalInfoFlete$ = this.modalInfoFlete.asObservable();
        this.modalShowFlete$ = this.modalShowFlete.asObservable();
        this.modalContratoFleteProcedencia$ = this.modalContratoFleteProcedencia.asObservable();
        this.modalInfoFleteProcedencia$ = this.modalInfoFleteProcedencia.asObservable();
        this.modalShowFleteProcedencia$ = this.modalShowFleteProcedencia.asObservable();
        this.modalShowEmpresaCambiarEstado$ = this.modalShowEmpresaCambiarEstado.asObservable();
        this.modalShowNoticia$ = this.modalShowNoticia.asObservable();
        this.modalFooter$ = this.modalFooter.asObservable();
        this.dataGuardarFlete$ = this.dataGuardarFlete.asObservable();
        this.modalMsjError$ = this.modalMsjError.asObservable();
        this.modalMsjSuccess$ = this.modalMsjSuccess.asObservable();
    }
    ModalService.prototype.setModalHeader = function (value) {
        this.modalHeader.next(value);
    };
    ModalService.prototype.setmodalInfoLiquidacion = function (value, contrato, secuencia, comprobante) {
        this.modalInfoLiquidacion.next(value);
        this.modalLiquidacionContrato.next(contrato);
        this.modalLiquidacionSecuencia.next(secuencia);
        this.modalLiquidacionComprobante.next(comprobante);
    };
    ModalService.prototype.setmodalShowLiquidacion = function (value) {
        this.modalShowLiquidacion.next(value);
        this.modalShowComprobante.next(false);
        this.modalShowDetalle.next(false);
        this.modalShowFlete.next(false);
        this.modalShowNoticia.next(false);
        this.modalShowFleteProcedencia.next(false);
    };
    ModalService.prototype.setmodalInfoComprobante = function (value) {
        this.modalInfoComprobante.next(value);
    };
    ModalService.prototype.setmodalShowComprobante = function (value) {
        this.modalShowComprobante.next(value);
        this.modalShowLiquidacion.next(false);
        this.modalShowDetalle.next(false);
        this.modalShowFlete.next(false);
        this.modalShowNoticia.next(false);
        this.modalShowFleteProcedencia.next(false);
    };
    ModalService.prototype.setModalShowDetalle = function (value) {
        this.modalShowDetalle.next(value);
        this.modalShowLiquidacion.next(false);
        this.modalShowComprobante.next(false);
        this.modalShowFlete.next(false);
        this.modalShowNoticia.next(false);
        this.modalShowFleteProcedencia.next(false);
    };
    ModalService.prototype.setModalShowFlete = function (value) {
        this.modalShowFlete.next(value);
        this.modalShowLiquidacion.next(false);
        this.modalShowComprobante.next(false);
        this.modalShowDetalle.next(false);
        this.modalShowNoticia.next(false);
        this.modalShowFleteProcedencia.next(false);
    };
    ModalService.prototype.setModalInfoFlete = function (value) {
        this.modalInfoFlete.next(value);
    };
    ModalService.prototype.setModalShowFleteProcedencia = function (value) {
        this.modalShowFleteProcedencia.next(value);
        this.modalShowFlete.next(false);
        this.modalShowLiquidacion.next(false);
        this.modalShowComprobante.next(false);
        this.modalShowDetalle.next(false);
        this.modalShowNoticia.next(false);
    };
    ModalService.prototype.setModalShowEmpresaCambiarEstado = function (value) {
        this.modalShowEmpresaCambiarEstado.next(value);
        this.modalShowFleteProcedencia.next(false);
        this.modalShowFlete.next(false);
        this.modalShowLiquidacion.next(false);
        this.modalShowComprobante.next(false);
        this.modalShowDetalle.next(false);
        this.modalShowNoticia.next(false);
    };
    ModalService.prototype.setModalContratoFleteProcedencia = function (contrato) {
        this.modalContratoFleteProcedencia.next(contrato);
    };
    ModalService.prototype.setModalInfoFleteProcedencia = function (value) {
        this.modalInfoFleteProcedencia.next(value);
    };
    ModalService.prototype.setModalFooter = function (value) {
        this.modalFooter.next(value);
    };
    ModalService.prototype.setModalInfoDetalle = function (value) {
        this.modalInfoDetalle.next(value);
    };
    ModalService.prototype.openModalLiquidacion = function (titulo, listDetalle, contrato, secuencia, comprobante) {
        this.setModalHeader(titulo);
        this.setmodalShowLiquidacion(true);
        this.setmodalInfoLiquidacion(listDetalle, contrato, secuencia, comprobante);
        this.modal.open();
    };
    ModalService.prototype.openModalComprobante = function (titulo, listComprobante) {
        this.setModalHeader(titulo);
        this.setmodalShowComprobante(true);
        this.setmodalInfoComprobante(listComprobante);
        this.modal.open();
    };
    ModalService.prototype.openModalTableResponsive = function (titulo, contrato) {
        this.setModalHeader(titulo);
        this.setModalShowDetalle(true);
        this.setModalInfoDetalle(contrato);
        this.modal.open();
    };
    ModalService.prototype.openModalFlete = function (titulo, viaje) {
        this.setModalHeader(titulo);
        this.setModalShowFlete(true);
        this.setModalInfoFlete(viaje);
        this.modal.open();
    };
    ModalService.prototype.openModalFleteProcedencia = function (titulo, contrato, procedencia) {
        this.setModalHeader(titulo);
        this.setModalShowFleteProcedencia(true);
        this.setModalContratoFleteProcedencia(contrato);
        this.setModalInfoFleteProcedencia(procedencia);
        this.modal.open();
    };
    ModalService.prototype.openModalEmpresaCambiarEstado = function (titulo, empresa, estadoid) {
        this.setModalHeader(titulo);
        this.setModalShowEmpresaCambiarEstado(true);
        //this.setModalEmpresaCambiarEstado(empresa);
        this.modal.open();
    };
    ModalService.prototype.setDatosGuardarFlete = function (data) {
        this.dataGuardarFlete.next(data);
    };
    ModalService.prototype.setErrorMsjModal = function (texto) {
        this.modalMsjError.next(texto);
    };
    ModalService.prototype.setSuccessMsjModal = function (texto) {
        this.modalMsjSuccess.next(texto);
    };
    ModalService.prototype.close = function () {
        this.modal.close();
    };
    ModalService = __decorate([
        Injectable(),
        __metadata("design:paramtypes", [])
    ], ModalService);
    return ModalService;
}());
export { ModalService };
//# sourceMappingURL=ModalService.js.map