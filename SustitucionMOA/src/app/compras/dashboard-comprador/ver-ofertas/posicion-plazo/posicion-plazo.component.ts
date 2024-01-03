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
    selector: 'app-posicion-plazo',
    templateUrl: './posicion-plazo.component.html',
    styleUrls: ['./posicion-plazo.component.css']
})
export class PosicionPlazoComponent implements OnInit {

    @Input()
    displayPlazo: boolean;

    @Input()
    public lista: any[] = [];

    @Input('locale') es: any;

    @Output() cerrarPlazoEmitter = new EventEmitter();
    @Output() guardarPlazoEmitter = new EventEmitter<any[]>();

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    }   
    

    ngOnInit() {       
        this.es = {
            firstDayOfWeek: 1,
            dayNames: ["domingo", "lunes", "martes", "miércoles", "jueves", "viernes", "sábado"],
            dayNamesShort: ["dom", "lun", "mar", "mié", "jue", "vie", "sáb"],
            dayNamesMin: ["D", "L", "M", "X", "J", "V", "S"],
            monthNames: ["enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"],
            monthNamesShort: ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"],
            today: 'Hoy',
            clear: 'Borrar'
        }
        
    }   


    onCerrarPlazos() {     
        this.cerrarPlazoEmitter.next();
    }

    onGuardarPlazos(){
        this.guardarPlazoEmitter.emit(this.lista);
    }
        
}
