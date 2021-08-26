import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ListBaseComponent } from './../../../common/base-components/list-base-component'
import { SessionDataService } from './../../../common/services/SessionDataService';
import { SecurityService } from './../../../common/services/SecurityService';
import { NavService } from './../../../common/services/NavService';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { ComprasService } from '../../compras.service';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { ModalService } from './../../../common/services/ModalService';
import { Solp } from '../../Solp';
import { SubPosicionViewModel } from './subPosicionViewModel';
import { EnumTipoImputacion } from '../../enum-tipo-imputacion'
import { EnumColumnaSubPosicion } from '../../enum-columna-subPosiciones'
import { ConfirmationService } from 'primeng/api';
import { type } from 'jquery';


@Component({
    selector: 'subPosicion',
    templateUrl: `subPosicion.component.html`,
    styleUrls: ['../../compras.component.css'],
})
export class SubPosicionComponent extends ListBaseComponent {

    @Input('model')
    protected model: Solp;

    @Input('locale')
    protected locale: any;

    @Input('combos') 
    protected combos:any;

    tituloColumnaTipoDeImputacion: string;
    listadoPosicionActul = Array<SubPosicionViewModel>();
    enumTipoImputacion: typeof EnumTipoImputacion = EnumTipoImputacion;
    enumColumnaSubPosicion: typeof EnumColumnaSubPosicion = EnumColumnaSubPosicion;
    total: number = 0;
    unidades: any[];

    tablaAFiltrar: any;
    autocomplete: any[];
    autocompletePaste: {Tabla:string, CodigoSap:string}[] = [];

    // array de columnas en la grilla
    // se utiliza esta array para luego cargar las posiciones dinamicamente segun la informacion del clipboard
    columnasGrilla: any = [
            { nombre: "codigoServicio", tipo: "codigoSap", tabla:"CodigoServicioSap" }, 
            { nombre: "tareaSubcontratar", tipo: "string" }, 
            { nombre: "cuentaTd", tipo: "numerico" }, 
            { nombre: "unidadMedida", tipo: "combo" }, 
            { nombre: "precioBruto", tipo: "decimal" }, 
            { nombre: "cuentaMayor", tipo: "codigoSap", tabla:"CuentasSolpSap" }, 
            { nombre: "tipoImputacion", tipo: "codigoSap" }];

    //variable para verificar si la posicion no fue dada de alta con los datos minimos
    posicionInvalida: boolean = false;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, private formBuilder: FormBuilder, protected router: Router,  private confirmationService: ConfirmationService
    ) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

    }

    setTabs() {
        this.setMenuSeccionTab("Sub Posiciones", "Sub Posiciones");
    }

    ngOnInit() {
        this.setTabs();

        // let primeraPosicion = this.model.posiciones[0]
        this.listadoPosicionActul = this.model.posicionActual.listadoSubPosiciones;
        // this.model.posicionActual = primeraPosicion;

        if(this.listadoPosicionActul.length == 0){
            this.nuevaPosicion(null);
        }

        this.validarDatosMinimosPosicionActual();
        this.calcularTotalSubPosicion();
        this.actualizarTipoDeImputacion();
    }

    validarDatosMinimosPosicionActual(): void {
        if ((this.model.posicionActual.textoGenerico == undefined || this.model.posicionActual.textoGenerico == "")
            || (this.model.posicionActual.tipoImputacion == undefined || this.model.posicionActual.tipoImputacion == "")) {
            this.posicionInvalida = true;
        }
        else
            this.posicionInvalida = false;
    }

    cambiarSubPosicion(): void {
        this.listadoPosicionActul = this.model.posicionActual.listadoSubPosiciones;
        this.validarDatosMinimosPosicionActual();
        this.actualizarTipoDeImputacion();
        this.calcularTotalSubPosicion();
    }

    actualizarTipoDeImputacion(): void {
        switch (this.model.posicionActual.tipoImputacion) {
            case this.enumTipoImputacion.CentroDeCosto:
                this.tituloColumnaTipoDeImputacion = "Centro de costo";
                this.tablaAFiltrar = 'CecoSolpSap';
                break;
            case this.enumTipoImputacion.OrdenDeOt:
                this.tituloColumnaTipoDeImputacion = "Orden de OT";
                this.tablaAFiltrar = 'OrdenSolpSap';
                break;
            case this.enumTipoImputacion.OrdenInversion:
                this.tituloColumnaTipoDeImputacion = "Orden de inversión"
                this.tablaAFiltrar = 'OrdenSolpSap';
                break;
            case this.enumTipoImputacion.Siniestro:
                this.tituloColumnaTipoDeImputacion = "Siniestro / Centro de beneficio"
                this.tablaAFiltrar = '';
                break;
        }
    }

    nuevaPosicion(rowSeleccionada: any): void {
        if(rowSeleccionada){
            let ultimoRegistroEnListado = this.listadoPosicionActul[this.listadoPosicionActul.length - 1]
            if (ultimoRegistroEnListado.id == rowSeleccionada.data.id) {
                ultimoRegistroEnListado.seleccionado = true;
                this.listadoPosicionActul.push(new SubPosicionViewModel(ultimoRegistroEnListado.subPosicion + 1));
            }
        }else{
            this.listadoPosicionActul.push(new SubPosicionViewModel(1));
        }
    }

    eliminarSubposiciones(): void {
        if (this.listadoPosicionActul.length > 0) {
            
            let subPosicionesAgregadas = this.listadoPosicionActul.filter(x => !x.eliminar);
            subPosicionesAgregadas.forEach((element, index, array) => {
                element.subPosicion = index + 1;
            });
            this.listadoPosicionActul = subPosicionesAgregadas;
            this.model.posicionActual.listadoSubPosiciones = this.listadoPosicionActul;
            this.calcularTotalSubPosicion();
        }

        if(this.listadoPosicionActul.length == 0){
            this.nuevaPosicion(null);
            this.model.posicionActual.listadoSubPosiciones = this.listadoPosicionActul;
        }
    }

    eliminarSubPosicion()
    {
        this.confirmationService.confirm({
            message: '¿Está seguro que desea eliminar la subposición?',
            accept: () => {              
                    this.eliminarSubposiciones();                  
            },
            reject: () => {
                
            }
        });
    }

    onPaste(evento: any, indexColumna: number, rowIndex: number): void {

        let datos = evento.clipboardData.getData("text");
        if (!datos.includes("Recuperando datos")) {
            this.spinnerComponent.showIt();
            //separo la informacion por filas 
            let filas = datos.split("\n");
            filas.forEach(element => {
                evento.preventDefault();
                //separo la informacion por columnas 
                let columnas = element.split("\t")
                this.SetValuesForColumns(indexColumna, columnas, rowIndex, this.listadoPosicionActul);
                rowIndex++;
            });
            this.calcularTotalSubPosicion();
            this.completarCodigosSapOnPaste();
        }
    }

    /**
    Metodo Auxuliar para cargar una fila dinamicamente
        indexColumn : posicion de la columna en la grilla coincide con el array columnasGrilla
    columnas : array de valores del clipboard que se obtiene de cada columna
    */
    SetValuesForColumns(indexColumn: number, columnas: any, rowIndex: number, listado: SubPosicionViewModel[]): void {
        let esEdicion = false;

        //piso las filas que tengan datos y si no tengo mas filas creo nuevas
        let tamañoArray = this.listadoPosicionActul.length;
        let fila: SubPosicionViewModel;
        if (rowIndex < tamañoArray) {
            fila = listado[rowIndex];
            esEdicion = true;
        } else {
            fila = new SubPosicionViewModel(this.listadoPosicionActul.length);
        }       

        for (let index = 0; index < columnas.length && index < this.columnasGrilla.length; index++) {
            let columna = this.columnasGrilla[indexColumn + index]
            switch (columna.tipo) {
                case "numerico":
                    let valor = Number.parseInt(columnas[index]);
                    fila[columna.nombre] = Number.isNaN(valor) ?  undefined:valor;
                    break;
                case "decimal":
                    let valorDecimal = Number.parseFloat(columnas[index]);
                    fila[columna.nombre] = Number.isNaN(valorDecimal) ?  undefined:valorDecimal;
                    break;
                case "combo":
                    let seleccion = this.combos.Unidades.find( x => x.Codigo.toLowerCase() == columnas[index].toLowerCase()) || {};
                    fila.unidadSeleccionada = seleccion;
                    break;
                case "codigoSap":
                    fila[columna.nombre] = { CodigoSap: columnas[index] }
       
                    this.autocompletePaste.push({
                        CodigoSap: columnas[index],
                        Tabla: columna.tabla || this.tablaAFiltrar
                    });
                    break;
                default:
                    fila[columna.nombre] = columnas[index];
                    break;
            }
        }

        if (!esEdicion) {
            listado.push(fila);
            listado.push(new SubPosicionViewModel(this.listadoPosicionActul.length));
        }
    }

    calcularTotalSubPosicion() {
        this.total = 1;
        this.listadoPosicionActul.forEach(posicion => {
            this.total = this.total + (+posicion.precioBruto);
        });
    }

    buscarCombo(event, type){
        switch (type) {
            case 'UNIDAD MEDIDA':
                this.unidades = this.combos.Unidades.filter(x=> x.Descripcion.toLowerCase().includes(event.query.toLowerCase()));
                break;   
                
            default:
                break;
        }
    }

    completarCodigosSapOnPaste(){
        try{
            this.subscription = this.service.obtenerDatosPorCodigosSap(this.autocompletePaste).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else { 
                        if(result){
                            this.listadoPosicionActul.forEach(c => {
                                if(c.codigoServicio && c.codigoServicio.CodigoSap && !c.codigoServicio.Codigo){
                                    c.codigoServicio = result.find(x=>x.Tabla == 'CodigoServicioSap' && x.CodigoSap == c.codigoServicio.CodigoSap);
                                }

                                if(c.cuentaMayor && c.cuentaMayor.CodigoSap && !c.cuentaMayor.Codigo){
                                    c.cuentaMayor = result.find(x=>x.Tabla == 'CuentasSolpSap' && x.CodigoSap == c.cuentaMayor.CodigoSap);
                                }

                                if(c.tipoImputacion && c.tipoImputacion.CodigoSap && !c.tipoImputacion.Codigo){
                                    c.tipoImputacion = result.find(x=>x.Tabla == this.tablaAFiltrar && x.CodigoSap == c.tipoImputacion.CodigoSap);
                                }
                            });
                        }
                    }

                    this.spinnerComponent.hideIt();
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.spinnerComponent.hideIt();
                }
            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.spinnerComponent.hideIt();
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    autocompleteSap(event, tablaAFiltrar){
        try{
            this.subscription = this.service.autocompleteSap(tablaAFiltrar || this.tablaAFiltrar, event.query.toLowerCase()).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else { 
                        this.autocomplete = result;
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.spinnerComponent.hideIt();
                }
            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

}
