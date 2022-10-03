import { Component, ElementRef, OnInit } from '@angular/core';
import { EcheqComponent } from '../echeq.component';
import { EcheqService } from '../echeq.service';

@Component({
  selector: 'app-gestion',
  templateUrl: './gestion.component.html',
  styleUrls: ['./gestion.component.css'],
  providers: [{ provide: EcheqService}]
})
export class GestionEcheqComponent extends EcheqComponent {

  ngOnInit() {
    this.setTabs();
    this.checkPermisos();

  }

  setTabs() {
    this.setMenuSeccionTab("echeq", "Gestion");
  }

  checkPermisos() {
    this.securityService.tienePermisoRedirect("VER ECHEQ");
  }

}
