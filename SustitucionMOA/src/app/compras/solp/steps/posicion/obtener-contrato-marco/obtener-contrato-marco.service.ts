import { Injectable, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ObtenerContratoMarcoService implements OnDestroy {
  public toogleOn = new Subject<boolean>();
  public finishedBusqueda = new Subject<void>();

  private visible = false;

  ngOnDestroy() {
    if (this.visible) {
      this.toogleOn.unsubscribe();
    }
  }

  show(value: boolean) {
    this.visible = value;
    this.toogleOn.next(this.visible);
  }

  close() {
    this.visible = false;
    this.toogleOn.next(this.visible);
  }
}
