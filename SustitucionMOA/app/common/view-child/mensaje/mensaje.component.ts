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