
import {throwError as observableThrowError,  Observable } from 'rxjs';
import {timeoutWith, map} from 'rxjs/operators';
import { Injectable } from '@angular/core';

import { BaseService } from './../common/services/BaseService';
import { ContratoAPrecio } from '../common/models/contratoAPrecio';
import { ContratoAFijar } from '../common/models/contratoAFijar';
import { ContratoFijacion } from '../common/models/contratoFijacion';
import { HttpHeaders, HttpParams } from '@angular/common/http';

@Injectable()
export class CrearContratoService extends BaseService {

    private setHeaders(){
        this.headers = new HttpHeaders();
        this.headers = this.headers.append('Content-Type', 'application/json');
        this.headers = this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers = this.headers.append('Cache-control', 'no-cache');
        this.headers = this.headers.append('Cache-control', 'no-store');
        this.headers = this.headers.append('Expires', '0');
        this.headers = this.headers.append('Pragma', 'no-cache');
    }

    obteneDatosContrato(tiponegocio: number): Observable<any> {
        this.setHeaders();

        let params: HttpParams = new HttpParams();
        params = params.append('tiponegocio', tiponegocio.toString());

        return this.http
            .get('/api/CrearContrato/ObteneDatosContrato', { params: params,  headers: this.headers });
    }

    obtenerDatosCompraNet(idProveedorDataAgro): Observable<any> {
        this.setHeaders();

        let params: HttpParams = new HttpParams();
        params = params.append('idProveedorDataAgro', idProveedorDataAgro);

        return this.http
            .get('/api/CrearContrato/ObtenerDatosCompraNet', { params: params, headers: this.headers });
    }

    searchLocalidad(term): Observable<any> {
        this.setHeaders();

        let params: HttpParams = new HttpParams();
        params = params.append('localidad', term);

        return this.http.get('/api/CrearContrato/GetLocalidadCombo', { params: params, headers: this.headers });
    }

    grabarContratoAPrecio(contrato: ContratoAPrecio): Observable<any> {
        let payload = new FormData();
        payload.append("contrato", JSON.stringify(contrato));
        return this.http
            .post('/api/CrearContrato/CrearContratoAPrecio', payload)
            .pipe(timeoutWith(90000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    grabarContratoAFijar(contrato: ContratoAFijar): Observable<any> {
        let payload = new FormData();
        payload.append("contrato", JSON.stringify(contrato));
        return this.http
            .post('/api/CrearContrato/CrearContratoAFijar', payload)
            .pipe(timeoutWith(90000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    buscarProveedoresConCorredor(term): Observable<any> {
        this.setHeaders();

        let params: HttpParams = new HttpParams();
        params = params.append('filtro', term);

        return this.http.get('/api/CrearContrato/BuscarProveedoresConCorredor', { params: params, headers: this.headers });
    }

    validarDirecto(): Observable<any> {
        this.setHeaders();

        return this.http
            .get('/api/CrearContrato/ValidarDirecto', { headers: this.headers });
    }

    grabarContratoFijacion(contrato: ContratoFijacion): Observable<any> {
        let payload = new FormData();
        payload.append("contrato", JSON.stringify(contrato));
        return this.http
            .post('/api/CrearContrato/CrearContratoFijacion', payload)
            .pipe(timeoutWith(90000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    habilitaciones(material: number, tiponegocio: number): Observable<any> {
        this.setHeaders();

        let params: HttpParams = new HttpParams();
        params = params.append('material', material.toString());
        params = params.append('tiponegocio', tiponegocio.toString());

        return this.http
            .get('/api/CrearContrato/Habilitaciones', { params: params, headers: this.headers });
    }

    obtenerFijacionesAutomaticas(esCorredorEnDataAgro, cuitProveedor, materialId, filtro): Observable<any> {
        this.setHeaders();

        let params: HttpParams = new HttpParams();
        params = params.append('esCorredorEnDataAgro', esCorredorEnDataAgro);
        params = params.append('cuitProveedor', cuitProveedor);
        params = params.append('materialId', materialId);
        params = params.append('filtro', filtro);

        return this.http.get('/api/CrearContrato/ObtenerFijacionesAutomaticas', { params: params, headers: this.headers });
    }

    validarProveedor(proveedorId): Observable<any> {
        this.setHeaders();

        let params: HttpParams = new HttpParams();
        params = params.append('proveedorId', proveedorId);
        return this.http.get('/api/CrearContrato/ValidarProveedor', { params: params, headers: this.headers });
    }

    traerPrecioMoaMateriales(tipoNegocioId): Observable<any> {
        this.setHeaders();

        let params: HttpParams = new HttpParams();
        params = params.append('tipoNegocioId', tipoNegocioId);
        return this.http.get('/api/CrearContrato/TraerPrecioMoaMateriales', { params: params, headers: this.headers });
    }
    ObteneContratosAcuerdo(): Observable<any> {
        this.setHeaders();

        return this.http
            .get('/api/CrearContrato/ObteneContratosAcuerdo', { headers: this.headers });
    }

    AltaMasivaAcuerdo(
        files: FileList,
        contratoAcuerdo: string
    ): Observable<any> {
        let formData = new FormData();

        for (let i = 0; i < files.length; i++) {
            let fileToUpload = files.item(i);
            formData.append("file", fileToUpload, fileToUpload.name);
        }

        formData.append("contratoAcuerdo", contratoAcuerdo.toString());

        return this.http
            .post("/api/CrearContrato/AltaMasivaAcuerdo", formData)
            .pipe(timeoutWith(120000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    traerContratoCompleto(negocioId, tipoNegocioId): Observable<any> {
        this.setHeaders();

        let params: HttpParams = new HttpParams();
        params = params.append('negocioId', negocioId);
        params = params.append('tipoNegocioId', tipoNegocioId);

        return this.http.get('/api/CrearContrato/TraerContratoCompleto', { params: params, headers: this.headers });
    }

    excelModeloAltaMasiva() {
        return this.http
            .get('/api/CrearContrato/ExcelModeloAltaMasiva', { headers: this.headers });
    }
}

@Injectable()
export class CrearContratoAPrecioService extends CrearContratoService {

}

@Injectable()
export class CrearContratoAFijarService extends CrearContratoService {
    
}

@Injectable()
export class CrearContratoFijacionService extends CrearContratoService {
 
}
@Injectable()
export class CrearContratoCargarNegocioService extends CrearContratoService {

}

@Injectable()
export class CrearContratoAltaMasivaService extends CrearContratoService {
   
}
