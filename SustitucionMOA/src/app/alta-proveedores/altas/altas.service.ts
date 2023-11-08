
import { Injectable } from '@angular/core';
import { BaseService } from './../../common/services/BaseService';
import { HttpParams } from '@angular/common/http';
import { CommonResponse } from '../../common/models/common-response';
import { throwError as observableThrowError } from 'rxjs';
import { timeoutWith } from 'rxjs/operators';
import { Observable } from 'rxjs';
@Injectable()
export class AltaEmpresaService extends BaseService {

    public getEmpresas(idTipoProveedor, fechaInicio: string, fechaFin: string): Observable<any> {
        let params: HttpParams = new HttpParams()
        .append('idTipoProveedor', idTipoProveedor.toString())
        .append('fechaInicio', fechaInicio)
        .append('fechaFin', fechaFin)

        return this.http
            .get('/api/AltaEmpresa/getEmpresas', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public setEstadoAprobacion(empresaId: number, estadoId: number, observacion: string, observacionesProveedor: string, estadoSIPER: string, razonSocial: string, codigoCliente: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('empresaId', empresaId.toString());
        params = params.append('estado', estadoId.toString());
        params = params.append('observacion', encodeURIComponent(observacion));
        params = params.append('observacionParaElProveedor', encodeURIComponent(observacionesProveedor));
        params = params.append('estadoSIPER', estadoSIPER);
        params = params.append('razonSocial', razonSocial);
        params = params.append('codigoCliente', codigoCliente);
        return this.http
            .post('/api/AltaEmpresa/setEstadoAprobacion', params, { headers: this.headersPost });
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
            .post('/api/AltaEmpresa/AgregarObservacion', params, { headers: this.headersPost });
    }

    public grabarAltaInternaGranos(cuit: string, mailVendedor: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('cuit', cuit);
        params = params.append('mailVendedor', mailVendedor);
        return this.http
            .get('/api/AltaEmpresaGranos/GrabarNuevoProveedorGranos', { params: params, headers: this.headers });
    }

    public eliminarCuitNoHabilitado(proveedorId: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append("proveedorId", proveedorId.toString());
        return this.http
            .get("/api/usuario/eliminarCuitNoHabilitado", { params: params, headers: this.headers });
    }
}
