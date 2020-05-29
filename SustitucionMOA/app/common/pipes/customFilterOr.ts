import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'customFilterOr'
})
export class CustomFilterOr implements PipeTransform {
    transform(values: any[], fieldOne: string, fieldTwo: string, filter: string): any {
        if (!values || !values.length) return [];
        if (!filter) return values;

        return values.filter(v => v[fieldOne].toUpperCase().indexOf(filter.toUpperCase()) >= 0 || v[fieldTwo].toUpperCase().indexOf(filter.toUpperCase()) >= 0);
    }
}