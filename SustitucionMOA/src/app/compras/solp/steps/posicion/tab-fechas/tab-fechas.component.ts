import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ListBaseComponent } from '../../../../../common/base-components/list-base-component';
import { FloatMsgService } from '../../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../../common/services/ModalService';
import { NavService } from '../../../../../common/services/NavService';
import { SecurityService } from '../../../../../common/services/SecurityService';
import { SessionDataService } from '../../../../../common/services/SessionDataService';
import { ComprasService } from '../../../../compras.service';
import { Solp } from '../../../solp';
import { SolpPosicion } from '../../../solp-posicion';

@Component({
  selector: 'tab-fechas',
  templateUrl: './tab-fechas.component.html',
  styleUrls: ['../../../../compras.component.css']
})
export class TabFechasComponent extends ListBaseComponent {

  @Input('model')
  protected model: Solp;

  @Input('locale')
  protected locale: any;

  @Input('posicion')
  protected posicion: SolpPosicion;

  constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
    protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, protected router: Router) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
  }

  fechaEntregaServicio: any;
  fechaDeLiberacion: any;
  hoy: Date = new Date();

  camposObligatorios: any[] = [
    { campo: 'fechaEntregaServicio', esObligatorio: false, esFijo: false },
    { campo: 'plazoDeEntrega', esObligatorio: true, esFijo: true }
  ];

  @Output() onEstCompleto = new EventEmitter<any>();

  ngOnChanges() {
    this.posicion.validateFechas();
  }

  public ngOnDestroy(): void {
    super.ngOnDestroy();
    this.posicion.validateFechas();
  }

  calcularFechaEntrega() {
      let fechaNueva = new Date(this.model.fechaEntrega);
      fechaNueva.setDate(fechaNueva.getDate() + parseInt(this.model.posicionActual.plazoDeEntrega.toString()));
      this.model.posicionActual.fechaEntregaServicio = fechaNueva;
  }

  calcularEntregaCalendario() {
    let today = new Date();    
    let fechaAValidar = new Date(this.model.posicionActual.fechaEntregaServicio);
    if (fechaAValidar > today) {
      // resta de fecha seleccionada menos la fecha minima parseada en dias
      let plazoNuevo = Math.ceil((this.model.posicionActual.fechaEntregaServicio.getTime() - this.model.fechaEntrega.getTime()) / (1000 * 60 * 60 * 24));
      if(plazoNuevo >= 0){
          this.model.posicionActual.plazoDeEntrega = plazoNuevo;          
      } else {
          this.model.posicionActual.fechaEntregaServicio = new Date(this.model.fechaEntrega);
          this.model.posicionActual.plazoDeEntrega = 0;
      }
    } else {
      this.model.posicionActual.plazoDeEntrega = 0;
    }
  }

  mostrarValidacion(campoAValidar, vacio){
    let camposVacios = this.camposObligatorios.find(x => x.campo == campoAValidar && x.esObligatorio);
    return (camposVacios != null && vacio == "");
  }

  public get configurarFechaCalendar(): boolean {

    if(this.model.trabajoHecho){
        return true;
    }

    if(this.model.trabajoHecho && this.model.urgencia){
        return true;
    }

    if(this.model.trabajoHecho && this.model.adicional){
        return true;
    }
    
    return false;
  }

}