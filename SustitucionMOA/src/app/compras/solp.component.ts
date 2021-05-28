import { animate, state, style, transition, trigger } from '@angular/animations';
import { WeekDay } from '@angular/common';
import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../common/base-components/base-component';
import { Paso } from '../common/models/paso';
import { MensajeComponent } from '../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../common/view-child/spinner/spinner.component';
import { Solp } from './Solp';
import * as uuid from 'uuid';
import { SelectItem } from 'primeng/api';
import { CampoObligatorioViewModel } from './campo-obligatorio-viewModel';
import { FacturaComponent } from '../factura/factura.component';
import { EnumPasoSolp } from './enum-paso-solp';



@Component({
    selector: 'app-solp',
    templateUrl: './solp.component.html',
    styleUrls: ['./compras.component.css'],
    animations: [
        trigger('fadeInOut', [
            transition(
                ':enter',
                [
                    style({ opacity: 0 }),
                    animate('1s ease-out',
                        style({ opacity: 1 }))
                ]
            ),
            transition(
                ':leave',
                [
                    style({ opacity: 1 }),
                    animate('0s ease-in',
                        style({ opacity: 0 }))
                ]
            )
        ]),
    ]
})
export class SolpComponent extends BaseComponent implements OnInit {

    // @HostListener('window:beforeunload', [ '$event' ])
    // handleClose($event) {
    //     // if(!this.cambiosGuardados)
    //     //     $event.returnValue = false;

    //     // if(!this.cambiosGuardados)
    //     //     return false;

    //     // return true;
    // }

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    cambiosGuardados: boolean = false;
    mostrarPreview: boolean = false;
    solpActual: Solp = new Solp();
    _pasoActual: Paso;
    es: any;

    enumSolp: typeof EnumPasoSolp = EnumPasoSolp;

    set pasoActual(value: Paso) {
        this.actualizarPasoCompleto(this._pasoActual);
        this._pasoActual = value;
    }

    get pasoActual(): Paso {
        return this._pasoActual;
    }

    pasos: Paso[] = [{
        Codigo: EnumPasoSolp.PliegoGeneracion1,
        Nombre: 'Generación',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Preview: false,
        Numero: 1
    },
    {
        Codigo: EnumPasoSolp.PliegoGeneracion2,
        Nombre: 'Generación',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Preview: false,
        Numero: 2
    },
    {
        Codigo: EnumPasoSolp.PliegoEspecificacion,
        Nombre: 'Especificaciones técnicas',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Preview: true,
        Numero: 3
    },
    {
        Codigo: EnumPasoSolp.PliegoCotizacion,
        Nombre: 'Cotización y plazo de ejecución',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Preview: true,
        Numero: 4
    },
    {
        Codigo: EnumPasoSolp.SolpCabecera,
        Nombre: 'Cabecera',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Preview: true,
        Numero: 5
    },
    {
        Codigo: EnumPasoSolp.SolpSubposiciones,
        Nombre: 'Subposiciones',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Preview: true,
        Numero: 6
    }];

    ngOnInit() {
        if (this.pasos && this.pasos.length > 0) {
            this.pasos[0].Activo = true;
            this.pasos[0].Iniciado = true;

            this.pasoActual = this.pasos[0];

            this.es = {
                firstDayOfWeek: 0,
                dayNames: ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"],
                dayNamesShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
                dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
                today: 'Today',
                clear: 'Clear'
            };

            this.solpActual.jornadaLaboralDias = [
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

            this.solpActual.fechaEntrega = new Date();
            this.solpActual.horaEntrega = new Date(1, 1, 1, 10, 0, 0, 0);
            this.solpActual.fechaLimiteFecha = new Date();
            this.solpActual.fechaLimiteHora = new Date(1, 1, 1, 10, 0, 0, 0);
            this.solpActual.visitaDeObraFecha = new Date();
            this.solpActual.visitaDeObraHora = new Date(1, 1, 1, 10, 0, 0, 0);
            this.solpActual.listaVisitas = [
                {
                    id: uuid.v4(),
                    visitaDeObraFecha: new Date(),
                    visitaDeObraHora: new Date(1, 1, 1, 10, 0, 0, 0)
                }];

            this.solpActual.comienzoJornadaLaboral = new Date(1, 1, 1, 7, 0, 0, 0);
            this.solpActual.terminoJornadaLaboral = new Date(1, 1, 1, 16, 0, 0, 0);
            this.solpActual.ejecucion = "30";
            this.solpActual.observacionesCotizacion = "Indicar la cantidad de dias con que se cuenta a partir de tener el equipo disponible, en una parada programada u que el trabajo depende de otros";

        }
    }
    // grupoCompras: any[];



    cambioPaso(paso) {
        this.pasos.forEach((p, i) => {
            if (p.Codigo == this.pasoActual.Codigo) {
                p.Activo = false;
                p.Iniciado = true;
                //p.Completo = true;
            } else if (p.Codigo == paso.Codigo) {
                p.Iniciado = true;
                p.Activo = true;
            }
        });

        this.pasoActual = paso;
    }

    pasoAnterior() {
        if (this.pasoActual.Numero == 1) {
            return { paso: null, descripcion: 'VOLVER' };
        } else {
            var prev = this.pasos.find(x => x.Numero == this.pasoActual.Numero - 1);

            return { paso: prev, descripcion: 'PASO ' + prev.Numero }
        }
    }

    pasoSiguiente() {
        if (this.pasoActual.Numero == this.pasos[this.pasos.length - 1].Numero) {
            return { paso: null, descripcion: 'FINALIZAR' };
        } else {
            var next = this.pasos.find(x => x.Numero == this.pasoActual.Numero + 1);

            return { paso: next, descripcion: 'PASO ' + next.Numero }
        }
    }

    navegar(paso) {
        if (paso.paso) {
            this.pasoActual = paso.paso
        } else if (paso.descripcion == 'VOLVER') {
            this.navService.navegarSeccion('/compras');
        } else if (paso.descripcion == 'FINALIZAR') {
            //guardar - finalizar
        }
    }

    salir() {
        this.navService.navegarSeccion('/compras');
    }

    guardarCambios() {
        this.cambiosGuardados = true;
    }

    finalizar() {

    }

    actualizarPasoCompleto(paso: Paso) {
        if (paso) {
            switch (paso.Codigo) {
                case EnumPasoSolp.PliegoGeneracion1:
                    paso.Completo = this.listaStringCompleta([
                        this.solpActual.nombreDeObra,
                        this.solpActual.fiscalContrato,
                        this.solpActual.mail,
                        this.solpActual.fechaEntrega,
                        this.solpActual.horaEntrega
                    ]);
                    break;
                case EnumPasoSolp.PliegoGeneracion2:
                    paso.Completo = this.listaStringCompleta([
                        this.solpActual.supervisorSector,
                        this.solpActual.supervisorTrabajo
                    ]);
                    break;
                case EnumPasoSolp.PliegoEspecificacion:
                    paso.Completo = this.listaStringCompleta([
                        this.solpActual.especificacionesViewModel.observaciones
                    ]);
                    break;
                case EnumPasoSolp.PliegoCotizacion:
                    paso.Completo = this.listaStringCompleta([
                        this.solpActual.ejecucion,
                        this.solpActual.jornadaLaboralDias,
                        this.solpActual.comienzoJornadaLaboral,
                        this.solpActual.terminoJornadaLaboral
                    ]);
                    break;
                case EnumPasoSolp.SolpCabecera:
                    paso.Completo = this.listaStringCompleta([
                        this.solpActual.selectClaseDocumento,
                        // this.solpActual.servicio,
                        // this.solpActual.textoGenerico,
                        // this.solpActual.fechaEntregaServicio,
                        // this.solpActual.fechaDeLiberacion,
                        // this.solpActual.centroEntrega,
                        // this.solpActual.almacenEntrega,
                        // this.solpActual.calleEntrega,
                        // this.solpActual.numeroEntrega,
                        // this.solpActual.grupoCompras,
                        // this.solpActual.articuloCompras,
                        // this.solpActual.monedaCompras
                    ]);
                    break;
                case EnumPasoSolp.SolpSubposiciones:
                    break;
            }
        }
    }

    listaStringCompleta(lista: any[]) {
        return lista.filter(x => !x || x.length == 0).length == 0;
    }

    scrollTo(el: HTMLElement){
        el.scrollIntoView();
    }
    //evento que se activa cuando se deja uno de los paso de la solp
    //infomacionSolp : { codigo : string , esPasoInvalido : bool}
    actualizarEstadoSolp(infomacionSolp: any) {
        var pasoSolp = this.pasos.find(x => x.Codigo == infomacionSolp.codigo);
        pasoSolp.Completo = !infomacionSolp.esPasoInvalido;
    }

    agregarPosicion(el: HTMLElement){
        this.solpActual.agregarNuevaPosicion();
        el.scrollIntoView();
        
    }



}

