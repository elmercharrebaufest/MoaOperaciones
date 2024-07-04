import { Directive, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({
    selector: '[appMouseLeave]'
})
export class MouseLeaveDirective {
    // Output event that can be listened to by the parent component
    @Output() mouseLeave = new EventEmitter<boolean>();

    @HostListener('mouseleave', ['$event'])
    onMouseLeave(event: MouseEvent) {
        console.log('leave?')
        this.mouseLeave.emit(false);
    }
}