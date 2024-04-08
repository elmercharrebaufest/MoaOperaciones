import { HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, throwError as observableThrowError } from "rxjs";
import { timeoutWith } from "rxjs/operators";
import { ApiResponse } from "../common/models/response";
import { BaseService } from "../common/services/BaseService";
import { Material } from "../common/models/material";
import { ListarOrdenesResiduosResponse } from "../common/models/ordenes-residuos/listarOrdenesResiduosResponse";

@Injectable({
    providedIn: 'root'
})
export class OrdenesResiduosService extends BaseService {

    public getMateriales(): Observable<ApiResponse<Array<Material>>> {
        return this.http
            .get<ApiResponse<Array<Material>>>('/api/OrdenResiduos/ObtenerMateriales')
            .pipe(timeoutWith(360000,
                observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")))
            );
    }

    public obtenerListadoOrdenes(fechaInicio: string, fechaFin: string) :
        Observable<ApiResponse<ListarOrdenesResiduosResponse>> {

        let params : HttpParams = new HttpParams()
            .append("fechaInicio", fechaInicio)
            .append("fechaFin", fechaFin);

        return this.http
            .get<ApiResponse<ListarOrdenesResiduosResponse>>(
                '/api/OrdenResiduos/ObtenerListadoOrdenes',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000,
                observableThrowError(
                    new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")
                ))
            );
    }
}