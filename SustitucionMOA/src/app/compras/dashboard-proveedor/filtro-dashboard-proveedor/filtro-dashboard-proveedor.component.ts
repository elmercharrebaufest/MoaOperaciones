import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { SpinnerComponent } from '../../../common/view-child/spinner/spinner.component';
import { ComprasService } from '../../compras.service';

@Component({
  selector: 'app-filtro-dashboard-proveedor',
  templateUrl: './filtro-dashboard-proveedor.component.html',
  styleUrls: ['./filtro-dashboard-proveedor.component.css']
})
export class FiltroDashboardProveedorComponent extends ListBaseComponent  {

   //#region Variables 
   private filtroForm: FormGroup;   
   nroSolp: string;
   @ViewChild(SpinnerComponent)
   protected spinnerComponent: SpinnerComponent;
   usuario: string;
   //#endregion
 
   // #region Observables
   constructor(protected service: ComprasService, protected navService: NavService,
     protected sessionDataService: SessionDataService, protected securityService: SecurityService,
     protected floatMsgService: FloatMsgService, protected modalService: ModalService,
     protected route: ActivatedRoute, protected router: Router, private formBuilder: FormBuilder) {
     super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
     this.usuario = sessionStorage.getItem("username");
     this.setFiltroBuquedaForm();
     this.onBuscar();
 }

 ngOnInit(): void {    
  }

  public setFiltroBuquedaForm() {
    this.filtroForm = this.formBuilder.group({
      nroSolp: '',
    });   
  }

  onBuscar() {
    this.service.getListarPOProveedor(1, 10, "", "", this.filtroForm.controls.nroSolp.value);  
  }
  
  public getFiltroForm() {
    return this.filtroForm;
  }

}
