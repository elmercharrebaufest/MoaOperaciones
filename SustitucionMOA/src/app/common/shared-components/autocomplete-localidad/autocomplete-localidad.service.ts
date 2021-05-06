import { Injectable } from "@angular/core";
import { URLSearchParams } from "@angular/http";
import { Observable, throwError } from "rxjs";
import "rxjs/add/observable/throw";
import "rxjs/add/operator/catch";
import "rxjs/add/operator/map";
import { map, timeoutWith } from "rxjs/operators";
import { BaseService } from "../../services/BaseService";


@Injectable()
export class AutocompleteLocalidadService extends BaseService {


  searchLocalidad(query: string): Observable<any> {
    this.headers = new Headers();
    this.headers.append("Content-Type", "application/json");
    this.headers.append("Accept", "q=0.8;application/json;q=0.9");
    this.headers.append("Cache-control", "no-cache");
    this.headers.append("Cache-control", "no-store");
    this.headers.append("Expires", "0");
    this.headers.append("Pragma", "no-cache");

    let params: URLSearchParams = new URLSearchParams();
    params.set("localidad", query);

    return this.http
      .get("/api/AltaEmpresaGranos/GetLocalidadCombo", {
        search: params,
        headers: this.headers,
      })
      .pipe(map(this.extractData));
  }

  getLocalidadById(localidadId: number) {
    this.headers = new Headers();
    this.headers.append("Content-Type", "application/json");
    this.headers.append("Accept", "q=0.8;application/json;q=0.9");
    this.headers.append("Cache-control", "no-cache");
    this.headers.append("Cache-control", "no-store");
    this.headers.append("Expires", "0");
    this.headers.append("Pragma", "no-cache");

    let params: URLSearchParams = new URLSearchParams();
    params.set("localidadId", localidadId.toString());

    return this.http
      .get("/api/AltaEmpresaGranos/GetLocalidad", {
        search: params,
        headers: this.headers,
      })
      .pipe(map(this.extractData));
  }
}
