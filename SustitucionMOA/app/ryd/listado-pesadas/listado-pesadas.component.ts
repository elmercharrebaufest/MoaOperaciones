import { Component, OnInit, ViewChild } from '@angular/core';
import { RYDService, RYDListadoPesadasService } from './../ryd.service';
import { RYDBaseComponent } from './../ryd.component';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { DropdownComponent, DropdownOption } from './../../common/view-child/dropdown/dropdown.component';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
declare var $: any;

@Component({
    selector: 'listado-pesadas',
    templateUrl: `./app/ryd/listado-pesadas/listado-pesadas.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: RYDService, useClass: RYDListadoPesadasService }]
})
export class ListadoPesadasComponent extends RYDBaseComponent {


    @ViewChild(MensajeComponent)
    private mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    private spinnerComponent: SpinnerComponent;

    @ViewChild(SpinnerSmallComponent)
    private spinnerSmallComponent: SpinnerSmallComponent;

    constructor(protected service: RYDListadoPesadasService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.spinnerSmallComponent = new SpinnerSmallComponent();

    }

    filtroCommodity = Array<any>();
    filtroExportador = Array<any>();
    commoditySelected: string = "";
    exportadorSelected: string = "";
    fechaInicio: string = "";
    fechaFin: string = "";
    data : any;
    tituloArchivo = "ReporteListadoPesadas.xls";

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR LISTADO PESADAS"); }

    ngAfterViewInit(): void {
        $('.form_datetime').datetimepicker({
            format: 'dd/mm/yyyy',
            language: 'es',
            weekStart: 1,
            todayBtn: 1,
            autoclose: 1,
            todayHighlight: 1,
            startView: 2,
            forceParse: 0,
            showMeridian: 1,
            pickTime: false,
            minView: 2,
            maxView: 4
        });
    }

    setTabs() {
        this.setMenuSeccionTab("ryd", "Listado de Pesadas");
    }

    ngOnInit() {
        super.ngOnInit();
        this.getFiltros();
    }

    getFiltros() {
        this.mensajeComponent.setMsgsEmpty();
        this.subscriptionDropDowns = this.service.getFiltros().subscribe(
            result => {
                //this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                    this.filtroCommodity = undefined;
                    this.filtroExportador = undefined;
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                    this.filtroCommodity = undefined;
                    this.filtroExportador = undefined;
                } else {
                    this.filtroCommodity = result.data.commodities;
                    this.filtroExportador = result.data.exportadores;
                }
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
                this.filtroCommodity = undefined;
                this.filtroExportador = undefined;
            }
        );
    }

    setFiltroCommodity(commodity: string) {
        this.commoditySelected = commodity;
    }

    setFiltroExportador(exportador: string) {
        this.exportadorSelected = exportador;
    }

    getDataEvent(fecha_inicio: string, fecha_fin: string) {
        this.fechaInicio = this.parseFecha(fecha_inicio);
        this.fechaFin = this.parseFecha(fecha_fin);
        this.getData();
        return false;
    }

    getData() {
        this.mensajeComponent.setMsgsEmpty();
        this.data = undefined;
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getListado(this.commoditySelected, this.exportadorSelected, this.fechaInicio, this.fechaFin).subscribe(
            result => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.data = result.data;
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    }

    exportExcel() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcelListadoPesada(this.commoditySelected, this.exportadorSelected, this.fechaInicio, this.fechaFin).subscribe(
            result => {
                this.spinnerSmallComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    var blob = new Blob([result], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, this.tituloArchivo);
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = this.tituloArchivo;
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                        return false;
                    }
                }
            },
            error => {
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    }

    parseFecha(fechaRaw: string) {
        if (fechaRaw != undefined) {
            var fecha, hora;
            var fechaRawArray = fechaRaw.split(" ");
            if (fechaRawArray.length > 1) {
                var fechaArray = fechaRawArray[0].split("-");
                if (fechaArray.length > 2) {
                    fecha = fechaArray[2] + "/" + fechaArray[1] + "/" + fechaArray[0];
                } else {
                    return fechaRaw;
                }
                return fecha;
            }
            
        }
        return fechaRaw;
    }

    isVisible() {
        return this.data != undefined && this.data.pesadas.length != 0;
    }
}