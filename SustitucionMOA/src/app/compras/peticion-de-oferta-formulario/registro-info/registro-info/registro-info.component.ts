import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { SolpCompraDto } from '../../../solp-compra';
import { ComprasService } from '../../../compras.service';
import { UsuarioService } from '../../../../usuario/usuario.service';
import { NavService } from '../../../../common/services/NavService';
import { SessionDataService } from '../../../../common/services/SessionDataService';
import { ModalService } from '../../../../common/services/ModalService';
import { FormBuilder } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { FloatMsgService } from '../../../../common/services/FloatMsgService';
import { SecurityService } from '../../../../common/services/SecurityService';
import { ActivatedRoute, Router } from '@angular/router';
import { ListBaseComponent } from '../../../../common/base-components/list-base-component';
import { RegistroInfoDto } from '../../../../modelos/registro-info';

@Component({
  selector: 'registro-info',
  templateUrl: './registro-info.component.html',
  styleUrls: ['./registro-info.component.css']
})

export class RegistroInfoComponent extends ListBaseComponent implements OnInit, OnChanges{

  @Input() solpCompraDto: SolpCompraDto;
  @Input() displayRegistroInfo: SolpCompraDto;

  @Output() cancelarRegistroEmitter = new EventEmitter();

  options: any[] = new Array()

  registrosInfo: RegistroInfoDto[] = new Array()
  displayConfirmacion: boolean;
  resultado: any;

  

  constructor(protected service: ComprasService, protected usuarioService: UsuarioService, protected navService: NavService, protected sessionDataService: SessionDataService,
    protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

    }
  ngOnChanges(changes: SimpleChanges): void {
    this.inicializarDatos();
  }

  ngOnInit() {
    this.inicializarDatos();
    }

  inicializarDatos(){
    this.registrosInfo = this.solpCompraDto.RegistrosInfo;
    if(this.solpCompraDto != undefined && this.solpCompraDto.PosicionCompras != null){     
      this.options = this.solpCompraDto.PosicionCompras.map(x => ({
        label: x.Indice + " - " + x.Tarea,
        value: x.Id
      })); 
     }

  }
  onCancelarRegistroInfo() {
    this.cancelarRegistroEmitter.next();
  }


  onHideRegistroDialog(dd) {
    this.cancelarRegistroEmitter.next();
    
  }

  filtrarPorPosicion(dd){
   this.registrosInfo = this.solpCompraDto.RegistrosInfo.filter(x => x.PosicionId == dd.value.value);
  }

  abrirModalConfirmacion(){
    this.displayConfirmacion = true;    
    let registros = this.solpCompraDto.RegistrosInfo.filter(x => x.Confirmado);

    let descripcion = {}
    console.log(registros, "registros")
  //Recorremos el arreglo 
  registros.forEach( x => {
  //Si la ciudad no existe en nuevoObjeto entonces
  //la creamos e inicializamos el arreglo de profesionales. 
  if(!descripcion.hasOwnProperty(x.Codigo)){
    descripcion[x.Codigo] = {
      detalle: []
    }
  }
  
  //Agregamos los datos de profesionales. 
  descripcion[x.Codigo].detalle.push({
      nombre: x.NombreProveedor + " - " + x.Cuit,
      descripcion: x.Indice + x.DescripcionPosicion + 
      x.CantidadAdjudicacion + x.Moneda + x.Unidad + x.Precio
    })  
  })
  this.resultado =  descripcion;
  console.log(this.resultado, "resultado");
  console.log(this.resultado.detalle, "resultado");
  }

  cerrarModalConfirmacion(){
    this.displayConfirmacion = false;
  }
 

}
