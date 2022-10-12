import { Component, Output, ViewChild, EventEmitter, OnInit} from '@angular/core';
import { DropdownComponent } from '../../../common/view-child/dropdown/dropdown.component';
import { FiltroFechaComponent } from '../../../common/view-child/filtro-fecha/filtro-fecha.component';
import { EcheqFilter } from './echeq-filter.model';


@Component({
  selector: 'app-echeq-filtros',
  templateUrl: './echeq-filtros.component.html',
  styleUrls: ['./echeq-filtros.component.css']
})
export class FiltrosComponent implements OnInit {

  //Con esto paso el evento a distintos componentes
  @Output() applyFilterEmitter = new EventEmitter<EcheqFilter>();

  @Output() searchDataEmitter = new EventEmitter<EcheqFilter>();

  @ViewChild(DropdownComponent)
  protected itemsPerPageComponent: DropdownComponent;
  
  @ViewChild(FiltroFechaComponent)
  protected filtroFechaComponent: FiltroFechaComponent;

  public echeqFilterModel: EcheqFilter; 
  
  constructor(){
    this.echeqFilterModel = new EcheqFilter();
  }

  ngOnInit(): void {
    this.onChangeFecha();
    
  }
  
  //Ejecuta el evento
  public onApplyFilter(){
    this.applyFilterEmitter.next(this.echeqFilterModel);
  }

  public onChangeFecha(){
    if(this.filtroFechaComponent.getFechaIncio() == "undefined" || this.filtroFechaComponent.getFechaFin() == "undefined"){
      return;
    }
    this.echeqFilterModel.periodo = this.filtroFechaComponent.periodo;
    this.echeqFilterModel.fechaInicio = this.filtroFechaComponent.getFechaIncio();
    this.echeqFilterModel.fechaFin = this.filtroFechaComponent.getFechaFin();
  
    this.searchDataEmitter.next(this.echeqFilterModel);
  }

  public onChangeContrato(){
    this.applyFilterEmitter.next(this.echeqFilterModel);

  }

  public onChangeTipoContrato(){
    this.applyFilterEmitter.next(this.echeqFilterModel);
  }

  public buscarBoton(){
    this.onChangeFecha();
  }
}
