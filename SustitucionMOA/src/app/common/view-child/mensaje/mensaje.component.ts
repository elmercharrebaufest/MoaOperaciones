import { Component } from '@angular/core';

@Component({
    selector: 'mensaje',
    templateUrl: `mensaje.component.html`
})

export class MensajeComponent {

    errorMsg = "";
    infoMsg = "";
    successMsg = "";

    setMsgsEmpty() {
        this.errorMsg = "";
        this.infoMsg = "";
        this.successMsg = "";
    }

    setErrorMsg(msg: string) {
        window.scroll(0, 0);
        this.errorMsg = msg;
    }

    setInfoMsg(msg: string) {
        window.scroll(0, 0);
        this.infoMsg = msg;
    }

    setSuccessMsg(msg: string) {
        window.scroll(0, 0);
        this.successMsg = msg;
    }
    
}