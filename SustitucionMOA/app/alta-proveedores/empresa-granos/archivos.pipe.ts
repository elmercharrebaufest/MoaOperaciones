import { Pipe, PipeTransform } from '@angular/core';
import { Archivo } from '../../common/models/archivo';

@Pipe({
    name: 'archivosFilterPipe',
    pure: false
})
export class ArchivoPipe implements PipeTransform {
    transform(archivos: any[], fileKey: string): any {
        if (!archivos || !fileKey) {
            return archivos;
        }
        console.log("Filtrando por filekey:", fileKey)
        var lista = archivos.filter(x => x.FileKey == fileKey);
        console.log(lista)
        return archivos.filter(x => x.FileKey == fileKey);
    }
}