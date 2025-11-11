import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { VentaSustentableService } from './../venta-sustentable.service'
import { BaseComponent } from './../../common/base-components/base-component';
import { NavService } from './../../common/services/NavService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { CampoProveedor, CampoSustentable, CampoCosecha, CampoProveedorDetalle } from './../sustentable'
import { VendedorProveedor } from '../../common/models/vendedorProveedor';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-edicion',
  templateUrl: './edicion.component.html',
  styleUrls: ['./edicion.component.css'],
  providers: [VentaSustentableService]
})
export class EdicionComponent extends BaseComponent implements OnInit {

  @ViewChild(MensajeComponent)
  protected mensajeComponent: MensajeComponent;

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;

  @ViewChild("spinnerDatosGenerales")
  protected spinnerDatosGenerales: SpinnerComponent;

  @ViewChild('fileInputEPA') fileInputEPA: ElementRef;

  @BlockUI() blockUI: NgBlockUI;

  constructor(protected service: VentaSustentableService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
    super(navService, securityService, floatMsgService, modalService);
    this.mensajeComponent = new MensajeComponent();
    this.spinnerComponent = new SpinnerComponent();
  }

  nombreEstablecimiento: string;
  pais: string;
  dataLocalidades = [];
  myLocalidades = <any>[];
  localidades: any = [];

  campoProveedor?: CampoProveedorDetalle;

  cosechas: any[];
  cosecha: any;
  localidad: any;

  localidadId: any;
  provinciaId: any;

  sojaParcial: boolean;
  sojaTotal: boolean;

  proveedorId: any;
  hectareasTotales: number;
  hectareasSoja: number;
  latitud: string;
  longitud: string;
  file: any;
  archivoEPANombre: string = "";
  archivoEpaUrl: string = "";
  fileEPA: any;
  proveedorNombre: any;
  campoCosechaId: any;
  NombreCosecha: string = "";
  localidadNombre: string;
  evidenciaEpaPresentada: boolean = false;


  proveedorSelected: any;
  esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";
  codigoProveedor: string = sessionStorage.getItem("proveedor");

  ngOnInit() {
    this.getParams();
    this.getCosechas();
    this.getCampoProveedor()
  }

  cargarArchivo(event: any) {
    let fileList: FileList = event.target.files;
    if (fileList.length > 0) {
      this.file = fileList[0];
    }
  }

  getParams() {
    const queryString = window.location.href;
    this.campoCosechaId = queryString.split('=')[1].split(';')[0];
    this.proveedorId = queryString.split('=')[2]
  }

  getCampoProveedor() {
    this.mensajeComponent.setMsgsEmpty();
    this.spinnerDatosGenerales.showIt();
    try {
      this.unsubscribe();
      this.subscription = this.service.getCampoProveedor(this.proveedorId, this.campoCosechaId).subscribe(
        (result: any) => {
          this.spinnerDatosGenerales.hideIt();
          if (result.logout == true) {
            this.sessionDataService.logout();
          } else if (result.error != undefined && result.error != "") {
            this.mensajeComponent.setErrorMsg(result.error);
          } else if (result.info != undefined) {
            this.mensajeComponent.setInfoMsg(result.info);
          } else {
            this.campoProveedor = result;
            console.log(result);
            this.procesarArchivo(result);

            this.hectareasSoja = result.HectareasSoja;
            this.hectareasTotales = result.HectareasTotales;
            this.NombreCosecha = result.NombreCosecha;
            this.proveedorNombre = result.ProveedorNombre;
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

  procesarArchivo(result: any) {
    if(result.EvidenciaEPA_Id == null || result.ArchivoEPA  == null)
      return;  
    // Detecta el tipo MIME por la extensión del nombre de archivo
      let mimeType = 'application/octet-stream'; // Valor por defecto
      if (result.NombreArchivoEPA.endsWith('.pdf')) {
        mimeType = 'application/pdf';
      } else if (result.NombreArchivoEPA.endsWith('.doc')) {
        mimeType = 'application/msword';
      } else if (result.NombreArchivoEPA.endsWith('.docx')) {
        mimeType = 'application/vnd.openxmlformats-officedocument.wordprocessingml.document';
      } else if (result.NombreArchivoEPA.endsWith('.jpg') || result.NombreArchivoEPA.endsWith('.jpeg')) {
        mimeType = 'image/jpeg';
      }

      const byteArray = new Uint8Array(result.ArchivoEPA);
      const blob = new Blob([byteArray], { type: mimeType });
      this.archivoEpaUrl = URL.createObjectURL(blob);
    
  }

  campoProveedorEditar() {
    let campoProveedor: CampoProveedor;
    let campoSustentable: CampoSustentable;
    let campoCosecha: CampoCosecha;

    if (this.validar()) {
      return;
    }

    campoSustentable = {
      Nombre: this.campoProveedor.NombreCampo,
      Renspa: this.campoProveedor.Renspa,
      Localidad_Id: this.campoProveedor.Localidad_Id
    }

    campoCosecha = {
      Campo: campoSustentable, Cosecha_Id: this.campoProveedor.CosechaId, Campo_Id: this.campoProveedor.CampoSustentableId,
      ToneladasAprobadas: this.campoProveedor.ToneladasAprobadas
    }

    campoProveedor = {
      HectareasTotales: this.campoProveedor.HectareasTotales, HectareasSoja: this.campoProveedor.HectareasSoja,
      Latitud: this.campoProveedor.Latitud, Longitud: this.campoProveedor.Longitud, Proveedor_Id: this.proveedorId,
      CampoCosecha: campoCosecha,
      CampoCosecha_Id: this.campoCosechaId,
      CUIT: this.campoProveedor.CUIT,
      RazonSocial: this.campoProveedor.ProveedorNombre,
      Archivo_Id: 0,
      BSVS2: this.campoProveedor.BSVS2,
      EPA: this.campoProveedor.EPA,
      EUDR: this.campoProveedor.EUDR,
      EvidenciaEPA_Id: 0,
      EvidenciaPresentada: this.campoProveedor.EvidenciaPresentada
    }

    this.mensajeComponent.setMsgsEmpty();
    this.spinnerComponent.showIt();
    try {
      this.unsubscribe();
      this.subscription = this.service.campoProveedorEditar(campoProveedor, this.file, this.fileEPA).subscribe(
        (result: any) => {
          this.spinnerComponent.hideIt();
          if (result.logout == true) {
            this.sessionDataService.logout();
          } else if (result.error != undefined && result.error != "") {
            this.mensajeComponent.setErrorMsg(result.error);
          } else if (result.info != undefined) {
            this.mensajeComponent.setInfoMsg(result.info);
          } else {
            this.blockUI.stop();

            this.mensajeComponent.setSuccessMsg(result.Mensaje);
            setTimeout(() => {
              this.goToSeccion('/sustentable/listado-campos');
            }, 3000);
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
      this.blockUI.stop();
      return false; //<-- Prevent Refresh
    }

    this.blockUI.stop();
    return false; //<-- Prevent Refresh

  }

  getCosechas() {
    this.mensajeComponent.setMsgsEmpty();
    this.spinnerComponent.showIt();
    try {
      this.unsubscribe();
      this.subscription = this.service.getCosechasCampo().subscribe(
        (result: any) => {
          this.spinnerComponent.hideIt();
          if (result.logout == true) {
            this.sessionDataService.logout();
          } else if (result.error != undefined && result.error != "") {
            this.mensajeComponent.setErrorMsg(result.error);
          } else if (result.info != undefined) {
            this.mensajeComponent.setInfoMsg(result.info);
          } else {
            this.cosechas = result;
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

  goToSeccion(path: string) {
    this.navService.navegarSeccion(path);
    return false;
  }

  validar() {
    this.mensajeComponent.setMsgsEmpty();
    if (this.campoProveedor.NombreCampo == "" || !this.campoProveedor.NombreCampo) {
      this.mensajeComponent.setErrorMsg("Falta completar Nombre del establecimiento.");
      return true;
    }
    if (this.campoProveedor.Renspa == "" || !this.campoProveedor.Renspa || this.campoProveedor.Renspa.length < 13) {
      this.mensajeComponent.setErrorMsg("Falta completar RENSPA.");
      return true;
    }
    if (!this.campoProveedor.HectareasTotales) {
      this.mensajeComponent.setErrorMsg("Falta completar hectareas totales.");
      return true;
    }
    if (this.campoProveedor.HectareasTotales <= 0) {
      this.mensajeComponent.setErrorMsg("Hectareas totales no puede ser 0 o un numero negativo.");
      return true;
    }
    if (!this.campoProveedor.HectareasSoja) {
      this.mensajeComponent.setErrorMsg("Falta completar hectareas de soja.");
      return true;
    }
    if (this.campoProveedor.HectareasSoja <= 0) {
      this.mensajeComponent.setErrorMsg("Hectareas de soja no puede ser 0 o un numero negativo.");
      return true;
    }
    if (this.campoProveedor.HectareasSoja > this.campoProveedor.HectareasTotales) {
      this.mensajeComponent.setErrorMsg("Usted declaro mayor cantidad de hectareas de soja que hectareas totales.");
      return true;
    }
    if (this.campoProveedor.Latitud == "" || !this.campoProveedor.Latitud) {
      this.mensajeComponent.setErrorMsg("Falta completar Latitud.");
      return true;
    }
    if (this.campoProveedor.Longitud == "" || !this.campoProveedor.Longitud) {
      this.mensajeComponent.setErrorMsg("Falta completar Longitud.");
      return true;
    }
    if (!this.campoProveedor.CUIT || this.campoProveedor.CUIT.length < 11) {
      this.mensajeComponent.setErrorMsg("El CUIT ingresado no es válido.");
      return true
    }
    if (!this.campoProveedor.Proveedor_Id || !this.campoProveedor.ProveedorNombre) {
      this.mensajeComponent.setErrorMsg("El proveedor no es válido.");
      return true
    }
    return false
  }

  debeCargarCuit = false;
  sePreseleccionoProveedor = false;
  proveedorSeleccionado = null;

  revisarProveedorSeleccionado() {
    if (this.proveedorSeleccionado)
      return;

    this.debeCargarCuit = !this.sePreseleccionoProveedor && !this.campoProveedor.CUIT;
    if (this.debeCargarCuit) {
      this.proveedorSeleccionado = null;
    }
  }
  onselectProveedor(proveedor?: VendedorProveedor) {
    if (proveedor) {
      this.proveedorSeleccionado = proveedor;
      this.getProveedorId(this.proveedorSeleccionado.idVendedor);
    }
  }
  getProveedorId(codigo: string) {
    this.blockUI.start("Seleccionando proveedor")
    this.subscription = this.service.getProveedor(codigo)
      .pipe(finalize(() => this.blockUI.stop()))
      .subscribe(
        (result: any) => {
          this.spinnerComponent.hideIt();
          if (result.logout == true) {
            this.sessionDataService.logout();
          } else if (result.error != undefined && result.error != "") {
            this.mensajeComponent.setErrorMsg(result.error);
          } else if (result.info != undefined) {
            this.mensajeComponent.setInfoMsg(result.info);
          } else {
            const cuit = result.CUIT ? result.CUIT : this.proveedorSeleccionado.cuit;
            const razonSocial = result.RazonSocial ? result.RazonSocial : this.proveedorSeleccionado.descVendedor;
            this.campoProveedor.CUIT = cuit;
            this.campoProveedor.ProveedorNombre = razonSocial;
          }
        },
        (error) => {
          this.spinnerComponent.hideIt();
          this.mensajeComponent.setErrorMsg(error.message);
        }
      );
  }

  onSePreseleccionaProveedor() {
    this.sePreseleccionoProveedor = true;
    this.debeCargarCuit = false
  }
  onQuery(value: string) {
    this.campoProveedor.ProveedorNombre = value;
    this.proveedorSeleccionado = null
  }

  cargarArchivoEpa(event: any) {
    let fileList: FileList = event.target.files;
    if (fileList.length > 0) {
      this.fileEPA = fileList[0];
      this.campoProveedor.NombreArchivoEPA = this.fileEPA.name;
    }
  }

  onEvidenciaEpaPresentadaChange() {
    if (this.evidenciaEpaPresentada) {
        this.fileEPA = null;
        if (this.fileInputEPA) {
            this.fileInputEPA.nativeElement.value = '';
        }
    }
  }
}