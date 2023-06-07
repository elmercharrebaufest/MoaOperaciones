import { Component, OnInit, ViewChild } from '@angular/core';
import { UsuarioService } from '../usuario.service';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';

@Component({
  selector: 'app-usuario-auditoria-list',
  templateUrl: './usuario-auditoria-list.component.html',
  styleUrls: ['./usuario-auditoria-list.component.css']
})
export class UsuarioAuditoriaListComponent implements OnInit {

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;
  data: any;
  itemsPerPage = 20;
  
  constructor(protected service: UsuarioService) { 
    this.spinnerComponent = new SpinnerComponent();
    this.service.getUsuarioCargarAuditoria().subscribe(usuario =>{
      this.data = null;
      if (usuario!=null && usuario >0) this.getAuditoriaUsuario(usuario.toString());
    });
  }

  ngOnInit() {

  }
  getAuditoriaUsuario(idUsuario: string) {
    this.spinnerComponent.showIt();
    try {
        this.service.getProveedorAuditoriaPorUsuario(idUsuario).subscribe(
            (result:any) => {
                this.spinnerComponent.hideIt();
                    this.data = result.data.usuario;
            },
            error => {
                this.spinnerComponent.hideIt();
            }
        );
    } catch (e) {
        this.spinnerComponent.hideIt();
        return false; //<-- Prevent Refresh
    }
  }

  isVisible() {
    return this.data && this.data.length != 0;
  }
}
