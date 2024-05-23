import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { BaseComponent } from '../../common/base-components/base-component';
import { CartaPresentacion } from '../../common/models/cartaPresentacion';
import { InformeComercial } from '../../common/models/informeComercial';
import { Material } from '../../common/models/material';
import { NuevoAcopio } from '../../common/models/nuevoAcopio';
import { NuevoProduccion } from '../../common/models/nuevoProduccion';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SpinnerSmallComponent } from '../../common/view-child/spinner-small/spinner-small.component';
import { EmpresaGranosService } from '../empresa-granos/empresa-granos.service';

@Component({
  selector: 'app-carta-presentacion',
  templateUrl: './carta-presentacion.component.html',
  styleUrls: ['./carta-presentacion.component.css'],
  providers: [EmpresaGranosService],

})
export class CartaPresentacionComponent extends BaseComponent implements OnInit {

  constructor(protected service: EmpresaGranosService, protected navService: NavService,
    protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
    protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
    super(navService, securytiService, floatMsgService, modalService);
  }

  mensajeError: string = "";
  listaMateriales: Array<Material> = [];
  listaCampanias: any = [];
  data = [];
  dataLocalidades = [];

  cartaPresentacion = new CartaPresentacion();

  campaniaActual: number;

  materialesData: any = null;

  searchTerm: FormControl = new FormControl();
  myLocalidades = <any>[];
  tryDoctype: string = "";
  keyword = "Nombre";
  localidades: any = [];
  autocompleteNotFoundText = "No encontrado";
  proveedorClasificacion: string = "";

  private nuevoAtributoCampo: NuevoProduccion = new NuevoProduccion();
  private nuevoAtributoAcompio: NuevoAcopio = new NuevoAcopio();

  @ViewChild("spinnerCartaPresentacion")
  protected spinnerCartaPresentacion: SpinnerSmallComponent;

  private selectUndefinedOptionValue: any;

  @ViewChild("spinnerModal")
  protected spinnerModal: SpinnerSmallComponent;

  @Input() proveedorId: number;
  @Input() displayType: string;


  ngOnInit(): void {
    this.agregarCampoCartaPresentacion();
    this.agregarAcopioCartaPresentacion();

    //Agarramos los input que son de autocomplete de localidad (que usan un componente aparte) y les ponemos en off el autocomplete de chrome, para que no rellene formularios
    setTimeout(() => {
      var autocompletesLocalidad = document.querySelectorAll('[placeholder="Localidad"]');
      for (let i = 0; i < autocompletesLocalidad.length; i++) {
        autocompletesLocalidad[i].setAttribute("autocomplete", "chrome-off");
      }
    }, 1000);

    this.cartaPresentacion.vendedorMailContacto = sessionStorage.getItem("username");
  }


  iniciarForm() {
    if (this.proveedorId > 0) {
      document.getElementById("openModalHiddenButtonCartaPresentacion").click();
      this.obtenerCampanias();
    }
    else {
      this.floatMsgService.setInfoMsg("No hay un proveedor seleccionado para completar la carta de presentación.")
    }
  }

  obtenerCampanias() {
    this.subscription = this.service.obtenerCampanias().subscribe(
      (result) => {
        let obj = result;
        this.listaCampanias = new Array();
        obj.forEach((element: { Descripcion: any; CampaniaId: any; }) => {
          let cam = {
            CampaniaActual: element.Descripcion,
            CampaniaIdActual: element.CampaniaId,
          };
          this.listaCampanias.push(cam);
        });

        this.obtenerMateriales();

      },
      (error) => {
        this.mensajeError = error.message;
      }
    );
  }

  obtenerMateriales() {

    //Sacamos lo de la lista de campaña, ya que ahora son independientes
    this.subscription = this.service.obtenerMateriales().subscribe(
      (result) => {
        let obj = JSON.parse(result);
        // this.listaCampanias = new Array();
        obj.Datos.forEach((element: { MaterialId: number; Descripcion: string; CampaniaActual: string; CampaniaIdActual: number; }) => {
          let mat = new Material();
          mat.Id = element.MaterialId;
          mat.Descripcion = element.Descripcion;
          mat.CampaniaActual = element.CampaniaActual;
          mat.CampaniaIdActual = element.CampaniaIdActual;
          this.listaMateriales.push(mat);

        });
      },
      (error) => {
        this.mensajeError = error.message;
      }
    );
  }

  agregarCampoCartaPresentacion() {
    this.cartaPresentacion.nuevosCampos.push(this.nuevoAtributoCampo);
    this.nuevoAtributoCampo = new NuevoProduccion();
  }

  borrarCampoCartaPresentacion(index) {
    this.cartaPresentacion.nuevosCampos.splice(index, 1);
  }

  agregarAcopioCartaPresentacion() {
    this.cartaPresentacion.nuevosAcopios.push(this.nuevoAtributoAcompio);
    this.nuevoAtributoAcompio = new NuevoAcopio();
  }
  borrarAcopioCartaPresentacion(index) {
    this.cartaPresentacion.nuevosAcopios.splice(index, 1);
  }

  onChangeLocalidad(term: string) {
    if (term.length > 2) {
      this.unsubscribe();
      this.subscription = this.service.searchLocalidad(term).subscribe(
        (result) => {
          this.localidades = result;
        },
        (error) => {
          this.mensajeError = error.message;
        }
      );
    }
  }

  selectEventCampoCartaPresentacion(item: { LocalidadId: number; }, index: string | number) {
    this.cartaPresentacion.nuevosCampos[index].LocalidadId =
      item.LocalidadId;
  }

  selectEventAcopioCartaPresentacion(item: { LocalidadId: number; }, index: string | number) {
    this.cartaPresentacion.nuevosAcopios[index].LocalidadID =
      item.LocalidadId;
  }

  obtenerInfoProveedor() {
    this.subscription = this.service
      .obtenerInfoProveedor(this.proveedorId)
      .subscribe(
        (result) => {
          this.proveedorClasificacion = result.ProveedorClasificacion;
        },
        (error) => {
          this.mensajeError = error.message;
        }
      );
  }


  generarCartaPresentacion() {
    this.spinnerCartaPresentacion.showIt();
    this.unsubscribe();
    this.cartaPresentacion.nuevosAcopios.forEach((campo) => {
      campo.CampaniaID = this.campaniaActual;
    });
    this.cartaPresentacion.nuevosCampos.forEach((campo) => {
      campo.CampaniaId = this.campaniaActual;
    });

    this.cartaPresentacion.campaniaID = this.campaniaActual;
    this.cartaPresentacion.vendedorCosecha = this.listaCampanias.find(x => x.CampaniaIdActual == this.campaniaActual).CampaniaActual;
    this.cartaPresentacion.vendedorActividad = this.proveedorClasificacion;


    if (this.validarCartaPresentacion()) {
      this.spinnerCartaPresentacion.hideIt();
      return;
    }

    this.mensajeError = "";
    this.subscription = this.service
      .generarCartaPresentacion(this.cartaPresentacion, this.proveedorId)
      .subscribe(
        (result) => {
          this.spinnerCartaPresentacion.hideIt();
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
                "Carta presentación.pdf"
              );
            } else {
              var url = window.URL.createObjectURL(blob);
              var link = document.createElement("a");
              document.body.appendChild(link);
              link.href = url;
              link.download = "Carta presentación.pdf";
              link.click();
              setTimeout(function () {
                window.URL.revokeObjectURL(url);
              }, 0);

              return false;
            }
          }
        },
        (error) => {
          this.spinnerCartaPresentacion.hideIt();
          this.mensajeError = error.message;
        }
      );
  }

  validarCartaPresentacion() {

    //Corredor
    if (
      this.cartaPresentacion.corredorBolsa == "" ||
      !this.cartaPresentacion.corredorBolsa
    ) {
      this.mensajeError = "No completo el campo bolsa.";
      return true;
    }

    if (
      this.cartaPresentacion.corredorNroRegistro == "" ||
      !this.cartaPresentacion.corredorNroRegistro
    ) {
      this.mensajeError = "No completo el número de registro.";
      return true;
    }

    // //Vendedor
    // if (
    //   this.cartaPresentacion.vendedorActividad == "" ||
    //   !this.cartaPresentacion.vendedorActividad
    // ) {
    //   this.mensajeError = "No seleccionó la actividad.";
    //   return true;
    // }

    if (
      this.cartaPresentacion.vendedorDomicilioFiscal == "" ||
      !this.cartaPresentacion.vendedorDomicilioFiscal
    ) {
      this.mensajeError = "No completo el domicilio fiscal.";
      return true;
    }

    if (
      this.cartaPresentacion.vendedorMailContacto == "" ||
      !this.cartaPresentacion.vendedorMailContacto
    ) {
      this.mensajeError = "No completo el mail de contacto.";
      return true;
    }

    if (
      this.cartaPresentacion.vendedorTelefonoContacto == "" ||
      !this.cartaPresentacion.vendedorTelefonoContacto
    ) {
      this.mensajeError = "No completo el teléfono de contacto.";
      return true;
    }

    if (
      this.cartaPresentacion.campaniaID == 0 ||
      !this.cartaPresentacion.campaniaID
    ) {
      this.mensajeError = "No completo la Campaña Actual.";
      return true;
    }

    var filaError = 0;

    for (const item of this.cartaPresentacion.nuevosCampos) {
      filaError++;
      if (item.MaterialId == null || item.MaterialId == 0) {
        this.mensajeError = `Debe completar el grano en la fila ${filaError} de capacidad productiva.`;
        return true;
      }
      if (item.LocalidadId == null || item.LocalidadId == 0) {
        this.mensajeError = `Debe completar la localidad en la fila ${filaError} de capacidad productiva.`;
        return true;
      }
      if(item.Hectareas != undefined)
      {
        if (item.Hectareas.toString().includes(".") || item.Hectareas.toString().includes(",") || item.Hectareas.toString().includes("e")) {
          this.mensajeError = `Las hectareas deben ser un número entero en la fila ${filaError} de capacidad productiva.`;
          return true;
        }
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
    this.cartaPresentacion.nuevosAcopios = this.cartaPresentacion.nuevosAcopios.filter(item => (
      (item.LocalidadID == null || item.LocalidadID == 0) && (item.Toneladas == null || item.Toneladas == 0)) == false);

    for (const item of this.cartaPresentacion.nuevosAcopios) {
      if (item.LocalidadID == null || item.LocalidadID == 0) {
        this.mensajeError = `Debe completar la localidad en la fila ${filaError} de capacidad planta.`;
        return true;
      }
      if (item.Toneladas == null || item.Toneladas == 0) {
        this.mensajeError = `Debe completar las Toneladas en la fila ${filaError}  de capacidad planta.`;
        return true;
      }

      if (item.Toneladas.toString().includes(".") || item.Toneladas.toString().includes(",") || item.Toneladas.toString().includes("e")) {
        this.mensajeError = `Las toneladas deben ser un número entero en la fila ${filaError} de capacidad planta.`;
        return true;
      }

      if (item.ArrendaPropia == null) {
        this.mensajeError = `Debe completar la condicion en la fila ${filaError} de capacidad planta.`;
        return true;
      }
    }
    return false;
  }
}
