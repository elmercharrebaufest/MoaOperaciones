import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-echeq-grilla',
  templateUrl: './echeq-grilla.component.html',
  styleUrls: ['./echeq-grilla.component.css']
})
export class GrillaComponent {

  @Input() listaContratosPendientesPago: any;

}
