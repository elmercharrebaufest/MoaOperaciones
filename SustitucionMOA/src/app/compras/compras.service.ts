import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { BaseService } from '../common/services/BaseService';
import { Solp } from './solp/solp';
import { EmailComposeModel } from '../common/email-compose/email-compose.model';
import { SolpPosicion } from './solp/solp-posicion';
import { AltaNuevoProveedor, EnvioSolpCompra, SolpCompraDto } from './solp-compra';
import { CircularDto } from '../modelos/circular-model';
import { PeticionDeOfertaCierreDto, PeticionDeOfertaDto, PeticionDeOfertaUsarioDto } from '../modelos/peticion-de-oferta-model';
import { AdjudicacionDto } from '../modelos/adjudicacion';
import { OrdenDeCompraSap } from '../modelos/ordenDeCompraSap';
import { RegistroInfoDto } from '../modelos/registro-info';

@Injectable({
    providedIn: 'root'
})
export class ComprasService extends BaseService {

    Date: Date;
    filtros = {
        pagina: 1,
        itemsPorPagina: 10,
        orden: "",
        columna: "Id",
        nroSolp: "",
        fechaDesde: null,
        fechaHasta: null,
        estados: "",
        sap: true,
        mantenimiento: true,
        web: true,
        usuarioId: null,
        centros: "",
        grupoDeCompras: ""
    }
    listaSolp: any;
    observableListaSolp = new Subject<any[]>();
    observableListaPO = new Subject<any[]>();


    public getCombos(): Observable<any> {
        return this.http
            .get('/api/compras/Combos', { headers: this.headers });
    }

    public obtenerUltimaSolp(): Observable<any> {
        return this.http
            .get('/api/compras/ObtenerUltimaSolp', { headers: this.headers });
    }

    public getListarSolp(pagina: number,
        itemsPorPagina: number,
        orden: string = this.filtros.orden,
        columna: string = this.filtros.columna,
        nroSolp: string = this.filtros.nroSolp,
        fechaDesde: any = this.filtros.fechaDesde,
        fechaHasta: any | null = this.filtros.fechaHasta,
        sap: boolean = this.filtros.sap,
        mantenimiento: boolean = this.filtros.mantenimiento,
        web: boolean = this.filtros.web,
        estados: any = this.filtros.estados,
        usuarioId: any = this.filtros.usuarioId): Observable<any> {
        let params: HttpParams = new HttpParams();
        pagina = pagina != null ? pagina : this.filtros.pagina;
        itemsPorPagina = itemsPorPagina != null ? itemsPorPagina : this.filtros.itemsPorPagina;
        columna = columna != "" ? columna : this.filtros.columna;
        params = params.set('pagina', pagina.toString());
        params = params.set('itemsPorPagina', itemsPorPagina.toString());
        params = params.set('orden', orden);
        params = params.set('columna', columna);
        params = params.set('nroSolp', nroSolp);
        params = params.set('fechaDesde', (fechaDesde != null ? fechaDesde : ""));
        params = params.set('fechaHasta', (fechaHasta != null ? fechaHasta : ""));
        params = params.set('sap', sap.toString());
        params = params.set('mantenimiento', mantenimiento.toString());
        params = params.set('web', web.toString());
        params = params.set('estados', estados);
        params = params.set('usuarioId', (usuarioId != null ? usuarioId.toString() : ""));
        return this.http
            .get('/api/compras/ListarSolp', { params: params, headers: this.headers });
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
    getPdfPeticionDeOfertaUsuario(idPeticionDeOfertaUsuario): Observable<any> {
        return this.http
            .get("/api/compras/GenerarPeticionDeOfertaUsuarioPdf?idPeticionDeOfertaUsuario=" + idPeticionDeOfertaUsuario.toString(), {
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
            TieneGrillaPersonal: solp.grillaPersonal,
            TieneFabricacionTallerExterno: solp.fabricacionTallerExterno,
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
            Adjuntos: solp.especificacionesViewModel.archivosEspecificaciones.map(x => { return { Id: x.id } })
                .concat(solp.archivosCotizaciones.map(x => { return { Id: x.id } })),

            DiasEjecucion: solp.ejecucion,
            JornadaLaboral: solp.jornadaLaboralDias.filter(x => x.selected).map(x => x.weekDay),
            JornadaLaboralDesde: solp.comienzoJornadaLaboral,
            JornadaLaboralHasta: solp.terminoJornadaLaboral,
            ObservacionesCotizacion: solp.observacionesCotizacion,
            ProveedorAsignadoId: solp.proveedorAsignado_Id,
            TrabajoYaHecho: solp.trabajoHecho,
            Adicional: solp.adicional,
            NroOrdenDeCompraAdicional: solp.ordenDeCompra,
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
                    ProveedorFijo: x.provedorFijo,
                    NombreProveedor: x.nombreProveedor,
                    NumeroContratoSuperior: x.numeroContratoSuperior,
                    NumeroPosicionContratoSuperior: x.numeroPosicionContratoSuperior,
                    OrganizacionCompras: x.orgCompras,
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

    public ListarFuenteAprovisionamiento(fecha: string, noMaterial: string, centro: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set('fechaEntregaPosicion', fecha);
        params = params.set('numeroMaterial', noMaterial);
        params = params.set('centro', centro);
        return this.http
            .get('/api/compras/ListarFuenteAprovisionamiento', { params: params, headers: this.headers });
    }

    public ObtenerContratoMarco(noContrato: string, centro: string): Observable<any> {

        let params: HttpParams = new HttpParams();
        params = params.set('numeroContrato', noContrato)
        params = params.set('centro', centro);

        return this.http
            .get('/api/compras/ObtenerContratoMarco', { params: params, headers: this.headers });
    }

    getFechaHora(fecha: Date, hora: Date) {
        let fechaHora = new Date(fecha);

        fechaHora.setHours(hora.getHours());
        fechaHora.setMinutes(hora.getMinutes());
        fechaHora.setSeconds(hora.getSeconds());
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

    getObjetoCodigo(codigo, tabla = null, Id = null) {
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

    autocompleteCodigoServicioSolp(valor: string) {
        let params: HttpParams = new HttpParams()
            .append('valor', valor)

        return this.http
            .get<any[]>("/api/compras/AutocompleteCodigoServicioSolp", { params: params })
    }

    autocompleteMaterialSolp(valor: string, centroId: number) {
        let params: HttpParams = new HttpParams()
            .append('valor', valor)
            .append('centroId', centroId.toString());

        return this.http
            .get<any[]>("/api/compras/AutocompleteMaterialSolp", { params: params })
    }

    autocompleteCodigoMaterialSolp(valor: string, centroId: number) {
        let params: HttpParams = new HttpParams()
            .append('valor', valor)
            .append('centroId', centroId.toString());

        return this.http
            .get<any[]>("/api/compras/AutocompleteCodigoMaterialSolp", { params: params })
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

    public getListarSolpCompras(pagina: number,
        itemsPorPagina: number,
        orden: string = this.filtros.orden,
        columna: string = this.filtros.columna,
        nroSolp: string = this.filtros.nroSolp,
        estados: any = this.filtros.estados,
        usuarios: any = this.filtros.usuarioId,
        centros: any = this.filtros.centros,
        grupoDeCompras: any = this.filtros.grupoDeCompras) {
        let params: HttpParams = new HttpParams()
        pagina = pagina != null ? pagina : this.filtros.pagina;
        itemsPorPagina = itemsPorPagina != null ? itemsPorPagina : this.filtros.itemsPorPagina;
        columna = columna != "" ? columna : this.filtros.columna;
        params = params.set('pagina', pagina.toString());
        params = params.set('itemsPorPagina', itemsPorPagina.toString());
        params = params.set('orden', orden);
        params = params.set('columna', columna);
        params = params.set('nroSolp', nroSolp);
        params = params.set('estados', estados);
        params = params.set('usuarios', usuarios);
        params = params.set('centros', centros);
        params = params.set('grupoDeCompras', grupoDeCompras);
        return this.http
            .get<any[]>('/api/compras/ListarSolpComprador', { params: params, headers: this.headers }).subscribe(
                (data: any[]) => {
                    this.observableListaSolp.next(data)
                }
            );
    }

    public getListarPOProveedor(pagina: number,
        itemsPorPagina: number,
        orden: string = this.filtros.orden,
        columna: string = this.filtros.columna,
        nroSolp: string = this.filtros.nroSolp) {
        let params: HttpParams = new HttpParams()
        pagina = pagina != null ? pagina : this.filtros.pagina;
        itemsPorPagina = itemsPorPagina != null ? itemsPorPagina : this.filtros.itemsPorPagina;
        columna = columna != "" ? columna : this.filtros.columna;
        params = params.set('pagina', pagina.toString());
        params = params.set('itemsPorPagina', itemsPorPagina.toString());
        params = params.set('orden', orden);
        params = params.set('columna', columna);
        params = params.set('nroSolp', nroSolp);
        return this.http
            .get<any[]>('/api/compras/ListarPOProveedor', { params: params, headers: this.headers }).subscribe(
                (data: any[]) => {
                    this.observableListaPO.next(data)
                }
            );
    }

    public getListarOfertasComprador(peticionOferta_Id): Observable<any> {
        let params: HttpParams = new HttpParams()
        params = params.set('peticionOferta_Id', peticionOferta_Id);
        return this.http
            .get<any[]>('/api/compras/ListarOfertasComprador', { params: params, headers: this.headers });
    }

    listarContratosAsociar(posiciones: SolpPosicion[]): Observable<any> {
        var json = posiciones.filter(x => x.codigoServicio != null).map(x => {
            return {
                FechaEntregaServicio: x.fechaEntregaServicio,
                Centro: { Id: x.selectCentroEntrega.Id, Codigo: x.selectCentroEntrega.Codigo },
                Indice: x.numeroPosicion,
                ProveedorFijo: x.provedorFijo,
                NumeroContratoSuperior: x.numeroContratoSuperior,
                CodigoMaterialSap: this.getObjetoCodigo(x.codigoServicio && x.codigoServicio.Codigo),
                Tarea: x.tareaSubcontratar,

            }
        });

        var solpJson = JSON.stringify(json);
        var payload = new FormData();
        payload.append('solpJson', solpJson);
        return this.http
            .post("/api/compras/ListarAsociarContrato", payload, { headers: this.headers });
    }

    public obtenerSolpCompras(id: number) {
        let params: HttpParams = new HttpParams()
        params = params.set('id', id.toString());
        return this.http
            .get<SolpCompraDto>('/api/compras/ObtenerSolpCompras', { params: params, headers: this.headers })

    }

    public GrabarPeticion(solp: EnvioSolpCompra) {
        let json = JSON.stringify({
            SolpId: solp.SolpId,
            PosIds: solp.PosIds,
            UsuarioIds: solp.UsuarioIds,
            Observacion: solp.Observacion,
            Adjuntos: solp.Adjuntos

        });

        var payload = new FormData();
        var archivos = solp.Adjuntos;
        if (archivos != null) {
            for (let i = 0; i < archivos.length; i++) {
                let fileToUpload = archivos[i];
                try {
                    payload.append("filePeticionDeOferta", fileToUpload as File, fileToUpload.name);
                } catch (e) {

                    console.log(e);
                }
            }
        }

        payload.append('json', json);

        return this.http
            .post<any>('/api/compras/GrabarPeticionDeOferta', payload, { headers: this.headers });
    }

    public listarProveedores(filtro: string) {
        let params: HttpParams = new HttpParams()
        params = params.set('filtro', filtro);
        return this.http
            .get<SolpCompraDto>('/api/compras/ListarProveedores', { params: params, headers: this.headers })
    }

    verLegajo(idPeticionDeOferta: number, idPeticionDeOfertaUsuario: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("peticionDeOfertaId", idPeticionDeOferta.toString());
        if (idPeticionDeOfertaUsuario != null) {
            params = params.set("idPeticionDeOfertaUsuario", idPeticionDeOfertaUsuario.toString());
        }
        return this.http
            .get("/api/compras/ObtenerLegajo", {
                params: params,
                headers: this.headers,
            });
    }

    descargarArchivo(idArchivo: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("idArchivo", idArchivo.toString());

        return this.http
            .get("/api/compras/DescargarArchivo", {
                params: params,
                headers: this.headers,
            });
    }

    adjuntarArchivoLegajo(idPeticion: number, files: any): Observable<any> {
        var payload = new FormData();

        for (let i = 0; i < files.length; i++) {
            let fileToUpload = files[i];
            payload.append("files", fileToUpload, fileToUpload.name);
        }


        payload.append('idPeticion', idPeticion.toString());

        return this.http
            .post<Solp>('/api/compras/GuardarAdjuntosPeticionDeOferta', payload, { headers: this.headers });

    }

    descargarLegajo(idPeticion: number, idPeticionDeOfertaUsuario: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("idPeticion", idPeticion.toString());
        if (idPeticionDeOfertaUsuario != null) {
            params = params.set("idPeticionDeOfertaUsuario", idPeticionDeOfertaUsuario.toString());
        }
        return this.http
            .get("/api/compras/DescargarLegajo", {
                params: params,
                headers: this.headers,
            });
    }

    public GrabarCircular(circular: CircularDto) {
        let json = JSON.stringify({
            UsuarioIds: circular.UsuarioIds,
            Observacion: circular.Observacion,
            Adjuntos: circular.Adjuntos,
            PlazoDeOferta: this.getFechaHora(circular.PlazoDeOfertaFecha, circular.PlazoDeOfertaHora),
            FechaEntrega: circular.FechaEntrega,
            RequiereCambioDeFecha: circular.RequiereCambioDeFecha,
            PeticionDeOferta_Id: circular.PeticionDeOferta_Id,
        });

        var payload = new FormData();
        var archivos = circular.Adjuntos;
        if (archivos != null) {
            for (let i = 0; i < archivos.length; i++) {
                let fileToUpload = archivos[i];
                try {
                    payload.append("fileCircular", fileToUpload as File, fileToUpload.name);
                } catch (e) {

                    console.log(e);
                }
            }
        }

        payload.append('json', json);

        return this.http
            .post<any>('/api/compras/GrabarCircular', payload, { headers: this.headers });
    }

    obtenerPeticionDeOferta(idPeticionDeOferta: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("peticionDeOfertaId", idPeticionDeOferta.toString());
        return this.http
            .get("/api/compras/ObtenerPeticionDeOferta", {
                params: params,
                headers: this.headers,
            });
    }

    public GrabarProveedorEnPeticion(usuariosId, peticion) {
        let json = JSON.stringify({
            UsuarioIds: usuariosId,
            Id: peticion

        });
        var payload = new FormData();
        payload.append('json', json);

        return this.http
            .post<any>('/api/compras/GrabarProveedorEnPeticion', payload, { headers: this.headers });
    }

    DescargarAdjuntosCotizacion(cotizacionId: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("cotizacionId", cotizacionId.toString());

        return this.http
            .get("/api/compras/DescargarAdjuntosCotizacion", {
                params: params,
                headers: this.headers,
            });
    }

    public grabarRevisionTecnica(petisiones: PeticionDeOfertaUsarioDto[]) {
        let json = JSON.stringify(petisiones);
        var payload = new FormData();
        payload.append('json', json);

        return this.http
            .post<any>('/api/compras/GrabarRevisionTecnica', payload, { headers: this.headers });
    }

    public obtenerCotizacion(id: number) {
        let params: HttpParams = new HttpParams()
        params = params.set('peticionDeOfertaId', id.toString());
        return this.http
            .get<PeticionDeOfertaDto>('/api/compras/ObtenerCotizacion', { params: params, headers: this.headers })

    }

    public GrabarCotizacion(cotizacion: any, esFinalizado: boolean) {
        let json = JSON.stringify({
            CotizacionId: cotizacion.CotizacionId,
            PeticionOfertaUsuarioId: cotizacion.PeticionOfertaUsuarioId,
            CotizacionPosiciones: cotizacion.CotizacionPosiciones,
            ObservacionEconomica: cotizacion.ObservacionEconomica,
            ObservacionTecnica: cotizacion.ObservacionTecnica,
            ArchivosNuevos: cotizacion.ArchivosNuevos,
            ArchivosGuardados: cotizacion.ArchivosGuardados,
            EsFinalizado: esFinalizado,
            RespetaServicios: cotizacion.RespetaServicios,
            RespetaMateriales: cotizacion.RespetaMateriales,
            CotizacionesHoras: cotizacion.CotizacionesHoras,
            CotizacionSubposiciones: cotizacion.CotizacionSubposiciones,
            PorcentajeDeHoras: cotizacion.PorcentajeDeHoras
        });

        var payload = new FormData();
        var archivos = cotizacion.ArchivosNuevos;
        if (archivos != null) {
            for (let i = 0; i < archivos.length; i++) {
                let fileToUpload = archivos[i];
                try {
                    payload.append("fileCotizacionRevisionEconomica", fileToUpload as File, fileToUpload.name);
                } catch (e) {

                    console.log(e);
                }
            }
        }

        var archiTecnico = cotizacion.ArchivosTecnico;

        if (archiTecnico != null) {
            for (let i = 0; i < archiTecnico.length; i++) {
                let fileToUpload = archiTecnico[i];
                try {
                    payload.append("fileCotizacionRevisionTecnica", fileToUpload as File, fileToUpload.name);
                } catch (e) {

                    console.log(e);
                }
            }
        }


        payload.append('json', json);

        return this.http
            .post<any>('/api/compras/GrabarCotizacion', payload, { headers: this.headers });
    }

    public obtenerPrecioTotalPosicionProveedor(cotizacion: any) {
        let json = JSON.stringify({
            CotizacionId: cotizacion.CotizacionId,
            PeticionOfertaUsuarioId: cotizacion.PeticionOfertaUsuarioId,
            CotizacionPosiciones: cotizacion.CotizacionPosiciones,
            CotizacionesHoras: cotizacion.CotizacionesHoras,
            CotizacionSubposiciones: cotizacion.CotizacionSubposiciones
        });

        var payload = new FormData();
        payload.append('json', json);

        return this.http
            .post<any>('/api/compras/ObtenerPrecioTotalPosicionProveedor', payload, { headers: this.headers });
    }

    public GrabarAdjudicacion(adjudicacion: AdjudicacionDto) {
        let json = JSON.stringify({
            Cotizacion_Id: adjudicacion.Cotizacion_Id,
            AdjudicacionPosiciones: adjudicacion.AdjudicacionPosiciones,
            Solp_Id: adjudicacion.Solp_Id,
            TextoDeCabecera: adjudicacion.TextoDeCabecera,
            CondicionesDeEntrega: adjudicacion.CondicionesDeEntrega,
            CondicionesDePago: adjudicacion.CondicionesDePago,
            Garantias: adjudicacion.Garantias

        });

        var payload = new FormData();
        payload.append('json', json);

        return this.http
            .post<any>('/api/compras/CrearOrdenDeCompra', payload, { headers: this.headers });
    }

    public listarAdjudicaciones(id: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("solpId", id.toString());
        return this.http
            .get("/api/compras/ListarAdjudicaciones", {
                params: params,
                headers: this.headers,
            });
    }

    public obtenerAdjudicacion(nroOC: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("nroOC", nroOC.toString());
        return this.http
            .get("/api/compras/ObtenerAdjudicacion", {
                params: params,
                headers: this.headers,
            });
    }

    public obtenerOrdenDeCompra(nroOC: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("nroOC", nroOC.toString());
        return this.http
            .get("/api/compras/ObtenerOrdenDeCompra", {
                params: params,
                headers: this.headers,
            });
    }

    // public obtenerOrdenDeCompra(filtro: string) {
    //     let params: HttpParams = new HttpParams()
    //     params = params.set('filtro', filtro);
    //     return this.http
    //         .get<OrdenDeCompraSap>('/api/compras/ObtenerOrdenDeCompra', { params: params, headers: this.headers })
    // }

    public guardarAdjudicacionAutomatica(registrosInfo: RegistroInfoDto[]) {
        let json = JSON.stringify(registrosInfo);
        var payload = new FormData();
        payload.append('json', json);

        return this.http
            .post<any>('/api/compras/GuardarAdjudicacionAutomatica', payload, { headers: this.headers });
    }

    public cerrarCotizacion(peticionId: number, observaciones: string) {
        var payload = new FormData();
        payload.append('peticionId', peticionId.toString());
        payload.append('observaciones', observaciones);

        return this.http
            .post<any>('/api/compras/CerrarCotizacion', payload, { headers: this.headers });
    }


}