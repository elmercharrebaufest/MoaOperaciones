import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbAlert } from '@ng-bootstrap/ng-bootstrap';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ConfirmationService } from 'primeng/api';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { PeticionDeOfertaDto, PeticionDeOfertaUsarioDto } from '../../../modelos/peticion-de-oferta-model';
import { ComprasService } from '../../compras.service';

@Component({
    selector: 'app-proveedor-peticion',
    templateUrl: './proveedor-peticion.component.html',
    styleUrls: ['./proveedor-peticion.component.css']
})
export class ProveedorPeticionComponent implements OnInit {


    @Input()
    displayProveedor: boolean;

    @Input()
    public peticion: PeticionDeOfertaDto;
    @Output() cerrarModalProveedorEmitter = new EventEmitter();
    selectedProv: number[] = []
    nroPeticion: any;
    displayOkProveedor: boolean;
    subscription: any;
    @BlockUI() blockUI: NgBlockUI;
    error: string = "";
    visualizarAlert = false;
    proveedores: any[] = new Array();
    proveedoresSeleccionados: any[] = new Array();
    proveedorSeleccionado: any;
    proveedor: PeticionDeOfertaUsarioDto;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    }

    ngOnInit() {

    }

    onCerrarModalProveedor() {
        this.iniciarModalProveedor();
        this.cerrarModalProveedorEmitter.next();
    }

    iniciarModalProveedor() {
        if (this.proveedoresSeleccionados != null && this.proveedoresSeleccionados.length > 0) {
            this.proveedoresSeleccionados.splice(0, this.proveedoresSeleccionados.length);
        }
        this.proveedorSeleccionado = "";
    }

    grabarProveedor() {

        this.validarProveedor(null);
        if (!this.visualizarAlert) {
            this.blockUI.start("Grabando...");
            try {

                this.subscription = this.service.GrabarProveedorEnPeticion(this.proveedoresSeleccionados.map(a => a.Id), this.peticion.Id).subscribe(
                    (result: any) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            //   this.floatMsgService.setErrorMsg(result.error);
                            this.error = result.error;
                            this.visualizarAlert = true;

                        } else if (result.info != undefined) {
                            // this.floatMsgService.setInfoMsg(result.info);
                            this.error = result.error;
                            this.visualizarAlert = true;
                        } else {
                            this.nroPeticion = result.data.IdEntidad;
                            this.displayOkProveedor = true;
                        }
                        this.blockUI.stop();
                    },
                    error => {
                        // this.floatMsgService.setErrorMsg(error.message);
                        this.error = error.message;
                        this.visualizarAlert = true;
                        this.blockUI.stop();

                    });
            } catch (e) {
                //  this.floatMsgService.setErrorMsg(e);
                this.error = e;
                this.visualizarAlert = true;
                this.blockUI.stop();
                return false; //<-- Prevent Refresh
            }
            return false; //<-- Prevent Refresh
        }
    }


    salir() {
        this.onCerrarModalProveedor();
        this.displayOkProveedor = false;
    }

    validarProveedor(event) {
        this.visualizarAlert = false;
        // if (this.proveedoresSeleccionados == null || this.proveedoresSeleccionados.length == 0) {
        //     this.error = "Debe seleccionar un proveedor que no este asociado.";
        //     return this.visualizarAlert = true;
        // }
        if(event != null && this.proveedoresSeleccionados.some(e => e.Id === event.Id)){
            this.error = "Debe seleccionar un proveedor que no este asociado.";
            this.visualizarAlert = true;
        }

        if(!this.visualizarAlert && event != null && this.peticion != null && this.peticion.Usuarios.some(e => e.UsuarioId === event.Id)){
            this.error = "Debe seleccionar un proveedor que no este asociado.";
            this.visualizarAlert = true;
        }

        if (!this.visualizarAlert && (this.proveedoresSeleccionados != null || this.proveedoresSeleccionados.length > 0) && this.peticion != null) {
            let proveedoresSelec = this.proveedoresSeleccionados;
            for (let index = 0; index < this.proveedoresSeleccionados.length; index++) {
                this.proveedor = this.peticion.Usuarios.filter(x => x.Id == proveedoresSelec[index].Id)[0];
                if (this.proveedor != null && this.proveedor != undefined) {
                    this.error = "El proveedor " + this.proveedor.RazonSocial + " (" + this.proveedor.CUIT + ") ya se encuentra incluido en la PO";
                    return this.visualizarAlert = true;
                }
            }
        }
        return this.visualizarAlert;
    }
    searchProveedor(event) {
        try {
            this.subscription = this.service.listarProveedores(event.query).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.proveedores = result.data;
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                });
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    selectProveedor(event) {
        try {          
                this.validarProveedor(event);
                if (!this.visualizarAlert) {
                    this.proveedoresSeleccionados.push(event);
                }
          
            this.proveedorSeleccionado = null;
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
        }
    }

    eliminarProveedor(event, proveedor) {
        this.proveedoresSeleccionados = this.proveedoresSeleccionados.filter(x => x.Id !== proveedor.Id);
        this.validarProveedor(null);
    }

}
