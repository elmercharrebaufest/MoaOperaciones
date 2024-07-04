import { EventEmitter, Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable, Subject, throwError } from 'rxjs';
import { BaseService } from '../common/services/BaseService';
import { Solp } from './solp/solp';
import { EmailComposeModel } from '../common/email-compose/email-compose.model';
import { SolpPosicion } from './solp/solp-posicion';
import { EnvioSolpCompra, SolpCompraDto } from './solp-compra';
import { CircularDto } from '../modelos/circular-model';
import { PeticionDeOfertaCierreDto, PeticionDeOfertaDto, PeticionDeOfertaRevisionTecnicaDto, PeticionDeOfertaUsarioDto } from '../modelos/peticion-de-oferta-model';
import { AdjudicacionDto, AdjudicacionEdicionDto } from '../modelos/adjudicacion';
import { OrdenDeCompraSap } from '../modelos/ordenDeCompraSap';
import { RegistroInfoDto } from '../modelos/registro-info';
import { PeticionVisualizacionPrecioDto } from '../modelos/peticion-visualizar-precio-dto';
import { ChatExternoComprasDto, ChatInternoComprasDto, ChatProveedorDto, ChatsDto } from './chat-interno/chat-interno.interface';
import { VisitaObraDto } from '../modelos/infoVisitasDeObraDto';
import { catchError, timeoutWith } from 'rxjs/operators';
import { EntradaServicio } from '../common/models/entradaServicio';
import { FiltroDto } from './agrupar-po-th/agrupar-po-th-filtro-model'
import { SolpDto } from './agrupar-po-th/agrupar-po-th-model';

@Injectable({
    providedIn: 'root'
})
export class ComprasService extends BaseService {

    Date: Date;
    filtros = {
        pagina: 1,
        itemsPorPagina: 10,
        orden: "",
        ordenAscendente: false,
        columnaNombre: '',
        columna: "Id",
        nroSolp: "",
        nroPo: "",
        nombrePedido: "",
        fechaDesde: null,
        fechaHasta: null,
        estados: "",
        sap: true,
        mantenimiento: true,
        web: true,
        repoAutomatica: true,
        listarPendiente: true,
        contratoMarco: true,
        usuarioId: null,
        centros: "",
        grupoDeCompras: "",
        estadoLicitacion: null,
        estadoCotizacion: null,
        claseDocumento: "",
        tipoImputacion: "",
        valorTipoImputacion: "",
        tratada: null
    }
    listaSolp: any;
    observableListaSolp = new Subject<any[]>();
    observableListaPO = new Subject<any[]>();

    onDataUpdate: EventEmitter<void> = new EventEmitter<void>();

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
        repoAutomatica: boolean = this.filtros.repoAutomatica,
        contratoMarco: boolean = this.filtros.contratoMarco,
        estados: any = this.filtros.estados,
        usuarios: any = this.filtros.usuarioId,
        centros: any = this.filtros.centros,
        grupoDeCompras: any = this.filtros.grupoDeCompras,
        claseDocumento: any = this.filtros.claseDocumento,
        tipoImputacion: any = this.filtros.tipoImputacion,
        valorTipoImputacion: any = this.filtros.valorTipoImputacion): Observable<any> {
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
        params = params.set('repoAutomatica', repoAutomatica.toString());
        params = params.set('contratoMarco', contratoMarco.toString());
        params = params.set('estados', estados);
        params = params.set('usuarios', usuarios);
        params = params.set('centros', centros);
        params = params.set('grupoDeCompras', grupoDeCompras);
        params = params.set('claseDocumento', claseDocumento);
        params = params.set('tipoImputacion', tipoImputacion);
        params = params.set('valorTipoImputacion', valorTipoImputacion);
        return this.http.get('/api/compras/ListarSolp', { params: params, headers: this.headers });
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

    notifyDataUpdate() {
        this.onDataUpdate.emit();
    }

    public getByProveedor(
        fechaInicio: any = this.filtros.fechaDesde,
        fechaHasta: any = this.filtros.fechaHasta,
        proveedorId: string,
        ordenCompraId: string,
        columnaOrden: string = this.filtros.columnaNombre,
        ordenAscendente: boolean = this.filtros.ordenAscendente,
        pagina: number = this.filtros.pagina,
        elementosPorPagina: number = this.filtros.itemsPorPagina,
    ): Observable<any> {

        let params: HttpParams = new HttpParams();

        params = params.set('fechaInicio', (fechaInicio != null ? fechaInicio : ""));
        params = params.set('fechaHasta', (fechaHasta != null ? fechaHasta : ""));
        params = params.set('vendedor', proveedorId);
        params = params.set('ordenCompraId', ordenCompraId);
        params = params.set('columnaOrden', columnaOrden);
        params = params.set('ordenAscendente', ordenAscendente.toString());
        params = params.set('pagina', pagina.toString());
        params = params.set('elementosPorPagina', elementosPorPagina.toString());

        return this.http
            .get<any[]>('/api/Order/GetByProveedor', { params: params, headers: this.headers })
    }


    public getSolicitantesByNroSolped(
        solpList: string[]
    ): Observable<any> {

        let params: HttpParams = new HttpParams();

        solpList.forEach(param => {
            params = params.append('solpList', param);
          });

        return this.http
            .get<any[]>('/api/Order/GetSolicitantesByNroSolped', { params, headers: this.headers })
    }

    public getByProveedorAsync(
        fechaInicio: any = this.filtros.fechaDesde,
        proveedorId: string, 
        DocumentoNumero: string,
        columnaOrden: string = this.filtros.columnaNombre,
        ordenAscendente: boolean = this.filtros.ordenAscendente,
        pagina: number = this.filtros.pagina, 
        elementosPorPagina: number = this.filtros.itemsPorPagina,
        verTodo: boolean | false
        ) : Observable<any> {

        let params: HttpParams = new HttpParams();
        
        params = params.set('fechaInicio', (fechaInicio != null ? fechaInicio : ""));
        params = params.set('vendedor', proveedorId);
        params = params.set('documentoNumero', DocumentoNumero);
        params = params.set('ColumnaOrden', columnaOrden);
        params = params.set('OrdenAscendente', ordenAscendente.toString());
        params = params.set('pagina', pagina.toString());
        params = params.set('elementosPorPagina', elementosPorPagina.toString());
        params = params.set('verTodo', verTodo.toString());
        
        return this.http
            .get<any[]>('/api/EntradaServicio/GetByProveedorAsync', { params: params, headers: this.headers }).pipe(
                catchError(error => {
                    return throwError(error);
                })
            );
    }

    ObtenerESLocales(verTodo: boolean | false, proveedorId: string | '') {
        let params: HttpParams = new HttpParams();
        params = params.set('verTodo', verTodo.toString());
        params = params.set('vendedor', proveedorId);
        
        return this.http
            .get<any[]>('/api/EntradaServicio/ObtenerESLocales', { params: params, headers: this.headers }).pipe(
                catchError(error => {
                    return throwError(error);
                })
            );
    }

    public deleteById(Id,TempId, AccountingDate) {
        //http.delete falla en ambiente QA - cambiado a Post
        var payload = new FormData();
        if (TempId !== null && TempId !== '') {
            payload.append('DocumentoNumero', JSON.stringify(TempId));
        }
        else {
            payload.append('DocumentoNumero', JSON.stringify(Id));
            payload.append('FechaContabilizacion', AccountingDate);
        }


        return this.http
            .post('/api/EntradaServicio/DeleteById', payload, { headers: this.headersPost })
    }

    public postCreateAsync(parametros : any, report : any): Observable<any> {
        return this.http.post('/api/EntradaServicio/CreateAsync', {parametros, report})
          .pipe(
            timeoutWith(30000, throwError(new Error('Se excedió el tiempo de espera, por favor inténtelo más tarde')))
          );
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
            ProveedorAsignado_Id: solp.proveedorAsignado_Id,
            TrabajoYaHecho: solp.trabajoHecho,
            Adicional: solp.adicional,
            Urgencia: solp.urgencia,
            CondEspProveedorAsignado: solp.condEspProveedorAsignado,
            NroOrdenDeCompraAdicional: solp.ordenDeCompra,
            RevisadoPor: solp.revisadoPor,
            ClaseDocumento: this.getObjetoCodigo(solp.selectClaseDocumento && solp.selectClaseDocumento.Codigo),
            Finalizar: solp.Finalizar,
            LiberadoresSapSolp: solp.liberadoresSap,
            THServicioPermanente: solp.thServicioPermanente,
            THAjustePolinomica: solp.thAjustePolinomica,
            THProveedorDirecto: solp.thProveedorDirecto,
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
                    CodigoMaterialSap: this.getObjetoCodigo(x.codigoServicio && x.codigoServicio.Codigo),
                    CodigoServicioSap: this.getObjetoCodigo(x.codigoServicio && x.codigoServicio.Codigo),
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

    autocompleteProveedor(valor: string) {
        let params: HttpParams = new HttpParams()
            .append('valor', valor)

        return this.http
            .get<any[]>("/api/compras/AutocompleteProveedor", { params: params })
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

    autocompleteMaterialRFC(posicion: SolpPosicion) {
        let params: HttpParams = new HttpParams()
            .append('material', posicion.tareaSubcontratarObj.Codigo)
            .append('centro', posicion.selectCentroEntrega.Codigo)
            .append('grupoDeCompras', posicion.tareaSubcontratarObj.GrupoCompras.Codigo);

        return this.http
            .get<any[]>("/api/compras/AutocompleteMaterialRFC", { params: params })
    }

    listarUnidadesDeMedida(material: string) {
        let params: HttpParams = new HttpParams().append('material', material);

        return this.http.get<any[]>("/api/compras/ListarUnidadesDeMedida", { params: params })
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
        grupoDeCompras: any = this.filtros.grupoDeCompras,
        fechaDesde: any = this.filtros.fechaDesde,
        fechaHasta: any = this.filtros.fechaHasta,
        sap: boolean = this.filtros.sap,
        mantenimiento: boolean = this.filtros.mantenimiento,
        web: boolean = this.filtros.web,
        repoAutomatica: boolean = this.filtros.repoAutomatica,
        listarPendiente: boolean = this.filtros.listarPendiente,
        contratoMarco: boolean = this.filtros.contratoMarco,
        claseDocumento: any = this.filtros.claseDocumento,
        tipoImputacion: any = this.filtros.tipoImputacion,
        valorTipoImputacion: any = this.filtros.valorTipoImputacion) {
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
        params = params.set('fechaDesde', (fechaDesde != null ? fechaDesde : ""));
        params = params.set('fechaHasta', (fechaHasta != null ? fechaHasta : ""));
        params = params.set('sap', sap.toString());
        params = params.set('mantenimiento', mantenimiento.toString());
        params = params.set('web', web.toString());
        params = params.set('repoAutomatica', repoAutomatica.toString());
        params = params.set('listarPendiente', listarPendiente.toString());
        params = params.set('contratoMarco', contratoMarco.toString());
        params = params.set('claseDocumento', claseDocumento);
        params = params.set('tipoImputacion', tipoImputacion);
        params = params.set('valorTipoImputacion', valorTipoImputacion);
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
        nroSolp: string = this.filtros.nroSolp,
        nroPo: string = this.filtros.nroPo,
        nombrePedido: string = this.filtros.nombrePedido,
        estadoLicitacion: number | null = this.filtros.estadoLicitacion,
        estadoCotizacion: number | null = this.filtros.estadoCotizacion,
        fechaDesde: string = this.filtros.fechaDesde,
        fechaHasta: string | null = this.filtros.fechaHasta
    ) {
        let params: HttpParams = new HttpParams()
        pagina = pagina != null ? pagina : this.filtros.pagina;
        itemsPorPagina = itemsPorPagina != null ? itemsPorPagina : this.filtros.itemsPorPagina;
        columna = columna != "" ? columna : this.filtros.columna;
        params = params.set('pagina', pagina.toString());
        params = params.set('itemsPorPagina', itemsPorPagina.toString());
        params = params.set('orden', orden);
        params = params.set('columna', columna);
        params = params.set('nroSolp', nroSolp);
        params = params.set('nroPo', nroPo);
        params = params.set('nombrePedido', nombrePedido);
        params = params.set('estadoLicitacion', estadoLicitacion != null ? estadoLicitacion.toString() : null);
        params = params.set('estadoCotizacion', estadoCotizacion != null ? estadoCotizacion.toString() : null);
        params = params.set('fechaDesde', (fechaDesde != null ? fechaDesde : ""));
        params = params.set('fechaHasta', (fechaHasta != null ? fechaHasta : ""));
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

    public obtenerPosicionesMultipleCompras(ids: string) {
        let params: HttpParams = new HttpParams()
        params = params.set('ids', ids);
        return this.http
            .get<SolpCompraDto>('/api/compras/ObtenerPosicionesMultipleCompras', { params: params, headers: this.headers })

    }

    public GrabarPeticion(solp: EnvioSolpCompra) {
        let json = JSON.stringify({
            SolpId: solp.SolpId,
            PosIds: solp.PosIds,
            UsuarioIds: solp.UsuarioIds,
            Observacion: solp.Observacion,
            Adjuntos: solp.Adjuntos,
            AdjuntoPliego: solp.AdjuntoPliego
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

    verLegajo(idPeticionDeOferta: number, idPeticionDeOfertaUsuario: number, esProveedor): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("peticionDeOfertaId", idPeticionDeOferta.toString());
        if (idPeticionDeOfertaUsuario != null) {
            params = params.set("idPeticionDeOfertaUsuario", idPeticionDeOfertaUsuario.toString());
        }
        params = params.set("esProveedor", esProveedor.toString());

        return this.http
            .get("/api/compras/ObtenerLegajo", {
                params: params,
                headers: this.headers,
            });
    }

    verLegajoParaExternos(adjudicacionId: string, token: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("adjudicacionId", adjudicacionId);
        params = params.set("token", token);
        return this.http
            .get("/api/compras/ObtenerLegajoParaExternos", {
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
            PlazoDeOferta: circular.PlazoDeOfertaHora == null ? circular.PlazoDeOfertaFecha : this.getFechaHora(circular.PlazoDeOfertaFecha, circular.PlazoDeOfertaHora),
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

    DescargarAdjuntosCotizacion(cotizacionId: number, desdeRevisionTecnica: boolean): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("cotizacionId", cotizacionId.toString());
        params = params.set("desdeRevisionTecnica", desdeRevisionTecnica.toString());

        return this.http
            .get("/api/compras/DescargarAdjuntosCotizacion", {
                params: params,
                headers: this.headers,
            });
    }

    public grabarRevisionTecnica(peticiones: PeticionDeOfertaUsarioDto[], finalizar: boolean, revisionTecnica: PeticionDeOfertaRevisionTecnicaDto) {
        let json = JSON.stringify(peticiones);
        let jsonRevision = JSON.stringify(revisionTecnica);

        var payload = new FormData();
        payload.append('json', json);
        payload.append('finalizar', finalizar.toString());
        payload.append('jsonRevision', jsonRevision);

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
            Garantias: adjudicacion.Garantias,
            EsMonedaProveedor: adjudicacion.EsMonedaProveedor,
            Proveedor: adjudicacion.Proveedor,
            RegionSap: adjudicacion.RegionSap
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

    public listarPeticiones(id: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("solpId", id.toString());
        return this.http
            .get("/api/compras/ListarPeticionesDeOferta", {
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

    public GrabarPeticionDeOfertaVisualizacionPrecio(peticion: PeticionVisualizacionPrecioDto) {

        const jsonPayload = JSON.stringify({
            Observacion: peticion.Observacion,
            PeticionDeOferta_Id: peticion.PeticionOfertaId
        });
        const payload = new FormData();
        payload.append('json', jsonPayload);

        peticion.Adjuntos.forEach((fileToUpload: File) => {
            payload.append("filePeticionDeOfertaVisualizacionPrecio", fileToUpload, fileToUpload.name);
        });

        return this.http.post<any>('/api/compras/GrabarPeticionDeOfertaVisualizacionPrecio', payload, { headers: this.headers });
    }

    public obtenerReporteOrdenDeCompra(nroOC: string, fechaDesde: string, fechaHasta: string, codigoProveedor: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("nroOC", nroOC);
        params = params.set("fechaDesde", fechaDesde);
        params = params.set("fechaHasta", fechaHasta);
        params = params.set("codigoProveedor", codigoProveedor);

        return this.http
            .get("/api/compras/ObtenerReporteOrdenDeCompra", {
                params: params,
                headers: this.headers
            });
    }

    public obtenerChat(solpId: string): Observable<ChatsDto> {
        let params: HttpParams = new HttpParams();
        params = params.set("solpId", solpId);

        return this.http
            .get<ChatsDto>("/api/compras/ObtenerChat", {
                params: params,
                headers: this.headers
            });
    }

    public obtenerChatProveedor(peticionDeOfertaUsuarioId: string): Observable<ChatsDto> {
        let params: HttpParams = new HttpParams();
        params = params.set("peticionDeOfertaUsuarioId", peticionDeOfertaUsuarioId);

        return this.http
            .get<ChatsDto>("/api/compras/ObtenerChatProveedor", {
                params: params,
                headers: this.headers
            });
    }

    public grabarMensajeChatInterno(mensaje: ChatInternoComprasDto) {
        let json = JSON.stringify(mensaje);
        

        var payload = new FormData();
        payload.append('json', json);

        return this.http
            .post<ChatInternoComprasDto>('/api/compras/GrabarMensajeChatInterno', payload, { headers: this.headers });
    }

    public grabarMensajeChatExterno(mensaje: ChatExternoComprasDto) {
        let json = JSON.stringify(mensaje);

        var payload = new FormData();
        payload.append('json', json);

        return this.http
            .post<ChatExternoComprasDto>('/api/compras/GrabarMensajeChatExterno', payload, { headers: this.headers });
    }


    public obtenerYExportarChat(solpId: string, peticionDeOfertaUsuarioId?: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("solpId", solpId);
        if(peticionDeOfertaUsuarioId != null){
            params = params.set("peticionDeOfertaUsuarioId", peticionDeOfertaUsuarioId);
        }
        return this.http
            .get("/api/compras/ObtenerYExportarChat", {
                params: params,
                headers: this.headers
            });
    }

    public validarSolpTratada(nroSolp: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("nroSolp", nroSolp);

        return this.http
            .get("/api/compras/ValidarSolpTratada", {
                params: params,
                headers: this.headers
            });
    }

    devolverMonedaProveedor(codigoProveedor: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("codigoProveedor", codigoProveedor.toString());

        return this.http
            .get("/api/compras/DevolverMonedaProveedor", {
                params: params,
                headers: this.headers,
            });
    }

    validarSubposicionConDiferenteMoneda(adjudicacion: AdjudicacionDto) {
        let json = JSON.stringify({
            Cotizacion_Id: adjudicacion.Cotizacion_Id,
            AdjudicacionPosiciones: adjudicacion.AdjudicacionPosiciones,
            Solp_Id: adjudicacion.Solp_Id,
            TextoDeCabecera: adjudicacion.TextoDeCabecera,
            CondicionesDeEntrega: adjudicacion.CondicionesDeEntrega,
            CondicionesDePago: adjudicacion.CondicionesDePago,
            Garantias: adjudicacion.Garantias,
            EsMonedaProveedor: adjudicacion.EsMonedaProveedor,
            Proveedor: adjudicacion.Proveedor
        });

        var payload = new FormData();
        payload.append('json', json);

        return this.http
            .post<any>('/api/compras/ValidarSubposicionConMonedaDiferente', payload, { headers: this.headers });
    }

    public listarLiberadorSap(): Observable<any> {
        return this.http.get("/api/compras/ListarLiberadorSap", {
            headers: this.headers,
        });
    }

    public listarUsuarioCreadorSolp(): Observable<any> {
        return this.http.get("/api/compras/ListarUsuarioCreadorSolp", {
            headers: this.headers,
        });
    }

    public ModificarAdjudicacion(adjudicacion: AdjudicacionEdicionDto) {
        let json = JSON.stringify(adjudicacion);
        var payload = new FormData();
        payload.append('json', json);
        return this.http
            .post<any>('/api/compras/ModificarOrdenDeCompra', payload, { headers: this.headers });
    }

    public listarVisitasDeObra(listaVisitas: VisitaObraDto[]): Observable<any> {
        return this.http.post("/api/compras/ListarVisitasDeObra", listaVisitas, {
            headers: this.headers
        });
    }

    public listarTablaSap(codigos: string[]): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("codigos", codigos.toString());
        return this.http.get("/api/compras/ListarTablaSap", {
            params: params,
            headers: this.headers,
        });
    }

    public listarClaseDocumento(usuarioId: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("usuarioId", usuarioId.toString());
        return this.http.get("/api/compras/ListarClaseDocumento", {
            params: params,
            headers: this.headers,
        });
    }

    public actualizarProveedorVisibleEnSolicitante(peticionDeOfertaUsuarioId: number, esVisible: boolean) {
        var payload = new FormData();
        payload.append('peticionDeOfertaUsuarioId', peticionDeOfertaUsuarioId.toString());
        payload.append('esVisible', esVisible.toString());

        return this.http
            .post<any>('/api/compras/ActualizarProveedorVisibleEnSolicitante', payload, { headers: this.headers });
    }

    public obtenerHistorial(id: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("id", id.toString());
        return this.http.get("/api/compras/ObtenerHistorial", {
            params: params,
            headers: this.headers,
        });
    }

    listarUsuarioSolicitante(): Observable<any> {
        return this.http
            .get("/api/compras/ListarUsuarioSolicitante", {
                headers: this.headers,
            });
    }

    public listarPosicionesPOMultiple(
        fechaDesde: any,
        fechaHasta: any,
        sap: boolean = false,
        mantenimiento: boolean,
        web: boolean,
        repoAutomatica: boolean,
        contratoMarco: boolean,
        centros: any,
        grupoDeCompras: any,
        claseDocumento: any,
        tipoImputacion: any,
        valorTipoImputacion: any,
        tratada: boolean | null,

    ): Observable<any>
    {
        let params: HttpParams = new HttpParams();
        params = params.set('fechaDesde', (fechaDesde != null ? fechaDesde : ""));
        params = params.set('fechaHasta', (fechaHasta != null ? fechaHasta : ""));
        params = params.set('sap', sap.toString());
        params = params.set('mantenimiento', mantenimiento.toString());
        params = params.set('web', web.toString());
        params = params.set('repoAutomatica', repoAutomatica.toString());
        params = params.set('contratoMarco', contratoMarco.toString());
        params = params.set('centros', centros);
        params = params.set('grupoDeCompras', grupoDeCompras);
        params = params.set('claseDocumento', claseDocumento);
        params = params.set('tipoImputacion', tipoImputacion);
        params = params.set('valorTipoImputacion', valorTipoImputacion);
        params = params.set('tratada', tratada != null ? tratada.toString() : null);

        return this.http.get('/api/compras/ListarPosicionesPOMultiple', { params: params, headers: this.headers });
    }


    public enviarMotivoRechazoES(motivo: any): Observable<any> {
        let json = JSON.stringify(motivo);
        var payload = new FormData();
        payload.append('json', json);
        return this.http.post<any>("/api/EntradaServicio/RechazarEntradaDeServicio", payload, { headers: this.headers });
    }

    public enviarAprobacionES(NRO_ES_LOCAL: string): Observable<any> {
        var payload = new FormData();
        payload.append('nro_es_local', NRO_ES_LOCAL);
        return this.http.post<any>("/api/EntradaServicio/AprobarEntradaDeServicio", payload, { headers: this.headers })
            .pipe(
                timeoutWith(30000, throwError(new Error('Se excedio el tiempo de espera, por favor inténtelo más tarde'))),
                catchError(error => {
                    return throwError(error);
                })
            );
    }

    public reasignarSuplente(data: any): Observable<any> {
        var payload = new FormData();
        payload.append('nro_es_local', data.NroEsLocal);
        payload.append('suplente', data.Suplente);
        return this.http.post<any>("/api/EntradaServicio/ReasignarSuplente", payload, { headers: this.headers })
            .pipe(
                catchError(error => {
                    return throwError(error);
                })
            );
    }

    public enviarEdicionIngresante(data: any): Observable<any> {
        var payload = new FormData();
        payload.append('ID', data.ID);
        payload.append('ColumnaEditar', data.ColumnaEditar);
        payload.append('NuevoValor', data.NuevoValor);
        return this.http.post<any>("/api/EntradaServicio/ActualizarInformacionIngresante", payload, { headers: this.headers })
            .pipe(
                catchError(error => {
                    return throwError(error);
                })
            );
    }


    public listarHistorialDeFechas(id: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.set("peticionId", id.toString());
        return this.http.get("/api/compras/ListarHistorialDeFechas", {
            params: params,
            headers: this.headers,
        });
    }

    public marcarChatProveedorComoLeido(proveedor: ChatProveedorDto) {
        let json = JSON.stringify(proveedor);
        var payload = new FormData();
        payload.append('json', json);

        return this.http
            .post<any>('/api/compras/MarcarChatProveedorComoLeido', payload, { headers: this.headers });
    }
    
    public listarSolpCondicionEspecial(filtrosPOAgrupada: FiltroDto): Observable<SolpDto> {
        let filtroJson = JSON.stringify(filtrosPOAgrupada);
        let params: HttpParams = new HttpParams()
        params = params.set('filtroJson', filtroJson);
        return this.http
            .get<SolpDto>('/api/compras/ListarSolpCondicionEspecial', { params: params, headers: this.headers })
    }

    public agruparPeticionesDeOferta(peticionDeOfertaIds: number) {
        var payload = new FormData();
        payload.append('peticionDeOfertaIds', peticionDeOfertaIds.toString());

        return this.http
            .post<SolpDto>('/api/compras/AgruparPeticionesDeOferta', payload, { headers: this.headers });
    }

    public buildReportES(report : any): Observable<any> {

        return this.http
            .post<any>('/api/ReporteES/BuildReportES', report, { headers: this.headers });
    }
}