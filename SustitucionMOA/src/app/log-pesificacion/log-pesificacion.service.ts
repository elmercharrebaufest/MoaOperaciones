import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from '../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { FiltroPesificacionViewModel } from './ViewModels/filtroPesificacionesViewModel';
import { FiltroPesificacionesAutomaticoViewModel } from './ViewModels/filtroPesificacionesAutomaticoViewModel';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class LogPesificacionService extends BaseService {

    public getLogPesificaciones(): Observable<any> {
        return this.http
            .get('/api/logPesificacion/Listar')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public getFiltrarPesificaciones(filtros: FiltroPesificacionViewModel): Observable<any> {
        let Filtrosjson = JSON.stringify(filtros);
        var params = new HttpParams();

        params = params.append('filtroJson', Filtrosjson)

        return this.http
            .get('/api/logPesificacion/ListarPorFiltros', {
                params: params,
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
        let Filtrosjson = JSON.stringify(filtros);
        var params = new HttpParams();

        params = params.append('filtroJson', Filtrosjson)

        return this.http
            .get('/api/logPesificacion/ListarPorFiltrosAutomaticas', {
                params: params,
                headers: this.headers,
            }
            )
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public descargarArchivoSubido(idArchivo : number): Observable<any> {
        var params = new HttpParams();
        params = params.append("idArchivo", idArchivo.toString())

        return this.http
            .get('/api/logPesificacion/DescargarArchivo', {
                params: params,
                headers: this.headers,
            }
            )
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }
}