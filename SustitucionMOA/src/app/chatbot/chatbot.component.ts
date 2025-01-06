import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '../common/base-components/base-component';

@Component({
    selector: 'app-chatbot',
    templateUrl: './chatbot.component.html',
    styleUrls: ['./chatbot.component.css']
})
export class ChatbotComponent extends BaseComponent implements OnInit {

    isChatbotVisible = false;

    ngOnInit() {
        this.isChatbotVisible = false;
    }

    toggleChatbot(): void {
        this.isChatbotVisible = !this.isChatbotVisible;
        console.log("this.isChatbotVisible ", this.isChatbotVisible);
    }
}