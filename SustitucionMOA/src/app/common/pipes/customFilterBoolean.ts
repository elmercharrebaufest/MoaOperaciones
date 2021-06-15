import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'customFilterBoolean'
})
export class CustomFilterBoolean implements PipeTransform {
    transform(values: any[], field: string, filter: boolean , aplicarFiltro : boolean): any {
         if (!values || !values.length) return [];
        if (filter == undefined ) return values;
        
        if (!aplicarFiltro) {
            return values;
        }
        
        return values.filter(v => v[field] == filter);
    }
}