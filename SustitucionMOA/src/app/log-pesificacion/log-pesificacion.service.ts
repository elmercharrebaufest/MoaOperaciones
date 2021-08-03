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
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))))
            .pipe(map(this.extractData));
    }

    public getFiltrarPesificaciones(filtros: FiltroPesificacionViewModel): Observable<any> {
        return this.http
            .get('/api/logPesificacion/ListarPorFiltros', {
                search: {
                    Fecha: filtros.fecha == undefined ? "":filtros.fecha,
                    Contrato: filtros.contrato ,
                    Proveedor: filtros.proveedor == undefined ? "":filtros.proveedor,
                    Mail: filtros.mail == undefined ? "":filtros.mail,
                    Fijacion: filtros.fijacion 
                },
                headers: this.headers,
            }
            )
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))))
            .pipe(map(this.extractData));
    }
    public getLogPesificacionesAutomaticas(): Observable<any> {
        return this.http
            .get('/api/logPesificacion/ListarAutomatica')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))))
            .pipe(map(this.extractData));
    }

    public getFiltrarPesificacionesAutomaticas(filtros: FiltroPesificacionesAutomaticoViewModel): Observable<any> {
        return this.http
            .get('/api/logPesificacion/ListarPorFiltrosAutomaticas', {
                search: {
                    Fecha: filtros.fecha == undefined ? "":filtros.fecha,
                    Proveedor: filtros.proveedor == undefined ? "":filtros.proveedor,
                    Mail: filtros.mail == undefined ? "":filtros.mail,
                },
                headers: this.headers,
            }
            )
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))))
            .pipe(map(this.extractData));
    }

    public descargarArchivoSubido(idArchivo : number): Observable<any> {
        return this.http
            .get('/api/logPesificacion/DescargarArchivo', {
                search: {
                   IdArchivo : idArchivo
                },
                headers: this.headers,
            }
            )
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))))
            .pipe(map(this.extractData));
    }
}