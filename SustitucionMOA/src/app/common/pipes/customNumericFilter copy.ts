import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'customNumber',
})
export class CustomNumberPipe implements PipeTransform {
    transform(
        value: number | string,
        locale?: string,
        style?: string,
        currency?: string,
    ): string {
        const numberFormat = new Intl.NumberFormat(locale || 'es-AR', { style, currency });
        return numberFormat.format(Number(value));
    }
}
