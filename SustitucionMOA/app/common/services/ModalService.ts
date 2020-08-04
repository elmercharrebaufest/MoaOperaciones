import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class ModalService {

    constructor() { }

    public modal: any;

    public modalHeader = new Subject<any>();
    public modalInfoLiquidacion = new Subject<any>();
    public modalLiquidacionContrato = new Subject<any>();
    public modalLiquidacionSecuencia = new Subject<any>();
    public modalLiquidacionComprobante = new Subject<any>();
    public modalShowLiquidacion = new Subject<any>();
    public modalInfoComprobante = new Subject<any>();
    public modalShowComprobante = new Subject<any>();
    public modalInfoDetalle = new Subject<any>();
    public modalShowDetalle = new Subject<any>();
    public modalInfoFlete = new Subject<any>();
    public modalShowFlete = new Subject<any>();
    public modalContratoFleteProcedencia = new Subject<any>();
    public modalInfoFleteProcedencia = new Subject<any>();
    public modalShowFleteProcedencia = new Subject<any>();
    public modalShowNoticia = new Subject<any>();
    public modalFooter = new Subject<any>();
    public dataGuardarFlete = new Subject<any>();
    public modalMsjError = new Subject<any>();
    public modalMsjSuccess = new Subject<any>();

    modalHeader$ = this.modalHeader.asObservable();
    modalInfoLiquidacion$ = this.modalInfoLiquidacion.asObservable();
    modalLiquidacionContrato$ = this.modalLiquidacionContrato.asObservable();
    modalLiquidacionSecuencia$ = this.modalLiquidacionSecuencia.asObservable();
    modalLiquidacionComprobante$ = this.modalLiquidacionComprobante.asObservable();
    modalShowLiquidacion$ = this.modalShowLiquidacion.asObservable();
    modalInfoComprobante$ = this.modalInfoComprobante.asObservable();
    modalShowComprobante$ = this.modalShowComprobante.asObservable();
    modalInfoDetalle$ = this.modalInfoDetalle.asObservable();
    modalShowDetalle$ = this.modalShowDetalle.asObservable();
    modalInfoFlete$ = this.modalInfoFlete.asObservable();
    modalShowFlete$ = this.modalShowFlete.asObservable();
    modalContratoFleteProcedencia$ = this.modalContratoFleteProcedencia.asObservable();
    modalInfoFleteProcedencia$ = this.modalInfoFleteProcedencia.asObservable();
    modalShowFleteProcedencia$ = this.modalShowFleteProcedencia.asObservable();
    modalShowNoticia$ = this.modalShowNoticia.asObservable();
    modalFooter$ = this.modalFooter.asObservable();
    dataGuardarFlete$ = this.dataGuardarFlete.asObservable();
    modalMsjError$ = this.modalMsjError.asObservable();
    modalMsjSuccess$ = this.modalMsjSuccess.asObservable();

    setModalHeader(value: any) {
        this.modalHeader.next(value);
    }

    setmodalInfoLiquidacion(value: any, contrato: string, secuencia: string, comprobante: string) {
        this.modalInfoLiquidacion.next(value);
        this.modalLiquidacionContrato.next(contrato);
        this.modalLiquidacionSecuencia.next(secuencia);
        this.modalLiquidacionComprobante.next(comprobante);
    }

    setmodalShowLiquidacion(value: any) {
        this.modalShowLiquidacion.next(value);
        this.modalShowComprobante.next(false);
        this.modalShowDetalle.next(false);
        this.modalShowFlete.next(false);
        this.modalShowNoticia.next(false);
        this.modalShowFleteProcedencia.next(false);
    }

    setmodalInfoComprobante(value: any) {
        this.modalInfoComprobante.next(value);
    }

    setmodalShowComprobante(value: any) {
        this.modalShowComprobante.next(value);
        this.modalShowLiquidacion.next(false);
        this.modalShowDetalle.next(false);
        this.modalShowFlete.next(false);
        this.modalShowNoticia.next(false);
        this.modalShowFleteProcedencia.next(false);
    }

    setModalShowDetalle(value: any) {
        this.modalShowDetalle.next(value);
        this.modalShowLiquidacion.next(false);
        this.modalShowComprobante.next(false);
        this.modalShowFlete.next(false);
        this.modalShowNoticia.next(false);
        this.modalShowFleteProcedencia.next(false);
    }

    setModalShowFlete(value: any) {
        this.modalShowFlete.next(value);
        this.modalShowLiquidacion.next(false);
        this.modalShowComprobante.next(false);
        this.modalShowDetalle.next(false);
        this.modalShowNoticia.next(false);
        this.modalShowFleteProcedencia.next(false);
    }

    setModalInfoFlete(value: any) {
        this.modalInfoFlete.next(value);
    }

    setModalShowFleteProcedencia(value: any) {
        this.modalShowFleteProcedencia.next(value);
        this.modalShowFlete.next(false);
        this.modalShowLiquidacion.next(false);
        this.modalShowComprobante.next(false);
        this.modalShowDetalle.next(false);
        this.modalShowNoticia.next(false);
    }

    setModalContratoFleteProcedencia(contrato: string) {
        this.modalContratoFleteProcedencia.next(contrato);
    }

    setModalInfoFleteProcedencia(value: any) {
        this.modalInfoFleteProcedencia.next(value);
    }

    setModalFooter(value: any) {
        this.modalFooter.next(value);
    }

    setModalInfoDetalle(value: any) {
        this.modalInfoDetalle.next(value);
    }

    openModalLiquidacion(titulo: string, listDetalle: any, contrato: string, secuencia: string, comprobante: string) {
        this.setModalHeader(titulo);
        this.setmodalShowLiquidacion(true);
        this.setmodalInfoLiquidacion(listDetalle, contrato, secuencia, comprobante);
        this.modal.open();
    }

    openModalComprobante(titulo: string, listComprobante: any) {
        this.setModalHeader(titulo);
        this.setmodalShowComprobante(true);
        this.setmodalInfoComprobante(listComprobante);
        this.modal.open();
    }

    openModalTableResponsive(titulo: string, contrato: any) {
        this.setModalHeader(titulo);
        this.setModalShowDetalle(true);
        this.setModalInfoDetalle(contrato);
        this.modal.open();
    }

    openModalFlete(titulo: string, viaje: any) {
        this.setModalHeader(titulo);
        this.setModalShowFlete(true);
        this.setModalInfoFlete(viaje);
        this.modal.open();
    }

    openModalFleteProcedencia(titulo: string, contrato: string, procedencia : any) {
        this.setModalHeader(titulo);
        this.setModalShowFleteProcedencia(true);
        this.setModalContratoFleteProcedencia(contrato);
        this.setModalInfoFleteProcedencia(procedencia);
        this.modal.open();
    }

    setDatosGuardarFlete(data: string) {
        this.dataGuardarFlete.next(data);
    }

    setErrorMsjModal(texto: string) {
        this.modalMsjError.next(texto);
    }

    setSuccessMsjModal(texto: string) {
        this.modalMsjSuccess.next(texto);
    }

    close() {
        this.modal.close();
    }
}