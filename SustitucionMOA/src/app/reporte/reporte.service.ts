
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { BaseService } from './../common/services/BaseService';
import { HttpHeaders, HttpParams } from '@angular/common/http';
 
@Injectable()
export class ReporteService extends BaseService {

    public getDatosContratos(numero_contrato: string, fijacion: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('numeroContrato', numero_contrato);
        params = params.append('fijacion', fijacion);

        return this.http
            .get('/api/CrearContrato/GetContratos', { params: params, headers: this.headers });
    }
    public getDatosCombos() {
        let headers = new HttpHeaders();
        headers = headers.append('Content-Type', 'application/json');
        headers = headers.append('Accept', 'q=0.8;application/json;q=0.9')
        headers = headers.append('Cache-control', 'no-cache');
        headers = headers.append('Cache-control', 'no-store');
        headers = headers.append('Expires', '0');
        headers = headers.append('Pragma', 'no-cache');

        let params: HttpParams = new HttpParams();
        params = params.append('tiponegocio', "1");

        return this.http
            .get('/api/CrearContrato/ObteneDatosContrato', { params: params,headers: headers });
    }
    public obtenerMateriales() {
        return this.http
            .get('/api/AltaEmpresaGranos/GetMateriales', { headers: this.headers });
    }
    public obteneContratos(fechaDesde, fechaHasta, entregaDesde, entregaHasta, fijacionHasta, corredorId, proveedorId,
        boletoId, clasificacionId, destinoId, estadoId, materialId, campaniaId, tipoNegocioId, pagoDiferidoTercero, calidadTercero, dolarizadoTercero, sustentableTercero, contratoCorredor) {
        let params: HttpParams = new HttpParams();
        params = params.append('fechaDesde', fechaDesde);
        params = params.append('fechaHasta', fechaHasta);
        params = params.append('entregaDesde', entregaDesde);
        params = params.append('entregaHasta', entregaHasta);
        params = params.append('fijacionHasta', fijacionHasta);
        params = params.append('corredorId', corredorId);
        params = params.append('proveedorId', proveedorId);
        params = params.append('boletoId', boletoId);
        params = params.append('clasificacionId', clasificacionId);
        params = params.append('destinoId', destinoId);
        params = params.append('estadoId', estadoId);
        params = params.append('materialId', materialId);
        params = params.append('campaniaId', campaniaId);
        params = params.append('tipoNegocioId', tipoNegocioId);
        params = params.append('pagoDiferidoTercero', pagoDiferidoTercero);
        params = params.append('calidadTercero', calidadTercero);
        params = params.append('dolarizadoTercero', dolarizadoTercero);
        params = params.append('sustentableTercero', sustentableTercero);
        params = params.append('contratoCorredor', contratoCorredor);

        return this.http
            .get('/api/CrearContrato/GetContratos', { params: params, headers: this.headers });
    }

    buscarProveedoresConCorredor(term): Observable<any> {
        let headers = new HttpHeaders();
        headers = headers.append('Content-Type', 'application/json');
        headers = headers.append('Accept', 'q=0.8;application/json;q=0.9')
        headers = headers.append('Cache-control', 'no-cache');
        headers = headers.append('Cache-control', 'no-store');
        headers = headers.append('Expires', '0');
        headers = headers.append('Pragma', 'no-cache');

        let params: HttpParams = new HttpParams();
        params = params.append('filtro', term);

        return this.http.get('/api/CrearContrato/BuscarProveedoresConCorredor', { params: params, headers: headers });
    }

    validarDirecto(): Observable<any> {
        let headers = new HttpHeaders();
        headers = headers.append('Content-Type', 'application/json');
        headers = headers.append('Accept', 'q=0.8;application/json;q=0.9')
        headers = headers.append('Cache-control', 'no-cache');
        headers = headers.append('Cache-control', 'no-store');
        headers = headers.append('Expires', '0');
        headers = headers.append('Pragma', 'no-cache');

        return this.http
            .get('/api/CrearContrato/ValidarDirecto', { headers: headers });
    }

    public exportContratos(fechaDesde, fechaHasta, entregaDesde, entregaHasta, fijacionHasta, corredorId, proveedorId,
        boletoId, clasificacionId, destinoId, estadoId, materialId, campaniaId, tipoNegocioId, pagoDiferidoTercero, calidadTercero, dolarizadoTercero, sustentableTercero, contratoCorredor) {
        let params: HttpParams = new HttpParams();
        params = params.append('fechaDesde', fechaDesde);
        params = params.append('fechaHasta', fechaHasta);
        params = params.append('entregaDesde', entregaDesde);
        params = params.append('entregaHasta', entregaHasta);
        params = params.append('fijacionHasta', fijacionHasta);
        params = params.append('corredorId', corredorId);
        params = params.append('proveedorId', proveedorId);
        params = params.append('boletoId', boletoId);
        params = params.append('clasificacionId', clasificacionId);
        params = params.append('destinoId', destinoId);
        params = params.append('estadoId', estadoId);
        params = params.append('materialId', materialId);
        params = params.append('campaniaId', campaniaId);
        params = params.append('tipoNegocioId', tipoNegocioId);
        params = params.append('pagoDiferidoTercero', pagoDiferidoTercero);
        params = params.append('calidadTercero', calidadTercero);
        params = params.append('dolarizadoTercero', dolarizadoTercero);
        params = params.append('sustentableTercero', sustentableTercero);
        params = params.append('contratoCorredor', contratoCorredor);
        
        return this.http
            .get('/api/CrearContrato/ExportContratos', { params: params, headers: this.headers });
    }

    public anularNegocio(negocioId , tipoNegocioId , motivo) {
        let params: HttpParams = new HttpParams();
        params = params.append('negocioId', negocioId);
        params = params.append('tipoNegocioId', tipoNegocioId);
        params = params.append('motivo', motivo);
        
        return this.http
            .get('/api/CrearContrato/AnularNegocio', { params: params, headers: this.headers });
    }
}

@Injectable()
export class ReporteContratoService extends ReporteService {
}

@Injectable()
export class ReporteCupoService extends ReporteService {
}
