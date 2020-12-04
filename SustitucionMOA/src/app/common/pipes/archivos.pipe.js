var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { Pipe } from '@angular/core';
var ArchivoPipe = /** @class */ (function () {
    function ArchivoPipe() {
    }
    ArchivoPipe.prototype.transform = function (archivos, fileKey) {
        if (!archivos || !fileKey) {
            return archivos;
        }
        return archivos.filter(function (x) { return x.FileKey == fileKey; });
    };
    ArchivoPipe = __decorate([
        Pipe({
            name: 'archivosFilterPipe',
            pure: false
        })
    ], ArchivoPipe);
    return ArchivoPipe;
}());
export { ArchivoPipe };
//# sourceMappingURL=archivos.pipe.js.map