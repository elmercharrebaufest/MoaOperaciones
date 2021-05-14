import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Formatter } from './../../formatter/Formatter';
import { BaseService } from './../../services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { identifierModuleUrl } from '@angular/compiler';
import { getLocaleDateTimeFormat } from '@angular/common';

@Injectable()
export class SeleccionarProveedorService extends BaseService {

  public getVendedores(fecha_inicio: string, fecha_fin: string): Observable<any> {
    let params: URLSearchParams = new URLSearchParams();
    params.set("fechaInicio", fecha_inicio);
    params.set("fechaFin", fecha_fin);
    return this.http.get("/api/vendedor/getVendedores", { search: params, headers: this.headers, })
      .pipe(timeoutWith(30000, observableThrowError(
        new Error(
          "Tiempo de respuesta agotado, por favor intentar nuevamente"
        )
      )
      ),
        map(this.extractData)
      );
  }


  public obtenerProveedorPorCodigo(codigoProveedor: string): Observable<any> {
    let params: URLSearchParams = new URLSearchParams();
    params.set("codigoProveedor", codigoProveedor);
    return this.http.get("/api/Usuario/VerificarYObtenerProveedor", { search: params, headers: this.headers, })
      .pipe(timeoutWith(30000, observableThrowError(
        new Error(
          "Tiempo de respuesta agotado, por favor intentar nuevamente"
        )
      )
      ),
        map(this.extractData)
      );
  }
}
