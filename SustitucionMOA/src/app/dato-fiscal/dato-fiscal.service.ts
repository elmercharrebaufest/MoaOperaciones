import { Injectable } from "@angular/core";
import { URLSearchParams } from "@angular/http";
import { Observable, throwError as observableThrowError } from "rxjs";
import { map, timeoutWith } from "rxjs/operators";
import { BaseService } from "./../common/services/BaseService";

@Injectable()
export class DatoFiscalService extends BaseService {
  public getDatosFiscales(vendedor: string): Observable<any> {
    let params: URLSearchParams = new URLSearchParams();
    params.set("vendedor", vendedor);
    return this.http
      .get("/api/vendedor/getDatoFiscales", {
        search: params,
        headers: this.headers,
      })
      .pipe(map(this.extractData));
  }

  public getVendedores(
    fecha_inicio: string,
    fecha_fin: string
  ): Observable<any> {
    let params: URLSearchParams = new URLSearchParams();
    params.set("fechaInicio", fecha_inicio);
    params.set("fechaFin", fecha_fin);
    return this.http
      .get("/api/vendedor/getVendedores", {
        search: params,
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
        ),
        map(this.extractData)
      );
  }

  public getVendedoresPendientes(): Observable<any> {
    let params: URLSearchParams = new URLSearchParams();
    this.headers = new Headers();
    this.headers.append('Content-Type', 'application/json');
    this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
    this.headers.append('Cache-control', 'no-cache');
    this.headers.append('Cache-control', 'no-store');
    this.headers.append('Expires', '0');
    this.headers.append('Pragma', 'no-cache');

    return this.http
      .get("/api/vendedor/getVendedoresPendientes", {
        search: params,
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
        ),
        map(this.extractData)
      );
  }

  public agregarVendedor(
    nuevoVendedorRazonSocial: string,
    nuevoVendedorCUIT: string
  ): Observable<any> {
    let params: URLSearchParams = new URLSearchParams();
    params.set("cuit", nuevoVendedorCUIT);
    params.set("razonSocial", nuevoVendedorRazonSocial);
    return this.http
      .get("/api/vendedor/agregarVendedor", { search: params })
      .pipe(
        timeoutWith(
          30000,
          observableThrowError(
            new Error(
              "Tiempo de respuesta agotado, por favor intentar nuevamente"
            )
          )
        ),
        map(this.extractData)
      );
  }

  public eliminarVendedor(proveedorId: number): Observable<any> {
    let params: URLSearchParams = new URLSearchParams();
    params.set("proveedorId", proveedorId.toString());
    return this.http
      .get("/api/vendedor/eliminarVendedor", { search: params })
      .pipe(
        timeoutWith(
          30000,
          observableThrowError(
            new Error(
              "Tiempo de respuesta agotado, por favor intentar nuevamente"
            )
          )
        ),
        map(this.extractData)
      );
  }
}
