import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Balanza, Commodity, BalanzaBusqueda, BalanzaAplicar } from './ryd-mantenimiento'
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';


@Injectable()
export class RYDMantenimientoService extends BaseService{

    getFiltros(method: string): Observable<any> {
        return this.http
            .get('/api/RYDMantenimiento/getFiltros' + method, { headers: this.headers })
            .timeoutWith(300000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }

    getInputDropDown(): Observable<any> {
        return this.http
            .get('/api/RYDMantenimiento/getInputDropDown', { headers: this.headers })
            .timeoutWith(300000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }

    getNroPuesto(itcId: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('itcID', itcId);
        return this.http
            .get('/api/RYDMantenimiento/getFiltrosNroPuesto', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }

    public buscarBalanza(descripcion: string, tipoSelected: string, codigoCabezalSelected: string, codigoSAP: string ): Observable<any> {

        let params: URLSearchParams = new URLSearchParams();
        params.set('descripcion', descripcion);
        params.set('tipoId', tipoSelected);
        params.set('codigoSAP', codigoSAP);
        params.set('codigoCabezalId', codigoCabezalSelected);
        return this.http
            .post('/api/RYDMantenimiento/buscarBalanza', params, this.headersPost)
            .map(this.extractData);
    }

    public guardarBalanza(codigo: string, descripcion: string, automatico: string, toleria: string, centroEmisor: string, tolerX: string, tipoSelected: string, pesoMaximo : string, codigoSAP: string, codigoCabezalSelected: string, itcSelected: string, nroPuestoSelected: string, tipoAccesoSelected: string): Observable<any> {

        let params: URLSearchParams = new URLSearchParams();
        params.set('codigo', codigo);
        params.set('descripcion', descripcion);
        params.set('automatico', automatico);
        params.set('toleria', toleria);
        params.set('centroEmisor', centroEmisor);
        params.set('tolerX', tolerX);
        params.set('tipoId', tipoSelected);
        params.set('pesoMaximo', pesoMaximo);
        params.set('codigoSAP', codigoSAP);
        params.set('codigoCabezalId', codigoCabezalSelected);
        params.set('itcId', itcSelected);
        params.set('nroPuestoId', nroPuestoSelected);
        params.set('tipoAccesoId', tipoAccesoSelected);

        return this.http
            .post('/api/RYDMantenimiento/guardarBalanza', params, this.headersPost)
            .map(this.extractData);
    }

    public actualizarBalanza(codigo: string, descripcion: string, automatico: string, toleria: string, centroEmisor: string, tolerX: string, tipoSelected: string, pesoMaximo: string, codigoSAP: string, codigoCabezalSelected: string, itcSelected: string, nroPuestoSelected: string, tipoAccesoSelected: string): Observable<any> {

        let params: URLSearchParams = new URLSearchParams();
        params.set('codigo', codigo);
        params.set('descripcion', descripcion);
        params.set('automatico', automatico);
        params.set('toleria', toleria);
        params.set('centroEmisor', centroEmisor);
        params.set('tolerX', tolerX);
        params.set('tipoId', tipoSelected);
        params.set('pesoMaximo', pesoMaximo);
        params.set('codigoSAP', codigoSAP);
        params.set('codigoCabezalId', codigoCabezalSelected);
        params.set('itcId', itcSelected);
        params.set('nroPuestoId', nroPuestoSelected);
        params.set('tipoAccesoId', tipoAccesoSelected);

        return this.http
            .post('/api/RYDMantenimiento/actualizarBalanza', params, this.headersPost)
            .map(this.extractData);
    }

    public borrarBalanza(codigo: string): Observable<any> {

        let params: URLSearchParams = new URLSearchParams();
        params.set('codigo', codigo);
        return this.http
            .post('/api/RYDMantenimiento/borrarBalanza', params, this.headersPost)
            .map(this.extractData);
    }

    /*public extractData(res: Response) {
        return res.json();
    }*/

    public busqueda(busquedas: Array<BalanzaBusqueda>): Observable<any> {
        let body = JSON.stringify(busquedas);
        return this.http
            .post('/api/RYDMantenimiento/busqueda', body, this.headersPost)
            .map(this.extractData);
    }

    aplicar(codigo: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('codigoId', codigo);
        return this.http
            .get('/api/RYDMantenimiento/aplicar', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
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
        let params: URLSearchParams = new URLSearchParams();
        params.set('commoditie', commoditie);
        return this.http
            .get('/api/RYDMantenimiento/getFiltrosCommodities', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }

    getInputCommodities(): Observable<any> {
        return this.http
            .get('/api/RYDMantenimiento/getCommodities', { headers: this.headers })
            .timeoutWith(300000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }

    public guardarCommodity(materialSAP: string, almacenOrigen: string, descripcion: string): Observable<any> {
        //let body = JSON.stringify(commodity);
        let params: URLSearchParams = new URLSearchParams();
        params.set('materialSAP', materialSAP);
        params.set('almacenOrigen', almacenOrigen);
        params.set('descripcion', descripcion);
        return this.http
            .post('/api/RYDMantenimiento/guardarCommodity', params, this.headersPost)
            .map(this.extractData);
    }

    public actualizarCommodity(materialSAP: string, almacenOrigen: string, descripcion: string, commoditieSelected: string): Observable<any> {
        //let body = JSON.stringify(commodity);
        let params: URLSearchParams = new URLSearchParams();
        params.set('materialSAP', materialSAP);
        params.set('almacenOrigen', almacenOrigen);
        params.set('descripcion', descripcion);
        params.set('commodityId', commoditieSelected);
        return this.http
            .post('/api/RYDMantenimiento/actualizarCommodity', params, this.headersPost)
            .map(this.extractData);
    }

    public borrarCommodity(commoditieSelected: string): Observable<any> {
        //let body = JSON.stringify(commodity);
        let params: URLSearchParams = new URLSearchParams();
        params.set('commodityId', commoditieSelected);
        return this.http
            .post('/api/RYDMantenimiento/borrarCommodity', params, this.headersPost)
            .map(this.extractData);
    }
    
}

@Injectable()
export class RYDMantenimientoExportadorService extends RYDMantenimientoService {

    getFiltros(): Observable<any> {
        return super.getFiltros("Exportador");
    }

    getExportador(exportador: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('exportador', exportador);
        return this.http
            .get('/api/RYDMantenimiento/getFiltrosExportador', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }

    getInputExportador(): Observable<any> {
        return this.http
            .get('/api/RYDMantenimiento/getExportador', { headers: this.headers })
            .timeoutWith(300000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }

    public guardarExportador(almacenSAP: string, descripcion: string): Observable<any> {
        //let body = JSON.stringify(commodity);
        let params: URLSearchParams = new URLSearchParams();
        params.set('almacenSAP', almacenSAP);
        params.set('descripcion', descripcion);
        return this.http
            .post('/api/RYDMantenimiento/guardarExportador', params, this.headersPost)
            .map(this.extractData);
    }

    public actualizarExportador(almacenSAP: string, descripcion: string, exportadorSelected: string): Observable<any> {
        //let body = JSON.stringify(commodity);
        let params: URLSearchParams = new URLSearchParams();
        params.set('almacenSAP', almacenSAP);
        params.set('descripcion', descripcion);
        params.set('exportadorId', exportadorSelected);
        return this.http
            .post('/api/RYDMantenimiento/actualizarExportador', params, this.headersPost)
            .map(this.extractData);
    }

    public borrarExportador(exportadorSelected: string): Observable<any> {
        //let body = JSON.stringify(commodity);
        let params: URLSearchParams = new URLSearchParams();
        params.set('exportadorId', exportadorSelected);
        return this.http
            .post('/api/RYDMantenimiento/borrarExportador', params, this.headersPost)
            .map(this.extractData);
    }

}