import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { PeticionDeOfertaDto, PeticionDeOfertaSolpPosicionDto, PeticionDeOfertaUsarioDto } from '../../modelos/peticion-de-oferta-model';
import { Table } from 'primeng/table';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { UsuarioService } from '../../usuario/usuario.service';
import { ComprasService } from '../compras.service';
import { CotizacionDto, CotizacionHoraDto } from '../../modelos/cotizacionDto';

@Component({
  selector: 'app-panel-horas',
  templateUrl: './panel-horas.component.html',
  styleUrls: ['./panel-horas.component.css']
})
export class PanelHorasComponent extends ListBaseComponent implements OnInit {

  constructor(protected service: ComprasService, protected usuarioService: UsuarioService, protected navService: NavService, protected sessionDataService: SessionDataService,
    protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
}

  @ViewChild("tabla")
  protected tabla: Table;

  @Input() peticionHs: CotizacionHoraDto[];
  @Input() cotizacion: CotizacionDto[];


  ngOnInit() {}

}
