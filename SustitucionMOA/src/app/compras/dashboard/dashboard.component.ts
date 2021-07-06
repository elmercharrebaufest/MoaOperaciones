import { Component, Input, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ListBaseComponent } from './../../common/base-components/list-base-component'
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { ComprasService } from '../compras.service';
import { SelectItem } from 'primeng/api';
import { Solp } from '../Solp';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { CrearContratoModule } from '../../crear-contrato/crear-contrato.module';
import { formatDate } from '@angular/common';
import { SortEvent } from 'primeng/api';
import { Table } from 'primeng/table';




declare var $: any;

@Component({
    selector: 'dashboard',
    templateUrl: `dashboard.component.html`,
    styleUrls: ['../compras.component.css'],
    providers: [ComprasService]

})
export class DashboardComponent extends ListBaseComponent {

    protected locale: any;
    
    @ViewChild("tabla")
    protected tabla: Table;
    

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

        this.locale = {
            firstDayOfWeek: 0,
            dayNames: ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"],
            dayNamesShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
            dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
            monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
            monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
            today: 'Today',
            clear: 'Clear'
        };

    }


    // minDate: Date;
    // maxDate: Date;
    // invalidDates: Date[];
    // rangeDates: Date[];
    desdeDashboard: Date;
    hastaDashboard: Date;
    estadoSolpItem: SelectItem[];
    // selectEstadoSolp: any;
    selectEstadoSolp: string[] = [];

    buscarDashboard: string;
    fechaSolp: any;
    hoy: Date = new Date();
    es: any;

    display: boolean = false;

    tablaSolp: any[];

    cols: any[];

    serviciosDashboard: any = "servicios"


    showDialog() {
        this.display = true;
    }

    cards = [
        { nombre: "Con documento de pliego", path: "/compras/solp" },
        // { nombre: "Con documentos requerimientos", path: ""},
        // { nombre: "Sin documento", path: ""},
        // { nombre: "Emergencia", path: ""},
        // { nombre: "Adicional", path: ""}
    ]


    goToSeccion(path: string) {
        $("#mySidenav").css({ 'right': '-270px' });
        $("#myMenuClose").css({ 'display': 'none' });
        $("#myMenuOpen").css({ 'display': 'block' });
        $("#coverAll").fadeOut();
        this.navService.navegarSeccion(path);
        return false;
    }

   


    ngOnInit() {
        this.navService.setSeccionList([]);

        

        // this.tablaSolp = [
        // {
        //     numeroSolp: "372872",
        //     fechaCreacion: "17/12/2021",
        //     estadoDoc: "Incompleto",
        //     estadoSolp: "Finalizada",
        //     tipoSolp: "Sin Doc.",
        //     estadoDocCodigo: "Incompleto",
        //     estadoSolpCodigo: "Finalizada",
        //     esSap: "Si",
        //     vincularPliego: "" 
        // }
        // ];

        this.getListarSolp();
        this.desdeDashboard = new Date();
        this.hastaDashboard = new Date();

        
    }

    ngAfterViewInit(): void {
        this.getCombos();

        this.tabla.filterConstraints['dateRangeFilter'] = (value, filter): boolean => {

            if (filter[0] != null && filter[1] != null)
                return value >= filter[0] &&
                    value <= filter[1];
            else if (filter[0] != null && filter[1] == null)
                return value >= filter[0]
            else if (filter[0] == null && filter[1] != null)
                return value <= filter[1].
            else
                return true;
        }

    }

    getListarSolp(){
            try {
                this.subscription = this.service.getListarSolp().subscribe(
                    result => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.floatMsgService.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.floatMsgService.setInfoMsg(result.info);
                        } else { 
                            this.tablaSolp = result.data;
                            console.log(this.tablaSolp)
                            this.tablaSolp.forEach(x => {
                                x.FechaCreacion = new Date(this.getDateFromAspNetFormat(x.FechaCreacion));
                                // x.FechaCreacion = formatDate(x.FechaCreacion)
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

    borrarSolp(idSolp){
        try {
            this.subscription = this.service.borrarSolp(idSolp).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else { 
                        this.getListarSolp();
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

    traerSolpId(idSolp){
        try {
            this.subscription = this.service.traerSolpId(idSolp).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else { 
                       
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

    getCombos(){
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
                        console.log(result);
                        this.estadoSolpItem = [];
                        result.EstadosSolpSap.forEach(cd => this.estadoSolpItem.push({
                            label: cd.Descripcion, value: cd.Id
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

    setUltimoAnio(){
        this.desdeDashboard = new Date();
        this.desdeDashboard.setFullYear(this.desdeDashboard.getFullYear() -1);
        this.hastaDashboard = new Date();
    }

    setUltimoMes(){
        this.desdeDashboard = new Date();
        this.desdeDashboard.setMonth(this.desdeDashboard.getMonth() -1);
        this.hastaDashboard = new Date();
    }

    filtrarFecha(dt, field, desde, hasta) {
        dt.filter([desde, hasta], field, 'dateRangeFilter');
        console.log(this.desdeDashboard, this.hastaDashboard);
    }

    filtrarPorFecha(){
        this.filtrarFecha(this.tabla, "FechaCreacion", this.desdeDashboard, this.hastaDashboard);
        // console.log(this.desdeDashboard, this.hastaDashboard);
    }

    
    

    

}






