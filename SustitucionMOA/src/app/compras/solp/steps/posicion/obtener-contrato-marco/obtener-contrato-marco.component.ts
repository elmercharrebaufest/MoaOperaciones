import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ContratoMarco, ContratoMarcoPosicion, ObtenerContratoMarco } from './contrato-marco.model';
import { ObtenerContratoMarcoService } from './obtener-contrato-marco.service';

@Component({
  selector: 'app-obtener-contrato-marco',
  templateUrl: './obtener-contrato-marco.component.html',
  styleUrls: ['./obtener-contrato-marco.component.css']
})
export class ObtenerContratoMarcoComponent implements OnInit {

  @Input()
  set centroEntrega(value: Array<any>) {
      this.centrosEntrega = value;
  }

  @Input() 
  set contratoMarco(value: ContratoMarco) {
    this.contratoMarcoModel = value;
    this.fillPosicionesAsOptions();
  };

  @Output()
  obtenerContratoMarcoEmitter = new EventEmitter<ObtenerContratoMarco>();

  @Output()
  agregarPosicionesContratoMarcoEmitter = new EventEmitter<ContratoMarco>();

  public visible: boolean;
  public centrosEntrega: Array<any>;
  public contratoMarcoModel: ContratoMarco  = null;
  public posicionesAsOptions: Array<any> = [];
  public formGroup: FormGroup
  muestroSpinner: boolean = false;
  primerBusqueda: boolean = true;

  constructor(private obtenerContratoMarcoService: ObtenerContratoMarcoService,
    private formBuilder: FormBuilder) {
    this.obtenerContratoMarcoService.toogleOn.subscribe(value => {
      this.visible = value;
        this.onClear();
    });
  }

  ngOnInit() {
    this.formGroup = this.formBuilder.group({
      centroEntrega: new FormControl('', Validators.required),
      numeroContrato: new FormControl('', Validators.required)
    });
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

  onSearchContrato(){
    try {
      if (this.centroEntregaValue && this.numeroContratoValue) {
        this.muestroSpinner = true;
  
        const payload = {
          centro: this.centroEntregaValue, 
          numeroContrato: this.numeroContratoValue
        } as ObtenerContratoMarco;
  
        this.contratoMarcoModel = null;
        this.obtenerContratoMarcoEmitter.next(payload);
  
        this.muestroSpinner = false;
        this.primerBusqueda = false;
  
      } else {
        this.muestroSpinner = false;
      }
  
    } catch (error) {
      this.muestroSpinner = false;
    }
  }

  onClear() {
    this.setCentroEntrega('');
    this.setNumeroContrato('');
    this.contratoMarcoModel = null;
    this.posicionesAsOptions = [];
    this.primerBusqueda = true;
  }

  onClose() {
    this.onClear();
    this.obtenerContratoMarcoService.close();
    this.primerBusqueda = true;
  }
  
  onAgregarPosiciones() {
    this.agregarPosicionesContratoMarcoEmitter.next(this.contratoMarcoModel);
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

  get canAddItems() {
      let posicionesSeleccionadas = new Array<ContratoMarcoPosicion>();
      if (this.contratoMarcoModel != null) {
        posicionesSeleccionadas = this.contratoMarcoModel.posiciones.filter(p => p.selected);
      }
      return posicionesSeleccionadas.length > 0;
  }

}
