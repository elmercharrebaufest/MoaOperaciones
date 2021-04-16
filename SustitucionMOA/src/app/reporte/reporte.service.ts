
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {timeoutWith, map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';



import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';
import { environment } from '../../environments/environment';
 
@Injectable()
export class ReporteService extends BaseService {

    public getDatosContratos(numero_contrato: string, fijacion: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/CrearContrato/GetContratos', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }
    public getDatosCombos() {
        
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');
        let params: URLSearchParams = new URLSearchParams();
        params.set('tiponegocio', "1");
        return this.http
            .get('/api/CrearContrato/ObteneDatosContrato', { search: params,headers: this.headers })
            .pipe(map(this.extractData));
    }
    public obtenerMateriales() {
        return this.http
            .get('/api/AltaEmpresaGranos/GetMateriales', { headers: this.headers }).pipe(
                map(this.extractData));
    }
    public obteneContratos(fechaDesde, fechaHasta, entregaDesde, entregaHasta, fijacionHasta, corredorId, proveedorId,
        boletoId, clasificacionId, destinoId, estadoId, materialId, campaniaId, tipoNegocioId, pagoDiferidoTercero, calidadTercero, dolarizadoTercero, sustentableTercero, contratoCorredor) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('fechaDesde', fechaDesde);
        params.set('fechaHasta', fechaHasta);
        params.set('entregaDesde', entregaDesde);
        params.set('entregaHasta', entregaHasta);
        params.set('fijacionHasta', fijacionHasta);
        params.set('corredorId', corredorId);
        params.set('proveedorId', proveedorId);
        params.set('boletoId', boletoId);
        params.set('clasificacionId', clasificacionId);
        params.set('destinoId', destinoId);
        params.set('estadoId', estadoId);
        params.set('materialId', materialId);
        params.set('campaniaId', campaniaId);
        params.set('tipoNegocioId', tipoNegocioId);
        params.set('pagoDiferidoTercero', pagoDiferidoTercero);
        params.set('calidadTercero', calidadTercero);
        params.set('dolarizadoTercero', dolarizadoTercero);
        params.set('sustentableTercero', sustentableTercero);
        params.set('contratoCorredor', contratoCorredor);

        return this.http
            .get('/api/CrearContrato/GetContratos', { search: params, headers: this.headers })
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

    public exportContratos(fechaDesde, fechaHasta, entregaDesde, entregaHasta, fijacionHasta, corredorId, proveedorId,
        boletoId, clasificacionId, destinoId, estadoId, materialId, campaniaId, tipoNegocioId, pagoDiferidoTercero, calidadTercero, dolarizadoTercero, sustentableTercero, contratoCorredor) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('fechaDesde', fechaDesde);
        params.set('fechaHasta', fechaHasta);
        params.set('entregaDesde', entregaDesde);
        params.set('entregaHasta', entregaHasta);
        params.set('fijacionHasta', fijacionHasta);
        params.set('corredorId', corredorId);
        params.set('proveedorId', proveedorId);
        params.set('boletoId', boletoId);
        params.set('clasificacionId', clasificacionId);
        params.set('destinoId', destinoId);
        params.set('estadoId', estadoId);
        params.set('materialId', materialId);
        params.set('campaniaId', campaniaId);
        params.set('tipoNegocioId', tipoNegocioId);
        params.set('pagoDiferidoTercero', pagoDiferidoTercero);
        params.set('calidadTercero', calidadTercero);
        params.set('dolarizadoTercero', dolarizadoTercero);
        params.set('sustentableTercero', sustentableTercero);
        params.set('contratoCorredor', contratoCorredor);
        
        return this.http
            .get('/api/CrearContrato/ExportContratos', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }

    public anularNegocio(negocioId , tipoNegocioId , motivo) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('negocioId', negocioId);
        params.set('tipoNegocioId', tipoNegocioId);
        params.set('motivo', motivo);
        return this.http
            .get('/api/CrearContrato/AnularNegocio', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }
}

@Injectable()
export class ReporteContratoService extends ReporteService {

    
}


@Injectable()
export class ReporteCupoService extends ReporteService {


}
