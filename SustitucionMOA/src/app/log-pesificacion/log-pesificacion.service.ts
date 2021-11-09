import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from '../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { FiltroPesificacionViewModel } from './ViewModels/filtroPesificacionesViewModel';
import { FiltroPesificacionesAutomaticoViewModel } from './ViewModels/filtroPesificacionesAutomaticoViewModel';

@Injectable()
export class LogPesificacionService extends BaseService {

    public getLogPesificaciones(): Observable<any> {
        return this.http
            .get('/api/logPesificacion/Listar')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public getFiltrarPesificaciones(filtros: FiltroPesificacionViewModel): Observable<any> {
        return this.http
            .get('/api/logPesificacion/ListarPorFiltros', {
                params: {
                    Fecha: filtros.fecha == undefined ? "":filtros.fecha,
                    Contrato: filtros.contrato.toString() ,
                    Proveedor: filtros.proveedor == undefined ? "":filtros.proveedor,
                    Mail: filtros.mail == undefined ? "":filtros.mail,
                    Fijacion: filtros.fijacion.toString() 
                },
                headers: this.headers,
            }
            )
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }
    public getLogPesificacionesAutomaticas(): Observable<any> {
        return this.http
            .get('/api/logPesificacion/ListarAutomatica')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public getFiltrarPesificacionesAutomaticas(filtros: FiltroPesificacionesAutomaticoViewModel): Observable<any> {
        return this.http
            .get('/api/logPesificacion/ListarPorFiltrosAutomaticas', {
                params: {
                    Fecha: filtros.fecha == undefined ? "":filtros.fecha,
                    Proveedor: filtros.proveedor == undefined ? "":filtros.proveedor,
                    Mail: filtros.mail == undefined ? "":filtros.mail,
                },
                headers: this.headers,
            }
            )
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public descargarArchivoSubido(idArchivo : number): Observable<any> {
        return this.http
            .get('/api/logPesificacion/DescargarArchivo', {
                params: {
                   IdArchivo : idArchivo.toString()
                },
                headers: this.headers,
            }
            )
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }
}