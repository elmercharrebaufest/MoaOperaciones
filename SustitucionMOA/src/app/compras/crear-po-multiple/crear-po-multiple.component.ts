import { Component, OnInit, ViewChild } from '@angular/core';
import { ComprasService } from '../compras.service';
import { UsuarioService } from '../../usuario/usuario.service';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ConfirmationService, SelectItem } from 'primeng/api';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Table } from 'primeng/table';
import { SolpCompraDto, PosicionCompra, SolpSubposicionDto, SolpProveedorDto, EnvioSolpCompra, AltaNuevoProveedor } from '../solp-compra';
import { EnumTipoSolpSap } from '../enum-tipo-solp-sap';
import { POPosicionDto } from '../../modelos/po-posicionDto';
import { Subscription } from 'rxjs';
import { EnumTipoImputacion } from '../enum-tipo-imputacion';

@Component({
    selector: 'app-crear-po-multiple',
    templateUrl: './crear-po-multiple.component.html',
    styleUrls: ['../compras.component.css',
        './crear-po-multiple.component.css'],
})

export class CrearPoMultipleComponent extends ListBaseComponent implements OnInit {

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
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
    posiciones: POPosicionDto[] = [];
    numeroPo?: number = null;

    tratada: SelectItem[] = [{ label: "Si", value: true }, { label: "No", value: false }, { label: "Todas", value: null }];
    selectTratada: boolean | null = null;

    filtrosPOMultiple: {
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
    } = {
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
        };

    ngOnInit() {

        this.filtrosPOMultiple = {
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
        };
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
        if (this.TodasPosicionesSeleccionadas) {
            this.posiciones.map(pos => pos.Selected = true);
        } else {
            this.posiciones.map(pos => pos.Selected = false);
        }
    }

    listarPosicionesPOMultiple() {
        try {
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
                this.numeroPo
            ).subscribe(
                (result: any) => {

                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.posiciones = result.data;
                        this.blockUI.stop();
                    }
                },
                error => {
                    this.blockUI.stop();
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    onBuscar() {
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
        this.listarPosicionesPOMultiple();
        sessionStorage.setItem('filtrosPOMultiple', JSON.stringify(this.filtrosPOMultiple));
    }

    recuperarFiltros() {
        const filtrosGuardados = JSON.parse(sessionStorage.getItem('filtrosPOMultiple'));
        if (filtrosGuardados) {
            this.sap = filtrosGuardados.sap;
            this.mantenimiento = filtrosGuardados.mantenimiento;
            this.web = filtrosGuardados.web;
            this.repoAutomatica = filtrosGuardados.repoAutomatica;
            this.contratoMarco = filtrosGuardados.contratoMarco;
            this.selectGrupoCompras = filtrosGuardados.gruposCompras;
            this.selectCentro = filtrosGuardados.centros;
            this.selectClaseDocumento = filtrosGuardados.claseDocumento;
            this.selectTipoImputacion = filtrosGuardados.tipoImputacion;
            this.selectValorTipoImputacion = filtrosGuardados.valorTipoImputacion;
            this.valorTipoImputacionFiltro = filtrosGuardados.subtipoImputacionCombo;
            this.fechaInicio = filtrosGuardados.fechaDesde;
            this.fechaFin = filtrosGuardados.fechaHasta;
            this.selectTratada = filtrosGuardados.tratada;
            this.numeroPo = filtrosGuardados.numeroPo;
            if (this.fechaInicio != undefined && this.fechaInicio.length > 0) {
                const [year, month, day] = this.fechaInicio.split('-').map(Number); //se maneja el cambio de día incorrecto por la zona horaria local
                if (this.fechaFin != undefined && this.fechaFin.length > 0) {
                    const [year2, month2, day2] = this.fechaFin.split('-').map(Number);
                    this.rangeDates = [new Date(year, month - 1, day), new Date(year2, month2 - 1, day2)];
                } else {
                    this.rangeDates = [new Date(year, month - 1, day)];
                }
            }
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

    publicarCotizacion(selectedPosiciones: any[]): void {
        const posicionesSeleccionadas = selectedPosiciones.filter(posicion => posicion.Selected === true);
        // Aquí puedes hacer lo que necesites con las posiciones seleccionadas
        const ids = posicionesSeleccionadas.map(pos => pos.Id);
        // Por ejemplo, puedes enviarlas a una función que maneje la lógica de publicación
        // this.enviarPosicionesSeleccionadas(posicionesSeleccionadas);
        if (ids.length > 0) {
            this.goToSeccionParam('/compras/peticion-de-oferta-formulario', JSON.stringify(ids));
        } else {
            this.floatMsgService.setInfoMsg("Debe seleccionar al menos una posicion.");
        }
    }



}



