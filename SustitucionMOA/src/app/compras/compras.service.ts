import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from '../common/services/BaseService';
import { Solp } from './solp/solp';
import { EmailComposeModel } from '../common/email-compose/email-compose.model';

@Injectable()
export class ComprasService extends BaseService {

    Date: Date

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
            TipoSolpSap: solp.tipoSolpSap,
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
            EspecificacionesTecnicas: solp.especificacionesViewModel.observaciones.replace(/(<img("[^"]*"|[^/">])*)>/gi, "$1/>"),
            TieneCondicionesGenerales: solp.tieneCondicionesGenerales,
            PasoCompletado: solp.pasoCompletado,
            EstadoPasos: solp.estadoPasos,
            UsuarioCompras: {
                Id: solp.usuarioComprasId
            },
            Adjuntos:   solp.especificacionesViewModel.archivosEspecificaciones.map(x => { return { Id: x.id } })
                .concat(solp.archivosCotizaciones.map(x => { return { Id: x.id } })),

            DiasEjecucion: solp.ejecucion,
            JornadaLaboral: solp.jornadaLaboralDias.filter(x => x.selected).map(x => x.weekDay),
            JornadaLaboralDesde: solp.comienzoJornadaLaboral,
            JornadaLaboralHasta: solp.terminoJornadaLaboral,
            ObservacionesCotizacion: solp.observacionesCotizacion,
            RevisadoPor: solp.revisadoPor,
            ClaseDocumento: this.getObjetoCodigo(solp.selectClaseDocumento && solp.selectClaseDocumento.Codigo),
            Finalizar: solp.Finalizar,
            Posiciones: solp.posiciones.map(x => {
                
                return {
                    Codigo: x.id,
                    PlazoEntrega: x.plazoDeEntrega,
                    FechaEntregaServicio: x.fechaEntregaServicio,
                    FechaLiberacion: x.fechaDeLiberacion,
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
                    //CodigosProveedores: this.getCodigosProveedores(x.rubroElectrico, x.rubroConsultoria, x.rubroCivil, x.rubroIngenieria, x.rubroMecanico),
                    Moneda: this.getObjetoCodigo(x.monedaSeleccionada && x.monedaSeleccionada.Codigo),
                    TipoImputacion: this.getObjetoCodigo(x.tipoImputacion && x.tipoImputacion.Codigo),
                    TipoPosicion: x.tipoPosicion != null ? this.getObjetoCodigo(x.tipoPosicion.Codigo) : null,
                    Estado: x.estado,
                    Indice: x.numeroPosicion,

                    TextoSuministro: x.textoSuministro,
                    Motivo: x.motivo,
                    Modelo: x.modelo,
                    CodigoMaterialSap: this.getObjetoCodigo(x.codigoServicio && x.codigoServicio.Codigo), // pepito

                    CodigoServicioSap: this.getObjetoCodigo(x.codigoServicio && x.codigoServicio.Codigo), // pepito
                    Tarea: x.tareaSubcontratar,
                    Cantidad: x.cuentaTd,
                    PrecioBruto: x.precioBruto,
                    Unidad: this.getObjetoCodigo(x.unidadSeleccionada && x.unidadSeleccionada.Codigo),

                    CuentaMayor: this.getObjetoCodigo(x.cuentaMayor && x.cuentaMayor.Codigo),
                    TipoImputacionValor: this.getObjetoCodigo(x.valorImputacion && x.valorImputacion.Codigo, x.valorImputacion && x.valorImputacion.Tabla),
                    Provincia: x.selectProvincia,


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

        var archivos = solp.especificacionesViewModel.archivosEspecificacionesNuevos;
        if (archivos != null) {
            for (let i = 0; i < archivos.length; i++) {
                let fileToUpload = archivos[i];
                payload.append("fileEspecificaciones", fileToUpload, fileToUpload.name);
              
            }
        }

        if (solp.archivosCotizacionesNuevos != null) {
            for (let i = 0; i < solp.archivosCotizacionesNuevos.length; i++) {
                let fileToUpload = solp.archivosCotizacionesNuevos[i];
                payload.append("fileCotizaciones", fileToUpload, fileToUpload.name);
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

    // getCodigosProveedores(electrico, consultoria, civil, ingenieria, mecanico) {
    //     let list = [];

    //     if (electrico) list.push('ELECTRICO');
    //     if (consultoria) list.push('CONSULTORIA');
    //     if (civil) list.push('CIVIL');
    //     if (ingenieria) list.push('INGENIERIA');
    //     if (mecanico) list.push('MECANICO');

    //     return list.join(',');
    // }

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

    autocompleteServicioSolp(valor: string) {
        let params: HttpParams = new HttpParams()
            .append('valor', valor)

        return this.http
            .get<any[]>("/api/compras/AutocompleteServicioSolp", { params: params })
    }

    autocompleteMaterialSolp(valor: string, centroId: number) {
        let params: HttpParams = new HttpParams()
            .append('valor', valor)
            .append('centroId', centroId.toString());

        return this.http
            .get<any[]>("/api/compras/AutocompleteMaterialSolp", { params: params })
    }

    obtenerDatosPorCodigosSap(codigos: any[]) {
        var payload = new FormData();
        payload.append('codigosSap', JSON.stringify(codigos));

        return this.http
            .post('/api/compras/ObtenerDatosPorCodigosSap', payload, { headers: this.headersPost });
    }

    obtenerDatosPorCodigosSapServicioSolp(codigos: string[]) {
        var payload = new FormData();
        payload.append('codigosSap', JSON.stringify(codigos));

        return this.http
            .post('/api/compras/ObtenerDatosPorCodigosSapServicioSolp', payload, { headers: this.headersPost });
    }

    obtenerUsuarioCompras(): Observable<any> {

        return this.http
            .get("/api/compras/ListarUsuarioCompras", {
                headers: this.headers,
            });
    }

    enviarEmail(emailCompose: EmailComposeModel) {
        var payload = new FormData();
        payload.append('emailCompose', JSON.stringify(emailCompose));
        return this.http
            .post<any>('/api/compras/EnviarEmail', payload, { headers: this.headersPost });
    }

    obtenerContratoMarco(centro: string, numeroContrato: string): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('numeroContrato', numeroContrato)        
            .append('centro', centro);

        return this.http
            .get("/api/compras/ObtenerContratoMarco", { params: params })
    }
}
