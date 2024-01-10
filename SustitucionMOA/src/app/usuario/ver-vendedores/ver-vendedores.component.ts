import { Component, OnInit, ViewChild } from '@angular/core';
import { MessageService } from 'primeng/api';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { UsuarioService } from '../usuario.service';
import { SessionDataService } from '../../common/services/SessionDataService';

@Component({
  selector: 'app-ver-vendedores',
  templateUrl: './ver-vendedores.component.html',
  styleUrls: ['./ver-vendedores.component.css'],
  providers: [MessageService]
})

export class VerVendedoresComponent implements OnInit {

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;

  vendedores: any;
  usuarioId: any;
  data: any;

  constructor(protected service: UsuarioService,
    private messageService: MessageService,
    protected sessionDataService: SessionDataService) {
    this.spinnerComponent = new SpinnerComponent();
    this.service.getUsuarioVerVendedores().subscribe(usuario =>{
      this.usuarioId = usuario;
      this.vendedores = null;
      if (usuario!=null && usuario >0) this.getVendedoresUsuario(usuario.toString());
    });
   }

  ngOnInit() {
  }

  getVendedoresUsuario(usuarioId: string) {
    this.spinnerComponent.showIt();
    try {
        this.service.getProvedoresUsuario(usuarioId).subscribe(
            (result:any) => {
                this.spinnerComponent.hideIt();
                    this.vendedores = result.data.proveedores;
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

  desasociarVendedor(vendedor: any){

      if(this.vendedores.length === 1){
        this.messageService.add({
          key: "toastAlerta",
          severity: "error",
          summary: "¡Atención!",
          detail: `No se puede realizar la desasociacion. El usuario opera unicamente con este vendedor.`
      });
      return;
      }
      try {
        this.service.desasociarVendedor(this.usuarioId, vendedor.Id).subscribe(
            (result:any) => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                  this.sessionDataService.logout();
              } else if (result.error != undefined && result.error != "") {
                  this.mostrarMsj(result.error, 'error');
                  
              } else if (result.info != undefined) {
                  this.mostrarMsj(result.info, 'warn');
              } else {
                this.data = result.data;
                this.getVendedoresUsuario(this.usuarioId);
              }
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

  mostrarMsj(msj: string, severity: string){
      this.messageService.add({
        key: "toastPopupDetalles",
        severity: severity,
        summary: "¡Atención!",
        detail: msj
    });
  }

}
