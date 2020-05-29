import { Component } from '@angular/core';

@Component({
    selector: 'spinner',
    templateUrl: `./app/common/view-child/spinner/spinner.component.html?v=${new Date().getTime()}`
})

export class SpinnerComponent {

    visible = false;

    showIt() {
        this.visible = true;
    }

    hideIt() {
        this.visible = false;
    }

}