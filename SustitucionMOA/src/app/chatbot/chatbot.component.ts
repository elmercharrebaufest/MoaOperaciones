import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-chatbot',
    templateUrl: './chatbot.component.html',
    styleUrls: ['./chatbot.component.css']
})
export class ChatbotComponent implements OnInit {

    isChatbotVisible = false;
    constructor() { }

    ngOnInit() {
        this.isChatbotVisible = false;
    }


    toggleChatbot(): void {
        this.isChatbotVisible = !this.isChatbotVisible;
    }
}