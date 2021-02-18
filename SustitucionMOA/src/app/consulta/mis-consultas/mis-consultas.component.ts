import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
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
import { Calendar } from 'primeng/calendar';

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

    @ViewChild("lastdate")
    protected lastdate: Calendar;

    cols: any[];
    colsFiltered: any[];
    consultas: Consulta[];
    estados: EstadoConsulta[];
    categorias: Categoria[];
    subcategorias: Subcategoria[];

    
    estadosList: SelectItem[];
    categoriasList: SelectItem[];
    subcategoriasList: SelectItem[];

    fecha: any;
    es: any;
    desde: boolean;
    hasta: boolean;

    isExternal: boolean;

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

        sessionStorage.setItem("periodo", "2");
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
            { field: 'Id', header: 'ID', width: 3, filterType: 'number', visibleExternal: true },
            { field: 'RazonSocialCorredor', header: 'Corredor', width: 5, filterType: 'text', visibleExternal: false },
            { field: 'RazonSocialProveedor', header: 'Proveedor', width: 5, filterType: 'text', visibleExternal: false },
            { field: 'Categoria.Nombre', header: 'Categoria', width: 5, filterType: 'list', visibleExternal: true, listItems: this.categoriasList, idField: 'idCategoria', change: this.setSubcategorias },
            { field: 'SubCategoria', header: 'Subcategoria', width: 5, filterType: 'list', visibleExternal: false, listItems: this.subcategoriasList, idField: 'idSubCategoria' },
            { field: 'Asunto', header: 'Asunto', width: 10, filterType: 'text', visibleExternal: true },
            { field: 'EstadoConsulta.Nombre', header: 'Estado', width: 5, filterType: 'custom', visibleExternal: true },
            { field: 'FechaCreacion', header: 'Fecha Inicio', width: 7, filterType: 'date', visibleExternal: false },
            { field: 'FechaUltimaModificacion', header: 'Ult. Modif.', width: 7, filterType: 'date', visibleExternal: true },
            { field: 'DiasReclamo', header: 'Dias de Rec', width: 5, filterType: 'number', visibleExternal: false }
        ];

        let isExternal = this.isExternal;
        this.colsFiltered = this.cols.filter(x=> !isExternal || x.visibleExternal);
    }

    setSubcategorias(categoriasSeleccionadas){
       
        if(this.subcategorias){
            this.subcategoriasList = [];
            this.subcategorias.filter(x=> categoriasSeleccionadas.length == 0 || categoriasSeleccionadas.includes(x.CategoriaId)).forEach(x => this.subcategoriasList.push({ label: x.Nombre, value: x.Id}));
        }
        
        return this.subcategoriasList;
    }

    onFechaChange(dt) {
        if (this.desde && !this.hasta)
            this.filtrarFecha(dt, this.fecha, null);
        else if (!this.desde && this.hasta)
            this.filtrarFecha(dt, null, this.fecha);
        else if(!this.desde && !this.hasta)
            this.filtrarFecha(dt, this.fecha, this.fecha);
    }

    onFechaRangeChange(dt) {
        if(this.desde && this.hasta && this.fecha[0] != null && this.fecha[1] != null)
            this.filtrarFecha(dt, this.fecha[0], this.fecha[1]);
    }

    filtrarFecha(dt, desde, hasta){
        dt.filter([desde, hasta], 'fechaUltimaModificacion', 'dateRangeFilter');
    }

    obtenerColorEstado(idEstado) {
        var estado = this.estados.find(x=> x.Id == idEstado);

        return estado.Color || 'grey';
    }

    cambiarCalendar(dt) {
        this.fecha = null;
        this.filtrarFecha(dt, null, null);

        if(this.desde && this.hasta)
            this.lastdate.selectionMode = 'range';
        else
            this.lastdate.selectionMode = 'single';
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
                        this.categorias = result.categorias;
                        this.categoriasList = [];
                        this.categorias.forEach(x => this.categoriasList.push({ label: x.Nombre, value: x.Id}));
                        this.estados = result.estados;
                        this.estadosList = [];
                        this.estados.forEach(x => this.estadosList.push({ label: x.Descripcion, value: x.Id}));
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
                        this.consultas = result.data;
                        this.consultas.forEach(x=> {
                            x.FechaCreacion = new Date(this.getDateFromAspNetFormat(x.FechaCreacion));
                            x.FechaUltimaModificacion = new Date(this.getDateFromAspNetFormat(x.FechaUltimaModificacion));
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

        return false; //<-- Prevent Refresh
    }

    public getDateFromAspNetFormat(date: string): number {
        const re = /-?\d+/;
        const m = re.exec(date);
        return parseInt(m[0], 10);
    }
}
