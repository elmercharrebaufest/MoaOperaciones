import { jsonpCallbackContext } from '@angular/common/http/src/module';
import { MenuItem } from 'primeng/api';
import * as uuid from 'uuid';
import { EnumTipoSolpSap } from '../enum-tipo-solp-sap';
import { SubPosicionViewModel } from "./steps/posicion/tab-subposicion/sub-posicion-view-model";
import { CotizacionPosicionDto } from '../../modelos/cotizacionDto';
import { Solp } from './solp';
import { Input } from '@angular/core';

export class SolpPosicion {
    public id: any;
    public numeroPosicion: number;
    public posicionCheck: boolean;
    public tipoPosicion: any;
    public concluido: boolean;

    public valorTotal: number;
    public valorNeto: number;

    public servicio: string;
    public materialCatalogado: string;
    public materialSinCatalogar: string;
    public material: string;
    public centroDeCosto: boolean;
    public ordenDeOt: boolean;
    public ordenDeInversion: boolean;
    public siniestroBeneficio: boolean;

    // fechas
    public fechaEntregaServicio: Date;
    public fechaDeLiberacion: Date;
    public plazoDeEntrega: number;

    // direccion de entrega
    public selectCentroEntrega: any;
    public selectAlmacenEntrega: any;
    public centroPorDefecto: any;
    public monedaPorDefecto: any;
    public posicionPorDefecto: any;
    public imputacionPorDefecto: any;

    public nombreEntrega: string;
    public calleEntrega: string;
    public numeroEntrega: string;
    public codigoPostalEntrega: string;
    public paisEntrega: string;

    // grupo de compras
    public selectGrupoCompras: any;
    public selectSolicitanteCompras: any;
    public necesidadCompras: string;
    public selectArticuloCompras: any;
    public textoSuministro: string;
    public motivo: string;
    public modelo: string;
    public selectProvincia: any;

    // proveedores 
    public rubroElectrico: boolean;
    public rubroCivil: boolean;
    public rubroMecanico: boolean;
    public rubroIngenieria: boolean;
    public rubroConsultoria: boolean;

    public proveedoresValidos: string[] = [];
    public proveedoresInvalidos: string[] = [];
    public proveedoresNoSugeridos: string[] = [];

    public estado: boolean;
    public indice: number;

    // Moneda
    public selectMonedaCompras: any;
    public monedaSeleccionada: any;

    public posicionSeleccionada: any;
    public imputacionSeleccionada: any;

    //TabItems
    public tabItems: MenuItem[] = [];
    //ActiveMenuTab. Por defecto abre en el tab de subosiciones
    public activeMenuTab: string;
    public activeItem: MenuItem;

    //Imputacion
    public valorImputacion: any;
    public cuentaMayor: any;

    //Posicion 
    public codigoServicio: any;
    public tareaSubcontratar: any;
    public cuentaTd: any;
    public precioBruto: any;
    public unidadSeleccionada: any;
    public tipoImputacion: any;
    public tareaSubcontratarObj: any;
    public numeroContratoSuperior: any;
    public numeroPosicionContratoSuperior: any;
    public provedorFijo: any;
    public nombreProveedor: any;
    public orgCompras: any;

    public posicionValida: boolean;

    //subPosiciones
    public listadoSubPosiciones: Array<SubPosicionViewModel>;

    //validacion
    public tabsPosicionValidos: TabsImputacionValidas = new TabsImputacionValidas();


    public isNewRow: boolean
    Cantidad?: any;
    Id?: any;
    Indice: any;
    CantidadPendiente: any;
    AdjudicacionCompleta: any;
    CantidadAdjudicacion: any;
    CantidadAdjudicada: any;
    NoDisponible: any;
    
    public mensaje: string = "";

    public get getMensaje(): string  {

        return typeof this.mensaje;
    }

    constructor(numeroPosicion, fiscalContrato, fechaEntrega, posicionADuplicar, 
        centroPorDefecto, direccionCentroPorDefecto, monedaPorDefecto, selectTipoPosicion) {
        this.id = uuid.v4();
        this.numeroPosicion = numeroPosicion;
        this.plazoDeEntrega = 10;
        this.fechaEntregaServicio = new Date(fechaEntrega);
        this.fechaEntregaServicio.setDate(fechaEntrega.getDate() + parseInt(this.plazoDeEntrega.toString()));

        this.fechaDeLiberacion = new Date();
        this.listadoSubPosiciones = new Array<SubPosicionViewModel>();
        //agrega un fila por defecto
        this.agregarSubPosicion(this.crearSubPosicion());

        this.selectSolicitanteCompras = fiscalContrato;
        this.estado = true;
        this.posicionCheck = false;

        this.setupValoresPorDefecto(centroPorDefecto, direccionCentroPorDefecto, monedaPorDefecto, selectTipoPosicion);

        if (posicionADuplicar) {
            //this.campo = posicionADuplicar.campo
            this.servicio = posicionADuplicar.servicio;
            this.materialCatalogado = posicionADuplicar.materialCatalogado;
            this.materialSinCatalogar = posicionADuplicar.materialSinCatalogar;
            this.tipoPosicion = posicionADuplicar.tipoPosicion;
            this.tipoImputacion = posicionADuplicar.tipoImputacion;
            this.centroDeCosto = posicionADuplicar.centroDeCosto;
            this.ordenDeOt = posicionADuplicar.ordenDeOt;
            this.ordenDeInversion = posicionADuplicar.ordenDeInversion;
            this.siniestroBeneficio = posicionADuplicar.siniestroBeneficio;
            this.fechaEntregaServicio = posicionADuplicar.fechaEntregaServicio;
            this.fechaDeLiberacion = posicionADuplicar.fechaDeLiberacion;
            this.plazoDeEntrega = posicionADuplicar.plazoDeEntrega;
            this.selectCentroEntrega = posicionADuplicar.selectCentroEntrega;
            this.selectAlmacenEntrega = posicionADuplicar.selectAlmacenEntrega;
            this.centroPorDefecto = posicionADuplicar.centroPorDefecto;
            this.monedaPorDefecto = posicionADuplicar.monedaPorDefecto;
            this.nombreEntrega = posicionADuplicar.nombreEntrega;
            this.calleEntrega = posicionADuplicar.calleEntrega;
            this.numeroEntrega = posicionADuplicar.numeroEntrega;
            this.codigoPostalEntrega = posicionADuplicar.codigoPostalEntrega;
            this.paisEntrega = posicionADuplicar.paisEntrega;
            this.selectGrupoCompras = posicionADuplicar.selectGrupoCompras;
            this.selectSolicitanteCompras = posicionADuplicar.selectSolicitanteCompras;
            this.necesidadCompras = posicionADuplicar.necesidadCompras;
            this.selectArticuloCompras = posicionADuplicar.selectArticuloCompras;
            this.textoSuministro = posicionADuplicar.textoSuministro;
            this.motivo = posicionADuplicar.motivo;
            this.modelo = posicionADuplicar.modelo;
            // this.rubroElectrico = posicionADuplicar.rubroElectrico;
            // this.rubroCivil = posicionADuplicar.rubroCivil;
            // this.rubroMecanico = posicionADuplicar.rubroMecanico;
            // this.rubroIngenieria = posicionADuplicar.rubroIngenieria;
            // this.rubroConsultoria = posicionADuplicar.rubroConsultoria;
            this.proveedoresValidos = posicionADuplicar.proveedoresValidos;
            this.proveedoresInvalidos = posicionADuplicar.proveedoresInvalidos;
            this.proveedoresNoSugeridos = posicionADuplicar.proveedoresNoSugeridos;
            this.selectMonedaCompras = posicionADuplicar.selectMonedaCompras;
            this.monedaSeleccionada = posicionADuplicar.monedaSeleccionada;
            this.posicionSeleccionada = posicionADuplicar.posicionSeleccionada;
            this.cuentaMayor = posicionADuplicar.cuentaMayor;
            this.valorImputacion = posicionADuplicar.TipoImputacionSap;
            this.posicionCheck = false;
            this.tipoImputacion = posicionADuplicar.tipoImputacion;
            this.valorImputacion = posicionADuplicar.valorImputacion;
            this.selectProvincia = posicionADuplicar.selectProvincia;
        }

        this.isNewRow = true;
    }

    public crearSubPosicion() {
        let subPosNumber = this.listadoSubPosiciones.length + 1;
        return new SubPosicionViewModel(subPosNumber);
    }

    public agregarSubPosicion(subPosicion: SubPosicionViewModel) {
        if (subPosicion) {
            this.listadoSubPosiciones.push(subPosicion);
        }
    }

    private setupValoresPorDefecto(centroPorDefecto, direccionCentroPorDefecto, monedaPorDefecto, selectTipoPosicion) {
        this.selectCentroEntrega = centroPorDefecto;
        if (direccionCentroPorDefecto) {
            this.codigoPostalEntrega = direccionCentroPorDefecto.Cp;
            this.calleEntrega = direccionCentroPorDefecto.Direccion;
            this.numeroEntrega = direccionCentroPorDefecto.Numero;
            this.paisEntrega = direccionCentroPorDefecto.Pais;
            if (centroPorDefecto != undefined) {
                this.nombreEntrega = centroPorDefecto.Descripcion;
            }
        }

        if (monedaPorDefecto) {
            this.monedaPorDefecto = monedaPorDefecto.Codigo;
            this.monedaSeleccionada = monedaPorDefecto;
        }
        if (selectTipoPosicion) {
            this.tipoPosicion = selectTipoPosicion;
        }
    }

    public setTabPosicion() {
        let hasCodigoServicio = typeof this.codigoServicio != "undefined" && this.codigoServicio;
        let hasTipoImputacion = typeof this.tipoImputacion != "undefined" && this.tipoImputacion;

        let isVisibleImputaciones = this.esTipoPosicionMaterial;
        let isDisabledImputaciones = this.esTipoPosicionServicio || (hasCodigoServicio && (!hasTipoImputacion || (hasTipoImputacion && hasTipoImputacion.Id == 0)));

        this.tabItems = [
            { label: 'Subposiciones', visible: this.esTipoPosicionServicio, disabled: this.esTipoPosicionMaterial },
            { label: 'Fechas', visible: true },
            { label: 'Datos de posición', visible: true },
            { label: 'Dirección de entrega', visible: true },
            { label: 'Proveedores', visible: true },
            { label: 'Imputaciones', visible: isVisibleImputaciones, disabled: isDisabledImputaciones }
        ];
        this.setInitialActiveMenuTab();
    }

    public setInitialActiveMenuTab() {
        this.activeItem = this.tabItems.find(x => x.visible);
        this.activeMenuTab = this.activeItem.label;
    }

    public calcularValorTotal() {
        let total = 0;
        if (this.esTipoPosicionServicio){
            if (this.listadoSubPosiciones && this.listadoSubPosiciones.length > 0) {
                this.listadoSubPosiciones.forEach(x => {
                    total += (x.precioBruto || 0) * (parseInt(x.cuentaTd) || 0);
                });
            }
        }
        else{
            total = (this.precioBruto || 0) * (parseInt(this.cuentaTd) || 0);  
        }
        this.valorTotal = total;
    }

    public doValidatePosicion(tipoSolpSap: EnumTipoSolpSap) {
        this.validatePosicion(tipoSolpSap);
        if (this.esTipoPosicionServicio) {
            this.validateSubposiciones();
        } 
        this.validateDireccionEntrega()
        this.validateProveedor();
        this.validateDatosPosicion();
        this.validateFechas();
       
        if (this.esTipoPosicionMaterial){
            this.validateImputaciones();
        }        
    }

    public validateSubposiciones() {
        this.tabsPosicionValidos.tabSubposiciones = true;

        if (this.listadoSubPosiciones.length == 0) {
            this.tabsPosicionValidos.tabSubposiciones = false;
        }

        this.listadoSubPosiciones.forEach(pos => {
            let hasNotUnidadSeleccionada = typeof pos.unidadSeleccionada === "undefined" || !pos.unidadSeleccionada;
            let hasNotTareaSubcontratarObj = typeof pos.tareaSubcontratarObj === "undefined" || !pos.tareaSubcontratarObj || pos.tareaSubcontratarObj.Descripcion == "";
            let hasNotCuentaTd = typeof pos.cuentaTd === "undefined" || !pos.cuentaTd;
            let hasNotCuentaMayor = typeof pos.cuentaMayor === "undefined" || !pos.cuentaMayor;
            let hasNotTipoImputacion = typeof pos.tipoImputacion === "undefined" || !pos.tipoImputacion;

            if (
                (hasNotUnidadSeleccionada || !pos.unidadSeleccionada.Id) &&
                (hasNotTareaSubcontratarObj || !pos.tareaSubcontratarObj.Descripcion) &&
                (hasNotCuentaTd) &&
                (!pos.precioBruto || pos.precioBruto.toString() == "" || typeof pos.precioBruto === "undefined" || pos.precioBruto.toString() == "0") &&
                (hasNotCuentaMayor || !pos.cuentaMayor.Id) &&
                (hasNotTipoImputacion || !pos.tipoImputacion.Id)
            ) {
                return;
            }
            if (hasNotUnidadSeleccionada || !pos.unidadSeleccionada.Id) {
                this.tabsPosicionValidos.tabSubposiciones = false;
                this.mensaje = "Pos. " + this.numeroPosicion + " - El campo unidad en la subposicion es obligatorio";

                return this.mensaje;
            }
            if (hasNotTareaSubcontratarObj) {
                this.tabsPosicionValidos.tabSubposiciones = false;
                this.mensaje = "Pos. " + this.numeroPosicion + " - El campo tarea a sub contratar en la subposicion  es obligatorio";
      
                return this.mensaje;
            }
            if (hasNotCuentaTd) {
                this.tabsPosicionValidos.tabSubposiciones = false;
                this.mensaje = "Pos. " + this.numeroPosicion + " - El campo cantidad en la subposicion es obligatorio";
      
                return this.mensaje;
            }
            if (!pos.precioBruto || pos.precioBruto.toString() == "" || typeof pos.precioBruto === "undefined" || pos.precioBruto.toString() == "0") {
                this.tabsPosicionValidos.tabSubposiciones = false;
                this.mensaje = "Pos. " + this.numeroPosicion + " - El campo precio en la subposicion es obligatorio";
      
                return this.mensaje;
            }
            if (hasNotCuentaMayor || !pos.cuentaMayor.Id) {
                this.tabsPosicionValidos.tabSubposiciones = false;
                this.mensaje = "Pos. " + this.numeroPosicion + " - El campo cuenta mayor en la subposicion es obligatorio";
      
                return this.mensaje;
            }
            if (hasNotTipoImputacion || !pos.tipoImputacion.Id) {
                this.tabsPosicionValidos.tabSubposiciones = false;
                this.mensaje = "Pos. " + this.numeroPosicion + " - El campo imputacion en la subposicion es obligatorio";
      
                return this.mensaje;
            }
        });
    }

    public validateDireccionEntrega() {
        this.tabsPosicionValidos.tabDireccionEntrega = true;
        if (!this.calleEntrega || this.calleEntrega == "" || typeof this.calleEntrega === "undefined")
        {
          this.mensaje = "";
          this.tabsPosicionValidos.tabDireccionEntrega = false;
          this.mensaje = "Pos. " + this.numeroPosicion + " - El campo calle de entrega es obligatorio";

          return this.mensaje;
        } 
    }

    public validateImputaciones() {
        let hasTipoImputacion = typeof this.tipoImputacion != "undefined" && this.tipoImputacion;
        if (hasTipoImputacion && hasTipoImputacion.id > 0)
        {        
            if (!this.valorImputacion || typeof this.valorImputacion === "undefined" || typeof this.valorImputacion === undefined)
            {
                this.tabsPosicionValidos.tabImputacion = false;
                this.mensaje = "Pos. " + this.numeroPosicion + " - El campo imputacion es obligatorio";

                return this.mensaje;
            }

            if (!this.cuentaMayor || typeof this.cuentaMayor === "undefined" || typeof this.cuentaMayor === undefined)
            {
                this.tabsPosicionValidos.tabImputacion = false;
                this.mensaje = "Pos. " + this.numeroPosicion + " - El campo cuenta mayor es obligatorio";

                return this.mensaje;
            }
        }
        
        this.tabsPosicionValidos.tabImputacion = true;
    }

    public validateProveedor() {
        this.tabsPosicionValidos.tabProveedor = true;
    }

    public validateDatosPosicion() {
        this.tabsPosicionValidos.tabDatosPosicion = true;       
        if (typeof this.selectGrupoCompras === "undefined" || this.selectGrupoCompras.Id == 0)
        {
          this.tabsPosicionValidos.tabDatosPosicion = false;
          this.mensaje = "Pos. " + this.numeroPosicion + " - El campo grupo de compras es obligatorio";

          return this.mensaje;
        }

        if (typeof this.selectArticuloCompras === "undefined" || this.selectArticuloCompras.Id == 0)
        {
          this.mensaje = "";  
          this.tabsPosicionValidos.tabDatosPosicion = false;
          this.mensaje = "Pos. " + this.numeroPosicion + " - El campo grupo de articulo es obligatorio";

          return this.mensaje;
        }

        if (!this.selectSolicitanteCompras || this.selectSolicitanteCompras == "" || typeof this.selectSolicitanteCompras === "undefined")
        {
          this.tabsPosicionValidos.tabDatosPosicion = false;
          this.mensaje = "Pos. " + this.numeroPosicion + " - El campo solicitante de compras es obligatorio";


          return this.mensaje;
        }

        if (this.esTipoPosicionMaterial) {
            if (!this.textoSuministro || this.textoSuministro == "" || this.textoSuministro == undefined)
            {
              this.mensaje = "";
              this.tabsPosicionValidos.tabDatosPosicion = false;
              this.mensaje = "Pos. " + this.numeroPosicion + " - El campo texto de suministro es obligatorio";

              return this.mensaje;
            }
            let hasCodigoServicio = typeof this.codigoServicio != "undefined" && this.codigoServicio;
            if(!hasCodigoServicio)
            {
                if (!this.modelo || this.modelo == "" || this.modelo == undefined)
                {
                  this.tabsPosicionValidos.tabDatosPosicion = false;
                  this.mensaje = "Pos. " + this.numeroPosicion + " - El campo modelo es obligatorio";
    
                  return this.mensaje;
                }
    
                if (!this.motivo || this.motivo == "" || this.motivo == undefined)
                {
                  this.tabsPosicionValidos.tabDatosPosicion = false;
                  this.mensaje = "Pos. " + this.numeroPosicion + " - El campo motivo es obligatorio";
    
                  return this.mensaje;
                }
            }
          
        }
       
    }

    public validateFechas() {
        if (typeof this.fechaEntregaServicio === "undefined" || typeof this.plazoDeEntrega === "undefined" || this.plazoDeEntrega.toString() == "")
        {
          this.tabsPosicionValidos.tabFechas = false;
          this.mensaje = "Pos. " + this.numeroPosicion + " - El campo fecha de entrega y plazo de entrega son obligatorios";

          return this.mensaje;
        }
    
        this.tabsPosicionValidos.tabFechas = true
    }

    public validatePosicion(tipoSolpSap: EnumTipoSolpSap) {
        this.tabsPosicionValidos.tabPosiciones = true;

        if (!this.selectCentroEntrega || typeof this.selectCentroEntrega === "undefined" || this.selectCentroEntrega == undefined)
        {
            this.tabsPosicionValidos.tabPosiciones = false;
            this.mensaje = "Pos. " + this.numeroPosicion + " - El campo centro de entrega es obligatorio";

            return this.mensaje;
        }

        

        if (!this.tareaSubcontratarObj || typeof this.tareaSubcontratarObj === "undefined" || this.tareaSubcontratarObj == "")
        {
            this.tabsPosicionValidos.tabPosiciones = false;
            this.mensaje = "Pos. " + this.numeroPosicion + " - El campo descripcion es obligatorio";
            console.log("descaripcion", this.tareaSubcontratarObj)
            return this.mensaje;
        }

        if (tipoSolpSap != EnumTipoSolpSap.Mantenimiento) {
            if (!this.selectAlmacenEntrega || typeof this.selectAlmacenEntrega === "undefined" || typeof this.selectAlmacenEntrega === undefined) {
                this.tabsPosicionValidos.tabPosiciones = false;
                this.mensaje = "Pos. " + this.numeroPosicion + " - El campo almacen de entrega es obligatorio";
      
                return this.mensaje;
            }
        }

        if (this.esTipoPosicionServicio) {
            if (!this.tipoImputacion || typeof this.tipoImputacion === "undefined" || this.tipoImputacion === undefined )
            {
                this.tabsPosicionValidos.tabPosiciones = false;
                this.mensaje = "Pos. " + this.numeroPosicion + " - El campo tipo de imputacion es obligatorio";
      
                return this.mensaje;
            }
        }

        if (this.esTipoPosicionMaterial || this.esTipoPosicionServicio) {
            if (!this.monedaSeleccionada || typeof this.monedaSeleccionada === "undefined" || typeof this.monedaSeleccionada === undefined || !this.monedaSeleccionada.Id)
            {
                this.tabsPosicionValidos.tabPosiciones = false;
                this.mensaje = "Pos. " + this.numeroPosicion + " - El campo moneda es obligatorio";
      
                return this.mensaje;
            }
        }

        if (this.esTipoPosicionMaterial) {
            if (!this.unidadSeleccionada || this.unidadSeleccionada == "" || typeof this.unidadSeleccionada === "undefined" || typeof this.unidadSeleccionada === undefined || !this.unidadSeleccionada.Id )
            {
                this.tabsPosicionValidos.tabPosiciones = false;
                this.mensaje = "Pos. " + this.numeroPosicion + " - El campo unidad es obligatorio";
      
                return this.mensaje;
            }
            if (!this.cuentaTd || this.cuentaTd == "" || typeof this.cuentaTd === "undefined" || typeof this.cuentaTd === undefined)
            {
                this.mensaje = "";
                this.tabsPosicionValidos.tabPosiciones = false;
                this.mensaje = "Pos. " + this.numeroPosicion + " - El campo cantidad es obligatorio";
      
                return this.mensaje;
            }
            if (!this.precioBruto || this.precioBruto == "" || typeof this.precioBruto === "undefined" || this.precioBruto == "0" || typeof this.precioBruto === undefined)
            {
                this.tabsPosicionValidos.tabPosiciones = false;
                this.mensaje = "Pos. " + this.numeroPosicion + " - El campo precio es obligatorio";
      
                return this.mensaje;
            }
        } 
    }

    public get isPosicionEliminada(): boolean {
        return this.estado == false;
    }

    public get esTipoPosicionMaterial(): boolean  {
        return typeof this.tipoPosicion != "undefined" && this.tipoPosicion && this.tipoPosicion.Codigo == "MATERIALES";
    }

    public get esTipoPosicionServicio(): boolean  {
        return typeof this.tipoPosicion != "undefined" && this.tipoPosicion && this.tipoPosicion.Codigo == "SERVICIO";
    }

}

export class TabsImputacionValidas {
    tabImputacion: boolean = true;
    tabProveedor: boolean = true;
    tabDireccionEntrega: boolean = false;
    tabDatosPosicion: boolean = false;
    tabFechas: boolean = false;
    tabSubposiciones:  boolean = false;
    tabPosiciones: boolean = true;
}