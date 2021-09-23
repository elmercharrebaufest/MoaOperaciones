import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from '../common/services/BaseService';
import { Solp } from './Solp';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class ComprasService extends BaseService {


    public getCombos(): Observable<any> {
        return this.http
            .get('/api/compras/Combos', { headers: this.headers });
    }

    public getListarSolp(): Observable<any> {
        return this.http
            .get('/api/compras/ListarSolp', { headers: this.headers });
    }

    public borrarSolp(idSolp: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set('idSolp', idSolp.toString());

        return this.http
            .get('/api/compras/BorrarSolp', { params: params, headers: this.headers });
    }

    public traerSolpId(idSolp: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set('idSolp', idSolp.toString());

        return this.http
            .get('/api/compras/TraerSolpId', { params: params, headers: this.headers });
    }

    getPdf(idSolp): Observable<any> {
        return this.http
            .get("/api/compras/GenerarSolpPdf?idSolp=" + idSolp.toString(), {
                headers: this.headers,
            });
    }

    getEncodedPdf(idSolp): Observable<any> {

        return this.http
            .get("/api/compras/PreviewSolpPdf?idSolp=" + idSolp.toString(), {
                headers: this.headers,
            });
    }

    public GuardarSolp(solp: Solp) {
        let solpJson = JSON.stringify({
            Id: solp.id,
            TipoSolp: this.getObjetoCodigo(solp.tipoSolp),
            NombreDeObra: solp.nombreDePedido,
            FiscalContrato: solp.fiscalContrato,
            Telefono: solp.telefono,
            Email: solp.mail,
            FechaHoraEntrega: this.getFechaHora(solp.fechaEntrega, solp.horaEntrega),
            SupervisorSector: solp.supervisorSector,
            SupervisorTrabajo: solp.supervisorTrabajo,
            VisitasObraMasiva: solp.listaVisitas.map(x => { return { Codigo: x.id, FechaHora: this.getFechaHora(x.visitaDeObraFecha, x.visitaDeObraHora) } }),
            TieneVisitaObra: solp.visitaDeObra,
            TieneVisitaObraMasiva: solp.visitaDeObraMasiva,
            TieneObradores: solp.obradores,
            TieneMedioElevacion: solp.modoElevacion,
            TieneAndamio: solp.andamio, // Agregada
            TieneTecnicoSeguridad: solp.tecnicoSeguridad,
            TieneDescripcionTecnica: solp.descripcionTecnica,
            TieneDocumentacionTecnica: solp.entregaDocumentacion,
            FechaHoraLimiteConsulta: this.getFechaHora(solp.fechaLimiteFecha, solp.fechaLimiteHora),
            ObservacionesGeneracion: solp.observacionesGeneracion,
            EspecificacionesTecnicas: solp.especificacionesViewModel.observaciones,
            DiasEjecucion: solp.ejecucion,
            ObservacionesCotizacion: solp.observacionesCotizacion,
            JornadaLaboral: solp.jornadaLaboralDias.filter(x => x.selected).map(x => x.weekDay),
            JornadaLaboralDesde: solp.comienzoJornadaLaboral,
            JornadaLaboralHasta: solp.terminoJornadaLaboral,
            Adjuntos: solp.especificacionesViewModel.archivosGuardadosEspecificaciones.map(x => { return { Id: x.id } }),
            ClaseDocumento: this.getObjetoCodigo(solp.selectClaseDocumento && solp.selectClaseDocumento.Codigo),
            Posiciones: solp.posiciones.filter(x => x.textoGenerico).map(x => {
                return {
                    Codigo: x.id,
                    TextoGenerico: x.textoGenerico,
                    PlazoEntrega: x.plazoDeEntrega,
                    FechaEntregaServicio: x.fechaEntregaServicio,
                    FechaLiberacion: x.fechaDeLiberacion,
                    EsConcluido: x.concluido,
                    EsFijacion: x.indiceFijacion,
                    Centro: this.getObjetoCodigo(x.selectCentroEntrega && x.selectCentroEntrega.Codigo),
                    Almacen: this.getObjetoCodigo(x.selectAlmacenEntrega && x.selectAlmacenEntrega.Codigo),
                    NombreEntrega: x.nombreEntrega,
                    CalleEntrega: x.calleEntrega,
                    NumeroEntrega: x.numeroEntrega,
                    CpEntrega: x.codigoPostalEntrega,
                    PaisEntrega: x.paisEntrega,
                    GrupoCompras: this.getObjetoCodigo(x.selectGrupoCompras && x.selectGrupoCompras.Codigo),
                    Solicitante: x.selectSolicitanteCompras,
                    NroNecesidad: x.necesidadCompras,
                    GrupoArticulo: this.getObjetoCodigo(x.selectArticuloCompras && x.selectArticuloCompras.Codigo),
                    CodigosProveedores: this.getCodigosProveedores(x.rubroElectrico, x.rubroConsultoria, x.rubroCivil, x.rubroIngenieria, x.rubroMecanico),
                    Moneda: this.getObjetoCodigo(x.monedaSeleccionada && x.monedaSeleccionada.Codigo),
                    TipoImputacion: this.getObjetoCodigo(x.tipoImputacion),
                    TipoPosicion: this.getObjetoCodigo('SERVICIO'),

                    Subposiciones: x.listadoSubPosiciones ? x.listadoSubPosiciones.filter(sp => {
                        return !!((sp.codigoServicio && sp.codigoServicio.Codigo) ||
                            sp.tareaSubcontratar ||
                            (sp.cuentaMayor && sp.cuentaMayor.Codigo) ||
                            sp.cuentaTd ||
                            sp.precioBruto > 0 ||
                            (sp.unidadSeleccionada && sp.unidadSeleccionada.Codigo) ||
                            (sp.tipoImputacion && sp.tipoImputacion.Codigo));
                    }).map(sp => {
                        return {
                            Codigo: sp.id,
                            Numero: sp.subPosicion,
                            CodigoServicioSap: this.getObjetoCodigo(sp.codigoServicio && sp.codigoServicio.Codigo),
                            Tarea: sp.tareaSubcontratar,
                            CuentaMayor: this.getObjetoCodigo(sp.cuentaMayor && sp.cuentaMayor.Codigo),
                            Cantidad: sp.cuentaTd,
                            PrecioBruto: sp.precioBruto,
                            Unidad: this.getObjetoCodigo(sp.unidadSeleccionada && sp.unidadSeleccionada.Codigo),
                            TipoImputacionValor: this.getObjetoCodigo(sp.tipoImputacion && sp.tipoImputacion.Codigo, sp.tipoImputacion && sp.tipoImputacion.Tabla)
                        }
                    }) : null,
                    Proveedores: [
                        ...this.getProveedores(x.proveedoresValidos, 'VALIDO'),
                        ...this.getProveedores(x.proveedoresNoSugeridos, 'NOSUGERIDO'),
                        ...this.getProveedores(x.proveedoresInvalidos, 'INVALIDO')
                    ]
                }
            })
        });

        var payload = new FormData();

        var archivos = solp.especificacionesViewModel.archivosAdjuntosNuevos;
        if (archivos != null) {
            for (let i = 0; i < archivos.length; i++) {
                let fileToUpload = archivos[i];
                payload.append("file", fileToUpload, fileToUpload.name);
            }
        }

        payload.append('solpJson', solpJson);

        return this.http
            .post<Solp>('/api/compras/GuardarSolp', payload, { headers: this.headers });
    }

    getFechaHora(fecha: Date, hora: Date) {
        let fechaHora = new Date(fecha);
        fechaHora.setHours(hora.getHours());
        fecha.setMinutes(hora.getMinutes());
        fecha.setSeconds(hora.getSeconds());

        return fechaHora;
    }

    getCodigosProveedores(electrico, consultoria, civil, ingenieria, mecanico) {
        let list = [];

        if (electrico) list.push('ELECTRICO');
        if (consultoria) list.push('CONSULTORIA');
        if (civil) list.push('CIVIL');
        if (ingenieria) list.push('INGENIERIA');
        if (mecanico) list.push('MECANICO');

        return list.join(',');
    }

    getProveedores(proveedores, codigo) {
        if (proveedores)
            return proveedores.map(p => { return { RazonSocial: p, TipoFiltroProveedorSolp: this.getObjetoCodigo(codigo) } });

        return [];
    }

    getObjetoCodigo(codigo, tabla = null) {
        if (codigo) {
            if (tabla) {
                return { Codigo: codigo, Tabla: tabla }
            }

            return { Codigo: codigo }
        }

        return null;
    }

    DescargarArchivo(archivoId: number): Observable<any> {
        return this.http
            .get("/api/compras/DescargarArchivo?archivoId=" + archivoId.toString(), {
                headers: this.headers,
            });
    }

    descargarZipPliego(idSolp: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("solpId", idSolp.toString());

        return this.http
            .get("/api/compras/DescargarZipPliego", {
                params: params,
                headers: this.headers,
            });
    }

    autocompleteSap(tabla: string, valor: string) {
        let params: HttpParams = new HttpParams()
            .append('tabla', tabla)
            .append('valor', valor)

        return this.http
            .get<any[]>("/api/compras/AutocompleteTablaSap", { params: params })
    }

    obtenerDatosPorCodigosSap(codigos: any[]) {
        var payload = new FormData();
        payload.append('codigosSap', JSON.stringify(codigos));

        return this.http
            .post('/api/compras/ObtenerDatosPorCodigosSap', payload, { headers: this.headersPost });
    }
}
