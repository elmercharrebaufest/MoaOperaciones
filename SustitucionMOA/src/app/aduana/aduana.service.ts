
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { BaseService } from './../common/services/BaseService';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class AduanaService extends BaseService {

    public getPesada(centro: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('centro', centro);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);
        return this.http
            .get('/api/aduana/getPesada', { params: params, headers: this.headers });
    }
    
    public getPesadaDetalle(centro: string, nroOrden: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('centro', centro);
        params = params.append('nroOrden', nroOrden);
        return this.http
            .get('/api/aduana/getPesadaDetalle', { params: params, headers: this.headers });
    }

    public getImagenCamaraConsolidacion(url: string, nombre: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('url', url);
        params = params.append('nombre', nombre);
        return this.http
            .get('/api/aduana/obtenerImagenCamaraConsolidacion', { params: params, headers: this.headers });
    }
}

