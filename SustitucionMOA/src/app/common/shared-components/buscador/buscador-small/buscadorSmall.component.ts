import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../../../base-components/base-component';
import { FloatMsgService } from '../../../services/FloatMsgService';
import { ModalService } from '../../../services/ModalService';
import { NavService } from '../../../services/NavService';
import { SecurityService } from '../../../services/SecurityService';
import { SessionDataService } from '../../../services/SessionDataService';
import { SpinnerComponent } from '../../../view-child/spinner/spinner.component';
import { BuscadorService } from './../buscador.service';
import { Resultado, ResultadoTipo } from './../Buscador';
import { OverlayPanel } from 'primeng/overlaypanel';
import { SpinnerSmallComponent } from '../../../view-child/spinner-small/spinner-small.component';
import { BuscadorComponent } from '../buscador.component';

@Component({
  selector: 'app-buscador-inteligente-small',
  templateUrl: './../buscador.component.html',
  styleUrls: ['./../buscador.component.css'],
  providers: [BuscadorService],
})
export class BuscadorSmallComponent extends BuscadorComponent {}