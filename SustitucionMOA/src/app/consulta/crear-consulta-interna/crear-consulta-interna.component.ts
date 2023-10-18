import { Component, ElementRef, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { ConsultaService } from '../consulta.service';
import { Seccion } from '../../common/models/seccion';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { ActivatedRoute } from '@angular/router';
import { Categoria, Destinatario, Subcategoria } from '../consulta';
import { SendDataService } from '../send-data.service';
import { DropdownComponent } from '../../common/view-child/dropdown/dropdown.component';
import { OrdenesDeCargaService } from '../../ordenes-de-carga/ordenes-de-carga.service';
import { DatePipe } from '@angular/common';

declare var $: any;

@Component({
  selector: 'app-crear-consulta-interna',
  templateUrl: './crear-consulta-interna.component.html',
  providers: [{ provide: ConsultaService, useClass: ConsultaService }],
  styleUrls: ['./crear-consulta-interna.component.css']
})
export class CrearConsultaInternaComponent extends ListBaseComponent {

  @ViewChild('fileInput')
  protected fileInput: ElementRef;

  @ViewChild('dropdown_categoria')
  protected categoriaDropdownComponent: DropdownComponent;

  constructor(
    protected service: ConsultaService,
    protected sendDataService: SendDataService,
    protected navService: NavService,
    protected sessionDataService: SessionDataService,
    protected securityService: SecurityService,
    protected floatMsgService: FloatMsgService,
    protected modalService: ModalService,
    protected ordenDeCargaService: OrdenesDeCargaService,
    private route: ActivatedRoute,
    private datePipe: DatePipe,
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
  comentario: string = "";
  ordenes: any[] = [];
  ordenSeleccionada: any = {};
  ordenId: number = 0;

  filtro: string = "";

  categoria: Categoria = null;
  categoriaCode: string;
  subcategoria: Subcategoria = null;
  destinatario: Destinatario;
  asunto: string;
  nuevoComentario: any;
  listaArchivos: Array<File> = new Array<File>();
  files: FileList = null;

  searchingCampos: boolean = false;

  displayModal = false;

  proveedorId: number;

  esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";

  setTabs() {
    this.setMenuSeccionTab("consulta", "crear-consulta-interna");
  }

  ngOnInit() {
    this.setTabs();
    this.navService.setSeccionList([new Seccion('/consulta/crear-consulta', 'crear-consulta', 'Nueva Consulta'), new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas'),
    new Seccion('/consulta/crear-consulta-interna', 'crear-consulta-interna', 'Nueva Consulta Interna')]);
    
    if (this.dataRecibida != undefined) {
      this.ordenSeleccionada = this.dataRecibida.orden;
      this.ordenId = this.ordenSeleccionada.Id;
    }
    this.getCombosConsultaInterna();
    //getDestinatarios y gtOrdenes
    console.log(this.ordenes);
    $(".adjuntarArchivo").click(function () {
      $(".adjuntarArchivo1").click();
    });
  }

  public ngOnDestroy(): void {
    this.sendDataService.limpiarData();
  }


  getCombosConsultaInterna() {
    this.unsubscribe();
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
            this.getDestinatarios();

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

  getDestinatarios() {
    this.unsubscribe();
    this.searchingCampos = true;
    try {
      this.subscription = this.service.getDestinatarios(this.ordenId).subscribe(
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

  }

  showDialog() {
    this.displayModal = true;
  }

  onSelectOrden(orden: any) {
    this.displayModal = false;
    this.ordenSeleccionada = orden;
    this.ordenId = this.ordenSeleccionada.Id;
    this.setearComentario();
    this.destinatario = undefined;
    this.getDestinatarios();
  }

  setearComentario() {
    if (this.categoriaCode == 'ORD' && this.ordenId > 0) {
      if (this.subcategoria.Code == 'CTG') {
        this.asunto = `Orden Nro ${this.ordenId} - CTG Pendiente.`
        this.comentario = `Estimado usuario, la CTG para la patente: ${this.ordenSeleccionada.PatenteChasis} se encuentra pendiente.`;
      } else if (this.subcategoria.Code == 'ERROROC') {
        this.asunto = `Orden Nro ${this.ordenId} - Error de datos.`
        this.comentario = `Estimado usuario, hubo un error en uno de los datos ingresados para la orden nro ${this.ordenId}.`;
      }else{
        this.asunto = "";
        this.comentario = "";
      }
    }
  }
}
