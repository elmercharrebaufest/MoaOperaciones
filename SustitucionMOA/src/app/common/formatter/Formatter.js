var Formatter = /** @class */ (function () {
    function Formatter() {
    }
    Formatter.DateToSting = function (fecha) {
        return fecha.toISOString().slice(0, 10);
    };
    Formatter.parseFecha = function (fechaCompletaRaw) {
        if (fechaCompletaRaw != undefined) {
            var fecha, hora;
            var fechaCompletaArray = fechaCompletaRaw.split(" ");
            if (fechaCompletaArray.length > 1) {
                var fechaArray = fechaCompletaArray[0].split("-");
                if (fechaArray.length > 2) {
                    fecha = fechaArray[2] + "/" + fechaArray[1] + "/" + fechaArray[0];
                }
                else {
                    return fechaCompletaRaw;
                }
                var horaArray = fechaCompletaArray[1].split(":");
                if (horaArray.length > 1) {
                    hora = horaArray[0] + ":" + horaArray[1];
                }
                else {
                    return fechaCompletaRaw;
                }
            }
            return fecha + " " + hora;
        }
        return fechaCompletaRaw;
    };
    Formatter.parseHora = function (horaCompletaRaw) {
        if (horaCompletaRaw != undefined) {
            var hora;
            var horaCompletaArray = horaCompletaRaw.split(" ");
            if (horaCompletaArray.length > 1) {
                var horaArray = horaCompletaArray[1].split(":");
                if (horaArray.length > 1) {
                    hora = horaArray[0] + ":" + horaArray[1];
                }
                else {
                    return horaCompletaRaw;
                }
            }
            else {
                return horaCompletaRaw;
            }
            return hora;
        }
        return horaCompletaRaw;
    };
    return Formatter;
}());
export { Formatter };
//# sourceMappingURL=Formatter.js.map