import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'customFilterContain'
})
export class CustomFilterContain implements PipeTransform {
    transform(values: any[], field: string, innerField: string, filterValue: string): any {
        if (!values || !values.length) return [];
        if (!filterValue) return values;

        var filtered = [];
        for (var i = 0; i < values.length; i++) {
            for (var j = 0; j < values[i][field].length; j++) {
                if (values[i][field][j][innerField].toUpperCase().indexOf(filterValue.toUpperCase()) >= 0) {
                    filtered.push(values[i]);
                    break;
                }
            }
        }

        return filtered;
    }
}