import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { ConsultaService } from '../consulta.service';
import { Seccion } from '../../common/models/seccion';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { Categoria, Comentario, Destinatario, Materiales, ReclamoImpositivo, Subcategoria } from '../consulta';
import { SendDataService } from '../send-data.service';
import { DropdownComponent } from '../../common/view-child/dropdown/dropdown.component';
import { OrdenesDeCargaService } from '../../ordenes-de-carga/ordenes-de-carga.service';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ConfirmationService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { AngularEditorComponent, AngularEditorConfig } from '@kolkov/angular-editor';
import { GET_ANGULAR_EDITOR_CONFIG, eliminarBotonesExtraEditor } from '../../common/configs/angularEditor.configs';
import { ReCaptchaComponent } from 'angular2-recaptcha';
import { ProveedorRaw } from '../../common/models/proveedorraw';
import { UsuarioService } from '../../usuario/usuario.service';
import { TipoPerfil } from '../../common/enums/TipoPerfil';

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

  @ViewChild('dtp_fecha_pago')
  protected fechaPagoDTP: ElementRef;

  @ViewChild('recaptchaComponent')
  protected captcha: ReCaptchaComponent;

  constructor(
    protected service: ConsultaService,
    protected sendDataService: SendDataService,
    protected navService: NavService,
    protected sessionDataService: SessionDataService,
    protected securityService: SecurityService,
    protected floatMsgService: FloatMsgService,
    protected modalService: ModalService,
    protected ordenDeCargaService: OrdenesDeCargaService,
    private confirmationService: ConfirmationService,
    protected usuarioService: UsuarioService
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
  vendedores: ProveedorRaw[];
  vendedor: ProveedorRaw;
  destinatariosOrden: Destinatario[];
  comentario: string = "";
  ordenes: any[] = [];
  ordenSeleccionada: any = {};
  ordenId: number = 0;

  filtro: string = "";

  /*Campos*/
  categoria: Categoria = null;
  categoriaCode: string;
  subcategoria: Subcategoria = null;
  subcategoriaCode: string;
  destinatariosSeleccionados: Destinatario[];
  asunto: string;
  nuevoComentario: any;
  listaArchivos: Array<File> = new Array<File>();
  files: FileList = null;
  fecha: Date;
  detalle: any;
  consulta: any;

  contrato: string;
  comprobante: any;
  comprobanteExtra: any;
  bolsaEmisoraOblea: string;
  listaMateriales: Materiales[];
  material: Materiales;
  material_Id: any;
  captchaOk: any = null;

  reclamoImpositivo: ReclamoImpositivo = new ReclamoImpositivo();

  impuesto: any;
  importe: any;
  fechaPago: string;
  fechaPagoDP: any;

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
    this.getVendedores();
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
            this.listaMateriales = result.materiales;
            this.ordenes = result.ordenes;
            this.setearDefaultCombos(result.categorias, result.subcategorias);

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

  getDestinatario() {
    this.searchingCampos = true;
    try {
      this.subscriptionDestinatarios = this.service.getDestinatarios(this.vendedor.Id).subscribe(
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
            this.destinatariosSeleccionados = [];
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

  getVendedores() {
    this.mensajeComponent.setMsgsEmpty();
    this.spinnerComponent.showIt();
    try {
      this.subscriptionDestinatarios = this.service.getVendedoresUsuario().subscribe(
        (result: any) => {
          this.spinnerComponent.hideIt();
          if (result.logout == true) {
            this.sessionDataService.logout();
          } else if (result.error != undefined && result.error != "") {
            this.mensajeComponent.setErrorMsg(result.error);
          } else if (result.info != undefined) {
            this.mensajeComponent.setInfoMsg(result.info);
          } else {
            this.vendedores = result.vendedores;
          }
        },
        error => {
          this.spinnerComponent.hideIt();
          this.mensajeComponent.setErrorMsg(error.message);
        }

      );
    } catch (e) {
      this.spinnerComponent.hideIt();
      this.mensajeComponent.setErrorMsg(e);
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
    this.subcategoria = undefined;
    this.subcategoriaCode = undefined;
    this.subcategoriasList = [];
    this.subcategorias.forEach(x => {
      if (x.CategoriaId == categoria.Id) {
        this.subcategoriasList.push(x);
      }
    });
    this.Avisos(this.categoriaCode);
    this.limpiarDetalle();
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

  setSubcategoriaCode() {
    this.subcategoriaCode = this.subcategoria.Code;
    this.Avisos(this.categoriaCode);
    this.limpiarDetalle();
  }

  postConsulta() {
    this.blockUI.start('Generando Consulta');
    this.spinnerComponent.showIt();

    if (this.validarConsulta()) {
      this.spinnerComponent.hideIt();
      this.blockUI.stop();
      return;
    }

    if (this.fechaPago != '' && this.fechaPago != null) {
      var dateParts = this.fechaPago.split("-");
      this.fecha = new Date(+dateParts[0], +dateParts[1] - 1, +dateParts[2]);
    }

    this.detalle = {
      Consulta_Id: 0, Fecha: this.fecha, ComprobanteNo: this.comprobante, OtroComprobanteNo: this.comprobanteExtra,
      ContratoNo: this.contrato, Importe: this.importe, Impuesto: this.impuesto, BolsaEmisoraOblea: this.bolsaEmisoraOblea, Material_Id: null,
      Orden_Id: this.ordenId, PatenteChasis: this.getPatenteChasisOC()
    }

    this.consulta = {
      CodigoCorredor: this.getCodigoCorredor(), RazonSocialCorredor: this.getRazonSocialCorredor(),
      CodigoProveedor: this.getCodigoProveedor(), RazonSocialProveedor: this.getRazonSocialProveedor(),
      Categoria_Id: this.categoria.Id, Detalle: this.detalle,
      SubCategoria_Id: this.subcategoria ? this.subcategoria.Id : null, Asunto: this.asunto,
      Usuario_Id: 0
    }

    let comentario: Comentario = { consulta_Id: 0, Detalle: this.comentario, Fecha: new Date(), Recordado: false, FechaRecordado: new Date() };


    try {
      this.subscription = this.service.AgregarConsultaInterna(this.consulta, comentario, this.destinatariosSeleccionados, this.listaArchivos).subscribe(
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
    this.destinatariosSeleccionados = undefined;
    this.subscriptionDestinatarios.unsubscribe();
    this.getDestinatariosOrden();
  }

  setearComentario() {
    if (this.categoriaCode == 'ORD' && this.ordenId > 0 && this.subcategoria != null) {
      if (this.subcategoria.Code == 'CTG') {
        this.asunto = `Orden Nro ${this.ordenId} - CTG Pendiente.`
        this.comentario = `Estimado usuario, Ud tiene un ctg activo para la patente ${this.getPatenteChasisOC()}, regularizar para poder dar ingreso`;
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

    if ((this.subcategoria == undefined || this.subcategoria == null) && this.subcategoriasList.length >= 1) {
      this.mensajeComponent.setErrorMsg("Por favor seleccione una subcategoria.");
      return true;
    }

    if (this.asunto == "" || !this.asunto) {
      this.mensajeComponent.setErrorMsg("El campo Asunto esta vacio.");
      return true;
    }


    if ((!this.destinatariosSeleccionados || !this.destinatariosSeleccionados.length) && this.categoriaCode !== "ORD") {
      this.mensajeComponent.setErrorMsg("Por favor seleccione un destinatario");
      return true;
    }

    if ((this.vendedor == undefined || this.vendedor == null) && this.categoriaCode !== "ORD") {
      this.mensajeComponent.setErrorMsg("El campo Vendedor esta vacio.");
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

    if ((this.categoriaCode == 'PARCOR' || this.categoriaCode == 'FINCOR') && !this.destinatariosSeleccionadosTodosMismoPerfil(TipoPerfil.Corredor)) {
      this.mensajeComponent.setErrorMsg("Para utilizar esta categoría debe seleccionar corredores.");
      return true;
    }
    if ((this.categoriaCode == 'PARDIR' || this.categoriaCode == 'FINDIR') && this.destinatariosSeleccionadosAlgunPerfil(TipoPerfil.Corredor)) {
      this.mensajeComponent.setErrorMsg("Para utilizar esta categoría debe seleccionar usuarios de estos tipos: Granos, No Granos, o Cliente.");
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

  generarVariable() {
    this.reclamoImpositivo.RazonSocialEmpresa = this.vendedor.RazonSocial;
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

  limpiarDetalle() {
    this.fecha = undefined;
    this.comprobante = undefined;
    this.comprobanteExtra = undefined;
    this.contrato = undefined;
    this.importe = undefined;
    this.impuesto = undefined;
    this.bolsaEmisoraOblea = undefined;
    this.ordenId = undefined;
  }

  mostrarAviso() {
    this.mensajeComponent.setMsgsEmpty();
    if (this.destinatariosSeleccionados.length > 1) {
      this.mensajeComponent.setInfoMsg("Se enviará copia del mail al cliente.");
      return true;
    }
  }

  getCodigoCorredor() {
    if (!this.vendedor)
      return null;
    return this.vendedor.CodigoCorredor ? this.vendedor.CodigoCorredor : null;
  }

  getRazonSocialCorredor() {
    if (!this.vendedor)
      return null;
    return this.vendedor.RazonSocialCorredor ? this.vendedor.RazonSocialCorredor : null;
  }

  getCodigoProveedor() {
    if (!this.vendedor)
      return null;
    return this.vendedor.CodigoProveedor ? this.vendedor.CodigoProveedor : null;
  }

  getRazonSocialProveedor() {
    if (!this.vendedor)
      return null;
    return this.vendedor.RazonSocial ? this.vendedor.RazonSocial : null;
  }

  destinatariosSeleccionadosTodosMismoPerfil(perfil: TipoPerfil): boolean {
    return this.destinatariosSeleccionados
      .every(destinatario => destinatario.NombreTipoUsuario == perfil)
  }

  destinatariosSeleccionadosAlgunPerfil(perfil: TipoPerfil): boolean {
    return this.destinatariosSeleccionados
      .every(destinatario => destinatario.NombreTipoUsuario == perfil)
  }
}