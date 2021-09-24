import { HttpHeaders, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, throwError as observableThrowError } from "rxjs";
import { map, timeoutWith } from "rxjs/operators";
import { BaseService } from "./../common/services/BaseService";

@Injectable()
export class DatoFiscalService extends BaseService {
  public getDatosFiscales(vendedor: string): Observable<any> {
    let params: HttpParams = new HttpParams();
    params = params.append("vendedor", vendedor);

    return this.http
      .get("/api/vendedor/getDatoFiscales", {
        params: params,
        headers: this.headers,
      });
  }

  public getVendedores(
    fecha_inicio: string,
    fecha_fin: string
  ): Observable<any> {
    let params: HttpParams = new HttpParams();
    params = params.append("fechaInicio", fecha_inicio);
    params = params.append("fechaFin", fecha_fin);

    return this.http
      .get("/api/vendedor/getVendedores", {
        params: params,
        headers: this.headers,
      })
      .pipe(
        timeoutWith(
          30000,
          observableThrowError(
            new Error(
              "Tiempo de respuesta agotado, por favor intentar nuevamente"
            )
          )
        )
      );
  }

  public getVendedoresPendientes(): Observable<any> {
    let params: HttpParams = new HttpParams();
    let headers = new HttpHeaders();
    headers = headers.append('Content-Type', 'application/json');
    headers = headers.append('Accept', 'q=0.8;application/json;q=0.9')
    headers = headers.append('Cache-control', 'no-cache');
    headers = headers.append('Cache-control', 'no-store');
    headers = headers.append('Expires', '0');
    headers = headers.append('Pragma', 'no-cache');

    return this.http
      .get("/api/vendedor/getVendedoresPendientes", {
        params: params,
        headers: this.headers,
      })
      .pipe(
        timeoutWith(
          30000,
          observableThrowError(
            new Error(
              "Tiempo de respuesta agotado, por favor intentar nuevamente"
            )
          )
        )
      );
  }

  public agregarVendedor(
    nuevoVendedorCUIT: string
  ): Observable<any> {
    let params: HttpParams = new HttpParams();
    params = params.append("cuit", nuevoVendedorCUIT);

    return this.http
      .get("/api/vendedor/agregarVendedor", { params: params })
      .pipe(
        timeoutWith(
          30000,
          observableThrowError(
            new Error(
              "Tiempo de respuesta agotado, por favor intentar nuevamente"
            )
          )
        )
      );
  }

  public eliminarVendedor(proveedorId: number): Observable<any> {
    let params: HttpParams = new HttpParams();
    params = params.append("proveedorId", proveedorId.toString());

    return this.http
      .get("/api/vendedor/eliminarVendedor", { params: params })
      .pipe(
        timeoutWith(
          30000,
          observableThrowError(
            new Error(
              "Tiempo de respuesta agotado, por favor intentar nuevamente"
            )
          )
        )
      );
  }
}
