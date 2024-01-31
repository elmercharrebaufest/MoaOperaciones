import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { EcheqBaseComponent } from '../echeq.component';
import { EcheqService } from '../echeq.service';
import { EcheqContrato } from './echeq-contrato.model';
import { EcheqFilter } from './echeq.filtros/echeq-filter.model';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';


@Component({
    selector: 'app-echeq-gestion',
    templateUrl: `echeq-gestion.component.html`,
    providers: [EcheqService]
})


export class EcheqGestionComponent extends EcheqBaseComponent implements OnInit{

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

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
        super.ngOnInit();
        this.setTabs();
        this.checkPermisos();
        this.setMenuSeccionTab("echeq", "Gestion");
    }

    //Aca se me va a llenar la lista de contratos con lo que me devuelve el servicio
    public echeqContratos : Array<EcheqContrato> = new Array<EcheqContrato>();

    //Lo voy a usar para filtros
    public contratosFiltrados : Array<EcheqContrato> = new Array<EcheqContrato>();

    //Este es el que me trae la info apenas entro al modulo
    public getContratoPendientePago(filter: EcheqFilter) {
        try {
            this.spinnerComponent.showIt();
            this.echeqService.GetData(filter.periodo, filter.fechaInicio, filter.fechaFin).subscribe(response => {
                this.spinnerComponent.hideIt();
                if (response.logout == true) {
                    this.sessionDataService.logout();
                } else if (response.error != undefined && response.error != "") {
                    this.floatMsgService.setErrorMsg(response.error);
                } else if (response.info != undefined) {
                    this.floatMsgService.setInfoMsg(response.info);
                } else {
                    //para mapear el model
                    this.echeqContratos = response.data.map( res => {
                        return new EcheqContrato(res)
                    });
                    this.contratosFiltrados = this.echeqContratos;
            }
                    },  
            error => {
                this.floatMsgService.setErrorMsg(error.message);
                this.spinnerComponent.hideIt();
            });
           
        } 
        catch (e) {
            this.floatMsgService.setErrorMsg(e);    
            this.spinnerComponent.hideIt();      
        }   
    }

    //Aplica el filtro a la lista 
    public applyFilter(filter: EcheqFilter){
       this.contratosFiltrados = this.echeqContratos;

       if(filter.tipoContrato != "Todos"){
        this.contratosFiltrados = this.contratosFiltrados.filter( item => item.tipoContrato == filter.tipoContrato);
       }
        
       if(filter.contrato != "" && filter.contrato != undefined){
        this.contratosFiltrados = this.contratosFiltrados.filter( item => item.contrato.indexOf(filter.contrato) != -1);
       }        
    }

    //
    public searchData(filter: EcheqFilter){
        this.getContratoPendientePago(filter);
    }

}

