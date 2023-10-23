import { Component, Input } from '@angular/core';

@Component({
  selector: 'spinner-loading',
  templateUrl: './spinner-loading.component.html',
})
export class SpinnerLoadingComponent {

  @Input() show: boolean;

}
