import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from './../common/services/BaseService';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class EcheqService extends BaseService {

    protected getContratoPendientePago(periodo: string, fecha_inicio: string, fecha_fin: string, method: string) : Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);
        return this.http
            .get(`/api/echeq/${method}`, { params: params, headers: this.headers });
    }

    public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        console.log("Soy el servicio que tanto buscan");
        return this.getContratoPendientePago(periodo, fecha_inicio, fecha_fin, 'ObtenerPendientePago');
    }
}

