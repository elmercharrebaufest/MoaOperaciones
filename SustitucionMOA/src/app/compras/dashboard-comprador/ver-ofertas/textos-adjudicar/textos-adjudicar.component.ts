import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { AdjudicacionDto } from '../../../../modelos/adjudicacion';
import { ComprasService } from '../../../compras.service';
import { NavService } from '../../../../common/services/NavService';
import { SecurityService } from '../../../../common/services/SecurityService';
import { FloatMsgService } from '../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../common/services/ModalService';
import { SessionDataService } from '../../../../common/services/SessionDataService';

@Component({
    selector: 'app-textos-adjudicar',
    templateUrl: './textos-adjudicar.component.html',
    styleUrls: ['./textos-adjudicar.component.css']
})
export class TextosAdjudicarComponent implements OnInit {

    @Input()
    displayTextos: boolean;

    
    @Input('locale') es: any;

    @Input()
    public adjudicacion: AdjudicacionDto;
    index: number

    textosIncompletos: boolean;

    @Output() cerrarModalTextosEmitter = new EventEmitter<{ textosIncompletos: boolean }>();
    @Output() aceptarModalTextosEmitter = new EventEmitter<{ textosIncompletos: boolean }>();
    activeTabs: boolean[] = [false, false, false, false];

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    }
   
    

    ngOnInit() {       
        if(this.adjudicacion == null){
            this.adjudicacion = {
                Id: null,
            };
        }
    }
    
    onCerrarTextos(aceptar:boolean) {     
        if(aceptar){ 
            this.reestablecerDatos()
        }  
        this.closeAccordion();
        this.cerrarModalTextosEmitter.next();
        this.validacionTextosIncompletos();
      
    }

   reestablecerDatos(){
    this.adjudicacion.CondicionesDeEntrega = "";
        this.adjudicacion.CondicionesDePago = "";
        this.adjudicacion.Garantias = "";
        this.adjudicacion.TextoDeCabecera = "";  
   }

   validacionTextosIncompletos(){
    this.textosIncompletos = false;
    if(this.adjudicacion.CondicionesDeEntrega == "" 
        && this.adjudicacion.CondicionesDePago == "" 
        && this.adjudicacion.Garantias == "" 
        && this.adjudicacion.TextoDeCabecera == ""){
            this.textosIncompletos = true;
    }
   }

  closeAccordion() {
    this.activeTabs = [false,false,false,false];
  }
        
}
