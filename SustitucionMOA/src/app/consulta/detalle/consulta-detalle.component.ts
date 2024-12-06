import { Component, ViewChild, ElementRef, Input, HostListener } from "@angular/core";
import { ActivatedRoute, } from "@angular/router";
import { MensajeComponent } from "./../../common/view-child/mensaje/mensaje.component";
import { SpinnerComponent } from "./../../common/view-child/spinner/spinner.component";
import { SessionDataService } from "./../../common/services/SessionDataService";
import { SecurityService } from "./../../common/services/SecurityService";
import {
    DropdownComponent,
} from "./../../common/view-child/dropdown/dropdown.component";
import { SpinnerSmallComponent } from "./../../common/view-child/spinner-small/spinner-small.component";
import { NavService } from "./../../common/services/NavService";
import { FloatMsgService } from "./../../common/services/FloatMsgService";
import { ModalService } from "./../../common/services/ModalService";
import { ConsultaService } from "../consulta.service";
import { BaseComponent } from "../../common/base-components/base-component";
import {
    Comentario,
    Categoria,
    EstadoConsulta,
    Subcategoria,
    Consulta,
} from "../consulta";
import { SelectItem } from "primeng/components/common/selectitem";
import { BlockUI, NgBlockUI } from "ng-block-ui";
import {
    UploadEvent,
    FileSystemFileEntry,
    FileSystemDirectoryEntry,
} from "ngx-file-drop";
import { DomSanitizer } from '@angular/platform-browser';
import { AngularEditorModule, AngularEditorConfig, AngularEditorComponent } from "@kolkov/angular-editor";
import { GET_ANGULAR_EDITOR_CONFIG, eliminarBotonesExtraEditor } from "../../common/configs/angularEditor.configs";
import { ComentarioAutoguardado } from "../../modelos/consulta/comentarioAutoguardado";
import { FileStorageService } from "../../common/services/FileStorageService";

declare var $: any;

@Component({
    selector: "consulta-detalle",
    templateUrl: `consulta-detalle.component.html`,
    styleUrls: [
        'consulta-detalle.component.css',
        'estilo-solo-detalle.component.css'
    ],
    providers: [{ provide: ConsultaService, useClass: ConsultaService }],
})
export class DetalleConsultaComponent extends BaseComponent {
    @BlockUI() blockUI: NgBlockUI;

    @ViewChild("dropdown_categoria")
    protected categoriaDropdownComponent: DropdownComponent;

    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    @ViewChild("spinnerModal")
    protected spinnerModal: SpinnerSmallComponent;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild("detalleConsulta")
    protected detalleConsulta: ElementRef;

    @ViewChild("angularEditor") editor: AngularEditorComponent;

    @HostListener('document:click', ['$event'])
    public onDocumentClick(event: MouseEvent): void {
        const targetElement = event.target as HTMLElement;
        var img = targetElement;
        if (img.className == "galeryimg col-md-12 cursor-pointer") {
            $('#myModal2').modal('show');
            var modalImg = document.getElementById("img01");
            $(modalImg).attr("src", $(img).attr("src"));
            $(document.getElementsByClassName("modal-backdrop")[0]).css("z-index", "0");
            $(document.getElementsByClassName("modal-backdrop")[0]).css("background-color", "none");
        } else {
            $('#myModal2').modal('hide');
        }
    }
    //    @HostListener('document:paste', ['$event'])
    //    public onPaste(e: any): void {
    //        e.preventDefault();
    //        const text = (e.originalEvent || e).clipboardData.getData('text/plain');
    //        window.document.execCommand('insertText', false, text);
    //    }
    constructor(
        private route: ActivatedRoute,
        protected service: ConsultaService,
        protected navService: NavService,
        protected securityService: SecurityService,
        protected sessionDataService: SessionDataService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService,
        protected html_sanitizer: DomSanitizer,
        private fileStorageService: FileStorageService
    ) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    @Input() consultaId: any;
    categoriaSelected: any;
    estados: EstadoConsulta[];
    categorias: Categoria[];
    subcategorias: Subcategoria[];
    causasConsulta: any;
    causaConsulta: any;
    causaConsultaId: number = 0;

    subcategoriasList: SelectItem[];
    estadosList: SelectItem[];
    categoriasList: SelectItem[];

    subcategoriaId: any;
    estadoId: number;
    categoriaId: number;
    MostrarDatosAdicionales: boolean = false;
    listaArchivos: Array<File> = new Array<File>();

    tieneSubcategorias: boolean = false;
    consulta: Consulta = {} as Consulta;
    comentariosList: any;
    estadoConsulta: number;
    file: any;
    fecha: any;
    hora: any;
    username = sessionStorage.getItem("username");
    detalle: string = "";
    esInterno = this.isAuthorized("CONSULTA ABM");
    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";
    cuitUsuarioSesion: string = sessionStorage.getItem("cuit");

    datosExtra = [];

    htmlContent: string;

    config: AngularEditorConfig = GET_ANGULAR_EDITOR_CONFIG();

    readonly AUTOGUARDADO_LOCALSTORAGE_KEY: string = 'ConsultaComentarioAutoguardado';
    debeAutoguardarComentario: boolean = false;

    datosExtrasMinimizado = false;
    @Input() modalMaximizado = false;
    iconDatosExtras = 'pi pi-minus'

    checkPermisos() {
        this.securityService.tienePermisoRedirect("CONTACTO MAIL");
    }

    setTabs() {
        this.setMenuSeccionTab("consulta", "detalle");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.jqueryOnInit();
        this.getCombos();
        this.recuperarComentarioAutoguardado();
        this.recuperarArchivosAutoguardados();
    }

    ngAfterViewInit(): void {
        this.scrollBottom();
        this.eliminarBotonesExtra();
    }

    scrollBottom() {
        this.detalleConsulta.nativeElement.scrollTop =
            this.detalleConsulta.nativeElement.scrollHeight;
    }

    isAuthorized(permiso: string) {
        return this.securityService.tienePermiso(permiso);
    }

    fileOver(event) {
        console.log(event);
    }

    fileLeave(event) {
        console.log(event);
    }

    public dropped(event: UploadEvent) {
        for (const droppedFile of event.files) {
            // Is it a file?
            if (droppedFile.fileEntry.isFile) {
                const fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
                fileEntry.file((file: File) => {
                    this.listaArchivos.push(file);
                    this.autoguardarArchivo(file);
                });
            } else {
                // It was a directory (empty directories are added, otherwise only files)
                const fileEntry =
                    droppedFile.fileEntry as FileSystemDirectoryEntry;
            }
        }
        this.file = this.listaArchivos;
    }

    async cargarArchivo(event: any) {
        let fileList: FileList = event.target.files;
        // let file;

        if (fileList.length > 0) {
            this.file = fileList;
            for (let i = 0; i < fileList.length; i++) {
                let file = fileList[i];
                this.listaArchivos.push(file);
                this.autoguardarArchivo(file);
            }
        }

        let $formInput = $("input[type=file]");
        $formInput.val(null);
    }

    comentarioPropio(comentario: Comentario): boolean {
        return (this.esInterno && comentario.CreadorInterno) ||
            !this.esInterno && !comentario.CreadorInterno;
    }

    actualizarCombos() {
        this.spinnerSmallComponent.showIt();
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();

        if (this.causaConsulta != undefined) {
            this.causaConsultaId = this.causaConsulta.Id;
        }

        this.subscription = this.service
            .actualizarCombos(
                this.consultaId,
                this.estadoId,
                this.categoriaId,
                this.subcategoriaId,
                this.causaConsultaId
            )
            .subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    this.spinnerSmallComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (
                        result.error != undefined &&
                        result.error != ""
                    ) {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result.data);
                    }
                },
                (error) => {
                    this.spinnerModal.hideIt();
                }
            );
    }

    validar() {
        this.mensajeComponent.setMsgsEmpty();
        if (
            (this.detalle == undefined || this.detalle == "") &&
            this.file == null
        ) {
            this.mensajeComponent.setErrorMsg(
                "Debe adjuntar un archivo o hacer un comentario."
            );
            return true;
        }
    }

    postComentario() {
        this.blockUI.start("Enviando comentario");
        if (this.validar()) {
            this.blockUI.stop();
            return;
        }
        this.Paste(null);
        let borderAnterior = `border=${String.fromCharCode(
            34
        )}0${String.fromCharCode(34)}`;
        let borderNuevo = `border=${String.fromCharCode(
            34
        )}1${String.fromCharCode(34)}`;

        this.detalle = this.detalle.replace(borderAnterior, borderNuevo);

        this.mensajeComponent.setMsgsEmpty();
        let comentario: Comentario = {
            consulta_Id: this.consultaId,
            Detalle: this.detalle,
            Fecha: new Date(),
            Recordado: false,
            FechaRecordado: new Date(),
        };
        this.subscription = this.service
            .agregarComentario(this.consultaId, comentario, this.listaArchivos)
            .subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                        this.blockUI.stop();
                    } else if (
                        result.error != undefined &&
                        result.error != ""
                    ) {
                        this.mensajeComponent.setErrorMsg(result.error);
                        this.blockUI.stop();
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                        this.blockUI.stop();
                    } else {
                        localStorage.removeItem(this.AUTOGUARDADO_LOCALSTORAGE_KEY);
                        this.fileStorageService.borrarTodosArchivosContacto();
                        this.getDetalleConsulta();
                        this.mensajeComponent.setSuccessMsg(
                            "Comentario enviado correctamente"
                        );
                        this.blockUI.stop();
                        //detallecomentario.innerHTML = "";
                        this.detalle = "";
                        this.file = null;
                        this.listaArchivos = [];
                    }
                },
                (error) => {
                    this.spinnerModal.hideIt();
                }
            );
    }

    setDatosExtra() {
        this.datosExtra = [
            { Nombre: "N° Consulta", Value: this.consulta.Id, NewLine: false },
            {
                Nombre: "Mail Usuario",
                Value: this.consulta.Usuario.Mail,
                NewLine: false,
            },
            {
                Nombre: "CUIT Usuario",
                Value: this.consulta.Usuario.CUIT,
                NewLine: false,
            },
            {
                Nombre: "Razón Social Corredor",
                Value: this.consulta.RazonSocialCorredor,
                NewLine: false,
            },
            {
                Nombre: "Codigo Corredor",
                Value: this.consulta.CodigoCorredor,
                NewLine: false,
            },
            {
                Nombre: "Razón Social Proveedor",
                Value: this.consulta.RazonSocialProveedor,
                NewLine: false,
            },
            {
                Nombre: "Codigo Proveedor",
                Value: this.consulta.CodigoProveedor,
                NewLine: false,
            },
            {
                Nombre: "Categoria",
                Value: this.consulta.Categoria.Nombre,
                NewLine: false,
            },
            {
                Nombre: "SubCategoria",
                Value: this.consulta.SubCategoria.Nombre,
                NewLine: false,
            },
            {
                Nombre: "N° de Contrato",
                Value: this.consulta.ContratoNo,
                NewLine: true,
            },
            { Nombre: "Importe", Value: this.consulta.Importe, NewLine: false },
            {
                Nombre: "Impuesto",
                Value: this.consulta.Impuesto,
                NewLine: false,
            },
            {
                Nombre: this.getNombreComprobante(),
                Value: this.consulta.ComprobanteNo,
                NewLine: true,
            },
            {
                Nombre: this.getNombreComprobanteExtra(),
                Value: this.consulta.OtroComprobanteNo,
                NewLine: true,
            },
            {
                Nombre: "N° Orden",
                Value: this.consulta.OrdenId,
                NewLine: false
            },
            {
                Nombre: "Patente Chasis",
                Value: this.consulta.PatenteChasis,
                NewLine: false
            },
            {
                Nombre: "Rubro",
                Value: this.consulta.Rubro,
                NewLine: true
            }
        ];
    }

    getNombreComprobanteExtra() {
        if (
            this.consulta.Categoria.Code == "REI" &&
            this.consulta.SubCategoria.Code == "PER"
        )
            return "Cliente";

        if (this.consulta.Categoria.Code == "APP") return "Material";

        if (
            this.consulta.Categoria.Code == "MATBA" &&
            this.consulta.SubCategoria.Code == "CAL"
        )
            return "Carátula";

        if (
            (this.consulta.Categoria.Code == "FLET" &&
                this.consulta.SubCategoria.Code == "PDF") ||
            (this.consulta.Categoria.Code == "FLET" &&
                this.consulta.SubCategoria.Code == "CCP")
        )
            return "N° Proforma";

        return "Otro Comprobante";
    }

    getNombreComprobante() {
        if (this.consulta.SubCategoria.Code == "RET")
            return "N° Salida de pago";

        if (
            this.consulta.SubCategoria.Code == "PER" ||
            this.consulta.Categoria.Code == "COM" ||
            (this.consulta.Categoria.Code == "PROVG" &&
                this.consulta.SubCategoria.Code == "VENC") ||
            (this.consulta.Categoria.Code == "PROVG" &&
                this.consulta.SubCategoria.Code == "POTR")
        )
            return "N° de factura";

        if (
            this.consulta.SubCategoria.Code == "NROR" ||
            this.consulta.Categoria.Code == "FINCOR" ||
            this.consulta.Categoria.Code == "FINDIR"
        )
            return "N° COE";

        if (
            this.consulta.Categoria.code == "CAL" ||
            this.consulta.Categoria.Code == "APP" ||
            (this.consulta.Categoria.Code == "MATBA" &&
                this.consulta.SubCategoria.Code == "CAL") ||
            (this.consulta.Categoria.Code == "FLET" &&
                this.consulta.SubCategoria.Code == "CCP")
        )
            return "CCPP";

        return "Comprobante";
    }

    cambiarEstadoPorCode(code: string) {
        var estadoIdGestion;
        this.estados.forEach((x) => {
            if (x.Code == code) {
                estadoIdGestion = x.Id;
            }
        });
        this.mensajeComponent.setMsgsEmpty();
        this.subscription = this.service
            .actualizarEstado(estadoIdGestion, this.consultaId)
            .subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (
                        result.error != undefined &&
                        result.error != ""
                    ) {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(
                            "El estado de la consulta cambió correctamente"
                        );
                        this.getDetalleConsulta();
                    }
                },
                (error) => {
                    this.spinnerModal.hideIt();
                }
            );
    }

    disable() {
        if (!this.consulta.EstadoConsulta)
            return true;
        if (
            this.consulta.EstadoConsulta.Code != "GES" &&
            this.consulta.EstadoConsulta.Code != "DOC" &&
            !this.esInterno
        ) {
            return true;
        }

        return false;
    }

    recordarComentario() {
        let borderAnterior = `border=${String.fromCharCode(
            34
        )}0${String.fromCharCode(34)}`;
        let borderNuevo = `border=${String.fromCharCode(
            34
        )}1${String.fromCharCode(34)}`;

        this.detalle = this.detalle.replace(borderAnterior, borderNuevo);

        this.mensajeComponent.setMsgsEmpty();
        this.subscription = this.service
            .recordarComentario(this.consultaId)
            .subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (
                        result.error != undefined &&
                        result.error != ""
                    ) {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result);
                    }
                },
                (error) => {
                    this.spinnerModal.hideIt();
                }
            );
    }

    descargarArchivo(archivoId: number) {
        this.service.DescargarArchivo(archivoId).subscribe(
            (result) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else {
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
                        return false;
                    }
                }
            },
            (error) => {
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    getCombos() {
        try {
            this.subscription = this.service.getCombos(true, true).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (
                        result.error != undefined &&
                        result.error != ""
                    ) {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.categorias = result.categorias;
                        this.categoriasList = [];
                        this.categorias.forEach((x) =>
                            this.categoriasList.push({
                                label: x.Nombre,
                                value: x.Id,
                            })
                        );
                        this.estados = result.estados;
                        this.estadosList = [];
                        this.estados.forEach((x) =>
                            this.estadosList.push({
                                label: x.Descripcion,
                                value: x.Id,
                            })
                        );
                        this.subcategorias = result.subcategorias;
                        this.subcategoriasList = [];
                        this.subcategorias.forEach((x) =>
                            this.subcategoriasList.push({
                                label: x.Nombre,
                                value: x.Id,
                            })
                        );
                        /*
                        if(this.subcategoriasList.length > 0){
                            this.tieneSubcategorias = true;
                        }*/
                        this.causasConsulta = result.causas;
                        this.getDetalleConsulta();
                    }
                },
                (error) => {
                    this.floatMsgService.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    getDetalleConsulta() {
        this.subscription = this.service
            .getDetalleConsulta(this.consultaId)
            .subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (
                        result.error != undefined &&
                        result.error != ""
                    ) {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        result.Comentarios = result.Comentarios.map((x) => {
                            x.Fecha = new Date(
                                this.getDateFromAspNetFormat(x.Fecha)
                            );
                            if (x.ComentarioRecordados.length >= 1) {
                                x.ComentarioRecordados.forEach((cr) => {
                                    cr.FechaRecordado = new Date(
                                        this.getDateFromAspNetFormat(
                                            cr.FechaRecordado
                                        )
                                    );
                                });
                            }
                            return x;
                        });
                        this.consulta = result;
                        this.setDatosExtra();
                        if (this.consulta.EstadoConsulta.Code == "INI" && this.esInterno) {
                            this.cambiarEstadoPorCode("GES");
                        }
                        if (
                            this.consulta.EstadoConsulta.Code == "GESRTA" &&
                            this.esInterno
                        ) {
                            this.cambiarEstadoPorCode("GES");
                        }
                        this.consulta.FechaCreacion = new Date(
                            this.getDateFromAspNetFormat(
                                this.consulta.FechaCreacion
                            )
                        );
                        this.consulta.Fecha = new Date(
                            this.getDateFromAspNetFormat(this.consulta.Fecha)
                        );

                        this.estadoId = result.EstadoConsultaId;
                        this.categoriaId = result.CategoriaId;
                        this.subcategoriaId = result.SubCategoriaId;
                        this.subcategoriasInicial();


                        try {
                            setTimeout(() => {
                                this.scrollBottom();
                            }, 200);
                        } catch { }
                    }
                },
                (error) => {
                    this.spinnerModal.hideIt();
                }
            );
    }
    sameAsHtml(html_content) {
        return this.html_sanitizer.bypassSecurityTrustHtml(html_content);
    }

    removerEstilos(ele) {
        ele.removeAttribute("style");

        if (ele.childNodes.length > 0) {
            for (let child in ele.childNodes) {
                /* filter element nodes only */
                if (ele.childNodes[child].nodeType == 1)
                    this.removerEstilos(ele.childNodes[child]);
            }
        }
    }

    Paste(e) {
        setTimeout(() => {
            let divComentario = document.getElementsByClassName(
                "angular-editor-textarea"
            )[0];

            divComentario.childNodes.forEach((element) => {
                if (element.nodeType == 1) this.removerEstilos(element);
            });

            let borderAnterior = `border=${String.fromCharCode(
                34
            )}0${String.fromCharCode(34)}`;
            let borderNuevo = `border=${String.fromCharCode(
                34
            )}1${String.fromCharCode(34)}`;

            divComentario.innerHTML = divComentario.innerHTML.replace(borderAnterior, borderNuevo);

        }, 200);
    }

    eliminarBotonesExtra() {

        let divToolBar = document.getElementsByClassName(
            "angular-editor-toolbar"
        )[0];
        const subscript = $("#subscript-");
        const superscript = $("#superscript-");

        const editorTextArea = $(".angular-editor-textarea");
        const editorButton = $(".angular-editor-button");
        eliminarBotonesExtraEditor(divToolBar, subscript, superscript, editorTextArea, editorButton)
    }

    subcategoriasInicial() {
        this.subcategoriasList = [];
        this.subcategorias.forEach((x) => {
            if (x.CategoriaId == this.categoriaId) {
                this.subcategoriasList.push({ label: x.Nombre, value: x.Id });
            }
        });

        if (this.subcategoriasList.length > 0) {
            this.tieneSubcategorias = true;
        } else {
            this.tieneSubcategorias = false;
        }
    }

    setSubcategorias(categoriasSeleccionadas) {
        if (this.subcategorias) {
            this.subcategoriasList = [];
            //this.subcategorias.filter(x=> categoriasSeleccionadas.length == 0 || categoriasSeleccionadas.map(y=> y.Id).includes(x.CategoriaId)).forEach(x => this.subcategoriasList.push({ label: x.Nombre, value: x.Id}));
            this.subcategorias.forEach((x) => {
                if (x.CategoriaId == categoriasSeleccionadas) {
                    this.subcategoriasList.push({
                        label: x.Nombre,
                        value: x.Id,
                    });
                }
            });
        }

        if (this.subcategoriasList.length > 0) {
            this.subcategoriaId = this.subcategoriasList[0].value;
            this.tieneSubcategorias = true;
        } else {
            this.tieneSubcategorias = false;
            this.subcategoriaId = null;
        }
        return this.subcategoriasList;
    }

    jqueryOnInit() {
        $(".adjuntarArchivo").click(function () {
            $(".adjuntarArchivo1").click();
        });
        $(".enviarComentario").click(function (e) {
            e.preventDefault();
        });
        $(".archivosDescarga").click(function (e) {
            e.preventDefault();
        });
        $(".botonActualizarCombos").click(function (e) {
            e.preventDefault();
        });
    }

    borrarArchivo(i: number) {
        let archivoABorrar = this.listaArchivos[i];
        this.listaArchivos.splice(i, 1);
        this.fileStorageService.borrarArchivoContactoAutoguardado(this.consultaId, archivoABorrar);
    }

    reabrirConsulta(): void {
        this.mensajeComponent.setMsgsEmpty();
        this.subscription = this.service
            .reabrirConsulta(this.consultaId)
            .subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (
                        result.error != undefined &&
                        result.error != ""
                    ) {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(
                            "La consulta se reabrió correctamente."
                        );
                        this.getDetalleConsulta();
                    }
                },
                (error) => {
                    this.spinnerModal.hideIt();
                }
            );
    }

    toggleDatosExtras() {
        this.datosExtrasMinimizado = !this.datosExtrasMinimizado;
        this.iconDatosExtras = this.datosExtrasMinimizado ? 'pi pi-plus' : 'pi pi-minus'
    }

    comentarioOnChange() {
        if (!this.debeAutoguardarComentario) {
            this.debeAutoguardarComentario = true;
            setTimeout(() => {
                let comentarioAGuardar: ComentarioAutoguardado = { ConsultaId: this.consultaId, Comentario: this.detalle };
                localStorage.setItem(this.AUTOGUARDADO_LOCALSTORAGE_KEY, JSON.stringify(comentarioAGuardar));
                this.debeAutoguardarComentario = false;
            }, 3000)
        }
    }

    recuperarComentarioAutoguardado() {
        let comentarioRecuperadoText = localStorage.getItem(this.AUTOGUARDADO_LOCALSTORAGE_KEY);
        if (comentarioRecuperadoText) {
            let comentarioRecuperado: ComentarioAutoguardado = JSON.parse(comentarioRecuperadoText);
            if (comentarioRecuperado && comentarioRecuperado.ConsultaId == this.consultaId) {
                this.detalle = comentarioRecuperado.Comentario;
            }
        }
    }

    autoguardarArchivo(archivo: File) {
        this.fileStorageService.guardarArchivoContacto(this.consultaId, archivo);
    }

    recuperarArchivosAutoguardados() {
        this.fileStorageService.obtenerArchivosContacto(this.consultaId)
            .then(archivosAutoguardados => {
                if (archivosAutoguardados && archivosAutoguardados.length > 0) {
                    archivosAutoguardados.forEach(x => this.listaArchivos.push(x.archivo));
                }
            })
            .catch(error => {
                console.error('Error al obtener archivos de contacto', error);
            });
    }
}
