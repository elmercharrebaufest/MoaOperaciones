import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from './../common/services/BaseService';
import { Comentario, Destinatario } from './consulta';
import { map } from 'rxjs/operators';
import { HttpHeaders, HttpParams } from '@angular/common/http';
import { OrdenDeCarga } from '../common/models/ordenes-de-carga/ordenDeCarga';

@Injectable()
export class ConsultaService extends BaseService {

    private ordenDeCarga: any;

    public AgregarConsulta(consulta: object, comentario: Comentario, archivo: any = null) {
        let consultaJson = JSON.stringify(consulta);
        let comentarioJson = JSON.stringify(comentario);
        var payload = new FormData();

        if (archivo != null) {
            for (let i = 0; i < archivo.length; i++) {
                let fileToUpload = archivo[i];
                payload.append("file", fileToUpload, fileToUpload.name);
            }
        }

        payload.append('consultaJson', consultaJson);
        payload.append('comentarioJson', comentarioJson);
        payload.append("file", archivo);

        return this.http
            .post('/api/consulta/Consulta', payload, { headers: this.headers });
    }

    public recordarComentario(consultaId: any): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set('consultaId', consultaId.toString());

        return this.http
            .get(`/api/Consulta/RecordarComentario`, { params: params, headers: this.headers });
    }

    public getCombos(excluir: boolean, mostrarCategoriaInterno: boolean): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set('excluir', excluir.toString());
        params = params.set('mostrarCategoriaInterno', mostrarCategoriaInterno.toString());

        return this.http
            .get('/api/consulta/Combos', { params: params, headers: this.headers });
    }

    public getCombosConsultaInterna(ordenId: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set('ordenId', ordenId.toString());
        return this.http
            .get('/api/consulta/CombosConsultaInterna', { params: params, headers: this.headers });
    }

    public getDestinatariosFas(ordenId: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set('ordenId', ordenId.toString());
        return this.http
            .get('/api/consulta/GetDestinatariosConsultaFas', { params: params, headers: this.headers });
    }

    public getDestinatarios(ordenId: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set('ordenId', ordenId.toString());
        return this.http
            .get('/api/consulta/GetDestinatariosConsulta', { params: params, headers: this.headers });
    }

    public getVendedoresUsuario(idUsuario: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set('idUsuario', idUsuario.toString());
        return this.http
            .get('/api/consulta/GetVendedoresUsuario', { params: params, headers: this.headers });
    }

    public listarConsultas(): Observable<any> {
        return this.http
            .get('/api/consulta/Consultas', { headers: this.headers });
    }

    public getConsultaDetalle(idConsulta): Observable<any> {
        return this.http
            .get('/api/consulta/Detalle?consultaId=' + idConsulta, { headers: this.headers });
    }

    public agregarComentario(consultaId: string, comentario: Comentario, archivo: any = null): Observable<any> {
        let comentarioJson = JSON.stringify(comentario);
        var payload = new FormData();

        if (archivo != null) {
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

    public getDetalleConsulta(consultaId: string) {
        let params: HttpParams = new HttpParams();
        params = params.set('consultaId', consultaId.toString());

        return this.http
            .get(`/api/Consulta/Detalle`, { params: params, headers: this.headers });
    }

    public actualizarCombos(consultaId: string, estadoId: number, categoriaId: number, subcategoriaId: number = null, causaConsultaId: number = null): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('consultaId', consultaId.toString());
        params = params.append('estadoConsultaId', estadoId.toString());
        params = params.append('categoriaId', categoriaId.toString());
        params = params.append('causaConsultaId', causaConsultaId.toString())
        if (subcategoriaId != null) {
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

    public reabrirConsulta(consultaId: number) {
        var payload = new FormData();
        payload.append('consultaId', consultaId.toString());

        return this.http
            .post('/api/consulta/ReabrirConsulta', payload, { headers: this.headersPost });
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

    generarReclamoImpositivo(reclamoImpositivo: any) {
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
            .post('/api/consulta/AnularConsulta', payload, { headers: this.headersPost })
    }

    public AgregarConsultaInterna(consulta: object, comentario: Comentario, archivo: any = null, destinatariosFas: Destinatario[]) {
        let consultaJson = JSON.stringify(consulta);
        let comentarioJson = JSON.stringify(comentario);
        let destinatariosFasJson = JSON.stringify(destinatariosFas);
        var payload = new FormData();

        if (archivo != null) {
            for (let i = 0; i < archivo.length; i++) {
                let fileToUpload = archivo[i];
                payload.append("file", fileToUpload, fileToUpload.name);
            }
        }

        payload.append('consultaJson', consultaJson);
        payload.append('comentarioJson', comentarioJson);
        payload.append('destinatariosFasJson', destinatariosFasJson);
        payload.append("file", archivo);

        return this.http
            .post('/api/consulta/AgregarConsultaInterna', payload, { headers: this.headers });
    }
}
