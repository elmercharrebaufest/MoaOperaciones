export class ArchivoDescarga {
    public Nombre: string;
    public Datos: Uint8Array;
    public blob: Blob;
    public icon: string;

    constructor(nombre: string, datos: Iterable<number>) {
        this.Nombre = nombre;
        this.Datos = new Uint8Array(datos);
        this.obtenerUrl();

        let dataType = 'image/jpeg';
        this.icon = "fa-file-image-o"

        if (this.Nombre.includes(".pdf")) {
            dataType = 'application/pdf';
            this.icon = "fa-file-pdf-o"
        }

        if (this.Nombre.includes(".zip")) {
            dataType = 'application/zip';
            this.icon = "fa-file-archive-o"
        }

        this.blob = new Blob([this.Datos], { type: dataType })
    }

    obtenerUrl() {
        console.log("Se ejecuta obtenerURL")
        let dataType = 'image/jpeg';

        if (this.Nombre.includes(".pdf")) {
            dataType = 'application/pdf';
        }

        if (this.Nombre.includes(".zip")) {
            dataType = 'application/zip';
        }

        this.blob = new Blob([this.Datos], { type: dataType })
    }
}
