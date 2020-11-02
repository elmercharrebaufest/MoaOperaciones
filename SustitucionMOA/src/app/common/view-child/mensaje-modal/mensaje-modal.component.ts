import { Component } from '@angular/core';

@Component({
    selector: 'mensaje-modal',
    templateUrl: `mensaje-modal.component.html`
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