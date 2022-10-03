import { Component, ElementRef, OnInit } from '@angular/core';
import { EcheqComponent } from '../echeq.component';
import { EcheqService } from '../echeq.service';

@Component({
  selector: 'app-mis-echeq',
  templateUrl: './mis-echeq.component.html',
  styleUrls: ['./mis-echeq.component.css'],
  providers: [{ provide: EcheqService}]
})
export class MisEcheqComponent extends EcheqComponent {

  ngOnInit() {
    this.setTabs();
    this.checkPermisos();
  }
  
  setTabs() {
    this.setMenuSeccionTab("echeq", "Mis Echeq");
  }
  
  checkPermisos() {
    this.securityService.tienePermisoRedirect("VER ECHEQ");
  }
 
}


