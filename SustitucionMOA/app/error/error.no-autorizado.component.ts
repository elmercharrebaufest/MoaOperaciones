import { Component } from '@angular/core';

@Component({
    selector: 'error',
    templateUrl: `./app/error/error.no-autorizado.component.html?v=${new Date().getTime()}`
})
export class NoAutorizadoComponent {

    mensaje: string;

    constructor() {
        this.mensaje = "El usuario no tiene los permisos necesarios";
    }
    
}