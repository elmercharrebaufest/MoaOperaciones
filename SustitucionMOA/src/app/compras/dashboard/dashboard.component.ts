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

        this.tablaSolp = [{
            numeroSolp: "32173821744",
            fechaCreacion: "27/04/2021",
            estadoDoc: "Finalizado",
            estadoSolp: "Liberada",
            tipoSolp: "C/Doc. Pliego",
            estadoDocCodigo: "Finalizada",
            estadoSolpCodigo: "Liberado",
            esSap: "",
            vincularPliego: "" 
        },
        {
            numeroSolp: "372872",
            fechaCreacion: "17/12/2021",
            estadoDoc: "Incompleto",
            estadoSolp: "Finalizada",
            tipoSolp: "Sin Doc.",
            estadoDocCodigo: "Incompleto",
            estadoSolpCodigo: "Finalizada",
            esSap: "Si",
            vincularPliego: "" 
        }
        ];

    }

    // estadoDoc: 
                // Creada, { Finalizada }
                // Incompleta, { - }    
                // Finalizado { Creada, Liberada, Parcialmente Liberada, Relac. a pedido compra, Finalizada }


    // estadoSolp: 
                // Creada, 
                // Liberada, 
                // Parcialmente Liberada, 
                // Relac. a pedido compra, 
                // Finalizada


    // tipo: 
                // C/Doc. Pliego, 
                // C/doc. Req, 
                // Sin Doc, 
                // Emerg., 
                // Adicional, 
                // Generar pliego





}






