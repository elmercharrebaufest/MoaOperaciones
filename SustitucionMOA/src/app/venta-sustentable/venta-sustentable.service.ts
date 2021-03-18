import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { CampoProveedor } from './sustentable';

@Injectable()
export class VentaSustentableService extends BaseService {
    getCamposProveedores() {
        let params: URLSearchParams = new URLSearchParams();
        return this.http
            .get('/api/CampoSustentable/CamposProveedores', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }

    getCosechas() {
        let params: URLSearchParams = new URLSearchParams();
        return this.http
            .get('/api/CampoSustentable/Cosechas', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }

    campoProveedorAgregar(campoProveedor: CampoProveedor, archivoKmz: File){
        var payload = new FormData();
        let camp = JSON.stringify(campoProveedor);
        
        payload.append('archivoKmz', archivoKmz);
        payload.append('campoProveedorJson', camp)
        return this.http
            .post('/api/CampoSustentable/CampoProveedorAgregar', payload, this.headersPost).pipe(
            map(this.extractData));
    }

    public getProveedor(codigo: string){
        let params: URLSearchParams = new URLSearchParams();
        params.set("codigo", codigo);
        return this.http.get("/api/Usuario/GetProveedorPorCodigo", {search: params, headers: this.headers,})
          .pipe(map(this.extractData));
      }

    searchLocalidad(term): Observable<any> {
        this.headers = new Headers();
        this.headers.append("Content-Type", "application/json");
        this.headers.append("Accept", "q=0.8;application/json;q=0.9");
        this.headers.append("Cache-control", "no-cache");
        this.headers.append("Cache-control", "no-store");
        this.headers.append("Expires", "0");
        this.headers.append("Pragma", "no-cache");

        let params: URLSearchParams = new URLSearchParams();
        params.set("localidad", term);

        return this.http
            .get("/api/AltaEmpresaGranos/GetLocalidadCombo", {
                search: params,
                headers: this.headers,
            })
            .pipe(map(this.extractData));
    }

    campoProveedorBorrar(campoCosechaId: number, proveedorId: number){
        let params: URLSearchParams = new URLSearchParams();
        params.set('campoCosechaId', campoCosechaId.toString());
        params.set('proveedorId', proveedorId.toString());
        return this.http
            .delete('/api/CampoSustentable/CampoProveedorBorrar',{search: params, headers: this.headers,}).pipe(
            map(this.extractData));
    }

    verificarDeclaracion(proveedorId: number) {
        let params: URLSearchParams = new URLSearchParams();
        params.set("proveedorId", proveedorId.toString());
        return this.http
            .get('/api/CampoSustentable/VerificarDeclaracion', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }

    firmarDeclaracion(proveedorId: number, hectareasTotales: number) {
        return this.http
            .post('/api/CampoSustentable/FirmarDeclaracion', { proveedorId: proveedorId, hectareasTotales: hectareasTotales }, this.headersPost).pipe(
                map(this.extractData));
    }


    imprimirDeclaracion(proveedorId: number) {
        let params: URLSearchParams = new URLSearchParams();
        params.set("proveedorId", proveedorId.toString());
        return this.http
            .get('/api/CampoSustentable/ImprimirDeclaracion', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }
}