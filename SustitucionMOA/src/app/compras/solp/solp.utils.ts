import { WeekDay } from '@angular/common';
import _ from 'lodash';
import { Paso } from '../../common/models/paso';
import { EnumPasoSolp } from '../enum-paso-solp';

export enum ComponentMode {
    Edition = 1,
    Loading = 2,
    View = 3,
    Creation = 4
}

export function setupSolpPasos(): { solp: Paso[], pliegoMultiple: Paso[] } {
    const solpregular = [{
        Codigo: EnumPasoSolp.PliegoGeneracion1,
        Nombre: 'Generación',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Preview: false,
        Numero: 1,
        Deshabilitado: false
    },
    {
        Codigo: EnumPasoSolp.PliegoGeneracion2,
        Nombre: 'Generación',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Preview: false,
        Numero: 2,
        Deshabilitado: false
    },
    {
        Codigo: EnumPasoSolp.PliegoEspecificacion,
        Nombre: 'Especificaciones técnicas',
        Activo: false,
        Completo: true,
        Iniciado: false,
        Preview: true,
        Numero: 3,
        Deshabilitado: false
    },
    {
        Codigo: EnumPasoSolp.PliegoCotizacion,
        Nombre: 'Cotización y plazo de ejecución',
        Activo: false,
        Completo: true,
        Iniciado: false, // este hace verde
        Preview: true,
        Numero: 4,
        Deshabilitado: false
    },
    {
        Codigo: EnumPasoSolp.SolpCabecera,
        Nombre: 'Posiciones',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Preview: true,
        Numero: 5,
        Deshabilitado: false
    }
        // ,
        // {
        //     Codigo: EnumPasoSolp.SolpSubposiciones,
        //     Nombre: 'Servicios',
        //     Activo: false,
        //     Completo: false,
        //     Iniciado: false,
        //     Preview: true,
        //     Numero: 6,
        //     Deshabilitado: false
        // }
    ];

    // para pliego múltiple, el paso 5 tiene otro nombre. El resto es igual.
    const pliegoMultiple = _.cloneDeep(solpregular);
    pliegoMultiple[4].Nombre = 'Vincular Solp';
    pliegoMultiple[4].Codigo = EnumPasoSolp.PliegoMultipleVincularSolp;

    return { solp: solpregular, pliegoMultiple: pliegoMultiple };
}

export function setupJornadaLaboralDias() {
    return [
        {
            weekDay: WeekDay.Monday,
            selected: true
        },
        {
            weekDay: WeekDay.Tuesday,
            selected: true
        },
        {
            weekDay: WeekDay.Wednesday,
            selected: true
        },
        {
            weekDay: WeekDay.Thursday,
            selected: true
        },
        {
            weekDay: WeekDay.Friday,
            selected: true
        },
        {
            weekDay: WeekDay.Saturday,
            selected: false
        },
        {
            weekDay: WeekDay.Sunday,
            selected: false
        }
    ];
}

export function setupDaysAndMonths() {
    return {
        firstDayOfWeek: 0,
        dayNames: ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"],
        dayNamesShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
        dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
        monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
        monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
        today: 'Today',
        clear: 'Clear'
    };
}