import { Component, OnInit, ViewChild } from '@angular/core';
import { VentaSustentableService } from './../venta-sustentable.service'
import { BaseComponent } from './../../common/base-components/base-component';
import { NavService } from './../../common/services/NavService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { Seccion } from './../../common/models/seccion';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { CampoProveedor, CampoSustentable, CampoCosecha } from './../sustentable'
import { AutocompleteLocalidadComponent } from "./../../common/shared-components/autocomplete-localidad/autocomplete-localidad.component";

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

  campoProveedor: any;

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

  proveedorNombre: any;
  campoCosechaId: any;
  NombreCosecha: any;
  localidadNombre: string;


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
    this.spinnerComponent.showIt();
    try {
      this.unsubscribe();
      this.subscription = this.service.getCampoProveedor(this.proveedorId, this.campoCosechaId).subscribe(
        (result:any) => {
          this.spinnerComponent.hideIt();
          if (result.logout == true) {
            this.sessionDataService.logout();
          } else if (result.error != undefined && result.error != "") {
            this.mensajeComponent.setErrorMsg(result.error);
          } else if (result.info != undefined) {
            this.mensajeComponent.setInfoMsg(result.info);
          } else {
            this.campoProveedor = result;
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

  campoProveedorEditar() {
    let campoProveedor: CampoProveedor;
    let campoSustentable: CampoSustentable;
    let campoCosecha: CampoCosecha;

    if (this.validar()) {
      return;
    }

    campoSustentable = {
      Nombre: this.campoProveedor.NombreCampo, Localidad_Id: this.campoProveedor.Localidad_Id
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
      CUIT: "",
      Archivo_Id:0
    }

    this.mensajeComponent.setMsgsEmpty();
    this.spinnerComponent.showIt();
    try {
      this.unsubscribe();
      this.subscription = this.service.campoProveedorEditar(campoProveedor, this.file).subscribe(
        (result:any) => {
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
        (result:any) => {
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
    /*
    if (this.file.length < 1 || !this.file) {
      this.mensajeComponent.setErrorMsg("Falta adjuntar el archivo Kmz.");
      return true;
    }
    */
    return false
  }


}