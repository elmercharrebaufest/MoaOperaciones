import { Component, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConsultaService } from '../consulta.service';
import { ListBaseComponent } from './../../common/base-components/list-base-component';
import { Seccion } from './../../common/models/seccion';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { NavService } from './../../common/services/NavService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { DropdownComponent } from './../../common/view-child/dropdown/dropdown.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { ReCaptchaComponent } from 'angular2-recaptcha';
import { Causa, Comentario, Categoria, Subcategoria, ReclamoImpositivo, Materiales } from '../consulta';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ConfirmationService } from 'primeng/api';
import { HttpStatusCodes } from '../../common/models/httpStatusCodes';
import { DatosDisconformidadCalidades, DatosLiquidacionObservada, SendDataService } from '../send-data.service';
import { SeleccionarProveedorComponent } from '../../common/shared-components/seleccionar-proveedor/seleccionar-proveedor.component';
import { CartaPorteService } from '../../carta-porte/carta-porte2.service';
import { finalize } from 'rxjs/operators';
import { CalidadCCPP, CalidadCCPPDiscrepa } from '../../common/models/cartaPorte';

declare var $: any;

@Component({
    selector: 'crear-consulta',
    templateUrl: `crear-consulta.component.html`,
    providers: [{ provide: ConsultaService, useClass: ConsultaService }, CartaPorteService],
    styleUrls: ['./crear-consulta.component.css'],
})
export class CrearConsultaComponent extends ListBaseComponent {

    @BlockUI() blockUI: NgBlockUI;

    @ViewChild('dropdown_categoria')
    protected categoriaDropdownComponent: DropdownComponent;

    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    @ViewChild('fileInput')
    protected fileInput: ElementRef;

    @ViewChild('dtp_fecha_pago')
    protected fechaPagoDTP: ElementRef;

    @ViewChild('recaptchaComponent')
    protected captcha: ReCaptchaComponent;

    @ViewChild('selectProveedor')
    protected selectProveedor: SeleccionarProveedorComponent;

    datosLiquidacionObservada?: DatosLiquidacionObservada;
    datosDisconformidadCalidades?: DatosDisconformidadCalidades;

    constructor(
        protected service: ConsultaService,
        protected navService: NavService,
        protected sessionDataService: SessionDataService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService,
        protected route: ActivatedRoute,
        protected router: Router,
        private confirmationService: ConfirmationService,
        private sendDataService: SendDataService,
        private cartaPorteService: CartaPorteService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.categoriaDropdownComponent = new DropdownComponent();
        this.spinnerSmallComponent = new SpinnerSmallComponent();
        this.datosLiquidacionObservada = sendDataService.getDatosLiquidacionObservada();
        this.datosDisconformidadCalidades = sendDataService.getDatosDisconformidadCalidades();
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CONTACTO MAIL"); }
    private selectUndefinedOptionValue: any;
    mensajeError: string;
    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";
    proveedor: string;
    nombre: string = sessionStorage.getItem("nombre");
    email: string;
    telefono: string;
    categoriaCount = 0;
    subcategoriaCount = 0;

    proveedorId: number;
    tieneSubcategorias: boolean = true;

    consulta: any;

    categorias: Categoria[];
    subcategorias: Subcategoria[];
    causas: Causa[];

    codigoCorredor: string;
    codigoProveedor: any;
    razonSocialProveedor: string = "";
    razonSocialCorredor: string;

    subcategoria: Subcategoria = {
        Id: null
    };
    subcategoriasList: Subcategoria[];
    categoria: Categoria;
    causa: Causa;
    categoriaCode: any;
    subcategoriaCode: any;

    proveedorSelected: any;
    listaArchivos: Array<File> = new Array<File>();

    Detalle: any;
    nuevoComentario: any;
    comprobanteExtra: any;

    caratula: any;
    comentario: string;
    contrato: any;
    razonSocial: string;
    cuit: string;
    nombreVendedor: string;
    comprobante: any;
    rubro: any;
    rubrosOptions: CalidadCCPP[] = [];
    rubrosSelected: CalidadCCPPDiscrepa[] = [];
    fechaPago: string;
    fecha: Date;
    importe: any;
    impuesto: any;
    bolsaEmisoraOblea: string;
    files: FileList = null;
    listaMateriales: Materiales[];
    material: Materiales;
    material_Id: any;

    fechaPagoDP: any;
    visibleButton: boolean = true;
    categoriaSelected: any;
    captchaOk: any = null;
    asunto: string;
    cliente: any;

    reclamoImpositivo: ReclamoImpositivo = new ReclamoImpositivo();
    fechaFactura: string;
    nombreDisabled: boolean = false;

    setTabs() {
        this.setMenuSeccionTab("consulta", "crear-consulta");
    }

    ngOnInit() {
        this.checkPermisos();
        this.setSeccionList();
        this.getCombos();
        if (this.esCorredor) {
            this.codigoCorredor = sessionStorage.getItem("proveedor");
        }
        else {
            this.codigoProveedor = sessionStorage.getItem("proveedor");
        }
        $(".adjuntarArchivo").click(function () {
            $(".adjuntarArchivo1").click();
        });

        this.validarNombre();
        this.setTabs();
    }

    setSeccionList() {
        if (this.securityService.tienePermiso("CARGAR CONSULTA") && this.securityService.tienePermiso("CARGAR CONSULTA INTERNA")) {
            this.navService.setSeccionList([new Seccion('/consulta/crear-consulta', 'crear-consulta', 'Nueva Consulta'), new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas'),
            new Seccion('/consulta/crear-consulta-interna', 'crear-consulta-interna', 'Nueva Consulta Interna')
            ]);
        }
        else if (this.securityService.tienePermiso("CARGAR CONSULTA")) {
            this.navService.setSeccionList([new Seccion('/consulta/crear-consulta', 'crear-consulta', 'Nueva Consulta'), new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas')
            ]);
        } else if (this.securityService.tienePermiso("CARGAR CONSULTA INTERNA")) {
            this.navService.setSeccionList([new Seccion('/consulta/crear-consulta-interna', 'crear-consulta-interna', 'Nueva Consulta Interna'), new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas')
            ]);
        } else {
            this.navService.setSeccionList([new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas')
            ]);
        }
    }

    ngAfterViewInit(): void {
        $(document).on("mouseover", '.form_datetime', function () {
            $(".form_datetime").datetimepicker({
                format: 'yyyy-mm-dd',
                language: 'es',
                weekStart: 1,
                todayBtn: 1,
                autoclose: 1,
                todayHighlight: 1,
                startView: 2,
                forceParse: 0,
                showMeridian: 1,
                pickTime: false,
                minView: 2,
                maxView: 4
            });
        });
    }

    cargarArchivo(event: any) {
        let fileList: FileList = event.target.files;
        let file;

        if (fileList.length > 0) {
            this.files = fileList;
            for (let i = 0; i < fileList.length; i++) {
                file = fileList[i];
                this.listaArchivos.push(file);
            }
        }

        let $formInput = $('input[type=file]');
        $formInput.val(null);
    }

    validarNombre() {
        if (this.nombre == "" || !this.nombre || this.nombre == 'No definido' || this.nombre == undefined || this.nombre == null) {
            this.nombreDisabled = false;
            this.nombre = ""
            return true
        }

        this.nombreDisabled = true;
    }

    getCombos() {
        this.unsubscribe();
        try {
            this.subscription = this.service.getCombos(true, false).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.categorias = result.categorias;
                        this.subcategorias = result.subcategorias;
                        this.causas = result.causas;
                        if (!this.esCorredor) {
                            this.proveedorId = result.proveedorId
                        }
                        this.listaMateriales = result.materiales;
                        if (this.datosLiquidacionObservada) {
                            this.setValoresInicialesParaLiquidacionObservada()
                        }
                        if (this.datosDisconformidadCalidades) {
                            this.setValoresInicialesParaDisconformidadCalidades()
                        }
                        //result.materiales.forEach(x => this.listaMateriales.push({ label: x.Descripcion, value: x.MaterialId }));
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(HttpStatusCodes.friendlyStatusCode(error.status));
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    onProveedorSeleccionado(proveedor: any) {
        this.proveedorSelected = proveedor;
        this.proveedorId = proveedor.proveedorId;
    }

    setMaterial(material) {
        this.listaMateriales.forEach(x => {
            if (x.MaterialId == material) {
                this.comprobanteExtra = x.Descripcion;
            }
        })
    }

    validarConsulta() {
        this.mensajeComponent.setMsgsEmpty();
        if (this.codigoProveedor == "" || !this.codigoProveedor) {
            if (this.esCorredor) {
                this.mensajeComponent.setErrorMsg("Vendedor no asociado a su perfil de corredor. Intente nuevamente");
                return true;
            }
            this.mensajeComponent.setErrorMsg("El campo proveedor esta vacio.");
            return true;
        }
        if (this.categoriaCount == 0) {
            this.mensajeComponent.setErrorMsg("Por favor seleccione una categoria.");
            return true;
        }
        if (this.tieneSubcategorias && this.subcategoriaCount == 0) {
            this.mensajeComponent.setErrorMsg("Por favor seleccione una subcategoria.");
            return true;
        }
        if (this.asunto == "" || !this.asunto) {
            this.mensajeComponent.setErrorMsg("El campo Asunto esta vacio.");
            return true;
        }
        if (this.nombre == "" || !this.nombre || this.nombre == 'No definido') {
            this.mensajeComponent.setErrorMsg("El campo Nombre esta vacio.");
            return true;
        }
        if (this.nuevoComentario == "" || !this.nuevoComentario) {
            this.mensajeComponent.setErrorMsg("El campo Comentario esta vacio.");
            return true;
        }
        if ((this.esCorredor && this.codigoCorredor == "") || (this.esCorredor && !this.codigoCorredor)) {
            this.mensajeComponent.setErrorMsg("El campo Corredor esta vacio.");
            return true;
        }
        if (this.categoriaCode == 'REI' && this.subcategoriaCode == 'RET') {
            this.fechaPago = (<HTMLInputElement>document.querySelectorAll('[fechaInicioInput]')[0]).value;

            if (this.comprobante == "" || !this.comprobante) {
                this.mensajeComponent.setErrorMsg("El campo N° Salida de pago esta vacio.");
                return true;
            }
            if (this.contrato == "" || !this.contrato) {
                this.mensajeComponent.setErrorMsg("El campo Contrato esta vacio.");
                return true;
            }
            if (this.impuesto == "" || !this.impuesto) {
                this.mensajeComponent.setErrorMsg("El campo Impuesto retenido esta vacio.");
                return true;
            }
            if (this.importe == "" || !this.importe) {
                this.mensajeComponent.setErrorMsg("El campo Importe retención esta vacio.");
                return true;
            }
            if (this.fechaPago == "" || !this.fechaPago) {
                this.mensajeComponent.setErrorMsg("Falta seleccionar el campo fecha");
                return true;
            }
        }
        if (this.categoriaCode == 'REI' && this.subcategoriaCode == 'PER') {
            this.fechaPago = (<HTMLInputElement>document.querySelectorAll('[fechaInicioInput]')[0]).value;
            if (this.fechaPago == "" || !this.fechaPago) {
                this.mensajeComponent.setErrorMsg("Falta seleccionar el campo fecha");
                return true;
            }
            if (this.comprobanteExtra == "" || !this.comprobanteExtra) {
                this.mensajeComponent.setErrorMsg("El campo Cliente de pago esta vacio.");
                return true;
            }
            if (this.comprobante == "" || !this.comprobante) {
                this.mensajeComponent.setErrorMsg("El campo N° de factura esta vacio.");
                return true;
            }
            if (this.impuesto == "" || !this.impuesto) {
                this.mensajeComponent.setErrorMsg("El campo Impuesto retenido esta vacio.");
                return true;
            }
        }
        if ((this.categoriaCode == 'BOL' && this.subcategoriaCode == 'CON') || (this.categoriaCode == 'BOL' && this.subcategoriaCode == 'REG')) {
            if (this.contrato == "" || !this.contrato) {
                this.mensajeComponent.setErrorMsg("El campo N° de contrato esta vacio.");
                return true;
            }
        }
        if (this.categoriaCode == 'BOL' && this.subcategoriaCode == 'OPC') {
            this.fechaPago = (<HTMLInputElement>document.querySelectorAll('[fechaInicioInput]')[0]).value;
            if (this.contrato == "" || !this.contrato) {
                this.mensajeComponent.setErrorMsg("El campo N° de contrato esta vacio.");
                return true;
            }
            if (this.bolsaEmisoraOblea == "" || !this.bolsaEmisoraOblea) {
                this.mensajeComponent.setErrorMsg("El campo bolsa Emisora de Oblea esta vacio.");
                return true;
            }
            if (this.listaArchivos == null || this.listaArchivos.length < 1) {
                this.mensajeComponent.setErrorMsg("Falta adjuntar liquidación y la oblea emitida por bolsa");
                return true;
            }
            if (this.fechaPago == "" || !this.fechaPago) {
                this.mensajeComponent.setErrorMsg("Falta seleccionar el campo fecha");
                return true;
            }
        }
        if (this.categoriaCode == 'ACT' && this.subcategoriaCode == 'IMP') {
            if (this.impuesto == "" || !this.impuesto) {
                this.mensajeComponent.setErrorMsg("El campo Impuesto esta vacio.");
                return true;
            }
            if (this.listaArchivos == null || this.listaArchivos.length < 1) {
                this.mensajeComponent.setErrorMsg("Falta adjuntar constancia");
                return true;
            }
        }
        if (this.categoriaCode == 'ACT' && this.subcategoriaCode == 'INF') {
            if (this.listaArchivos == null || this.listaArchivos.length < 1) {
                this.mensajeComponent.setErrorMsg("Falta adjuntar Informe Comercial");
                return true;
            }
        }
        if (this.categoriaCode == 'ACT' && this.subcategoriaCode == 'CAP') {
            if (this.listaArchivos == null || this.listaArchivos.length < 1) {
                this.mensajeComponent.setErrorMsg("Falta adjuntar Carta presentacón");
                return true;
            }
        }
        if (this.categoriaCode == 'ACT' && this.subcategoriaCode == 'CM05') {
            if (this.listaArchivos == null || this.listaArchivos.length < 1 || this.listaArchivos.filter(x => x.type == "application/pdf").length < 1) {
                this.mensajeComponent.setErrorMsg("Falta adjuntar el formulario del CM05, el mismo debe estar en formato PDF.");
                return true;
            }
        }
        if ((this.categoriaCode == 'PAR' && this.subcategoriaCode == 'NROR') || (this.categoriaCode == 'FIN' && this.subcategoriaCode)) {
            if (this.contrato == "" || !this.contrato) {
                this.mensajeComponent.setErrorMsg("El campo N° de contrato esta vacio.");
                return true;
            }
        }
        if (this.categoriaCode == 'CAL') {
            if ((this.contrato == "" || !this.contrato) && (this.comprobante == "" || !this.comprobante)) {
                this.mensajeComponent.setErrorMsg("Debe completar Campo N° de contrato o CCPP.");
                return true;
            }
        }
        if (this.categoriaCode == 'DISCAL') {
            if (!this.rubrosSelected || !this.rubrosSelected.length) {
                this.mensajeComponent.setErrorMsg("Debe seleccionar al menos un rubro.");
                return true;
            }
            if (!this.validarRubrosSeleccionados()) {
                this.mensajeComponent.setErrorMsg("Debe cargar un valor de discrepancia para cada rubro seleccionado.");
                return true;
            }
            if ((!this.material)) {
                this.mensajeComponent.setErrorMsg("Debe seleccionar un material.");
                return true;
            }
            this.rubro = this.rubrosSelected.map(rubro => `${rubro.caracteristica} - ${rubro.resultadoCalado} Calado - Discrepa ${rubro.discrepanciaCalidad} %`).join("\n");
        }
        if (this.categoriaCode == 'COM') {
            if (this.comprobante == "" || !this.comprobante) {
                this.mensajeComponent.setErrorMsg("El campo N° de Factura esta vacio.");
                return true;
            }
        }
        if (this.categoriaCode == 'APP') {
            if (this.comprobanteExtra == "" || !this.comprobanteExtra) {
                this.mensajeComponent.setErrorMsg("El campo Material esta vacio.");
                return true;
            }

            if ((this.comprobante == "" || !this.comprobante) && (this.contrato == "" || !this.contrato)) {
                this.mensajeComponent.setErrorMsg("Debe completar Campo N° de contrato o CCPP.");
                return true;
            }
        }
        if (this.categoriaCode == 'PES') {
            if (this.contrato == "" || !this.contrato) {
                this.mensajeComponent.setErrorMsg("El campo N° de contrato esta vacio.");
                return true;
            }
        }
        if (this.categoriaCode == 'MATBA' && this.subcategoriaCode == 'CAL') {
            if ((this.comprobante == "" || !this.comprobante) && (this.comprobanteExtra == "" || !this.comprobanteExtra)) {
                this.mensajeComponent.setErrorMsg("Debe completar Campo carátula o CCPP.");
                return true;
            }
        }
        if (this.categoriaCode == 'FLET' && this.subcategoriaCode == 'CCP') {
            if (this.comprobante == "" || !this.comprobante) {
                this.mensajeComponent.setErrorMsg("El campo CCPP esta vacio.");
                return true;
            }
        }
        if (this.categoriaCode == 'PAG') {
            this.fechaPago = (<HTMLInputElement>document.querySelectorAll('[fechaInicioInput]')[0]).value;
            if ((this.comprobanteExtra == "" || !this.comprobanteExtra) && (this.comprobante == "" || !this.comprobante)
                && (this.contrato == "" || !this.contrato) && (this.fechaPago == "" || !this.fechaPago)) {
                this.mensajeComponent.setErrorMsg("Debe completar uno de los campos obligatorios.");
                return true;
            }
        }
        if (this.categoriaCode == 'CRCPE') {
            if (this.comprobante == "" || !this.comprobante) {
                this.mensajeComponent.setErrorMsg("El campo Nro de CTG esta vacio.");
                return true;
            }
        }
    }



    postConsulta() {
        this.blockUI.start('Generando Consulta');
        this.spinnerComponent.showIt();

        if (this.esCorredor) {
            if (this.proveedorSelected) {
                this.codigoProveedor = this.proveedorSelected.idVendedor;
                this.razonSocialProveedor = this.proveedorSelected.descVendedor;
            }
            this.razonSocialCorredor = this.nombre;
        }
        else {
            this.razonSocialProveedor = this.nombre;
        }

        if (this.validarConsulta()) {
            this.spinnerComponent.hideIt();
            this.blockUI.stop();
            return;
        }

        if (this.fechaPago != '' && this.fechaPago != null) {
            var dateParts = this.fechaPago.split("-");
            this.fecha = new Date(+dateParts[0], +dateParts[1] - 1, +dateParts[2]);
        }

        this.Detalle = {
            Consulta_Id: 0, Fecha: this.fecha,
            ComprobanteNo: this.comprobante, OtroComprobanteNo: this.comprobanteExtra,
            ContratoNo: this.contrato, Importe: this.importe,
            Impuesto: this.impuesto, BolsaEmisoraOblea: this.bolsaEmisoraOblea,
            Material_Id: this.material ? this.material.MaterialId : null,
            Rubro: this.rubro
        }

        this.consulta = {
            CodigoCorredor: this.codigoCorredor, RazonSocialCorredor: this.razonSocialCorredor,
            CodigoProveedor: this.codigoProveedor, RazonSocialProveedor: this.razonSocialProveedor,
            Categoria_Id: this.categoria.Id, Detalle: this.Detalle,
            SubCategoria_Id: this.subcategoria.Id, Asunto: this.asunto
        }

        let comentario: Comentario = { consulta_Id: 0, Detalle: this.nuevoComentario, Fecha: new Date(), Recordado: false, FechaRecordado: new Date() };

        try {
            this.subscription = this.service.AgregarConsulta(this.consulta, comentario, this.listaArchivos).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                        this.blockUI.stop();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                        this.blockUI.stop();
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                        this.blockUI.stop();
                    } else {
                        this.spinnerComponent.hideIt();
                        this.blockUI.stop();
                        this.consulta.Id = result.ConsultaDto.Id;
                        if (result.Mensaje != undefined && result.Mensaje != "") {
                            this.confirmationService.confirm({
                                message: result.Mensaje + '\n¿Desea proceder con la carga?',
                                accept: () => {
                                    this.goToSeccion('/consulta/mis-consultas');
                                },
                                reject: () => {
                                    this.anularConsulta(result.Mensaje);
                                }
                            });
                        }
                        else {
                            this.goToSeccion('/consulta/mis-consultas');
                        }
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
    }

    anularConsulta(motivoRechazo: string) {
        try {
            this.subscription = this.service.AnularConsulta(this.consulta.Id, motivoRechazo).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                        this.blockUI.stop();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                        this.blockUI.stop();
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                        this.blockUI.stop();
                    } else {
                        this.spinnerComponent.hideIt();
                        this.blockUI.stop();
                        if (result.Mensaje != undefined && result.Mensaje != "") {
                            this.floatMsgService.setSuccessMsg(result.Mensaje);
                        }
                        else {
                            this.goToSeccion('/consulta/mis-consultas');
                        }
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
    }

    setSubcategorias(categoria) {
        this.categoriaCount = 1;
        this.subcategoriaCode = null;
        this.categoria = categoria;
        this.categoriaCode = categoria.Code;
        this.subcategoriasList = [];

        this.Avisos(this.categoriaCode);

        this.subcategorias.forEach(x => {
            if (x.CategoriaId == categoria.Id) {
                if (x.Code == "INF" && this.esCorredor) {
                }
                else if (x.Code == "CAP" && !this.esCorredor) {
                }
                else {
                    this.subcategoriasList.push(x);
                }
            }
        });

        if (this.subcategoriasList.length == 0) {
            this.tieneSubcategorias = false;
        }
        else {
            this.tieneSubcategorias = true;
        }
    }

    borrarArchivo(i: number) {
        this.listaArchivos.splice(i, 1);
    }

    Avisos(categoriaCode) {
        this.mensajeComponent.setMsgsEmpty();
        if (categoriaCode == "BOL" && this.subcategoriaCode == "OPC") {
            this.mensajeComponent.setInfoMsg("Recuerde Adjuntar liquidación y la oblea emitida por bolsa");
            return true;
        }
        if (categoriaCode == "ACT" && this.subcategoriaCode == "IMP") {
            this.mensajeComponent.setInfoMsg("Recuerde Adjuntar Constancia");
            return true;
        }
        if (categoriaCode == "ACT" && this.subcategoriaCode == "CM05") {
            this.mensajeComponent.setInfoMsg("Recuerde adjuntar un único formulario CM05.");
            return true;
        }
        this.mensajeComponent.setMsgsEmpty();
    }

    handleCorrectCaptcha(event: any) {
        this.captchaOk = event;
    }

    setCodeSubcategoria(subcategoria) {
        this.subcategoriaCount = 1;
        this.subcategoriaCode = subcategoria.Code;
        this.Avisos(this.categoriaCode)
    }

    generarVariable() {
        this.reclamoImpositivo.RazonSocialEmpresa = this.nombre;
        this.reclamoImpositivo.Reclamos = [
            { Fecha: "", Importe: "", Certificado: "" }
        ]
    }

    agregarReclamo() {
        this.reclamoImpositivo.Reclamos.push({ Fecha: "", Importe: "", Certificado: "" });
    }

    eliminarReclamo(numeroReclamo: number) {
        this.reclamoImpositivo.Reclamos.forEach((value, index) => {
            if (this.reclamoImpositivo.Reclamos.indexOf(value) == numeroReclamo) this.reclamoImpositivo.Reclamos.splice(index, 1);
        });
    }

    setFechaReclamo(numeroReclamo: number, event: Event) {
        this.reclamoImpositivo.Reclamos[numeroReclamo].Fecha = event
    }

    validarReclamo() {
        this.mensajeComponent.setMsgsEmpty();
        if (this.reclamoImpositivo.RazonSocialProveedor == "" || !this.reclamoImpositivo.RazonSocialProveedor) {
            this.mensajeComponent.setErrorMsg("El campo razón social proveedor esta vacío.");
            return true;
        }
        if (this.reclamoImpositivo.Dni == "" || !this.reclamoImpositivo.Dni) {
            this.mensajeComponent.setErrorMsg("El campo DNI esta vacío.");
            return true;
        }
        if (this.reclamoImpositivo.Cuit == "" || !this.reclamoImpositivo.Cuit) {
            this.mensajeComponent.setErrorMsg("El campo Cuit esta vacío.");
            return true;
        }
        if (this.reclamoImpositivo.Vinculo == "" || !this.reclamoImpositivo.Vinculo) {
            this.mensajeComponent.setErrorMsg("El campo vinculo esta vacío.");
            return true;
        }
        this.reclamoImpositivo.Reclamos.forEach(reclamo => {
            if (reclamo.Certificado == "" || !reclamo.Certificado) {
                this.mensajeComponent.setErrorMsg("El campo N° certificado esta vacío.");
                return true;
            }
            if (reclamo.Fecha == "" || !reclamo.Fecha) {
                this.mensajeComponent.setErrorMsg("El campo fecha esta vacío.");
                return true;
            }
            if (reclamo.Importe == "" || !reclamo.Importe) {
                this.mensajeComponent.setErrorMsg("El campo importe esta vacío.");
                return true;
            }
        });

        return false;
    }

    generarReclamoImpositivo() {
        this.blockUI.start('Generando documento.');
        this.spinnerComponent.showIt();

        if (this.validarReclamo()) {
            this.spinnerComponent.hideIt();
            this.blockUI.stop();
            return;
        }

        try {
            this.subscription = this.service.generarReclamoImpositivo(this.reclamoImpositivo).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                        this.blockUI.stop();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                        this.blockUI.stop();
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                        this.blockUI.stop();
                    } else {
                        this.spinnerComponent.hideIt();
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "application/octet-stream",
                        });

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
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
    }

    public extraOnDestroy(): void {
        this.sendDataService.limpiarDatosLiquidacionObservados();
        this.sendDataService.limpiarDatosDisconformidadCalidades();
    }

    setValoresInicialesParaLiquidacionObservada() {
        const code = this.datosLiquidacionObservada.Tipo == "Final" ? "FIN" : "PAR";
        const categoria = this.categorias.find(categoria => categoria.Code === code)
        if (categoria)
            this.setSubcategorias(categoria)
        const subCategoria = this.subcategoriasList.find(subcategoria => subcategoria.Nombre === "No registradas, Observadas y Rechazadas")
        if (subCategoria) {
            this.subcategoria = subCategoria;
            this.setCodeSubcategoria(categoria)
        }
        this.asunto = `Liquidación observada - nro comprobante ${this.datosLiquidacionObservada.NroComprobante}`
        this.comprobante = this.datosLiquidacionObservada.NroComprobante
        this.contrato = this.datosLiquidacionObservada.NroContrato

        if (this.esCorredor)
            this.setValorProveedor()
    }

    setValoresInicialesParaDisconformidadCalidades() {
        this.blockUI.start('Cargando calidades');
        this.cartaPorteService
            .detalleCalidades(this.datosDisconformidadCalidades.NroCCPP)
            .pipe(finalize(() => this.blockUI.stop()))
            .subscribe(({ info, error, data }) => {
                if (error || info) {
                    this.mensajeComponent.setErrorMsg(error || info)
                    return;
                }
                this.rubrosOptions = data
                this.nuevoComentario =
                    [
                        'Característica - Calado - Cámara',
                        ...data.map(calidad => `${calidad.caracteristica} - ${calidad.resultadoCalado} - ${calidad.resultadoCamara}`)
                    ].join('\n')
            })
        const categoria = this.categorias.find(categoria => categoria.Code === "DISCAL")
        if (categoria)
            this.setSubcategorias(categoria)

        this.asunto = `Disconformidad con calidades ${this.datosDisconformidadCalidades.Material || ''}`
        this.comprobante = this.datosDisconformidadCalidades.NroCCPP
        this.contrato = this.datosDisconformidadCalidades.NroContrato

        if (this.esCorredor)
            this.setValorProveedor()
    }

    setValorProveedor() {
        this.selectProveedor.setSelected({
            idVendedor: this.codigoCorredor,
            descVendedor: sessionStorage.getItem('nombre')
        })
    }

    validarRubrosSeleccionados() {
        return this.rubrosSelected.every(rubro => !!rubro.discrepanciaCalidad)
    }
}