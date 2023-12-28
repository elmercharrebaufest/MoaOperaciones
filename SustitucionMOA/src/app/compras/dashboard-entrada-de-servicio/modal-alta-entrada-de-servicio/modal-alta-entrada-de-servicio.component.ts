import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ModalService } from '../../../common/services/ModalService';

interface Item {
  id: string;
}

@Component({
  selector: 'app-modal-alta-entrada-de-servicio',
  templateUrl: './modal-alta-entrada-de-servicio.component.html',
  styleUrls: ['./modal-alta-entrada-de-servicio.component.css']
})

export class ModalAltaEntradaDeServicioComponent implements OnInit {

  step: number = 1;
  showAllTables: boolean = false;

  @Input() itemSelected: Item[] = [];
  @Input() itemIdSelected: string = '';
  @Output() closeModal = new EventEmitter<void>();

  constructor(private modalService: ModalService) { }

  ngOnInit() {
  }

  openModal() {
    this.step = 1;
  }

  siguientePaso() {
    if (this.step < this.itemSelected.length) {
      this.step++;
    } else {
      this.showAllTables = true;
    }
  }

  esFilaPar(index: number): boolean {
    return index % 2 === 0;
  }

  refresh() {
    //Agregar lógica para refrescar Cantidades
  }

  certificarPosicion() {
    //Agregar lógica completar el Alta
    this.closeModal.emit();
  }                   
}
