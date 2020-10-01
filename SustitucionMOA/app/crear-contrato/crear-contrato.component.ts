import { Component, OnInit, ViewChild } from '@angular/core';
import { CrearContratoService } from './crear-contrato.service';
import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { SessionDataService } from './../common/services/SessionDataService';
import { SecurityService } from './../common/services/SecurityService';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { Seccion } from './../common/models/Seccion';



@Component({
    selector: 'app-contrato',
    template: ``,
    providers: [CrearContratoService]
})
export class CrearContratoBaseComponent extends ListBaseComponent {

    constructor(protected service: CrearContratoService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    datosContrato: any = new Array();
    materiales: any = new Array();
    monedas: any = new Array();
    destinos: any = new Array();
    campanias: any = new Array();
    //boletos: any =  new Array();
    bolsasSelect: any = new Array();
    bolsasConfirma: any = new Array();
    bolsasFisico: any = new Array();
    bolsasCarta: any = new Array();
    condicionVendedor: any = new Array();
    


    checkPermisos() { this.securityService.tienePermisoRedirect("CREAR CONTRATOS"); }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([
            new Seccion('/crear-contrato/aprecio', 'crear-contrato', 'A Precio'),
            new Seccion('/crear-contrato/afijar', 'crear-contrato', 'A Fijar'),
            //new Seccion('/crear-contrato/fijaciones', 'crear-contrato', 'Fijaciones')
        ]);
        this.obteneDatosContrato();

    }

    obteneDatosContrato() {
        this.unsubscribe();
        this.subscription = this.service.obteneDatosContrato().subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    let obj = JSON.parse(result);
                    this.datosContrato = obj;
                    obj.Datos.material.forEach(element => {
                        let el = {
                            Id: element.MaterialId,
                            Descripcion: element.Descripcion
                        }
                        this.materiales.push(el);
                    });
                    obj.Datos.Bolsa.forEach(element => {
                        let el = {
                            Id: element.Id,
                            Descripcion: element.Descripcion
                        }
                        this.bolsasConfirma.push(el);
                        if (el.Descripcion == "Bs As" || el.Descripcion == "Rosario")
                            this.bolsasFisico.push(el);
                        if (element.Descripcion == "Bs As")
                            this.bolsasCarta.push(el);
                    });
                    obj.Datos.Clasificacion.forEach(element => {
                        let el = {
                            Id: element.Id,
                            Descripcion: element.Descripcion
                        }
                        this.condicionVendedor.push(el);
                    });
                    obj.Datos.Destino.forEach(element => {
                        let el = {
                            Id: element.Id,
                            Descripcion: element.Descripcion
                        }
                        this.destinos.push(el);
                    });
                    obj.Datos.campania.forEach(element => {
                        let el = {
                            Id: element.CampaniaId,
                            Descripcion: element.Descripcion
                        }
                        this.campanias.push(el);
                    });
                    obj.Datos.moneda.forEach(element => {
                        let el = {
                            Id: element.MonedaId,
                            Descripcion: element.Descripcion
                        }
                        this.monedas.push(el);
                    });
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    }

    

    //isVisible(): boolean {
    //    if (this.data && this.data.contratosInfo.length != 0)
    //        return true;
    //    else
    //        return false;
    //}

    
}