import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { UsuarioService } from '../../../usuario/usuario.service';
import { ComprasService } from '../../compras.service';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { Table } from 'primeng/table';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { PeticionDeOfertaDto, PeticionDeOfertaSolpPosicionDto, PeticionDeOfertaUsarioDto } from '../../../modelos/peticion-de-oferta-model';
import { Solp } from '../../solp/solp';
import { CotizacionHoraDto, CotizacionDto } from '../../../modelos/cotizacionDto';
import { AdjudicacionDto } from '../../../modelos/adjudicacion';

@Component({
    selector: 'app-ver-ofertas',
    templateUrl: './ver-ofertas.component.html',
    styleUrls: [
        './ver-ofertas.component.css']
})
export class VerOfertasComponent extends ListBaseComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;

    @ViewChild("tabla")
    protected tabla: Table;

    @Input()
    public peticion: PeticionDeOfertaDto;

    peticionOferta: PeticionDeOfertaDto;
    SolpDto: Solp;
    tablaOfertas: PeticionDeOfertaDto;
    adjudicacion: AdjudicacionDto;
    Cotizacion: CotizacionDto;


    TodasPosicionesSeleccionadas: boolean = false;
    displayAdjudicacionCreada: boolean;
    errores: any = [];
    displayVisualizarErrores: boolean;
    numeroOrdenDeCompra: any;

    displayPanelHs: boolean = false;

    @Input()
    public peticionHs: CotizacionHoraDto;
    error: string;


    constructor(protected service: ComprasService, protected usuarioService: UsuarioService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }




    ngOnInit() {
        if (this.route.params) {
            this.route.params.forEach((params: Params) => {
                let peticionOferta_Id = parseInt(params["id"]);
                this.verOfertas(peticionOferta_Id);
            })
        };

        if (this.tablaOfertas == null) {
            this.tablaOfertas = {
                Id: null,
                FechaEntregaFormateado: null,
                PlazoDeOferta: null,
                CUIT: null,
                Mail: null,
                Usuarios: new Array(),
                SolpDto: null,
                Selected: null,
                RespetaMateriales: null,
                RespetaServicios: null,
                Solp_Id: null,
                NroSolp: null,
                FechaCreacion: null,
                UsuarioCreador_Id: null,
                Observaciones: null,
                TipoPosicionCodigo: null,
                CotizacionId: null,
                PeticionDeOfertaPosicion: new Array(),
                Cotizacion: null,
                ObservacionTecnica: null,
                ObservacionEconomica: null,
                Cantidad: null,
            };
        }
        if (this.adjudicacion == null || this.adjudicacion == undefined) {
            this.adjudicacion = {
                Id: null
            };
        }
    }

    seleccionarTodo() {
        if (this.TodasPosicionesSeleccionadas) {
            this.tablaOfertas.PeticionDeOfertaPosicion.map(pos => pos.Selected = true);
        } else {
            this.tablaOfertas.PeticionDeOfertaPosicion.map(pos => pos.Selected = false);
        }
    }



    verOfertas(peticionOferta_Id) {
        try {
            this.blockUI.start('Cargando...');
            this.subscription = this.service.getListarOfertasComprador(peticionOferta_Id).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.tablaOfertas = result.data;
                    }
                    this.blockUI.stop();
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.blockUI.stop();
                });
        } catch (e) {
            this.blockUI.stop();
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    descargarAdjuntosCotizacion(cotizacionId) {
        this.blockUI.start("Descargando...");
        this.service.DescargarAdjuntosCotizacion(cotizacionId)
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
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            )
    }

    validacionCantidad(cantidad: number, cantidadPendiente: number) {
        if (cantidad > cantidadPendiente) {
            this.floatMsgService.setErrorMsg("La cantidad ingresada debe ser menor a " + cantidadPendiente);
        }
    }

    esUltimaPosicion(posicion: PeticionDeOfertaSolpPosicionDto) {
        let ultimoElemento = this.tablaOfertas.PeticionDeOfertaPosicion[this.tablaOfertas.PeticionDeOfertaPosicion.length - 1];
        return ultimoElemento.Id == posicion.Id;
    }

    verPosicion(peticionPosicion: PeticionDeOfertaSolpPosicionDto) {
        peticionPosicion.expanded = peticionPosicion.expanded == true ? false : true;
    }

    mostrarPanelHs() {
        this.displayPanelHs = true;
    }

    onCerrarPanel() {
        this.displayPanelHs = false;
    }


    crearAdjudicacion(usuario: PeticionDeOfertaUsarioDto) {
        var lista = []
        if (this.tablaOfertas.PeticionDeOfertaPosicion.filter(x => x.Selected).length > 0) {
            this.tablaOfertas.PeticionDeOfertaPosicion.forEach((peticion) => {
                if (peticion.Selected && !peticion.Posicion.AdjudicacionCompleta) { 
                    usuario.Cotizacion.CotizacionPosiciones.forEach((cotizacionPos) => {
                        if (peticion.Id == cotizacionPos.PeticionDeOfertaSolpPosicion_Id)
                            lista.push({
                                Posicion: peticion.Posicion.Indice,
                                CotizacionPosicion_Id: cotizacionPos.Id,
                                Cantidad: this.tablaOfertas.TipoPosicionCodigo == 'MATERIALES' ?
                                 peticion.Posicion.CantidadAdjudicacion : 1,
                                SolpPosicion_Id: peticion.Posicion.Id,
                                CantidadCotizada: cotizacionPos.Cantidad,
                                CantidadSolp: peticion.Posicion.CantidadPendiente,
                                CantidadAdjudicada: this.tablaOfertas.TipoPosicionCodigo == 'MATERIALES' ?
                                 peticion.Posicion.CantidadAdjudicada : 1,
                                NoDisponible: cotizacionPos.NoDisponible
                            })
                    })
                } else {
                    peticion.Selected = false;
                }

            });

            if (lista.length > 0) {
                this.error = this.validarAdjudicacion(lista);
                if (this.error != "") {
                    this.floatMsgService.setErrorMsg(this.error)
                    return;
                }
            } else {
                this.floatMsgService.setInfoMsg("Debe seleccionar alguna posicion valida para adjudicar");
                return;
            }
            this.adjudicacion.AdjudicacionPosiciones = lista;
            this.adjudicacion.Cotizacion_Id = usuario.Cotizacion.Id;
            this.adjudicacion.Solp_Id = this.tablaOfertas.Solp_Id;
            this.confirmacionAdjudicar();
        } else {
            this.floatMsgService.setInfoMsg("Debe seleccionar alguna posicion para adjudicar");
        }
    }

    validarAdjudicacion(lista): string {
        var self = this;
        var breakFor = false;
        this.error = "";
        lista.forEach(element => {
            if (!breakFor) {
                if (this.tablaOfertas.TipoPosicionCodigo == 'MATERIALES') {
                    if (element.Cantidad == null || element.Cantidad == undefined || element.Cantidad <= 0) {
                        self.error = "Pos " + element.Posicion + " - La cantidad adjudicada debe ser mayor a 0";
                        breakFor = true;
                        return self.error;
                    }
                }

                if (this.tablaOfertas.TipoPosicionCodigo == 'MATERIALES') {
                    if (element.Cantidad > element.CantidadSolp) {
                        self.error = "Pos " + element.Posicion + " - La cantidad adjudicada no debe ser mayor que la cantidad pendiente";
                        breakFor = true;
                        return self.error;
                    }
                }

                if(element.NoDisponible == true){
                    self.error = "Pos " + element.Posicion + " - No se puede adjudicar una posicion no disponible";
                        breakFor = true;
                        return self.error;
                }
            }

        });
        return this.error;
    }

    confirmacionAdjudicar() {
        this.confirmationService.confirm({
            header: "¡Ultimo Paso!",
            acceptLabel: "SI, CONFIRMAR",
            rejectLabel: "VOLVER",
            message: 'Está a punto de enviar la adjudicacion <b>¿Desea continuar?</b>',
            accept: () => {
                this.guardarAdjudicacion()
            },
            reject: () => {
            }
        });
    }

    guardarAdjudicacion() {
        this.blockUI.start("Grabando...");
        try {
            this.subscription = this.service.GrabarAdjudicacion(this.adjudicacion).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    }
                    else if (result.Errores != undefined && result.Errores != null && result.Errores.length > 0) {
                        this.errores = result.Errores;
                        this.displayVisualizarErrores = true;
                    }
                    else {
                        this.numeroOrdenDeCompra = result.NumeroPedido;
                        this.displayAdjudicacionCreada = true;
                        
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

    salir() {
        this.displayAdjudicacionCreada = false;
        this.verOfertas(this.tablaOfertas.Id.toString())
    }

    salirVisualizarErrores() {
        this.displayVisualizarErrores = false;
    }
}
