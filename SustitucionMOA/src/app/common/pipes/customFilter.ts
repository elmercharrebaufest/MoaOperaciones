import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'customFilter'
})
export class CustomFilter implements PipeTransform {
    transform(values: any[], field: string, filter: string): any {
        if (!values || !values.length) return [];
        if (!filter) return values;

        //Filtro compuesto
        if (filter.indexOf('|') > 0) {
            let filtros = filter.split('|');
            return values.filter(v => filtros.some(f => v[field].toString().toUpperCase().indexOf(f.toUpperCase()) >= 0));
        }


        //Filtro simple
        return values.filter(v => v[field].toString().toUpperCase().indexOf(filter.toUpperCase()) >= 0);
    }
}