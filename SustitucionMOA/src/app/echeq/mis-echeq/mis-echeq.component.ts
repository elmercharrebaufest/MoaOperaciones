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
import { EcheqContrato } from '../gestion/echeq-contrato.model';
import { EcheqReporte } from './mis-echeq-reporte.model';
import { MisEcheqFilter } from './mis-echeq.filtros/mis-echeq-filter.model';

@Component({
  selector: 'app-mis-echeq',
  templateUrl: './mis-echeq.component.html',
  styleUrls: ['./mis-echeq.component.css'],
  providers: [EcheqService]
})
export class MisEcheqComponent extends EcheqBaseComponent implements OnInit{
  

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

  ngOnInit() {
    this.setTabs();
    this.setMenuSeccionTab("echeq", "Mis Echeq");
    super.ngOnInit();
    this.checkPermisos();
  }

  //Lo voy a usar para filtros
  public datosReporte : Array<EcheqReporte> = new Array<EcheqReporte>();
  

  checkPermisos() {
    this.securityService.tienePermisoRedirect("VER ECHEQ");
  }

   //Este es el que me trae la info apenas entro al modulo
   getDatosReporte(filter: MisEcheqFilter) {
    try {
      this.subscription = this.service.GetDatosReporte(filter.periodo, filter.fechaInicio, filter.fechaFin).subscribe(
          (result: any) => {
              if (result.logout == true) {
                  this.sessionDataService.logout();
              } else if (result.error != undefined && result.error != "") {
                  this.floatMsgService.setErrorMsg(result.error);
              } else if (result.info != undefined) {
                  this.floatMsgService.setInfoMsg(result.info);
              } else {
                  this.data = result.data;
                  this.datosReporte = this.data;
                  // this.spinnerComponent.hideIt();
              }
          },
          error => {
              this.floatMsgService.setErrorMsg(error.message);
          }
      );
    } catch (e) {
        this.floatMsgService.setErrorMsg(e);
    }
  }


  // Aplica el filtro a la lista 
    public applyFilter(filter: MisEcheqFilter){
      this.datosReporte = this.data;

      if(filter.CodigoProveedor != "" && filter.CodigoProveedor != undefined){
        this.datosReporte = this.datosReporte.filter( item => item.CodigoProveedor.indexOf(filter.CodigoProveedor) != -1);
      }   

      if(filter.Mail != "" && filter.Mail != undefined){
        this.datosReporte = this.datosReporte.filter( item => item.Mail.indexOf(filter.Mail) != -1);
      }   

      if(filter.Contrato != "" && filter.Contrato != undefined){
        this.datosReporte = this.datosReporte.filter( item => item.Contrato.indexOf(filter.Contrato) != -1);
      }   

      if(filter.Liquidacion != "" && filter.Liquidacion != undefined){
        this.datosReporte = this.datosReporte.filter( item => item.Liquidacion.indexOf(filter.Liquidacion) != -1);
      }   
      
      if(filter.LiquidacionMarcada != "null"){
        let filtro = filter.LiquidacionMarcada == "true";
        this.datosReporte = this.datosReporte.filter( item => item.LiquidacionMarcada == filtro);
      }  

      if(filter.EcheqGenerados != "null"){
        let filtro = filter.EcheqGenerados == "true";
        this.datosReporte = this.datosReporte.filter( item => item.EcheqGenerados == filtro);
      }  

    }

  //
  public searchData(filter: MisEcheqFilter){
      this.getDatosReporte(filter);
  }

 
}


