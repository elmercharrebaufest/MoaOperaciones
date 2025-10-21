import { DatePipe } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from "@angular/router";
import { BaseComponent } from '../../common/base-components/base-component';
import { Notificacion } from '../../common/models/notificacion';
import { Rol } from '../../common/models/rol';
import { TipoUsuario } from '../../common/models/tipoUsuario';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { UsuarioService } from '../../usuario/usuario.service';
import { NotificacionesService } from '../notificaciones.service';
import { AngularEditorConfig } from "@kolkov/angular-editor";
import { Adjuntos } from '../../common/models/adjuntos';

import { MessageService } from 'primeng/api';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

declare var $: any;

@Component({
    selector: 'app-alta-notificaciones',
    templateUrl: './alta-notificaciones.component.html',
    styleUrls: ['./alta-notificaciones.component.css'],
    providers: [NotificacionesService, DatePipe, MessageService]
})
export class AltaNotificacionesComponent extends BaseComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild('horaInicioSelect') horaInicioSelect: ElementRef;

    rolesUsuarioSeleccionado: Array<Rol> = [];
    tipoUsuarioArray: Array<TipoUsuario> = [];
    roles: Array<Rol> = [];
    notificacionId: number = 0;
    imagenPrevisualizacion: { name: string, fileAttached: File }[] = [];
    imageList: { name: string, fileAttached: File }[] = [];
    videoList: { name: string, fileAttached: File }[] = [];
    pdfList: { name: string, fileAttached: File }[] = [];


    notificacion: Notificacion = new Notificacion();
    adjuntos: Array<Adjuntos> = [];

    fecha_inicio: string;
    fecha_fin: string;
    horaInicio: number;
    fecha_creacion: string;
    selectedPrioridad: string;
    mensajeError: string = "";
    nombreError: string = "";
    detalle: string = "";
    file: any;
    listaArchivos: Array<File> = new Array<File>();

    allRoles: boolean = false;
    allTipos: boolean = false;

    titulos: Array<string> = ["Externo", "Interno"];
    rolesSeleccionados: Array<Rol> = [];


    constructor(protected service: NotificacionesService,
        protected usuarioService: UsuarioService, protected navService: NavService,
        private route: ActivatedRoute,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        public datepipe: DatePipe,
        private messageService: MessageService) {
        super(navService, securytiService, floatMsgService, modalService);
    }

    ngOnInit() {

        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) this.notificacionId = params["id"];
        });

        this.navService.setSeccionList([]);

        //this.getRolesOptions();


        this.getRolesOptions()
            .then((message) => {
                // Aquí puedes ejecutar el código que necesitas después de obtener los roles
                if (this.notificacionId > 0) {
                    this.obtenerNotificacion();
                }
            })
            .catch((error) => {
                // Manejo de errores si la obtención de roles falla
                console.error("Error en la obtención de roles:", error);
            });


        //if (this.notificacionId > 0) {
        //    this.obtenerNotificacion();
        //}
    }

    config: AngularEditorConfig = {
        editable: true,
        spellcheck: true,
        height: "auto",
        minHeight: "100px",
        maxHeight: "200px",
        width: "100%",
        minWidth: "530px",
        translate: "yes",
        enableToolbar: true,
        showToolbar: true,
        defaultParagraphSeparator: "",
        defaultFontName: "Arial",
        defaultFontSize: "5",
        fonts: [
            { class: "arial", name: "Arial" },
            { class: "times-new-roman", name: "Times New Roman" },
            { class: "calibri", name: "Calibri" },
            { class: "comic-sans-ms", name: "Comic Sans MS" },
        ],
        customClasses: [
            {
                name: "quote",
                class: "quote",
            },
            {
                name: "redText",
                class: "redText",
            },
            {
                name: "titleText",
                class: "titleText",
                tag: "h1",
            },
        ],
        uploadUrl: "v1/image",
        sanitize: true,
        toolbarPosition: "top",
    };

    guardarPrioridad() {
        if (this.selectedPrioridad === 'Alta') {
            this.notificacion.Prioridad = 1;
        } else if (this.selectedPrioridad === 'Media') {
            this.notificacion.Prioridad = 2;
        } else if (this.selectedPrioridad === 'Baja') {
            this.notificacion.Prioridad = 3;
        }
    }


    eliminarBotonesExtra() {
        let divToolBar = document.getElementsByClassName(
            "angular-editor-toolbar"
        )[0];

        let toolBars = divToolBar.childNodes;

        if (toolBars.length == 14) {
            let toolBar0 = toolBars[0];
            let toolBar2 = toolBars[2];
            let toolBar3 = toolBars[3];
            let toolBar4 = toolBars[4];
            let toolBar5 = toolBars[5];
            let toolBar6 = toolBars[6];
            let toolBar7 = toolBars[7];
            let toolBar8 = toolBars[8];
            let toolBar9 = toolBars[9];
            let toolBar10 = toolBars[10];
            let toolBar11 = toolBars[11];
            let toolBar13 = toolBars[13];

            divToolBar.removeChild(toolBar0);
            divToolBar.removeChild(toolBar2);
            divToolBar.removeChild(toolBar3);
            divToolBar.removeChild(toolBar4);
            divToolBar.removeChild(toolBar5);
            divToolBar.removeChild(toolBar6);
            divToolBar.removeChild(toolBar7);
            divToolBar.removeChild(toolBar9);
            divToolBar.removeChild(toolBar10);
            divToolBar.removeChild(toolBar11);
            divToolBar.removeChild(toolBar13);
        }

        $("#subscript-").hide();
        $("#superscript-").hide();

        $(".angular-editor-textarea").css("font-size", "large");
        $(".angular-editor-button").css("font-size", "large");
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

    fileOver(event) {
        console.log(event);
    }

    fileLeave(event) {
        console.log(event);
    }

    ngAfterViewInit(): void {

        this.eliminarBotonesExtra();

        //this.focusSection("focusSection");

        $(document).ready(function () {
            $(".form_datetime1").datetimepicker({
                format: 'dd/mm/yyyy',
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

            $(".form_datetime2").datetimepicker({
                format: 'dd/mm/yyyy',
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


    focusSection(sectionId) {
        var section = document.getElementById(sectionId);
        section.focus();
    }


    validar() {
        //this.messageService.add({ severity: 'error', summary: 'Error', detail: 'El Campo nombre debe tener al menos 3 caracteres.' });


        if (this.notificacion.Nombre === undefined || this.notificacion.Nombre.length < 3) {

            this.mensajeError = "El campo nombre debe tener al menos 3 caracteres.";

            //this.errorInput.nativeElement.focus();
            //this.enfocarInput();

            this.focusSection("Nombre");
            //this.messageService.add({ severity: 'error', summary: 'Error', detail: this.mensajeError });
            this.messageService.add({ key: 'tc', severity: 'warn', summary: 'Nombre', detail: this.mensajeError });
            //this.messageService.add({ key: 'tl', severity: 'error', summary: 'Error', detail: 'El Campo nombre debe tener al menos 3 caracteres.' });
            return false;
        }

        if (this.notificacion.Nombre !== undefined && this.notificacion.Nombre.length > 70) {
            this.mensajeError = "El campo nombre no puede tener más de 70 caracteres.";
            this.focusSection("Nombre");
            this.messageService.add({ key: 'tc', severity: 'warn', summary: 'Nombre', detail: this.mensajeError });
            return false;
        }

        if (this.fecha_inicio.length == 0) {
            this.mensajeError = "Ingrese fecha desde.";
            this.focusSection("noCursor");
            this.messageService.add({ key: 'tc', severity: 'warn', summary: 'Fecha', detail: this.mensajeError });
            return false;
        }

        if (this.fecha_inicio.length != 0 && this.fecha_fin.length != 0) {

            const fechaInicio = new Date(this.fecha_inicio);
            const fechaFin = new Date(this.fecha_fin);
            if (fechaInicio.getFullYear() > fechaFin.getFullYear() ||
                (fechaInicio.getFullYear() === fechaFin.getFullYear() && fechaInicio > fechaFin)) {
                this.mensajeError = "La fecha desde debe ser menor a la fecha hasta";
                this.focusSection("noCursor");
                this.messageService.add({ key: 'tc', severity: 'warn', summary: 'Fecha', detail: this.mensajeError });
                return false;
            }
        }

        if (this.horaInicio.toString() == "") {
            this.mensajeError = "Ingrese la hora.";
            this.focusSection("hora");
            this.messageService.add({ key: 'tc', severity: 'warn', summary: 'Hora', detail: this.mensajeError });
            return false;
        }

        if (this.fecha_fin.length == 0) {
            this.mensajeError = "Ingrese la fecha hasta.";
            this.focusSection("fechaFin");
            this.messageService.add({ key: 'tc', severity: 'warn', summary: 'Fecha', detail: this.mensajeError });
            return false;
        }

        if (this.notificacion.Mensaje === undefined || this.notificacion.Mensaje.length < 3) {
            this.mensajeError = "El campo mensaje debe tener al menos 3 caracteres.";
            this.focusSection("editor");
            this.messageService.add({ key: 'tc', severity: 'warn', summary: 'Mensaje', detail: this.mensajeError });
            return false;
        }

        //if (this.notificacion.LinkAdjunto != undefined) {
        //    if (this.notificacion.LinkAdjunto.length > 0) {
        //        if (!this.validarURL()) {
        //            this.mensajeError = "La dirección del link adjunto es inválida.";
        //            this.messageService.add({ key: 'tc', severity: 'error', summary: 'error', detail: this.mensajeError });
        //            return false;
        //        }
        //    }
        //}

        if (this.notificacion.Mensaje.length == 0) {
            this.mensajeError = "Ingrese el mensaje.";
            this.focusSection("editor");
            this.messageService.add({ key: 'tc', severity: 'warn', summary: 'Mensaje', detail: this.mensajeError });
            return false;
        }

        if (this.roles.filter(x => x.checked).length == 0) {
            this.mensajeError = "Seleccione algún rol.";
            this.focusSection("seccionRoles");
            this.messageService.add({ key: 'tc', severity: 'warn', summary: 'Mensaje', detail: this.mensajeError });
            return false;
        }


        return true;
    }

    validarURL() {
        var pattern = new RegExp('^(https?:\\/\\/)?' + // protocol
            '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|' + // domain name
            '((\\d{1,3}\\.){3}\\d{1,3}))' + // OR ip (v4) address
            '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' + // port and path
            '(\\?[;&a-z\\d%_.~+=-]*)?' + // query string
            '(\\#[-a-z\\d_]*)?$', 'i'); // fragment locator
        return !!pattern.test(this.notificacion.LinkAdjunto);
    }


    convertBase64ToFile(archivo): File {
        const mimeType = archivo.AdjuntoTipo; // Cambia el tipo MIME según tu caso
        const byteCharacters = atob(archivo.AdjuntoContenido);
        const byteArrays = [];

        for (let offset = 0; offset < byteCharacters.length; offset += 512) {
            const slice = byteCharacters.slice(offset, offset + 512);
            const byteNumbers = new Array(slice.length);

            for (let i = 0; i < slice.length; i++) {
                byteNumbers[i] = slice.charCodeAt(i);
            }

            const byteArray = new Uint8Array(byteNumbers);
            byteArrays.push(byteArray);
        }

        const blob = new Blob(byteArrays, { type: mimeType });
        const file = new File([blob], archivo.AdjuntoNombre, { type: mimeType });

        return file;
    }



    obtenerNotificacion() {
        try {
            this.blockUI.start('Cargando...');
            this.subscriptionDropDowns = this.service.getNotificacion(this.notificacionId).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.notificacion = result.data;
                        this.notificacion.FiltroRoles.forEach(element => {
                            if (this.roles.find(x => x.Id == element.toString())) {
                                this.roles.find(x => x.Id == element.toString()).checked = true;
                            } else {
                                console.log("no se pudo encontrar el elemento" + element.toString());
                            }
                        });

                        this.allRoles = this.roles.filter(x => x.checked).length == this.roles.length;
                        this.getCheckboxInternalRoles().checked = this.roles.filter(x => x.Code == 'Interno' && !x.checked).length == 0;
                        this.getCheckboxExternalRoles().checked = this.roles.filter(x => x.Code == 'Externo' && !x.checked).length == 0;

                        this.fecha_inicio = this.notificacion.FechaInicio.toString();
                        this.horaInicio = this.notificacion.HoraInicio;
                        this.fecha_fin = this.notificacion.FechaFin.toString();
                        // this.fecha_creacion = this.notificacion.FechaCreacion.toString();

                        if (this.notificacion.Prioridad === 1) {
                            this.selectedPrioridad = 'Alta';
                        } else if (this.notificacion.Prioridad === 2) {
                            this.selectedPrioridad = 'Media';
                        } else if (this.notificacion.Prioridad === 3) {
                            this.selectedPrioridad = 'Baja';
                        }

                        result.data.ArchivosAdjuntos.forEach(adjunto => {
                            const byteCharacters = atob(adjunto.AdjuntoContenido);
                            const byteArrays = [];


                        })

                        this.notificacion.ArchivosAdjuntos.forEach(adjunto => {



                            /// el archivo me llega en base64 para poder guardarlo nuevamente lo convierto a file.
                            const fileAttached = this.convertBase64ToFile(adjunto)


                            if (adjunto.AdjuntoTipo == 'previsualizacion') {
                                this.imagenPrevisualizacion.push({ name: adjunto.AdjuntoNombre, fileAttached: fileAttached });
                            }
                            if (adjunto.AdjuntoTipo.startsWith('image/')) {


                                this.imageList.push({ name: adjunto.AdjuntoNombre, fileAttached: fileAttached });
                            }
                            if (adjunto.AdjuntoTipo.startsWith('video/')) {
                                this.videoList.push({ name: adjunto.AdjuntoNombre, fileAttached: fileAttached });
                            }
                            if (adjunto.AdjuntoTipo == 'application/pdf') {
                                this.pdfList.push({ name: adjunto.AdjuntoNombre, fileAttached: fileAttached });
                            }

                        });
                    }
                    this.blockUI.stop();
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
            this.blockUI.stop();
        }
    }

    //getRolesOptions() {
    //    try {
    //        this.subscriptionDropDowns = this.usuarioService.getRoles().subscribe(
    //            (result:any) => {
    //                if (result.logout == true) {
    //                    this.sessionDataService.logout();
    //                } else if (result.error != undefined && result.error != "") {
    //                    this.mensajeComponent.setErrorMsg(result.error);
    //                } else if (result.info != undefined) {
    //                    this.mensajeComponent.setInfoMsg(result.info);
    //                } else {

    //                    this.roles = result.data.roles.filter((rol) => rol.Code === "Externo" || rol.Code === "Interno");
    //                    //this.roles = result.data.roles;
    //                }


    //            },
    //            error => {
    //                this.mensajeComponent.setErrorMsg(error.message);
    //            }
    //        );
    //    } catch (e) {
    //        this.mensajeComponent.setErrorMsg(e);
    //    }
    //}


    getRolesOptions(): Promise<any> {
        return new Promise((resolve, reject) => {
            try {
                this.subscriptionDropDowns = this.usuarioService.getRoles().subscribe(
                    (result: any) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                            reject("Usuario desconectado");
                        } else if (result.error != undefined && result.error != "") {
                            this.mensajeComponent.setErrorMsg(result.error);
                            reject("Error en la obtención de roles");
                        } else if (result.info != undefined) {
                            this.mensajeComponent.setInfoMsg(result.info);
                            resolve("Información recibida");
                        } else {
                            this.roles = result.data.roles.filter((rol) => rol.Code === "Externo" || rol.Code === "Interno");
                            //this.roles = result.data.roles;
                            resolve("Roles obtenidos");
                        }
                    },
                    error => {
                        this.mensajeComponent.setErrorMsg(error.message);
                        reject(error.message);
                    }
                );
            } catch (e) {
                this.mensajeComponent.setErrorMsg(e);
                reject(e);
            }
        });
    }

    getCheckboxAllRoles(): HTMLInputElement {
        return document.getElementById('checkAllRoles') as HTMLInputElement;
    }

    getCheckboxExternalRoles(): HTMLInputElement {
        return document.getElementById('checkExternalRoles') as HTMLInputElement;
    }

    getCheckboxInternalRoles(): HTMLInputElement {
        return document.getElementById('checkInternalRoles') as HTMLInputElement;
    }

    checkAllRoles() {
        setTimeout(() => {
            this.roles.forEach(element => {
                element.checked = this.allRoles;
            });

            this.getCheckboxExternalRoles().checked = this.allRoles;
            this.getCheckboxInternalRoles().checked = this.allRoles;
        }, 0);
    }

    checkExternalRoles(checked: boolean): void {
        setTimeout(() => {
            this.roles.filter(rol => rol.Code == 'Externo').forEach(rol => {
                rol.checked = checked;
            });

            this.getCheckboxAllRoles().checked = checked && this.getCheckboxInternalRoles().checked;

        }, 100);
    }

    checkInternalRoles(checked: boolean): void {
        setTimeout(() => {
            this.roles.filter(rol => rol.Code == 'Interno').forEach(rol => {
                rol.checked = checked;
            });

            this.getCheckboxAllRoles().checked = checked && this.getCheckboxExternalRoles().checked;
        }, 100);
    }

    submit() {

        this.fecha_inicio = (<HTMLInputElement>document.querySelectorAll('[fechaInicioInput]')[0]).value;
        this.fecha_fin = (<HTMLInputElement>document.querySelectorAll('[fechaFinInput]')[0]).value;


        this.horaInicio = this.horaInicioSelect.nativeElement.value;

        if (!this.validar()) {
            this.spinnerComponent.hideIt();
            return;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.blockUI.start('Guardando...');
        this.unsubscribe();

        this.notificacion.FiltroRoles = this.roles.filter(x => x.checked);

        var dateParts = this.fecha_inicio.split("/");

        const startDateString = (<HTMLInputElement>document.querySelectorAll('[fechaInicioInput]')[0]).value;
        const endDateString = (<HTMLInputElement>document.querySelectorAll('[fechaFinInput]')[0]).value;

        const startDateParts = startDateString.split('/');
        const endDateParts = endDateString.split('/');

        const startDateFormatted = startDateParts[1] + '/' + startDateParts[0] + '/' + startDateParts[2];
        const endDateFormatted = endDateParts[1] + '/' + endDateParts[0] + '/' + endDateParts[2];

        const startDate = new Date(startDateFormatted);
        const endDate = new Date(endDateFormatted);

        startDate.setUTCHours(this.horaInicio, 0, 0, 0);
        endDate.setUTCHours(this.horaInicio, 0, 0, 0);

        this.notificacion.FechaInicio = new Date(startDate.toISOString());
        this.notificacion.FechaFin = new Date(endDate.toISOString());

        this.notificacion.FechaCreacion = new Date();

        this.notificacion.HoraInicio = this.horaInicio;

        this.notificacion.ArchivosAdjuntos = [];
        this.adjuntos = this.notificacion.ArchivosAdjuntos;

        this.adjuntos = [
            ...this.imagenPrevisualizacion.map(image => {
                return {
                    AdjuntoTipo: 'previsualizacion',
                    AdjuntoNombre: image.name,
                    AdjuntoContenido: image.fileAttached
                } as Adjuntos;
            }),
            ...this.imageList.map(image => {
                return {
                    AdjuntoTipo: 'imagen',
                    AdjuntoNombre: image.name,
                    AdjuntoContenido: image.fileAttached
                } as Adjuntos;
            }),
            ...this.videoList.map(video => {
                return {
                    AdjuntoTipo: 'video',
                    AdjuntoNombre: video.name,
                    AdjuntoContenido: video.fileAttached
                } as Adjuntos;
            }),
            ...this.pdfList.map(pdf => {
                return {
                    AdjuntoTipo: 'pdf',
                    AdjuntoNombre: pdf.name,
                    AdjuntoContenido: pdf.fileAttached
                } as Adjuntos;
            })
        ];

        this.subscription = this.service
            .grabar(this.notificacion, this.adjuntos)
            .subscribe(
                (result) => {
                    this.spinnerComponent.hideIt();
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
                        this.mensajeComponent.setMsgsEmpty();
                        document
                            .getElementById("openModalNotificacion")
                            .click();
                    }
                    this.blockUI.stop();
                },
                (error) => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            );
    }


    redirigirAListado() {

        document
            .getElementById("botonCerrarModal")
            .click();
        this.navService.navegarSeccion(
            "/novedades"
        );
    }

    cargarImagenPrevisualizacion(event: any): void {
        const files: FileList = event.target.files;

        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            if (file.type.startsWith('image/')) {
                const imageUrl = URL.createObjectURL(file);

                const modifiedFile = new File([file], file.name, { type: 'previsualizacion' });

                if (this.imagenPrevisualizacion.length >= 1) {
                    this.imagenPrevisualizacion[0] = { name: file.name, fileAttached: modifiedFile };
                } else {
                    this.imagenPrevisualizacion.push({ name: file.name, fileAttached: modifiedFile });
                }
            }
        }
    }

    cargarListaImagenes(event: any): void {
        const files: FileList = event.target.files;

        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            if (file.type.startsWith('image/')) {
                this.imageList.push({ name: file.name, fileAttached: file });
            }
        }
    }

    cargarVideo(event: any): void {
        const files: FileList = event.target.files;

        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            if (file.type.startsWith('video/')) {
                this.videoList.push({ name: file.name, fileAttached: file });
            }
        }
    }

    cargarPDF(event: any): void {
        const files: FileList = event.target.files;

        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            if (file.type === 'application/pdf') {
                this.pdfList.push({ name: file.name, fileAttached: file });
            }
        }
    }

    removerPrevisualizacion(index: number): void {
        this.imagenPrevisualizacion.splice(index, 1);
        (<HTMLInputElement>document.getElementById("previewImageFile")).value = "";
    }

    removerImagenLista(index: number): void {
        this.imageList.splice(index, 1);
        (<HTMLInputElement>document.getElementById("imageFile")).value = "";
    }

    removerVideoLista(index: number): void {
        this.videoList.splice(index, 1);
        (<HTMLInputElement>document.getElementById("videoFile")).value = "";
    }

    removerPDFLista(index: number): void {
        this.pdfList.splice(index, 1);
        (<HTMLInputElement>document.getElementById("pdfFile")).value = "";
    }
}
