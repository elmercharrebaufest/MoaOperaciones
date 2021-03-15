import { Component, OnInit, ViewChild} from '@angular/core';
import { VentaSustentableService } from './../venta-sustentable.service'
import { BaseComponent } from './../../common/base-components/base-component';
import { NavService } from './../../common/services/NavService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { Seccion } from './../../common/models/seccion';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { CampoProveedor } from './../sustentable'


@Component({
  selector: 'app-alta',
  templateUrl: './alta.component.html',
  providers: [VentaSustentableService]
})
export class AltaComponent  extends BaseComponent implements OnInit {

  @ViewChild(MensajeComponent)
  protected mensajeComponent: MensajeComponent;

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;

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

  localidadId: any;
  provinciaId: any;

  sojaParcial: boolean;
  sojaTotal: boolean;

  hectareasTotales: number;
  hectareasSoja: number;
  latitud: string;
  longitud: string;
  file: any;
  

  ngOnInit() {
    this.navService.setSeccionList([new Seccion('/sustentable/alta', 'alta', 'Dar de Alta'), new Seccion('/sustentable/listado-campos', 'listado-campos', 'Listado Campos')]);
  
  }

  setTabs() {
    this.setMenuSeccionTab("Alta", "Dar de Alta");
  }

  onChangeSearchLocalidad(term: string) {
      if (term.length > 2) {
          this.unsubscribe();
          this.subscription = this.service.searchLocalidad(term).subscribe(
              result => {
                  this.localidades = result;
              },
              error => {
                  this.mensajeComponent.setErrorMsg(error.message);
              }
          );
      }
  }

  selectEventLocalidad(item) {
    this.localidadId = item.LocalidadId;
    this.provinciaId = item.ProvinciaId;
  }

  cargarArchivo(event: any) {
    let fileList: FileList = event.target.files;
    if (fileList.length > 0) {
        this.file = fileList[0];
    }
  }

  campoProveedorAgregar(){
    debugger
    let campoProveedor: CampoProveedor;

    if(this.validar()){
      return;
    }

    console.log(this.sojaParcial, this.sojaTotal);

    campoProveedor = {
      HectareasTotales: this.hectareasTotales, HectareasSoja: this.hectareasSoja, 
      Latitud: this.latitud, Longitud: this.longitud
    }

    this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        try {
            this.unsubscribe();
            this.subscription = this.service.campoProveedorAgregar(campoProveedor, this.file).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
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

  validar(){
    this.mensajeComponent.setMsgsEmpty();
    debugger
    if(this.nombreEstablecimiento == "" || !this.nombreEstablecimiento){
      this.mensajeComponent.setErrorMsg("Falta completar Nombre del establecimiento.");
      return true;
    }
    if(!this.hectareasTotales){
      this.mensajeComponent.setErrorMsg("Falta completar hectareas totales.");
      return true;
    }
    if(this.hectareasTotales <= 0){
      this.mensajeComponent.setErrorMsg("Hectareas totales no puede ser 0 o un numero negativo.");
      return true;
    }
    if(!this.hectareasSoja){
      this.mensajeComponent.setErrorMsg("Falta completar hectareas de soja.");
      return true;
    }
    if(this.hectareasSoja <= 0){
      this.mensajeComponent.setErrorMsg("Hectareas de soja no puede ser 0 o un numero negativo.");
      return true;
    }
    if(this.hectareasSoja > this.hectareasTotales){
      this.mensajeComponent.setErrorMsg("Usted declaro mayor cantidad de hectareas de soja que hectareas totales.");
      return true;
    }
    if(this.latitud == "" || !this.latitud){
      this.mensajeComponent.setErrorMsg("Falta completar Latitud.");
      return true;
    }
    if(this.longitud == "" || !this.longitud){
      this.mensajeComponent.setErrorMsg("Falta completar Longitud.");
      return true;
    }
    if(this.file.length < 1 || !this.file){
      this.mensajeComponent.setErrorMsg("Falta adjuntar el archivo Kmz.");
      return true;
    }
    
    return false
  }
}
