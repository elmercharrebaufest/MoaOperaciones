import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import _ from 'lodash';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { SelectItem } from 'primeng/api';
import { Table } from 'primeng/table';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { POPosicionDto } from '../../modelos/po-posicionDto';
import { SolpCrearPoMultipleDto } from '../../modelos/Solp-CrearPoMultipleDto.model';
import { ComprasService } from '../compras.service';
import { EnumTipoImputacion } from '../enum-tipo-imputacion';
import { PosicionCrearPoMultipleDto } from '../../modelos/Posicion-CrearPoMultipleDto.model';
import { ActionResult } from '../../../serviceHelpers/actionResult.Interface';
import { SubPosicionCrearPoMultipleDto } from '../../modelos/SubPosicion-CrearPoMultipleDto.model';

@Component({
    selector: 'app-crear-po-multiple',
    templateUrl: './crear-po-multiple.component.html',
    styleUrls: ['../compras.component.css',
        './crear-po-multiple.component.css'],
})

export class CrearPoMultipleComponent extends ListBaseComponent implements OnInit {

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

        this.usuario = sessionStorage.getItem("username");

        this.locale = {
            firstDayOfWeek: 0,
            dayNames: ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"],
            dayNamesShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
            dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
            monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
            monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
            today: 'Hoy',
            clear: 'Borrar'
        };
    }

    @BlockUI() blockUI: NgBlockUI;

    @ViewChild("tabla")
    protected tabla: Table;

    usuario: string;

    TodasPosicionesSeleccionadas: boolean = false;
    protected locale: any;
    filteredfechas: any;
    fechaInicio: string = null;
    fechaFin: string = null;
    rangeDates: Date[];
    tipoFiltroFecha = 1;
    desdeDashboard: Date;
    hastaDashboard: Date;
    grupoComprasFiltro: SelectItem[];
    selectGrupoCompras: string[] = [];
    centroFiltro: SelectItem[];
    selectCentro: string[] = [];
    claseDocumentoFiltro: SelectItem[];
    selectClaseDocumento: string[] = [];
    tipoImputacionFiltro: SelectItem[];
    selectTipoImputacion: string[] = [];
    valorTipoImputacionFiltro: SelectItem[] = [];
    selectValorTipoImputacion: string[] = [];
    hoy: Date = new Date();
    es: any;
    checkedFilterSap = false;
    checkedFilterMantenimiento = false;
    checkedFilterWeb = false;
    clasesDocumento: number[] = [];
    @ViewChild('myCalendar', undefined)
    sap: boolean = false;
    mantenimiento: boolean = false;
    web: boolean = false;
    repoAutomatica: boolean = false;
    contratoMarco: boolean = false;
    posiciones: POPosicionDto[] = []; // para búsqueda "MATERIAL"
    solps: SolpCrearPoMultipleDto[] = []; // para búsqueda "SERVICIO"
    numeroPo?: number = null;

    tratada: SelectItem[] = [{ label: "Tiene PO", value: true }, { label: "No tiene PO", value: false }, { label: "Ver Todas", value: null }];
    selectTratada: boolean | null = null;

    tiposSolp: SelectItem[] = [];
    selectTipoSolp?: string;
    showNombrePliegoConditionList: string[] = [];
    get showNombrePliego() {
        return this.showNombrePliegoConditionList.includes(this.selectTipoSolp);
    }

    showTipoPliegoMultipleConditionList: string[] = [];
    get showTipoPliegoMultiple() {
        return this.showTipoPliegoMultipleConditionList.includes(this.selectTipoSolp);
    }


    tipoPliegoItem: SelectItem[];
    selectTipoPliego: string[] = [];

    nombrePliego?: string;

    lastSearch?: string;

    filtrosPOMultiple: iFiltrosPoMultiple;

    private readonly filtrosPOMultipleDefault: iFiltrosPoMultiple = {
        sap: false,
        mantenimiento: false,
        web: false,
        repoAutomatica: false,
        contratoMarco: false,
        gruposCompras: [],
        centros: [],
        claseDocumento: [],
        tipoImputacion: [],
        valorTipoImputacion: [],
        subtipoImputacionCombo: [],
        fechaDesde: null,
        fechaHasta: null,
        tratada: null,
        numeroPo: null,
        selectTipoSolp: null,
        nombrePliego: null,
        selectTipoPliego: [],
    };

    ngOnInit() {

        this.filtrosPOMultiple = _.cloneDeep(this.filtrosPOMultipleDefault);
        this.recuperarFiltros();
        this.listarPosicionesPOMultiple();
    }

    ngAfterViewInit(): void {
        this.getCombos();
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
                        this.centroFiltro = [];
                        this.grupoComprasFiltro = [];
                        this.claseDocumentoFiltro = [];
                        this.tipoImputacionFiltro = [];
                        this.tiposSolp = [];
                        this.tipoPliegoItem = [];

                        result.Centro.forEach(c => this.centroFiltro.push({
                            label: c.Codigo + " - " + c.Descripcion, value: c.Id
                        }));
                        result.GrupoCompras.forEach(gc => this.grupoComprasFiltro.push({
                            label: gc.Codigo + " - " + gc.Descripcion, value: gc.Id
                        }));
                        result.ClaseDocumento.forEach(cd => this.claseDocumentoFiltro.push({
                            label: cd.Codigo + " - " + cd.Descripcion, value: cd.Id
                        }));
                        result.TipoImputacion.forEach(ti => this.tipoImputacionFiltro.push({
                            label: ti.Descripcion + " - " + ti.Codigo, value: ti.Codigo
                        }));
                        result.TipoPosicionSolp.forEach(tp => {
                            this.tiposSolp.push({
                                label: tp.Codigo, value: tp.Codigo
                            })
                        });
                        this.filtrosPOMultipleDefault.selectTipoSolp = result.DefaultTipoPosicionSolpCrearPoMultiple;
                        if (!this.selectTipoSolp) { this.selectTipoSolp = result.DefaultTipoPosicionSolpCrearPoMultiple; }
                        this.showNombrePliegoConditionList = result.showNombrePliegoConditionList;
                        this.showTipoPliegoMultipleConditionList = result.showTipoPliegoMultipleConditionList;

                        result.TipoPliego.forEach((e: { Descripcion: string; Id: string; }) => {
                            this.tipoPliegoItem.push({
                                label: e.Descripcion, value: e.Id
                            });
                            if (!this.filtrosPOMultiple.selectTipoPliego || !this.filtrosPOMultiple.selectTipoPliego.length) {
                                this.selectTipoPliego.push(e.Id);
                            }
                        });
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

        return false;
    }

    subtipoImputacionCombo() {
        var tablas: string[] = [];
        this.selectTipoImputacion.forEach(tipo => {
            switch (tipo) {
                case EnumTipoImputacion.CentroDeCosto:
                    tablas.push('CecoSolpSap');
                    break;
                case EnumTipoImputacion.OrdenDeOt:
                case EnumTipoImputacion.OrdenInversion:
                    tablas.push('OrdenSolpSap');
                    break;
                case EnumTipoImputacion.Siniestro:
                    tablas.push('CentroBeneficio');
                    break;
            }
        });

        try {
            this.subscription = this.service.listarTablaSap(tablas).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.valorTipoImputacionFiltro = [];
                        this.valorTipoImputacionFiltro = result.data.map(vti => ({
                            label: `${vti.CodigoDescripcion}`, value: vti.Id
                        }));
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                });
        }
        catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false;
    }

    seleccionarTodo() {
        if (this.lastSearch == 'SERVICIO') {
            if (this.TodasPosicionesSeleccionadas) {
                this.solps.map(solp => solp.Selected = true);
            } else {
                this.solps.map(solp => solp.Selected = false);
            }
        } else {
            if (this.TodasPosicionesSeleccionadas) {
                this.posiciones.map(pos => pos.Selected = true);
            } else {
                this.posiciones.map(pos => pos.Selected = false);
            }
        }
    }

    listarPosicionesPOMultiple() {
        try {
            this.lastSearch = null;
            this.blockUI.start('Cargando...');
            this.subscription = this.service.listarPosicionesPOMultiple(
                this.fechaInicio,
                this.fechaFin,
                this.sap,
                this.mantenimiento,
                this.web,
                this.repoAutomatica,
                this.contratoMarco,
                this.selectCentro.join(","),
                this.selectGrupoCompras.join(","),
                this.selectClaseDocumento.join(","),
                this.selectTipoImputacion.join(","),
                this.selectValorTipoImputacion.join(","),
                this.selectTratada,
                this.numeroPo,
                this.selectTipoSolp,
                this.nombrePliego,
                this.selectTipoPliego.join(",")
            ).subscribe(
                (result: ActionResult<POPosicionDto[]> | ActionResult<SolpCrearPoMultipleDto[]>) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        if (this.selectTipoSolp === "SERVICIO") {
                            this.posiciones = [];
                            this.solps = result.data as SolpCrearPoMultipleDto[];
                            console.log("Estas son las Solps:", this.solps);
                        } else {
                            this.posiciones = result.data as POPosicionDto[];
                            this.solps = [];
                        }
                        this.lastSearch = this.selectTipoSolp;
                    }
                    this.blockUI.stop();
                },
                error => {
                    this.blockUI.stop();
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    onBuscar() {
        this.listarPosicionesPOMultiple();
        this.guardarFiltros();
    }

    listarPosicionesPOMultiplePorId(solp: SolpCrearPoMultipleDto) {
        try {
            this.blockUI.start('Cargando...');
            this.subscription = this.service.listarPosicionesPOMultipleIdSolp(solp.Id
            ).subscribe(
                (result: ActionResult<PosicionCrearPoMultipleDto[]>) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        solp.Posiciones = result.data;
                        solp.Expanded = true;
                    }
                    this.blockUI.stop();
                },
                error => {
                    this.blockUI.stop();
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    listarSubPosicionesPOMultiplePorId(posicion: PosicionCrearPoMultipleDto) {
        try {
            this.blockUI.start('Cargando...');
            this.subscription = this.service.listarSubPosicionesPOMultipleIdSolp(posicion.Id
            ).subscribe(
                (result: ActionResult<SubPosicionCrearPoMultipleDto[]>) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        posicion.SubPosiciones = result.data;
                        posicion.Expanded = true;
                    }
                    this.blockUI.stop();
                },
                error => {
                    this.blockUI.stop();
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    solpExpandToggle(solp: SolpCrearPoMultipleDto): void {
        if (solp.Expanded) {
            solp.Expanded = false;
            return;
        }

        if (solp.Posiciones && solp.Posiciones.length) {
            solp.Expanded = true;
        } else {
            // no olvidar de marcar solp.Expanded en el resultado de la subscription.
            this.listarPosicionesPOMultiplePorId(solp)
        }
    }

    posicionExpandToggle(posicion: PosicionCrearPoMultipleDto): void {
        if (posicion.Expanded) {
            posicion.Expanded = false;
            return;
        }

        if (posicion.SubPosiciones && posicion.SubPosiciones.length) {
            posicion.Expanded = true;
        } else {
            // no olvidar de marcar solp.Expanded en el resultado de la subscription.
            this.listarSubPosicionesPOMultiplePorId(posicion)
        }
    }

    download() {
        try {
            this.blockUI.start('Cargando...');

            this.subscription = this.service.descargarPosicionesPOMultiple(
                this.fechaInicio,
                this.fechaFin,
                this.sap,
                this.mantenimiento,
                this.web,
                this.repoAutomatica,
                this.contratoMarco,
                this.selectCentro.join(","),
                this.selectGrupoCompras.join(","),
                this.selectClaseDocumento.join(","),
                this.selectTipoImputacion.join(","),
                this.selectValorTipoImputacion.join(","),
                this.selectTratada,
                this.numeroPo,
                this.selectTipoSolp
            ).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        var byteArray = new Uint8Array(result.file);
                        var blob = new Blob([byteArray], {
                            type: result.contentType,
                        });

                        this.downloadArchivoLocal(blob, result.fileName);
                    }

                    this.blockUI.stop();
                },
                error => {
                    this.blockUI.stop();
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    private downloadArchivoLocal(blob: Blob, nombreArchivo: string): void { // esto habría que moverlo a otro lado, hay demasiadas copias del mismo código.
        if ((window.navigator as any).msSaveOrOpenBlob) {
            // IE11
            (window.navigator as any).msSaveOrOpenBlob(blob, nombreArchivo);
        }
        else {
            var url = window.URL.createObjectURL(blob);
            var link = document.createElement("a");
            document.body.appendChild(link);
            link.href = url;
            link.download = nombreArchivo;
            link.click();
            setTimeout(function () {
                window.URL.revokeObjectURL(url);
            }, 0);
            return;
        }
    }

    clearFilters(): void {
        this.filtrosPOMultiple = _.cloneDeep(this.filtrosPOMultipleDefault);
        this.aplicarFiltrosDesdeGuardados();
    }

    private guardarFiltros(): void {
        this.filtrosPOMultiple.sap = this.sap;
        this.filtrosPOMultiple.mantenimiento = this.mantenimiento;
        this.filtrosPOMultiple.web = this.web;
        this.filtrosPOMultiple.repoAutomatica = this.repoAutomatica;
        this.filtrosPOMultiple.contratoMarco = this.contratoMarco;
        this.filtrosPOMultiple.gruposCompras = this.selectGrupoCompras;
        this.filtrosPOMultiple.centros = this.selectCentro;
        this.filtrosPOMultiple.claseDocumento = this.selectClaseDocumento;
        this.filtrosPOMultiple.tipoImputacion = this.selectTipoImputacion;
        this.filtrosPOMultiple.valorTipoImputacion = this.selectValorTipoImputacion;
        this.filtrosPOMultiple.subtipoImputacionCombo = this.valorTipoImputacionFiltro;
        this.filtrosPOMultiple.fechaDesde = this.fechaInicio;
        this.filtrosPOMultiple.fechaHasta = this.fechaFin;
        this.filtrosPOMultiple.tratada = this.selectTratada;
        this.filtrosPOMultiple.numeroPo = this.numeroPo;
        this.filtrosPOMultiple.selectTipoSolp = this.selectTipoSolp;
        this.filtrosPOMultiple.nombrePliego = this.nombrePliego;
        this.filtrosPOMultiple.selectTipoPliego = _.cloneDeep(this.selectTipoPliego);
        sessionStorage.setItem('filtrosPOMultiple', JSON.stringify(this.filtrosPOMultiple));
    }

    private recuperarFiltros() {
        let filtrosPOMultipleParsed: iFiltrosPoMultiple = JSON.parse(sessionStorage.getItem('filtrosPOMultiple'));
        if (filtrosPOMultipleParsed) {
            this.filtrosPOMultiple = _.cloneDeep(filtrosPOMultipleParsed);
        }
        else {
            this.filtrosPOMultiple = _.cloneDeep(this.filtrosPOMultipleDefault);
        }
        this.aplicarFiltrosDesdeGuardados();
    }

    private aplicarFiltrosDesdeGuardados(): void {
        if (this.filtrosPOMultiple) {
            this.sap = this.filtrosPOMultiple.sap;
            this.mantenimiento = this.filtrosPOMultiple.mantenimiento;
            this.web = this.filtrosPOMultiple.web;
            this.repoAutomatica = this.filtrosPOMultiple.repoAutomatica;
            this.contratoMarco = this.filtrosPOMultiple.contratoMarco;
            this.selectGrupoCompras = this.filtrosPOMultiple.gruposCompras;
            this.selectCentro = this.filtrosPOMultiple.centros;
            this.selectClaseDocumento = this.filtrosPOMultiple.claseDocumento;
            this.selectTipoImputacion = this.filtrosPOMultiple.tipoImputacion;
            this.selectValorTipoImputacion = this.filtrosPOMultiple.valorTipoImputacion;
            this.valorTipoImputacionFiltro = this.filtrosPOMultiple.subtipoImputacionCombo;
            this.fechaInicio = this.filtrosPOMultiple.fechaDesde;
            this.fechaFin = this.filtrosPOMultiple.fechaHasta;
            this.selectTratada = this.filtrosPOMultiple.tratada;
            this.numeroPo = this.filtrosPOMultiple.numeroPo;
            if (this.fechaInicio != undefined && this.fechaInicio.length > 0) {
                const [year, month, day] = this.fechaInicio.split('-').map(Number); //se maneja el cambio de día incorrecto por la zona horaria local
                if (this.fechaFin != undefined && this.fechaFin.length > 0) {
                    const [year2, month2, day2] = this.fechaFin.split('-').map(Number);
                    this.rangeDates = [new Date(year, month - 1, day), new Date(year2, month2 - 1, day2)];
                } else {
                    this.rangeDates = [new Date(year, month - 1, day)];
                }
            } else {
                this.rangeDates = undefined;
            }
            this.selectTipoSolp = this.filtrosPOMultiple.selectTipoSolp;
            this.nombrePliego = this.filtrosPOMultiple.nombrePliego;
            this.selectTipoPliego = _.cloneDeep(this.filtrosPOMultiple.selectTipoPliego);
        }
    }

    listarClaseDocumento() {
        try {
            this.subscription = this.service.listarClaseDocumento(sessionStorage.getItem("usuarioId")).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else {
                        this.clasesDocumento = result;
                        if (this.clasesDocumento.length > 0)
                            sessionStorage.setItem('clasesDocumentoUsuario', JSON.stringify(this.clasesDocumento));
                    }
                },
                error => { this.floatMsgService.setErrorMsg(error.message); }
            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false;
        }
        return false;
    }

    mismaClaseDocumento(claseDocumento_Id: number): boolean {
        return this.clasesDocumento.some(x => x === claseDocumento_Id);
    }

    onSelect(event: any) {
        if (this.rangeDates[0] && this.rangeDates[1] == null) {
            let d = new Date(Date.parse(event));
            this.fechaInicio = `${d.getFullYear()}-${d.getMonth() + 1}-${d.getDate()}`;
            this.fechaFin = '';
        } else {
            let d = new Date(Date.parse(event));
            this.fechaFin = `${d.getFullYear()}-${d.getMonth() + 1}-${d.getDate()}`;
        }
    }

    returnToTodaysDate() {
        this.fechaInicio = "";
        this.fechaFin = "";
        if (this.posiciones.length > 0) {
            this.mensajeComponent.setMsgsEmpty();
        }
    }

    public async publicarCotizacion(): Promise<void> {

        let ids: number[] = [];
        let error: boolean = false;
        let errorMessaje: string = "";

        this.blockUI.start('Cargando...');

        try {

            if (this.lastSearch == 'SERVICIO') {
                const selectedSolp: SolpCrearPoMultipleDto[] = this.solps;
                let solpSeleccionadas: SolpCrearPoMultipleDto[] = selectedSolp.filter(solp => solp.Selected === true);
                // para cada solp marcada, busco los id de posición y los concateno en un sólo array
                for (let solp of solpSeleccionadas) {
                    if (!solp.Posiciones || !solp.Posiciones.length) {
                        await (this.service.listarPosicionesPOMultipleIdSolp(solp.Id).toPromise())
                            .then(x => solp.Posiciones = x.data)
                            .catch(reason => {
                                error = true;
                                errorMessaje = reason.message;
                            });
                        if (error) { throw errorMessaje; }
                    }

                    let posiciones: number[] = solp.Posiciones.reduce((accPos, pos) => accPos.concat(pos.Id), []);
                    ids = ids.concat(posiciones);
                }
            } else {
                const selectedPosiciones: POPosicionDto[] = this.posiciones;
                const posicionesSeleccionadas: POPosicionDto[] = selectedPosiciones.filter(posicion => posicion.Selected === true);
                ids = posicionesSeleccionadas.map(pos => pos.Id);
            }

            if (ids.length > 0) {
                this.goToSeccionParam('/compras/peticion-de-oferta-formulario', JSON.stringify(ids));
            } else {
                this.floatMsgService.setInfoMsg("Debe seleccionar al menos una posicion.");
            }
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
        } finally {
            this.blockUI.stop();
        }
    }
}

interface iFiltrosPoMultiple {
    sap: boolean;
    mantenimiento: boolean;
    web: boolean;
    repoAutomatica: boolean;
    contratoMarco: boolean;
    gruposCompras: string[];
    centros: string[];
    claseDocumento: string[];
    tipoImputacion: string[];
    valorTipoImputacion: string[];
    subtipoImputacionCombo: SelectItem[];
    fechaDesde: string;
    fechaHasta: string;
    tratada: boolean | null;
    numeroPo?: number;
    selectTipoSolp?: string;
    nombrePliego?: string;
    selectTipoPliego: string[];
}
