
import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from '../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { HttpClient, HttpParams } from '@angular/common/http';


@Injectable()
export class ListadoNovedadesService extends BaseService {

    constructor(protected http: HttpClient) {
        super(http);
    }

 

    public getListadoCompletoNovedades(): Observable<any> {
        return this.http
            .get('/api/Notificacion/GetListadoCompletoNotificacion')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    

 

  

 
  

}
