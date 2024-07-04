import { animate, state, style, transition, trigger } from "@angular/animations";

export const mainAnimation = trigger('animateMain', [
    state('openMain', style({
        width: 'calc(100% - 260px)',
        position: 'relative',
        left: '260px',
        top: '0px',
    })),
    state('closedMain', style({
        width: 'calc(100% - 5rem)',
        position: 'relative',
        left: '5rem',
        top: '-1rem',
    })),
    transition('openMain <=> closedMain', animate('450ms ease')),
])