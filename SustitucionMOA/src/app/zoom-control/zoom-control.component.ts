import { Component } from '@angular/core';

@Component({
    selector: 'app-zoom-control',
    templateUrl: './zoom-control.component.html',
    styleUrls: ['./zoom-control.component.css']
})
export class ZoomControlComponent {
    zoomLevel: number = 80; // Nivel de zoom inicial en porcentaje

    // Método para aumentar el zoom
    increaseZoom() {
        if (this.zoomLevel < 150) { // Limitar a un máximo del 150%
            this.zoomLevel += 10;
            this.applyZoom();
        }
    }

    // Método para disminuir el zoom
    decreaseZoom() {
        if (this.zoomLevel > 50) { // Limitar a un mínimo del 50%
            this.zoomLevel -= 10;
            this.applyZoom();
        }
    }

    // Método para aplicar el zoom
    private applyZoom() {
        document.documentElement.style.zoom = `${this.zoomLevel}%`;
    }
}
