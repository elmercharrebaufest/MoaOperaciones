
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {timeoutWith, map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';


import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';
import { ContratoAPrecio } from '../common/models/contratoAPrecio';
import { ContratoAFijar } from '../common/models/contratoAFijar';


@Injectable()
export class CrearContratoService extends BaseService {

    obteneDatosContrato(): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        return this.http
            .get('/api/CrearContrato/ObteneDatosContrato', { headers: this.headers })
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
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    ObtenerFechaHasta(fechaBase) {
        var hoy = fechaBase != undefined ? fechaBase : new Date();
        var anio = hoy.getFullYear();
        var mesPost = hoy.getMonth() + 1;
        var dia = hoy.getDate();
        var ultimoDia = new Date(anio, hoy.getMonth() + 1, 0).getDate();

        if (dia === 1) {
            dia = new Date(anio, hoy.getMonth() + 1, 0).getDate();
            mesPost = hoy.getMonth() + 1;
        }
        if (dia === ultimoDia || (mesPost === 2 && dia >= 29)) {
            dia = new Date(anio, mesPost, 0).getDate();
        }
        if (mesPost === 13) {
            mesPost = 1;
            anio += 1;
        }
        //if (mesPost < 10) {
        //    mesPost = "0" + mesPost.toString();
        //}
        //if (dia < 10) {
        //    dia = "0" + dia.toString();
        //}
        return new Date(anio, mesPost, dia);// dia + '-' + mesPost + '-' + anio;
    }

    grabarContratoAFijar(contrato: ContratoAFijar): Observable<any> {
        let payload = new FormData();
        payload.append("contrato", JSON.stringify(contrato));
        return this.http
            .post('/api/CrearContrato/CrearContratoAFijar', payload)
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
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

