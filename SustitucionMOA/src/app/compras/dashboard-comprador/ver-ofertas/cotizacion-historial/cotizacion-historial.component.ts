import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CotizacionHistorialDto } from '../../../../modelos/cotizacion-historial-model';

@Component({
  selector: 'app-cotizacion-historial',
  templateUrl: './cotizacion-historial.component.html',
  styleUrls: ['./cotizacion-historial.component.css']
})
export class CotizacionHistorialComponent implements OnInit {

  @Input() public historiales: CotizacionHistorialDto[]; 
  @Input() displayHistorial: boolean;   
  @Output() cerrarHistorialEmitter = new EventEmitter();
  
  responsiveOptions;
  visualizarHoras: boolean;

  constructor() { 
    this.responsiveOptions = [
      {
          breakpoint: '1024px',
          numVisible: 3,
          numScroll: 3
      },
      {
          breakpoint: '768px',
          numVisible: 2,
          numScroll: 2
      },
      {
          breakpoint: '560px',
          numVisible: 1,
          numScroll: 1
      }
  ];
  }

  ngOnInit() {
    if(this.historiales == null || this.historiales == undefined) {
      this.historiales = [];
    }
  }

 onCerrarHistorial() {            
  this.cerrarHistorialEmitter.next();      
 }
 
 mostrarSubPosiciones(posicion: any){
  this.historiales.forEach(coti => {
    coti.Cotizacion.CotizacionPosiciones.forEach(pos => {
      if(pos.Id == posicion.Id){
        pos.MostrarSubposiciones = !pos.MostrarSubposiciones;
      }
    });
  });
 }

 mostrarHoras(){
  this.visualizarHoras = !this.visualizarHoras;
 }
}






