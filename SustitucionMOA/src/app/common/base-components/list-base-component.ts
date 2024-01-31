import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseService } from './../services/BaseService';
import { BaseComponent } from './base-component';
import { FiltroFechaComponent } from './../view-child/filtro-fecha/filtro-fecha.component';
import { MensajeComponent } from './../view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../view-child/spinner-small/spinner-small.component';
import { DropdownComponent, DropdownOption } from './../view-child/dropdown/dropdown.component';
import { SessionDataService } from './../services/SessionDataService';
import { NavService } from './../services/NavService';
import { SecurityService } from './../services/SecurityService';
import { FloatMsgService } from './../services/FloatMsgService';
import { ModalService } from './../services/ModalService';



@Component({
    selector: 'app-list-base',
    template: ``,
    providers: [BaseService]
})
export class ListBaseComponent extends BaseComponent implements OnInit {

    @ViewChild(FiltroFechaComponent)
    protected filtroFechaComponent: FiltroFechaComponent;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(DropdownComponent)
    protected itemsPerPageComponent: DropdownComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    constructor(protected service: BaseService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securytiService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securytiService, floatMsgService, modalService);
        this.filtroFechaComponent = new FiltroFechaComponent();
        this.mensajeComponent = new MensajeComponent();
        this.itemsPerPageComponent = new DropdownComponent();
        this.spinnerComponent = new SpinnerComponent();
        this.spinnerSmallComponent = new SpinnerSmallComponent();

    }

    tituloArchivo = "";
    itemsPerPage = sessionStorage.getItem("itemsPerPage") ? sessionStorage.getItem("itemsPerPage") : "10";
    proveedor = sessionStorage.getItem("proveedor")
    tipoDropdown: string = 'numberItems';
    orderedByColumn: string = "fechaDescargaDate";
    orderDirection: number = 1;
    data: any = null;

    checkPermisos() { }

    ngOnInit() {
        this.checkPermisos();
        this.getData();
    }

    setItemsPerPage(numberOfItems: string) {
        this.itemsPerPage = numberOfItems;
    }

    orderColumnBy(column: string) {
        if (column == this.orderedByColumn) {
            this.orderDirection = -this.orderDirection;
        } else {
            this.orderDirection = 1;
            this.orderedByColumn = column;
        }
    }

    getData() {
        this.data = null;
        this.vaciarFiltros();
        if(this.mensajeComponent != undefined) {
            this.mensajeComponent.setMsgsEmpty();
        }

        if(this.spinnerComponent != undefined) {
            this.spinnerComponent.showIt();        
        }
        
        this.unsubscribe();
        if(this.filtroFechaComponent != undefined){
            this.subscription = this.service.getData(this.filtroFechaComponent.periodo, this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin).subscribe(
                (result:any) => {
                    this.data = null;
                    this.mensajeComponent.setMsgsEmpty();
                    
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.data = result.data;
                        if(result.data==null && result.comprobantes && result.comprobantes.comprobantes){
                            this.data = result.comprobantes;
    
                        }
                        this.cargarFiltrosVariables(result);
                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        }
      
        return false;
    }

    exportExcel() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcel(this.filtroFechaComponent.periodo, this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin).subscribe(
            (result:any) => {
                this.spinnerSmallComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                }else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    //var blob = new Blob([result], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                    var blob = new Blob([result], { type: 'application/octet-stream' });

                    //var uagent = navigator.userAgent.toLowerCase();
                    //if (/safari/.test(uagent) && !/chrome/.test(uagent)) {
                        //console.log("SAFARI");
                    //} else 
                    if (window.navigator.msSaveOrOpenBlob) {
                        //IE11
                        window.navigator.msSaveOrOpenBlob(blob, this.tituloArchivo);
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = this.tituloArchivo;
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                        //return false;
                    }

                }
            },
            error => {
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
        return false;  // <- Prevent href del a
    }

    protected vaciarFiltros() { }

    protected cargarFiltrosVariables(result: any) { }
}