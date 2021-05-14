import { Directive, Output, EventEmitter, HostListener } from '@angular/core';

@Directive({
  selector: '[appDragAndDrop]'
})
export class DragAndDropDirective {

  @Output() onFileDropped = new EventEmitter<any>();

  constructor() { }


  //Dragover listener, when something is dragged over our host element
  @HostListener('dragover', ["$event"])
  public onDragOver(evt) {
    evt.preventDefault();
    evt.stopPropagation();
  }

  //Dragleave listener, when something is dragged away from our host element
  @HostListener('dragleave', ['$event'])
  public onDragLeave(evt) {
    evt.preventDefault();
    evt.stopPropagation();
  }

  @HostListener("drop", ["$event"]) public ondrop(evt) {
    evt.preventDefault();
    evt.stopPropagation();
    var main = this;

    let files = evt.dataTransfer.files;
    if (files.length > 0) {
      const reader = new FileReader();
      reader.readAsDataURL(files[0]);
      reader.onload = (function (theFile) {
        return function (e) {
            main.onFileDropped.emit(e.target.result);
        };
      })(files[0]);
    }
  }

}
