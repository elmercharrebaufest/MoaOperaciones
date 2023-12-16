import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseService } from '../common/services/BaseService';
import { Observable, throwError as observableThrowError, throwError } from 'rxjs';
import { catchError, timeoutWith } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ComunicacionesService extends BaseService {

  constructor(protected http: HttpClient) {
    super(http);
  }

  public getComunicaciones(idProveedor: string, start_date: string, end_date: string): Observable<any> {
    let params = new HttpParams();
    params = params.append('vendedor', idProveedor);
    params = params.append('fechaInicio', start_date);
    params = params.append('fechaFin', end_date);
    return this.http
        .get('/api/Comunicacion/GetAllByProveedor', { params: params, headers: this.headers })
        .pipe(timeoutWith(600000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }



  public postComunicacionLeida(idComunicacion: number[]): Observable<any> {
    const objectComunicacion = { ItemId: idComunicacion };
    return this.http
      .post('/api/Comunicacion/PostComunicacionLeida', objectComunicacion, {
        headers: this.headers
      })
      .pipe(timeoutWith(600000, throwError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
  }

  public postComunicacionNoLeida(idComunicacion: number[]): Observable<any> {
    const objectComunicacion = { ItemId: idComunicacion };
    return this.http
      .post('/api/Comunicacion/PostComunicacionNoLeida', objectComunicacion, {
        headers: this.headers
      })
      .pipe(timeoutWith(600000, throwError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))
    ));
  }
}