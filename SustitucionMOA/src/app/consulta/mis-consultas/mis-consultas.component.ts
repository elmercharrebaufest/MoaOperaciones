import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FiltroFechaComponent } from './../../common/view-child/filtro-fecha/filtro-fecha.component';
import { ListBaseComponent } from './../../common/base-components/list-base-component'
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { DropdownComponent, DropdownOption } from './../../common/view-child/dropdown/dropdown.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { element } from '@angular/core/src/render3/instructions';
import { Seccion } from '../../common/models/seccion';
import { ConsultaService } from '../consulta.service';
import { Table } from 'primeng/table';
import { Categoria, Consulta, EstadoConsulta, Subcategoria } from '../consulta';
import { SelectItem } from 'primeng/components/common/selectitem';
import { formatDate } from '@angular/common';

declare var $: any;

@Component({
    selector: 'mis-consultas',
    templateUrl: `mis-consultas.component.html`,
    providers: [{ provide: ConsultaService, useClass: ConsultaService }]

})
export class MisConsultasComponent extends ListBaseComponent {

    @ViewChild('dropdown_categoria')
    protected categoriaDropdownComponent: DropdownComponent;

    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    @ViewChild("spinnerModal")
    protected spinnerModal: SpinnerSmallComponent;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild("dt")
    protected table: Table;

    @ViewChild("containerList")
    protected containerList: HTMLDivElement;

    cols: any[];
    colsFiltered: any[];
    consultas: Consulta[];
    estados: EstadoConsulta[];
    estadosSummary: EstadoConsulta[];
    categorias: Categoria[];
    subcategorias: Subcategoria[];
    
    subcategoriasList: SelectItem[];

    es: any;
    datesRange: SelectItem[] = [{label:'Fecha', value:null},{label:'Desde', value:'desde'}, {label:'Hasta', value:'hasta'}, {label:'Rango', value:'rango'}];
    isExternal: boolean;
    showFilters: boolean;
    windowSize: string;

    @HostListener('window:resize', ['$event']) onResize(event) {
        this.setColumnasByWindowSize();
     }

    constructor(protected service: ConsultaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CONTACTO MAIL"); }

    setTabs() {
        this.setMenuSeccionTab("consulta", "mis-consultas");
    }

    ngAfterViewInit(): void {
        this.listarConsultas();
        this.getCombos();

        this.es = {
            firstDayOfWeek: 1,
            dayNames: [ "domingo","lunes","martes","miércoles","jueves","viernes","sábado" ],
            dayNamesShort: [ "dom","lun","mar","mié","jue","vie","sáb" ],
            dayNamesMin: [ "D","L","M","X","J","V","S" ],
            monthNames: [ "enero","febrero","marzo","abril","mayo","junio","julio","agosto","septiembre","octubre","noviembre","diciembre" ],
            monthNamesShort: [ "ene","feb","mar","abr","may","jun","jul","ago","sep","oct","nov","dic" ],
            today: 'Hoy',
            clear: 'Borrar'
        }

        this.table.filterConstraints['dateRangeFilter'] = (value, filter): boolean => {
            
            if(filter[0] != null && filter[1] != null)
                return value.getDate() >= filter[0].getDate() &&
                value.getDate() <= filter[1].getDate();
            else if(filter[0] != null && filter[1] == null)
                return value.getDate() >= filter[0].getDate()
            else if(filter[0] == null && filter[1] != null)
                return value.getDate() <= filter[1].getDate()
            else 
                return true;
        }
    }
    
    ngOnInit(){
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/consulta/crear-consulta', 'crear-consulta', 'Nueva Consulta'), new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas')]);
    }

    addDays(date, days) {
        var result = new Date(date);
        result.setDate(result.getDate() + days);
        return result;
    }

    setColumnas(){
        this.cols = [
            { field: 'Id',                      header: 'Id',           filterType: 'text',     visibleExternal: true,  width: 6,  size: 4 },
            { field: 'RazonSocialCorredor',     header: 'Corredor',     filterType: 'text',     visibleExternal: false, width: 10, size: 4 },
            { field: 'RazonSocialProveedor',    header: 'Proveedor',    filterType: 'text',     visibleExternal: false, width: 10, size: 3 },
            { field: 'Categoria',               header: 'Categoria',    filterType: 'custom',   visibleExternal: true,  width: 10, size: 1 },
            { field: 'SubCategoria',            header: 'Subcategoria', filterType: 'custom',   visibleExternal: false, width: 12, size: 2 },
            { field: 'Asunto',                  header: 'Asunto',       filterType: 'text',     visibleExternal: true,  width: 16, size: 0 },
            { field: 'EstadoConsulta',          header: 'Estado',       filterType: 'custom',   visibleExternal: true,  width: 10, size: 1 },
            { field: 'FechaCreacion',           header: 'Fecha Inicio', filterType: 'date',     visibleExternal: false, width: 12, size: 2, selectionMode : 'single' },
            { field: 'FechaUltimaModificacion', header: 'Ult. Modif.',  filterType: 'date',     visibleExternal: true,  width: 12, size: 3, selectionMode : 'single' },
            { field: 'DiasReclamo',             header: 'Días',         filterType: 'text',     visibleExternal: false, width: 6,  size: 4 }
        ];

        let isExternal = this.isExternal;
        this.colsFiltered = this.cols.filter(x=> !isExternal || x.visibleExternal);
        this.setColumnasByWindowSize();
    }

    setColumnasByWindowSize(){
        var size = window.innerWidth;

        if(!this.isExternal){
            if(size <= 640 && this.windowSize != 'xs'){
                this.colsFiltered = this.cols.filter(x=> x.size <= 0)
                this.windowSize = 'xs'
            }

            if(size > 640 && size <= 768 && this.windowSize != 's'){
                this.colsFiltered = this.cols.filter(x=> x.size <= 1)
                this.windowSize = 's'
            }

            if(size > 768 && size <= 1160 && this.windowSize != 'm'){
                this.colsFiltered = this.cols.filter(x=> x.size <= 2)
                this.windowSize = 'm'
            }
            
            if(size > 1160 && size <= 1200 && this.windowSize != 'g'){
                this.colsFiltered = this.cols.filter(x=> x.size <= 3)
                this.windowSize = 'g'
            }

            if(size > 1200 && this.windowSize != 'xg'){
                this.colsFiltered = this.cols.filter(x=> x.size <= 4)
                this.windowSize = 'xg'
            }
        }
    }

    setSubcategorias(categoriasSeleccionadas){
        if(this.subcategorias){
            this.subcategoriasList = [];
            this.subcategorias.filter(x=> categoriasSeleccionadas.length == 0 || categoriasSeleccionadas.map(y=> y.Id).includes(x.CategoriaId)).forEach(x => this.subcategoriasList.push({ label: x.Nombre, value: x.Id}));
        }
        
        return this.subcategoriasList;
    }

    onFechaChange(dt, col) {
        let desde = col.selectedRange == 'desde' || col.selectedRange == 'rango';
        let hasta = col.selectedRange == 'hasta' || col.selectedRange == 'rango';

        if (desde && !hasta)
            this.filtrarFecha(dt, col.field, col.fecha, null);
        else if (!desde && hasta)
            this.filtrarFecha(dt, col.field, null, col.fecha);
        else if(!desde && !hasta)
            this.filtrarFecha(dt, col.field, col.fecha, col.fecha);
    }

    onFechaRangeChange(dt, col) {
        let desde = col.selectedRange == 'desde' || col.selectedRange == 'rango';
        let hasta = col.selectedRange == 'hasta' || col.selectedRange == 'rango';

        if(desde && hasta && col.fecha[0] != null && col.fecha[1] != null)
            this.filtrarFecha(dt, col.field, col.fecha[0], col.fecha[1]);
    }

    filtrarFecha(dt, field, desde, hasta){
        dt.filter([desde, hasta], field, 'dateRangeFilter');
    }

    cambiarCalendar(dt, col) {
        col.fecha = null;
        this.filtrarFecha(dt, col.field, null, null);

        if(col.selectedRange == 'rango')
            col.selectionMode = 'range';
        else
            col.selectionMode = 'single';
    }

    getCombos() {
        try {
            this.subscription = this.service.getCombos().subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        // this.categorias = result.categorias;
                        // this.estados = result.estados;
                        this.subcategorias = result.subcategorias;
                        this.subcategoriasList = [];
                        this.subcategorias.forEach(x => this.subcategoriasList.push({ label: x.Nombre, value: x.Id}));

                        this.isExternal = result.isExternal;
                        this.setColumnas();
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

    listarConsultas() {
        this.unsubscribe();
        try {
            this.subscription = this.service.listarConsultas().subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.consultas = result.data.consultas;
                        this.consultas.forEach(x=> {
                            x.Fecha = new Date(this.getDateFromAspNetFormat(x.Fecha));
                            x.FechaCreacion = new Date(this.getDateFromAspNetFormat(x.FechaCreacion));
                            x.FechaUltimaModificacion = new Date(this.getDateFromAspNetFormat(x.FechaUltimaModificacion));
                        });
                        this.estados = result.data.estados;
                        this.estados.forEach(e => {
                            let estado = result.data.estados.filter(x=> x.Id == e.Id)[0];
                            e.Cantidad = estado.Cantidad;
                        });

                        this.categorias = result.data.categorias;
                        this.categorias.forEach(c => {
                            let categoria = result.data.categorias.filter(x=> x.Id == c.Id)[0];
                            c.Cantidad = categoria.Cantidad;
                        });

                        let estadosCode = ['INI', 'GES'];
                        this.estadosSummary = result.data.estados.filter(e=> estadosCode.indexOf(e.Code) >= 0);
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

    getIds(options){
        return options.map(x=>x.Id);
    }

    exportConsultas(){
        var data = this.consultas.map(c => {
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
                "Dias de Reclamo":c.DiasReclamo,
                "Fecha de pago / Fecha factura / Fecha emision de la oblea": c.Fecha ? formatDate(c.Fecha, "dd/MM/yyyy", "en-EN") : "",
                "Nro Salida de pago / Nro factura": c.ComprobanteNo || "",
                "Nro Contrato": c.ContratoNo || "",
                "Impuesto retenido / Impuesto percibido / Impuesto": c.Impuesto || "",
                "Importe retención": c.Importe || "",
                "Causa":c.CausaConsulta? c.CausaConsulta.Nombre : '',
                "Bolsa emisora de oblea": c.BolsaEmisoraOblea || ""
            }
        });

        this.DownloadJsonData(data, 'Consultas', true);
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
}
