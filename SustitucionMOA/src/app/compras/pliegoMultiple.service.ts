import { BaseService } from "../common/services/BaseService";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { PliegoDto } from "./dashboard-pliego-multiple/pliegoDto.interface";
import { HttpParams } from "@angular/common/http";
import { SolpDto } from "./solp/steps/vincular-solp-pliego-multiple/solpDto.interface";


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
}