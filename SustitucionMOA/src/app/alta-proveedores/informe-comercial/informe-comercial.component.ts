import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../../common/base-components/base-component';
import { InformeComercial } from '../../common/models/informeComercial';
import { NuevoAcopio } from '../../common/models/nuevoAcopio';
import { NuevoProduccion } from '../../common/models/nuevoProduccion';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SpinnerSmallComponent } from '../../common/view-child/spinner-small/spinner-small.component';
import { AltaEmpresaService } from '../altas/altas.service';
import { EmpresaGranosService } from '../empresa-granos/empresa-granos.service';

@Component({
  selector: 'app-informe-comercial',
  templateUrl: './informe-comercial.component.html',
  styleUrls: ['./informe-comercial.component.css'],
  providers: [EmpresaGranosService],

})
export class InformeComercialComponent extends BaseComponent implements OnInit {

  constructor(protected service: EmpresaGranosService, protected navService: NavService,
    protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
    protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
    super(navService, securytiService, floatMsgService, modalService);
  }

  private newAttributeAlm: NuevoAcopio = new NuevoAcopio();
  private newAttribute: NuevoProduccion = new NuevoProduccion();
  mensajeError: string = "";
  informe = new InformeComercial();

  @Input() proveedorId: number;


  ngOnInit(): void {
    console.log("El proveedorID:", this.proveedorId)
    this.navService.setSeccionList([]);
  }

  @ViewChild("spinnerModal")
  protected spinnerModal: SpinnerSmallComponent;

  addFieldValue() {
    this.informe.NuevosCampos.push(this.newAttribute);
    this.newAttribute = new NuevoProduccion();
  }

  deleteFieldValue(index) {
    this.informe.NuevosCampos.splice(index, 1);
  }

  addFieldValueAlm() {
    this.informe.NuevosAcopios.push(this.newAttributeAlm);
    this.newAttributeAlm = new NuevoAcopio();
  }

  deleteFieldValueAlm(index) {
    this.informe.NuevosAcopios.splice(index, 1);
  }

  validarInforme() {
    if (this.informe.direccion == "" || !this.informe.direccion) {
      this.mensajeError = "No completo la direccion.";
      return true;
    }
    if (!this.informe.codigoPostal || this.informe.codigoPostal == "") {
      this.mensajeError = "No completo el codigo postal.";
      return true;
    }
    if (
      !this.informe.localidadId ||
      this.informe.localidadId == null ||
      this.informe.localidadId == 0
    ) {
      this.mensajeError =
        "No completo la Localidad en Domicilio Actividad.";
      return true;
    }
    if (
      !this.informe.ContactoComercial.Apellido ||
      this.informe.ContactoComercial.Apellido == ""
    ) {
      this.mensajeError = "No completo el Apellido del contacto.";
      return true;
    }
    if (
      !this.informe.ContactoComercial.Nombres ||
      this.informe.ContactoComercial.Nombres == ""
    ) {
      this.mensajeError = "No completo el Nombre del contacto.";
      return true;
    }
    if (
      !this.informe.ContactoComercial.Puesto ||
      this.informe.ContactoComercial.Puesto == ""
    ) {
      this.mensajeError = "No completo el Puesto del contacto.";
      return true;
    }
    if (
      !this.informe.ContactoComercial.Telefono1 ||
      this.informe.ContactoComercial.Telefono1 == ""
    ) {
      this.mensajeError = "No completo el Telefono del contacto.";
      return true;
    }
    if (this.informe.CampaniaId == 0 || !this.informe.direccion) {
      this.mensajeError = "No completo la Campaña Actual.";
      return true;
    }
    var filaError = 0;

    for (const item of this.informe.NuevosCampos) {
      filaError++;
      if (item.MaterialId == null || item.MaterialId == 0) {
        this.mensajeError = `Debe completar el grano en la fila ${filaError} de capacidad productiva.`;
        return true;
      }
      if (item.LocalidadId == null || item.LocalidadId == 0) {
        this.mensajeError = `Debe completar la localidad en la fila ${filaError} de capacidad productiva.`;
        return true;
      }

      if (item.Hectareas.toString().includes(".") || item.Hectareas.toString().includes(",") || item.Hectareas.toString().includes("e")) {
        this.mensajeError = `Las hectareas deben ser un número entero en la fila ${filaError} de capacidad productiva.`;
        return true;
      }

      if (item.Toneladas == null || item.Toneladas == 0) {
        this.mensajeError = `Debe completar las toneladas en la fila ${filaError} de capacidad productiva.`;
        return true;
      }

      if (item.Toneladas.toString().includes(".") || item.Toneladas.toString().includes(",") || item.Toneladas.toString().includes("e")) {
        this.mensajeError = `Las toneladas deben ser un número entero en la fila ${filaError} de capacidad productiva.`;
        return true;
      }

      if (item.ArrendaPropia == null) {
        this.mensajeError = `Debe completar la condicion en la fila ${filaError} de capacidad productiva.`;
        return true;
      }
    }

    filaError = 0;
    this.informe.NuevosAcopios = this.informe.NuevosAcopios.filter(item => (
      (item.LocalidadID == null || item.LocalidadID == 0) && (item.Toneladas == null || item.Toneladas == 0)) == false);

    for (const item of this.informe.NuevosAcopios) {
      filaError++;
      if (item.LocalidadID == null || item.LocalidadID == 0) {
        this.mensajeError = `Debe completar la localidad en la fila ${filaError} de capacidad planta.`;
        return true;
      }

      if (item.Toneladas == null || item.Toneladas == 0) {
        this.mensajeError = `Debe completar las Toneladas en la fila ${filaError} de capacidad planta.`;
        return true;
      }

      if (item.Toneladas.toString().includes(".") || item.Toneladas.toString().includes(",") || item.Toneladas.toString().includes("e")) {
        this.mensajeError = `Las toneladas deben ser un número entero en ${filaError} de capacidad productiva.`;
        return true;
      }

      if (item.ArrendaPropia == null) {
        this.mensajeError = `Debe completar la condicion en la fila ${filaError} de capacidad planta.`;
        return true;
      }
    }
    return false;
  }



  /*
    generarInformeComercial() {
      this.spinnerModal.showIt();
      this.unsubscribe();
      this.informe.NuevosAcopios.forEach((campo) => {
        campo.CampaniaID = this.campaniaActual;
      });
      this.informe.NuevosCampos.forEach((campo) => {
        campo.CampaniaId = this.campaniaActual;
      });
      this.informe.CampaniaId = this.campaniaActual;
      for (const item of this.listaCampanias) {
        if (item.CampaniaIdActual == this.campaniaActual) {
          this.informe.Campania = item.CampaniaActual;
        }
      }
  
      if (this.validarInforme()) {
        this.spinnerModal.hideIt();
        return;
      }
  
      this.mensajeError = "";
      this.subscription = this.service
        .generarInformeComercial(this.informe, this.proveedorId)
        .subscribe(
          (result) => {
            this.spinnerModal.hideIt();
            if (result.error) {
              this.mensajeError = result.error;
            } else {
              var byteArray = new Uint8Array(result.data);
              var blob = new Blob([byteArray], {
                type: "application/pdf",
              });
              if (window.navigator.msSaveOrOpenBlob) {
                // IE11
                window.navigator.msSaveOrOpenBlob(
                  blob,
                  "Informe comercial" + ".pdf"
                );
              } else {
                var url = window.URL.createObjectURL(blob);
                var link = document.createElement("a");
                document.body.appendChild(link);
                link.href = url;
                link.download = "Informe comercial" + ".pdf";
                link.click();
                setTimeout(function () {
                  window.URL.revokeObjectURL(url);
                }, 0);
  
                return false;
              }
            }
          },
          (error) => {
            this.spinnerModal.hideIt();
            this.mensajeError = error.message;
          }
        );
    }
  
  
    */
}
