import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-file-modal',
  templateUrl: './file-modal.component.html',
  styleUrls: ['./file-modal.component.css']
})
export class FileModalComponent {
  @Input() files: { name: string, url: string }[] = [];
  showModal: boolean = false;

  openModal() {
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  downloadFile(file: { name: string, url: string }) {
    const link = document.createElement('a');
    link.href = file.url;
    link.download = file.name;
    link.click();
  }
}
