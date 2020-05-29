import { Component } from '@angular/core';

@Component({
    selector: 'mensaje',
    templateUrl: `./app/common/view-child/mensaje/mensaje.component.html?v=${new Date().getTime()}`
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
        this.errorMsg = msg;
    }

    setInfoMsg(msg: string) {
        this.infoMsg = msg;
    }

    setSuccessMsg(msg: string) {
        this.successMsg = msg;
    }
    
}