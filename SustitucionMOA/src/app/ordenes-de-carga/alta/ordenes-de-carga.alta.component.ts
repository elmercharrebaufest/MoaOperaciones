import { DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { AltaEmpresaService } from '../../alta-proveedores/altas/altas.service';
import { EmpresaGranosService } from '../../alta-proveedores/empresa-granos/empresa-granos.service';
import { BaseComponent } from '../../common/base-components/base-component';
import { Material } from '../../common/models/material';
import { OrdenDeCarga } from '../../common/models/ordenes-de-carga/ordenDeCarga';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { UsuarioService } from '../../usuario/usuario.service';
import { OrdenesDeCargaService } from '../ordenes-de-carga.service';

declare var $: any;

@Component({
    selector: 'app-ordenes-de-carga.alta',
    templateUrl: './ordenes-de-carga.alta.component.html',
    styleUrls: ['./ordenes-de-carga.alta.component.css']
})

export class OrdenesDeCargaAlta extends BaseComponent implements OnInit {
    
    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    ordenDeCargaId: number = 0;

    ordenDeCarga: OrdenDeCarga = new OrdenDeCarga();
    mensajeError: string = "";
    mensajeSuccess: string = "";
    private selectUndefinedOptionValue: any;

    listaMateriales: Material[];
    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";
    esComercial: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA COMERCIALES');
    puedeEditarContrato: boolean = false;

    constructor(protected service: OrdenesDeCargaService,
        protected usuarioService: UsuarioService, protected navService: NavService,
        private route: ActivatedRoute,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected empresaGranosService: EmpresaGranosService,
        public datepipe: DatePipe) {
        super(navService, securytiService, floatMsgService, modalService);
    }

    ngOnInit() {
        this.ordenDeCarga.Cantidad = 30000;

        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) this.ordenDeCargaId = params["id"];
        });

        this.navService.setSeccionList([]);

        if (this.ordenDeCargaId > 0) {
            this.obtenerOrdenDeCarga();
        }

        this.obtenerMateriales();

        if(this.isAuthorized('VER ORDENES DE CARGA DE TERCEROS')){
            this.ordenDeCarga.CUITCliente = 0;
        }
    }

    obtenerMateriales() {
        //Sacamos lo de la lista de campaña, ya que ahora son independientes
        this.subscription = this.service.getMateriales().subscribe(
            (result) => {
                this.listaMateriales = result.data;
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    validar() {
        //REVISAR MAÑANA, POR EL MOMENTO PUEDEN CARGAR
        if(this.ordenDeCarga.CUITCliente != 0){
            if (this.ordenDeCarga.CUITCliente.toString().trim().length != 11) {
                this.mensajeComponent.setInfoMsg("Ingrese un CUIT de cliente válido.");
                return false;
            }
        }

        if (this.ordenDeCarga.NombreChofer.trim().length < 2) {
            this.mensajeComponent.setInfoMsg("Ingrese el nombre del chofer.");
            return false;
        }

        if (this.ordenDeCarga.ApellidoChofer.trim().length < 2) {
            this.mensajeComponent.setInfoMsg("Ingrese el apellido del chofer.");
            return false;
        }

        /*VER ESTA VALIDACION, ACA VALIDA COMO SI FUERA UN CUIT PERO EN EL FRONT DICE QUE PONGA EL DNI/CUIL*/
        if (this.ordenDeCarga.CUITChofer.toString().trim().length != 11) {
            this.mensajeComponent.setInfoMsg("Ingrese un CUIT de chofer válido.");
            return false;
        }

        if (this.ordenDeCarga.PatenteAcoplado.trim().length < 6) {
            this.mensajeComponent.setInfoMsg("Ingrese una patente válida.");
            return false;
        }

        if (this.ordenDeCarga.ChasisAcoplado.trim().length < 6) {
            this.mensajeComponent.setInfoMsg("Ingrese un número de chasis válido.");
            return false;
        }

        if (this.ordenDeCarga.RazonSocialTransporte.trim().length < 2) {
            this.mensajeComponent.setInfoMsg("Ingrese la razón social del transporte.");
            return false;
        }

        if (this.ordenDeCarga.CUITTransporte.toString().trim().length != 11) {
            this.mensajeComponent.setInfoMsg("Ingrese un CUIT de transporte válido.");
            return false;
        }

        return true;
    }

    cargaFalsa() {
        this.ordenDeCarga.llenar()
    }

    obtenerOrdenDeCarga() {
        try {
            this.subscriptionDropDowns = this.service.getEditarOrdenDeCarga(this.ordenDeCargaId).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.ordenDeCarga = result.data;

                        debugger
                        if(this.esComercial && result.data.ColorSemaforo != "green"){
                            this.puedeEditarContrato = true;
                        }
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }


    submit() {
        if (!this.validar()) {
            this.spinnerComponent.hideIt();
            return;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();

        if (this.ordenDeCargaId > 0) {
            this.subscription = this.service
                .editar(this.ordenDeCarga)
                .subscribe(
                    (result) => {
                        this.spinnerComponent.hideIt();
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (
                            result.error != undefined &&
                            result.error != ""
                        ) {
                            this.mensajeComponent.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.mensajeComponent.setInfoMsg(result.info);
                        } else {
                            this.mensajeComponent.setMsgsEmpty();

                            this.mensajeSuccess = result.data.Mensaje;

                            this.ordenDeCargaId = result.data.IdEntidad;

                            document
                                .getElementById("openModalNotificacion")
                                .click();
                        }
                    },
                    (error) => {
                        this.spinnerComponent.hideIt();
                        this.mensajeComponent.setErrorMsg(error.message);
                    }
                );

        }
        else {
            this.subscription = this.service
                .agregar(this.ordenDeCarga)
                .subscribe(
                    (result) => {
                        this.spinnerComponent.hideIt();
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (
                            result.error != undefined &&
                            result.error != ""
                        ) {
                            this.mensajeComponent.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.mensajeComponent.setInfoMsg(result.info);
                        } else {
                            this.mensajeComponent.setMsgsEmpty();

                            this.mensajeSuccess = result.data.Mensaje;

                            this.ordenDeCargaId = result.data.IdEntidad;

                            document
                                .getElementById("openModalNotificacion")
                                .click();
                        }
                    },
                    (error) => {
                        this.spinnerComponent.hideIt();
                        this.mensajeComponent.setErrorMsg(error.message);
                    }
                );
        }
    }

    aceptar() {
        document
            .getElementById("botonCerrarModal")
            .click();

        this.redirigirADetalles();
    }

    redirigirADetalles() {
        this.goToSeccionParam('/ordenes-de-carga/detalle/', this.ordenDeCargaId.toString())
    }

    redirigirAListado() {
        this.navService.navegarSeccion(
            "/ordenes-de-carga"
        );
    }
}
