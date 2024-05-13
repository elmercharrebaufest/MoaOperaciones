import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { SpinnerComponent } from '../../../common/view-child/spinner/spinner.component';
import { ComprasService } from '../../compras.service';
import { Subscription } from 'rxjs';
import { ListadoDashboardCertificacionDeServiciosComponent } from '../listado-dashboard-certificacion-de-servicios/listado-dashboard-certificacion-de-servicios.component';

@Component({
  selector: 'app-filtro-dashboard-certificacion-de-servicios',
  templateUrl: './filtro-dashboard-certificacion-de-servicios.component.html',
  styleUrls: ['./filtro-dashboard-certificacion-de-servicios.component.css']
})
export class FiltroDashboardCertificacionDeServiciosComponent extends ListBaseComponent {

  @ViewChild(SpinnerComponent)

   //#region Variables 
   filtroForm: FormGroup;
   filtroFormOrden: FormGroup
   proveedor: string;
   ordenCompraId: string;
   length = 0;
   fechaInicio = "2023-10-01";
   fechaFin = "";
   protected spinnerComponent: SpinnerComponent;
   usuario: string;
  //  subscripcionPO: Subscription;
   tablaPO: any;
   //#endregion
 
   // #region Observables
   constructor(protected service: ComprasService, protected navService: NavService,
     protected sessionDataService: SessionDataService, protected securityService: SecurityService,
     protected floatMsgService: FloatMsgService, protected modalService: ModalService,
     protected route: ActivatedRoute, protected router: Router, private fb: FormBuilder,
     protected listadoComponent: ListadoDashboardCertificacionDeServiciosComponent) {
     super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
     this.usuario = sessionStorage.getItem("username");
     this.getFiltroForm();
     this.setFiltroBuquedaForm();
 }

 ngOnInit(): void {  
  this.filtroForm = new FormGroup ({
    proveedor: new FormControl(''),
    ordenCompraId: new FormControl(''),
    fechaInicio: new FormControl('')
  });  

  this.spinnerComponent = new SpinnerComponent();
  }

  public setFiltroBuquedaForm() {
    this.filtroForm = this.fb.group({
      proveedor: [''],
        ordenCompraId: [''],
        fechaInicio: ['']
    }); 
  }

  getFiltroForm() {
    return this.filtroForm;
  }
  
  onBuscar() {
    this.proveedor = this.filtroForm.get('proveedor').value;
      this.ordenCompraId = this.filtroForm.get('ordenCompraId').value
      this.fechaInicio = this.filtroForm.get('fechaInicio').value;
      this.listadoComponent.getListarPO(this.proveedor, this.ordenCompraId, this.fechaInicio, this.fechaFin);
  }
}
