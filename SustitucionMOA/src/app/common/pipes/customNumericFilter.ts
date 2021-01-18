import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'customNumericFilter'
})
export class CustomNumericFilter implements PipeTransform {
    transform(values: any[], field: string, filter: number, defaultValue: number): any {
        if (!values || !values.length) return [];
        if (!filter) return values;
        
        if (filter == defaultValue) {
            return values;
        }
        
        return values.filter(v => v[field].toString().toUpperCase().indexOf(filter) >= 0);
    }
}