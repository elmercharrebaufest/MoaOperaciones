import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
  selector: '[numeric-format]'
})
export class FormatNumericDirective {

  @Input('decimals') decimals: number = 2; // Por defecto, se permiten hasta 2 decimales

  private check(value: string, decimals: number) {
    const decimalSeparator = '.';
    const regExpString = `^\\s*\\d+(?:[${decimalSeparator}]\\d{0,${decimals}})?$`;
    return String(value).match(new RegExp(regExpString));
  }

  private specialKeys = [
    'Backspace', 'Tab', 'End', 'Home', 'ArrowLeft', 'ArrowRight', 'Delete'
  ];

  constructor(private el: ElementRef) {}

  @HostListener('keydown', ['$event'])
  onKeyDown(event: KeyboardEvent) {
    const currentValue: string = this.el.nativeElement.value;
    const selectionStart: number = this.el.nativeElement.selectionStart;
    const selectionEnd: number = this.el.nativeElement.selectionEnd;
    const nextValue: string =
      currentValue.slice(0, selectionStart) +
      event.key +
      currentValue.slice(selectionEnd);
  
    if (
      this.specialKeys.indexOf(event.key) !== -1 ||
      (event.key === '.' &&
        (!currentValue.includes('.') || selectionStart !== 0) &&
        /^\d*$/.test(currentValue.slice(0, selectionStart))) // Permitir el punto solo después de un número y no al principio
    ) {
      return; // Permitir teclas especiales y el punto decimal solo una vez después de un número y no al principio
    }
  
    if (!this.check(nextValue, this.decimals)) {
      event.preventDefault(); // Evitar entrada no válida
    }
  }
}
