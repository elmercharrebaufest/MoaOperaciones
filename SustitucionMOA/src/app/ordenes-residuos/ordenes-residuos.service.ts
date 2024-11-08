import { HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, throwError as observableThrowError } from "rxjs";
import { timeoutWith } from "rxjs/operators";
import { ApiResponse } from "../common/models/response";
import { BaseService } from "../common/services/BaseService";
import { Material } from "../common/models/material";
import { ListarOrdenesResiduosResponse } from "../common/models/ordenes-residuos/listarOrdenesResiduosResponse";
import { LocalidadDto } from "../common/models/common/localidadDto";
import { Proveedor } from "../common/models/proveedor";
import { PatentesClienteDto } from "../common/models/ordenes-residuos/patentesClienteDto";
import { Planta } from "../common/models/ordenes-residuos/planta";
import { Domicilio } from "../common/models/ordenes-residuos/domicilio";
import { TransportesIds } from "../common/models/ordenes-residuos/transportesIds";
import { OrdenCargaResiduosDto } from "../common/models/ordenes-residuos/ordenCargaResiduosDto";
import { GrabarOrdenResponse } from "../common/models/ordenes-residuos/grabarOrdenResponse";
import { DestinoScato } from "../common/models/scato/destinoScato";

@Injectable({
    providedIn: 'root'
})
export class OrdenesResiduosService extends BaseService {
    
    // public getCorredores(): Observable<ApiResponse<Array<Proveedor>>> {
    //     return this.http
    //         .get<ApiResponse<Array<Proveedor>>>('/api/OrdenResiduos/ObtenerCorredores')
    //         .pipe(timeoutWith(360000,
    //             observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")))
    //     );
    // }
    
    public getClientesResiduos(): Observable<ApiResponse<Array<Proveedor>>> {
        return this.http
            .get<ApiResponse<Array<Proveedor>>>('/api/OrdenResiduos/ObtenerClientes')
            .pipe(timeoutWith(360000,
                observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")))
            );
    }

    public obtenerProveedor(idProveedor: number): Observable<ApiResponse<Proveedor>> {
        let params : HttpParams = new HttpParams()
            .append("idProveedor", idProveedor.toString());
    
        return this.http
            .get<ApiResponse<Proveedor>>(
                '/api/OrdenResiduos/ObtenerProveedor',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000,
                observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")))
            );
    }
    
    public getMateriales(): Observable<ApiResponse<Array<Material>>> {
        return this.http
            .get<ApiResponse<Array<Material>>>('/api/OrdenResiduos/ObtenerMateriales')
            .pipe(timeoutWith(360000,
                observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")))
            );
    }
    
    public getLocalidades(): Observable<ApiResponse<Array<LocalidadDto>>> {
        return this.http
            .get<ApiResponse<Array<LocalidadDto>>>('/api/OrdenResiduos/ObtenerLocalidades')
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

    public getPatentes(clienteId: number): Observable<ApiResponse<PatentesClienteDto>> {
        let params : HttpParams = new HttpParams()
            .append("clienteId", clienteId.toString());
        
        return this.http
            .get<ApiResponse<PatentesClienteDto>>(
                '/api/OrdenResiduos/ObtenerPatentes',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000,
                observableThrowError(
                    new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")
                ))
            );
    }

    public obtenerPlantas(cuit: string): Observable<ApiResponse<Planta[]>> {
        let params : HttpParams = new HttpParams()
            .append("cuit", cuit);
        
        return this.http
            .get<ApiResponse<Planta[]>>(
                '/api/OrdenResiduos/ObtenerPlantas',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000,
                observableThrowError(
                    new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")
                ))
            );
    }

    public obtenerDomicilios(cuit: string): Observable<ApiResponse<Domicilio[]>> {
        let params : HttpParams = new HttpParams()
            .append("cuit", cuit);
        
        return this.http
            .get<ApiResponse<Domicilio[]>>(
                '/api/OrdenResiduos/ObtenerDomicilios',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000,
                observableThrowError(
                    new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")
                ))
            );
    }

    public obtenerIdsTransportes(clienteId: number, patenteAcoplado: string): Observable<ApiResponse<TransportesIds>> {
        let params : HttpParams = new HttpParams()
            .append("clienteId", clienteId.toString())
            .append("patenteAcoplado", patenteAcoplado);
        
        return this.http
            .get<ApiResponse<TransportesIds>>(
                '/api/OrdenResiduos/ObtenerIdsTransportes',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000,
                observableThrowError(
                    new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")
                ))
            );
    }

    public esCuilCuitValido(cuilCuit: string): Observable<ApiResponse<boolean>> {
        let params : HttpParams = new HttpParams()
            .append("cuilCuit", cuilCuit.toString());
        
        return this.http
            .get<ApiResponse<boolean>>(
                '/api/OrdenResiduos/EsCuilCuitValido',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000,
                observableThrowError(
                    new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")
                ))
            );
    }

    public crearOrdenResiduos(ordenResiduos: OrdenCargaResiduosDto): Observable<ApiResponse<GrabarOrdenResponse>> {
        let payload = new FormData();
        payload.append("ordenResiduosJson", JSON.stringify(ordenResiduos));
        
        return this.http
            .post<ApiResponse<GrabarOrdenResponse>>('/api/OrdenResiduos/AgregarOrden', payload)
            .pipe(timeoutWith(360000,
                observableThrowError(
                    new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")
                ))
            );
    }

    public editarOrdenResiduos(ordenResiduos: OrdenCargaResiduosDto): Observable<ApiResponse<GrabarOrdenResponse>> {
        let payload = new FormData();
        payload.append("ordenResiduosJson", JSON.stringify(ordenResiduos));
        
        return this.http
            .post<ApiResponse<GrabarOrdenResponse>>('/api/OrdenResiduos/EditarOrden', payload)
            .pipe(timeoutWith(360000,
                observableThrowError(
                    new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")
                ))
            );
    }

    public obtenerOrdenResiduos(idOrden: number): Observable<ApiResponse<OrdenCargaResiduosDto>> {
        let params : HttpParams = new HttpParams()
            .append("idOrden", idOrden.toString());
        
        return this.http
            .get<ApiResponse<OrdenCargaResiduosDto>>(
                '/api/OrdenResiduos/ObtenerOrden',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000,
                observableThrowError(
                    new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")
                ))
            );
    }

    public verificarTransporte(ordenId: number): Observable<ApiResponse<OrdenCargaResiduosDto>> {
        let params : HttpParams = new HttpParams()
            .append("ordenId", ordenId.toString());
        
        return this.http
            .get<ApiResponse<OrdenCargaResiduosDto>>(
                '/api/OrdenResiduos/VerificarTransporte',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000,
                observableThrowError(
                    new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")
                ))
            );
    }

    public anularOrdenResiduos(ordenId: number): Observable<ApiResponse<OrdenCargaResiduosDto>> {
        let params : HttpParams = new HttpParams()
            .append("ordenId", ordenId.toString());
        
        return this.http
            .get<ApiResponse<OrdenCargaResiduosDto>>(
                '/api/OrdenResiduos/AnularOrden',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000,
                observableThrowError(
                    new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")
                ))
            );
    }

    public resolverSolicitudEdicion(ordenId: number, aprobarSolicitud: boolean): Observable<ApiResponse<OrdenCargaResiduosDto>> {
        let params : HttpParams = new HttpParams()
            .append("ordenId", ordenId.toString())
            .append("aprobarSolicitud", aprobarSolicitud.toString());
    
        return this.http
            .get<ApiResponse<OrdenCargaResiduosDto>>(
                '/api/OrdenResiduos/ActualizarSolicitudEdicion',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000,
                observableThrowError(
                    new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")
                ))
            );
    }

    public obtenerDestinosMercaderia(cuit: string): Observable<ApiResponse<DestinoScato[]>> {
        let params : HttpParams = new HttpParams()
            .append("cuit", cuit);
        
        return this.http
            .get<ApiResponse<DestinoScato[]>>(
                '/api/OrdenResiduos/ObtenerDestinosMercaderia',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000,
                observableThrowError(
                    new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde")
                ))
            );
    }
}