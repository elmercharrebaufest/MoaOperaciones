import { Component, Input } from '@angular/core';

@Component({
    selector: 'recalculando-spinner',
    templateUrl: 'recalculando-spinner.component.html',
    styleUrls: ['recalculando-spinner.component.css']
})

export class RecalculandoSpinnerComponent {
    @Input() description: string = "Cargando, por favor espere...";
    constructor() { }
}