import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { identifierModuleUrl } from '@angular/compiler';
import { getLocaleDateTimeFormat } from '@angular/common';

@Injectable()
export class ConsultaService extends BaseService {
    public sendContactoMail(
        proveedor: string,
        nombre: string,
        email: string,
        telefono: string,
        categoria: string,
        camposAdicionales: string,
        comentario: string,
        contrato: string,
        razonSocial: string,
        cuit: string,
        nombreVendedor: string,
        comprobante: string,
        fechaPago: string,
        importe: string,
        impuesto: string,
        inscripcion: string,
        motivo: string,
        archivo: any
    ): Observable<any> {
        var payload = new FormData();
        var data = {
            proveedor: proveedor, nombre: nombre, email: email, telefono: telefono, categoria: categoria, camposAdicionales: camposAdicionales, comentario: comentario, contrato: contrato, razonSocial: razonSocial,
            cuit: cuit, nombreVendedor: nombreVendedor, comprobante: comprobante, fechaPago: fechaPago, importe: importe, importeDecimal: 0, impuesto: impuesto, inscripcion: inscripcion, motivo: motivo
        }
        payload.append("contacto", JSON.stringify(data));
        payload.append("file", archivo);
        return this.http
            .post('/api/contactoMail/sendContactoMail', payload, this.headersPost).pipe(
            map(this.extractData));
    }


    public getCategorias(): Observable<any> {
        return this.http
            .get('/api/contactoMail/getCategorias', { headers: this.headers }).pipe(
            map(this.extractData));
    }

    public getEstados(): Observable<any> {
        return this.http
            .get('/api/contactoMail/getEstados', { headers: this.headers }).pipe(
            map(this.extractData));
    }

    public listarConsultas(): Observable<any> {
        return this.http
            .get('/api/contactoMail/listarConsultas', { headers: this.headers }).pipe(
            map(this.extractData));
    }

    public getConsultaDetalle(idConsulta): Observable<any> {
        return this.http
            .get('/api/consulta/Detalle?consultaId=' + idConsulta, { headers: this.headers }).pipe(
            map(this.extractData));
    }
}

    public getDetalleConsulta(consultaId){
        let params: URLSearchParams = new URLSearchParams();
        params.set('consultaId', consultaId.toString());
        return this.http
            .get('/api/consulta/Detalle', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public agregarComentario(consultaId: number, detalle: string, usuarioId: number){
        var comentario = {detalle: detalle, fecha: new Date(), consultaId: consultaId, usuarioId: usuarioId}
        return this.http
            .post(`/api/consulta/${consultaId}/Comentarios`, comentario, this.headersPost).map(this.extractData);
    }
}
