import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ConfirmationService, SelectItem } from 'primeng/api';
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
import { CotizacionHoraDto, CotizacionDto, CotizacionPosicionDto } from '../../../modelos/cotizacionDto';
import { AdjudicacionDto } from '../../../modelos/adjudicacion';
import { TextosAdjudicarComponent } from './textos-adjudicar/textos-adjudicar.component';

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

    @ViewChild("textoAdjudicar")
    protected modalTexto: TextosAdjudicarComponent;

    @Input()
    public peticion: PeticionDeOfertaDto;
    peticionOferta: PeticionDeOfertaDto;
    SolpDto: Solp;
    tablaOfertas: PeticionDeOfertaDto;
    adjudicacion: AdjudicacionDto;
    Cotizacion: CotizacionDto;
    displayTextos: boolean;
    TodasPosicionesSeleccionadas: boolean = false;
    displayAdjudicacionCreada: boolean;
    errores: any = [];
    displayVisualizarErrores: boolean;
    numeroOrdenDeCompra: any;
    displayPanelHs: boolean = false;
    textoRacionalInCompleto: boolean;
    displayTextoIncompleto: boolean;
    regionSap: SelectItem[];
    selectedRegion: any;
    displayRegionSap: boolean;
    centroDire: any;
    centroDireLista: any;

    @Input()
    public peticionHs: CotizacionHoraDto;
    error: string;
    displayVisualizarPrecio: boolean;
    peticionOferta_Id: number;
    displayGenerarOCMoneda: boolean;
    generarOC: boolean;
    moneda: any;
    resultado: any;
    centroLista: any[];
    usuario: any;
    lista: any[];
    numerosDePedido: any;
    displayPlazo: boolean;

    constructor(protected service: ComprasService, protected usuarioService: UsuarioService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    ngOnInit() {
        if (this.route.params) {
            this.route.params.forEach((params: Params) => {
                this.peticionOferta_Id = parseInt(params["id"]);
                this.verOfertas(this.peticionOferta_Id);
            })
            this.textoRacionalInCompleto = true;
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
                Id: null,
            };
        }
    }

    ngAfterViewInit(): void {
        this.getCombos();
    }

    seleccionarTodo() {
        if (this.TodasPosicionesSeleccionadas) {
            this.tablaOfertas.PeticionDeOfertaPosicion.map(pos => {
                if (!pos.Posicion.AdjudicacionCompleta) {
                    pos.Selected = true;
                }
            });
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
        this.service.DescargarAdjuntosCotizacion(cotizacionId, false)
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
        this.lista = []
        this.usuario = usuario;
        this.adjudicacion.Proveedor = usuario.CodigoProveedor;
        if (this.tablaOfertas.PeticionDeOfertaPosicion.filter(x => x.Selected).length > 0) {
            this.tablaOfertas.PeticionDeOfertaPosicion.forEach((peticion) => {
                if (peticion.Selected && !peticion.Posicion.AdjudicacionCompleta) {
                    usuario.Cotizacion.CotizacionPosiciones.forEach((cotizacionPos) => {
                        if (peticion.Id == cotizacionPos.PeticionDeOfertaSolpPosicion_Id)
                        this.lista.push({
                                Posicion: peticion.Posicion.Indice,
                                CotizacionPosicion_Id: cotizacionPos.Id,
                                PlazoDeEntrega: peticion.Posicion.FechaEntregaServicio,
                                Cantidad: this.tablaOfertas.TipoPosicionCodigo == 'MATERIALES' ?
                                    peticion.Posicion.CantidadAdjudicacion : 1,
                                SolpPosicion_Id: peticion.Posicion.Id,
                                CantidadCotizada: cotizacionPos.Cantidad,
                                CantidadSolp: peticion.Posicion.CantidadPendiente,
                                CantidadAdjudicada: this.tablaOfertas.TipoPosicionCodigo == 'MATERIALES' ?
                                    peticion.Posicion.CantidadAdjudicada : 1,
                                NoDisponible: cotizacionPos.NoDisponible,
                                MonedaCotizacion: cotizacionPos.Moneda_Id,
                                MonedaPO: peticion.Posicion.MonedaId,
                                MonedaId: cotizacionPos.Moneda_Id,
                                CentroPosicion: peticion.Posicion.Centro,
                                Descripcion: peticion.Posicion.CodigoMaterialSap.Descripcion == null ? peticion.Posicion.Tarea : peticion.Posicion.CodigoMaterialSap.Descripcion
                            })
                    })
                } else {
                    peticion.Selected = false;
                }
            });
            this.parsearFecha();
            if (this.lista.length > 0) {
                this.error = this.validarAdjudicacion(this.lista);
                if (this.error != "") {
                    this.floatMsgService.setErrorMsg(this.error)
                    return;
                }
            } else {
                this.floatMsgService.setInfoMsg("Debe seleccionar alguna posición válida para adjudicar");
                return;
            }
            var posRegion = this.lista[0].CentroPosicion.CodigoSap;

            if (posRegion) {
                this.centroDire = this.centroDireLista.find(c => c.label == posRegion);
                this.selectedRegion = { label: this.centroDire.label, value: this.centroDire.value };
            }

            this.adjudicacion.AdjudicacionPosiciones = this.lista;
            this.adjudicacion.Cotizacion_Id = usuario.Cotizacion.Id;
            this.adjudicacion.Solp_Id = this.tablaOfertas.Solp_Id;
           
            this.validacionTextosIncompletos();
            if(this.textoRacionalInCompleto == true){
                this.displayTextoIncompleto = true;
            } else { 
                this.abrirModalPosicionPlazo();        
            }

            this.displayRegionSap = true;

            if(!this.displayRegionSap){
                this.proveedor = usuario.CodigoProveedor;
                this.mostrarModalGenerarOCMoneda(usuario);
            }
            
        } else {
            this.floatMsgService.setInfoMsg("Debe seleccionar alguna posición para adjudicar");
        }
    }

    public parsearFecha() {
        if (this.lista != undefined) {
            for (let index = 0; index < this.lista.length; index++) {
                if (this.lista[index].PlazoDeEntrega != null) {
                    var milliseconds = parseInt(this.lista[index].PlazoDeEntrega.substring(6));
                    var date = new Date(milliseconds);
                    this.lista[index].PlazoDeEntrega = date
                }
            }
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

                if (element.NoDisponible == true) {
                    self.error = "Pos " + element.Posicion + " - No se puede adjudicar una posición no disponible";
                    breakFor = true;
                    return self.error;
                }

                if (this.tablaOfertas.TipoPosicionCodigo == 'MATERIALES' && this.tablaOfertas.Adicional) {
                    if (element.MonedaCotizacion != element.MonedaPO) {
                        self.error = "Pos " + element.Posicion + " - La moneda de la cotización y de la OC debe ser la misma";
                        breakFor = true;
                        return self.error;
                    }
                }
            }
        });
        return this.error;
    }

    mostrarValidacionMoneda() {
        const posicionesSeleccionadas = this.tablaOfertas.PeticionDeOfertaPosicion
            .filter(x => x.Selected && !x.Posicion.AdjudicacionCompleta);

        const posiciones = this.usuario.Cotizacion.CotizacionPosiciones
            .filter(cotizacionPos =>
                posicionesSeleccionadas.some(peticion => peticion.Id === cotizacionPos.PeticionDeOfertaSolpPosicion_Id)
            );

        const primeraMoneda = posiciones[0].Moneda_Id; // Tomamos la moneda de la primera posición

        for (let i = 0; i < posiciones.length; i++) {
            if (posiciones[i].Moneda_Id !== primeraMoneda) {
                return true; // Si encontramos una moneda diferente, devolvemos true
            }
           
            for (let j = 0; j < posiciones[i].CotizacionSubPosiciones.length; j++) {
                if (posiciones[i].CotizacionSubPosiciones[j].Moneda_Id !== primeraMoneda) {
                    return true; // Si encontramos una moneda diferente, devolvemos true
                }
            }
        }

        return false;
    }

    ocultarGenerarOCSerivicioDiferentesMonedas(){
        if(this.tablaOfertas.TipoPosicionCodigo != 'MATERIALES'){

        }
    }

    mostrarModalGenerarOCMoneda(proveedor){
        if(this.mostrarValidacionMoneda()){
            this.displayGenerarOCMoneda = true;
            this.devolverMonedaProveedor(proveedor.CodigoProveedor);
        }else{
            this.confirmacionAdjudicar();
        }
    }

    onGenerarOC(){
        this.generarOC = false;
        this.confirmacionAdjudicar();
    }

    onGenerarOCProveedor(){
        this.generarOC = true;
        if(this.moneda == undefined){
         this.floatMsgService.setErrorMsg("El proveedor no tiene una moneda configurada");
         this.onCerrarMoneda();
        }else{
            this.confirmacionAdjudicar();
        }
      
    }

    onCerrarMoneda(){
        this.displayGenerarOCMoneda = false;
    }

    devolverMonedaProveedor(codigo) {
        this.blockUI.start('Cargando...');
        this.service.devolverMonedaProveedor(codigo)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        this.moneda = result.Moneda;                   
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
       this.blockUI.stop();
    }

    confirmacionAdjudicar() {
        this.confirmationService.confirm({
            header: "¡Último Paso!",
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
        this.adjudicacion.EsMonedaProveedor = this.generarOC;
        try {
            this.guardarAdjudicacionTextos();
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
                        this.onCerrarMoneda();
                        this.noContinuarAdjudicacion();                        
                    }
                    else {
                        this.numerosDePedido = result.NumerosDePedido;
                        this.displayAdjudicacionCreada = true;
                        this.adjudicacion.CondicionesDeEntrega = "";
                        this.adjudicacion.CondicionesDePago = "";
                        this.adjudicacion.Garantias = "";
                        this.adjudicacion.TextoDeCabecera = "";
                        this.onCerrarMoneda();
                        this.noContinuarAdjudicacion();
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

    validacionTextosIncompletos(){
        this.textoRacionalInCompleto = false;
        if((this.adjudicacion.CondicionesDeEntrega == "" || this.adjudicacion.CondicionesDeEntrega == undefined) 
            && (this.adjudicacion.CondicionesDePago == "" || this.adjudicacion.CondicionesDePago == undefined)
            && (this.adjudicacion.Garantias == "" || this.adjudicacion.Garantias == undefined)
            && (this.adjudicacion.TextoDeCabecera == "" || this.adjudicacion.TextoDeCabecera == undefined)){
                this.textoRacionalInCompleto = true;
        }
    }

    salir() {
        this.displayAdjudicacionCreada = false;
        this.verOfertas(this.tablaOfertas.Id.toString())
    }

    salirVisualizarErrores() {
        this.displayVisualizarErrores = false;
    }

    abrilModalTextos() {
        this.displayTextos = true;
    }

    cerrarModalTextos() {
        this.displayTextos = false;
    }

    aceptarModalTextos() {
        this.displayTextos = false;
    }

    guardarAdjudicacionTextos() {
        this.adjudicacion.CondicionesDeEntrega = this.modalTexto.adjudicacion.CondicionesDeEntrega;
        this.adjudicacion.CondicionesDePago = this.modalTexto.adjudicacion.CondicionesDePago;
        this.adjudicacion.Garantias = this.modalTexto.adjudicacion.Garantias;
        this.adjudicacion.TextoDeCabecera = this.modalTexto.adjudicacion.TextoDeCabecera;
    }

    onEditarCelda(cotizacion: any, campo: string, valorInicial: any) {
        if (cotizacion[campo] === valorInicial) {
            cotizacion[campo] = ''; // Limpia el valor si es igual al valorInicial
        }
    }

    onReestablecerValor(cotizacion: any, campo: string) {
        if (cotizacion[campo] === '' || cotizacion[campo] === null) {
            cotizacion[campo] = 0; // Restablece a cero si está en blanco
        }
    }

    validarNumero(event: any) {
        const inputValue = event.target.value;
        if (isNaN(inputValue) || inputValue < 0) {
            event.target.value = 0; // Borra el valor si es negativo
        }
    }

    cerrarModalPrecios() {
        this.displayVisualizarPrecio = false;
    }

    abrirModalPrecios() {
        this.displayVisualizarPrecio = true;
    }


    obtenerPrimerPlazo(cotizacionPosicion: any): string {
    let plazos = '';

    if (cotizacionPosicion.PrimerPlazoDeOferta > 0) {
        plazos += 'Plazo: ' + cotizacionPosicion.PrimerPlazoDeOferta + ' - Cantidad: ' + cotizacionPosicion.PrimeraCantidad;
    } else {
        plazos += 'sin Plazo';
    }    
    return plazos;
    }

    obtenerSegundoPlazo(cotizacionPosicion: any): string {
    let plazos = '';
    if (cotizacionPosicion.SegundoPlazoDeOferta > 0) {
        plazos += 'Plazo: ' + cotizacionPosicion.SegundoPlazoDeOferta + ' - Cantidad: ' + cotizacionPosicion.SegundaCantidad ;
    }
    return plazos;
    }
    
    obtenerTercerPlazo(cotizacionPosicion: any): string {
    let plazos = '';
    
    if (cotizacionPosicion.TercerPlazoDeOferta > 0) {
        plazos += 'Plazo: ' + cotizacionPosicion.TercerPlazoDeOferta + ' - Cantidad: ' + cotizacionPosicion.TerceraCantidad ;
    }
    return plazos;
    }

    aceptarRegion(){
        this.adjudicacion.RegionSap = this.selectedRegion.value;
        console.log("this.selectedRegion", this.selectedRegion.value);
        this.validacionTextosIncompletos();
        if(this.textoRacionalInCompleto == true){
            this.displayTextoIncompleto = true;
        }
        this.displayRegionSap = false;
    }

    salirRegion(){
        this.displayRegionSap = false;
    }

    continuarAdjudicacion(){
        this.displayTextoIncompleto = false;
        this.abrirModalPosicionPlazo();        
    }

    noContinuarAdjudicacion(){
    this.displayTextoIncompleto = false;
    }

    getCombos() {
        try {
            this.subscription = this.service.getCombos().subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.regionSap = [];
                        this.centroDireLista = [];
                        result.CentrosDireccion.forEach(d => this.centroDireLista.push({
                            label: d.CodigoSap, value: d.RegionSap_Id
                        }));
                        result.Regiones.forEach(d => this.regionSap.push({
                            label: d.Descripcion, value: d.Id
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

    abrirModalPosicionPlazo(){
        this.displayPlazo = true;
    }

    cerrarPlazo(){     
        this.displayPlazo = false;
    }

    guardarPlazo(listaParam: any[]){
       this.lista = listaParam;
       this.displayPlazo = false;
       this.proveedor = this.usuario.CodigoProveedor;
       this.mostrarModalGenerarOCMoneda(this.usuario)
    }
}
