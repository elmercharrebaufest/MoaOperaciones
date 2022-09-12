import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ObtenerContratoMarcoService } from './obtener-contrato-marco.service';

@Component({
  selector: 'app-obtener-contrato-marco',
  templateUrl: './obtener-contrato-marco.component.html',
  styleUrls: ['./obtener-contrato-marco.component.css']
})
export class ObtenerContratoMarcoComponent implements OnInit {

  public visible: boolean;
  formGroup: FormGroup

  constructor(private obtenerContratoMarcoService: ObtenerContratoMarcoService,
    private formBuilder: FormBuilder) {
    this.obtenerContratoMarcoService.toogleOn.subscribe(value => this.visible = value);
  }

  ngOnInit() {
    this.formGroup = this.formBuilder.group({    
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
    if (!this.formGroup.valid) {
      return;
    }
  }

  onClose() {
    this.obtenerContratoMarcoService.close();
  }
  
  onAgregarItems() {

  }

}
