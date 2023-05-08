import { jsonpCallbackContext } from '@angular/common/http/src/module';
import { MenuItem } from 'primeng/api';
import * as uuid from 'uuid';
import { EnumTipoSolpSap } from '../enum-tipo-solp-sap';
import { SubPosicionViewModel } from "./steps/posicion/tab-subposicion/sub-posicion-view-model";
import { CotizacionPosicionDto } from '../../modelos/cotizacionDto';

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
        let isDisabledImputaciones = this.esTipoPosicionServicio || (hasCodigoServicio && !hasTipoImputacion);

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
            let hasNotTareaSubcontratarObj = typeof pos.tareaSubcontratarObj === "undefined" || !pos.tareaSubcontratarObj;
            let hasNotCuentaTd = typeof pos.cuentaTd === "undefined" || !pos.cuentaTd;
            let hasNotCuentaMayor = typeof pos.cuentaMayor === "undefined" || !pos.cuentaMayor;
            let hasNotTipoImputacion = typeof pos.tipoImputacion === "undefined" || !pos.tipoImputacion;

            if (
                (hasNotUnidadSeleccionada || !pos.unidadSeleccionada.Id) &&
                (hasNotTareaSubcontratarObj || !pos.tareaSubcontratarObj.Id) &&
                (hasNotCuentaTd) &&
                (!pos.precioBruto || pos.precioBruto.toString() == "" || typeof pos.precioBruto === "undefined" || pos.precioBruto.toString() == "0") &&
                (hasNotCuentaMayor || !pos.cuentaMayor.Id) &&
                (hasNotTipoImputacion || !pos.tipoImputacion.Id)
            ) {
                return;
            }
            if (hasNotUnidadSeleccionada || !pos.unidadSeleccionada.Id) {
                this.tabsPosicionValidos.tabSubposiciones = false;
                return;
            }
            if (hasNotTareaSubcontratarObj) {
                this.tabsPosicionValidos.tabSubposiciones = false;
                return;
            }
            if (hasNotCuentaTd) {
                this.tabsPosicionValidos.tabSubposiciones = false;
                return;
            }
            if (!pos.precioBruto || pos.precioBruto.toString() == "" || typeof pos.precioBruto === "undefined" || pos.precioBruto.toString() == "0") {
                this.tabsPosicionValidos.tabSubposiciones = false;
                return;
            }
            if (hasNotCuentaMayor || !pos.cuentaMayor.Id) {
                this.tabsPosicionValidos.tabSubposiciones = false;
                return;
            }
            if (hasNotTipoImputacion || !pos.tipoImputacion.Id) {
                this.tabsPosicionValidos.tabSubposiciones = false;
                return;
            }
        });
    }

    public validateDireccionEntrega() {
        this.tabsPosicionValidos.tabDireccionEntrega = true;
        
        if (!this.calleEntrega || this.calleEntrega == "" || typeof this.calleEntrega === "undefined")
        {
          this.tabsPosicionValidos.tabDireccionEntrega = false;
          return
        }
        //se comentó la validacion de el telefono
        // if (!this.numeroEntrega || this.numeroEntrega == "" || typeof this.numeroEntrega === "undefined")
        // {
        //   this.tabsPosicionValidos.tabDireccionEntrega = false;
        //   return
        // }
    }

    public validateImputaciones() {
        let hasTipoImputacion = typeof this.tipoImputacion != "undefined" && this.tipoImputacion;
        if (hasTipoImputacion)
        {        
            if (typeof this.valorImputacion === "undefined" || typeof this.cuentaMayor === "undefined")
            {
                this.tabsPosicionValidos.tabImputacion = false;
                return;
            }
        }
        
        this.tabsPosicionValidos.tabImputacion = true;
    }

    public validateProveedor() {
        this.tabsPosicionValidos.tabProveedor = true;
    }

    public validateDatosPosicion() {
        this.tabsPosicionValidos.tabDatosPosicion = true;
        
        if (typeof this.selectGrupoCompras === "undefined" || typeof this.selectArticuloCompras === "undefined")
        {
          this.tabsPosicionValidos.tabDatosPosicion = false;
          return;
        }
        if (!this.selectSolicitanteCompras || this.selectSolicitanteCompras == "" || typeof this.selectSolicitanteCompras === "undefined")
        {
          this.tabsPosicionValidos.tabDatosPosicion = false;
          return;
        }

        if (this.esTipoPosicionMaterial) {
            if (!this.textoSuministro || this.textoSuministro == "" || typeof this.textoSuministro === "undefined")
            {
              this.tabsPosicionValidos.tabDatosPosicion = false;
              return;
            }
        }
       
    }

    public validateFechas() {
        if (typeof this.fechaEntregaServicio === "undefined" || typeof this.plazoDeEntrega === "undefined" || this.plazoDeEntrega.toString() == "")
        {
          this.tabsPosicionValidos.tabFechas = false;
          return;
        }
    
        this.tabsPosicionValidos.tabFechas = true
    }

    public validatePosicion(tipoSolpSap: EnumTipoSolpSap) {
        this.tabsPosicionValidos.tabPosiciones = true;

        if (!this.selectCentroEntrega || typeof this.selectCentroEntrega === "undefined" || this.selectCentroEntrega == undefined)
        {
            this.tabsPosicionValidos.tabPosiciones = false;
            return;
        }

        if (tipoSolpSap != EnumTipoSolpSap.Mantenimiento) {
            if (!this.selectAlmacenEntrega || typeof this.selectAlmacenEntrega === "undefined" || typeof this.selectAlmacenEntrega === undefined) {
                this.tabsPosicionValidos.tabPosiciones = false;
                return;
            }
        }

        if (!this.tareaSubcontratarObj || typeof this.tareaSubcontratarObj === "undefined" || typeof this.tareaSubcontratarObj === undefined)
        {
            this.tabsPosicionValidos.tabPosiciones = false;
            return;
        }

        if (this.esTipoPosicionServicio) {
            if (!this.tipoImputacion || typeof this.tipoImputacion === "undefined" || this.tipoImputacion === undefined )
            {
                this.tabsPosicionValidos.tabPosiciones = false;
                return;
            }
        }

        if (this.esTipoPosicionMaterial || this.esTipoPosicionServicio) {
            if (!this.monedaSeleccionada || typeof this.monedaSeleccionada === "undefined" || typeof this.monedaSeleccionada === undefined || !this.monedaSeleccionada.Id)
            {
                this.tabsPosicionValidos.tabPosiciones = false;
                return;
            }
        }

        if (this.esTipoPosicionMaterial) {
            if (!this.unidadSeleccionada || this.unidadSeleccionada == "" || typeof this.unidadSeleccionada === "undefined" || typeof this.unidadSeleccionada === undefined || !this.unidadSeleccionada.Id )
            {
                this.tabsPosicionValidos.tabPosiciones = false;
                return;
            }
            if (!this.cuentaTd || this.cuentaTd == "" || typeof this.cuentaTd === "undefined" || typeof this.cuentaTd === undefined)
            {
                this.tabsPosicionValidos.tabPosiciones = false;
                return;
            }
            if (!this.precioBruto || this.precioBruto == "" || typeof this.precioBruto === "undefined" || this.precioBruto == "0" || typeof this.precioBruto === undefined)
            {
                this.tabsPosicionValidos.tabPosiciones = false;
                return;
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