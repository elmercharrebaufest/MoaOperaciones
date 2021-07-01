import { Component, Input } from '@angular/core';
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




declare var $: any;

@Component({
    selector: 'dashboard',
    templateUrl: `dashboard.component.html`,
    styleUrls: ['../compras.component.css'],
    providers: [ComprasService]

})
export class DashboardComponent extends ListBaseComponent {

    protected locale: any;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);



    }

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

        this.estadoSolpItem = [
            { label: 'Creada', value: 'Creada' },
            { label: 'Liberada', value: 'Liberada' },
            { label: 'Parc. liberada', value: 'Parc. liberada' },
            { label: 'Relac. a ped. compra', value: 'Relac. a ped. compra' },
            { label: 'Finalizada', value: 'Finalizada' }
        ];

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

        // this.tablaSolp = [{
        //     numeroSolp: "32173821744",
        //     fechaCreacion: "27/04/2021",
        //     estadoDoc: "Finalizado",
        //     estadoSolp: "Liberada",
        //     tipoSolp: "C/Doc. Pliego",
        //     estadoDocCodigo: "Finalizada",
        //     estadoSolpCodigo: "Liberado",
        //     esSap: "",
        //     vincularPliego: "" 
        // },
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


}






