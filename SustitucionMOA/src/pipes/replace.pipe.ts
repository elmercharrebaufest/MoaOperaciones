import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'replace'
})
export class ReplacePipe implements PipeTransform {
    transform(value: string, searchValue: string | RegExp, replaceValue: string): string {
        console.log('replace pipe');
        return value.replace(searchValue, replaceValue);
    }
}
