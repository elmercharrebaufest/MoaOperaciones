import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { EcheqBaseComponent } from '../echeq.component';
import { EcheqService } from '../echeq.service';

@Component({
  selector: 'app-mis-echeq',
  templateUrl: './mis-echeq.component.html',
  styleUrls: ['./mis-echeq.component.css'],
  providers: [{ provide: EcheqService}]
})
export class MisEcheqComponent extends EcheqBaseComponent  implements OnInit{
  

  constructor(protected echeqService: EcheqService, 
    protected navService: NavService,  
    protected sessionDataService: SessionDataService, 
    protected securityService: SecurityService,
    protected floatMsgService: FloatMsgService, 
    protected modalService: ModalService,
    public confirmationService: ConfirmationService,
    protected route: ActivatedRoute, 
    protected router: Router) {
        super(echeqService, navService, sessionDataService, securityService, floatMsgService, modalService);
    }



  // setTabs() {
  //   this.setMenuSeccionTab("echeq", "Mis Echeq");
  // }
 
  ngOnInit() {
    this.setTabs();
    this.setMenuSeccionTab("echeq", "Mis Echeq");
}
  
  checkPermisos() {
    this.securityService.tienePermisoRedirect("VER ECHEQ");
  }
 
}


