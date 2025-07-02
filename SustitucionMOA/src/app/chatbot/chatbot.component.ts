import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '../common/base-components/base-component';
import { SecurityService } from '../common/services/SecurityService';
import { NavService } from '../common/services/NavService';
import { AduanaService } from '../aduana/aduana.service';
import { FloatMsgService } from '../common/services/FloatMsgService';
import { ModalService } from '../common/services/ModalService';

@Component({
    selector: 'app-chatbot',
    templateUrl: './chatbot.component.html',
    styleUrls: ['./chatbot.component.css']
})
export class ChatbotComponent extends BaseComponent implements OnInit {
    constructor(protected navService: NavService, protected service: AduanaService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
    }

    isChatbotVisible = false;

    ngOnInit() {
        this.isChatbotVisible = false;
    }

    toggleChatbot(): void {
        this.isChatbotVisible = !this.isChatbotVisible;
    }

    visible() {
        let tienepermiso = this.securityService.tienePermiso("CHATBOTMOLAVISIBLE");
        // se usa este permiso para no crear uno nuevo y es el unico que tiene solo adminsitracion.
        //borrar cuando se pase a produccion todo la logica de permisos para ver le chat.
        return tienepermiso;
    }
}