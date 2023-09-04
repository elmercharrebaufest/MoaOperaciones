import { animate, style, transition, trigger } from "@angular/animations";

export const fadeInAnimation = trigger('fadeIn', [
    transition(':enter', [
        style({ opacity: 0, transform: 'translateY(30px)' }),
        animate('300ms', style({ opacity: 1, transform: 'translateY(0)' }))
    ])
])