import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'customFilterEstados'
})
export class CustomFilterEstados implements PipeTransform {
    transform(values: any[], field: string, stateFilters: string[] | null): any {
        if (!values || !values.length) return [];
        if (!stateFilters || !stateFilters.length) return values;

        return values.filter(v => stateFilters.find(
            filter => v[field] == filter
        ))
    }
}