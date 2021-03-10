
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {timeoutWith, map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';


import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';
import { ContratoAPrecio } from '../common/models/contratoAPrecio';
import { ContratoAFijar } from '../common/models/contratoAFijar';
import { ContratoFijacion } from '../common/models/contratoFijacion';


@Injectable()
export class CrearContratoService extends BaseService {
    //grabarContratoAltaMasiva(contratoAcuerdo: string, adjunto: FileList) {
    //    throw new Error("Method not implemented.");
    //}

    obteneDatosContrato(tiponegocio: number): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        let params: URLSearchParams = new URLSearchParams();
        params.set('tiponegocio', tiponegocio.toString());

        return this.http
            .get('/api/CrearContrato/ObteneDatosContrato', { search: params,  headers: this.headers })
            .pipe(map(this.extractData));
    }

    obtenerDatosCompraNet(idProveedorDataAgro): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        let params: URLSearchParams = new URLSearchParams();
        params.set('idProveedorDataAgro', idProveedorDataAgro);

        return this.http
            .get('/api/CrearContrato/ObtenerDatosCompraNet', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }

    searchLocalidad(term): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        let params: URLSearchParams = new URLSearchParams();
        params.set('localidad', term);

        return this.http.get('/api/CrearContrato/GetLocalidadCombo', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }

    grabarContratoAPrecio(contrato: ContratoAPrecio): Observable<any> {
        let payload = new FormData();
        payload.append("contrato", JSON.stringify(contrato));
        return this.http
            .post('/api/CrearContrato/CrearContratoAPrecio', payload)
            .pipe(timeoutWith(90000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    grabarContratoAFijar(contrato: ContratoAFijar): Observable<any> {
        let payload = new FormData();
        payload.append("contrato", JSON.stringify(contrato));
        return this.http
            .post('/api/CrearContrato/CrearContratoAFijar', payload)
            .pipe(timeoutWith(90000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    buscarProveedoresConCorredor(term): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        let params: URLSearchParams = new URLSearchParams();
        params.set('filtro', term);

        return this.http.get('/api/CrearContrato/BuscarProveedoresConCorredor', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }

    validarDirecto(): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        return this.http
            .get('/api/CrearContrato/ValidarDirecto', { headers: this.headers })
            .pipe(map(this.extractData));
    }

    grabarContratoFijacion(contrato: ContratoFijacion): Observable<any> {
        let payload = new FormData();
        payload.append("contrato", JSON.stringify(contrato));
        return this.http
            .post('/api/CrearContrato/CrearContratoFijacion', payload)
            .pipe(timeoutWith(90000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    habilitaciones(material: number, tiponegocio: number): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        let params: URLSearchParams = new URLSearchParams();
        params.set('material', material.toString());
        params.set('tiponegocio', tiponegocio.toString());

        return this.http
            .get('/api/CrearContrato/Habilitaciones', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }

    obtenerFijacionesAutomaticas(esCorredorEnDataAgro, cuitProveedor, materialId, filtro): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        let params: URLSearchParams = new URLSearchParams();
        params.set('esCorredorEnDataAgro', esCorredorEnDataAgro);
        params.set('cuitProveedor', cuitProveedor);
        params.set('materialId', materialId);
        params.set('filtro', filtro);

        return this.http.get('/api/CrearContrato/ObtenerFijacionesAutomaticas', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }

    validarProveedor(proveedorId): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        let params: URLSearchParams = new URLSearchParams();
        params.set('proveedorId', proveedorId);
        return this.http.get('/api/CrearContrato/ValidarProveedor', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }

    traerPrecioMoaMateriales(tipoNegocioId): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        let params: URLSearchParams = new URLSearchParams();
        params.set('tipoNegocioId', tipoNegocioId);
        return this.http.get('/api/CrearContrato/TraerPrecioMoaMateriales', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }
    ObteneContratosAcuerdo(): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        return this.http
            .get('/api/CrearContrato/ObteneContratosAcuerdo', { headers: this.headers })
            .pipe(map(this.extractData));
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
            .pipe(timeoutWith(120000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }
}

@Injectable()
export class CrearContratoAPrecioService extends CrearContratoService {

    //public grabarContrato(contrato: ContratoAPrecio): Observable<any> {
    //    return this.grabarContratoAPrecio(contrato);
    //}

    
}

@Injectable()
export class CrearContratoAFijarService extends CrearContratoService {

    
}

@Injectable()
export class CrearContratoFijacionService extends CrearContratoService {

    

}

@Injectable()
export class CrearContratoAltaMasivaService extends CrearContratoService {

   
}
