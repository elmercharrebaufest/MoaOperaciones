import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

import { ComprasService } from '../../compras.service';
import { UsuarioService } from '../../../usuario/usuario.service';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { SecurityService } from '../../../common/services/SecurityService';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { PeticionDeOfertaDto, PeticionDeOfertaSolpPosicionDto } from '../../../modelos/peticion-de-oferta-model';
import { CotizacionMaterialComponent } from './cotizacion-material/cotizacion-material.component';
import { GuardarCotizacion } from '../../../modelos/cotizacionDto';
import { CotizacionServicioComponent } from './cotizacion-servicio/cotizacion-servicio.component';

@Component({
    selector: 'app-cotizacion-formulario',
    templateUrl: 'cotizacion-formulario.component.html',
    styleUrls: ['./cotizacion-formulario.component.css'],
    providers: [ComprasService, UsuarioService]
})
export class CotizacionFormularioComponent extends ListBaseComponent implements OnInit, OnChanges {

    @BlockUI() blockUI: NgBlockUI;
    @ViewChild('cotizacionMaterial') cotizacionMaterial: CotizacionMaterialComponent
    @ViewChild('cotizacionServicio') cotizacionServicio: CotizacionServicioComponent
    peticion: PeticionDeOfertaDto;
    posicionesCompra: PeticionDeOfertaSolpPosicionDto[];
    @Input() esFinalizado: boolean;
    cotizaciones: GuardarCotizacion[];
    displayCotizacionCreada: boolean;
    visualizarMensajeDeModificacion: boolean;
    cotizacion: any;
    archivos = new Array<File>()
    archivosEconomico: File[];
    archivosTecnico: File[];

    constructor(protected service: ComprasService, protected usuarioService: UsuarioService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }
    ngOnChanges(changes: SimpleChanges): void {

        if (this.route.params) {
            this.route.params.forEach((params: Params) => {
                let id = parseInt(params["id"]);
                this.obtenerCotizacion(id);
            })
        };
    }

    ngOnInit() {
        this.peticion = { Id: null, NroSolp: null, PeticionDeOfertaPosicion: null, Cotizacion: null };
        if (this.route.params) {
            this.route.params.forEach((params: Params) => {
                let id = parseInt(params["id"]);
                this.obtenerCotizacion(id);
            })
        };
    }

    obtenerCotizacion(id) {
        try {
            this.blockUI.start('Cargando...');
            this.subscription = this.service.obtenerCotizacion(id).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.peticion = result.data;
                        this.posicionesCompra = this.peticion.PeticionDeOfertaPosicion;
                        this.parsearFecha();
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

    public parsearFecha() {
        for (let index = 0; index < this.posicionesCompra.length; index++) {

            if (this.posicionesCompra[index].Posiciones.CotizacionPosicion.FechaDeEntrega != null) {
                var milliseconds = parseInt(this.posicionesCompra[index].Posiciones.CotizacionPosicion.FechaDeEntrega.substring(6));
                var date = new Date(milliseconds);
                this.posicionesCompra[index].Posiciones.CotizacionPosicion.FechaDeEntrega = date
            }
        }

    }
    salir() {
        this.navService.navegarSeccion("/compras/dashboardProveedor");
    }
    onShowFinalizarDialog() {
        this.esFinalizado = true;
        this.ObtenerCotizacion();
        this.ObtenerCotizacionPosicion();
        this.obtenerArchivosNuevos();
        let mensaje = this.ValidarCotizacionFinalizada();
        if (mensaje != "") {
            this.floatMsgService.setErrorMsg(mensaje)
            return;
        }
        this.confirmationService.confirm({
            header: "¡Ultimo Paso!",
            acceptLabel: "SI, CONFIRMAR",
            rejectLabel: "VOLVER",
            message: 'Esta a punto de enviar la cotizacion. <b>¿Desea confirmar?</b>',
            accept: () => {
                this.finalizarCotizacion();
            },
            reject: () => {
            }
        });
    }

    public finalizarCotizacion() {
        this.guardarCotizacion();
    }

    public ObtenerCotizacion() {
        if (this.peticion.TipoPosicionCodigo == "MATERIALES") {
            this.cotizacion = this.cotizacionMaterial.getCotizacion();
        } else {
            this.cotizacion = this.cotizacionServicio.getCotizacion();
        }
    }

    public ObtenerCotizacionPosicion() {
        if (this.peticion.TipoPosicionCodigo == "MATERIALES") {
            this.cotizaciones = this.cotizacionMaterial.crearCotizacionPosicion();
        } else {
            this.cotizaciones = this.cotizacionServicio.crearCotizacionPosicion();
        }
    }

    public guardarCotizacion() {
        this.obtenerArchivosNuevos();
        this.ObtenerCotizacion();
        this.blockUI.start("Grabando...");
        try {

            this.subscription = this.service.GrabarCotizacion(this.cotizacion, this.esFinalizado).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        // this.nroPeticion = result.data.IdEntidad;
                        this.displayCotizacionCreada = true;
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

    public obtenerArchivosNuevos() {
        if (this.peticion.TipoPosicionCodigo == "MATERIALES") {
            this.archivos = this.cotizacionMaterial.ObtenerArchivos();
        } else {
            this.archivosEconomico = this.cotizacionServicio.ObtenerArchivosEconomicos()
            this.archivosTecnico = this.cotizacionServicio.ObtenerArchivosEconomicos();
        }

    }

    public ValidarCotizacionFinalizada() {
        var mensaje = "";
        var breakFor = false;
        if (this.peticion.TipoPosicionCodigo == "MATERIALES") {

            this.cotizaciones.forEach(function (cotizacion, i) {
                if (!breakFor) {
                    if (cotizacion.Cantidad == 0 || cotizacion.Cantidad == undefined) {
                        mensaje = "Pos. " + cotizacion.Posicion + " - La Ctd. cotizada es obligatoria";
                        breakFor = true;
                        return mensaje;
                    }
                    if (cotizacion.UnidadDeMedidaId == 0) {
                        mensaje = "Pos. " + cotizacion.Posicion + " - La Um. cotizada es obligatoria";
                        breakFor = true;
                        return mensaje;
                    }
                    if (cotizacion.MonedaId == 0) {
                        mensaje = "Pos. " + cotizacion.Posicion + " - La Moneda es obligatoria";
                        breakFor = true;
                        return mensaje;
                    }
                    if (cotizacion.Precio == 0) {
                        mensaje = "Pos. " + cotizacion.Posicion + " - El Precio cotizado es obligatorio";
                        breakFor = true;
                        return mensaje;
                    }
                    if (cotizacion.FechaDeEntrega == null) {
                        mensaje = "Pos. " + cotizacion.Posicion + " - La Fecha de entrega cotizada es obligatoria";
                        breakFor = true;
                        return mensaje;
                    }
                }
            });

            var archivoWeb = this.archivos.reduce((sum, file) => sum + file.size, 0);
            if (archivoWeb > 10000000) {
                mensaje = "El archivo economico adjuntado no debe superar los 10Mb";
                return mensaje;
            }
        }else{
            if(this.peticion.RespetaMateriales == null || this.peticion.RespetaMateriales == undefined){
                mensaje = "El campo respeta materiales es obligatorio";
                return mensaje;
            }
            if(this.peticion.RespetaServicios == null || this.peticion.RespetaServicios == undefined){
                mensaje = "El campo respeta servicios es obligatorio";
                return mensaje;
            }
            var archivoWeb = this.archivosEconomico.reduce((sum, file) => sum + file.size, 0);
            if (archivoWeb > 10000000) {
                mensaje = "El archivo economico adjuntado no debe superar los 10Mb";
                return mensaje;
            }
    
            var archivoWeb = this.archivosTecnico.reduce((sum, file) => sum + file.size, 0);
            if (archivoWeb > 10000000) {
                mensaje = "El archivo tecnico adjuntado no debe superar los 10Mb";
                return mensaje;
            }

            this.cotizacion.CotizacionesHoras.forEach(function (cotizacionHora, i) {
                if (!breakFor) {

                    if(cotizacionHora.Gremio == "" && cotizacionHora.Categoria == ""){
                        mensaje = "El Gremio y la Categoria son campos obligatorios";
                        breakFor = true;
                        return mensaje;
                    }

                    if(cotizacionHora.Gremio != "" && cotizacionHora.Categoria == ""){
                        mensaje = "Para el Gremio " + cotizacionHora.Gremio + ", el campo Categoria es obligatorio";
                        breakFor = true;
                        return mensaje;
                    }

                    if(cotizacionHora.Categoria != "" && cotizacionHora.Gremio == ""){
                        mensaje = "Para la Categoria " + cotizacionHora.Categoria + ", el campo Gremio es obligatorio";
                        breakFor = true;
                        return mensaje;
                    }

                    if(cotizacionHora.CantidadPersonas == 0 && cotizacionHora.HorasNormales == 0 && cotizacionHora.HorasNocturnas == 0){
                        mensaje = "Para la Categoria " + cotizacionHora.Categoria + ", debe ingresar cantidad de personas y horas";
                        breakFor = true;
                        return mensaje;
                    }

                    if(cotizacionHora.CantidadPersonas > 0 && cotizacionHora.HorasNormales == 0 && cotizacionHora.HorasNocturnas == 0){
                        mensaje = "Para la Categoria " + cotizacionHora.Categoria + ", debe ingresar horas";
                        breakFor = true;
                        return mensaje;
                    }

                    if((cotizacionHora.HorasNormales > 0 || cotizacionHora.HorasNocturnas > 0) && cotizacionHora.CantidadPersonas == 0){
                        mensaje = "Para la Categoria " + cotizacionHora.Categoria +", el campo cantidad de personas es obligatorio";
                        breakFor = true;
                        return mensaje;
                    }
                }
            });
        }

      
        return mensaje;
    }
}
