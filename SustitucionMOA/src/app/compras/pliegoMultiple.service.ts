import { BaseService } from "../common/services/BaseService";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { PliegoDto } from "./dashboard-pliego-multiple/pliegoDto.interface";
import { HttpParams, HttpClient } from "@angular/common/http";
import { SolpDto } from "./solp/steps/vincular-solp-pliego-multiple/solpDto.interface";
import { Solp } from "./solp/solp";
import { ComprasService } from "./compras.service";


@Injectable({
    providedIn: 'root'
})
export class PliegoMultipleService extends BaseService {

    constructor(protected comprasService: ComprasService, protected http: HttpClient) {
        super(http);
    }


    public getPliegoMultiple(nombrePliego?: string): Observable<PliegoDto[]> {
        let params: HttpParams = new HttpParams();
        if (nombrePliego) {
            params = params.set('nombrePliego', nombrePliego);
        }

        return this.http
            .get<PliegoDto[]>('/api/PliegoMultiple/GetPliegosMultiples', { params: params, headers: this.headers });
    }

    public getSolpDisponiblesPliegosMultiple(numeroSolp: string, fechaInicio: string, fechaFin: string, creador: string, fiscal: string, sap: boolean, mantenimiento: boolean): Observable<SolpDto[]> {
        let params: HttpParams = new HttpParams();
        if (numeroSolp) {
            params = params.set('numeroSolp', numeroSolp);
        }

        if (fechaInicio) {
            params = params.set('fechaInicio', fechaInicio);
        }

        if (fechaFin) {
            params = params.set('fechaFin', fechaFin);
        }

        if (creador) {
            params = params.set('creador', creador);
        }

        if (fiscal) {
            params = params.set('fiscal', fiscal);
        }

        if (sap) {
            params = params.set('sap', sap.toString());
        }

        if (mantenimiento) {
            params = params.set('mantenimiento', mantenimiento.toString());
        }

        return this.http
            .get<SolpDto[]>('/api/PliegoMultiple/GetSolpDisponiblesPliegosMultiple', { params: params, headers: this.headers });
    }

    public vincularSolpPliegoMultiple(pliegos: Solp, idSolps: number[]): Observable<any> {
        let solpJson = this.comprasService.armarSolpString(pliegos, false);

        var payload = new FormData();

        var archivos = pliegos.especificacionesViewModel.archivosEspecificacionesNuevos;
        if (archivos != null) {
            for (let i = 0; i < archivos.length; i++) {
                let fileToUpload = archivos[i];
                payload.append("fileEspecificaciones", fileToUpload, fileToUpload.name);

            }
        }

        if (pliegos.archivosCotizacionesNuevos != null) {
            for (let i = 0; i < pliegos.archivosCotizacionesNuevos.length; i++) {
                let fileToUpload = pliegos.archivosCotizacionesNuevos[i];
                payload.append("fileCotizaciones", fileToUpload, fileToUpload.name);
            }
        }

        if (pliegos.archivosCotizacionesNuevosCondEsp != null) {
            for (let i = 0; i < pliegos.archivosCotizacionesNuevosCondEsp.length; i++) {
                let fileToUpload = pliegos.archivosCotizacionesNuevosCondEsp[i];
                payload.append("fileCotizacionesCondEsp", fileToUpload, fileToUpload.name);
            }
        }

        payload.append('pliegoData', solpJson);
        payload.append('solpsAsociar', JSON.stringify(idSolps));

        return this.http
            .post<Solp>('/api/PliegoMultiple/CrearPliegoMultiple', payload, { headers: this.headers });
    }

    public eliminarPliegoMultiple(idPliego: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set('idPliego', idPliego.toString());
        return this.http
            .delete<any>('/api/PliegoMultiple/EliminarPliegoMultiple', { params: params, headers: this.headers });
    }

    public descargarZipPliego(idPliego: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("pliegoId", idPliego.toString());

        return this.http
            .get("/api/PliegoMultiple/DescargarZipPliego", {
                params: params,
                headers: this.headers,
            });
    }

    public traerPliegoId(idPliego: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set('idPliego', idPliego.toString());
        return this.http
            .get('/api/PliegoMultiple/TraerPliegoId', { params: params, headers: this.headers });
    }
}