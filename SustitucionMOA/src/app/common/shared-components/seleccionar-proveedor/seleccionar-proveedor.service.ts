import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from './../../services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class SeleccionarProveedorService extends BaseService {

  public getVendedores(fecha_inicio: string, fecha_fin: string): Observable<any> {
    let params: HttpParams = new HttpParams();
    params = params.set("fechaInicio", fecha_inicio);
    params = params.set("fechaFin", fecha_fin);

    return this.http.get("/api/vendedor/getVendedores", { params: params, headers: this.headers, })
      .pipe(timeoutWith(30000, observableThrowError(
        new Error(
          "Tiempo de respuesta agotado, por favor intentar nuevamente"
        )
      )));
  }

  public obtenerProveedorPorCodigo(codigoProveedor: string): Observable<any> {
    let params: HttpParams = new HttpParams();
    params = params.set("codigoProveedor", codigoProveedor);

    return this.http.get("/api/Usuario/VerificarYObtenerProveedor", { params: params, headers: this.headers, })
      .pipe(timeoutWith(30000, observableThrowError(
        new Error(
          "Tiempo de respuesta agotado, por favor intentar nuevamente"
        )
      )));
  }
}
