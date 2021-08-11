import { HttpHeaders, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import "rxjs/add/observable/throw";
import "rxjs/add/operator/catch";
import "rxjs/add/operator/map";
import { BaseService } from "../../services/BaseService";

@Injectable()
export class AutocompleteLocalidadService extends BaseService {

  searchLocalidad(query: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.append("Content-Type", "application/json");
    headers = headers.append("Accept", "q=0.8;application/json;q=0.9");
    headers = headers.append("Cache-control", "no-cache");
    headers = headers.append("Cache-control", "no-store");
    headers = headers.append("Expires", "0");
    headers = headers.append("Pragma", "no-cache");

    let params: HttpParams = new HttpParams();
    params = params.set("localidad", query);

    return this.http
      .get("/api/AltaEmpresaGranos/GetLocalidadCombo", {
        params: params,
        headers: headers,
      });
  }

  getLocalidadById(localidadId: number) {
    let headers = new HttpHeaders();
    headers = headers.append("Content-Type", "application/json");
    headers = headers.append("Accept", "q=0.8;application/json;q=0.9");
    headers = headers.append("Cache-control", "no-cache");
    headers = headers.append("Cache-control", "no-store");
    headers = headers.append("Expires", "0");
    headers = headers.append("Pragma", "no-cache");

    let params: HttpParams = new HttpParams();
    params = params.set("localidadId", localidadId.toString());

    return this.http
      .get("/api/AltaEmpresaGranos/GetLocalidad", {
        params: params,
        headers: headers,
      });
  }
}
