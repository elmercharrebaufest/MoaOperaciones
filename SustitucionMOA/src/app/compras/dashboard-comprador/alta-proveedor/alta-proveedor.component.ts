import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ConfirmationService } from 'primeng/api';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { MensajeComponent } from '../../../common/view-child/mensaje/mensaje.component';
import { ResultadoDto } from '../../../modelos/resultadoDto';
import { UsuarioService } from '../../../usuario/usuario.service';
import { ComprasService } from '../../compras.service';
import { AltaNuevoProveedor } from '../../solp-compra';

@Component({
  selector: 'app-alta-proveedor',
  templateUrl: './alta-proveedor.component.html',
  styleUrls: ['./alta-proveedor.component.css', '../../compras.component.css'],
  providers: [ComprasService, UsuarioService]
})
export class AltaProveedorComponent extends ListBaseComponent implements OnInit {

  @Input() displayAltaProveedor: boolean;
  displayProvCreado: boolean;
  datoProveedor: string;
  altaNuevoProveedor: AltaNuevoProveedor;
  visualizarAlert = false;
  resultadoProveedor: AltaNuevoProveedor;

  @BlockUI() blockUI: NgBlockUI;
  mensajeError: string;
  resultado: ResultadoDto;

  @Output() cerrarPopupProveedorEmitter = new EventEmitter();
  @Output() proveedorDtoEmitter = new EventEmitter<{ proveedorDto: AltaNuevoProveedor }>();

  constructor(protected service: ComprasService, protected usuarioService: UsuarioService, protected navService: NavService, protected sessionDataService: SessionDataService,
    protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
  }



  ngOnInit() {
    this.limpiarCampos();
  }

  altaNuevoProveedorCompras() {
    this.validarMail();
    if(!this.visualizarAlert){
      try {
        this.blockUI.start('Cargando...');
        this.usuarioService.altaNuevoProveedorCompras(this.altaNuevoProveedor).subscribe(
          (result: any) => {
            if (result.logout == true) {
              this.sessionDataService.logout();         
            } else if (result.error != undefined && result.error != "") {           
               this.mensajeError = result.error;             
               this.visualizarAlert = true;             
              // this.floatMsgService.setErrorMsg(result.error);            
            } else if (result.info != undefined) {              
               this.mensajeError = result.info;              
              this.visualizarAlert = true;            
               //  this.floatMsgService.setInfoMsg(result.info);
            } else {
              this.resultado = result.data;
              this.validarResultado(this.resultado);
             
              if(!this.visualizarAlert){
                this.mensajeError = "";
                this.displayProvCreado = true;
                this.datoProveedor = this.resultado.Descripcion;
                this.nuevoProveedor(this.resultado.ProveedorDto);
              }
              this.blockUI.stop();
            }
          },
          error => {
            this.floatMsgService.setErrorMsg(error.message);
            this.mensajeError = error.message;              
            this.visualizarAlert = true; 
            this.blockUI.stop();
          });
      } catch (e) {
        this.floatMsgService.setErrorMsg(e);
        this.mensajeError = e;              
        this.visualizarAlert = true; 
        return false; //<-- Prevent Refresh
      }
    }
    return false; //<-- Prevent Refresh
  }

  salirPopupProveedor() {
    this.displayAltaProveedor = false;
    this.displayProvCreado = false;
    this.visualizarAlert = false;
    this.cerrarPopupProveedorEmitter.next();
    this.limpiarCampos();
  }

  validarResultado(resultadoDto){
    if(resultadoDto.Errores != null && resultadoDto.Errores.length > 0){
        this.mensajeError = resultadoDto.Errores[0].Message;
        return this.visualizarAlert = true;
    }
    this.visualizarAlert = false;
  }

  validarMail(){
    const validateEmailRegex = /^\S+@\S+\.\S+$/;
    if(this.altaNuevoProveedor.Mail != "" && validateEmailRegex.test(this.altaNuevoProveedor.Mail) == false){
      this.mensajeError = "Verificar formato del mail";
      return this.visualizarAlert = true;
    }
    this.visualizarAlert = false;
  }

  limpiarCampos(){
    this.altaNuevoProveedor = { CUIT: null, Mail: null, RazonSocial: null };
  }

  enviarProveedorDto() {
    this.proveedorDtoEmitter.next({ proveedorDto: this.resultadoProveedor });
  }

  nuevoProveedor(proveedor){
    this.resultadoProveedor = { CUIT: proveedor.CUIT, Mail: proveedor.Mail, RazonSocial: proveedor.RazonSocial, Id: proveedor.Id };
  }
  
}
