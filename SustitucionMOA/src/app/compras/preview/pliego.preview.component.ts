import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../../common/base-components/base-component';
import { jsPDF } from "jspdf";
import { Solp } from '../solp/solp';
import { ComprasService } from '../compras.service';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
    selector: 'pliego-preview',
    templateUrl: './pliego.preview.component.html',
    styleUrls: ['../compras.component.css'],
})
export class PliegoPreviewComponent extends BaseComponent implements OnInit {

    _pdf;

    @Input('src')
    set pdf (value) {
        this._pdf = this.sanitizer.bypassSecurityTrustResourceUrl(value);
    }

    constructor(protected service: ComprasService, protected navService: NavService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService, 
        protected sessionDataService: SessionDataService, private sanitizer: DomSanitizer) {
        super(navService, securityService, floatMsgService, modalService);
    }
    
    ngOnInit() {
    }
}
