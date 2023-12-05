import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'customTipoProveedorFilter'
})
export class CustomTipoProveedorFilter implements PipeTransform {
    transform(values: any[], field: string, filter: number | string, defaultValue: number): any {
        if (!values || !values.length) return [];
        if (!filter || filter === 'null') return values;

        if (filter == defaultValue) {
            return values;
        }

        if(filter == 2){
            return values.filter(v => v.IdTipoUsuario.toString().toUpperCase()==filter &&
            (v.RazonSocialCorredor == undefined || v.RazonSocialCorredor ==''));
        }else if(filter == 4){
            return values.filter(v => (v.IdTipoUsuario.toString().toUpperCase() == 2 &&
            (v.RazonSocialCorredor !='')) ||
            v[field].toString().toUpperCase() == filter);
        }
        
        return values.filter(v => v.IdTipoUsuario.toString().toUpperCase() == filter);    
    }
}