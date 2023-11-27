import { Component, Input, OnDestroy, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { ConfirmationService, SelectItem } from 'primeng/api';
import { MenuItem, Message } from 'primeng/components/common/api';
import { ListBaseComponent } from '../../../../common/base-components/list-base-component';
import { SessionDataService } from '../../../../common/services/SessionDataService';
import { SecurityService } from '../../../../common/services/SecurityService';
import { NavService } from '../../../../common/services/NavService';
import { FloatMsgService } from '../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../common/services/ModalService';
import { ComprasService } from '../../../compras.service'
import { SpinnerComponent } from '../../../../common/view-child/spinner/spinner.component';
import { ContratoMarcoComponent } from './contrato-marco/contrato-marco.component';
import { ValidadorPasoSolpService } from '../../../validadorPasoSolpService';
import { SubPosicionViewModel } from './tab-subposicion/sub-posicion-view-model';
import { EnumColumnaSubPosicion } from '../../../enum-columna-subPosiciones';
import { Solp } from '../../solp';
import { SolpPosicion } from '../../solp-posicion';
import { mergeMap, map, switchMap } from 'rxjs/operators';
import { from, Observable, of } from 'rxjs';
import { ObtenerContratoMarcoService } from './obtener-contrato-marco/obtener-contrato-marco.service';
import { ContratoMarco, ContratoMarcoSubposicion, ObtenerContratoMarco } from './obtener-contrato-marco/contrato-marco.model';

declare var $: any;

@Component({
    selector: 'cabecera',
    templateUrl: `cabecera.component.html`,
    styleUrls: [
        '../../../compras.component.css',
        './cabecera.component.css'
    ]
})
export class CabeceraComponent extends ListBaseComponent implements OnDestroy {

    @Input('combos')
    protected combos: any;

    @Input('model')
    protected model: Solp;

    @Input('locale')
    protected locale: any;

    @ViewChild("containerList")
    protected containerList: HTMLDivElement;

    @ViewChild(ContratoMarcoComponent)
    protected asociarContrato: ContratoMarcoComponent;

    @Input('imputacion')
    protected imputacion: any;

    @Input('moneda')
    protected moneda: any;

    @Input('esCreacionSolp')
    protected esCreacionSolp: boolean;

    @Input('getDatosUltimaSolp')
    protected datosUltimaSolp;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    //Posiciones
    posiciones: SelectItem[];
    selectPosicion: any;
    formularioPosicion: [FormGroup];
    formularioActual: FormGroup;
    validFormEliminarPosicion = true;
    arraryErrores: any = new Array<{ id: number, text: string }>();
    posicionSeleccionada: any;
    //Fuera de la tabla
    claseDocumento: SelectItem[];
    editarDocumento: boolean = false;
    disabled: boolean = true;
    concluido: boolean = false;

    //Variables de la grilla
    centroEntrega: SelectItem[];
    monedaCompras: SelectItem[];
    almacenEntrega: SelectItem[];
    unidades: any[];
    total: number = 0;

    //Autocompletes
    tablaAFiltrar: any;
    autocomplete: any[];
    autocompleteServiciosSolp: any[];
    autocompletePaste: { Tabla: string, CodigoSap: string }[] = [];
    autocompleteServiciosSolpPaste: string[] = [];
    autocompletePosicionRFC: any;

    //Variables tabs
    proveedoresAutocomplete: any;
    fechaEntregaServicio: any;
    fechaDeLiberacion: any;
    listadoPosicionActual = Array<SubPosicionViewModel>();
    hoy: Date = new Date();
    todasPosicionesSeleccionadas: boolean = false;

    // tituloColumnaTipoDeImputacion: string;
    // enumTipoImputacion: typeof EnumTipoImputacion = EnumTipoImputacion;
    enumColumnaSubPosicion: typeof EnumColumnaSubPosicion = EnumColumnaSubPosicion;

    // Lista de asociar contrato marco
    listaContratos: ContratoMarco[] = [];
    // Booleano para popup contrato marco
    displayAsociar: boolean = false;
    // Booleano para mostrar boton de asociar
    displayBotonAsociar: boolean = false;
    // Texto boton asociar
    textoAsociarBtn = 'ASOCIAR CONTRATO'

    tipoPosicion: SelectItem[];
    tipoImputacion: any[];
    imputacionSeleccionada: any;

    camposObligatorios: any[] = [
        { campo: 'selectTipoPosicion', esObligatorio: true, esFijo: true },
        { campo: 'selectClaseDocumento', esObligatorio: true, esFijo: true },
        { campo: 'centroDeCosto', esObligatorio: false, esFijo: true },
        { campo: 'ordenDeOt', esObligatorio: false, esFijo: true },
        { campo: 'ordenDeInversion', esObligatorio: false, esFijo: true },
        { campo: 'siniestroBeneficio', esObligatorio: false, esFijo: true },
        { campo: 'tipoImputacion', esObligatorio: true, esFijo: true },
        { campo: 'selectCentroEntrega', esObligatorio: true, esFijo: false },
        { campo: 'selectAlmacenEntrega', esObligatorio: false, esFijo: false },
        { campo: 'calleEntrega', esObligatorio: true, esFijo: true },
        { campo: 'paisEntrega', esObligatorio: false, esFijo: true },
        { campo: 'numeroEntrega', esObligatorio: false, esFijo: true },
        // { campo: 'rubroElectrico', esObligatorio: false, esFijo: false },
        // { campo: 'rubroCivil', esObligatorio: false, esFijo: false },
        // { campo: 'rubroIngenieria', esObligatorio: false, esFijo: false },
        // { campo: 'rubroMecanico', esObligatorio: false, esFijo: false },
        // { campo: 'rubroConsultoria', esObligatorio: false, esFijo: false },
        { campo: 'proveedoresValidos', esObligatorio: false, esFijo: true },
        { campo: 'proveedoresInvalidos', esObligatorio: false, esFijo: true },
        { campo: 'proveedoresNoSugeridos', esObligatorio: false, esFijo: true },
        { campo: 'selectMonedaCompras', esObligatorio: true, esFijo: true }
    ];

    // array de columnas en la grilla
    // se utiliza esta array para luego cargar las posiciones dinamicamente segun la informacion del clipboard
    columnasGrilla: any = [
        { nombre: "codigoServicio", tipo: "codigoServicioSolp" },
        { nombre: "tareaSubcontratar", tipo: "tarea" },
        { nombre: "cuentaTd", tipo: "numerico" },
        { nombre: "unidadMedida", tipo: "combo" },
        { nombre: "precioBruto", tipo: "decimal" },
        { nombre: "cuentaMayor", tipo: "codigoSap", tabla: "CuentasSolpSap" },
        { nombre: "tipoImputacion", tipo: "codigoSap" }
    ];

    mensajesEncabezado: Message[] = [];

    items: MenuItem[];
    activeItem: MenuItem;
    contratoMarco: ContratoMarco = null;

    @ViewChild('menuItems') menu: MenuItem[];
    contratosAsociar: any;
    listaDePosicionesAsociar: any;
    alertaParaAsociar: boolean;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, private formBuilder: FormBuilder, protected router: Router,
        private validadorPasoSolpService: ValidadorPasoSolpService, private confirmationService: ConfirmationService,
        private obtenerContratoMarcoService: ObtenerContratoMarcoService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    ngOnInit(): void {
        this.setCombos();

        if (this.model.posiciones.length == 0) {
            this.agregarPosicion();
            this.deshabilitarImputaciones();
        }

        if (this.model.posicionActual != undefined) {
            this.model.posicionActual.setTabPosicion();
        }


        this.listarContratosAsociados();
    }

    public setCombos(): void {
        //Obtengo todas las opciones de los autocomplete
        if (this.combos != undefined) {
            this.claseDocumento = this.combos.ClaseDocumento;
            this.centroEntrega = this.combos.Centro;
            this.monedaCompras = this.combos.Moneda;
            this.tipoPosicion = this.combos.TipoPosicion;
            this.tipoImputacion = this.combos.TipoImputacion;
            this.unidades = this.combos.Unidades;
            if (this.model.posiciones.length > 0) {
                this.model.posiciones.forEach(posicion => {
                    posicion.selectComboAlmacenes = this.combos.Almacen.filter(x => x.IdPadre == posicion.selectCentroEntrega.Id);
                });
            }
        }
    }

    ngOnChanges() {

        this.setTabs();
        this.setCombos();

        //Hace que clase documento no use la primera opcion como predeterminada
        var clase = this.claseDocumento != undefined ? this.claseDocumento[0] : null;
        let claseDocumento = this.model.selectClaseDocumento !== undefined && this.model.selectClaseDocumento.Id > 0 ? this.model.selectClaseDocumento : clase;
        this.model.selectClaseDocumento = this.model.selectClaseDocumento !== undefined && this.model.selectClaseDocumento.Id > 0 ? this.model.selectClaseDocumento : 0;
        this.actualizarCamposObligatorios(this.model.selectClaseDocumento);
        this.setControlesObligatorios(claseDocumento);

        this.validadorPasoSolpService.formulario = this.formularioActual;

        if (this.model.cargoPasoCinco) {
            this.validadorPasoSolpService.aplicarValidaciones();
        }

        if (this.model.centroPorDefecto && this.model.posicionActual && !this.model.posicionActual.selectCentroEntrega)
            this.model.posicionActual.selectCentroEntrega = this.combos.Centro.find(x => x.Codigo == this.model.centroPorDefecto)

        if (this.model.monedaPorDefecto && this.model.posicionActual && !this.model.posicionActual.monedaSeleccionada)
            this.model.posicionActual.monedaSeleccionada = this.combos.Moneda.find(x => x.Codigo == this.model.monedaPorDefecto)

        if (this.model.posicionActual && !this.model.posicionActual.selectSolicitanteCompras)
            this.model.posicionActual.selectSolicitanteCompras = this.model.fiscalContrato;

        this.model.cargoPasoCinco = true;
        this.editarDocumento = this.disableDocumento();
        this.listadoPosicionActual = this.model.posicionActual ? this.model.posicionActual.listadoSubPosiciones : [];

        if (this.model.vincularAPliego) {
            this.mensajesEncabezado.push({ severity: 'warn', summary: '', detail: 'No es posible editar esta pantalla desde la plataforma. Para editar diríjase a SAP.' });
            this.formularioActual.disable();
        }
        else {
            this.mensajesEncabezado = [];
            this.formularioActual.enable();
        }

        this.validarTipoPosicion();
        this.validarTabCompleto();
        this.validarNuevaPosicion();
        if (this.model.posicionActual != undefined) {
            this.model.posicionActual.setTabPosicion();
        }
    }

    setTabs() {
        this.setMenuSeccionTab("Cabecera", "Cabecera");
    }

    validarTipoPosicion(): void {
        if (this.model.selectTipoPosicion == undefined)
            this.model.tableHide = true;
    }

    activateMenu(activeItem, posicion) {
        posicion.activeMenuTab = activeItem.activeItem.label;
    }

    setupAlmacenEntregaByCentro() {
        this.model.posicionActual.selectComboAlmacenes = this.combos.Almacen.filter(x => x.IdPadre == this.model.posicionActual.selectCentroEntrega.Id);
    }

    seleccionarTodo() {
        if (this.todasPosicionesSeleccionadas) {
            this.model.posiciones.map(pos => pos.posicionCheck = true);
        } else {
            this.model.posiciones.map(pos => pos.posicionCheck = false);
        }
    }


    agregarPosicion() {
        var ultimaPosicion = this.model.posiciones.length > 0 ?
            this.model.posiciones[this.model.posiciones.length - 1] as any : null;
        this.model.agregarNuevaPosicion(ultimaPosicion as SolpPosicion);
        this.setupAlmacenEntregaByCentro();
        this.model.posicionActual.setTabPosicion();
        if (!this.model.nroSolp && !this.combos.CombosSeteados) {
            this.completarDatosUltimaSolp();
            this.combos.CombosSeteados = true;
        }
    }

    duplicarPosicion(el: HTMLElement) {
        //habria que hacer un filter que esten en true los check y a ese resultado hacer un for
        var posicionChequeadas = this.model.posiciones.filter(x => x.posicionCheck === true);
        if (posicionChequeadas.length > 0) {
            var _this = this;
            posicionChequeadas.forEach(function (item1: any) {
                _this.model.agregarNuevaPosicion(item1 as SolpPosicion);
                _this.setupAlmacenEntregaByCentro();
            });
            //  el.scrollIntoView();
            this.validarNuevaPosicion();
        }
    }

    validarTabCompleto() {
        if (this.model.posiciones.length < 0 && this.model.posiciones != undefined || this.model.posiciones != null) {
            this.model.posiciones.forEach(posicion => {
                posicion.doValidatePosicion(this.model.tipoSolpSap);
            });
        } else {
            this.agregarPosicion();
        }
    }

    validarFinal() {
        this.model.posiciones.forEach(posicion => {
            if (!posicion.tabsPosicionValidos.tabImputacion) {
                console.log("posicion N° " + posicion.numeroPosicion + " tiene el tab Imputaciones sin completar")
                //return false
            }

            if (!posicion.tabsPosicionValidos.tabProveedor) {
                console.log("posicion N° " + posicion.numeroPosicion + " tiene el tab Proveedor sin completar")
                //return false
            }

            if (!posicion.tabsPosicionValidos.tabDireccionEntrega) {
                console.log("posicion N° " + posicion.numeroPosicion + " tiene el tab Dirección de Entrega sin completar")
                //return false
            }

            if (!posicion.tabsPosicionValidos.tabDatosPosicion) {
                console.log("posicion N° " + posicion.numeroPosicion + " tiene el tab Datos de Posición sin completar")
                //return false
            }

            if (!posicion.tabsPosicionValidos.tabFechas) {
                console.log("posicion N° " + posicion.numeroPosicion + " tiene el tab Fechas sin completar")
                //return false
            }

            if (!posicion.tabsPosicionValidos.tabSubposiciones) {
                console.log("posicion N° " + posicion.numeroPosicion + " tiene el tab Fechas sin completar")
                //return false
            }

            return true
        });
    }

    eliminarPosicion() {
        var posicionChequeadas = this.model.posiciones.filter(x => x.posicionCheck === true);
        if (posicionChequeadas.length > 0) {
            this.confirmationService.confirm({
                message: '¿Está seguro de que desea eliminar la posición?',
                accept: () => {
                    posicionChequeadas.forEach(pos =>
                        this.model.eliminarPosicion(pos as SolpPosicion)
                    );
                    this.listarContratosAsociados();
                    if (this.model.posiciones.length == 1 && this.model.posiciones[0].selectAlmacenEntrega == undefined) {
                        this.setupAlmacenEntregaByCentro();
                    }
                },
                reject: () => {
                }
            });
        }
    }

    eliminarPosicionUnicaSubPosicion(posicion: SolpPosicion) {
        this.model.eliminarPosicion(posicion);
    }

    recuperarPosicion() {
        var posicionChequeadas = this.model.posiciones.filter(pos => pos.posicionCheck === true && pos.estado === false);
        if (posicionChequeadas.length > 0) {
            posicionChequeadas.forEach(pos => this.model.recuperarPosicion(pos));
        }
    }

    //Validacion de las posiciones
    validarPosicionActual() {
        this.validFormEliminarPosicion = this.model.posicionActual.estado;
        this.model.posicionActual.posicionValida = !this.validadorPasoSolpService.esPasoInvalido();
    }

    //Validaciones de campos
    mostrarValidacion(campoAValidar, vacio) {
        let camposVacios = this.camposObligatorios.find(x => x.campo == campoAValidar && x.esObligatorio);
        return (camposVacios != null && vacio == 0);
    }

    //Actualiza los campos obligatorios segun la clase de documento
    actualizarCamposObligatorios(claseDocumento) {
        //reset de obligatorios configurables
        this.camposObligatorios.forEach(c => {
            if (!c.esFijo)
                c.esObligatorio = false;
        });

        let camposObligatorios = this.combos.CamposObligatoriosCabeceraSolp.filter(x => x.ClaseDocumentoCodigo == claseDocumento.Codigo);

        camposObligatorios.forEach(c => {
            if (this.camposObligatorios.find(x => x.campo == c.Codigo) != null)
                this.camposObligatorios.find(x => x.campo == c.Codigo).esObligatorio = true;

        });
    }

    //Seteo de campos obligatorios
    setControlesObligatorios(claseDocumento) {
        if (!claseDocumento || claseDocumento > 0) {
            return
        }
        this.actualizarCamposObligatorios(claseDocumento);

        if (!this.formularioActual) {
            this.formularioActual = this.formBuilder.group({});
            this.camposObligatorios.forEach(x => {
                let control = x.esObligatorio ? new FormControl('', [Validators.required]) : new FormControl();
                this.formularioActual.addControl(x.campo, control);
            });

            return;
        }

        this.camposObligatorios.forEach(x => {
            let formControl = this.formularioActual.controls[x.campo];
            formControl.clearValidators();
            if (x.esObligatorio) {
                formControl.setValidators(Validators.required);
            }
        });
    }

    //Esta funcion solo trabaja con los forms, activa el mensaje de error cuando tocas el campo o cuando guardas la solp
    mostrarError(nombreCampo: string): boolean {
        if (this.formularioActual && this.formularioActual.controls) {
            let campoObligatorio = this.camposObligatorios.find(x => x.campo == nombreCampo);
            if (campoObligatorio) {
                let control = this.formularioActual.controls[nombreCampo];
                return (control.invalid || (control.errors && control.errors.required))
                    && (control.dirty || control.touched)
            }
        }
        return false;
    }

    //Muestra el * en los campos obligatorios
    mostrarAsterisco(nombreCampo: string) {
        return this.camposObligatorios.find(x => x.campo == nombreCampo).esObligatorio ? '*' : '';
    }

    //Funcion que no permite cambiar la clase de documento una vez que se guardo en SAP
    disableDocumento() {
        if (this.model.nroSolp) {
            return true;
        } else {
            return false;
        }
    }
    //Valida nueva posicion
    validarNuevaPosicion() {
        if (this.model.posicionActual && this.model.posicionActual.concluido == undefined) {
            this.disabled = false;
        } else {
            this.disabled = true;
        }
    }

    //Cambia la clase de documento y el seteo de los campos obligatorios
    cambiarClaseDocumento() {
        this.setControlesObligatorios(this.model.selectClaseDocumento);
        this.validarPosicionActual();
    }

    // Se cargan los contratos asociados a la grilla de posiciones
    cargarContratosAsociados(contratos, posicion: SolpPosicion) {
        var arrayContratos = [];
        this.listaContratos = [];

        var obj = JSON.stringify(contratos, function (key, value) {
            const contrato = {
                ProveedorFijo: value[0].ProveedorFijo,
                NombreProveedor: value[0].NombreProveedor,
                NumeroContratoSuperior: value[0].NumeroContratoSuperior,
                UnidadMedida: value[0].UnidadMedida,
                OrganizacionCompras: value[0].OrganizacionCompras,
                CentroAprovisionamiento: value[0].CentroAprovisionamiento,
                ClaveMoneda: value.ClaveMoneda,
                PeriodoValidez: value.FinPeriodoValidez,
                PrecioBruto: value.ImporteMonedaBapi,
                Almacen: value.Almacen,
                GrupoArticulo: value.GrupoArticuloMateriales,
                GrupoCompras: value.GrupoCompras
            }
            arrayContratos.push(contrato);
        })

        this.asociarContrato.onChangeContrato();
        this.listaContratos = arrayContratos;
        this.validarBotonAsociarEditarContrato();
    }

    // Se consumen los endpoints de contrato marco y fuente de aprovisionamiento
    validarFuenteAprovisionamiento(posicion: SolpPosicion) {
        try {
            this.subscription = this.service.ListarFuenteAprovisionamiento(posicion.fechaEntregaServicio.toISOString().substring(0, 10), posicion.codigoServicio.CodigoSap.substr(-8), posicion.selectCentroEntrega.CodigoSap).pipe(
                mergeMap((result: any) =>
                    from(result.data.filter(contrato => contrato.NumeroContratoSuperior)).pipe(
                        mergeMap(
                            (data: any) => this.service.ObtenerContratoMarco(data.NumeroContratoSuperior, posicion.selectCentroEntrega.CodigoSap).pipe(
                                map((contrato: any) => ({
                                    ...result.data.filter(contrato => contrato.NumeroContratoSuperior),
                                    ...contrato.data[0],
                                    ...contrato.data[0].Posiciones.find(material => material.NumeroMaterial == posicion.codigoServicio.CodigoSap)
                                }))
                            )
                        )
                    )
                )
            ).subscribe((result: any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    if (result) {
                        this.cargarContratosAsociados(result, posicion);
                    }
                }
            }, error => {
                this.displayBotonAsociar = false;
                this.floatMsgService.setErrorMsg(error.message);
            }
            )
        } catch (e) {
            this.displayBotonAsociar = false;
            this.floatMsgService.setErrorMsg(e);
        }
    }

    buscarCombo(event, type) {
        switch (type) {
            case 'CENTRO':
                this.centroEntrega = this.combos.Centro.filter(x => x.CodigoDescripcion.toLowerCase().includes(event.query.toLowerCase()));
                break;
            case 'ALMACEN':
                if (this.model.posicionActual.selectCentroEntrega == undefined) {
                    this.almacenEntrega = [];
                }
                else {
                    this.almacenEntrega = this.combos.Almacen.filter(x => x.IdPadre == this.model.posicionActual.selectCentroEntrega.Id &&
                        x.CodigoDescripcion.toLowerCase().includes(event.query.toLowerCase()));
                }
                break;
            case 'MONEDA COMPRAS':
                this.monedaCompras = this.combos.Moneda.filter(x => x.CodigoDescripcion.toLowerCase().includes(event.query.toLowerCase()));
                break;
            case 'UNIDAD MEDIDA':
                this.unidades = this.combos.Unidades.filter(x => x.Descripcion.toLowerCase().includes(event.query.toLowerCase()));
                break;
            default:
                break;
        }
    }

    //Funciones de los tabs
    centroSeleccionado(posicion) {
        let direccionCentro: any;

        if (!this.model.posicionActual.selectCentroEntrega && this.model.centroPorDefecto) {
            this.model.posicionActual.selectCentroEntrega = this.model.centroPorDefecto;
        }

        if (this.model.posicionActual.selectCentroEntrega) {
            direccionCentro = this.combos.CentrosDireccion.find(x => x.CodigoSap == this.model.posicionActual.selectCentroEntrega.CodigoSap);
        }

        this.model.posicionActual.nombreEntrega = this.model.posicionActual.nombreEntrega
            || (this.model.posicionActual.selectCentroEntrega == undefined ? "" : this.model.posicionActual.selectCentroEntrega.Descripcion);

        posicion.selectComboAlmacenes = this.combos.Almacen.filter(x => x.IdPadre == posicion.selectCentroEntrega.Id);

        this.fillValoresDireccion(direccionCentro);
    }

    fillValoresDireccion(direccionCentro) {
        if (direccionCentro !== undefined) {
            this.model.posicionActual.codigoPostalEntrega = direccionCentro.Cp;
            this.model.posicionActual.calleEntrega = direccionCentro.Direccion;
            this.model.posicionActual.numeroEntrega = direccionCentro.Numero;
            this.model.posicionActual.paisEntrega = direccionCentro.Pais;
            if (this.model.posicionActual.selectCentroEntrega != undefined) {
                this.model.posicionActual.nombreEntrega = this.model.posicionActual.selectCentroEntrega.Descripcion;
            }
        } else {
            this.model.posicionActual.codigoPostalEntrega = "";
            this.model.posicionActual.calleEntrega = "";
            this.model.posicionActual.numeroEntrega = "";
            this.model.posicionActual.paisEntrega = "";
            this.model.posicionActual.nombreEntrega = "";
        }
    }

    //servicios
    /**
    Metodo Auxuliar para cargar una fila dinamicamente
        indexColumn : posicion de la columna en la grilla coincide con el array columnasGrilla
    columnas : array de valores del clipboard que se obtiene de cada columna
    */
    SetValuesForColumns(indexColumn: number, columnas: any, rowIndex: number, listado: SubPosicionViewModel[]): void {
        let esEdicion = false;

        //piso las filas que tengan datos y si no tengo mas filas creo nuevas
        let tamañoArray = this.listadoPosicionActual.length;
        let fila: SubPosicionViewModel;
        if (rowIndex < tamañoArray) {
            fila = listado[rowIndex];
            esEdicion = true;
        } else {
            fila = new SubPosicionViewModel(this.listadoPosicionActual.length);
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
            listado.push(new SubPosicionViewModel(this.listadoPosicionActual.length));
        }
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
                this.SetValuesForColumns(indexColumna, columnas, rowIndex, this.listadoPosicionActual);
                rowIndex++;
            });
            // this.calcularTotalSubPosicion();
            this.completarCodigosSapOnPaste();
            this.completarServicioSolpOnPaste();
            this.endEditCell(dt);
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
                            this.listadoPosicionActual.forEach(c => {
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
                            this.listadoPosicionActual.forEach(c => {
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

    autocompleteMaterialSolp(event) {
        const idCentro = this.model.posicionActual.selectCentroEntrega.Id;
        if (idCentro == null) {
            return;
        }
        try {
            this.subscription = this.service.autocompleteMaterialSolp(event.query.toLowerCase(), idCentro).subscribe(
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

    autocompleteCodigoMaterialSolp(event) {
        const idCentro = this.model.posicionActual.selectCentroEntrega.Id;
        if (idCentro == null) {
            return;
        }
        try {
            this.subscription = this.service.autocompleteCodigoMaterialSolp(event.query.toLowerCase(), idCentro).subscribe(
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

    //Funciones de la tabla
    onSelectServicio(posicion: SolpPosicion, dt) {
        posicion.tareaSubcontratar = posicion.codigoServicio.Descripcion;
        posicion.textoSuministro = posicion.codigoServicio.Descripcion;
        posicion.tareaSubcontratarObj = { ...posicion.codigoServicio };
        this.autocompletarCamposMaterial(posicion);

        this.endEditCell(dt);


    }

    onSelectTarea(posicion: SolpPosicion, dt) {
        posicion.tareaSubcontratar = posicion.tareaSubcontratarObj.Descripcion;
        posicion.textoSuministro = posicion.tareaSubcontratarObj.Descripcion;
        posicion.codigoServicio = { ...posicion.tareaSubcontratarObj };

        this.autocompletarCamposMaterial(posicion);
        this.endEditCell(dt);

        this.listarContratosAsociados()

    }

    autocompletarCamposMaterial(posicion: SolpPosicion) {
        this.autocompleteMaterialRFC(posicion).pipe(
            switchMap((result: any) => { //para asegurarme de que autocompleteMaterialRFC haya terminado antes de seguir con la lógica
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    this.autocompletePosicionRFC = result;
                }
                return of(null); // Por si no se devuelve nada desde el observable
            })
        ).subscribe(() => {
            if (this.autocompletePosicionRFC != undefined) {
                if (this.combos.Unidades.find(x => x.Codigo == this.autocompletePosicionRFC.Unidad)) {
                    posicion.unidadSeleccionada = this.combos.Unidades.find(x => x.Codigo == this.autocompletePosicionRFC.Unidad);
                    posicion.unidadMedida = this.autocompletePosicionRFC.Unidad;
                }
                if (this.combos.Moneda.find(x => x.Codigo == this.autocompletePosicionRFC.Moneda)) {
                    posicion.monedaSeleccionada = this.combos.Moneda.find(x => x.Codigo == this.autocompletePosicionRFC.Moneda);
                }
                posicion.precioBruto = this.autocompletePosicionRFC.Precio;
            }
        });
        posicion.cuentaMayor = "";
        var grupoArticuloAux = this.combos.GrupoArticulo.find(x => x.Descripcion == posicion.codigoServicio.GrupoArticulo.Descripcion);
        if (grupoArticuloAux) {
            posicion.selectArticuloCompras = grupoArticuloAux;
        }
        //var cuentaMayorAux = this.combos.CuentaMayor.find(x => x.Descripcion == posicion.codigoServicio.CuentaMayor.Descripcion);
        if (posicion.codigoServicio.CuentaMayor && posicion.codigoServicio.CuentaMayor.Id > 0) {
            posicion.cuentaMayor = posicion.codigoServicio.CuentaMayor;
        }
    }

    private autocompleteMaterialRFC(posicion: SolpPosicion): Observable<any> {
        return this.service.autocompleteMaterialRFC(posicion);
    }

    onBlurTarea(event, posicion: SubPosicionViewModel) {
        posicion.tareaSubcontratar = event.target.value;
        posicion.tareaSubcontratarObj = { Descripcion: event.target.value }
    }

    onFocusTarea(event) {
        event.target.select();
    }

    endEditCell(dt) {
        dt.closeCellEdit();
    }

    calcularValorNeto(posicion: SolpPosicion): void {
        if (posicion != null && posicion != undefined) {
            posicion.calcularValorTotal();
        }
        this.model.calcularValorTotalPorMoneda();
    }

    onSelectMoneda() {
        this.model.calcularValorTotalPorMoneda();
    }

    cambiarTipoSolp() {
        if (this.model.selectTipoPosicion) {
            this.model.posiciones.forEach(posicion => {
                this.model.eliminarPosicion(posicion as SolpPosicion)
                // posicion.tipoPosicion = this.model.selectTipoPosicion;
                // this.model.posicionActual = posicion;
                // this.model.posicionActual.setTabPosicion();
                // posicion.calcularValorTotal();
                // posicion.doValidatePosicion(this.model.tipoSolpSap);
            });
            if (this.model.posiciones.length == 1) {
                this.setupAlmacenEntregaByCentro();
            }
            this.model.calcularValorTotalPorMoneda();
            this.listarContratosAsociados();
        }
    }

    // Abre el modal de Asociar contrato
    showAsociarDialog() {
        this.displayAsociar = true;
    }

    cancelarAsociar() {
        this.displayAsociar = false;
    }

    // Se asocian los datos de la grilla a su respectiva posicion
    asociarCM($event) {
        let posicionIndex = this.model.posiciones.findIndex(posicion => posicion.id == $event.posicionSeleccionada.id);

        this.model.posiciones[posicionIndex].numeroContratoSuperior = $event.contrato.NumeroContratoSuperior;
        this.model.posiciones[posicionIndex].numeroPosicionContratoSuperior = $event.contrato.NumeroPosicionContratoSuperior;
        this.model.posiciones[posicionIndex].provedorFijo = $event.contrato.ProveedorFijo;
        this.model.posiciones[posicionIndex].nombreProveedor = $event.contrato.NombreProveedor;
        this.model.posiciones[posicionIndex].orgCompras = $event.contrato.OrganizacionCompras;
        this.model.posiciones[posicionIndex].precioBruto = $event.contrato.PrecioBruto;

        // Set Combos
        let unidadSeleccionadaAux = this.combos.Unidades.find(x => x.Descripcion == $event.contrato.UnidadMedida);
        this.model.posiciones[posicionIndex].unidadSeleccionada = unidadSeleccionadaAux;

        // let monedaSeleccionadaAux = this.combos.Moneda.find(x => x.Codigo == $event.contrato.ClaveMoneda);
        //this.model.posiciones[posicionIndex].monedaSeleccionada = monedaSeleccionadaAux;

        //  let almacenSeleccionadoAux = this.combos.Almacen.find(x => x.Codigo == $event.contrato.Almacen);
        // this.model.posiciones[posicionIndex].selectAlmacenEntrega = almacenSeleccionadoAux;

        //  let grupoArticuloSeleccionadoAux = this.combos.GrupoArticulo.find(x => x.Codigo == $event.contrato.GrupoArticulo);
        //  this.model.posiciones[posicionIndex].selectArticuloCompras = grupoArticuloSeleccionadoAux;

        let grupoComprasSeleccionadoAux = this.combos.GrupoCompras.find(x => x.Codigo == $event.contrato.GrupoCompras);
        this.model.posiciones[posicionIndex].selectGrupoCompras = grupoComprasSeleccionadoAux;

        this.listarContratosAsociados();
    }

    public showObtenerContratoMarcoDialog() {
        this.obtenerContratoMarcoService.show(true);
    }

    public obtenerContratoMarco(args: ObtenerContratoMarco) {
        this.service.obtenerContratoMarco(args.centro, args.numeroContrato).subscribe((response: any) => {
            if (response.data && response.data.length) {
                this.contratoMarco = new ContratoMarco(response.data[0]);

                if ((this.esTipoMaterial && this.contratoMarco.posiciones.every(pos => pos.subPosiciones.length == 0 || pos.subPosiciones == null))
                    || (this.esTipoServicio && this.contratoMarco.posiciones.every(pos => pos.subPosiciones.length > 0))) {
                    //this.contratoMarco = null;
                } else {
                    this.contratoMarco = null;
                }
            }
            this.listarContratosAsociados()
        });
    }

    public agregarPosicionesContratoMarco(contratoMarco: ContratoMarco) {
        if (contratoMarco != null) {
            contratoMarco.posiciones.filter(pos => pos.selected == true).forEach(pos => {
                let centro = this.combos.Centro.find(x => x.Codigo == pos.centro);
                let direccionCentro = this.combos.CentrosDireccion.find(x => x.CodigoSap == centro.CodigoSap);
                let moneda = this.combos.Moneda.find(x => x.Codigo == contratoMarco.claveMoneda);

                let servicioMaterialObj = {
                    Codigo: pos.numeroMaterial,
                    Descripcion: pos.textoMaterialOServicio,
                    UnidadMedidaBase: pos.unidadMedida
                };

                let newPos = this.model.nuevaPosicion(null, centro, direccionCentro, moneda);

                newPos.codigoServicio = servicioMaterialObj;
                newPos.tareaSubcontratar = servicioMaterialObj.Descripcion;
                newPos.tareaSubcontratarObj = { ...servicioMaterialObj };

                newPos.numeroContratoSuperior = pos.numeroDocumentoCompras;
                newPos.numeroPosicionContratoSuperior = pos.numeroPosicionDocumentoCompras;
                newPos.provedorFijo = contratoMarco.numeroCuentaProveedor;
                newPos.nombreProveedor = contratoMarco.nombreProveedor;
                newPos.orgCompras = contratoMarco.organizacionCompras;
                newPos.precioBruto = pos.importeMonedaBapi;

                let unidadMedidaObj = this.combos.Unidades.find(u => u.Descripcion == pos.unidadMedida);
                if (unidadMedidaObj) {
                    newPos.unidadSeleccionada = unidadMedidaObj;
                }

                let tipoImputacionObj = this.combos.TipoImputacion.find(m => m.CodigoSap == pos.tipoImputacionCompras);
                if (tipoImputacionObj) {
                    newPos.tipoImputacion = tipoImputacionObj;
                }

                let almacenSeleccionadoObj = this.combos.Almacen.find(x => x.Codigo == pos.almacen);
                newPos.selectComboAlmacenes = this.combos.Almacen;
                if (almacenSeleccionadoObj) {
                    newPos.selectAlmacenEntrega = almacenSeleccionadoObj;
                }

                let grupoArticuloSeleccionadoObj = this.combos.GrupoArticulo.find(x => x.Codigo == pos.grupoArticuloMateriales);
                if (grupoArticuloSeleccionadoObj) {
                    newPos.selectArticuloCompras = grupoArticuloSeleccionadoObj;
                }

                let grupoComprasSeleccionadoObj = this.combos.GrupoCompras.find(x => x.Codigo == contratoMarco.grupoCompras);
                if (grupoComprasSeleccionadoObj) {
                    newPos.selectGrupoCompras = grupoComprasSeleccionadoObj;
                }

                let canAddSubPos = newPos.esTipoPosicionServicio && pos.subPosiciones.length && pos.subPosiciones.filter(subpos => subpos.selected).length;
                if (canAddSubPos) {
                    newPos.listadoSubPosiciones = [];
                    pos.subPosiciones.filter(subpos => subpos.selected).forEach(subPos => {
                        let newSubPos = newPos.crearSubPosicion();

                        let codigoServicioObj = {
                            Codigo: subPos.numeroServicio,
                            Descripcion: subPos.textoBreve,
                            UnidadMedidaBase: subPos.unidadMedidaBase
                        };
                        newSubPos.codigoServicio = { ...codigoServicioObj }
                        newSubPos.tareaSubcontratar = subPos.textoBreve;
                        newSubPos.tareaSubcontratarObj = { ...codigoServicioObj };

                        var unidadSeleccionadaObj = this.combos.Unidades.find(x => x.Descripcion == subPos.unidadMedidaBase);
                        if (unidadSeleccionadaObj) {
                            newSubPos.unidadSeleccionada = unidadSeleccionadaObj;
                            newSubPos.unidadMedida = unidadSeleccionadaObj.Descripcion;
                        }

                        newSubPos.cuentaTd = subPos.cantidadPositivoONegativo;
                        newSubPos.precioBruto = subPos.precioUnitario;
                        newSubPos.calcularValorNeto();
                        newPos.agregarSubPosicion(newSubPos);
                        newPos.calcularValorTotal();
                    });
                }

                this.model.agregarNuevaPosicionDesdeContratoMarco(newPos);
                //this.agregarPosicion();

            });
            this.model.calcularValorTotalPorMoneda();
        }
        this.obtenerContratoMarcoService.close();
        this.listarContratosAsociados();
    }

    public get esTipoMaterial(): boolean {
        return this.tienePosicionSeleccionada && this.model.selectTipoPosicion.Codigo == "MATERIALES";
    }

    public get esTipoServicio(): boolean {
        return this.tienePosicionSeleccionada && this.model.selectTipoPosicion.Codigo == "SERVICIO";
    }

    public get esTipoContratoMarco() {
        return this.model.posicionActual.tipoPosicion == "CONTRATO MARCO";
    }

    public get tienePosicionSeleccionada(): boolean {
        return (this.model.selectTipoPosicion != undefined
            && this.model.selectTipoPosicion != null
            && this.model.selectTipoPosicion.Id != ""
            && this.model.selectClaseDocumento != undefined
            && this.model.selectClaseDocumento != null
            && this.model.selectClaseDocumento.Id > 0
        );
    }

    public get tieneCodigoServicio(): boolean {
        return this.model.selectTipoPosicion.Codigo == "MATERIALES" && this.model.posicionActual.codigoServicio != null;
    }

    clearCode(posicion) {
        if (posicion.tareaSubcontratar != null) {
            posicion.codigoServicio = null;
        }
    }

    clearCode2() {
        if (this.model.posicionActual.codigoServicio != null) {
            this.model.posicionActual.tareaSubcontratarObj = null;
        }
    }

    checkCode() {
        if (this.model.posicionActual.codigoServicio == '' || this.model.posicionActual.codigoServicio == null) {
            this.model.posicionActual.tareaSubcontratarObj = null;

        }
    }

    buscarContratosAsociados() {
        // var posiciones = this.model.posiciones.filter(x => x.Centro === true);
    }

    eliminarContratoAsociar(posicion) {

        posicion.numeroContratoSuperior = "";
        posicion.provedorFijo = "";
        posicion.nombreProveedor = "";
        posicion.orgCompras = "";
        posicion.numeroPosicionContratoSuperior = "";
    }

    listarContratosAsociados() {
        try {

            this.subscription = this.service.listarContratosAsociar(this.model.posiciones
            ).subscribe(
                (result: any) => {

                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.listaDePosicionesAsociar = result.data;
                        this.validarBotonAsociarEditarContrato();
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false;
        }

        return false;
    }

    validarBotonAsociarEditarContrato() {

        //eliminar contrato marco     

        if ((this.listaDePosicionesAsociar == null || this.listaDePosicionesAsociar.length == 0)) {
            this.alertaParaAsociar = false;
            this.displayBotonAsociar = false;
        } else {
            var puedoAsociarContrato = this.listaDePosicionesAsociar.
                some(x => x.Proveedor == null || x.Proveedor == "")
            if (puedoAsociarContrato) {
                this.alertaParaAsociar = true;
                this.displayBotonAsociar = true;
                this.textoAsociarBtn = 'ASOCIAR CONTRATO'
            } else {
                this.alertaParaAsociar = false;
                this.displayBotonAsociar = true;
                this.textoAsociarBtn = 'EDITAR CONTRATO'
            }
        }

    }

    private completarDatosUltimaSolp() {
        if (this.datosUltimaSolp != undefined) {
            if (this.datosUltimaSolp.ClaseDocumento != null) {
                this.model.selectClaseDocumento = this.datosUltimaSolp.ClaseDocumento;
            }

            if (this.datosUltimaSolp.TipoPosicion != null) {
                this.model.selectTipoPosicion = this.datosUltimaSolp.TipoPosicion;
                this.cambiarTipoSolp();
            }

            if (this.model.posiciones[0] != undefined) {
                if (this.datosUltimaSolp.Centro != null) {
                    this.model.posiciones[0].selectCentroEntrega = this.datosUltimaSolp.Centro;
                    this.model.posicionActual = this.model.posiciones[0];

                    console.log("this.combos.CentrosDireccion", this.combos.CentrosDireccion);
                    console.log("this.model.posicionActual.selectCentroEntrega.CodigoSap", this.model.posicionActual.selectCentroEntrega)
                    let direccionCentro = this.combos.CentrosDireccion.find(x => x.CodigoSap == this.model.posicionActual.selectCentroEntrega.CodigoSap);
                    console.log("direccionCentro", direccionCentro);
                    this.fillValoresDireccion(direccionCentro);

                }

                if (this.datosUltimaSolp.Almacen != null) {
                    this.model.posiciones[0].selectComboAlmacenes.push(this.datosUltimaSolp.Almacen);
                    this.model.posiciones[0].selectAlmacenEntrega = this.datosUltimaSolp.Almacen;
                }

                if (this.datosUltimaSolp.GrupoCompras != null) {
                    this.model.posiciones[0].selectGrupoCompras = this.datosUltimaSolp.GrupoCompras;
                }

                if (this.model.selectTipoPosicion != undefined && this.model.selectTipoPosicion.Codigo == "SERVICIO") {
                    if (this.datosUltimaSolp.CuentaMayorSP != null) {
                        var newPos = this.model.posiciones[0].crearSubPosicion()
                        this.model.posiciones[0].agregarSubPosicion(newPos);
                        this.model.posiciones[0].listadoSubPosiciones[0].cuentaMayor = this.datosUltimaSolp.CuentaMayorSP;
                    }
                } else {
                    if (this.datosUltimaSolp.CuentaMayor != null) {
                        this.model.posiciones[0].cuentaMayor = this.datosUltimaSolp.CuentaMayor;
                    }
                }
            }

        }
    }

    private deshabilitarImputaciones() {
        this.model.posiciones.forEach(element => {
            element.setTabPosicion();
        });
    }
}