import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../../base-components/base-component';
import { FloatMsgService } from '../../services/FloatMsgService';
import { ModalService } from '../../services/ModalService';
import { NavService } from '../../services/NavService';
import { SecurityService } from '../../services/SecurityService';
import { SessionDataService } from '../../services/SessionDataService';
import { SpinnerComponent } from '../../view-child/spinner/spinner.component';
import { BuscadorService } from './buscador.service';
import { Resultado } from './Buscador';

@Component({
  selector: 'app-buscador-inteligente',
  templateUrl: './buscador.component.html',
  styleUrls: ['./buscador.component.css'],
  providers: [BuscadorService],
})
export class BuscadorComponent extends BaseComponent implements OnInit {

  @ViewChild("spinnerModal")
  protected spinnerModalComponent: SpinnerComponent;

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;

  constructor(protected service: BuscadorService, protected navService: NavService,
    protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
    protected floatMsgService: FloatMsgService, protected modalService: ModalService) {

    super(navService, securytiService, floatMsgService, modalService);
    this.spinnerComponent = new SpinnerComponent();
    this.spinnerModalComponent = new SpinnerComponent();
  }

  palabraABuscar: string;
  resultados: Resultado[] = [];

  resultado1: Resultado = {Id: 1, Value: 'hola', Link: 'hola.com', Tipo: 'Contrato'}
  resultado2: Resultado = {Id: 2, Value: 'hola2', Link: 'hola2.com', Tipo: 'Contrato2'}

  ngOnInit() {
    var input = document.getElementById("buscador");

    input.addEventListener("keyup", function(event) {
      if (event.keyCode === 13) {
        event.preventDefault();
        document.getElementById("autoBusqueda").click();
      }
    }); 
  }

  autoBusqueda(){
    this.resultados = [];
    try {
      this.subscription = this.service.getResultados(this.palabraABuscar).subscribe(
          (result: any) => {
              if (result.logout == true) {
                  this.sessionDataService.logout();
              } else if (result.error != undefined && result.error != "") {
                  this.floatMsgService.setErrorMsg(result.error);
              } else if (result.info != undefined) {
                  this.floatMsgService.setInfoMsg(result.info);
              } else {
                  this.resultados = result.data;
              }
          },
          error => {
              this.floatMsgService.setErrorMsg(error.message);
          }
      );
    } catch (e) {
        this.floatMsgService.setErrorMsg(e);
        return false; //<-- Prevent Refresh
    }
  }

}
