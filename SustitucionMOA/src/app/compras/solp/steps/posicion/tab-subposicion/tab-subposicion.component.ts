import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService, Message, SelectItem } from 'primeng/api';
import { ListBaseComponent } from '../../../../../common/base-components/list-base-component';
import { FloatMsgService } from '../../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../../common/services/ModalService';
import { NavService } from '../../../../../common/services/NavService';
import { SecurityService } from '../../../../../common/services/SecurityService';
import { SessionDataService } from '../../../../../common/services/SessionDataService';
import { ComprasService } from '../../../../compras.service';
import { EnumColumnaSubPosicion } from '../../../../enum-columna-subPosiciones';
import { EnumTipoImputacion } from '../../../../enum-tipo-imputacion';
import { SubPosicionViewModel } from './sub-posicion-view-model';
import { Solp } from '../../../solp';
import { SolpPosicion } from '../../../solp-posicion';

@Component({
    selector: 'app-tab-subposicion',
    templateUrl: './tab-subposicion.component.html',
    styleUrls: [
        '../../../../compras.component.css',
        './tab-subposicion.component.css'
    ]
})
export class TabSubposicionComponent extends ListBaseComponent {

    @Input('model')
    protected model: Solp;

    @Input('locale')
    protected locale: any;

    @Input('combos')
    protected combos: any;

    @Input('listadoSubposiciones')
    protected listadoSubposiciones: Array<SubPosicionViewModel>;

    @Input('posicion')
    protected posicion: SolpPosicion;

    @Input('imputacion')
    protected imputacion: any;

    @Output() eliminarPosicion = new EventEmitter<any>();

    tituloColumnaTipoDeImputacion: string;
    enumTipoImputacion: typeof EnumTipoImputacion = EnumTipoImputacion;
    enumColumnaSubPosicion: typeof EnumColumnaSubPosicion = EnumColumnaSubPosicion;
    total: number = 0;
    unidades: any[];

    tablaAFiltrar: any;
    autocomplete: any[];
    autocompleteServiciosSolp: any[];
    autocompletePaste: { Tabla: string, CodigoSap: string }[] = [];
    autocompleteServiciosSolpPaste: string[] = [];
    arraryErrores: any = new Array<{ id: number, text: string }>();

    tipoImputacion: any[];
    imputacionSeleccionada: any;

    monedaCompras: SelectItem[];
    monedaSeleccionada: any;

    //variable para verificar si la posicion no fue dada de alta con los datos minimos
    posicionInvalida: boolean = false;

    mensajesEncabezado: Message[] = [];

    // array de columnas en la grilla
    // se utiliza esta array para luego cargar las posiciones dinamicamente segun la informacion del clipboard
    columnasGrilla: any = [
        { nombre: "codigoServicio", tipo: "codigoServicioSolp" },
        { nombre: "tareaSubcontratar", tipo: "tarea" },
        { nombre: "cuentaTd", tipo: "numerico" },
        { nombre: "unidadMedida", tipo: "combo" },
        { nombre: "precioBruto", tipo: "decimal" },
        { nombre: "valorNeto", tipo: "numerico" },
        { nombre: "cuentaMayor", tipo: "codigoSap", tabla: "CuentasSolpSap" },
        { nombre: "tipoImputacion", tipo: "codigoSap" }
    ];

    camposObligatorios: any[] = [
        { campo: 'codigoServicio', esObligatorio: false, esFijo: true },
        { campo: 'tareaSubcontratar', esObligatorio: true, esFijo: true },
        { campo: 'cuentaTd', esObligatorio: true, esFijo: true },
        { campo: 'unidadMedida', esObligatorio: true, esFijo: true },
        { campo: 'precioBruto', esObligatorio: true, esFijo: true },
        { nombre: "valorNeto", tipo: "numerico" },
        { campo: 'cuentaMayor', esObligatorio: true, esFijo: true },
        { campo: 'tipoImputacion', esObligatorio: true, esFijo: true },
        { campo: 'servicio', esObligatorio: true, esFijo: true },
        { campo: 'centroDeCosto', esObligatorio: false, esFijo: true },
        { campo: 'ordenDeOt', esObligatorio: false, esFijo: true },
        { campo: 'ordenDeInversion', esObligatorio: false, esFijo: true },
        { campo: 'siniestroBeneficio', esObligatorio: false, esFijo: true }
    ];

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router,
        private confirmationService: ConfirmationService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    validarErrorCustom(subposicion: any, valor: any, campoAValidar: string) {
        return ((this.camposObligatorios.find(x => x.campo == campoAValidar).esObligatorio) && valor.toString().length == 0);
    }

    validarConNoNulo(subposicion: any, valor: any, campoAValidar: string) {
        if (subposicion.tareaSubcontratar === "") {
            return false
        }

        return valor == null ||
            ((this.camposObligatorios.find(x => x.campo == campoAValidar).esObligatorio)
                && valor.toString().length == 0);
    }

    validarPrecioBruto(subposicion: any, valor: any, campoAValidar: string) {
        if (subposicion.tareaSubcontratar === "") {
            return false
        }

        return valor == null ||
            ((this.camposObligatorios.find(x => x.campo == campoAValidar).esObligatorio)
                && (valor.toString().length == 0 || valor === 0));
    }

    ngOnInit(): void {
        this.unidades = this.combos.Unidades;
        this.actualizarPosicion();
        this.posicion.validateSubposiciones();
    }

    //Como el componente tiene inputs angular llama a esta funcion antes del onInit, entonces el ngOnInit no es necesario.
    ngOnChanges() {
        this.actualizarPosicion();

        if (this.model.imputacionPorDefecto && !this.model.posicionActual.imputacionSeleccionada)
            this.model.posicionActual.imputacionSeleccionada = this.combos.TipoImputacion.find(x => x.Codigo == this.model.imputacionPorDefecto)

        this.imputacionSeleccionada = this.tipoImputacion;

        this.actualizarTipoDeImputacion();
    }

    public ngOnDestroy(): void {
        super.ngOnDestroy();
    }

    actualizarPosicion() {
        if (this.listadoSubposiciones.length == 0) {
            this.nuevaPosicion(null);
        }

        // this.validarDatosMinimosPosicionActual();
        this.calcularTotalSubPosicion();
        this.actualizarTipoDeImputacion();

        this.actualizarCamposObligatorios(this.model.selectClaseDocumento);
        this.model.posiciones = this.sortPosiciones();

        if (this.model.vincularAPliego) {
            this.mensajesEncabezado.push({ severity: 'warn', summary: '', detail: 'No es posible editar esta pantalla desde la plataforma. Para editar dirijase a SAP' });
        }
        else {
            this.mensajesEncabezado = [];
        }
    }

    actualizarCamposObligatorios(claseDocumento) {
        //reset de obligatorios configurables
        this.camposObligatorios.forEach(c => {
            if (!c.esFijo)
                c.esObligatorio = false;
        });

        let camposObligatoriosFiltrados = this.combos.CamposObligatoriosCabeceraSolp.filter(x => x.ClaseDocumentoCodigo == claseDocumento.Codigo);

        camposObligatoriosFiltrados.forEach(c => {
            let mandatoryField = this.camposObligatorios.find(x => x.campo == c.Codigo);
            if (mandatoryField) {
                mandatoryField.esObligatorio = true;
            }
        });
    }

    cambiarSubPosicion(): void {
        this.listadoSubposiciones = this.model.posicionActual.listadoSubPosiciones;

        // this.validarDatosMinimosPosicionActual();
        this.actualizarTipoDeImputacion();
        this.calcularTotalSubPosicion();
    }

    actualizarTipoDeImputacion(): void {
        if (this.posicion.tipoImputacion) {
            switch (this.posicion.tipoImputacion.Codigo) {
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
    }

    nuevaPosicion(rowSeleccionada: any): void {
        if (!this.model.vincularAPliego) {
            if (rowSeleccionada) {
                let ultimoRegistroEnListado = this.listadoSubposiciones[this.listadoSubposiciones.length - 1]
                if (ultimoRegistroEnListado.id == rowSeleccionada.data.id) {
                    ultimoRegistroEnListado.seleccionado = true;
                    this.listadoSubposiciones.push(new SubPosicionViewModel(ultimoRegistroEnListado.subPosicion + 1));
                }
            } else {
                this.listadoSubposiciones.push(new SubPosicionViewModel(1));
            }
        }
    }

    eliminarSubposiciones(indice = -1): void {
        if (this.listadoSubposiciones.length > 0) {
            if (indice >= 0) {
                this.listadoSubposiciones.splice(indice, 1);
                this.reEnumerarSubposiciones(this.listadoSubposiciones);
            } else {
                this.listadoSubposiciones = [];
            }
            this.posicion.listadoSubPosiciones = this.listadoSubposiciones;
            this.calcularTotalSubPosicion();
        }

        if (this.listadoSubposiciones.length == 0) {
            this.nuevaPosicion(null);
            this.posicion.listadoSubPosiciones = this.listadoSubposiciones;
        }
    }

    reEnumerarSubposiciones(listadoSubposiciones: Array<SubPosicionViewModel>) {
        for (let i = 0; i < listadoSubposiciones.length; i++) {
            listadoSubposiciones[i].subPosicion = i + 1;
        }
    }

    eliminarSubPosicionIndividual(indice: number): void {
        this.confirmationService.confirm({
            message: '¿Está seguro de que desea eliminar la subposición?',
            accept: () => {
                this.eliminarSubposiciones(indice);
            },
            reject: () => {

            }
        });
    }

    eliminarSubPosicion() {
        this.confirmationService.confirm({
            message: '¿Está seguro de que desea eliminar todas las subposiciones?',
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
                this.SetValuesForColumns(indexColumna, columnas, rowIndex, this.listadoSubposiciones);
                rowIndex++;
            });
            this.calcularTotalSubPosicion();
            this.completarCodigosSapOnPaste();
            this.completarServicioSolpOnPaste();
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
        let tamañoArray = this.listadoSubposiciones.length;
        let fila: SubPosicionViewModel;
        if (rowIndex < tamañoArray) {
            fila = listado[rowIndex];
            esEdicion = true;
        } else {
            fila = new SubPosicionViewModel(this.listadoSubposiciones.length);
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
                    var codigoSap = columnas[index].trim();

                    fila[columna.nombre] = { CodigoSap: codigoSap };

                    this.autocompletePaste.push({
                        CodigoSap: codigoSap,
                        Tabla: columna.tabla || this.tablaAFiltrar
                    });
                    break;
                case "codigoServicioSolp":
                    fila[columna.nombre] = { CodigoSap: columnas[index] };

                    this.autocompleteServiciosSolpPaste.push(columnas[index]);
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
            listado.push(new SubPosicionViewModel(this.listadoSubposiciones.length));
        }
    }

    calcularTotalSubPosicion() {
        this.posicion.calcularValorTotal();
        this.model.calcularValorTotalPorMoneda();
    }

    calcularValorNeto(subPosicion: SubPosicionViewModel): void {
        if (subPosicion != null && subPosicion != undefined) {
            subPosicion.calcularValorNeto();
        }
        this.posicion.calcularValorTotal();
        this.model.calcularValorTotalPorMoneda();
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
                            this.listadoSubposiciones.forEach(c => {
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

    completarServicioSolpOnPaste() {
        try {
            this.subscription = this.service.obtenerDatosPorCodigosSapServicioSolp(this.autocompleteServiciosSolpPaste).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        if (result && result.length > 0) {
                            this.listadoSubposiciones.forEach(c => {
                                if (c.codigoServicio && c.codigoServicio.CodigoSap) {
                                    var datos = result.find(x => x.Codigo == c.codigoServicio.CodigoSap);
                                    if (datos) {
                                        c.codigoServicio = datos;
                                        c.tareaSubcontratar = c.codigoServicio.Descripcion;

                                        var unidadSeleccionadaAux = this.combos.Unidades.find(x => x.Descripcion == c.codigoServicio.UnidadMedidaBase);
                                        if (unidadSeleccionadaAux) {
                                            c.unidadSeleccionada = unidadSeleccionadaAux;
                                            c.unidadMedida = unidadSeleccionadaAux.Descripcion;
                                        }
                                    }
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
            let filter = tablaAFiltrar || this.tablaAFiltrar;
            if (filter == 'OrdenSolpSap' && event.query.toLowerCase().length < 4) {
                this.autocomplete = [];
                return;
            }

            this.subscription = this.service.autocompleteSap(filter, event.query.toLowerCase()).subscribe(
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

    autocompleteServicioSolp(event) {
        try {
            this.subscription = this.service.autocompleteServicioSolp(event.query.toLowerCase()).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.autocompleteServiciosSolp = result;
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.spinnerComponent.hideIt();
                });
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    autocompleteCodigoServicioSolp(event) {
        try {
            this.subscription = this.service.autocompleteCodigoServicioSolp(event.query.toLowerCase()).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.autocompleteServiciosSolp = result;
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.spinnerComponent.hideIt();
                });
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    onSelectServicio(posicion: SubPosicionViewModel, dt) {
        posicion.tareaSubcontratar = posicion.codigoServicio.Descripcion;
        posicion.tareaSubcontratarObj = { ...posicion.codigoServicio };

        var unidadSeleccionadaAux = this.combos.Unidades.find(x => x.Descripcion == posicion.codigoServicio.UnidadMedidaBase);

        if (unidadSeleccionadaAux) {
            posicion.unidadSeleccionada = unidadSeleccionadaAux;
            posicion.unidadMedida = unidadSeleccionadaAux.Descripcion;
        }

        this.endEditCell(dt);
    }

    onSelectTarea(posicion: SubPosicionViewModel, dt) {
        posicion.tareaSubcontratar = posicion.tareaSubcontratarObj.Descripcion;
        posicion.codigoServicio = { ...posicion.tareaSubcontratarObj };

        var unidadSeleccionadaAux = this.combos.Unidades.find(x => x.Descripcion == posicion.codigoServicio.UnidadMedidaBase);
        posicion.unidadSeleccionada = unidadSeleccionadaAux;
        posicion.unidadMedida = unidadSeleccionadaAux.Descripcion;
        this.endEditCell(dt);
    }

    onBlurTarea(event, posicion: SubPosicionViewModel) {
        posicion.tareaSubcontratar = event.target.value;
        posicion.tareaSubcontratarObj = { Descripcion: event.target.value }
    }

    endEditCell(dt) {
        dt.closeCellEdit();
    }

    sortPosiciones() {
        var sortedPosiciones = this.model.posiciones.sort((p1, p2) => p1.numeroPosicion - p2.numeroPosicion);
        return sortedPosiciones;
    }

    eliminarSubPosicionSolpValidate(rowIndex: number) {
        if (this.listadoSubposiciones.length === 1) {
            this.confirmationService.confirm({
                key: 'validarEliminarSubPosicion',
                message: 'No se puede borrar la subposición, ya que no se permiten posiciones sin subposición',
                accept: () => {
                    //this.salir();
                    this.eliminarPosicion.emit(this.posicion);
                },
                reject: () => {
                }
            });
        } else if (this.listadoSubposiciones.length > 1) {
            this.eliminarSubPosicionIndividual(rowIndex);
        }
    }

    public get monedaPosicion(): string {

        let codigoMoneda = '';

        if (this.posicion.monedaSeleccionada) {
            codigoMoneda = this.posicion.monedaSeleccionada.Codigo;
        }
        return codigoMoneda;

    }

    clearCode(posicion: SubPosicionViewModel) {
        if (posicion.tareaSubcontratar != null) {
            posicion.codigoServicio = null;
        }
    }

    clearCode2(posicion: SubPosicionViewModel) {
        if (posicion.codigoServicio != null) {
            posicion.tareaSubcontratarObj = null;
            posicion.tareaSubcontratar = null;

        }
    }

    checkCode(posicion: SubPosicionViewModel) {
        if (posicion.codigoServicio == '' || posicion.codigoServicio == null) {
            posicion.tareaSubcontratarObj = null;

        }
    }

}