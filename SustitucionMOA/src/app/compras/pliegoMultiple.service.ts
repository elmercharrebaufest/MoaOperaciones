import { BaseService } from "../common/services/BaseService";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { PliegoDto } from "./dashboard-pliego-multiple/pliegoDto.interface";
import { HttpParams } from "@angular/common/http";


@Injectable({
    providedIn: 'root'
})
export class PliegoMultipleService extends BaseService {
    public getPliegoMultiple(nombrePliego?: string): Observable<PliegoDto[]> {
        let params: HttpParams = new HttpParams();
        if (nombrePliego) {
            params = params.set('nombrePliego', nombrePliego);
        }

        return this.http
            .get<PliegoDto[]>('/api/PliegoMultiple/GetPliegosMultiples', { params: params, headers: this.headers });
    }
}