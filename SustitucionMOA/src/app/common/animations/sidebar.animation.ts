import { animate, state, style, transition, trigger } from "@angular/animations";

export const sidebarAnimation = trigger('animateSidebar', [
    state('open', style({
        width: '260px', // Adjust as needed
        opacity: 1,
    })),
    state('closed', style({
        width: '0px',
        opacity: 0,
    })),
    transition('open <=> closed', animate('500ms ease')),
])