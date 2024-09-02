import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({
    selector: '[appHoverLeftBorder]'
})
export class HoverLeftBorderDirective {
    // Output event that can be listened to by the parent component
    @Output() hoverLeftBorder = new EventEmitter<void>();

    constructor(private el: ElementRef) { }

    @HostListener('mousemove', ['$event'])
    onMouseMove(event: MouseEvent) {
        const rect = this.el.nativeElement.getBoundingClientRect();
        const distanceFromLeftBorder = event.clientX - rect.left;
        // Adjust '10' based on how wide you want the hover area to be
        if (distanceFromLeftBorder >= 0 && distanceFromLeftBorder <= 5) {
            this.hoverLeftBorder.emit();
        }
    }
}