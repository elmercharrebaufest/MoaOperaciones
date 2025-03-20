import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ConfirmationService, SelectItem } from 'primeng/api';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { UsuarioService } from '../../../usuario/usuario.service';
import { ComprasService } from '../../compras.service';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { Table } from 'primeng/table';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { PeticionDeOfertaDto, PeticionDeOfertaSolpPosicionDto, PeticionDeOfertaUsarioDto } from '../../../modelos/peticion-de-oferta-model';
import { Solp } from '../../solp/solp';
import { CotizacionHoraDto, CotizacionDto } from '../../../modelos/cotizacionDto';
import { AdjudicacionDto } from '../../../modelos/adjudicacion';
import { TextosAdjudicarComponent } from './textos-adjudicar/textos-adjudicar.component';
import { CotizacionHistorialDto } from '../../../modelos/cotizacion-historial-model';
import { forkJoin } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { ApiResponse } from '../../../common/models/response';
import { Permiso } from '../../../common/enums/Permisos';

@Component({
    selector: 'app-ver-ofertas',
    templateUrl: './ver-ofertas.component.html',
    styleUrls: ['./ver-ofertas.component.css']
})
export class VerOfertasComponent extends ListBaseComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;

    @ViewChild("tabla")
    protected tabla: Table;

    @ViewChild("textoAdjudicar")
    protected modalTexto: TextosAdjudicarComponent;

    @Input()
    public peticion: PeticionDeOfertaDto;
    peticionOferta: PeticionDeOfertaDto;
    SolpDto: Solp;
    tablaOfertas: PeticionDeOfertaDto;
    adjudicacion: AdjudicacionDto;
    Cotizacion: CotizacionDto;
    displayTextos: boolean;
    TodasPosicionesSeleccionadas: boolean = false;
    displayAdjudicacionCreada: boolean;
    errores: any = [];
    displayVisualizarErrores: boolean;
    numeroOrdenDeCompra: any;
    displayPanelHs: boolean = false;
    textoRacionalIncompleto: boolean;
    displayTextoIncompleto: boolean;
    textoRacionalModal: string;
    regionSap: SelectItem[];
    selectedRegion: any;
    displayRegionSap: boolean;
    centroDire: any;
    centroDireLista: any;
    ordenDeCompra: AdjudicacionDto;
    nroOC: string;
    displayValidacionMoneda: boolean = false;
    displayAdmiteCertificacionesParciales: boolean = false;

    @Input()
    public peticionHs: CotizacionHoraDto;
    error: string;
    displayVisualizarPrecio: boolean;
    peticionOferta_Id: number;
    displayGenerarOCMoneda: boolean;
    generarOC: boolean;
    moneda: any;
    resultado: any;
    centroLista: any[];
    usuario: any;
    lista: any[];
    numerosDePedido: any;
    displayPlazo: boolean;
    mensaje: string;
    displayHistorial: boolean;
    historiales: CotizacionHistorialDto[] = [];
    esTipoPOMultiple: boolean;
    displayVisualizarMovimientos: boolean;
    mensajeValidacionMoneda: any;
    esAuditor: boolean = this.tienePermiso(Permiso.VerComoAuditor);
    esComprasAdmin: boolean = this.tienePermiso(Permiso.AdjudicarDentroDelPlazoDeOfertas);


    constructor(protected service: ComprasService, protected usuarioService: UsuarioService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    ngOnInit() {
        if (this.route.params) {
            this.route.params.forEach((params: Params) => {
                this.peticionOferta_Id = parseInt(params["id"]);
                this.verOfertas(this.peticionOferta_Id);
            })
            this.textoRacionalIncompleto = true;
        };

        if (this.tablaOfertas == null) {
            this.tablaOfertas = {
                Id: null,
                FechaEntregaFormateado: null,
                PlazoDeOferta: null,
                CUIT: null,
                Mail: null,
                Usuarios: new Array(),
                SolpDto: null,
                Selected: null,
                RespetaMateriales: null,
                RespetaServicios: null,
                Solp_Id: null,
                NroSolp: null,
                FechaCreacion: null,
                UsuarioCreador_Id: null,
                Observaciones: null,
                TipoPosicionCodigo: null,
                CotizacionId: null,
                PeticionDeOfertaPosicion: new Array(),
                Cotizacion: null,
                ObservacionTecnica: null,
                ObservacionEconomica: null,
                Cantidad: null,
            } as unknown as PeticionDeOfertaDto;
        }
        if (this.adjudicacion == null || this.adjudicacion == undefined) {
            this.adjudicacion = {
                Id: undefined,
                AdmiteCertificacionesParciales: true
            };
        }
    }

    ngAfterViewInit(): void {
        this.getCombos();
    }

    esTipoSolpMateriales(): boolean {
        return this.tablaOfertas.TipoPosicionCodigo == 'MATERIALES';
    }

    seleccionarTodo() {
        if (this.TodasPosicionesSeleccionadas) {
            this.tablaOfertas.PeticionDeOfertaPosicion.map(pos => {
                if (!pos.Posicion.AdjudicacionCompleta && !pos.EstaEliminado && !pos.Selected) {
                    pos.Selected = true;
                }
            });
        } else {
            this.tablaOfertas.PeticionDeOfertaPosicion.map(pos => pos.Selected = false);
        }
    }

    checkSelectAllIfNeeded(): void {
        this.TodasPosicionesSeleccionadas = this.tablaOfertas.PeticionDeOfertaPosicion
            .filter(pos => !pos.Posicion.AdjudicacionCompleta && !pos.EstaEliminado)
            .every(x => x.Selected);
    }

    verOfertas(peticionOferta_Id) {
        try {
            this.blockUI.start('Cargando...');
            this.subscription = this.service.getListarOfertasComprador(peticionOferta_Id).subscribe(
                (result) => {
                    let peticionDto = this.manejarErroresApiResponse(result);
                    if (peticionDto) {
                        this.tablaOfertas = peticionDto;
                        if (peticionDto.NrosSolp && peticionDto.NrosSolp.length > 1) {
                            this.esTipoPOMultiple = true;
                        }
                        this.nroOC = this.tablaOfertas.NroOrdenDeCompraAdicional || "";
                        if (this.tablaOfertas.Adicional == true) {
                            this.obtenerAdjudicacion(this.nroOC);
                        } else {
                            this.setTextoCondicionEspecial();
                        }
                        this.setMensajeTabla();
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                },
                () => { this.blockUI.stop(); });
        } catch (e) {
            this.blockUI.stop();
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    obtenerAdjudicacion(nroOC) {
        this.blockUI.start('Cargando...');
        this.service.obtenerAdjudicacion(nroOC).subscribe(
            (result) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                }
                else {
                    this.ordenDeCompra = result.data;

                    this.adjudicacion.CondicionesDeEntrega = this.ordenDeCompra.CondicionesDeEntrega;
                    this.adjudicacion.CondicionesDePago = this.ordenDeCompra.CondicionesDePago;
                    this.adjudicacion.Garantias = this.ordenDeCompra.Garantias;
                    this.adjudicacion.TextoDeCabecera = this.ordenDeCompra.TextoDeCabecera;
                    this.setTextoCondicionEspecial();
                    this.blockUI.stop();
                }
            },
            (error) => {
                this.blockUI.stop();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        )
    }

    descargarAdjuntosCotizacion(cotizacionId) {
        this.blockUI.start("Descargando...");
        this.service.DescargarAdjuntosCotizacion(cotizacionId, false).subscribe(
            (result) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                }
                else {
                    let byteArray = new Uint8Array(result.FileContents);
                    let blob = new Blob([byteArray], {
                        type: "application/octet-stream",
                    });

                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(
                            blob,
                            result.FileDownloadName
                        );
                    } else {
                        let url = window.URL.createObjectURL(blob);
                        let link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = result.FileDownloadName;
                        link.click();
                        setTimeout(function () {
                            window.URL.revokeObjectURL(url);
                        }, 0);
                        this.blockUI.stop();
                        return false;
                    }
                    this.blockUI.stop();
                }
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
                this.blockUI.stop();
            }
        )
    }

    validacionCantidad(cantidad: number, cantidadPendiente: number) {
        if (cantidad > cantidadPendiente) {
            this.floatMsgService.setErrorMsg("La cantidad ingresada debe ser menor a " + cantidadPendiente);
        }
    }

    esUltimaPosicion(posicion: PeticionDeOfertaSolpPosicionDto) {
        let ultimoElemento = this.tablaOfertas.PeticionDeOfertaPosicion[this.tablaOfertas.PeticionDeOfertaPosicion.length - 1];
        return ultimoElemento.Id == posicion.Id;
    }

    verPosicion(peticionPosicion: PeticionDeOfertaSolpPosicionDto) {
        peticionPosicion.expanded = peticionPosicion.expanded == true ? false : true;
    }

    mostrarPanelHs() {
        this.displayPanelHs = true;
    }

    onCerrarPanel() {
        this.displayPanelHs = false;
    }

    guardarCertificacionesParciales() {
        let adjudicaciones: AdjudicacionDto[] = [];
        if (this.tablaOfertas.Usuarios) {
            this.tablaOfertas.Usuarios.forEach((poUsuario, i, arr) => {
                if (poUsuario.Cotizacion && poUsuario.Cotizacion.Adjudicaciones) {
                    adjudicaciones.push(...poUsuario.Cotizacion.Adjudicaciones);
                }
            });
        }

        try {
            this.blockUI.start();
            this.subscription = this.service.GuardarCertificacionesParciales(adjudicaciones).subscribe(
                (result) => {
                    this.manejarErroresApiResponse(result);
                    this.blockUI.stop();
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            );
        }
        catch (err) {
            this.floatMsgService.setErrorMsg(err);
            this.blockUI.stop();
        }
    }

    crearAdjudicacion(usuario: PeticionDeOfertaUsarioDto) {
        this.lista = []
        this.usuario = usuario;
        this.adjudicacion.Proveedor = usuario.CodigoProveedor;
        if (this.tablaOfertas.PeticionDeOfertaPosicion.filter(x => x.Selected).length > 0) {
            this.tablaOfertas.PeticionDeOfertaPosicion.forEach((peticion) => {
                if (peticion.Selected && !peticion.Posicion.AdjudicacionCompleta) {
                    usuario.Cotizacion.CotizacionPosiciones.forEach((cotizacionPos) => {
                        if (peticion.Id == cotizacionPos.PeticionDeOfertaSolpPosicion_Id)
                            this.lista.push({
                                PeticionDeOferta_Id: this.tablaOfertas.Id,
                                Posicion: peticion.Posicion.Indice,
                                CotizacionPosicion_Id: cotizacionPos.Id,
                                PlazoDeEntrega: peticion.Posicion.FechaEntregaServicio,
                                Cantidad: this.esTipoSolpMateriales() ? peticion.Posicion.CantidadAdjudicacion : 1,
                                SolpPosicion_Id: peticion.Posicion.Id,
                                CantidadCotizada: cotizacionPos.Cantidad,
                                CantidadSolp: peticion.Posicion.CantidadPendiente,
                                CantidadAdjudicada: this.esTipoSolpMateriales() ? peticion.Posicion.CantidadAdjudicada : 1,
                                NoDisponible: cotizacionPos.NoDisponible,
                                MonedaCotizacion: cotizacionPos.Moneda_Id,
                                MonedaPO: peticion.Posicion.MonedaId,
                                MonedaId: cotizacionPos.Moneda_Id,
                                CentroPosicion: peticion.Posicion.Centro,
                                EsMaterialCatalogado: this.esTipoSolpMateriales() && !!peticion.Posicion.CodigoMaterialSap.Codigo,
                                CodigoMaterialSap: peticion.Posicion.CodigoMaterialSap.Codigo,
                                CodigoCentroSap: peticion.Posicion.Centro.CodigoSap,
                                CodigoGrupoComprasSap: peticion.Posicion.GrupoCompras.CodigoSap,
                                Descripcion: peticion.Posicion.CodigoMaterialSap.Descripcion == null ? peticion.Posicion.Tarea : peticion.Posicion.CodigoMaterialSap.Descripcion
                            })
                    })
                } else {
                    peticion.Selected = false;
                }
            });
            this.parsearFecha();
            if (this.lista.length > 0) {
                this.error = this.validarAdjudicacion(this.lista);
                if (this.error != "") {
                    this.floatMsgService.setErrorMsg(this.error)
                    return;
                }
            } else {
                this.floatMsgService.setInfoMsg("Debe seleccionar alguna posición válida para adjudicar");
                return;
            }
            this.validarFechaVigenciaRegistroInfo(usuario);
        } else {
            this.floatMsgService.setInfoMsg("Debe seleccionar alguna posición para adjudicar");
        }
    }

    public parsearFecha() {
        if (this.lista != undefined) {
            for (const element of this.lista) {
                if (element.PlazoDeEntrega != null) {
                    let date = new Date(element.PlazoDeEntrega);
                    element.PlazoDeEntrega = date
                }
            }
        }
    }

    validarAdjudicacion(lista): string {
        let self = this;
        let breakFor = false;
        this.error = "";
        lista.forEach(element => {
            if (!breakFor) {
                if (this.esTipoSolpMateriales()) {
                    if (element.Cantidad == null || element.Cantidad == undefined || element.Cantidad <= 0) {
                        self.error = "Pos " + element.Posicion + " - La cantidad adjudicada debe ser mayor a 0";
                        breakFor = true;
                        return self.error;
                    }
                }

                if (this.esTipoSolpMateriales()) {
                    if (element.Cantidad > element.CantidadSolp) {
                        self.error = "Pos " + element.Posicion + " - La cantidad adjudicada no debe ser mayor que la cantidad pendiente";
                        breakFor = true;
                        return self.error;
                    }
                }

                if (element.NoDisponible == true) {
                    self.error = "Pos " + element.Posicion + " - No se puede adjudicar una posición no disponible";
                    breakFor = true;
                    return self.error;
                }

                if (this.esTipoSolpMateriales() && this.tablaOfertas.Adicional) {
                    if (element.MonedaCotizacion != element.MonedaPO) {
                        self.error = "Pos " + element.Posicion + " - La moneda de la cotización y de la OC debe ser la misma";
                        breakFor = true;
                        return self.error;
                    }
                }
            }
        });
        return this.error;
    }

    mostrarValidacionMoneda() {
        const posicionesSeleccionadas = this.tablaOfertas.PeticionDeOfertaPosicion
            .filter(x => x.Selected && !x.Posicion.AdjudicacionCompleta);

        const posiciones = this.usuario.Cotizacion.CotizacionPosiciones
            .filter(cotizacionPos =>
                posicionesSeleccionadas.some(peticion => peticion.Id === cotizacionPos.PeticionDeOfertaSolpPosicion_Id)
            );

        const primeraMoneda = posiciones[0].Moneda_Id; // Tomamos la moneda de la primera posición
        const todasLasSubposiciones = [];
        for (const element of posiciones) {
            if (element.Moneda_Id !== primeraMoneda) {
                return true; // Si encontramos una moneda diferente, devolvemos true
            }
            todasLasSubposiciones.push(...element.CotizacionSubPosiciones);
        }

        for (let i = 1; i < todasLasSubposiciones.length; i++) {
            if (todasLasSubposiciones[i].Moneda_Id !== todasLasSubposiciones[i - 1].Moneda_Id) {
                return true;
            }
        }

        return false;
    }

    ocultarGenerarOCSerivicioDiferentesMonedas() {
        if (!this.esTipoSolpMateriales()) {

        }
    }

    mostrarModalGenerarOCMoneda(proveedor) {
        if (this.mostrarValidacionMoneda()) {
            this.displayGenerarOCMoneda = true;
            this.devolverMonedaProveedor(proveedor.CodigoProveedor);
        } else {
            this.validarMonedasDiferentes();
        }
    }

    validarMonedasDiferentes() {
        if (this.adjudicacion != undefined) {
            this.adjudicacion.PeticionDeOferta_Id = this.tablaOfertas.Id;
            this.adjudicacion.EsMonedaProveedor = this.generarOC;
            this.adjudicacion.TextoDeCabecera = this.sanitizeInput(this.adjudicacion.TextoDeCabecera);
            this.adjudicacion.Garantias = this.sanitizeInput(this.adjudicacion.Garantias);
            this.adjudicacion.CondicionesDeEntrega = this.sanitizeInput(this.adjudicacion.CondicionesDeEntrega);
            this.adjudicacion.CondicionesDePago = this.sanitizeInput(this.adjudicacion.CondicionesDePago);

            this.service.ValidarPrecioCotizado(this.adjudicacion).subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        if (result.data.Errores.length > 0) {
                            this.mensajeValidacionMoneda = result.data.Errores[0];
                            this.displayValidacionMoneda = true;
                        } else {
                            this.verificarCertificacionesParciales();
                        }
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
        }
    }

    sanitizeInput(input?: string): string {
        if (input == null) { return ''; }
        // Regex que incluye más caracteres potencialmente peligrosos
        const dangerousCharacters = /[<>"'&\\\{\}\$\`\%]/g;
        return input.replace(dangerousCharacters, '');
    }
    onCerrarValidacionMoneda() {
        this.displayValidacionMoneda = false;
    }

    onSiguientePasoValidacionMoneda() {
        this.displayValidacionMoneda = false;
        this.verificarCertificacionesParciales();
    }

    onGenerarOC() {
        this.generarOC = false;
        this.validarMonedasDiferentes();
    }

    onGenerarOCProveedor() {
        this.generarOC = true;
        if (this.moneda == undefined) {
            this.floatMsgService.setErrorMsg("El proveedor no tiene una moneda configurada");
            this.onCerrarMoneda();
        } else {
            this.validarMonedasDiferentes();
        }

    }

    onCerrarMoneda() {
        this.displayGenerarOCMoneda = false;
    }

    devolverMonedaProveedor(codigo) {
        this.blockUI.start('Cargando...');
        this.service.devolverMonedaProveedor(codigo)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        this.moneda = result.Moneda;
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
        this.blockUI.stop();
    }

    confirmacionAdjudicar() {
        this.confirmationService.confirm({
            header: "¡Último Paso!",
            acceptLabel: "SI, CONFIRMAR",
            rejectLabel: "VOLVER",
            message: 'Está a punto de enviar la adjudicación. <br/><b>¿Desea continuar?</b><br/><br/>',
            accept: () => {
                this.guardarAdjudicacion()
            },
            reject: () => {
            }
        });
    }

    guardarAdjudicacion() {
        this.blockUI.start("Grabando...");
        this.adjudicacion.EsMonedaProveedor = this.generarOC;
        try {
            this.guardarAdjudicacionTextos();
            this.subscription = this.service.GrabarAdjudicacion(this.adjudicacion).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    }
                    else if (result.Errores != undefined && result.Errores != null && result.Errores.length > 0) {
                        this.errores = result.Errores;
                        this.displayVisualizarErrores = true;
                        this.onCerrarMoneda();
                        this.noContinuarAdjudicacion();
                    }
                    else {
                        this.numerosDePedido = result.NumerosDePedido;
                        this.displayAdjudicacionCreada = true;
                        this.adjudicacion.CondicionesDeEntrega = "";
                        this.adjudicacion.CondicionesDePago = "";
                        this.adjudicacion.Garantias = "";
                        this.adjudicacion.TextoDeCabecera = "";
                        this.onCerrarMoneda();
                        this.noContinuarAdjudicacion();
                    }
                    this.blockUI.stop();
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.blockUI.stop();

                });
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    validacionTextosIncompletos() {
        this.textoRacionalIncompleto = false;
        if ((this.adjudicacion.CondicionesDeEntrega == "" || this.adjudicacion.CondicionesDeEntrega == undefined)
            && (this.adjudicacion.CondicionesDePago == "" || this.adjudicacion.CondicionesDePago == undefined)
            && (this.adjudicacion.Garantias == "" || this.adjudicacion.Garantias == undefined)
            && (this.adjudicacion.TextoDeCabecera == "" || this.adjudicacion.TextoDeCabecera == undefined || this.adjudicacion.TextoDeCabecera == `Justificación de condición especial: ${this.tablaOfertas.SolpDto.ObservacionesCotizacionCondEsp}`)) {
            this.textoRacionalIncompleto = true;
            this.textoRacionalModal = "No se completó ningún racional.";
        }
    }

    salir() {
        this.displayAdjudicacionCreada = false;
        this.verOfertas(this.tablaOfertas.Id.toString())
    }

    salirVisualizarErrores() {
        this.displayVisualizarErrores = false;
    }

    abrirModalTextos() {
        this.displayTextos = true;
    }

    cerrarModalTextos() {
        this.displayTextos = false;
    }

    guardarAdjudicacionTextos() {
        const txtCondPago = (this.tablaOfertas.SolpDto.CertificacionAutomatica ?
            "* Certificaciones automáticas\n* Forma de facturar: ver NOTA V condiciones predeterminadas\n " : "") + this.modalTexto.adjudicacion.CondicionesDePago;

        this.adjudicacion.CondicionesDeEntrega = this.modalTexto.adjudicacion.CondicionesDeEntrega;
        this.adjudicacion.CondicionesDePago = txtCondPago;
        this.adjudicacion.Garantias = this.modalTexto.adjudicacion.Garantias;
        this.adjudicacion.TextoDeCabecera = this.modalTexto.adjudicacion.TextoDeCabecera;
    }

    onEditarCelda(cotizacion: any, campo: string, valorInicial: any) {
        if (cotizacion[campo] === valorInicial) {
            cotizacion[campo] = ''; // Limpia el valor si es igual al valorInicial
        }
    }

    onReestablecerValor(cotizacion: any, campo: string) {
        if (cotizacion[campo] === '' || cotizacion[campo] === null) {
            cotizacion[campo] = 0; // Restablece a cero si está en blanco
        }
    }

    validarNumero(event: any) {
        const inputValue = event.target.value;
        if (isNaN(inputValue) || inputValue < 0) {
            event.target.value = 0; // Borra el valor si es negativo
        }
    }

    cerrarModalPrecios() {
        this.displayVisualizarPrecio = false;
        // this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
        //     this.router.navigate(['compras/ver-ofertas/' + this.peticionOferta_Id]);
        // });
    }

    onVisualizarPrecioGuardado() {
        this.displayVisualizarPrecio = false;
        this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
            this.router.navigate(['compras/ver-ofertas/' + this.peticionOferta_Id]);
        });
    }

    abrirModalPrecios() {
        this.displayVisualizarPrecio = true;
    }

    cerrarModalMovimientos() {
        this.displayVisualizarMovimientos = false;
    }

    abrirModalMovimientos() {
        this.displayVisualizarMovimientos = true;
    }

    obtenerPrimerPlazo(cotizacionPosicion: any): string {
        let plazos = '';

        if (cotizacionPosicion.PrimerPlazoDeOferta > 0) {
            plazos += 'Plazo: ' + cotizacionPosicion.PrimerPlazoDeOferta + ' - Cantidad: ' + cotizacionPosicion.PrimeraCantidad;
        } else {
            plazos += 'sin Plazo';
        }
        return plazos;
    }

    obtenerSegundoPlazo(cotizacionPosicion: any): string {
        let plazos = '';
        if (cotizacionPosicion.SegundoPlazoDeOferta > 0) {
            plazos += 'Plazo: ' + cotizacionPosicion.SegundoPlazoDeOferta + ' - Cantidad: ' + cotizacionPosicion.SegundaCantidad;
        }
        return plazos;
    }

    obtenerTercerPlazo(cotizacionPosicion: any): string {
        let plazos = '';

        if (cotizacionPosicion.TercerPlazoDeOferta > 0) {
            plazos += 'Plazo: ' + cotizacionPosicion.TercerPlazoDeOferta + ' - Cantidad: ' + cotizacionPosicion.TerceraCantidad;
        }
        return plazos;
    }

    aceptarRegion() {
        this.adjudicacion.RegionSap = this.selectedRegion.value;
        this.validacionTextosIncompletos();
        this.displayRegionSap = false;
        if (this.textoRacionalIncompleto == true) {
            this.displayTextoIncompleto = true;
        } else {
            this.abrirModalPosicionPlazo();
        }
    }

    salirRegion() {
        this.displayRegionSap = false;
    }

    continuarAdjudicacion() {
        this.displayTextoIncompleto = false;
        this.abrirModalPosicionPlazo();
    }

    noContinuarAdjudicacion() {
        this.displayTextoIncompleto = false;
    }

    configurarAdmiteCertificacionesParciales(admite: boolean) {
        this.adjudicacion.AdmiteCertificacionesParciales = admite;
        this.displayAdmiteCertificacionesParciales = false;
        this.confirmacionAdjudicar();
    }

    cancelarAdmiteCertificacionesParciales() {
        this.displayAdmiteCertificacionesParciales = false;
    }

    getCombos() {
        try {
            this.subscription = this.service.getCombos().subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.regionSap = [];
                        this.centroDireLista = [];
                        result.CentrosDireccion.forEach(d => this.centroDireLista.push({
                            label: d.CodigoSap, value: d.RegionSap_Id
                        }));
                        result.Regiones.forEach(d => this.regionSap.push({
                            label: d.Descripcion, value: d.Id
                        }));
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    abrirModalPosicionPlazo() {
        this.displayPlazo = true;
    }

    cerrarPlazo() {
        this.displayPlazo = false;
    }

    guardarPlazo(listaParam: any[]) {
        this.lista = listaParam;
        this.displayPlazo = false;
        this.proveedor = this.usuario.CodigoProveedor;
        this.mostrarModalGenerarOCMoneda(this.usuario)
    }

    verificarCondicionEspecial(): boolean {
        return this.tablaOfertas.SolpDto.Urgencia == true ||
            this.tablaOfertas.SolpDto.Adicional == true ||
            this.tablaOfertas.SolpDto.TrabajoYaHecho == true ||
            this.tablaOfertas.SolpDto.CondEspProveedorAsignado == true ||
            this.tablaOfertas.SolpDto.ConPresupuesto == true
    }

    setTextoCondicionEspecial(): void {
        if (this.verificarCondicionEspecial()) {
            if (!this.adjudicacion.TextoDeCabecera) {
                this.adjudicacion.TextoDeCabecera = "";
            }

            if (this.adjudicacion.TextoDeCabecera != undefined &&
                this.adjudicacion.TextoDeCabecera != "" &&
                this.modalTexto.adjudicacion.TextoDeCabecera !=
                this.tablaOfertas.SolpDto.ObservacionesCotizacionCondEsp) {
                this.adjudicacion.TextoDeCabecera += '\n\n';
            }
            this.adjudicacion.TextoDeCabecera += `Justificación de condición especial:\n${this.tablaOfertas.SolpDto.ObservacionesCotizacionCondEsp}`;
        }
    }

    setMensajeTabla() {
        if (this.tablaOfertas.Usuarios) {
            this.tablaOfertas.Usuarios.forEach(usuario => {
                if (usuario.Cotizacion && usuario.Cotizacion.CotizacionPosiciones) {
                    usuario.Cotizacion.CotizacionPosiciones.forEach(cotizacionPosicion => {
                        cotizacionPosicion.MensajeTablaVerOfertas = '';
                        if (!cotizacionPosicion.Completado) {
                            cotizacionPosicion.MensajeTablaVerOfertas = 'Sin cotizar'
                        }
                        if (cotizacionPosicion.Adjudicado) {
                            cotizacionPosicion.MensajeTablaVerOfertas = 'Adjudicado'
                        }
                    });
                }
            });
        }
    }

    public actualizarVisibilidad(proveedor, cambiarEstado) {
        if (!cambiarEstado) {
            this.mensaje = '';
            try {
                this.blockUI.start('Cargando...');
                this.service.actualizarProveedorVisibleEnSolicitante(proveedor.Id, proveedor.VisibleSolicitante).subscribe(
                    () => {
                        this.mensaje = "Los datos se actualizaron correctamente."
                        this.blockUI.stop();
                    },
                    (error) => {
                        this.blockUI.stop();
                        this.mensajeComponent.setErrorMsg(error.message);
                    }
                );
            } catch (e) {
                this.floatMsgService.setErrorMsg(e);
                return false; //<-- Prevent Refresh
            }
        }
        return false; //<-- Prevent Refresh
    }

    obtenerHistorial(id) {
        try {
            this.blockUI.start('Cargando...');
            this.subscription = this.service.obtenerHistorial(id).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.historiales = result.data;
                    }
                    this.blockUI.stop();
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.blockUI.stop();
                });
        } catch (e) {
            this.blockUI.stop();
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    abrirModalHistorial(id) {
        this.displayHistorial = true;
        this.obtenerHistorial(id);
    }

    cerrarHistorial() {
        this.displayHistorial = false;
    }

    getTotalPreciosPorMoneda(cotizacionPosicion: any): string {
        const preciosPorMoneda: { [key: string]: number } = {};
        for (const subpos of cotizacionPosicion.CotizacionSubPosiciones) {
            if (!subpos.MonedaDescripcion) {
                continue;
            }
            if (!preciosPorMoneda[subpos.MonedaDescripcion]) {
                preciosPorMoneda[subpos.MonedaDescripcion] = 0;
            }
            preciosPorMoneda[subpos.MonedaDescripcion] += subpos.PrecioUnidad;
        }
        this.resultado = '';
        for (const moneda in preciosPorMoneda) {
            if (preciosPorMoneda.hasOwnProperty(moneda)) {
                const precioFormateado = preciosPorMoneda[moneda].toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                this.resultado += `${moneda} ${precioFormateado}<br>`;
            }
        }
        return this.resultado;
    }
    posicionesSinVigencia = [];
    validarFechaVigenciaRegistroInfo(usuario) {
        const listaAvalidar = this.lista
            .filter(x => x.EsMaterialCatalogado);
        if (!listaAvalidar.length) {
            this.mostrarModalRegionSap(usuario)
            return
        }
        this.blockUI.start('Validando fechas de vigencia ...');
        forkJoin(listaAvalidar
            .map(({ CodigoGrupoComprasSap, CodigoCentroSap, CodigoMaterialSap, CotizacionPosicion_Id }) =>
                this.service.validarFechaVigenciaRegistroInfo({
                    grupoComprasCodigoSap: CodigoGrupoComprasSap,
                    centroCodigoSap: CodigoCentroSap,
                    materialCodigoSap: CodigoMaterialSap,
                    cotizacionPosicionId: CotizacionPosicion_Id
                })))
            .pipe(
                finalize(() => this.blockUI.stop())
            )
            .subscribe(res => {
                const errores = res.filter(x => x.error);
                if (errores.length > 0) {
                    this.floatMsgService.setErrorMsg(errores[0].error);
                    return;
                }
                this.posicionesSinVigencia = this.lista.filter(x =>
                    x.EsMaterialCatalogado
                    && this.obtenerInfoRespuestaRegistroVencido(x.CotizacionPosicion_Id, res)
                ).map(x => ({
                    ...x,
                    FechaVigencia:
                        this.obtenerInfoRespuestaRegistroVencido(x.CotizacionPosicion_Id, res)
                            .data.FechaVigencia
                }));
                if (!this.posicionesSinVigencia.length) {
                    this.mostrarModalRegionSap(usuario)
                }
            })
    }

    onCerrarActualizarSinVigenciaModal() {
        this.posicionesSinVigencia = [];
    }
    obtenerInfoRespuestaRegistroVencido(cotizacionPosicionId, listaRespuesta) {
        return listaRespuesta.find(y => y.data.CotizacionPosicionId === cotizacionPosicionId && !y.data.EstaVigente)
    }

    mostrarModalRegionSap(usuario) {
        let posRegion = this.lista[0].CentroPosicion.CodigoSap;

        if (posRegion) {
            this.centroDire = this.centroDireLista.find(c => c.label == posRegion);
            this.selectedRegion = { label: this.centroDire.label, value: this.centroDire.value };
        }

        this.adjudicacion.AdjudicacionPosiciones = this.lista;
        this.adjudicacion.Cotizacion_Id = usuario.Cotizacion.Id;
        this.adjudicacion.Solp_Id = this.tablaOfertas.Solp_Id;

        this.displayRegionSap = true;
    }

    verificarCertificacionesParciales() {
        if (this.esTipoSolpMateriales() || this.tablaOfertas.TrabajoHecho === true) {
            this.confirmacionAdjudicar();
        }
        else {
            this.displayAdmiteCertificacionesParciales = true;
        }
    }

    manejarErroresApiResponse<T>(response: ApiResponse<T>): T | null {
        if (response.logout) {
            this.sessionDataService.logout();
            return null;
        }
        if (response.error) {
            this.floatMsgService.setErrorMsg(response.error);
            return null;
        }
        if (response.info) {
            this.floatMsgService.setInfoMsg(response.info);
        }
        return response.data || null;
    }
}

