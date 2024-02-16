import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
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
    @Input()
    public adjudicacion: AdjudicacionDto;
    @Input('locale') es: any;

    index: number;
    activeTabs: boolean[] = [false, false, false, false];
    condicionesDeEntrega?: string;
    condicionesDePago?: string;
    garantias?: string;
    textoDeCabecera?: string;

    @Output() cerrarModalTextosEmitter = new EventEmitter();

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router) {
    }

    ngOnInit() {
        if (this.adjudicacion == null) {
            this.adjudicacion = {
                Id: null,
            }
        }
        setTimeout(() => {
            this.condicionesDeEntrega = this.adjudicacion.CondicionesDeEntrega;
            this.condicionesDePago = this.adjudicacion.CondicionesDePago;
            this.garantias = this.adjudicacion.Garantias;
            this.textoDeCabecera = this.adjudicacion.TextoDeCabecera;
            console.log(this.adjudicacion.TextoDeCabecera);
        }, 2000);
    }

    onCerrarTextos(aceptar: boolean) {
        console.log(this.adjudicacion.CondicionesDeEntrega);
        if (aceptar) {
            this.condicionesDeEntrega = this.adjudicacion.CondicionesDeEntrega;
            this.condicionesDePago = this.adjudicacion.CondicionesDePago;
            this.garantias = this.adjudicacion.Garantias;
            this.textoDeCabecera = this.adjudicacion.TextoDeCabecera;
        } else {
            this.adjudicacion.CondicionesDeEntrega = this.condicionesDeEntrega;
            this.adjudicacion.CondicionesDePago = this.condicionesDePago;
            this.adjudicacion.Garantias = this.garantias;
            this.adjudicacion.TextoDeCabecera = this.textoDeCabecera;
        }
        this.closeAccordion();
        this.cerrarModalTextosEmitter.next();
    }

    closeAccordion() {
        this.activeTabs = [false, false, false, false];
    }

}
