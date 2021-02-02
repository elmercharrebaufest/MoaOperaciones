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
import { Categoria, Consulta, EstadoConsulta } from '../consulta';
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

    constructor(protected service: ConsultaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CONTACTO MAIL"); }

    setTabs() {
        this.setMenuSeccionTab("consulta", "mis-consultas");
    }

    ngAfterViewInit(): void {

        this.listarConsultas();
        this.getCategorias();
        this.getEstados();
    }
    
    ngOnInit(){
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas'), new Seccion('/consulta/crear-consulta', 'crear-consulta', 'Nueva Consulta')]);

        this.cols = [
            { field: 'id', header: 'ID' },
            { field: 'asunto', header: 'Asunto' },
            { field: 'estado', header: 'Estado' },
            { field: 'categoria', header: 'Categoria' },
            { field: 'fechaUltimaModificacion', header: 'Ult. Modif.' }
        ];

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

    addDays(date, days) {
        var result = new Date(date);
        result.setDate(result.getDate() + days);
        return result;
      }

    @ViewChild("dt")
    protected table: Table;

    @ViewChild("lastdate")
    protected lastdate: Calendar;

    cols: any[];
    consultas: Consulta[];
    estados: EstadoConsulta[];
    categorias: Categoria[];

    estadosList: SelectItem[];
    categoriasList: SelectItem[];

    fecha: any;
    es: any;
    desde: boolean;
    hasta: boolean;

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
        var estado = this.estados.find(x=> x.id == idEstado);

        return estado.color || 'grey';
    }

    cambiarCalendar(dt) {
        this.fecha = null;
        this.filtrarFecha(dt, null, null);

        if(this.desde && this.hasta)
            this.lastdate.selectionMode = 'range';
        else
            this.lastdate.selectionMode = 'single';
    }

    getCategorias() {
        try {
            this.subscription = this.service.getCategorias().subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.categorias = result.data;
                        this.categoriasList = [];
                        this.categorias.forEach(x => this.categoriasList.push({ label: x.label, value: x.id}));
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

    getEstados() {
        try {
            this.subscription = this.service.getEstados().subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.estados = result.data;
                        this.estadosList = [];
                        this.estados.forEach(x => this.estadosList.push({ label: x.nombre, value: x.id}));
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
                            x.fechaCreacion = new Date(this.getDateFromAspNetFormat(x.fechaCreacion));
                            x.fechaUltimaModificacion = new Date(this.getDateFromAspNetFormat(x.fechaUltimaModificacion));
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

    mostrarCamposAdicionales(idCategoria) {
        var categoria = this.categorias.find(x=>x.id == idCategoria);

        return categoria.camposAdicionales
    }
}