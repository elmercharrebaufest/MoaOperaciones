import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Comentario } from './consulta';
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

    public AgregarConsulta(consulta: object) {
        let body = JSON.stringify(consulta);
        let params: URLSearchParams = new URLSearchParams();
        return this.http
            .post('/api/consulta/Consulta', consulta, { search: params, headers: this.headers }).map(this.extractData);
    }

    public getCombos(): Observable<any> {
        return this.http
            .get('/api/consulta/Combos', { headers: this.headers }).pipe(
            map(this.extractData));
    }

    public listarConsultas(): Observable<any> {
        return this.http
            .get('/api/consulta/Consultas', { headers: this.headers }).pipe(
            map(this.extractData));
    }



    public getConsultaDetalle(idConsulta): Observable<any> {
        return this.http
            .get('/api/consulta/Detalle?consultaId=' + idConsulta, { headers: this.headers }).pipe(
            map(this.extractData));
    }

    public agregarComentario(consultaId: string, comentario: Comentario): Observable<any>{
        let body = JSON.stringify(comentario);
        let params: URLSearchParams = new URLSearchParams();
        params.set('consultaId', consultaId.toString());
        return this.http
            .post('/api/consulta/Comentarios', comentario, { search: params, headers: this.headers }).map(this.extractData);
    }

    public getDetalleConsulta(consultaId: string){
        let params: URLSearchParams = new URLSearchParams();
        params.set('consultaId', consultaId.toString());
        return this.http
            .get(`/api/Consulta/Detalle`, { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public actualizarCombos(consultaId: string, estadoId: number, categoriaId: number, subcategoriaId: number = null): Observable<any>{
        let params: URLSearchParams = new URLSearchParams();
        params.set('consultaId', consultaId.toString());
        params.set('estadoConsultaId', estadoId.toString());
        params.set('categoriaId', categoriaId.toString());
        if(subcategoriaId != null){
            params.set('subcategoriaId', subcategoriaId.toString());
        }
        return this.http
            .get(`/api/consulta/ActualizarCombos`, { search: params, headers: this.headers }).pipe(
            map(this.extractData));

    }

    public adjuntar(archivo: FileList, consultaId: string, comentarioId: number): Observable<any> {
        var payload = new FormData();
        payload.append('consultaId', consultaId);
        payload.append('comentarioId', comentarioId.toString());
        payload.append("file", archivo.toString());
        return this.http
            .post('/api/consulta/Adjuntos', payload, this.headersPost).pipe(
            map(this.extractData));
    }

    DescargarArchivo(archivoId: number): Observable<any> {
        this.headers = new Headers();
        this.headers.append("Content-Type", "application/json");
        this.headers.append("Accept", "q=0.8;application/json;q=0.9");
        this.headers.append("Cache-control", "no-cache");
        this.headers.append("Cache-control", "no-store");
        this.headers.append("Expires", "0");
        this.headers.append("Pragma", "no-cache");
        let params: URLSearchParams = new URLSearchParams();
        params.set("archivoId", archivoId.toString());
        return this.http
            .get("/api/consulta/DescargarArchivo", {
                search: params,
                headers: this.headers,
            })
            .pipe(map(this.extractData));
    }
}
