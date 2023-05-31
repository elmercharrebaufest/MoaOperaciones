import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbAlert } from '@ng-bootstrap/ng-bootstrap';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ConfirmationService } from 'primeng/api';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { CircularDto } from '../../../modelos/circular-model';
import { PeticionDeOfertaDto } from '../../../modelos/peticion-de-oferta-model';
import { ComprasService } from '../../compras.service';

@Component({
    selector: 'app-orden-de-compra-detalle',
    templateUrl: './ordenDeCompraDetalle.component.html',
    styleUrls: ['./ordenDeCompraDetalle.component.css']
})
export class OrdenDeCompraDetalleComponent implements OnInit, OnChanges {


    @Input()
    displayOrdenDeCompra: boolean;
    @Input()
    public ordenDeCompra: any;       
    @Output() cerrarOrdenDeCompraEmitter = new EventEmitter();
    
    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    }
    ngOnChanges(changes: SimpleChanges): void {
        if(this.ordenDeCompra != null){
       
        }
    }
    
    ngOnInit() {
      
    }   

    onCerrarOrdenDeCompra() {
       
        this.cerrarOrdenDeCompraEmitter.next();
    }
}

