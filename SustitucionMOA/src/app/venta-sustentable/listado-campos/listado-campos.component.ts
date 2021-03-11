import { Component, OnInit } from '@angular/core';
import { VentaSustentableService } from './../venta-sustentable.service'

@Component({
  selector: 'app-listado-campos',
  templateUrl: './listado-campos.component.html',
  providers: [VentaSustentableService]
})
export class ListadoCamposComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
