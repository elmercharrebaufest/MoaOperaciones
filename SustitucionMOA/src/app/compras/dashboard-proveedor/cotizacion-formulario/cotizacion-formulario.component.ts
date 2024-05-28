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
import { CotizacionDto, CotizacionHoraDto, GuardarCotizacion } from '../../../modelos/cotizacionDto';
import { CotizacionServicioComponent } from './cotizacion-servicio/cotizacion-servicio.component';
import { forEach } from '@angular/router/src/utils/collection';

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
    peticion: PeticionDeOfertaDto = { Id: null, NroSolp: null, PeticionDeOfertaPosicion: null, Cotizacion: null };
    posicionesCompra: PeticionDeOfertaSolpPosicionDto[];
    @Input() esFinalizado: boolean;
    cotizaciones: GuardarCotizacion[];
    displayCotizacionCreada: boolean;
    cotizacion: any;
    archivos = new Array<File>()
    archivosEconomico: File[];
    archivosTecnico: File[];
    cotizacionSubposiciones: any;
    cambioDeGrillaOk: boolean;
    displayHoras: boolean = false;
    displayFinalizada: boolean = false;
    confirmoHoras: boolean;
    displayObservacionTecnica: boolean;
    displayPropuestaEconomica: boolean;
    confirmoPropuestaEconomica: boolean;
    displayPropuestaTecnica: boolean;
    confirmoPropuestaTecnica: boolean;

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

            if (this.posicionesCompra[index].Posiciones.FechaEntregaServicio != null) {
                var milliseconds = parseInt(this.posicionesCompra[index].Posiciones.FechaEntregaServicio.substring(6));
                var date = new Date(milliseconds);
                this.posicionesCompra[index].Posiciones.FechaEntregaServicio = date
            }

            if (this.posicionesCompra[index].Posiciones.CotizacionPosicion.FechaDeVigencia != null) {
                var milliseconds = parseInt(this.posicionesCompra[index].Posiciones.CotizacionPosicion.FechaDeVigencia.substring(6));
                var date = new Date(milliseconds);
                this.posicionesCompra[index].Posiciones.CotizacionPosicion.FechaDeVigencia = date
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
        this.ObtenerCotizacionSubposicion();
        this.obtenerArchivosNuevos();
        let mensaje = this.ValidarCotizacionFinalizada();
        if (mensaje != "") {
            this.floatMsgService.setErrorMsg(mensaje)
            return;
        }
        this.RevalidarGrillaHoras();       
    }

    public finalizarCotizacion(esFinalizado) {
        this.guardarCotizacion(esFinalizado);
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

    public ObtenerCotizacionSubposicion() {
        if (this.peticion.TipoPosicionCodigo != "MATERIALES") {
            this.cotizacionSubposiciones = this.cotizacionServicio.crearCotizacionSubPosicion();
        }
    }

    public guardarCotizacion(esFinalizado) {
        if (esFinalizado == undefined) this.esFinalizado = false;
        this.obtenerArchivosNuevos();
        this.ObtenerCotizacion();
            this.blockUI.start("Grabando...");
            try {

                this.subscription = this.service.GrabarCotizacion(this.cotizacion, esFinalizado).subscribe(
                    (result: any) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.floatMsgService.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.floatMsgService.setInfoMsg(result.info);
                        } else {
                            // this.nroPeticion = result.data.IdEntidad;
                            if (result.data.Errores && result.data.Errores.length > 0) {
                                this.floatMsgService.setErrorMsg(result.data.Errores[0]);
                            } else {
                                this.displayCotizacionCreada = true;
                            }
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
            this.archivosTecnico = this.cotizacionServicio.ObtenerArchivosTecnicos();
        }
    }

    public ValidarCotizacionFinalizada() {
        var mensaje = "";
        var breakFor = false;
        if (this.peticion.TipoPosicionCodigo == "MATERIALES") {
            var self = this;
            if (this.peticion.PideDescripcionTecnica) this.peticion.RespetaMateriales = false;
            if (this.peticion.RespetaMateriales == null || this.peticion.RespetaMateriales == undefined) {
                mensaje = "El campo respeta materiales es obligatorio";
                return mensaje;
            }
            this.cotizaciones.forEach(function (cotizacion, i) {
                if (!breakFor && (cotizacion.NoDisponible == false || cotizacion.NoDisponible == undefined)) {
                    if ((cotizacion.Cantidad <= 0 || cotizacion.Cantidad == undefined) && cotizacion.UnidadDeMedidaId == 0 && cotizacion.MonedaId == 0 && cotizacion.Precio <= 0 && cotizacion.FechaDeEntrega == null && cotizacion.FechaDeVigencia == null) {
                        mensaje = "Pos. " + cotizacion.Posicion + " Por favor tildar NO DISPONIBLE en el caso de no contar con el material";
                        breakFor = true;
                        return mensaje;
                    }
                    if (cotizacion.Cantidad <= 0 || cotizacion.Cantidad == undefined) {
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
                    if (cotizacion.Precio <= 0) {
                        mensaje = "Pos. " + cotizacion.Posicion + " - El Precio cotizado es obligatorio";
                        breakFor = true;
                        return mensaje;
                    }
                    const decimalPart = (cotizacion.Precio % 1).toFixed(2);
                    if (decimalPart != '0.00' && cotizacion.monedaCompras == "CLP") {
                        mensaje = "Pos. " + cotizacion.Posicion + ": Para la moneda seleccionada no es posible ingresar decimales en el precio";
                        breakFor = true;
                        return mensaje;
                    }
                    if (cotizacion.FechaDeEntrega == null) {
                        mensaje = "Pos. " + cotizacion.Posicion + " - La Fecha de entrega cotizada es obligatoria";
                        breakFor = true;
                        return mensaje;
                    }
                    if (cotizacion.FechaDeVigencia == null) {
                        mensaje = "Pos. " + cotizacion.Posicion + " - La Fecha de vigencia es obligatoria";
                        breakFor = true;
                        return mensaje;
                    }

                    var noTieneArchivo = (self.cotizacion.ArchivosNuevos == null || self.cotizacion.ArchivosNuevos.length == 0 &&
                        (self.cotizacion.ArchivosTipo == null || self.cotizacion.ArchivosTipo.filter(x => x.FileKey == "CotizacionRevisionEconomica") == null
                            || self.cotizacion.ArchivosTipo.filter(x => x.FileKey == "CotizacionRevisionEconomica").length == 0));
                    if (self.peticion.RespetaMateriales == false && self.peticion.ObservacionEconomica == "" && noTieneArchivo) {
                        mensaje = "Debe adjuntar un archivo o agregar una observación";
                        breakFor = true;
                        return mensaje;
                    }
                    if (self.peticion.PideDescripcionTecnica && noTieneArchivo) {
                        mensaje = "Debe adjuntar la documentación solicitada";
                        breakFor = true;
                        return mensaje;
                    }

                    if (cotizacion.CantidadSubpos != cotizacion.Cantidad ||
                        cotizacion.UnidadDeMedidaSubpos != cotizacion.UnidadDeMedidaId) {
                        if (
                            self.cotizacion.ObservacionEconomica == "" || ((self.cotizacion.ArchivosNuevos == null
                                || self.cotizacion.ArchivosNuevos.length == 0 &&
                                (self.cotizacion.ArchivosTipo == null || self.cotizacion.ArchivosTipo.filter(x => x.FileKey == "CotizacionRevisionEconomica") == null
                                    || self.cotizacion.ArchivosTipo.filter(x => x.FileKey == "CotizacionRevisionEconomica").length == 0)))) {
                            mensaje = "Pos. " + cotizacion.Posicion + ": Debe explicar en las observaciones por qué modificó la cantidad y/o unidad de medida. Para estos casos también debe adjuntar un archivo.";
                            breakFor = true;
                            return mensaje;
                        }
                    }

                    const plazosYOfertas = [
                        { plazo: cotizacion.PrimerPlazoDeOferta, cantidad: Number(cotizacion.PrimeraCantidad) },
                        { plazo: cotizacion.SegundoPlazoDeOferta, cantidad: Number(cotizacion.SegundaCantidad) },
                        { plazo: cotizacion.TercerPlazoDeOferta, cantidad: Number(cotizacion.TerceraCantidad) },
                    ];

                    const sumaCantidades = plazosYOfertas.reduce((suma, item) => suma + (item.cantidad || 0), 0);
                    const sumaCorrecta = sumaCantidades === cotizacion.Cantidad;

                    if (!sumaCorrecta) {
                        mensaje = "Pos. " + cotizacion.Posicion + ": La suma de las cantidades debe ser igual a la cantidad cotizada: " + self.formatearNumero(cotizacion.Cantidad);
                        breakFor = true;
                        return mensaje;
                    }
                }
            });
        } else {
            if (this.peticion.RespetaMateriales == null || this.peticion.RespetaMateriales == undefined) {
                mensaje = "El campo respeta materiales es obligatorio";
                return mensaje;
            }
            if (this.peticion.RespetaServicios == null || this.peticion.RespetaServicios == undefined) {
                mensaje = "El campo respeta servicios es obligatorio";
                return mensaje;
            }
            var count = 0;
            this.posicionesCompra.forEach(function (posicionServicio, i) {
                count++;
                if (posicionServicio.Posiciones.CotizacionPosicion.PrimerPlazoDeOferta <= 0 || posicionServicio.Posiciones.CotizacionPosicion.PrimerPlazoDeOferta == undefined) {
                    mensaje = "Propuesta Económica - " + "Pos. " + count + " - El plazo de entrega es obligatorio: Debe indicar la cantidad de días.";
                    breakFor = true;
                    return mensaje;
                }
            });

            this.cotizacion.CotizacionesHoras.forEach(function (cotizacionHora, i) {
                if (!breakFor) {
                    if (cotizacionHora.CantidadPersonas > 0 || cotizacionHora.HorasNormales > 0 || cotizacionHora.HorasNocturnas > 0) {
                        if (cotizacionHora.Gremio == "" && cotizacionHora.Categoria == "") {
                            mensaje = "El Gremio y la Categoría son campos obligatorios";
                            breakFor = true;
                            return mensaje;
                        }

                        if (cotizacionHora.Gremio != "" && cotizacionHora.Categoria == "") {
                            mensaje = "Para el Gremio " + cotizacionHora.Gremio + ", el campo Categoría es obligatorio";
                            breakFor = true;
                            return mensaje;
                        }

                        if (cotizacionHora.Categoria != "" && cotizacionHora.Gremio == "") {
                            mensaje = "Para la Categoría " + cotizacionHora.Categoria + ", el campo Gremio es obligatorio";
                            breakFor = true;
                            return mensaje;
                        }

                        if (cotizacionHora.CantidadPersonas <= 0 && cotizacionHora.HorasNormales <= 0 && cotizacionHora.HorasNocturnas <= 0) {
                            mensaje = "Para la Categoría " + cotizacionHora.Categoria + ", debe ingresar cantidad de personas y horas";
                            breakFor = true;
                            return mensaje;
                        }

                        if (cotizacionHora.CantidadPersonas > 0 && cotizacionHora.HorasNormales <= 0 && cotizacionHora.HorasNocturnas <= 0) {
                            mensaje = "Para la Categoría " + cotizacionHora.Categoria + ", debe ingresar horas";
                            breakFor = true;
                            return mensaje;
                        }

                        if ((cotizacionHora.HorasNormales > 0 || cotizacionHora.HorasNocturnas > 0) && cotizacionHora.CantidadPersonas <= 0) {
                            mensaje = "Para la Categoría " + cotizacionHora.Categoria + ", el campo cantidad de personas es obligatorio";
                            breakFor = true;
                            return mensaje;
                        }
                    }
                }
            });

            if (this.peticion.PersonalHoras == true) {
                var categorias = ['SHyMA'];

                var mostrarMensaje = this.peticion.Cotizacion.CotizacionesHoras
                    .filter(x => !categorias.includes(x.Categoria))
                    .every(x => (Number(x.CantidadPersonas) <= 0));

                if (mostrarMensaje) {
                    mensaje = "En el panel de horas debe completar las categorías que aplican a esta propuesta.";
                    return mensaje
                }
            }
            if ((!this.peticion.PorcentajeDeHoras || this.peticion.PorcentajeDeHoras == 0) &&
                this.peticion.Cotizacion.CotizacionesHoras.filter(x => x.Gremio == 'UOCRA' || x.ConfigurarHora == true).some(x => Number(x.CantidadPersonas) > 0)) {
                mensaje = "El campo Porcentaje de expediente es obligatorio cuando se ingresa la estimación de horas en UOCRA";
            }

            var self = this;
            this.cotizacionSubposiciones.forEach(function (subposicion, i) {
                if (!breakFor) {
                    if (subposicion.Cantidad <= 0 || subposicion.Cantidad == undefined) {
                        mensaje = "Propuesta Económica - " + "Pos. " + subposicion.Posicion + ": La Ctd. cotizada es obligatoria";
                        breakFor = true;
                        return mensaje;
                    }
                    if (subposicion.UnidadDeMedidaId == 0) {
                        mensaje = "Propuesta Económica - " + "Pos. " + subposicion.Posicion + ": La Um. cotizada es obligatoria";
                        breakFor = true;
                        return mensaje;
                    }
                    if (subposicion.MonedaId == 0) {
                        mensaje = "Propuesta Económica - " + "Pos. " + subposicion.Posicion + ": La Moneda es obligatoria";
                        breakFor = true;
                        return mensaje;
                    }
                    if (subposicion.Precio <= 0) {
                        mensaje = "Propuesta Económica - " + "Pos. " + subposicion.Posicion + ": El Precio cotizado es obligatorio";
                        breakFor = true;
                        return mensaje;
                    }
                    const decimalPart = (subposicion.Precio % 1).toFixed(2);
                    if (decimalPart != '0.00' && subposicion.monedaCompras == "CLP") {
                        mensaje = "Propuesta Económica - " + "Pos. " + subposicion.Posicion + ": Para la moneda seleccionada no es posible ingresar decimales en el precio";
                        breakFor = true;
                        return mensaje;
                    }
                    var noTieneArchivoTecnico = self.archivosTecnico == null || self.archivosTecnico.length == 0 &&
                        (self.cotizacion.ArchivosTipo == null || self.cotizacion.ArchivosTipo.filter(x => x.FileKey == "CotizacionRevisionTecnica") == null
                            || self.cotizacion.ArchivosTipo.filter(x => x.FileKey == "CotizacionRevisionTecnica").length == 0);
                    if ((self.peticion.RespetaServicios == false || self.peticion.RespetaMateriales == false) && self.peticion.ObservacionTecnica == "" && noTieneArchivoTecnico) {
                        mensaje = "Propuesta Técnica - Debe adjuntar documentación o agregar una observación";
                        breakFor = true;
                        return mensaje;
                    }
                    if (self.peticion.PideDescripcionTecnica && noTieneArchivoTecnico) {
                        mensaje = "Propuesta Técnica - Debe adjuntar la documentación solicitada";
                        breakFor = true;
                        return mensaje;
                    }
                    if ((subposicion.CantidadSubpos != subposicion.Cantidad || subposicion.UnidadDeMedidaSubpos != subposicion.UnidadDeMedidaId) &&
                        (self.cotizacion.ObservacionEconomica == "" && (self.archivosEconomico == null || self.archivosEconomico.length == 0 &&
                            (self.cotizacion.ArchivosTipo == null || self.cotizacion.ArchivosTipo.filter(x => x.FileKey == "CotizacionRevisionEconomica") == null
                                || self.cotizacion.ArchivosTipo.filter(x => x.FileKey == "CotizacionRevisionEconomica").length == 0)))) {
                        mensaje = "Propuesta Económica - " + "Pos. " + subposicion.Posicion + ": Debe explicar en las observaciones por qué modificó la cantidad y/o unidad de medida. Para estos casos también debe adjuntar un archivo.";
                        breakFor = true;
                        return mensaje;
                    }
                }
            });
        }

        return mensaje;
    }

    public RevalidarGrillaHoras() {
        this.ObtenerCotizacion();
        debugger;
        this.cambioDeGrillaOk = false;
        if (this.cotizacion.EsNuevaCotizacion) {
            if (!this.confirmoHoras && !this.hayCambiosEnGrillaDeHoras(this.cotizacion.CotizacionesHoras, this.cotizacion.CotizacionesHorasOriginal)) {
                this.displayHoras = true;                          
            }
            else if(!this.confirmoPropuestaTecnica && this.hayCambiosEnPropuestaTecnica()){
                this.confirmoHoras = true; 
                this.displayPropuestaTecnica = true;
            }else if(!this.confirmoPropuestaEconomica && this.hayCambiosEnPropuestaEconomica()){
                this.confirmoPropuestaTecnica = true;
                this.displayPropuestaEconomica = true;  
            }else{
                this.mostrarDialogFinalizar();
            }   
        }else{
            this.mostrarDialogFinalizar();
        }       
    }

    aceptarHoras(){
        this.displayHoras = false;   
        this.confirmoHoras = true;     
        if(this.hayCambiosEnPropuestaTecnica()){
            this.displayPropuestaTecnica = true;
        }else{
            this.mostrarDialogFinalizar();
        }     
    }

    salirHoras(){
        this.displayHoras = false; 
    }
    
    aceptarPropuestaTecnica(){
        this.confirmoPropuestaTecnica = true;  
        this.displayPropuestaTecnica = false; 
        if(this.hayCambiosEnPropuestaEconomica()){
            this.displayPropuestaEconomica = true;  
        }else{
            this.mostrarDialogFinalizar();
        }
    }

    salirPropuestaTecnica(){
        this.displayPropuestaTecnica = false; 
    }

    aceptarPropuestaEconomica(){
        this.confirmoPropuestaEconomica = true;  
        this.displayPropuestaEconomica = false; 
        this.mostrarDialogFinalizar();     
        
    }

    salirPropuestaEconomica(){
        this.displayPropuestaEconomica = false; 
    }

    public mostrarDialogFinalizar() {    
        this.confirmationService.confirm({
            header: "¡Último Paso!",
            acceptLabel: "SI, CONFIRMAR",
            rejectLabel: "VOLVER",
            message: 'Está a punto de enviar la cotización. <br>Podrá volver a editarla mientras el plazo de oferta esté vigente y no se le haya adjudicado una orden de compra. <b>¿Desea continuar?</b>',
            accept: () => {
                this.finalizarCotizacion(true);
            },
            reject: () => {
            }
        });
    }

    public hayCambiosEnGrillaDeHoras(original: CotizacionHoraDto[], cambios: CotizacionHoraDto[]): boolean {
        if (original.length !== cambios.length) {
            return true;
        }
    
        for (let i = 0; i < original.length; i++) {
            const originalItem = original[i];
            const cambioItem = cambios[i];
    
            if (!this.sonIguales(originalItem, cambioItem)) {
                return true;
            }
        }
    
        return false;
    }
    
    public sonIguales(objA: any, objB: any): boolean {
        for (const key in objA) {
            if (objA.hasOwnProperty(key)) {
                if (objA[key] !== objB[key]) {
                    return false;
                }
            }
        }
        return true;
    }

    public hayCambiosEnPropuestaEconomica(){
        this.ObtenerCotizacion();
        return this.cotizacion.ObservacionEconomica == this.cotizacion.ObservacionEconomicaOriginal
         || this.cotizacion.ArchivosNuevos.length == 0;
    }

    public hayCambiosEnPropuestaTecnica(){
        this.ObtenerCotizacion();
        return this.cotizacion.ObservacionTecnica == this.cotizacion.ObservacionTecnicaOriginal ||
        this.cotizacion.ArchivosTecnico.length == 0;
    }

    formatearNumero(numero: number) {
        return numero.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }
}
