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
  contratoMarco: ContratoMarco = null;

  @Output()
  obtenerContratoMarcoEmitter = new EventEmitter<ObtenerContratoMarco>();

  @Output()
  agregarPosicionesContratoMarcoEmitter = new EventEmitter<Array<ContratoMarcoPosicion>>();

  public visible: boolean;
  public centrosEntrega: Array<any>;
  public formGroup: FormGroup

  constructor(private obtenerContratoMarcoService: ObtenerContratoMarcoService,
    private formBuilder: FormBuilder) {
    this.obtenerContratoMarcoService.toogleOn.subscribe(value => this.visible = value);
  }

  ngOnInit() {
    this.formGroup = this.formBuilder.group({
      centroEntrega: new FormControl('', Validators.required),
      numeroContrato: new FormControl('', Validators.required)
    });
  }

  showInputError(fieldName: string): boolean {
    if (this.formGroup && this.formGroup.controls) {
        return (this.formGroup.controls[fieldName].invalid || (this.formGroup.controls[fieldName].errors && this.formGroup.controls[fieldName].errors.required))
            && (this.formGroup.controls[fieldName].dirty || this.formGroup.controls[fieldName].touched)
    }
    return false;
  }

  onSearchContrato(){
    if (this.centroEntregaValue && this.numeroContratoValue) {
      const payload = {
        centro: this.centroEntregaValue, 
        numeroContrato: this.numeroContratoValue
      } as ObtenerContratoMarco;

      this.obtenerContratoMarcoEmitter.next(payload);
    }
  }

  onClose() {
    this.obtenerContratoMarcoService.close();
  }
  
  onAgregarPosiciones() {
    let posicionesSeleccionas = Array<ContratoMarcoPosicion>();
    this.agregarPosicionesContratoMarcoEmitter.next(posicionesSeleccionas);
  }

  get centroEntregaValue() {
    let centroSeleccionado = this.formGroup.get('centroEntrega').value;
    let centro = centroSeleccionado != undefined && centroSeleccionado != null ? centroSeleccionado.Codigo : "";
    return centro;
  }
  get numeroContratoValue() {
      return this.formGroup.get('numeroContrato').value;
  }

}
