import { Component } from '@angular/core';

@Component({
    selector: 'error',
    templateUrl: `error.no-autorizado.component.html`
})
export class NoAutorizadoComponent {

    mensaje: string;

    constructor() {
        this.mensaje = "El usuario no tiene los permisos necesarios";
    }
    
}