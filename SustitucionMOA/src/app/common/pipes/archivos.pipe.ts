import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'archivosFilterPipe',
    pure: false
})
export class ArchivoPipe implements PipeTransform {
    transform(archivos: any[], fileKey: string): any {
        if (!archivos || !fileKey) {
            return archivos;
        }
        return archivos.filter(x => x.FileKey == fileKey);
    }
}