
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { BaseService } from './../../common/services/BaseService';
import { HttpParams } from '@angular/common/http';
import { CommonResponse } from '../../common/models/common-response';

@Injectable()
export class AltaEmpresaService extends BaseService {

    public getEmpresas(idTipoProveedor): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('idTipoProveedor', idTipoProveedor.toString());

        return this.http
            .get('/api/AltaEmpresa/getEmpresas', { params: params, headers: this.headers });
    }

    public setEstadoAprobacion(empresaId: number, estadoId: number, observacion: string, observacionesProveedor: string, estadoSIPER: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('empresaId', empresaId.toString());
        params = params.append('estado', estadoId.toString());
        params = params.append('observacion', encodeURIComponent(observacion));
        params = params.append('observacionParaElProveedor', encodeURIComponent(observacionesProveedor));
        params = params.append('estadoSIPER', estadoSIPER);
        params = params.append('razonSocial', razonSocial);
        params = params.append('codigoCliente', codigoCliente.toString());
        return this.http
            .post('/api/AltaEmpresa/setEstadoAprobacion', params, {headers: this.headersPost});
    }

    public solicitarInformacion(empresaId: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('empresaId', empresaId.toString());
        return this.http
            .get('/api/AltaEmpresa/solicitarInformacion', { params: params, headers: this.headers });
    }

    public getEstados(): Observable<any> {
        return this.http
            .get('/api/AltaEmpresa/getEstados', { headers: this.headers });
    }

    cargarSolicitudUsuario(): Observable<any> {
        return this.http
            .get('/api/AltaEmpresaGranos/CargarSolicitudUsuario');
    }

    public VerificarEstadoDataAgro(empresaId: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('proveedorID', empresaId.toString());
        return this.http
            .get('/api/AltaEmpresa/VerificarEstadoDataAgro', { params: params, headers: this.headers });
    }

    public GuardarSIPER(proveedorId: number, estadoSIPER: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('proveedorId', proveedorId.toString());
        params = params.append('estadoSIPER', estadoSIPER);
        return this.http
            .get<CommonResponse>('/api/AltaEmpresa/GuardarSIPER', { params: params, headers: this.headers });
    }

    public getRubros(): Observable<any> {
        return this.http
            .get('/api/usuario/getRubros', { headers: this.headers });
    }

    public getTipoCambiario(): Observable<any> {
        return this.http
            .get('/api/dataagro/GetTipoCambiario', { headers: this.headers });
    }

    public proveedorNoGranosOperando(proveedorId: number, razonSocial: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('proveedorId', proveedorId.toString())
        params = params.append('razonSocial', razonSocial)
        return this.http
            .get('/api/AltaEmpresaNoGranos/HabilitarNoGranosOperando', { params: params, headers: this.headers });
    }

    public agregarObservacion(empresaId: number, observacion: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('empresaId', empresaId.toString());
        params = params.append('observacion', encodeURIComponent(observacion));
        return this.http
            .post('/api/AltaEmpresa/AgregarObservacion', params, {headers: this.headersPost});
    }

    public grabarAltaInternaGranos(cuit: string, mailVendedor: string){
        let params: HttpParams = new HttpParams();
        params = params.append('cuit', cuit);
      params = params.append('mailVendedor', mailVendedor);  
      return this.http
             .get('/api/AltaEmpresaGranos/GrabarNuevoProveedorGranos', { params: params, headers: this.headers });   
    }
}
