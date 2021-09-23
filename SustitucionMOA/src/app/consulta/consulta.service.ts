import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from './../common/services/BaseService';
import { Comentario } from './consulta';
import { map } from 'rxjs/operators';
import { HttpHeaders, HttpParams } from '@angular/common/http';

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
            .post('/api/contactoMail/sendContactoMail', payload, { headers: this.headersPost });
    }

    public AgregarConsulta(consulta: object, comentario: Comentario, archivo: any  = null) {
        let consultaJson = JSON.stringify(consulta);
        let comentarioJson = JSON.stringify(comentario);
        var payload = new FormData();

        if(archivo != null)
        {
            for (let i = 0; i < archivo.length; i++) {
                let fileToUpload = archivo[i];
                payload.append("file", fileToUpload, fileToUpload.name);
            }
        }

        payload.append('consultaJson', consultaJson);
        payload.append('comentarioJson', comentarioJson);
        payload.append("file", archivo);
        
        return this.http
            .post('/api/consulta/Consulta',  payload , {headers: this.headers});
    }

    public recordarComentario(consultaId: any): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set('consultaId', consultaId.toString());

        return this.http
            .get(`/api/Consulta/RecordarComentario`, { params: params, headers: this.headers });
    }

    public getCombos(excluir: boolean): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set('excluir', excluir.toString());

        return this.http
            .get('/api/consulta/Combos', { params: params, headers: this.headers });
    }

    public listarConsultas(): Observable<any> {
        return this.http
            .get('/api/consulta/Consultas', { headers: this.headers });
    }

    public getConsultaDetalle(idConsulta): Observable<any> {
        return this.http
            .get('/api/consulta/Detalle?consultaId=' + idConsulta, { headers: this.headers });
    }

    public agregarComentario(consultaId: string, comentario: Comentario, archivo: any = null): Observable<any>{
        let comentarioJson = JSON.stringify(comentario);
        var payload = new FormData();
        
        if(archivo != null)
        {
            for (let i = 0; i < archivo.length; i++) {
                let fileToUpload = archivo[i];
                payload.append("file", fileToUpload, fileToUpload.name);
            }
        }

        payload.append('comentarioJson', comentarioJson);
        payload.append('consultaId', consultaId.toString());
        payload.append("file", archivo);

        return this.http
            .post('/api/consulta/Comentarios', payload, { headers: this.headers });
    }

    public getDetalleConsulta(consultaId: string){
        let params: HttpParams = new HttpParams();
        params = params.set('consultaId', consultaId.toString());

        return this.http
            .get(`/api/Consulta/Detalle`, { params: params, headers: this.headers });
    }

    public actualizarCombos(consultaId: string, estadoId: number, categoriaId: number, subcategoriaId: number = null, causaConsultaId: number = null): Observable<any>{
        let params: HttpParams = new HttpParams();
        params = params.append('consultaId', consultaId.toString());
        params = params.append('estadoConsultaId', estadoId.toString());
        params = params.append('categoriaId', categoriaId.toString());
        params = params.append('causaConsultaId', causaConsultaId.toString())
        if(subcategoriaId != null){
            params = params.append('subcategoriaId', subcategoriaId.toString());
        }
        return this.http
            .get(`/api/consulta/ActualizarCombos`, { params: params, headers: this.headers });

    }

    public adjuntar(archivo: any, consultaId: string, comentarioId: number): Observable<any> {
        var payload = new FormData();
        payload.append('consultaId', consultaId);
        payload.append('comentarioId', comentarioId.toString());
        payload.append("file", archivo);

        return this.http
            .post('/api/consulta/Adjuntos', payload, { headers: this.headersPost });
    }

    public actualizarEstado(estadoId: number, consultaId: string) {
        var payload = new FormData();
        payload.append('consultaId', consultaId);
        payload.append('estadoConsultaId', estadoId.toString());

        return this.http
            .post('/api/consulta/ActualizarEstado', payload, { headers: this.headersPost });
    }

    DescargarArchivo(archivoId: number): Observable<any> {
        let headers = new HttpHeaders();
        headers = headers.append("Content-Type", "application/json");
        headers = headers.append("Accept", "q=0.8;application/json;q=0.9");
        headers = headers.append("Cache-control", "no-cache");
        headers = headers.append("Cache-control", "no-store");
        headers = headers.append("Expires", "0");
        headers = headers.append("Pragma", "no-cache");

        let params: HttpParams = new HttpParams();
        params = params.append("archivoId", archivoId.toString());

        return this.http
            .get("/api/consulta/DescargarArchivo", {
                params: params,
                headers: headers,
            });
    }

    obtenerMateriales(): Observable<any> {
        let headers = new HttpHeaders();
        headers = headers.append("Content-Type", "application/json");
        headers = headers.append("Accept", "q=0.8;application/json;q=0.9");
        headers = headers.append("Cache-control", "no-cache");
        headers = headers.append("Cache-control", "no-store");
        headers = headers.append("Expires", "0");
        headers = headers.append("Pragma", "no-cache");

        return this.http
            .get("/api/AltaEmpresaGranos/GetMateriales", {
                headers: headers,
            });
    }

    generarReclamoImpositivo(reclamoImpositivo: any){
        let headers = new HttpHeaders();
        headers = headers.append("Content-Type", "application/json");
        headers = headers.append("Accept", "q=0.8;application/json;q=0.9");
        headers = headers.append("Cache-control", "no-cache");
        headers = headers.append("Cache-control", "no-store");
        headers = headers.append("Expires", "0");
        headers = headers.append("Pragma", "no-cache");

        let params: HttpParams = new HttpParams();
        let reclamoImpositivoJson = JSON.stringify(reclamoImpositivo);
        params = params.append("reclamoImpositivoJson", reclamoImpositivoJson);
        
        return this.http
            .get("/api/consulta/GenerarReclamoImpositivoPdf", {
                params: params,
                headers: headers,
            });
    }

    AnularConsulta(consultaId: number, motivoRechazo: string) {
        var payload = new FormData();
        payload.append('consultaId', consultaId.toString());
        payload.append('motivoRechazo', motivoRechazo);

        return this.http
            .post('/api/consulta/AnularConsulta', payload, this.headersPost).pipe(
                map(this.extractData));
    }
}
