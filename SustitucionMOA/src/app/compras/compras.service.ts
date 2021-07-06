import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from '../common/services/BaseService';
import { Solp } from './Solp';
import { Http, Response, URLSearchParams } from '@angular/http';

@Injectable()
export class ComprasService extends BaseService {

    public getCombos(): Observable<any> {
        return this.http
            .get('/api/compras/Combos', { headers: this.headers }).pipe(
            map(this.extractData));
    }

    public getListarSolp(): Observable<any> {
        return this.http
            .get('/api/compras/ListarSolp', { headers: this.headers }).pipe(
            map(this.extractData));
    }



    public borrarSolp(idSolp: number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('idSolp', idSolp.toString());
        return this.http
            .get('/api/compras/BorrarSolp', {  search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public traerSolpId(idSolp: number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('idSolp', idSolp.toString());
        return this.http
            .get('/api/compras/TraerSolpId', {  search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public GuardarSolp(solp: Solp) {
        let solpJson = JSON.stringify({
            Id: solp.id,
            nombreDePedido: solp.nombreDePedido,
            FiscalContrato: solp.fiscalContrato,
            Telefono: solp.telefono,
            Email: solp.mail,
            FechaHoraEntrega: this.getFechaHora(solp.fechaEntrega, solp.horaEntrega),
            SupervisorSector: solp.supervisorSector,
            SupervisorTrabajo: solp.supervisorTrabajo,
            VisitasObraMasiva: solp.listaVisitas.map(x=> { return {Codigo: x.id, FechaHora: this.getFechaHora(x.visitaDeObraFecha, x.visitaDeObraHora) }}),
            TieneVisitaObra: solp.visitaDeObra,
            TieneVisitaObraMasiva: solp.visitaDeObraMasiva,
            TieneObradores: solp.obradores,
            TieneMedioElevacion: solp.modoElevacion,
            TieneTecnicoSeguridad: solp.tecnicoSeguridad,
            TieneDescripcionTecnica: solp.descripcionTecnica,
            TieneDocumentacionTecnica: solp.entregaDocumentacion,
            FechaHoraLimiteConsulta: this.getFechaHora(solp.fechaLimiteFecha, solp.fechaLimiteHora),
            ObservacionesGeneracion: solp.observacionesGeneracion,
            EspecificacionesTecnicas: solp.especificacionesViewModel.observaciones,
            DiasEjecucion: solp.ejecucion,
            ObservacionesCotizacion: solp.observacionesCotizacion,
            JornadaLaboral: solp.jornadaLaboralDias.filter(x=>x.selected).map(x=>x.weekDay),
            JornadaLaboralDesde: solp.comienzoJornadaLaboral,
            JornadaLaboralHasta: solp.terminoJornadaLaboral,
            Adjuntos: solp.especificacionesViewModel.archivosGuardadosEspecificaciones.map(x=>x.id),
            Posiciones: solp.posiciones.map(x=> {
                return {
                    Codigo: x.id,
                    TextoGenerico: x.textoGenerico,
                    PlazoEntrega: x.plazoDeEntrega,
                    FechaEntregaServicio: x.fechaEntregaServicio,
                    FechaLiberacion: x.fechaDeLiberacion,
                    EsConcluido: x.concluido,
                    EsFijacion: x.indiceFijacion,
                    Centro: { Codigo: x.selectCentroEntrega.Codigo },
                    Almacen: { Codigo: x.selectAlmacenEntrega.Codigo },
                    NombreEntrega: x.nombreEntrega,
                    CalleEntrega: x.calleEntrega,
                    NumeroEntrega: x.numeroEntrega,
                    CpEntrega: x.codigoPostalEntrega,
                    PaisEntrega: x.paisEntrega,
                    GrupoCompras: { Codigo: x.selectGrupoCompras.Codigo },
                    Solicitante: x.selectSolicitanteCompras,
                    NroNecesidad: x.necesidadCompras,
                    GrupoArticulo: { Codigo: x.selectArticuloCompras.Codigo },
                    CodigosProveedores: `ELECTRICO:${x.rubroElectrico}|CIVIL:${x.rubroCivil}|CONSULTORIA:${x.rubroConsultoria}|INGENIERIA:${x.rubroIngenieria}|MECANICO:${x.rubroMecanico}`,
                    Moneda: { Codigo: x.selectMonedaCompras.Codigo },
                    TipoImputacion: { Codigo: x.tipoImputacion },
                    TipoPosicion: { Codigo: 'SERVICIO' },

                    Subposiciones: x.listadoSubPosiciones.map(sp => {
                        return {
                            Codigo: sp.id,
                            Numero: sp.subPosicion,
                            CodigoServicioSap: { Codigo: sp.codigoServicio },
                            Tarea: sp.tareaSubcontratar,
                            CuentaMayor: sp.cuentaMayor,
                            Cantidad: sp.cuentaTd,
                            Unidad: { Codigo: sp.unidadSeleccionada.Codigo },
                            TipoImputacionValor: sp.tipoImputacion
                        }
                    }),
                    Proveedores: [
                        ...x.proveedoresValidos.map(p => { return { RazonSocial: p, TipoFiltroProveedorSolp: { Codigo: 'VALIDO'} } }),
                        ...x.proveedoresNoSugeridos.map(p => { return { RazonSocial: p, TipoFiltroProveedorSolp: { Codigo: 'NOSUGERIDO'} } }),
                        ...x.proveedoresInvalidos.map(p => { return { RazonSocial: p, TipoFiltroProveedorSolp: { Codigo: 'INVALIDO'} } })
                    ]
                }
            })
        });
        var payload = new FormData();

        var archivos = solp.especificacionesViewModel.archivosAdjuntosNuevos;
        if(archivos != null)
        {
            for (let i = 0; i < archivos.length; i++) {
                let fileToUpload = archivos[i];
                payload.append("file", fileToUpload, fileToUpload.name);
            }
        }

        payload.append('solpJson', solpJson);
        
        return this.http
            .post('/api/compras/GuardarSolp',  payload , this.headers).pipe(
                map(this.extractData));
    }

    getFechaHora(fecha: Date, hora: Date){
        return new Date(fecha.getFullYear(), fecha.getMonth(), fecha.getDay(), hora.getHours(), hora.getMinutes(), hora.getSeconds(), 0);
    }

    DescargarArchivo(archivoId: number): Observable<any> {
        this.headers = new Headers();
        this.headers.append("Content-Type", "application/json");
        this.headers.append("Accept", "q=0.8;application/json;q=0.9");
        this.headers.append("Cache-control", "no-cache");
        this.headers.append("Cache-control", "no-store");
        this.headers.append("Expires", "0");
        this.headers.append("Pragma", "no-cache");
        return this.http
            .get("/api/compras/DescargarArchivo?archivoId=" + archivoId.toString(), {
                headers: this.headers,
            })
            .pipe(map(this.extractData));
    }
}