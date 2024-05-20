import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService, SelectItem } from 'primeng/api';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { ComprasService } from '../compras.service';
import { FiltroDto } from './agrupar-po-th-filtro-model';
import { EnumTipoImputacion } from '../enum-tipo-imputacion';
import { SolpDto } from './agrupar-po-th-model';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

@Component({
  selector: 'app-agrupar-po-th',
  templateUrl: './agrupar-po-th.component.html',
  styleUrls: ['../compras.component.css',
    './agrupar-po-th.component.css']
})
export class AgruparPoThComponent extends ListBaseComponent implements OnInit {
  @BlockUI() blockUI: NgBlockUI;
  agrupada: boolean;
  nombrePedido: string;
  nroSolp: string;
  nroPo: string;
  codigoProveedor: string;
  tipoPosicion: boolean;
  listaSolp: SolpDto;

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

  usuario: string;
  protected locale: any;

  TodasPosicionesSeleccionadas: boolean = false;
  fechaInicio: Date;
  fechaFin: Date;
  rangeDates: Date[];
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
  clasesDocumento: number[] = [];
  @ViewChild('myCalendar', undefined)
  sap: boolean = false;
  mantenimiento: boolean = false;
  web: boolean = false;
  repoAutomatica: boolean = false;
  contratoMarco: boolean = false;
  orden: string;
  columna: string = "NroSolp";
  itemsPorPagina: number = 10;
  pagina: number = 1;

  Agrupada: SelectItem[] = [{ label: "Si", value: true }, { label: "No", value: false }, { label: "Todas", value: null }];
  selectAgrupada: boolean | null = null;
  
  filtrosPOAgrupada: FiltroDto = {
    Pagina: 0,
    ItemsPorPagina: 0,
    Orden: '',
    Columna: '',
    NroSolp: '',
    NroPo: '',
    NombrePedido: '',
    FechaDesde: null,
    FechaHasta: new Date(),
    Sap: false,
    Mantenimiento: false,
    Web: false,
    RepoAutomatica: false,
    ContratoMarco: false,
    Estados: '',
    Usuarios: '',
    Centros: '',
    GrupoDeCompras: '',
    ClaseDocumento: '',
    TipoImputacion: '',
    ValorTipoImputacion: '',
    CodigoProveedor: '',
    ListarPendiente: false,
    EsServicio: true,
    Agrupada: false
  };

  ngOnInit() {
    this.filtrosPOAgrupada = {
      Pagina: 1,
      ItemsPorPagina: 10,
      Orden: '',
      Columna: 'NroSolp',
      NroSolp: '',
      NroPo: '',
      NombrePedido: '',
      FechaDesde: null,
      FechaHasta: new Date(),
      Sap: false,
      Mantenimiento: false,
      Web: false,
      RepoAutomatica: false,
      ContratoMarco: false,
      Estados: '',
      Usuarios: '',
      Centros: '',
      GrupoDeCompras: '',
      ClaseDocumento: '',
      TipoImputacion: '',
      ValorTipoImputacion: '',
      CodigoProveedor: '',
      ListarPendiente: false,
      EsServicio: true,
      Agrupada: false
    };
    this.recuperarFiltros();
    this.listarSolpCondicionEspecial()
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

  onBuscar() {
    this.filtrosPOAgrupada.Pagina = this.pagina,
    this.filtrosPOAgrupada.ItemsPorPagina = this.itemsPorPagina,
    this.filtrosPOAgrupada.Orden = this.orden,
    this.filtrosPOAgrupada.Columna = this.columna,
    this.filtrosPOAgrupada.CodigoProveedor = this.codigoProveedor;
    this.filtrosPOAgrupada.NombrePedido = this.nombrePedido;
    this.filtrosPOAgrupada.GrupoDeCompras = this.selectGrupoCompras.join(",");
    this.filtrosPOAgrupada.Centros = this.selectCentro.join(",");
    this.filtrosPOAgrupada.ClaseDocumento = this.selectClaseDocumento.join(",");
    this.filtrosPOAgrupada.TipoImputacion = this.selectTipoImputacion.join(",");
    this.filtrosPOAgrupada.ValorTipoImputacion = this.selectValorTipoImputacion.join(",");
    this.filtrosPOAgrupada.FechaDesde = this.fechaInicio;
    this.filtrosPOAgrupada.FechaHasta = this.fechaFin;
    this.filtrosPOAgrupada.Agrupada = this.agrupada;
    this.filtrosPOAgrupada.EsServicio = this.tipoPosicion;
    this.filtrosPOAgrupada.Sap = this.sap;
    this.filtrosPOAgrupada.Mantenimiento = this.mantenimiento;
    this.filtrosPOAgrupada.Web = this.web;
    this.filtrosPOAgrupada.RepoAutomatica = this.repoAutomatica;
    this.filtrosPOAgrupada.ContratoMarco = this.contratoMarco;
    console.log("buscar this.filtrosPOAgrupada", this.filtrosPOAgrupada);
    this.listarSolpCondicionEspecial();
    sessionStorage.setItem('filtrosPOAgrupada', JSON.stringify(this.filtrosPOAgrupada));
  }

  listarSolpCondicionEspecial() {
    try {
        this.blockUI.start('Cargando...');
        this.subscription = this.service.listarSolpCondicionEspecial(this.filtrosPOAgrupada
        ).subscribe(
            (result: any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    this.listaSolp = result.data;
                    console.log("this.listaSolp", this.listaSolp);
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

  recuperarFiltros() {
    const filtrosGuardados = JSON.parse(sessionStorage.getItem('filtrosPOAgrupada'));
    if (filtrosGuardados) {
        this.pagina = filtrosGuardados.Pagina;
        this.itemsPorPagina = filtrosGuardados.ItemsPorPagina;
        this.orden = filtrosGuardados.Orden;
        this.columna = filtrosGuardados.Columna;
        this.sap = filtrosGuardados.Sap;
        this.mantenimiento = filtrosGuardados.Mantenimiento;
        this.web = filtrosGuardados.Web;
        this.repoAutomatica = filtrosGuardados.RepoAutomatica;
        this.contratoMarco = filtrosGuardados.ContratoMarco;
        this.selectGrupoCompras = filtrosGuardados.GrupoDeCompras ? filtrosGuardados.GrupoDeCompras.split(",") : [];
        this.selectCentro = filtrosGuardados.Centros ? filtrosGuardados.Centros.split(",") : [];
        this.selectClaseDocumento = filtrosGuardados.ClaseDocumento ? filtrosGuardados.ClaseDocumento.split(",") : [];
        this.selectTipoImputacion = filtrosGuardados.TipoImputacion ? filtrosGuardados.TipoImputacion.split(",") : [];
        this.selectValorTipoImputacion = filtrosGuardados.ValorTipoImputacion ? filtrosGuardados.ValorTipoImputacion.split(",") : [];
        this.codigoProveedor = filtrosGuardados.CodigoProveedor;
        this.nombrePedido = filtrosGuardados.NombrePedido;
        this.fechaInicio = filtrosGuardados.FechaDesde;
        this.fechaFin = filtrosGuardados.FechaHasta;
        this.agrupada = filtrosGuardados.Agrupada;
        this.tipoPosicion = filtrosGuardados.EsServicio;
    }
  }

  onSelect(event: any) {
    if (this.rangeDates[0] && this.rangeDates[1] == null) {
        let d = new Date(Date.parse(event));
        this.fechaInicio = event;
        this.fechaFin = event;
    } else {
        let d = new Date(Date.parse(event));
        this.fechaFin = event;
    }
}
}
