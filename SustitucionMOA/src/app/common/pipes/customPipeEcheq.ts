import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'customPipeEcheq'
})
export class CustomPipeEcheq implements PipeTransform {
    transform(values: string): string {
        if (values.length >= 7) {
            const arr = values.split('')
            arr.splice(8,0,'-')
            return arr.join('')
        }
        return values;
    }
}