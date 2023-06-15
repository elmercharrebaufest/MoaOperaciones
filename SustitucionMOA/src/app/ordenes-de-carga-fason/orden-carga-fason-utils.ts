export function sumarDias(fecha, dias) {
    fecha.setDate(fecha.getDate() + dias);
    return fecha;
}

export function setupDaysAndMonths() {
    return {
        firstDayOfWeek: 0,
        dayNames: ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"],
        dayNamesShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
        dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
        monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
        monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
        today: 'Hoy',
        clear: 'Borrar'
    };
}

export interface DestinoFason {
    CentroDescripcion: string;
    CentroId: number;
    ClienteDescripcion: string;
    ClienteId: number;
    Id: number;
    KmARecorrer: string;
    LocalidadDescripcion: string;
    LocalidadId: number;
    ProvinciaDescripcion: string;
    ProvinciaId: number
}