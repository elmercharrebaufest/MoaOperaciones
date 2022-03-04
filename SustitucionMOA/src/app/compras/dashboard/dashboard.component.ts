import { Component, Input, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, RouteReuseStrategy } from '@angular/router';
import { ListBaseComponent } from './../../common/base-components/list-base-component'
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { ComprasService } from '../compras.service';
import { ConfirmationService, SelectItem } from 'primeng/api';
import { Solp } from '../Solp';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { CrearContratoModule } from '../../crear-contrato/crear-contrato.module';
import { formatDate } from '@angular/common';
import { SortEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { EnumTipoSolpSap } from '../enum-tipo-solp-sap';
import { NullAstVisitor } from '@angular/compiler';
import { zip } from 'rxjs';



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

    @BlockUI() blockUI: NgBlockUI;
    
    @Input('model') 
    protected model:Solp;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService) {
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


   
    desdeDashboard: Date;
    hastaDashboard: Date;
    estadoSolpItem: SelectItem[];
    selectEstadoSolp: string[] = [];
    buscarDashboard: string;
    fechaSolp: any;
    hoy: Date = new Date();
    es: any;
    display: boolean = false;
    tablaSolp: any[];
    tablaSolpCopy: any[];
    cols: any[];
    serviciosDashboard: any = "Servicios"
    solp: Solp = new Solp();

    checkedFilterSap = false;
    checkedFilterMantenimiento = false;

    showDialog() {
        this.display = true;
    }

    cards = [
        { nombre: "Con documento de pliego", path: "/compras/solp/0", tipoSolp: "CON_PLIEGO" },
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

    goToSeccionParam(path: string, param: any) {
        $("#mySidenav").css({ 'right': '-270px' });
        $("#myMenuClose").css({ 'display': 'none' });
        $("#myMenuOpen").css({ 'display': 'block' });
        $("#coverAll").fadeOut();
        this.navService.navegarSeccionParam(path, param);
        return false;
    }


    ngOnInit() {
        this.navService.setSeccionList([]);

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

    getStatusDocumentoSolp(data: any): String {
        return data.Posiciones.every(x => x.Estado == false) && data.NroSolp != null ? 'Borrardo en sap' : data.EstadoDocumento.Descripcion;
    }

    public getColorDocumentoSolp(data: any): String {
        return data.Posiciones.every(x => x.Estado == false) && data.NroSolp != null ? '#DD441E' : data.EstadoDocumento.Color;
    }

    getListarSolp(){
            try {
                this.spinnerComponent.showIt();

                this.subscription = this.service.getListarSolp().subscribe(
                    (result:any) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.floatMsgService.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.floatMsgService.setInfoMsg(result.info);
                        } else { 
                            this.tablaSolp = result.data;
                            this.tablaSolp.forEach(x => {
                                x.FechaCreacion = new Date(this.getDateFromAspNetFormat(x.FechaCreacion));
                                x.VincularPliego = x.TipoSolpSap == EnumTipoSolpSap.Mantenimiento || x.TipoSolpSap == EnumTipoSolpSap.SAP;
                                x.PliegoVinculado = (x.TipoSolpSap == EnumTipoSolpSap.Mantenimiento || x.TipoSolpSap == EnumTipoSolpSap.SAP) &&
                                                    x.EstadoDocumento.Codigo == "CREADO";
                                    
                            });
                            this.tablaSolpCopy = result.data;
                            this.spinnerComponent.hideIt();
                        }
                    },
                    error => {
                        this.floatMsgService.setErrorMsg(error.message);
                        this.spinnerComponent.hideIt();
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
                (result:any) => {
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

   

    getCombos(){
        try {
            this.subscription = this.service.getCombos().subscribe(
                (result:any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else { 
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
    }

    filtrarPorFecha(){
        this.filtrarFecha(this.tabla, "FechaCreacion", this.desdeDashboard, this.hastaDashboard);   
    }

    filtrarPorSap(){
        this.checkedFilterSap = !this.checkedFilterSap;
        this.filtrarTablaPorTipoSolp();
    }

    filtrarPorMantenimiento(){
        this.checkedFilterMantenimiento = !this.checkedFilterMantenimiento;
        this.filtrarTablaPorTipoSolp();
    }

    filtrarTablaPorTipoSolp(){
        let tablaPrincipal = this.tablaSolpCopy;
        if(this.checkedFilterSap && this.checkedFilterMantenimiento) {
            tablaPrincipal = tablaPrincipal.filter(x => x.TipoSolpSap === EnumTipoSolpSap.SAP || x.TipoSolpSap === EnumTipoSolpSap.Mantenimiento);
        } else if(this.checkedFilterMantenimiento) {
            tablaPrincipal = tablaPrincipal.filter(x => x.TipoSolpSap === EnumTipoSolpSap.Mantenimiento);
        } else if(this.checkedFilterSap) {
            tablaPrincipal = tablaPrincipal.filter(x => x.TipoSolpSap === EnumTipoSolpSap.SAP);
        }
        this.tablaSolp = tablaPrincipal;
    }


    
    eliminarPosicionDashboard(idSolp) {
        this.confirmationService.confirm({
            message: '¿Está seguro que desea eliminar la SOLP?',
            accept: () => {
                this.borrarSolp(idSolp)
            },
            reject: () => {
            }
        });
    }

    generarZipPliego(idSolp){
        this.blockUI.start('Generando ')
        this.service.descargarZipPliego(idSolp)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "application/octet-stream",
                        });

                        if (window.navigator.msSaveOrOpenBlob) {
                            // IE11
                            window.navigator.msSaveOrOpenBlob(
                                blob,
                                result.FileDownloadName
                            );
                        } else {
                            var url = window.URL.createObjectURL(blob);
                            var link = document.createElement("a");
                            document.body.appendChild(link);
                            link.href = url;
                            link.download = result.FileDownloadName;
                            link.click();
                            setTimeout(function () {
                                window.URL.revokeObjectURL(url);
                            }, 0);
                            this.blockUI.stop();
                            return false;
                        }
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
    }

    descargarPdf(idSolp): void {
        if (idSolp != undefined) {
            this.service.getPdf(idSolp)
                .subscribe(
                    (result) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        }
                        else {
                            var byteArray = new Uint8Array(result.FileContents);
                            var blob = new Blob([byteArray], {
                                type: "application/octet-stream",
                            });

                            this.downloadArchivoLocal(blob, result.FileDownloadName);
                        }
                    },
                    (error) => {
                        this.spinnerSmallComponent.hideIt();
                        this.mensajeComponent.setErrorMsg(error.message);
                    }
                )
        }
    }

    vincularAPliego(solpId: number) {
        this.goToSeccionParam('/compras/solp', solpId);
    }

    private downloadArchivoLocal(blob: Blob, nombreArchivo: string): void {
        if (window.navigator.msSaveOrOpenBlob) {
            // IE11
            window.navigator.msSaveOrOpenBlob(
                blob,
                nombreArchivo
            );
        } else {
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

}