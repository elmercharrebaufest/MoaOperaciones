
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Usuario } from './usuario'
import { BaseService } from './../common/services/BaseService';
import { HttpParams } from '@angular/common/http';
import { AltaNuevoProveedor } from '../compras/solp-compra';

@Injectable()
export class UsuarioService extends BaseService {

    guardarRolesUsuario(usuarioSeleccionado: any, idRoles: any, usuarioSap: string) {
        let params: HttpParams = new HttpParams();

        var idUsuario = usuarioSeleccionado.Id;

        params = params.append('idRoles', idRoles);
        params = params.append('idUsuario', idUsuario);
        params = params.append('usuarioSap', usuarioSap);


        return this.http
            .get('/api/usuario/GuardarRoles', { params: params, headers: this.headers });
    }

    public cambiarContrasenia(contraseniaActual: string, contraseniaNueva: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('contraseniaActual', contraseniaActual);
        params = params.append('contraseniaNueva', contraseniaNueva);

        return this.http
            .get('/api/usuario/cambiarContrasenia', { params: params, headers: this.headers });
    }

    public alta(usuario: Usuario): Observable<any> {
        let body = JSON.stringify(usuario);
        return this.http
            .post('/api/usuario/alta', body, {headers: this.headersPost});
    }

    public getPerfiles(): Observable<any> {
        return this.http
            .get('/api/usuario/getPerfiles', { headers: this.headers });
    }

    public getUsuarios(): Observable<any> {
        return this.http
            .get('/api/usuario/getUsuarios', { headers: this.headers });
    }

    public getVendedores(): Observable<any> {
        return this.http
            .get('/api/usuario/getVendedores', { headers: this.headers });
    }

    public desbloquearUsuario(usuario: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('usuario', usuario);
        return this.http
            .get('/api/usuario/desbloquear', { params: params, headers: this.headers });
    }

    public deshabilitarUsuario(mailUsuario: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('mailUsuario', mailUsuario);
        return this.http
            .get('/api/usuario/deshabilitar', { params: params, headers: this.headers });
    }

    public habilitarUsuario(mailUsuario: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('mailUsuario', mailUsuario);
        return this.http
            .get('/api/usuario/habilitar', { params: params, headers: this.headers });
    }

    public seleccionarVendedor(vendedor: string, descripcion: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('vendedor', vendedor);
        params = params.append('descripcion', descripcion);
        return this.http
            .get('/api/usuario/seleccionarVendedor', { params: params, headers: this.headers });
    }

    public getRoles(): Observable<any> {
        return this.http
            .get('/api/usuario/getRoles', { headers: this.headers });
    }

    public obtenerRolesUsuario(usuarioSeleccionado: any) {
        let params: HttpParams = new HttpParams();
        var idUsuario = usuarioSeleccionado.Id;

        params = params.append('idUsuario', idUsuario);
        return this.http
            .get('/api/usuario/ObtenerRolesUsuario', { params: params, headers: this.headers });
    }

    public getRubros(): Observable<any> {
        return this.http
            .get('/api/usuario/getRubros', { headers: this.headers });
    }

    grabarNuevoProveedorNoGranos(razonSocial: any, cuit: any, email: any, telefono: any, realizarAnalisisNOSIS: any, IdRubro: any, condicionDePago: any,
        servicioPrestado: any, organizacionDeCompra: any, razonDeEleccion: any, facturacionAnual: any, solicitanteInterno: any, idProveedor: any,
        observacionesParaElProveedor: any, requiereVerificacionCompras: any, ingresoAPlanta: any, altaInterna: any, siperObligatorio: any,
        observacionInterna: any) {
        let params: HttpParams = new HttpParams();
        params = params.append('razonSocial', razonSocial);
        params = params.append('cuit', cuit);
        params = params.append('email', email);
        params = params.append('telefono', telefono);
        params = params.append('realizarAnalisisNOSIS', realizarAnalisisNOSIS);
        params = params.append('IdRubro', IdRubro);
        params = params.append('condicionDePago', condicionDePago);
        params = params.append('servicioPrestado', servicioPrestado);
        params = params.append('organizacionDeCompra', organizacionDeCompra);
        params = params.append('razonDeEleccion', razonDeEleccion);
        params = params.append('facturacionAnual', facturacionAnual);
        params = params.append('solicitanteInterno', solicitanteInterno);
        params = params.append('idProveedor', idProveedor);
        params = params.append('observacionesParaElProveedor', observacionesParaElProveedor);
        params = params.append('requiereVerificacionCompras', requiereVerificacionCompras);
        params = params.append('ingresoAPlanta', ingresoAPlanta);
        params = params.append('altaInterna', altaInterna);
        params = params.append('siperObligatorio', siperObligatorio);
        params = params.append('observacionInterna', observacionInterna);

        return this.http
            .get('/api/usuario/GrabarNuevoProveedorNoGranos', { params: params, headers: this.headers });
    }

    rechazarNuevoProveedorNoGranos(proveedorId: any, observacionesParaElProveedor: any) {
        let params: HttpParams = new HttpParams();
        params = params.append('idProveedor', proveedorId);
        params = params.append('observacionesParaElProveedor', observacionesParaElProveedor)

        return this.http
            .get('/api/usuario/RechazarProveedorNoGranos', { params: params, headers: this.headers });
    }

    public getTipoCambiario(): Observable<any> {
        return this.http
            .get('/api/dataagro/GetTipoCambiario', { headers: this.headers });
    }

    public getRazonSocial(cuit: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('cuit', cuit);

        return this.http
             .get('/api/AltaEmpresaNoGranos/GetRazonSocial', { params: params, headers: this.headers });
    }

    public altaNuevoProveedorCompras(altaNuevoProveedor: AltaNuevoProveedor) {
        let json = JSON.stringify({
            CUIT: altaNuevoProveedor.CUIT,
            Mail: altaNuevoProveedor.Mail,          
            RazonSocial: altaNuevoProveedor.RazonSocial
        });

        var payload = new FormData();
        payload.append('json', json);

        return this.http
            .post<any>('/api/usuario/GrabarProveedor', payload, { headers: this.headers });
    }
}