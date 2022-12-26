import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { AplicacionCcppBaseComponent } from '../aplicacion-ccpp.base.component';
import { AplicacionCCPP, SeccionAplicacionCCPP } from '../aplicacion-ccpp.model';
import { AplicacionCcppService } from '../aplicacion-ccpp.service';
import { DropdownComponent } from '../../common/view-child/dropdown/dropdown.component';

@Component({
  selector: 'app-listado',
  templateUrl: './listado.component.html',
  styleUrls: ['./listado.component.css']
})
export class ListadoComponent extends AplicacionCcppBaseComponent implements OnInit,AfterViewInit {
  @ViewChild("FiltroEstado")filtroEstadoComponent:DropdownComponent;
  aplicaciones?: AplicacionCCPP[];
  contratoFiltro = new FormControl();
  ccppFiltro = new FormControl();
  estadoFiltro = new FormControl();
  disabled = false;
  show = false;
  constructor(
    protected service: AplicacionCcppService,
    protected navService: NavService,
    protected sessionDataService: SessionDataService,
    protected securityService: SecurityService,
    protected floatMsgService: FloatMsgService,
    protected modalService: ModalService) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
  }

  ngOnInit() {
    this.crearSecciones();
  }
  exportExcelAplicacionesCCPP() { }
  get isVisible(): boolean {
    return this.aplicaciones && !!this.aplicaciones.length && this.show
  }
  getListadoFechas() {
    this.show = false;
    this.getListado();
    this.setMenuSeccionTab(SeccionAplicacionCCPP, "Estado de cargas");
  }
  getListado() { }
  ngAfterViewInit(): void {
    this.filtroEstadoComponent.setSelectItem("");
  }
}
