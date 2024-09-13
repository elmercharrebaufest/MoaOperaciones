import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from './../common/services/BaseService';
import { CampoProveedor, CampoProveedorDetalle } from './sustentable';
import { HttpHeaders, HttpParams } from '@angular/common/http';

@Injectable()
export class VentaSustentableService extends BaseService {
    getCamposProveedores() {
        let params: HttpParams = new HttpParams();
        return this.http
            .get('/api/CampoSustentable/CamposProveedores', { params: params, headers: this.headers });
    }

    getCosechasFiltro() {
        return this.getCosechas(true);
    }

    getCosechasCampo() {
        return this.getCosechas(false);
    }

    campoProveedorAgregar(campoProveedor: CampoProveedor, archivoKmz: File, UsarArchivo_Id: boolean) {
        var payload = new FormData();
        let camp = JSON.stringify(campoProveedor);

        payload.append('archivoKmz', archivoKmz);
        payload.append('campoProveedorJson', camp);
        payload.append('UsarArchivoId', UsarArchivo_Id.toString());
        return this.http
            .post('/api/CampoSustentable/CampoProveedorAgregar', payload, { headers: this.headersPost });
    }

    campoProveedorEditar(campoProveedor: CampoProveedor, archivoKmz: File) {
        var payload = new FormData();
        let camp = JSON.stringify(campoProveedor);

        payload.append('archivoKmz', archivoKmz);
        payload.append('campoProveedorJson', camp);
        return this.http
            .post('/api/CampoSustentable/CampoProveedorEditar', payload, { headers: this.headersPost });
    }

    public getProveedor(codigo: string) {
        let params: HttpParams = new HttpParams();
        params = params.append("codigo", codigo);
        return this.http.get("/api/Usuario/GetProveedorPorCodigo", { params: params, headers: this.headers, });
    }

    searchLocalidad(term: string): Observable<any> {
        let headers = new HttpHeaders();
        headers = headers.append("Content-Type", "application/json");
        headers = headers.append("Accept", "q=0.8;application/json;q=0.9");
        headers = headers.append("Cache-control", "no-cache");
        headers = headers.append("Cache-control", "no-store");
        headers = headers.append("Expires", "0");
        headers = headers.append("Pragma", "no-cache");

        let params: HttpParams = new HttpParams();
        params = params.append("localidad", term);

        return this.http
            .get("/api/AltaEmpresaGranos/GetLocalidadCombo", {
                params: params,
                headers: headers,
            });
    }

    campoProveedorBorrar(campoCosechaId: number, proveedorId: number) {
        var payload = new FormData();
        payload.append("proveedorId", proveedorId.toString());
        payload.append("campoCosechaId", campoCosechaId.toString());

        return this.http
            .post('/api/CampoSustentable/CampoProveedorBorrar', payload, { headers: this.headersPost, });
    }

    getCampoProveedor(proveedorId: any, campoCosechaId: any): Observable<CampoProveedorDetalle> {
        let params: HttpParams = new HttpParams();
        params = params.append("proveedorId", proveedorId);
        params = params.append("campoCosechaId", campoCosechaId)

        return this.http
            .get<CampoProveedorDetalle>('/api/CampoSustentable/CampoProveedor', { params: params, headers: this.headers });
    }

    verificarDeclaracion(proveedorId: number, cosechaId: number, CUIT: string) {

        let params: HttpParams = new HttpParams();
        params = params.append("proveedorId", proveedorId.toString());
        params = params.append("cosechaId", cosechaId.toString());
        params = params.append("CUITDeclaracion", CUIT)

        return this.http
            .get('/api/CampoSustentable/VerificarDeclaracion', { params: params, headers: this.headers });
    }
    generarDeclaracionProveedor(proveedorId: number, cosechaId: number, hectareasTotales: number, CUIT: string, RazonSocial: string) {
        return this.http
            .post('/api/CampoSustentable/GenerarDeclaracionProveedor', {
                proveedorId: proveedorId,
                cosechaId: cosechaId,
                hectareasTotales: hectareasTotales,
                CUITDeclaracion: CUIT,
                RazonSocialDeclaracion: RazonSocial
            }, { headers: this.headersPost });
    }

    imprimirDeclaracion(proveedorId: number, cosechaId: number, CUIT: string) {
        let params: HttpParams = new HttpParams();
        params = params.append("proveedorId", proveedorId.toString());
        params = params.append("cosechaId", cosechaId.toString());
        params = params.append("CUIT", CUIT);

        return this.http
            .get('/api/CampoSustentable/ImprimirDeclaracion', { params: params, headers: this.headers });
    }

    adjuntarDeclaracionFirmada(proveedorId: number, cosechaId: number, CUITDeclaracion: string, fileSubido: File) {
        var payload = new FormData();

        payload.append('proveedorId', proveedorId.toString());
        payload.append('cosechaId', cosechaId.toString());
        payload.append('CUITDeclaracion', CUITDeclaracion);
        payload.append('fileSubido', fileSubido);
        return this.http
            .post('/api/CampoSustentable/AdjuntarDeclaracionFirmada', payload, { headers: this.headersPost });
    }

    exportExcel() {
        return this.http
            .get('/api/CampoSustentable/ExportarCamposProveedores');
    }

    borrarCampoSustentable(proveedorId: number, cosechaId: number) {
        let params: HttpParams = new HttpParams();
        params = params.set("proveedorId", proveedorId.toString());
        params = params.set("cosechaId", cosechaId.toString());
        return this.http
            .get('/api/CampoSustentable/BorrarCampoSustentable', { params: params, headers: this.headers });
        // .pipe(map(this.extractData)

    }

    descargarArchivoKMZ(campoCosechaId: number, proveedorId: number): Observable<any> {


        let params: HttpParams = new HttpParams();
        params = params.set("campoCosechaId", campoCosechaId.toString());
        params = params.set("proveedorId", proveedorId.toString());

        return this.http
            .get('/api/CampoSustentable/DescargarArchivoKMZ', { params: params, headers: this.headers })
        // .pipe(map(this.extractData));
    }

    private getCosechas(incluirInactivas: boolean) {
        let params: HttpParams = new HttpParams();
        params = params.append("incluirInactivas", incluirInactivas.toString());
        return this.http
            .get('/api/CampoSustentable/Cosechas', { params: params, headers: this.headers });
    }
}