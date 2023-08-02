import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { SolpCompraDto } from '../../../solp-compra';
import { ComprasService } from '../../../compras.service';
import { UsuarioService } from '../../../../usuario/usuario.service';
import { NavService } from '../../../../common/services/NavService';
import { SessionDataService } from '../../../../common/services/SessionDataService';
import { ModalService } from '../../../../common/services/ModalService';
import { FormBuilder } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { FloatMsgService } from '../../../../common/services/FloatMsgService';
import { SecurityService } from '../../../../common/services/SecurityService';
import { ActivatedRoute, Router } from '@angular/router';
import { ListBaseComponent } from '../../../../common/base-components/list-base-component';
import { RegistroInfoDto } from '../../../../modelos/registro-info';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

@Component({
    selector: 'registro-info',
    templateUrl: './registro-info.component.html',
    styleUrls: ['./registro-info.component.css'],

})

export class RegistroInfoComponent extends ListBaseComponent implements OnInit, OnChanges {

    @Input() solpCompraDto: SolpCompraDto;
    @Input() displayRegistroInfo: SolpCompraDto;

    @Output() cancelarRegistroEmitter = new EventEmitter();
    @BlockUI() blockUI: NgBlockUI;
    options: any[] = new Array()

    registrosInfo: RegistroInfoDto[] = new Array()
    displayConfirmacion: boolean;
    resultado: any;
    descripcion: {};
    registros: RegistroInfoDto[];
    displayAdjudicacionCreada: boolean;
    resultadoAdjudicacion: any = new Array();


    constructor(protected service: ComprasService, protected usuarioService: UsuarioService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

    }
    ngOnChanges(changes: SimpleChanges): void {
        this.inicializarDatos();
    }

    ngOnInit() {
        this.inicializarDatos();
    }

    inicializarDatos() {
        if(this.solpCompraDto.RegistrosInfo && this.solpCompraDto.RegistrosInfo != undefined){
            this.registrosInfo = this.solpCompraDto.RegistrosInfo;
            if (this.solpCompraDto != undefined && this.solpCompraDto.PosicionCompras != null) {
                this.options = this.solpCompraDto.PosicionCompras.map(x => ({
                    label: x.Indice + " - " + x.Tarea,
                    value: x.Id
                }));
            }
            this.registrosInfo = this.solpCompraDto.RegistrosInfo.filter(x => x.Indice == this.registrosInfo[0].Indice);
        }
      
    }
    onCancelarRegistroInfo() {
        this.cancelarRegistroEmitter.next();
    }


    onHideRegistroDialog(dd) {
        this.cancelarRegistroEmitter.next();

    }

    filtrarPorPosicion(dd) {
        this.registrosInfo = this.solpCompraDto.RegistrosInfo.filter(x => x.PosicionId == dd.value.value);
    }

    abrirModalConfirmacion() {
        this.registros = this.solpCompraDto.RegistrosInfo.filter(x => x.Confirmado == true);
        this.resultado = [];
        this.descripcion = {}

        if (this.registros.length == 0) {
            this.confirmationService.confirm({
                header: "Falta seleccionar registros",
                key:"avisoV",
                message: 'Debe seleccionar algun registro info para poder continuar.',
                accept: () => {
                    return;
                },                
            });
            return;
        }

        if (this.registros.some(item => item.CantidadAdjudicacion <= 0)) {
            this.confirmationService.confirm({
                header: "Error",
                key: "avisoV",
                message: 'Debe ingresar una cantidad para los registros seleccionados',
                accept: () => {
                    return;
                },
            });
            return;
        }

       var totalAdjudicada = this.registros.reduce((sum, registro) => Number(sum + Number(registro.CantidadAdjudicacion)), 0);
        console.log(totalAdjudicada, "total adjudicado");
        if (this.registros.some(item => item.Cantidad < totalAdjudicada)) {
            this.confirmationService.confirm({
                header: "Cantidad Incorrecta",
                key: "avisoV",
                message: 'La cantidad a adjudicar no debe superar la cantidad pendiente.',
                accept: () => {
                    return;
                },
            });
            return;
        }



        this.registros.forEach(x => {
            if (!this.descripcion.hasOwnProperty(x.Codigo)) {
                this.descripcion[x.Codigo] = {
                    detalle: [],
                    proveedor: x.NombreProveedor + " - " + x.Cuit
                }
            }

            this.descripcion[x.Codigo].detalle.push({
                nombre: x.NombreProveedor + " - " + x.Cuit,
                descripcion: "Posicion: " + x.Indice + " Tarea: " + x.DescripcionPosicion + " Cantidad a Adjudicar: " + x.CantidadAdjudicacion + " Moneda: " + x.Moneda + " Unidad: " + x.Unidad + " Precio: " + x.Precio,
                Posicion: x.Indice,
                Tarea: x.DescripcionPosicion,
                CantidadAdjudicar: x.CantidadAdjudicacion,
                Moneda: x.Moneda,
                Unidad: x.Unidad,
                Precio: x.Precio
            })
        })

        this.resultado = this.descripcion;
        this.displayConfirmacion = true;

    }

    cerrarModalConfirmacion() {
        this.displayConfirmacion = false;
    }
    cerrarModalConfirmacionAdjudicacion() {
        this.onCancelarRegistroInfo() 
        this.displayAdjudicacionCreada = false;
        this.displayConfirmacion = false;
    }

    cerrarModalHomeAdjudicacion(){
        this.navService.navegarSeccion('/compras/dashboardComprador');
    }

    guardarAdjudicacion() {
        this.blockUI.start("Grabando...");
        try {
            this.resultadoAdjudicacion = new Array();       
            this.subscription = this.service.guardarAdjudicacionAutomatica(this.solpCompraDto.RegistrosInfo.filter(x => x.Confirmado == true)).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.confirmationService.confirm({
                            header: "Error",
                            key: "avisoV",
                            message: result.error,
                            accept: () => {
                                this.blockUI.stop();
                            },
                        });
                    } else if (result.info != undefined) {
                        this.confirmationService.confirm({
                            header: "Error",
                            key: "avisoV",
                            message: result.info,
                            accept: () => {
                                this.blockUI.stop();
                            },
                        });
                    }
                    else {
                        this.displayAdjudicacionCreada = true;
                        this.resultadoAdjudicacion = result.data;
                    }
                    this.blockUI.stop();
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.blockUI.stop();

                });
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh


    }
}
