import { Component, Input, OnInit, ViewChild, Output, EventEmitter, LOCALE_ID, ElementRef } from '@angular/core';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerSmallComponent } from '../../common/view-child/spinner-small/spinner-small.component';
import { ChatComprasDto, ChatExternoComprasDto, ChatInternoComprasDto, ChatProveedorDto, ChatsDto } from './chat-interno.interface';
import { BaseComponent } from '../../common/base-components/base-component';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { ComprasService } from '../compras.service';
import localeES from '@angular/common/locales/es';
import { registerLocaleData } from '@angular/common';
import { Notificacion } from '../../common/models/notificacion';
import { Rol } from '../../common/models/rol';
import { NotificacionesService } from '../../notificaciones/notificaciones.service';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { ConsultaService } from '../../consulta/consulta.service';
import { element } from '@angular/core/src/render3';
registerLocaleData(localeES, 'es');

@Component({
    selector: 'app-chat-interno',
    templateUrl: './chat-interno.component.html',
    providers: [{ provide: LOCALE_ID, useValue: 'es' }, { provide: ConsultaService, useClass: ConsultaService }],
    styleUrls: ['./chat-interno.component.css'],

})
export class ChatInternoComponent extends BaseComponent implements OnInit {

    @BlockUI() blockUI: NgBlockUI;
    @Input() displayChatInterno: boolean;
    @Input() chat: ChatsDto;
    @Input() dasboardComprador: boolean;
    @Input() dasboardProveedor: boolean;


    @Output() cerrardisplayChatEmitter = new EventEmitter();

    @ViewChild('contenedorMensajes') contenedorMensajes: any;

    public chatMensaje: ChatInternoComprasDto[] = [];
    public chatCompras: ChatComprasDto;
    public chatProveedores: ChatProveedorDto[] = [];


    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    @ViewChild("spinnerModal")
    protected spinnerModal: SpinnerSmallComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;
    
    resultado: any;
    mensajeNuevo: string = "";
    notificacion: Notificacion = new Notificacion();
    roles: Array<Rol> = [];
    visualizarAlert = false;
    error: string = "";
    esChatComprador: boolean = true;
    esChatProveedor: boolean = false;
    
    proveedorSeleccionado: ChatProveedorDto;

    constructor(
        private route: ActivatedRoute,
        protected service: ComprasService,
        protected serviceConsulta: NotificacionesService,

        protected navService: NavService,
        protected securityService: SecurityService,
        protected sessionDataService: SessionDataService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService,
        protected html_sanitizer: DomSanitizer,
    ) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();

    }

    ngOnInit() {
        if(this.proveedorSeleccionado == undefined){
            this.proveedorSeleccionado = { 
                PeticionDeOferta_Id: 0,
                PeticionDeOfertaUsuario_Id: 0,
                Mensajes: []
            };
        }
    }

    grabarMensajeChatInterno() {
        let mensajeChat: ChatInternoComprasDto = {
            Mensaje: this.mensajeNuevo,
            Solp_Id: this.chat.ChatCompras.Solp_Id
        };

        this.blockUI.start('Grabando ');

        this.subscription = this.service
            .grabarMensajeChatInterno(mensajeChat)
            .subscribe(
                (result: any) => {
                    this.blockUI.stop();
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
                        this.resultado = result.info;

                        if (JSON.parse(sessionStorage.getItem("permisos")).indexOf("COMPRADOR") != -1) {
                            mensajeChat.RolUsuario = "SOLP";
                        } else {
                            mensajeChat.RolUsuario = "COMPRADOR";
                        }

                        const fechaActual = new Date();
                        mensajeChat.FechaEnvioDate = fechaActual;
                        mensajeChat.FechaEnvio = this.formatFecha(fechaActual);
                        mensajeChat.FechaDiaEnvio = this.formatFechaDia(fechaActual);
                        mensajeChat.Mail = sessionStorage.getItem("username");
                        mensajeChat.Usuario_Id = parseInt(sessionStorage.getItem("usuarioId"));
                        this.chat.ChatCompras.Mensajes.push(mensajeChat);

                        //this.actualizarChat();
                        this.mensajeNuevo = "";
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.spinnerModal.hideIt();
                }
            );
    }

    formatFecha(fecha: Date): string {
        const day = this.padZero(fecha.getDate());
        const month = this.padZero(fecha.getMonth() + 1);
        const year = fecha.getFullYear();
        const hours = this.padZero(fecha.getHours());
        const minutes = this.padZero(fecha.getMinutes());
        const seconds = this.padZero(fecha.getSeconds());

        return `${day}-${month}-${year} ${hours}:${minutes}:${seconds}`;
    }

    formatFechaDia(fecha: Date): string {
        const daysOfWeek = ['dom.', 'lun.', 'mar.', 'mi�.', 'jue.', 'vie.', 's�b.'];
        const dayOfWeek = daysOfWeek[fecha.getDay()];
        const day = fecha.getDate();
        const month = fecha.toLocaleString('es-ES', { month: 'short' });

        return `${dayOfWeek}, ${day} ${month}.`;
    }

    padZero(value: number): string {
        return value < 10 ? `0${value}` : `${value}`;
    }    

    onCerrarChat() {
        this.error = "";
        this.visualizarAlert = false;
        this.chat.ChatCompras.RazonSocialComprador = "";
        if(this.proveedorSeleccionado != undefined){
            this.proveedorSeleccionado.RazonSocialProveedor = "";
        }
        this.cerrardisplayChatEmitter.next();
        this.esChatComprador = true;
        this.mostrarChatComprador();
    }

    comentarioPropio(mensajes: ChatInternoComprasDto) {
        return (parseInt(sessionStorage.getItem("usuarioId")) == mensajes.Usuario_Id);
    }

    obtenerYExportarChat(solpId: string, peticionDeOfertaUsuarioId?: string): void {
        this.service.obtenerYExportarChat(solpId ,peticionDeOfertaUsuarioId).subscribe(
            (result) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else {
                    var byteArray = new Uint8Array(result.FileContents);
                    var blob = new Blob([byteArray], {
                        type: "text/plain",
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
                console.error('Error al llamar al servicio:', error);
            }
        );
    }

    onMostrarDialog() {
        if(this.esChatComprador && !this.dasboardProveedor){
            this.mostrarChatComprador();
        } else { 
            this.mostrarChatProveedor(this.chat.ChatProveedores[0]);
        }
    }

    validar() {
        this.error = "";
        this.visualizarAlert = false;
        if (this.mensajeNuevo == undefined || this.mensajeNuevo == "") {
            this.error = "No se pueden enviar mensajes vacios.";
            this.visualizarAlert = true;
            return true;
        } else {
            if(this.esChatComprador && !this.dasboardProveedor){
                this.grabarMensajeChatInterno();
            } else {
                this.grabarMensajeChatExterno();
            }
        }
    }

    mostrarChatProveedor(proveedor: ChatProveedorDto) {    
        this.proveedorSeleccionado = proveedor;
        this.esChatProveedor = true;
        this.esChatComprador = false;
    }

    mostrarChatComprador(){
        this.proveedorSeleccionado = null;
        this.esChatComprador = true;
        this.esChatProveedor = false;
    }

    grabarMensajeChatExterno() {
        let mensajeChat: ChatExternoComprasDto = {
            Mensaje: this.mensajeNuevo,
            PeticionDeOferta_Id: this.proveedorSeleccionado.PeticionDeOferta_Id,
            PeticionDeOfertaUsuario_Id: this.proveedorSeleccionado.PeticionDeOfertaUsuario_Id
        };
        this.blockUI.start('Grabando ');

        this.subscription = this.service
            .grabarMensajeChatExterno(mensajeChat)
            .subscribe(
                (result: any) => {
                    this.blockUI.stop();
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
                        this.resultado = result.info;

                        if (JSON.parse(sessionStorage.getItem("permisos")).indexOf("SOLP") != -1) {
                            mensajeChat.RolUsuario = "SOLP";
                            
                        } else {
                            mensajeChat.RolUsuario = "PROVEEDOR";
                        }
                        const fechaActual = new Date();
                        mensajeChat.FechaEnvioDate = fechaActual;
                        mensajeChat.FechaEnvio = this.formatFecha(fechaActual);
                        mensajeChat.FechaDiaEnvio = this.formatFechaDia(fechaActual);
                        mensajeChat.Mail = sessionStorage.getItem("username");
                        mensajeChat.Usuario_Id = parseInt(sessionStorage.getItem("usuarioId"));
                        mensajeChat.PeticionDeOferta_Id = this.proveedorSeleccionado.PeticionDeOferta_Id;
                        mensajeChat.PeticionDeOfertaUsuario_Id = this.proveedorSeleccionado.PeticionDeOfertaUsuario_Id;

                        this.proveedorSeleccionado.Mensajes.push(mensajeChat);
                        this.mensajeNuevo = "";
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.spinnerModal.hideIt();
                }
            );
    }

    formatoCUIT(cuit: string): string {
        // Eliminar cualquier caracter que no sea un dígito
        const cuitNumerico = cuit.replace(/\D/g, '');
    
        // Aplicar el formato xx-xxxxxxxx-x
        return cuitNumerico.replace(/^(\d{2})(\d{8})(\d{1})$/, '$1-$2-$3');
    }

    marcarChatProveedorComoLeido(proveedor: ChatProveedorDto): void {
        this.service.marcarChatProveedorComoLeido(proveedor).subscribe(
            (result) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else {
                    var byteArray = new Uint8Array(result.FileContents);
                    var blob = new Blob([byteArray], {
                        type: "text/plain",
                    });

                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(
                            blob,
                            result.FileDownloadName
                        );
                    } else { }
                }
            },
            (error) => {
                console.error('Error al llamar al servicio:', error);
            }
        );
    }

    tieneMensajesSinLeer(proveedor: ChatProveedorDto): boolean {
        return proveedor.Mensajes.some(mensaje => mensaje.Leido == false);
    }
    
    tieneMensajes(proveedor: ChatProveedorDto): boolean {
        return proveedor.Mensajes.length > 0;
    }
    
}
