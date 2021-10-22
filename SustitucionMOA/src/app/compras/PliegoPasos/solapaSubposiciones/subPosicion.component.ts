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
import { ThrowStmt } from '@angular/compiler';


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
    protected combos: any;

    tituloColumnaTipoDeImputacion: string;
    listadoPosicionActul = Array<SubPosicionViewModel>();
    enumTipoImputacion: typeof EnumTipoImputacion = EnumTipoImputacion;
    enumColumnaSubPosicion: typeof EnumColumnaSubPosicion = EnumColumnaSubPosicion;
    total: number = 0;
    unidades: any[];

    tablaAFiltrar: any;
    autocomplete: any[];
    autocompletePaste: { Tabla: string, CodigoSap: string }[] = [];
    arraryErrores: any = new Array<{ id: number, text: string }>();

    // array de columnas en la grilla
    // se utiliza esta array para luego cargar las posiciones dinamicamente segun la informacion del clipboard
    columnasGrilla: any = [
        { nombre: "codigoServicio", tipo: "codigoSap", tabla: "CodigoServicioSap" },
        { nombre: "tareaSubcontratar", tipo: "tarea" },
        { nombre: "cuentaTd", tipo: "numerico" },
        { nombre: "unidadMedida", tipo: "combo" },
        { nombre: "precioBruto", tipo: "decimal" },
        { nombre: "cuentaMayor", tipo: "codigoSap", tabla: "CuentasSolpSap" },
        { nombre: "tipoImputacion", tipo: "codigoSap" }];

    //variable para verificar si la posicion no fue dada de alta con los datos minimos
    posicionInvalida: boolean = false;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, private formBuilder: FormBuilder, protected router: Router, private confirmationService: ConfirmationService
    ) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

    }

    camposObligatorios: any[] = [
        { campo: 'codigoServicio', esObligatorio: false, esFijo: true },
        { campo: 'tareaSubcontratar', esObligatorio: true, esFijo: true },
        { campo: 'cuentaTd', esObligatorio: true, esFijo: true },
        { campo: 'unidadMedida', esObligatorio: true, esFijo: true },
        { campo: 'precioBruto', esObligatorio: true, esFijo: true },
        { campo: 'cuentaMayor', esObligatorio: true, esFijo: true },
        { campo: 'tipoImputacion', esObligatorio: true, esFijo: true },
        { campo: 'servicio', esObligatorio: true, esFijo: true },
        { campo: 'centroDeCosto', esObligatorio: false, esFijo: true },
        { campo: 'ordenDeOt', esObligatorio: false, esFijo: true },
        { campo: 'ordenDeInversion', esObligatorio: false, esFijo: true },
        { campo: 'siniestroBeneficio', esObligatorio: false, esFijo: true },
        { campo: 'tipoImputacion', esObligatorio: true, esFijo: true },
        { campo: 'textoGenerico', esObligatorio: true, esFijo: true },
        { campo: 'fechaEntregaServicio', esObligatorio: true, esFijo: false },
        { campo: 'fechaDeLiberacion', esObligatorio: false, esFijo: false },
        { campo: 'plazoDeEntrega', esObligatorio: true, esFijo: true },
        { campo: 'concluido', esObligatorio: false, esFijo: true },
        { campo: 'indiceFijacion', esObligatorio: false, esFijo: true },
        { campo: 'selectCentroEntrega', esObligatorio: false, esFijo: false },
        { campo: 'nombreEntrega', esObligatorio: false, esFijo: true },
        { campo: 'codigoPostalEntrega', esObligatorio: false, esFijo: true },
        { campo: 'selectAlmacenEntrega', esObligatorio: false, esFijo: false },
        { campo: 'calleEntrega', esObligatorio: true, esFijo: true },
        { campo: 'paisEntrega', esObligatorio: false, esFijo: true },
        { campo: 'numeroEntrega', esObligatorio: true, esFijo: true },
        { campo: 'selectGrupoCompras', esObligatorio: false, esFijo: false },
        { campo: 'selectArticuloCompras', esObligatorio: true, esFijo: true },
        { campo: 'selectSolicitanteCompras', esObligatorio: true, esFijo: true },
        { campo: 'necesidadCompras', esObligatorio: false, esFijo: true },
        { campo: 'rubroElectrico', esObligatorio: false, esFijo: true },
        { campo: 'rubroCivil', esObligatorio: false, esFijo: true },
        { campo: 'rubroIngenieria', esObligatorio: false, esFijo: true },
        { campo: 'rubroMecanico', esObligatorio: false, esFijo: true },
        { campo: 'rubroConsultoria', esObligatorio: false, esFijo: true },
        { campo: 'proveedoresValidos', esObligatorio: false, esFijo: true },
        { campo: 'proveedoresInvalidos', esObligatorio: false, esFijo: true },
        { campo: 'proveedoresNoSugeridos', esObligatorio: false, esFijo: true },
        { campo: 'selectMonedaCompras', esObligatorio: true, esFijo: true },
    ];

    actualizarCamposObligatorios(claseDocumento) {
        //reset de obligatorios configurables
        this.camposObligatorios.forEach(c => {
            if (!c.esFijo)
                c.esObligatorio = false;
        });

        let camposObligatoriosFiltrados = this.combos.CamposObligatoriosCabeceraSolp.filter(x => x.ClaseDocumentoCodigo == claseDocumento.Codigo);

        camposObligatoriosFiltrados.forEach(c => {
            this.camposObligatorios.find(x => x.campo == c.Codigo).esObligatorio = true;
        });
    }

    validarErrorCustom(subposicion: any, valor: any, campoAValidar: string) {
        return ((this.camposObligatorios.find(x => x.campo == campoAValidar).esObligatorio) && valor.toString().length == 0);  
    }

    validarConNoNulo(subposicion: any, valor: any, campoAValidar: string){    
        if(subposicion.tareaSubcontratar === ""){
            return false
        }  
        
        return valor == null || 
            ((this.camposObligatorios.find(x => x.campo == campoAValidar).esObligatorio) 
            && valor.toString().length == 0);
    }
          
    validarPrecioBruto(subposicion: any, valor: any, campoAValidar: string){
        if(subposicion.tareaSubcontratar === ""){
            return false
        }  
        
        return valor == null ||
            ((this.camposObligatorios.find(x => x.campo == campoAValidar).esObligatorio) 
            && (valor.toString().length == 0 || valor === 0));
    }




    setTabs() {
        this.setMenuSeccionTab("Sub Posiciones", "Sub Posiciones");
    }

    ngOnInit() {
        this.setTabs();
        this.listadoPosicionActul = this.model.posicionActual.listadoSubPosiciones;
        // this.model.posicionActual = primeraPosicion;

        if (this.listadoPosicionActul.length == 0) {
            this.nuevaPosicion(null);
        }

        this.validarDatosMinimosPosicionActual();
        this.calcularTotalSubPosicion();
        this.actualizarTipoDeImputacion();
        this.actualizarCamposObligatorios(this.model.selectClaseDocumento);

    }



    validarDatosMinimosPosicionActual(): void {
        if ((this.model.posicionActual.textoGenerico == undefined || this.model.posicionActual.textoGenerico == "")
            || (this.model.posicionActual.tipoImputacion == undefined || this.model.posicionActual.tipoImputacion == "")) {
            this.posicionInvalida = true;
        } else
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
                this.tablaAFiltrar = 'CentroBeneficio';
                break;
        }
    }

    nuevaPosicion(rowSeleccionada: any): void {
        if (rowSeleccionada) {
            let ultimoRegistroEnListado = this.listadoPosicionActul[this.listadoPosicionActul.length - 1]
            if (ultimoRegistroEnListado.id == rowSeleccionada.data.id) {
                ultimoRegistroEnListado.seleccionado = true;
                this.listadoPosicionActul.push(new SubPosicionViewModel(ultimoRegistroEnListado.subPosicion + 1));
            }
        } else {
            this.listadoPosicionActul.push(new SubPosicionViewModel(1));
        }
    }

    eliminarSubposiciones(indice = -1): void {
        if (this.listadoPosicionActul.length > 0) {
            if (indice >= 0) {
                this.listadoPosicionActul.splice(indice, 1);
            } else {
                this.listadoPosicionActul = [];
            }
            this.model.posicionActual.listadoSubPosiciones = this.listadoPosicionActul;
            this.calcularTotalSubPosicion();
        }

        if (this.listadoPosicionActul.length == 0) {
            this.nuevaPosicion(null);
            this.model.posicionActual.listadoSubPosiciones = this.listadoPosicionActul;
        }
    }

    eliminarSubPosicionIndividual(indice: number): void {
        this.confirmationService.confirm({
            message: '¿Está seguro que desea eliminar la subposición?',
            accept: () => {
                this.eliminarSubposiciones(indice);
            },
            reject: () => {

            }
        });
    }

    eliminarSubPosicion() {
        this.confirmationService.confirm({
            message: '¿Está seguro que desea eliminar todas las subposiciones?',
            accept: () => {
                this.eliminarSubposiciones();
            },
            reject: () => {

            }
        });
    }

    onPaste(evento: any, indexColumna: number, rowIndex: number, dt): void {

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
            this.endEditCell(dt);
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
                    fila[columna.nombre] = Number.isNaN(valor) ? undefined : valor;
                    break;
                case "decimal":
                    let valorDecimal = Number.parseFloat(columnas[index]);
                    fila[columna.nombre] = Number.isNaN(valorDecimal) ? undefined : valorDecimal;
                    break;
                case "combo":
                    let seleccion = this.combos.Unidades.find(x => x.Codigo.toLowerCase() == columnas[index].toLowerCase()) || {};
                    fila.unidadSeleccionada = seleccion;
                    break;
                case "codigoSap":
                    fila[columna.nombre] = { CodigoSap: columnas[index] }

                    this.autocompletePaste.push({
                        CodigoSap: columnas[index],
                        Tabla: columna.tabla || this.tablaAFiltrar
                    });
                    break;
                case "tarea":
                    fila[columna.nombre] = columnas[index];
                    fila.tareaSubcontratarObj = { Descripcion: columnas[index] }
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

    buscarCombo(event, type) {
        switch (type) {
            case 'UNIDAD MEDIDA':
                this.unidades = this.combos.Unidades.filter(x => x.Descripcion.toLowerCase().includes(event.query.toLowerCase()));
                break;

            default:
                break;
        }
    }

    completarCodigosSapOnPaste() {
        try {
            this.subscription = this.service.obtenerDatosPorCodigosSap(this.autocompletePaste).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        if (result) {
                            this.listadoPosicionActul.forEach(c => {
                                if (c.codigoServicio && c.codigoServicio.CodigoSap && !c.codigoServicio.Codigo) {
                                    c.codigoServicio = result.find(x => x.Tabla == 'CodigoServicioSap' && x.CodigoSap == c.codigoServicio.CodigoSap);
                                    c.tareaSubcontratarObj = { ...c.codigoServicio };
                                    c.tareaSubcontratar = c.codigoServicio.Descripcion;
                                }

                                if (c.cuentaMayor && c.cuentaMayor.CodigoSap && !c.cuentaMayor.Codigo) {
                                    c.cuentaMayor = result.find(x => x.Tabla == 'CuentasSolpSap' && x.CodigoSap == c.cuentaMayor.CodigoSap);
                                }

                                if (c.tipoImputacion && c.tipoImputacion.CodigoSap && !c.tipoImputacion.Codigo) {
                                    c.tipoImputacion = result.find(x => x.Tabla == this.tablaAFiltrar && x.CodigoSap == c.tipoImputacion.CodigoSap);
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

    autocompleteSap(event, tablaAFiltrar, soloDescripcion = false) {
        try {
            this.subscription = this.service.autocompleteSap(tablaAFiltrar || this.tablaAFiltrar, event.query.toLowerCase()).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.autocomplete = soloDescripcion ? result.map(x => x.Descripcion.trim()) : result;
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

    onSelectServicio(posicion: SubPosicionViewModel, dt) {

        console.log(posicion);
        console.log(dt);
        posicion.tareaSubcontratar = posicion.codigoServicio.Descripcion;
        posicion.tareaSubcontratarObj = { ...posicion.codigoServicio };
        this.endEditCell(dt);
    }

    onSelectTarea(posicion: SubPosicionViewModel, dt) {
        posicion.tareaSubcontratar = posicion.tareaSubcontratarObj.Descripcion;
        posicion.codigoServicio = { ...posicion.tareaSubcontratarObj };
        this.endEditCell(dt);
    }

    onBlueTarea(event, posicion: SubPosicionViewModel) {
        posicion.tareaSubcontratar = event.target.value;
        posicion.tareaSubcontratarObj = { Descripcion: event.target.value }
    }

    endEditCell(dt) {
        dt.closeCellEdit();
    }
}
