import { Component } from '@angular/core';

@Component({
    selector: 'mensaje',
    templateUrl: `mensaje.component.html`
})

export class MensajeComponent {

    errorMsg = "";
    infoMsg = "";
    successMsg = "";
    infoBlancoMsg = "";

    setMsgsEmpty() {
        this.errorMsg = "";
        this.infoMsg = "";
        this.successMsg = "";
        this.infoBlancoMsg = "";
    }

    setErrorMsg(msg: string) {
        window.scroll(0, 0);
        this.errorMsg = msg;
    }

    setInfoMsg(msg: string) {
        window.scroll(0, 0);
        this.infoMsg = msg;
    }

    setInfoBlancoMsg(msg: string) {
        window.scroll(0, 0);
        this.infoBlancoMsg = msg;
    }

    setSuccessMsg(msg: string) {
        window.scroll(0, 0);
        this.successMsg = msg;
    }
    
}