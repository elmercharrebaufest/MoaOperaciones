import { Component } from '@angular/core';

@Component({
    selector: 'mensaje-modal',
    templateUrl: `./app/common/view-child/mensaje-modal/mensaje-modal.component.html?v=${new Date().getTime()}`
})

export class MensajeModalComponent {

    errorMsg = "";
    successMsg = "";

    setMsgsEmpty() {
        this.errorMsg = "";
        this.successMsg = "";
    }

    setErrorMsg(msg: string) {
        this.errorMsg = msg;
    }

    setSuccessMsg(msg: string) {
        this.successMsg = msg;
    }

}