
import { Pipe, PipeTransform } from '@angular/core';
import { Formatter } from '../formatter/Formatter';

@Pipe({
    name: 'defaultNumber'
})
export class DefaultNumberParser implements PipeTransform {
    transform(values: number | string): string {
        return Formatter.formatNumberWithPoint(values.toString());
    }
}