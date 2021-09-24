
import {throwError as observableThrowError,  Observable } from 'rxjs';
import {map, timeoutWith} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { BalanzaBusqueda } from './ryd-mantenimiento'
import { BaseService } from './../common/services/BaseService';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class RYDMantenimientoService extends BaseService{

    getFiltros(method: string): Observable<any> {
        return this.http
            .get('/api/RYDMantenimiento/getFiltros' + method, { headers: this.headers })
            .pipe(timeoutWith(300000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    getInputDropDown(): Observable<any> {
        return this.http
            .get('/api/RYDMantenimiento/getInputDropDown', { headers: this.headers })
            .pipe(timeoutWith(300000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    getNroPuesto(itcId: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('itcID', itcId);
        return this.http
            .get('/api/RYDMantenimiento/getFiltrosNroPuesto', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    public buscarBalanza(descripcion: string, tipoSelected: string, codigoCabezalSelected: string, codigoSAP: string ): Observable<any> {

        let params: HttpParams = new HttpParams();
        params = params.append('descripcion', descripcion);
        params = params.append('tipoId', tipoSelected);
        params = params.append('codigoSAP', codigoSAP);
        params = params.append('codigoCabezalId', codigoCabezalSelected);

        return this.http
            .post('/api/RYDMantenimiento/buscarBalanza', params, {headers:this.headersPost});
    }

    public guardarBalanza(codigo: string, descripcion: string, automatico: string, toleria: string, centroEmisor: string, tolerX: string, tipoSelected: string, pesoMaximo : string, codigoSAP: string, codigoCabezalSelected: string, itcSelected: string, nroPuestoSelected: string, tipoAccesoSelected: string): Observable<any> {

        let params: HttpParams = new HttpParams();
        params = params.append('codigo', codigo);
        params = params.append('descripcion', descripcion);
        params = params.append('automatico', automatico);
        params = params.append('toleria', toleria);
        params = params.append('centroEmisor', centroEmisor);
        params = params.append('tolerX', tolerX);
        params = params.append('tipoId', tipoSelected);
        params = params.append('pesoMaximo', pesoMaximo);
        params = params.append('codigoSAP', codigoSAP);
        params = params.append('codigoCabezalId', codigoCabezalSelected);
        params = params.append('itcId', itcSelected);
        params = params.append('nroPuestoId', nroPuestoSelected);
        params = params.append('tipoAccesoId', tipoAccesoSelected);

        return this.http
            .post('/api/RYDMantenimiento/guardarBalanza', params, {headers:this.headersPost});
    }

    public actualizarBalanza(codigo: string, descripcion: string, automatico: string, toleria: string, centroEmisor: string, tolerX: string, tipoSelected: string, pesoMaximo: string, codigoSAP: string, codigoCabezalSelected: string, itcSelected: string, nroPuestoSelected: string, tipoAccesoSelected: string): Observable<any> {

        let params: HttpParams = new HttpParams();
        params = params.append('codigo', codigo);
        params = params.append('descripcion', descripcion);
        params = params.append('automatico', automatico);
        params = params.append('toleria', toleria);
        params = params.append('centroEmisor', centroEmisor);
        params = params.append('tolerX', tolerX);
        params = params.append('tipoId', tipoSelected);
        params = params.append('pesoMaximo', pesoMaximo);
        params = params.append('codigoSAP', codigoSAP);
        params = params.append('codigoCabezalId', codigoCabezalSelected);
        params = params.append('itcId', itcSelected);
        params = params.append('nroPuestoId', nroPuestoSelected);
        params = params.append('tipoAccesoId', tipoAccesoSelected);

        return this.http
            .post('/api/RYDMantenimiento/actualizarBalanza', params, {headers:this.headersPost});
    }

    public borrarBalanza(codigo: string): Observable<any> {

        let params: HttpParams = new HttpParams();
        params = params.append('codigo', codigo);
        return this.http
            .post('/api/RYDMantenimiento/borrarBalanza', params, {headers:this.headersPost});
    }

    public busqueda(busquedas: Array<BalanzaBusqueda>): Observable<any> {
        let body = JSON.stringify(busquedas);
        return this.http
            .post('/api/RYDMantenimiento/busqueda', body, {headers:this.headersPost});
    }

    aplicar(codigo: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('codigoId', codigo);
        return this.http
            .get('/api/RYDMantenimiento/aplicar', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }
}

@Injectable()
export class RYDMantenimientoBalanzaService extends RYDMantenimientoService {
}

@Injectable()
export class RYDMantenimientoCommoditiesService extends RYDMantenimientoService {

    getFiltros(): Observable<any> {
        return super.getFiltros("Commodities");
    }

    getCommodities(commoditie: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('commoditie', commoditie);
        return this.http
            .get('/api/RYDMantenimiento/getFiltrosCommodities', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    getInputCommodities(): Observable<any> {
        return this.http
            .get('/api/RYDMantenimiento/getCommodities', { headers: this.headers })
            .pipe(timeoutWith(300000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    public guardarCommodity(materialSAP: string, almacenOrigen: string, descripcion: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('materialSAP', materialSAP);
        params = params.append('almacenOrigen', almacenOrigen);
        params = params.append('descripcion', descripcion);
        return this.http
            .post('/api/RYDMantenimiento/guardarCommodity', params, {headers:this.headersPost});
    }

    public actualizarCommodity(materialSAP: string, almacenOrigen: string, descripcion: string, commoditieSelected: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('materialSAP', materialSAP);
        params = params.append('almacenOrigen', almacenOrigen);
        params = params.append('descripcion', descripcion);
        params = params.append('commodityId', commoditieSelected);
        return this.http
            .post('/api/RYDMantenimiento/actualizarCommodity', params, {headers:this.headersPost});
    }

    public borrarCommodity(commoditieSelected: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('commodityId', commoditieSelected);
        return this.http
            .post('/api/RYDMantenimiento/borrarCommodity', params, {headers:this.headersPost});
    }
    
}

@Injectable()
export class RYDMantenimientoExportadorService extends RYDMantenimientoService {

    getFiltros(): Observable<any> {
        return super.getFiltros("Exportador");
    }

    getExportador(exportador: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('exportador', exportador);
        return this.http
            .get('/api/RYDMantenimiento/getFiltrosExportador', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    getInputExportador(): Observable<any> {
        return this.http
            .get('/api/RYDMantenimiento/getExportador', { headers: this.headers })
            .pipe(timeoutWith(300000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    public guardarExportador(almacenSAP: string, descripcion: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('almacenSAP', almacenSAP);
        params = params.append('descripcion', descripcion);
        return this.http
            .post('/api/RYDMantenimiento/guardarExportador', params, {headers:this.headersPost});
    }

    public actualizarExportador(almacenSAP: string, descripcion: string, exportadorSelected: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('almacenSAP', almacenSAP);
        params = params.append('descripcion', descripcion);
        params = params.append('exportadorId', exportadorSelected);
        return this.http
            .post('/api/RYDMantenimiento/actualizarExportador', params, {headers:this.headersPost});
    }

    public borrarExportador(exportadorSelected: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('exportadorId', exportadorSelected);
        return this.http
            .post('/api/RYDMantenimiento/borrarExportador', params, {headers:this.headersPost});
    }
}