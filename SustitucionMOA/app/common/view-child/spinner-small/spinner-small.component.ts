import { Component } from '@angular/core';
import { SpinnerComponent } from './../spinner/spinner.component';

@Component({
    selector: 'spinner-small',
    templateUrl: `./app/common/view-child/spinner-small/spinner-small.component.html?v=${new Date().getTime()}`
})

export class SpinnerSmallComponent extends SpinnerComponent {

}