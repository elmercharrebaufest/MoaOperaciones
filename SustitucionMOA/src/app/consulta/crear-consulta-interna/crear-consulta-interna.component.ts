import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { ConsultaService } from '../consulta.service';
import { Seccion } from '../../common/models/seccion';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { Categoria, Comentario, Destinatario, Subcategoria } from '../consulta';
import { SendDataService } from '../send-data.service';
import { DropdownComponent } from '../../common/view-child/dropdown/dropdown.component';
import { OrdenesDeCargaService } from '../../ordenes-de-carga/ordenes-de-carga.service';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ConfirmationService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { AngularEditorComponent, AngularEditorConfig } from '@kolkov/angular-editor';
import { GET_ANGULAR_EDITOR_CONFIG, eliminarBotonesExtraEditor } from '../../common/configs/angularEditor.configs';

declare var $: any;

@Component({
  selector: 'app-crear-consulta-interna',
  templateUrl: './crear-consulta-interna.component.html',
  providers: [{ provide: ConsultaService, useClass: ConsultaService }],
  styleUrls: ['./crear-consulta-interna.component.css']
})
export class CrearConsultaInternaComponent extends ListBaseComponent implements AfterViewInit {

  @ViewChild('fileInput')
  protected fileInput: ElementRef;

  @BlockUI() blockUI: NgBlockUI;

  @ViewChild('dropdown_categoria')
  protected categoriaDropdownComponent: DropdownComponent;

  @ViewChild("angularEditorComentario") editor: AngularEditorComponent;

  constructor(
    protected service: ConsultaService,
    protected sendDataService: SendDataService,
    protected navService: NavService,
    protected sessionDataService: SessionDataService,
    protected securityService: SecurityService,
    protected floatMsgService: FloatMsgService,
    protected modalService: ModalService,
    protected ordenDeCargaService: OrdenesDeCargaService,
    private confirmationService: ConfirmationService
  ) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    this.dataRecibida = this.sendDataService.getData();
    this.categoriaDropdownComponent = new DropdownComponent();
  }

  dataRecibida: any = {} = null;
  categorias: Categoria[];
  subcategorias: Subcategoria[] = [];
  subcategoriasList: Subcategoria[] = [];
  destinatarios: Destinatario[];
  destinatariosOrden: Destinatario[];
  destinatariosFas: Destinatario[];
  comentario: string = "";
  ordenes: any[] = [];
  ordenSeleccionada: any = {};
  ordenId: number = 0;

  filtro: string = "";

  /*Campos*/
  categoria: Categoria = null;
  categoriaCode: string;
  subcategoria: Subcategoria = null;
  destinatario: Destinatario;
  asunto: string;
  nuevoComentario: any;
  listaArchivos: Array<File> = new Array<File>();
  files: FileList = null;
  fecha: Date;
  detalle: any;
  consulta: any;

  searchingCampos: boolean = false;

  displayModal = false;
  proveedorId: number;
  esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";

  subscriptionDestinatarios: Subscription;

  config: AngularEditorConfig = GET_ANGULAR_EDITOR_CONFIG();

  checkPermisos() { this.securityService.tienePermisoRedirect("CONTACTO MAIL"); }

  setTabs() {
    this.setMenuSeccionTab("consulta", "crear-consulta-interna");
  }

  ngOnInit() {
    this.setTabs();
    this.checkPermisos();
    this.setSeccionList();
    if (this.dataRecibida != undefined) {
      this.ordenSeleccionada = this.dataRecibida.orden;
      this.ordenId = this.ordenSeleccionada.Id;
      this.getDestinatariosOrden();
    }
    this.getCombosConsultaInterna();
    this.getAllDestinatarios();
    $(".adjuntarArchivo").click(function () {
      $(".adjuntarArchivo1").click();
    });
  }
  ngAfterViewInit(): void {
    this.eliminarBotonesExtra()
  }

  public ngOnDestroy(): void {
    this.sendDataService.limpiarData();
    this.subscription.unsubscribe();
    this.subscriptionDestinatarios.unsubscribe();
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

  getCombosConsultaInterna() {
    try {
      this.subscription = this.service.getCombosConsultaInterna(this.ordenId).subscribe(
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
            this.ordenes = result.ordenes;
            this.setearDefaultCombos(result.categorias, result.subcategorias);
            //this.getAllDestinatarios();

            if (!this.esCorredor) {
              this.proveedorId = result.proveedorId
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
    return false; //<-- Prevent Refresh
  }

  getAllDestinatarios() {
    this.searchingCampos = true;
    try {
      this.subscriptionDestinatarios = this.service.getDestinatarios(this.ordenId).subscribe(
        (result: any) => {
          this.searchingCampos = false;
          if (result.logout == true) {
            this.sessionDataService.logout();
          } else if (result.error != undefined && result.error != "") {
            this.floatMsgService.setErrorMsg(result.error);
          } else if (result.info != undefined) {
            this.floatMsgService.setInfoMsg(result.info);
          } else {
            this.destinatarios = result.destinatarios;
          }
        },
        error => {
          this.floatMsgService.setErrorMsg(error.message);
          this.searchingCampos = false;
        }
      );
    } catch (e) {
      this.floatMsgService.setErrorMsg(e);
      return false; //<-- Prevent Refresh
    }
    return false; //<-- Prevent Refresh
  }

  getDestinatariosOrden() {
    this.searchingCampos = true;
    try {
      this.subscriptionDestinatarios = this.service.getDestinatariosFas(this.ordenId).subscribe(
        (result: any) => {
          this.searchingCampos = false;
          if (result.logout == true) {
            this.sessionDataService.logout();
          } else if (result.error != undefined && result.error != "") {
            this.floatMsgService.setErrorMsg(result.error);
          } else if (result.info != undefined) {
            this.floatMsgService.setInfoMsg(result.info);
          } else {
            this.destinatariosOrden = result.destinatarios;
          }
        },
        error => {
          this.floatMsgService.setErrorMsg(error.message);
          this.searchingCampos = false;
        }
      );
    } catch (e) {
      this.floatMsgService.setErrorMsg(e);
      return false; //<-- Prevent Refresh
    }
    return false; //<-- Prevent Refresh
  }

  setSubcategorias(categoria) {
    this.categoria = categoria;
    this.categoriaCode = this.categoria.Code;
    this.subcategoriasList = [];
    this.subcategorias.forEach(x => {
      if (x.CategoriaId == categoria.Id) {
        this.subcategoriasList.push(x);
      }
    });
  }

  cargarArchivo(event: any) {
    let fileList: FileList = event.target.files;
    let file;

    if (fileList.length > 0) {
      this.files = fileList;
      for (let i = 0; i < fileList.length; i++) {
        file = fileList[i];
        //file.name = "file" + i; //si no es aca se senombra en el postConsulta
        this.listaArchivos.push(file);
      }
    }

    let $formInput = $('input[type=file]');
    $formInput.val(null);
  }

  borrarArchivo(i: number) {
    this.listaArchivos.splice(i, 1);
  }

  setearDefaultCombos(categorias: Categoria[], subcategorias: Subcategoria[]) {
    /*  Categoria Ordenes   */
    if (this.dataRecibida != undefined) {
      this.categoria = categorias.find(c => c.Code == 'ORD');
      this.setSubcategorias(this.categoria);
      this.subcategoria = subcategorias.find(s => s.Code == this.dataRecibida.codSubcategoria);
    }
  }

  postConsulta() {
    this.blockUI.start('Generando Consulta');
    this.spinnerComponent.showIt();

    if (this.validarConsulta()) {
      this.spinnerComponent.hideIt();
      this.blockUI.stop();
      return;
    }

    this.detalle = {
      Consulta_Id: 0, Fecha: null, ComprobanteNo: null, OtroComprobanteNo: null,
      ContratoNo: null, Importe: null, Impuesto: null, BolsaEmisoraOblea: null, Material_Id: null,
      Orden_Id: this.ordenId, PatenteChasis: this.getPatenteChasisOC()
    }

    this.consulta = {
      CodigoCorredor: null, RazonSocialCorredor: null,
      CodigoProveedor: null, RazonSocialProveedor: null,
      Categoria_Id: this.categoria.Id, Detalle: this.detalle,
      SubCategoria_Id: this.subcategoria.Id, Asunto: this.asunto, Usuario_Id: 0
    }

    let comentario: Comentario = { consulta_Id: 0, Detalle: this.comentario, Fecha: new Date(), Recordado: false, FechaRecordado: new Date() };

    if(this.categoriaCode !== 'ORD'){
      this.destinatariosFas = undefined; 
    }

    try {
      this.subscription = this.service.AgregarConsultaInterna(this.consulta, comentario, this.listaArchivos, this.destinatariosFas).subscribe(
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

  showDialog() {
    this.displayModal = true;
  }

  onSelectOrden(orden: any) {
    this.displayModal = false;
    this.ordenSeleccionada = orden;
    this.ordenId = this.ordenSeleccionada.Id;
    this.setearComentario();
    this.destinatariosFas = undefined;
    this.subscriptionDestinatarios.unsubscribe();
    this.getDestinatariosOrden();
  }

  setearComentario() {
    if (this.categoriaCode == 'ORD' && this.ordenId > 0 && this.subcategoria != null) {
      if (this.subcategoria.Code == 'CTG') {
        this.asunto = `Orden Nro ${this.ordenId} - CTG Pendiente.`
        this.comentario = `Estimado usuario, la CTG para la patente: ${this.getPatenteChasisOC()} se encuentra pendiente.`;
      } else if (this.subcategoria.Code == 'ERROROC') {
        this.asunto = `Orden Nro ${this.ordenId} - Error de datos.`
        this.comentario = `Estimado usuario, hubo un error en uno de los datos ingresados para la orden nro ${this.ordenId}.`;
      } else {
        this.asunto = "";
        this.comentario = "";
      }
    }
  }

  validarConsulta() {
    this.mensajeComponent.setMsgsEmpty();

    if (this.categoria == undefined || this.categoria == null) {
      this.mensajeComponent.setErrorMsg("Por favor seleccione una categoria.");
      return true;
    }

    if (this.subcategoria == undefined || this.subcategoria == null) {
      this.mensajeComponent.setErrorMsg("Por favor seleccione una subcategoria.");
      return true;
    }

    if (this.asunto == "" || !this.asunto) {
      this.mensajeComponent.setErrorMsg("El campo Asunto esta vacio.");
      return true;
    }


    if (this.destinatario == undefined && this.categoriaCode !== "ORD") {
      this.mensajeComponent.setErrorMsg("El campo Destinatario esta vacio.");
      return true;
    }

    if ((this.destinatariosFas == undefined || this.destinatariosFas.length == 0) && this.categoriaCode === "ORD") {
      this.mensajeComponent.setErrorMsg("Debe elegir al menos un destinatario.");
      return true;
    }

    if (this.comentario == "" || !this.comentario) {
      this.mensajeComponent.setErrorMsg("El campo Comentario esta vacio.");
      return true;
    }

    if (this.categoriaCode == "ORD" && (this.ordenId == 0 || this.ordenSeleccionada == undefined)) {
      this.mensajeComponent.setErrorMsg("Debe seleccionar una orden.");
      return true;
    }
  }

  getPatenteChasisOC(): string {
    if (this.categoriaCode !== 'ORD')
      return null;
    return this.ordenSeleccionada.PatenteChasis ? this.ordenSeleccionada.PatenteChasis
      : this.ordenSeleccionada.ChasisAcoplado ? this.ordenSeleccionada.ChasisAcoplado : null;
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

  mostrarAviso(){
    this.mensajeComponent.setMsgsEmpty();
      if(this.destinatariosFas.length > 1){
        this.mensajeComponent.setInfoMsg("Se va a proceder a crear una consulta para cada uno de los destinatarios seleccionados.");
        return true;
      }
    }

}