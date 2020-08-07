import { Component, OnInit } from '@angular/core';
import { EmpresaGranosService } from '../empresa-granos/empresa-granos.service';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { BaseComponent } from '../../common/base-components/base-component';

@Component({
  selector: 'app-proveedor-detalle',
  templateUrl: './app/alta-proveedores/proveedor-detalle/proveedor-detalle.component.html',
  styleUrls: ['./app/alta-proveedores/proveedor-detalle/proveedor-detalle.component.css', '../Content/css/bootstrap.min.css']
})
export class ProveedorDetalleComponent extends BaseComponent implements OnInit {

  constructor(protected service: EmpresaGranosService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securytiService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
    super(navService, securytiService, floatMsgService, modalService);
  }
  ngOnInit(): void {
  }

}
