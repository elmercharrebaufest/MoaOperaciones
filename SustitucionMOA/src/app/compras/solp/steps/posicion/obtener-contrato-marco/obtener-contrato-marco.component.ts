import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ContratoMarco, ContratoMarcoPosicion, ObtenerContratoMarco } from './contrato-marco.model';
import { ObtenerContratoMarcoService } from './obtener-contrato-marco.service';
import { ComprasService } from '../../../../compras.service';
import { Solp } from '../../../solp';

@Component({
  selector: 'app-obtener-contrato-marco',
  templateUrl: './obtener-contrato-marco.component.html',
  styleUrls: ['./obtener-contrato-marco.component.css']
})
export class ObtenerContratoMarcoComponent implements OnInit {
  defaultCentroEntrega: any;

  private _model: Solp;
  @Input('model')
  set model(value: Solp) {
    this._model = value;
    if (value) {
      this.initializeProvider();
    }
  }
  get model(): Solp {
    return this._model;
  }

  @Input()
  set centroEntrega(value: Array<any>) {
    this.centrosEntrega = value.filter(x => x.FiltroComprador == true);
  }

  @Input()
  set contratoMarco(value: ContratoMarco) {
    this.contratoMarcoModel = value;
    this.fillPosicionesAsOptions();
    this.muestroSpinner = false;
  };

  @Output()
  obtenerContratoMarcoEmitter = new EventEmitter<ObtenerContratoMarco>();

  @Output()
  agregarPosicionesContratoMarcoEmitter = new EventEmitter<ContratoMarco>();

  public visible: boolean;
  public centrosEntrega: Array<any>;
  public contratoMarcoModel: ContratoMarco = null;
  public posicionesAsOptions: Array<any> = [];
  public formGroup: FormGroup
  muestroSpinner: boolean = false;
  primerBusqueda: boolean = true;

  @Input()
  set ListaContratos(value: ContratoMarco[]) {
    if (value && value.length > 0) {
      this.numerosContratoOptions = value.map(contrato => {
        return { label: this.eliminarCerosIniciales(contrato.numeroDocumentoCompras), value: contrato.numeroDocumentoCompras };
      });
      // Eliminar duplicados si es necesario
      this.numerosContratoOptions = this.numerosContratoOptions.filter((thing, index, self) =>
        index === self.findIndex((t) => (
          t.value === thing.value
        ))
      );
    } else {
      this.numerosContratoOptions = [];
    }
    this.muestroSpinner = false;
  }

  numerosContratoOptions: any[] = [];
  proveedoresFiltrados: any[];

  constructor(private obtenerContratoMarcoService: ObtenerContratoMarcoService,
    private formBuilder: FormBuilder,
    private comprasService: ComprasService) {
    this.obtenerContratoMarcoService.toogleOn.subscribe(value => {
      this.visible = value;
      if (value) {
        this.onClear();
        this.initializeProvider();
      }
    });

    this.obtenerContratoMarcoService.finishedBusqueda.subscribe(() => {
      this.muestroSpinner = false;
    });
  }

  ngOnInit() {
    this.formGroup = this.formBuilder.group({
      centroEntrega: new FormControl('', Validators.required),
      numeroContrato: new FormControl('', Validators.required),
      codProveedor: new FormControl('')
    });

    this.defaultCentroEntrega = this.centrosEntrega.find(centro => centro.Codigo === "1029");
    this.initializeProvider();
  }

  private initializeProvider() {
    console.log(this.model);
    if (this.model && this.formGroup) {
      const codigoSap = this.model.codigoProveedorSap ? String(this.model.codigoProveedorSap).trim() : '';
      const tieneProveedorAsignado = this.model.proveedorAsignado_Id || (this.model.proveedorAsignado && this.model.proveedorAsignado.length > 0);

      if (codigoSap.length > 0 || tieneProveedorAsignado) {
        this.formGroup.get('codProveedor').setValue({
          Id: this.model.proveedorAsignado_Id,
          RazonSocial: this.model.proveedorAsignado,
          CodigoProveedorSap: codigoSap
        });

        if (codigoSap.length > 0) {
          this.muestroSpinner = true;
          const payload = {
            centro: this.defaultCentroEntrega ? this.defaultCentroEntrega.Codigo : '',
            numeroContrato: '',
            codigoProveedor: codigoSap
          } as ObtenerContratoMarco;

          this.obtenerContratoMarcoEmitter.next(payload);
          this.primerBusqueda = false;
        }
      }
    }
  }

  showInputError(fieldName: string): boolean {
    if (this.formGroup && this.formGroup.controls) {
      return (this.formGroup.controls[fieldName].invalid || (this.formGroup.controls[fieldName].errors && this.formGroup.controls[fieldName].errors.required));
    }
    return false;
  }

  private fillPosicionesAsOptions() {
    this.posicionesAsOptions = [];
    if (this.contratoMarcoModel) {
      this.posicionesAsOptions = this.contratoMarcoModel.posiciones.map(pos => {
        return {
          numeroPosicion: pos.numeroPosicionDocumentoCompras,
          textoMaterialOServicio: pos.textoMaterialOServicio,
          fullTextoMaterialOServicio: `${pos.numeroPosicionDocumentoCompras} - ${pos.textoMaterialOServicio}`
        }
      });
    }
  }

  onSelectPosicion(event) {
    if (event) {
      this.contratoMarcoModel.posiciones
        .filter(pos => pos.numeroPosicionDocumentoCompras == event.numeroPosicion)
        .forEach(pos => pos.selected = true);
    }
  }

  onClickSelectPosicion(posicion: ContratoMarcoPosicion) {
    if (posicion && !posicion.selected) {
      posicion.subPosiciones.forEach(subpos => {
        subpos.selected = false;
      });
      posicion.allSubPosicionesSelected = false;
    }
  }

  onClickSelectAllSubPosiciones(posicion: ContratoMarcoPosicion) {
    if (posicion) {
      posicion.subPosiciones.forEach(subpos => {
        subpos.selected = posicion.allSubPosicionesSelected;
      });
      if (posicion.allSubPosicionesSelected) {
        posicion.selected = true;
      }
    }
  }

  onClickSelectSubPosicion(posicion: ContratoMarcoPosicion) {
    if (posicion) {
      let hasSubPosicionesSeleccionadas = posicion.subPosiciones.filter(subpos => subpos.selected == true).length > 0;
      if (hasSubPosicionesSeleccionadas) {
        posicion.selected = true;
      }
      else {
        posicion.allSubPosicionesSelected = false;
      }
    }
  }

  onSearchContrato() {
    try {
      if (this.centroEntregaValue && this.numeroContratoValue) {
        this.muestroSpinner = true;

        let codProveedor = '';
        if (this.codProveedorValue && this.codProveedorValue.CodigoProveedorSap) {
          codProveedor = this.codProveedorValue.CodigoProveedorSap;
        } else if (this.model && this.model.codigoProveedorSap && !this.codProveedorValue) {
          // Si el campo está vacío, no mandamos el proveedor del modelo
          codProveedor = '';
        }

        const payload = {
          centro: this.centroEntregaValue,
          numeroContrato: this.numeroContratoValue,
          codigoProveedor: codProveedor
        } as ObtenerContratoMarco;

        this.contratoMarcoModel = null;
        this.obtenerContratoMarcoEmitter.next(payload);

        this.primerBusqueda = false;

      } else {
        this.muestroSpinner = false;
      }

    } catch (error) {
      this.muestroSpinner = false;
    }
  }

  filtrarProveedores(event) {
    this.comprasService.listarProveedores(event.query).subscribe((result: any) => {
      this.proveedoresFiltrados = result.data;
    });
  }

  onSelectProveedor(event) {
    if (event) {
      this.muestroSpinner = true;
      let codProveedor = '';
      if (event && event.CodigoProveedorSap) {
        codProveedor = event.CodigoProveedorSap;
      }

      const payload = {
        centro: this.centroEntregaValue || (this.defaultCentroEntrega ? this.defaultCentroEntrega.Codigo : ''),
        numeroContrato: '',
        codigoProveedor: codProveedor
      } as ObtenerContratoMarco;

      this.contratoMarcoModel = null;
      this.obtenerContratoMarcoEmitter.next(payload);

      this.primerBusqueda = false;
    }
  }

  onClear() {
    this.setCentroEntrega('');
    this.setNumeroContrato('');

    if (this.model && this.model.codigoProveedorSap) {
      this.initializeProvider();
    } else {
      this.formGroup.get('codProveedor').setValue('');
    }

    this.contratoMarcoModel = null;
    this.posicionesAsOptions = [];
    this.primerBusqueda = true;
  }

  onClose() {
    this.onClear();
    this.obtenerContratoMarcoService.close();
    this.primerBusqueda = true;
    this.defaultCentroEntrega = this.centrosEntrega.find(centro => centro.Codigo === "1029");
  }

  onAgregarPosiciones() {
    this.agregarPosicionesContratoMarcoEmitter.next(this.contratoMarcoModel);
    this.onClose();
  }

  private setCentroEntrega(value: any) { this.formGroup.get('centroEntrega').setValue(value); }
  private setNumeroContrato(value: any) { this.formGroup.get('numeroContrato').setValue(value); }

  get centroEntregaValue() {
    let centroSeleccionado = this.formGroup.get('centroEntrega').value;
    let centro = centroSeleccionado != undefined && centroSeleccionado != null ? centroSeleccionado.Codigo : "";
    return centro;
  }
  get numeroContratoValue() {
    return this.formGroup.get('numeroContrato').value;
  }

  get codProveedorValue() {
    return this.formGroup.get('codProveedor').value;
  }

  get canAddItems() {
    let posicionesSeleccionadas = new Array<ContratoMarcoPosicion>();
    if (this.contratoMarcoModel != null) {
      posicionesSeleccionadas = this.contratoMarcoModel.posiciones.filter(p => p.selected);
    }
    return posicionesSeleccionadas.length > 0;
  }

  eliminarCerosIniciales(numero: string | number): string {
    if (typeof numero === 'number') {
      numero = numero.toString();
    }
    return numero.replace(/^0+/, '');
  }

}
