import { Component, ViewChild, HostListener, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute, } from '@angular/router';
import { ListBaseComponent } from './../../common/base-components/list-base-component'
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { DropdownComponent } from './../../common/view-child/dropdown/dropdown.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { Seccion } from '../../common/models/seccion';
import { ConsultaService } from '../consulta.service';
import { Table } from 'primeng/table';
import { Categoria, Consulta, EstadoConsulta, Subcategoria, Materiales, obtenerOpcionesFiltroPorCreacion, OpcionFiltroAsociadaCreacion, ReqListadoConsultaDto } from '../consulta';
import { SelectItem } from 'primeng/components/common/selectitem';
import { formatDate } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { HttpStatusCodes } from '../../common/models/httpStatusCodes';
import { DatosCartaPorteConDisconformidadCalidades, SendDataService } from '../send-data.service';
import { debounceTime, finalize, } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { DirOrden } from '../../common/enums/DirOrden';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { UsuarioService } from '../../usuario/usuario.service';
import { TipoConfiguracionUsuario } from '../../common/enums/TipoConfiguracionUsuario';

export interface ConfiguracionColumna {
    field: string,
    header: string,
    filterType: string,
    visibleExternal: boolean,
    selectionMode?: 'range' | 'single',
    width: number,
    size: number,
    visible?: boolean,
    filteredValue?: string
    sortdropdown?: string
}

declare var $: any;

@Component({
    selector: 'mis-consultas',
    templateUrl: `mis-consultas.component.html`,
    styleUrls: ['mis-consultas.component.css'],
    providers: [{ provide: ConsultaService, useClass: ConsultaService }, UsuarioService],
    // changeDetection: ChangeDetectionStrategy.OnPush
})
export class MisConsultasComponent extends ListBaseComponent {
    @BlockUI() blockUI: NgBlockUI;
    @ViewChild('dropdown_categoria')
    protected categoriaDropdownComponent: DropdownComponent;

    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    @ViewChild("spinnerModal")
    protected spinnerModal: SpinnerSmallComponent;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild("topSpinner")
    protected spinnerComponent: SpinnerComponent;
    @ViewChild('bottomSpinner')
    protected spinnerBottomComponent: SpinnerComponent;

    @ViewChild("dt")
    protected table: Table;

    @ViewChild("containerList")
    protected containerList: HTMLDivElement;

    cols: ConfiguracionColumna[] = [
        { field: 'Id', header: 'Id', filterType: 'text', visibleExternal: true, width: 8, size: 4, visible: true, filteredValue: '' },
        { field: 'RazonSocialCorredor', header: 'Corredor', filterType: 'text', visibleExternal: false, width: 10, size: 4, visible: true, filteredValue: '' },
        { field: 'RazonSocialProveedor', header: 'Proveedor', filterType: 'text', visibleExternal: false, width: 10, size: 3, visible: true, filteredValue: '' },
        { field: 'Categoria', header: 'Categoria', filterType: 'custom', visibleExternal: true, width: 10, size: 1, sortdropdown: 'Categoria.Nombre', visible: true },
        { field: 'SubCategoria', header: 'Subcategoria', filterType: 'custom', visibleExternal: false, width: 12, size: 2, sortdropdown: 'SubCategoria.Nombre', visible: true },
        { field: 'Asunto', header: 'Asunto', filterType: 'text', visibleExternal: true, width: 16, size: 0, visible: true, filteredValue: '' },
        { field: 'EstadoConsulta', header: 'Estado', filterType: 'custom', visibleExternal: true, width: 10, size: 1, sortdropdown: 'EstadoConsulta.Descripcion', visible: true },
        { field: 'Material', header: 'Material', filterType: 'custom', visibleExternal: true, width: 10, size: 3, sortdropdown: 'Material', visible: true },
        { field: 'FechaCreacion', header: 'Fecha Inicio', filterType: 'date', visibleExternal: false, width: 12, size: 3, selectionMode: 'single', visible: true },
        { field: 'FechaUltimaModificacion', header: 'Ult. Modif.', filterType: 'date', visibleExternal: true, width: 12, size: 3, selectionMode: 'single', visible: true },
        { field: 'DiasReclamo', header: 'Días', filterType: 'text', visibleExternal: false, width: 6, size: 4 },
    ];;
    colsFiltered: ConfiguracionColumna[];
    consultas: Consulta[];
    consultasFiltradas: Consulta[];
    estados: EstadoConsulta[];
    selectedEstados: EstadoConsulta[] = [
        { Id: 1, Code: 'INI', Descripcion: 'Iniciada' },
        { Id: 2, Code: 'GES', Descripcion: 'En gestión' },
        { Id: 3, Code: 'GESRTA', Descripcion: 'En gestión Rta' },
        { Id: 4, Code: 'DOC', Descripcion: 'Solicitud de información' },
        { Id: 5, Code: 'REC', Descripcion: 'Rechazado' }
    ]
    estadosSummary: EstadoConsulta[];
    categorias: Categoria[];
    subcategorias: Subcategoria[];
    materiales: Materiales[];
    materialesList: SelectItem[];
    consultaId: any = 1;
    paraInnerHtml: string = "";
    subcategoriasList: SelectItem[];
    mostrarDetalle: boolean = false;
    es: any;
    datesRange: SelectItem[] = [{ label: 'Fecha', value: null }, { label: 'Desde', value: 'desde' }, { label: 'Hasta', value: 'hasta' }, { label: 'Rango', value: 'rango' }];
    isExternal: boolean;
    showFilters: boolean;
    windowSize: string;
    esInterno = this.isAuthorized('CONSULTA ABM');
    widthModal: string;
    asunto: string;
    estadoConsultaSeleccionado: EstadoConsulta[];
    categoriaSeleccionada: Categoria[];

    opcionesFiltroPorCreacion = obtenerOpcionesFiltroPorCreacion();
    filtrosPorCreacionSeleccionados: { key: OpcionFiltroAsociadaCreacion, label: OpcionFiltroAsociadaCreacion }[] = [];

    datosCartaPorteConDisconformidadCalidades?: DatosCartaPorteConDisconformidadCalidades;

    iconoModalDetalle = 'pi-window-maximize';
    modalMaximizado = false;

    $buscarConsultas = new Subject<void>();
    $guardarConfiguracion = new Subject<string>();

    @HostListener('window:resize', ['$event']) onResize(event) {
        this.setColumnasByWindowSize();
    }

    constructor(
        private usuarioService: UsuarioService,
        private cdr: ChangeDetectorRef,
        protected service: ConsultaService,
        protected navService: NavService,
        protected sessionDataService: SessionDataService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService,
        protected route: ActivatedRoute,
        protected router: Router,
        private sendDataService: SendDataService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

        this.datosCartaPorteConDisconformidadCalidades = sendDataService.getDatosCartaPorteConDisconformidadCalidades();
        this.$buscarConsultas.pipe(debounceTime(350)).subscribe(() => {
            this.toggleSpinner(true)
            this.listarConsultas()
        }
        )
        this.$guardarConfiguracion.pipe(debounceTime(250)).subscribe((valor) => {
            this.usuarioService.guardarConfiguracionUsuario({
                valor,
                tipo: TipoConfiguracionUsuario.ColumnaConsultas
            }).subscribe(console.info)
        })
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CONTACTO MAIL"); }

    setTabs() {
        this.setMenuSeccionTab("consulta", "Mis Consultas");
    }

    ngAfterViewInit(): void {
        this.toggleSpinner(true)
        this.listarConsultas();
        this.getCombos();

        this.es = {
            firstDayOfWeek: 1,
            dayNames: ["domingo", "lunes", "martes", "miércoles", "jueves", "viernes", "sábado"],
            dayNamesShort: ["dom", "lun", "mar", "mié", "jue", "vie", "sáb"],
            dayNamesMin: ["D", "L", "M", "X", "J", "V", "S"],
            monthNames: ["enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"],
            monthNamesShort: ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"],
            today: 'Hoy',
            clear: 'Borrar'
        }

        this.table.filterConstraints['DateRangeFilter'] = (value, filter): boolean => {
            if (filter[0] != null && filter[1] != null)
                return value >= filter[0] &&
                    value <= filter[1];
            else if (filter[0] != null && filter[1] == null) {
                return value >= filter[0];
            }
            else if (filter[0] == null && filter[1] != null)
                return value <= filter[1]
            else
                return true;
        }
    }

    setfilter() {
        this.route.queryParams.subscribe(params => {
            const filtrosActivados = params['filtrosActivados'];
            const categoria = params['categoria'];
            const estadoConsulta = params['filter'];
            const proveedor = params['proveedor'];
            const idProveedor = params['idProveedor'];


            if (filtrosActivados && filtrosActivados === 'true') {
                this.showFilters = true;

            }

            if (filtrosActivados && filtrosActivados === 'true' && categoria) {
                if (idProveedor.startsWith('C')) {
                    this.seleccionarOpcionFiltro('RazonSocialCorredor', proveedor);

                }
                else {
                    this.seleccionarOpcionFiltro('RazonSocialProveedor', proveedor);
                }

            }

            if (filtrosActivados && filtrosActivados === 'true' && categoria) {
                this.seleccionarOpcionFiltro('Categoria', categoria);
            }

            if (filtrosActivados && filtrosActivados === 'true' && estadoConsulta) {
                this.seleccionarOpcionFiltro('EstadoConsulta', estadoConsulta);
            }



        });
    }


    seleccionarOpcionFiltro(columna: string, valor: string) {

        valor = decodeURIComponent(valor);

        switch (columna) {



            case 'RazonSocialCorredor':


                this.table.filter(valor, 'RazonSocialCorredor', 'contains');
                let colC = this.cols.find(cols => cols.header === 'Corredor');

                colC.filteredValue = valor;
                break;
            case 'RazonSocialProveedor':
                this.table.filter(valor, 'RazonSocialProveedor', 'contains');
                let colP = this.cols.find(cols => cols.header === 'Proveedor');

                colP.filteredValue = valor;
                break;
            case 'EstadoConsulta':
                // Encuentra la opción correspondiente en la lista de estados y selecciónala
                const estadoSeleccionado = this.estados.find(estado => estado.Code === valor);
                if (estadoSeleccionado) {
                    this.estadoConsultaSeleccionado = [estadoSeleccionado]; // Asigna la opción seleccionada al filtro

                    this.table.filter(estadoSeleccionado.Descripcion, 'EstadoConsulta.Descripcion', 'equals');
                }
                break;
            case 'Categoria':
                // Encuentra la opción correspondiente en la lista de categorías y selecciónala
                const categoriaSeleccionada = this.categorias.find(categoria => categoria.Nombre === valor);
                if (categoriaSeleccionada) {
                    this.categoriaSeleccionada = [categoriaSeleccionada]; // Asigna la opción seleccionada al filtro
                    this.table.filter(categoriaSeleccionada.Nombre, 'Categoria.Nombre', 'equals');
                    // Además, puedes manejar cualquier lógica relacionada con la selección de subcategorías si es necesario
                }
                break;
            // Repite el proceso para otros filtros si es necesario
            default:
                break;
        }
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.setSeccionList();
    }

    setSeccionList() {
        if (this.securityService.tienePermiso("CARGAR CONSULTA") && this.securityService.tienePermiso("CARGAR CONSULTA INTERNA")) {
            this.navService.setSeccionList([new Seccion('/consulta/crear-consulta', 'crear-consulta', 'Nueva Consulta'), new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas'),
            new Seccion('/consulta/crear-consulta-interna', 'crear-consulta-interna', 'Nueva Consulta Interna')
            ]);
        }
        else if (this.securityService.tienePermiso("CARGAR CONSULTA")) {
            this.navService.setSeccionList([new Seccion('/consulta/crear-consulta', 'crear-consulta', 'Nueva Consulta'), new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas')
            ]);
        } else if (this.securityService.tienePermiso("CARGAR CONSULTA INTERNA")) {
            this.navService.setSeccionList([new Seccion('/consulta/crear-consulta-interna', 'crear-consulta-interna', 'Nueva Consulta Interna'), new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas')
            ]);
        } else {
            this.navService.setSeccionList([new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas')
            ]);
        }
    }

    addDays(date, days) {
        var result = new Date(date);
        result.setDate(result.getDate() + days);
        return result;
    }

    setColumnas() {

        let isExternal = this.isExternal;
        this.colsFiltered = this.cols.filter(x => !isExternal || x.visibleExternal);
        this.setColumnasByWindowSize();
    }

    setColumnasByWindowSize() {
        var size = window.innerWidth;

        if (!this.isExternal) {
            if (size <= 640 && this.windowSize != 'xs') {
                this.colsFiltered = this.cols.filter(x => x.size <= 0)
                this.windowSize = 'xs'
            }

            if (size > 640 && size <= 768 && this.windowSize != 's') {
                this.colsFiltered = this.cols.filter(x => x.size <= 1)
                this.windowSize = 's'
            }

            if (size > 768 && size <= 1160 && this.windowSize != 'm') {
                this.colsFiltered = this.cols.filter(x => x.size <= 2)
                this.windowSize = 'm'
            }

            if (size > 1160 && size <= 1200 && this.windowSize != 'g') {
                this.colsFiltered = this.cols.filter(x => x.size <= 3)
                this.windowSize = 'g'
            }

            if (size > 1200 && this.windowSize != 'xg') {
                this.colsFiltered = this.cols.filter(x => x.size <= 4)
                this.windowSize = 'xg'
            }
        }

        this.obtenerConfiguracionDeTablasDelUsuario()
    }

    setSubcategorias(categoriasSeleccionadas) {
        if (this.subcategorias) {
            this.subcategoriasList = [];
            this.subcategorias.filter(x => categoriasSeleccionadas.length == 0 || categoriasSeleccionadas.map(y => y.Id).includes(x.CategoriaId)).forEach(x => this.subcategoriasList.push({ label: x.Nombre, value: x.Id }));
        }

        return this.subcategoriasList;
    }

    onFechaChange(dt: any, col: { selectedRange: string; field: any; fecha: any; }) {
        let desde = col.selectedRange == 'desde' || col.selectedRange == 'rango';
        let hasta = col.selectedRange == 'hasta' || col.selectedRange == 'rango';

        if (desde && !hasta)
            this.filtrarFecha(dt, col.field, col.fecha, null);
        else if (!desde && hasta)
            this.filtrarFecha(dt, col.field, null, col.fecha);
        else if (!desde && !hasta)
            this.filtrarFecha(dt, col.field, col.fecha, col.fecha);
    }

    onFechaRangeChange(dt, col) {
        let desde = col.selectedRange == 'desde' || col.selectedRange == 'rango';
        let hasta = col.selectedRange == 'hasta' || col.selectedRange == 'rango';

        if (desde && hasta && col.fecha[0] != null && col.fecha[1] != null)
            this.filtrarFecha(dt, col.field, col.fecha[0], col.fecha[1]);
    }

    filtrarFecha(dt, field, desde, hasta) {
        // dt.filter([desde, hasta], field, 'DateRangeFilter');
        this.cambiarFiltro(field, [desde, hasta], (!!desde) || (!!hasta));
    }

    cambiarCalendar(dt, col) {
        col.fecha = null;
        this.filtrarFecha(dt, col.field, null, null);

        if (col.selectedRange == 'rango')
            col.selectionMode = 'range';
        else
            col.selectionMode = 'single';
    }

    getCombos() {
        try {
            this.subscription = this.service.getCombos(false, true).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.subcategorias = result.subcategorias;
                        this.subcategoriasList = [];
                        this.subcategorias.forEach(x => this.subcategoriasList.push({ label: x.Nombre, value: x.Id }));
                        this.materiales = result.materiales;
                        this.materialesList = [];
                        this.materiales.forEach(x => this.materialesList.push({ label: x.Descripcion, value: x.MaterialId }))
                        this.isExternal = result.isExternal;
                        this.setColumnas();
                    }
                },
                (error: HttpErrorResponse) => {
                    this.floatMsgService.setErrorMsg(HttpStatusCodes.friendlyStatusCode(error.status));
                    console.log(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    openModal(idConsulta, asunto) {
        if (this.mostrarDetalle) {
            return this.resetVariables();
        }

        this.consultaId = idConsulta;
        this.mostrarDetalle = true;
        this.asunto = asunto;
        document.getElementById("openModalHiddenButton").click();
    }

    filtros?: Record<keyof Consulta, any> = {
        EstadoConsultaId: [1, 2, 3, 4, 5]
    } as any;

    get requestListado(): ReqListadoConsultaDto {
        return {
            page: this.page,
            pageSize: this.pageSize,
            orderBy: this.ordenCol,
            filtros: this.filtros,
            dirOrden: this.dirOrden
        }
    }

    listarConsultas() {
        this.unsubscribe();
        try {
            this.subscription = this.service
                .listarConsultas(this.requestListado)
                .subscribe(
                    (result: any) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.floatMsgService.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.floatMsgService.setInfoMsg(result.info);
                        } else {
                            this.consultas = result.data.consultas.length ? this.mapearLista(result.data.consultas) : [];
                            this.filtrarPorTipoGeneracion();
                            this.totalConsultas = result.data.totalConsultas;
                            this.pageSize = result.data.pageItem;

                            this.estados = result.data.estados;

                            this.categorias = result.data.categorias;

                            let estadosCode = ['INI', 'GES', 'GESRTA', 'DOC'];
                            this.estadosSummary = result.data.estados.filter(e => estadosCode.indexOf(e.Code) >= 0);

                            if (this.datosCartaPorteConDisconformidadCalidades)
                                this.abrirDetalleConsultaCartaPorteConDiscrepanciaCalidad();
                            this.setfilter();
                        }
                        this.toggleSpinner(false);
                        this.cdr.detectChanges()
                    },
                    (error: HttpErrorResponse) => {
                        this.toggleSpinner(false);
                        this.cdr.detectChanges()
                        this.floatMsgService.setErrorMsg(HttpStatusCodes.friendlyStatusCode(error.status));
                        console.log(error.message);
                    }
                );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    getIds(options) {
        return options.map(x => x.Id);
    }

    getLabel(option) {
        this.materialesList.forEach(element => {
            if (element.value == option) {
                return element.label;
            }
        });
    }

    exportandoConsultas = false;
    exportConsultas() {
        try {
            this.spinnerComponent.showIt();
            this.exportandoConsultas = true;
            this.service.listaExportacionConsultas(this.requestListado.filtros)
                .subscribe((res) => {
                    const lista = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent);
                    if (lista) {
                        var data = this.mapearLista(lista).map(c => {
                            return {
                                "Id": c.Id,
                                "Corredor": c.RazonSocialCorredor || "",
                                "Proveedor": c.RazonSocialProveedor,
                                "Categoria": c.Categoria.Nombre,
                                "SubCategoria": c.SubCategoria.Nombre,
                                "Asunto": c.Asunto,
                                "Estado": c.EstadoConsulta.Descripcion,
                                "Fecha Creacion": formatDate(c.FechaCreacion, "dd/MM/yyyy", "en-EN"),
                                "Fecha Ultima Modificacion": formatDate(c.FechaUltimaModificacion, "dd/MM/yyyy", "en-EN"),
                                "Dias de Reclamo": c.DiasReclamo,
                                "Fecha de pago / Fecha factura / Fecha emision de la oblea": c.Fecha ? formatDate(c.Fecha, "dd/MM/yyyy", "en-EN") : "",
                                "Nro Salida de pago / Nro factura": c.ComprobanteNo || "",
                                "Nro Contrato": c.ContratoNo || "",
                                "Impuesto retenido / Impuesto percibido / Impuesto": c.Impuesto || "",
                                "Importe retención": c.Importe || "",
                                "Causa": c.CausaConsulta ? c.CausaConsulta.Nombre : '',
                                "Bolsa emisora de oblea": c.BolsaEmisoraOblea || "",
                                "Usuario que inicia consulta": c.MailUsuarioIniciaConsulta || '',
                                "Rubro/s (Discrepancia)": c.Rubro || ''
                            }
                        });

                        this.DownloadJsonData(data, 'Consultas', true);
                    }
                    this.spinnerComponent.hideIt();
                    this.exportandoConsultas = false
                })
        } catch (e) {
            this.exportandoConsultas = false;
            console.error(e)
        }
    }

    resetVariables() {
        this.consultaId = 1;
        this.mostrarDetalle = false;
    }

    DownloadJsonData(JSONData, FileTitle, ShowLabel) {
        //If JSONData is not an object then JSON.parse will parse the JSON string in an Object
        let separator = ';';

        var arrData = typeof JSONData != 'object' ? JSON.parse(JSONData) : JSONData;
        var CSV = '';
        //This condition will generate the Label/Header
        if (ShowLabel) {
            var row = "";
            //This loop will extract the label from 1st index of on array
            for (var index in arrData[0]) {
                //Now convert each value to string and comma-seprated
                row += index + separator;
            }
            row = row.slice(0, -1);
            //append Label row with line break
            CSV += row + '\r\n';
        }
        //1st loop is to extract each row
        for (var i = 0; i < arrData.length; i++) {
            var row = "";
            //2nd loop will extract each column and convert it in string comma-seprated
            for (var index in arrData[i]) {
                row += '"' + arrData[i][index] + '"' + separator;
            }
            row.slice(0, row.length - 1);
            //add a line break after each row
            CSV += row + '\r\n';
        }
        if (CSV == '') {
            alert("Invalid data");
            return;
        }
        //Generate a file name
        var filename = FileTitle + (new Date());
        var blob = new Blob([CSV], {
            type: 'text/csv;charset=utf-8;'
        });
        if (navigator.msSaveBlob) { // IE 10+
            navigator.msSaveBlob(blob, filename);
        } else {
            var link = document.createElement("a");
            if (link.download !== undefined) { // feature detection
                // Browsers that support HTML5 download attribute
                var url = URL.createObjectURL(blob);
                link.setAttribute("href", url);
                link.style.visibility = "hidden";
                link.download = filename + ".csv";
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }
        }
    }
    filtrarPorTipoGeneracion() {
        if (!this.consultas || !this.consultas.length) {
            return this.consultasFiltradas = [];
        }
        const filtrarPorGeneradasPorUsuario = this.filtrarPorGeneradasPorUsuario();
        const filtrarPorGeneradasPorMOA = this.filtrarPorGeneradasPorMOA();
        const filtrarPorGeneradasPorExterno = this.filtrarPorGeneradasPorExterno();
        //Opción donde se ven todas las consultas
        //Retornamos sin filtrar
        if ((filtrarPorGeneradasPorExterno &&
            filtrarPorGeneradasPorMOA &&
            filtrarPorGeneradasPorUsuario) ||
            (!filtrarPorGeneradasPorExterno &&
                !filtrarPorGeneradasPorMOA &&
                !filtrarPorGeneradasPorUsuario) ||
            this.filtrosPorCreacionSeleccionados.length == 0) {
            this.consultasFiltradas = [...this.consultas]
            return;
        }

        this.consultasFiltradas = this.consultas.filter(consulta => {
            return (filtrarPorGeneradasPorExterno && consulta.GeneradaExternamente) ||
                (filtrarPorGeneradasPorMOA && consulta.GeneradaInternamente) ||
                (filtrarPorGeneradasPorUsuario && consulta.GeneradaPorUsuarioSesion)
        })
    }

    filtrarPorGeneradasPorUsuario() {
        return !!this.filtrosPorCreacionSeleccionados.find(sel => sel.key === OpcionFiltroAsociadaCreacion.PorUsuario)
    }
    filtrarPorGeneradasPorMOA() {
        return !!this.filtrosPorCreacionSeleccionados.find(sel => sel.key === OpcionFiltroAsociadaCreacion.PorMOA)
    }
    filtrarPorGeneradasPorExterno() {
        return !!this.filtrosPorCreacionSeleccionados.find(sel => sel.key === OpcionFiltroAsociadaCreacion.Externa)
    }

    public extraOnDestroy(): void {
        this.sendDataService.limpiarDatosCartaPorteConDisconformidadCalidades();
    }

    abrirDetalleConsultaCartaPorteConDiscrepanciaCalidad() {
        this.blockUI.start('Buscando Consulta por Disconformidad ...')
        this.service.obtenerConsultaDisconformidad(this.datosCartaPorteConDisconformidadCalidades.NroCCPP)
            .pipe(finalize(() => this.blockUI.stop()))
            .subscribe(res => {
                const consulta = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent)
                if (!consulta) {
                    return;
                }
                this.sendDataService.limpiarDatosCartaPorteConDisconformidadCalidades()
                this.openModal(consulta.Id, consulta.Asunto)
            })
    }
    toggleMaximizarModalDetalle() {
        this.modalMaximizado = !this.modalMaximizado;
        this.iconoModalDetalle = this.modalMaximizado ? 'pi-window-minimize' : 'pi-window-maximize'
    }
    verSeleccionColumnas = false;
    private keyConfiguracionTablas = 'columnasMisConsultas'

    obtenerConfiguracionDeTablasDelUsuario() {
        let colConfig = sessionStorage.getItem(this.keyConfiguracionTablas);
        if (colConfig) {
            let visibleCols = colConfig.split(',');
            const cantidadColumnas = visibleCols.length;
            // const  = visibleCols.length;

            this.colsFiltered = this.colsFiltered
                .map(c => {
                    c.visible = visibleCols.includes(c.field);
                    // c.width=
                    return c;
                })
        }
        else {
            this.guardarConfiguracionDeTablasDeUsuario();
            this.obtenerConfiguracionDeTablasDelUsuario();
        }
    }

    guardarConfiguracionDeTablasDeUsuario() {
        let visibleColumns = this.colsFiltered.filter(col => col.visible).map(col => col.field);
        const valorGuardado = visibleColumns.join(',');
        sessionStorage.setItem(this.keyConfiguracionTablas, valorGuardado);
        this.colsFiltered = [... this.colsFiltered]
        this.$guardarConfiguracion.next(valorGuardado)
    }

    pageSize = 20;
    totalConsultas = 0;
    page = 1;
    ordenCol: keyof Consulta = "FechaUltimaModificacion"
    dirOrden = DirOrden.Desc;

    handlePageEvent({ page }: { page: number; rows: number; pageCount: number }) {
        this.page = page + 1;
        this.$buscarConsultas.next()
    }

    cambiarFiltro(campo: string, value: any, buscar = true) {
        this.filtros[campo] = value;
        if (buscar) {
            this.$buscarConsultas.next();
        }
    }

    toggleSpinner(ver: boolean) {
        if (ver) {
            this.spinnerComponent.showIt()
            if (this.spinnerBottomComponent)
                this.spinnerBottomComponent.showIt()
        }
        else {
            this.spinnerComponent.hideIt()
            if (this.spinnerBottomComponent)
                this.spinnerBottomComponent.hideIt()
        }
    }
    changeSort(event: { field: string, order: number }) {
        this.dirOrden = event.order == -1 ? DirOrden.Desc : DirOrden.Asc;
        this.ordenCol = event.field as keyof Consulta;
        this.$buscarConsultas.next();
    }
    mapearLista(lista: Consulta[]): Consulta[] {
        return lista.map(x => {
            x.Fecha = x.Fecha == undefined ? null : new Date(this.getDateFromAspNetFormat(x.Fecha));
            x.FechaCreacion = new Date(this.getDateFromAspNetFormat(x.FechaCreacion));
            x.FechaUltimaModificacion = new Date(this.getDateFromAspNetFormat(x.FechaUltimaModificacion));
            return x;
        });
    }
}
