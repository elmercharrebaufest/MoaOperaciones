
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs';
import { Usuario } from './usuario'



import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';
import { environment } from '../../environments/environment';

@Injectable()
export class UsuarioService extends BaseService {

    guardarRolesUsuario(usuarioSeleccionado: any, idRoles: any) {
        let params: URLSearchParams = new URLSearchParams();

        var idUsuario = usuarioSeleccionado.Id;

        params.set('idRoles', idRoles);
        params.set('idUsuario', idUsuario);
        return this.http
            .get('/api/usuario/GuardarRoles', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }

    public cambiarContrasenia(contraseniaActual: string, contraseniaNueva: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('contraseniaActual', contraseniaActual);
        params.set('contraseniaNueva', contraseniaNueva);
        return this.http
            .get('/api/usuario/cambiarContrasenia', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }

    public alta(usuario: Usuario): Observable<any> {
        let body = JSON.stringify(usuario);
        return this.http
            .post('/api/usuario/alta', body, this.headersPost).pipe(
                map(this.extractData));
    }

    public getPerfiles(): Observable<any> {
        return this.http
            .get('/api/usuario/getPerfiles', { headers: this.headers }).pipe(
                map(this.extractData));
    }

    public getUsuarios(): Observable<any> {
        return this.http
            .get('/api/usuario/getUsuarios', { headers: this.headers }).pipe(
                map(this.extractData));
    }

    public getVendedores(): Observable<any> {
        return this.http
            .get('/api/usuario/getVendedores', { headers: this.headers }).pipe(
                map(this.extractData));
    }

    public desbloquearUsuario(usuario: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('usuario', usuario);
        return this.http
            .get('/api/usuario/desbloquear', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }

    public deshabilitarUsuario(mailUsuario: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('mailUsuario', mailUsuario);
        return this.http
            .get('/api/usuario/deshabilitar', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }

    public habilitarUsuario(mailUsuario: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('mailUsuario', mailUsuario);
        return this.http
            .get('/api/usuario/habilitar', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }

    public seleccionarVendedor(vendedor: string, descripcion: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('vendedor', vendedor);
        params.set('descripcion', descripcion);
        return this.http
            .get('/api/usuario/seleccionarVendedor', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }

    public getRoles(): Observable<any> {
        return this.http
            .get('/api/usuario/getRoles', { headers: this.headers }).pipe(
                map(this.extractData));
    }

    public obtenerRolesUsuario(usuarioSeleccionado: any) {
        let params: URLSearchParams = new URLSearchParams();
        var idUsuario = usuarioSeleccionado.Id;

        params.set('idUsuario', idUsuario);
        return this.http
            .get('/api/usuario/ObtenerRolesUsuario', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }

    public getRubros(): Observable<any> {
        return this.http
            .get('/api/usuario/getRubros', { headers: this.headers }).pipe(
                map(this.extractData));
    }

    grabarNuevoProveedorNoGranos(razonSocial: any, cuit: any, email: any, telefono: any, realizarAnalisisNOSIS: any, IdRubro: any, condicionDePago: any,
        servicioPrestado: any, organizacionDeCompra: any, razonDeEleccion: any, facturacionAnual: any, solicitanteInterno: any, idProveedor: any,
        observacionesParaElProveedor: any, requiereVerificacionCompras: any, ingresoAPlanta : any, altaInterna : any) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('razonSocial', razonSocial);
        params.set('cuit', cuit);
        params.set('email', email);
        params.set('telefono', telefono);
        params.set('realizarAnalisisNOSIS', realizarAnalisisNOSIS);
        params.set('IdRubro', IdRubro);
        params.set('condicionDePago', condicionDePago);
        params.set('servicioPrestado', servicioPrestado);
        params.set('organizacionDeCompra', organizacionDeCompra);
        params.set('razonDeEleccion', razonDeEleccion);
        params.set('facturacionAnual', facturacionAnual);
        params.set('solicitanteInterno', solicitanteInterno);
        params.set('idProveedor', idProveedor);
        params.set('observacionesParaElProveedor', observacionesParaElProveedor);
        params.set('requiereVerificacionCompras', requiereVerificacionCompras);
        params.set('ingresoAPlanta', ingresoAPlanta);
        params.set('altaInterna', altaInterna);

        return this.http
            .get('/api/usuario/GrabarNuevoProveedorNoGranos', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }

    rechazarNuevoProveedorNoGranos(proveedorId: any, observacionesParaElProveedor: any) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('idProveedor', proveedorId);
        params.set('observacionesParaElProveedor', observacionesParaElProveedor)

        return this.http
            .get('/api/usuario/RechazarProveedorNoGranos', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }
}