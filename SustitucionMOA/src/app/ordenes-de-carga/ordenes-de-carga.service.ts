
import { throwError as observableThrowError, Observable, BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith } from 'rxjs/operators';
import { OrdenDeCarga } from '../common/models/ordenes-de-carga/ordenDeCarga';
import { ObtenerContratosDisponiblesResponse } from '../common/models/ordenes-de-carga/obtenerContratosDisponiblesResponse';
import { ApiResponse } from '../common/models/response';
import { ValidarSisaCorredorClienteResponse } from '../common/models/ordenes-de-carga/validarSisaCorredorClienteResponse';
import { ValidarCuitExisteScatoResponse } from '../common/models/ordenes-de-carga/validarSisaCorredorClienteResponse copy';
import { Planta } from '../common/models/ordenes-de-carga/planta';
import { Domicilio } from '../common/models/ordenes-de-carga/domicilio';
import { ValidarIntermediarioFleteResponse } from '../common/models/ordenes-de-carga/ValidarIntermediarioFleteResponse';

@Injectable({
    providedIn: 'root'
})
export class OrdenesDeCargaService extends BaseService {
    private _ordenDeCargaSeleccionado = new BehaviorSubject<number>(0);
    private estadosFiltro = new BehaviorSubject<Array<string>>([
        "Pendiente",
        "Confirmado",
        "Pendiente aprobación crédito",
        "Entrega generada",
        "Anulada",
        "Vencida",
        "Entrega pendiente",
        "Anulada por vencimiento",
        "Anulación solicitada",
        "Edición solicitada",
        "Error de datos",
        "Contrato vencido",
        "Edición rechazada",
        "Sin Enviar a SAP",
        "Entrega anulada, pedido pendiente de anulación"
    ]);

    setOrdenDeCargaSeleccionado(value: number) {
        this._ordenDeCargaSeleccionado.next(value);
    }
    getOrdenDeCargaSeleccionado() {
        return this._ordenDeCargaSeleccionado.asObservable()
    }

    get estadosFiltrosSeleccionados(): Array<string> {
        return this.estadosFiltro.value;
    }
    setEstadosFiltro(value: Array<string>) {
        this.estadosFiltro.next(value)
    }
    public getOrdenDeCarga(ordenDeCargaId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenDeCargaId', ordenDeCargaId.toString());

        return this.http
            .get<OrdenDeCarga>('/api/OrdenDeCarga/Get', { params: params })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public getEditarOrdenDeCarga(ordenDeCargaId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenDeCargaId', ordenDeCargaId.toString());

        return this.http
            .get<OrdenDeCarga>('/api/OrdenDeCarga/GetEditar', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public getListado(fechaInicio: string, fechaFin: string): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('fechaInicio', fechaInicio)
            .append('fechaFin', fechaFin);

        return this.http
            .get<OrdenDeCarga[]>('/api/OrdenDeCarga/GetListado', { params: params })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public solicitarAnulacion(ordenDeCargaId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenDeCargaId', ordenDeCargaId.toString())

        return this.http
            .get<OrdenDeCarga[]>('/api/OrdenDeCarga/SolicitarAnulacion', { params: params })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public rechazarSolicitudAnulacion(ordenDeCargaId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenDeCargaId', ordenDeCargaId.toString())

        return this.http
            .get<OrdenDeCarga[]>('/api/OrdenDeCarga/RechazarSolicitudAnulacion', { params: params })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public solicitarEdicion(ordenDeCargaId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenDeCargaId', ordenDeCargaId.toString());

        return this.http
            .get<OrdenDeCarga[]>('/api/OrdenDeCarga/SolicitarEdicion', { params: params })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public rechazarSolicitudEdicion(ordenDeCargaId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenDeCargaId', ordenDeCargaId.toString())

        return this.http
            .get<OrdenDeCarga[]>('/api/OrdenDeCarga/RechazarSolicitudEdicion', { params: params })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public agregar(ordenDeCarga: OrdenDeCarga): Observable<any> {
        let payload = new FormData();
        console.log(ordenDeCarga)
        payload.append(
            "ordenDeCargaJson",
            JSON.stringify(ordenDeCarga)
        );
        console.log("payload:", payload)

        return this.http
            .post('/api/OrdenDeCarga/Agregar', payload)
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public editar(ordenDeCarga: OrdenDeCarga): Observable<any> {
        let payload = new FormData();
        console.log(ordenDeCarga)
        payload.append(
            "ordenDeCargaJson",
            JSON.stringify(ordenDeCarga)
        );
        console.log("payload:", payload)

        return this.http
            .post('/api/OrdenDeCarga/Editar', payload)
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public enviarOrdenesASAP(ordenesIds: Array<number>): Observable<ApiResponse<string>> {
        return this.http
            .post<ApiResponse<string>>
            ('/api/OrdenDeCarga/EnviarOrdenesASAP', ordenesIds);
    }
    public anular(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/AnularOrden', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public anularPorVencimiento(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/AnularOrdenPorVencimiento', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }
    public activarOC(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/ActivarOC', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public edicionFinalizada(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString())

        return this.http
            .get('/api/OrdenDeCarga/EdicionFinalizada', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public notificarTransporte(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/NotificarTransporte', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public obtenerContratos(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/ObtenerContratos', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public obtenerPedidos(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/ObtenerPedidos', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public obtenerCorredores(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/ObtenerCorredores', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public seleccionarContrato(ordenId: Number, contrato: string): Observable<any> {
        let payload = new FormData();
        payload.append(
            "contratoSAP",
            contrato
        );
        payload.append(
            "ordenId",
            ordenId.toString()
        );

        return this.http
            .post('/api/OrdenDeCarga/SeleccionarContrato', payload)
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }
    
    public seleccionarFactura(ordenId: Number, facturaSeleccionada: string): Observable<ApiResponse<any>> {
        let payload = new FormData();
        payload.append(
            "facturaSeleccionada",
            facturaSeleccionada
        );
        payload.append(
            "ordenId",
            ordenId.toString()
        );

        return this.http
            .post<ApiResponse<any>>('/api/OrdenDeCarga/SeleccionarFactura', payload)
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public seleccionarPedido(ordenId: Number, pedido: string): Observable<any> {
        let payload = new FormData();
        payload.append(
            "pedido",
            pedido
        );
        payload.append(
            "ordenId",
            ordenId.toString()
        );

        return this.http
            .post('/api/OrdenDeCarga/SeleccionarPedido', payload)
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public seleccionarCorredor(ordenId: Number, corredor: string): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString())
            .append('corredor', corredor);

        return this.http
            .get('/api/OrdenDeCarga/SeleccionarCorredor', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public verificarSituacionCrediticia(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/VerificarSituacionCrediticia', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public verificarTransporte(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/VerificarTransporte', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }


    public getMateriales(): Observable<any> {
        return this.http
            .get('/api/OrdenDeCarga/Materiales')
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }


    public forzarCreacionOrden(ordenId: Number): Observable<any> {
        let payload = new FormData();

        payload.append(
            "ordenId",
            ordenId.toString()
        );

        return this.http
            .post('/api/OrdenDeCarga/ForzarCreacionOrden', payload)
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public getPatentes(ordenDeCarga: OrdenDeCarga): Observable<any> {
        let payload = new FormData();
        // console.log(ordenDeCarga)
        payload.append(
            "ordenDeCargaJson",
            JSON.stringify(ordenDeCarga)
        );
        return this.http
            .post('/api/OrdenDeCarga/ObtenerPatentes', payload);
    }

    public visualizarCliente(codigoCorredor: string, fechaInicio: string, fechaFin: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append("corredor", codigoCorredor);
        params = params.append("fechaInicio", fechaInicio);
        params = params.append("fechaFin", fechaFin);
        params = params.append("pendiente", 'x');

        return this.http
            .get('/api/OrdenDeCarga/VisualizarCliente', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public visualizarProducto(contrato: string, fechaInicio: string, fechaFin: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        // params = params.append("clienteCuit", clienteCuit);
        params = params.append("contrato", contrato);
        params = params.append("fechaInicio", fechaInicio);
        params = params.append("fechaFin", fechaFin);
        params = params.append("pendiente", 'x');

        return this.http
            .get('/api/OrdenDeCarga/VisualizarProducto', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public validarCorredorClienteContratoProducto(clienteCuit: string, clienteCodigo: string, contrato: string, codigoCorredor: string, usuarioEmail: string, fechaInicio: string, fechaFin: string, productoId: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append("clienteCuit", clienteCuit);
        params = params.append("clienteCodigo", clienteCodigo);
        params = params.append("contrato", contrato);
        params = params.append("corredor", codigoCorredor);
        params = params.append("usuarioEmail", usuarioEmail);
        params = params.append("fechaInicio", fechaInicio);
        params = params.append("fechaFin", fechaFin);
        params = params.append("productoId", productoId);
        params = params.append("pendiente", 'x');

        return this.http
            .get('/api/OrdenDeCarga/validarCorredorClienteContratoProducto', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public obtenerContratosDisponibles(clienteCodigo: string, corredorCodigo: string, fechaDesde: string,
        fechaHasta: string): Observable<ObtenerContratosDisponiblesResponse> {

        let params: HttpParams = new HttpParams();
        params = params.append("clienteCodigo", clienteCodigo);
        params = params.append("corredorCodigo", corredorCodigo);
        params = params.append("fechaDesde", fechaDesde);
        params = params.append("fechaHasta", fechaHasta);

        return this.http
            .get<ObtenerContratosDisponiblesResponse>('/api/OrdenDeCarga/ObtenerContratosDisponibles', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }
    public validarExisteCuitScato(cuit: string): Observable<ApiResponse<ValidarCuitExisteScatoResponse>> {
        let params: HttpParams = new HttpParams();
        params = params.append("cuit", cuit);

        return this.http
            .get
            <ApiResponse<ValidarCuitExisteScatoResponse>>
            ('/api/OrdenDeCarga/ValidarCuitExisteScato', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }
    public validarSisaCuit(cuit: string, campo: string): Observable<ApiResponse<boolean>> {
        let params: HttpParams = new HttpParams()
            .append("cuit", cuit)
            .append("campo", campo)

        return this.http
            .get<ApiResponse<boolean>>(
                '/api/OrdenDeCarga/ValidarSisaCuit',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }
    public validarSisaCorredorCliente(corredorCodigo: string, clienteCodigo: string): Observable<ApiResponse<ValidarSisaCorredorClienteResponse>> {
        let params: HttpParams = new HttpParams()
            .append("corredorCodigo", corredorCodigo)
            .append("clienteCodigo", clienteCodigo);

        return this.http
            .get<ApiResponse<ValidarSisaCorredorClienteResponse>>(
                '/api/OrdenDeCarga/ValidarSisaCorredorCliente',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }
    public enviarMailGestionarAltaCuit(cuit: string, razonSocial: string, esIntermediarioFlete: boolean): Observable<ApiResponse<boolean>> {
        let params: HttpParams = new HttpParams()
            .append("cuit", cuit)
            .append("razonSocial", razonSocial)
            .append("esIntermediarioFlete", esIntermediarioFlete.toString());

        return this.http
            .get<ApiResponse<boolean>>(
                '/api/OrdenDeCarga/GestionarAltaCuit',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }
    public obtenerPlantasDestino(destinoCuit: string): Observable<ApiResponse<Planta[]>> {
        let params: HttpParams = new HttpParams()
            .append("destinoCuit", destinoCuit);

        return this.http
            .get<ApiResponse<Planta[]>>(
                '/api/OrdenDeCarga/ObtenerPlantasDestino',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
    }

    public obtenerDomiciliosDestino(destinoCuit: string): Observable<ApiResponse<Domicilio[]>> {
        let params: HttpParams = new HttpParams()
            .append("destinoCuit", destinoCuit);

        return this.http
            .get<ApiResponse<Domicilio[]>>(
                '/api/OrdenDeCarga/ObtenerDomiciliosDestino',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
    }
    public validarCuitRuca(cuit: string): Observable<ApiResponse<boolean>> {
        let params: HttpParams = new HttpParams()
            .append("cuit", cuit);

        return this.http
            .get<ApiResponse<boolean>>(
                '/api/OrdenDeCarga/ValidarCuitRuca',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
    }

    public validarIntermediarioFlete(cuit: string): Observable<ApiResponse<ValidarIntermediarioFleteResponse>> {
        let params: HttpParams = new HttpParams()
            .append("cuit", cuit);

        return this.http
            .get<ApiResponse<ValidarIntermediarioFleteResponse>>(
                '/api/OrdenDeCarga/ValidarIntermediarioFlete',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
    }
    public validarCuilChofer(cuil: string): Observable<ApiResponse<boolean>> {
        let params: HttpParams = new HttpParams()
            .append("cuilChofer", cuil);

        return this.http
            .get<ApiResponse<boolean>>(
                '/api/OrdenDeCarga/ValidarCuilChofer',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
    }
    public validarCuitTransporte(cuil: string): Observable<ApiResponse<boolean>> {
        let params: HttpParams = new HttpParams()
            .append("cuitTransporte", cuil);

        return this.http
            .get<ApiResponse<boolean>>(
                '/api/OrdenDeCarga/ValidarCuitTransporte',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
    }
    public obtenerFacturasDeContrato(numeroContrato: string): Observable<ApiResponse<Array<string>>> {
        let params: HttpParams = new HttpParams()
            .append("numeroContrato", numeroContrato);

        return this.http
            .get<ApiResponse<Array<string>>>(
                '/api/OrdenDeCarga/FacturasDisponibles',
                { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
    }
}
