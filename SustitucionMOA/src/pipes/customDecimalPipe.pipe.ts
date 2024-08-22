import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'customDecimalPipe'
})
export class CustomDecimalPipe implements PipeTransform {
  transform(value: string): string {
    // Reemplazar la coma por un punto y devolver el valor transformado
    return value.replace(',', '.');
  }
}