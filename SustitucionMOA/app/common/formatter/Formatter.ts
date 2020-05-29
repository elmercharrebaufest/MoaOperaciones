export class Formatter {

    static DateToSting(fecha: Date): string {
        return fecha.toISOString().slice(0, 10);
    }

    static parseFecha(fechaCompletaRaw: string) {
        if (fechaCompletaRaw != undefined) {
            var fecha, hora;
            var fechaCompletaArray = fechaCompletaRaw.split(" ");
            if (fechaCompletaArray.length > 1) {
                var fechaArray = fechaCompletaArray[0].split("-");
                if (fechaArray.length > 2) {
                    fecha = fechaArray[2] + "/" + fechaArray[1] + "/" + fechaArray[0];
                } else {
                    return fechaCompletaRaw;
                }
                var horaArray = fechaCompletaArray[1].split(":");
                if (horaArray.length > 1) {
                    hora = horaArray[0] + ":" + horaArray[1];
                } else {
                    return fechaCompletaRaw;
                }
            }

            return fecha + " " + hora;
        }
        return fechaCompletaRaw;
    }

    static parseHora(horaCompletaRaw: string) {
        if (horaCompletaRaw != undefined) {
            var hora;
            var horaCompletaArray = horaCompletaRaw.split(" ");
            if (horaCompletaArray.length > 1) {
                var horaArray = horaCompletaArray[1].split(":");
                if (horaArray.length > 1) {
                    hora = horaArray[0] + ":" + horaArray[1];
                } else {
                    return horaCompletaRaw;
                }
            } else {
                return horaCompletaRaw;
            }

            return hora;
        }
        return horaCompletaRaw;
    }

}