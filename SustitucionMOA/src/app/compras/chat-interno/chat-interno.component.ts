import { Component, Input, OnInit, ViewChild, Output, EventEmitter, LOCALE_ID } from '@angular/core';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerSmallComponent } from '../../common/view-child/spinner-small/spinner-small.component';
import { ChatComprasDto, ChatInternoComprasDto } from './chat-interno.interface';
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
  @Input() chat: ChatComprasDto;
  @Output() cerrardisplayChatEmitter = new EventEmitter();

  @ViewChild('contenedorMensajes') contenedorMensajes: any;
  
  public chatMensaje: ChatInternoComprasDto;

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
  }

  grabarMensajeChatInterno() {
    let mensajeChat: ChatInternoComprasDto = {
        Mensaje: this.mensajeNuevo,
        PeticionDeOferta_Id: this.chat.PeticionDeOferta_Id

    };
    this.subscription = this.service
        .grabarMensajeChatInterno(mensajeChat)
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
                    this.resultado = result.info;

                    this.actualizarChat();
                    this.mensajeNuevo = "";
                }
            },
            (error) => {
                this.spinnerModal.hideIt();
            }
        );
    }

    actualizarChat() {
        try {
            this.subscription = this.service.obtenerChat(this.chat.PeticionDeOferta_Id.toString())
              .subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        result.Mensajes = result.Mensajes.map((x) => {
                            x.FechaEnvioDate = new Date(
                                this.getDateFromAspNetFormat(x.FechaEnvioDate)                                
                            );   
                            return x;
                        });
                        result.FechaCreacionDate = new Date(
                            this.getDateFromAspNetFormat(result.FechaCreacionDate)                                
                        );   
                        this.chat = result;
                        this.onMostrarDialog()
                    };
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

    onCerrarPeticion() {
        this.cerrardisplayChatEmitter.next();
    }

    comentarioPropio(mensajes: ChatInternoComprasDto) {
        return (this.chat.UsuarioActualId == mensajes.Usuario_Id);
    }

    obtenerYExportarChat(peticionDeOfertaId: string): void {
        this.service.obtenerYExportarChat(peticionDeOfertaId).subscribe(
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
        if (this.contenedorMensajes) {
          this.contenedorMensajes.nativeElement.scrollTop = this.contenedorMensajes.nativeElement.scrollHeight;
        }
      }

    validar() {
        this.visualizarAlert = false;
        console.log("estoy", this.mensajeNuevo);
        if (this.mensajeNuevo == undefined || this.mensajeNuevo == "") {
            this.error = "No se pueden enviar mensajes vacios.";
            this.visualizarAlert = true;
            return true;
        } else {
            this.grabarMensajeChatInterno();
        }
    }
}
