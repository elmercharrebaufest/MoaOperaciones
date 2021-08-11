
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs';
import { Empresa } from './Empresa'
import { HistorialAprobaciones } from './Empresa'



import { Formatter } from './../../common/formatter/Formatter';
import { BaseService } from './../../common/services/BaseService';
import { environment } from '../../../environments/environment';

@Injectable()
export class AltaEmpresaService extends BaseService {

    public getEmpresas(idTipoProveedor): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('idTipoProveedor', idTipoProveedor.toString());

        return this.http
            .get('/api/AltaEmpresa/getEmpresas', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }


    public setEstadoAprobacion(empresaId: number, estadoId: number, observacion: string, observacionesProveedor: string, estadoSIPER: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('empresaId', empresaId.toString());
        params.set('estado', estadoId.toString());
        params.set('observacion', encodeURIComponent(observacion));
        params.set('observacionParaElProveedor', encodeURIComponent(observacionesProveedor));
        params.set('estadoSIPER', estadoSIPER);
        return this.http
            .post('/api/AltaEmpresa/setEstadoAprobacion', params, this.headersPost).map(this.extractData);
    }

    public solicitarInformacion(empresaId: number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('empresaId', empresaId.toString());
        return this.http
            .get('/api/AltaEmpresa/solicitarInformacion', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }



    public getEstados(): Observable<any> {
        return this.http
            .get('/api/AltaEmpresa/getEstados', { headers: this.headers }).pipe(
                map(this.extractData));
    }

    cargarSolicitudUsuario(): Observable<any> {
        return this.http
            .get('/api/AltaEmpresaGranos/CargarSolicitudUsuario')
            .pipe(map(this.extractData));
    }

    public VerificarEstadoDataAgro(empresaId: number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('proveedorID', empresaId.toString());
        return this.http
            .get('/api/AltaEmpresa/VerificarEstadoDataAgro', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }

    public GuardarSIPER(proveedorId: number, estadoSIPER: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('proveedorId', proveedorId.toString());
        params.set('estadoSIPER', estadoSIPER);
        return this.http
            .get('/api/AltaEmpresa/GuardarSIPER', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }

    public getRubros(): Observable<any> {
        return this.http
            .get('/api/usuario/getRubros', { headers: this.headers }).pipe(
                map(this.extractData));
    }

    public getTipoCambiario(): Observable<any> {
        return this.http
            .get('/api/dataagro/GetTipoCambiario', { headers: this.headers }).pipe(
                map(this.extractData));
    }

    public proveedorNoGranosOperando(proveedorId: number, razonSocial: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('proveedorId', proveedorId.toString())
        params.set('razonSocial', razonSocial)
        return this.http
            .get('/api/AltaEmpresaNoGranos/HabilitarNoGranosOperando', { search: params, headers: this.headers })
            .pipe(map(this.extractData))
    }


    public agregarObservacion(empresaId: number, observacion: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('empresaId', empresaId.toString());
        params.set('observacion', encodeURIComponent(observacion));
        return this.http
            .post('/api/AltaEmpresa/AgregarObservacion', params, this.headersPost).map(this.extractData);
    }

    public grabarAltaInternaGranos(cuit: string, mailVendedor: string){
        let params: URLSearchParams = new URLSearchParams();
        params.set('cuit', cuit);
        params.set('mailVendedor', mailVendedor);

        return this.http
             .get('/api/AltaEmpresaGranos/GrabarNuevoProveedorGranos', { search: params, headers: this.headers }).pipe(
                map(this.extractData));   
    }
}