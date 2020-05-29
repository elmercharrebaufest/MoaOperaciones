import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'customFilter'
})
export class CustomFilter implements PipeTransform {
    transform(values: any[], field: string, filter: string): any {
        if (!values || !values.length) return [];
        if (!filter) return values;

        return values.filter(v => v[field].toUpperCase().indexOf(filter.toUpperCase()) >= 0);
    }
}