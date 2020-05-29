import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'shortenString'
})
export class ShortenStringPipe implements PipeTransform {
    transform(value: string): any {
        if (value.length > 8) {
            return value.slice(0, 8) + "...";
        } else {
            return value;
        }
    }
}