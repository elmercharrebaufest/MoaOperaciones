import { animate, state, style, transition, trigger } from '@angular/animations';
import { WeekDay } from '@angular/common';
import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../common/base-components/base-component';
import { Paso } from '../common/models/paso';
import { MensajeComponent } from '../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../common/view-child/spinner/spinner.component';
import { PosicionSolp, Solp } from './Solp';
import * as uuid from 'uuid';
import { ComprasService } from './compras.service';
import { NavService } from '../common/services/NavService';
import { SessionDataService } from '../common/services/SessionDataService';
import { SecurityService } from '../common/services/SecurityService';
import { FloatMsgService } from '../common/services/FloatMsgService';
import { ModalService } from '../common/services/ModalService';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ConfirmationService, Message } from 'primeng/components/common/api';
import { MessageService } from 'primeng/components/common/messageservice';
import { SelectItem } from 'primeng/api';
import { CampoObligatorioViewModel } from './campo-obligatorio-viewModel';
import { FacturaComponent } from '../factura/factura.component';
import { EnumPasoSolp } from './enum-paso-solp';
import { CabeceraComponent } from './SolpPasos/cabecera.component';
import { ActivatedRoute, Params } from '@angular/router';
import { EspecificacionesViewModel } from './PliegoPasos/solapaTres/especificacionesViewModel';
import { SubPosicionViewModel } from './PliegoPasos/solapaSubposiciones/subPosicionViewModel';
import { DashboardComponent } from './dashboard/dashboard.component';
import { DialogModule } from 'primeng/dialog';
import { FormGroup } from '@angular/forms';

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
    ],
    providers: [ComprasService, MessageService]
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

    @BlockUI() blockUI: NgBlockUI;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild(CabeceraComponent)
    protected cabecera: CabeceraComponent;

    @ViewChild(DashboardComponent)
    protected dashboard: DashboardComponent;

    cambiosGuardados: boolean = false;
    mostrarPreview: boolean = false;
    pdfPreview: any;
    urlPdf: any;
    solpActual: Solp = new Solp();
    _pasoActual: Paso;
    es: any;
    enumSolp: typeof EnumPasoSolp = EnumPasoSolp;
    combos: any;
    solpId: number = 0;
    visitasFinalizar: SelectItem[];
    finalizarOk: boolean;
    displayFinalizar: boolean = false;
    displaySAP: boolean;
    displayErrorSAP: boolean;

    listadoErrores: string[];


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
        Nombre: 'Servicios',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Preview: true,
        Numero: 6
    }];

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securytiService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        private messageService: MessageService, private route: ActivatedRoute, private confirmationService: ConfirmationService) {
        super(navService, securytiService, floatMsgService, modalService);

    }

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
            let fechaLimiteFecha = new Date();
            this.solpActual.horaEntrega = new Date(1, 1, 1, 10, 0, 0, 0);
            this.solpActual.fechaLimiteFecha = this.sumarDias(fechaLimiteFecha, 6);
            this.solpActual.fechaLimiteHora = new Date(1, 1, 1, 10, 0, 0, 0);
            this.solpActual.visitaDeObraFecha = new Date();
            this.solpActual.visitaDeObraHora = new Date(1, 1, 1, 10, 0, 0, 0);
            this.solpActual.listaVisitas = [
                {
                    id: uuid.v4(),
                    visitaDeObraFecha: this.solpActual.fechaEntrega,
                    visitaDeObraHora: new Date(1, 1, 1, 10, 0, 0, 0)
                }];

            this.solpActual.comienzoJornadaLaboral = new Date(1, 1, 1, 7, 0, 0, 0);
            this.solpActual.terminoJornadaLaboral = new Date(1, 1, 1, 16, 0, 0, 0);
            this.solpActual.ejecucion = "30";
            this.solpActual.observacionesCotizacion = "Indicar la cantidad de días con que se cuenta a partir de tener el equipo disponible, en una parada programada o que el trabajo depende de otros";

            this.solpActual.centroPorDefecto = 1029;
            this.solpActual.monedaPorDefecto = "ARP";


            this.getCombos();

            if (this.route.params) {
                this.route.params.forEach((params: Params) => {
                    if (params["id"] > 0) this.solpId = params["id"];
                    if (params["tipoSolp"]) this.solpActual.tipoSolp = params["tipoSolp"];
                });

                if (this.solpId > 0) {
                    this.traerSolpId(this.solpId);
                }
            }



        }
    }

    sumarDias(fecha, dias) {
        fecha.setDate(fecha.getDate() + dias);
        return fecha;
    }

    traerSolpId(idSolp) {
        try {
            this.blockUI.start('Cargando...');
            this.spinnerComponent.showIt();

            this.subscription = this.service.traerSolpId(idSolp).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.cargarSolpActual(result.data);
                        this.spinnerComponent.hideIt();
                        this.blockUI.stop();
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.spinnerComponent.hideIt();
                    this.blockUI.stop();
                });
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    cargarSolpActual(solp) {

        // Paso 1
        this.solpActual.id = solp.Id;
        this.solpActual.tipoSolp = solp.TipoSolp && solp.TipoSolp.Codigo || '';
        this.solpActual.nombreDePedido = solp.NombreDeObra || '';
        this.solpActual.fiscalContrato = solp.FiscalContrato || '';
        this.solpActual.telefono = solp.Telefono || '';
        this.solpActual.mail = solp.Email || '';
        // this.solpActual.fechaEntrega = new Date(this.getDateFromAspNetFormat(solp.FechaHoraEntrega));
        // this.solpActual.horaEntrega = new Date(this.getDateFromAspNetFormat(solp.FechaHoraEntrega));

        // Paso 2
        this.solpActual.supervisorSector = solp.SupervisorSector || '';
        this.solpActual.supervisorTrabajo = solp.SupervisorTrabajo || '';
        this.solpActual.listaVisitas = solp.VisitasObraMasiva.map(x => {
            return {
                id: x.Codigo,
                visitaDeObraFecha: new Date(this.getDateFromAspNetFormat(x.FechaHora)),
                visitaDeObraHora: new Date(this.getDateFromAspNetFormat(x.FechaHora))
            } || '';
        });
        this.solpActual.visitaDeObraMasiva = solp.TieneVisitaObraMasiva;
        this.solpActual.visitaDeObra = solp.TieneVisitaObra;
        this.solpActual.obradores = solp.TieneObradores;
        this.solpActual.modoElevacion = solp.TieneMedioElevacion;
        this.solpActual.andamio = solp.TieneAndamio;
        this.solpActual.tecnicoSeguridad = solp.TieneTecnicoSeguridad;
        this.solpActual.descripcionTecnica = solp.TieneDescripcionTecnica;
        this.solpActual.entregaDocumentacion = solp.TieneDocumentacionTecnica;
        this.solpActual.fechaLimiteFecha = new Date(this.getDateFromAspNetFormat(solp.FechaHoraLimiteConsulta));
        this.solpActual.fechaLimiteHora = new Date(this.getDateFromAspNetFormat(solp.FechaHoraLimiteConsulta));
        this.solpActual.observacionesGeneracion = solp.ObservacionesGeneracion;

        // Paso 3
        this.solpActual.especificacionesViewModel = new EspecificacionesViewModel();
        this.solpActual.especificacionesViewModel.archivosGuardadosEspecificaciones = solp.Adjuntos.map(x => {
            return {
                id: x.Id,
                nombreArchivo: x.Nombre
            }
        }),
            this.solpActual.especificacionesViewModel.observaciones = solp.EspecificacionesTecnicas || this.solpActual.especificacionesViewModel.valorPorDefecto;

        this.solpActual.tieneCondicionesGenerales = solp.TieneCondicionesGenerales;

        // Paso 4
        this.solpActual.jornadaLaboralDias.forEach(k => {
            k.selected = solp.JornadaLaboral.includes(k.weekDay);
        });
        this.solpActual.comienzoJornadaLaboral = new Date(this.getDateFromAspNetFormat(solp.JornadaLaboralDesde));
        this.solpActual.terminoJornadaLaboral = new Date(this.getDateFromAspNetFormat(solp.JornadaLaboralHasta));
        this.solpActual.ejecucion = solp.DiasEjecucion || '';
        this.solpActual.observacionesCotizacion = solp.ObservacionesCotizacion;

        // Paso 5
        this.solpActual.selectClaseDocumento = solp.ClaseDocumento;

        if (solp.Posiciones && solp.Posiciones.length > 0) {
            let ultimaPos = solp.Posiciones[solp.Posiciones.length - 1];

            let posActual = this.solpActual.posicionActual;
            let solpActual = this.solpActual;

            solp.Posiciones.forEach(x => {
                posActual.id = x.Codigo;
                posActual.textoGenerico = x.TextoGenerico;
                posActual.plazoDeEntrega = x.PlazoEntrega;
                posActual.fechaEntregaServicio = new Date(this.getDateFromAspNetFormat(x.FechaEntregaServicio));
                posActual.fechaDeLiberacion = new Date(this.getDateFromAspNetFormat(x.FechaLiberacion));
                posActual.concluido = x.EsConcluido;
                posActual.indiceFijacion = x.EsFijacion;
                posActual.selectCentroEntrega = x.Centro;
                posActual.selectAlmacenEntrega = x.Almacen;
                posActual.nombreEntrega = x.NombreEntrega;
                posActual.calleEntrega = x.CalleEntrega;
                posActual.numeroEntrega = x.NumeroEntrega;
                posActual.codigoPostalEntrega = x.CpEntrega;
                posActual.paisEntrega = x.PaisEntrega;
                posActual.selectSolicitanteCompras = x.Solicitante;
                posActual.necesidadCompras = x.NroNecesidad;
                posActual.selectGrupoCompras = x.GrupoCompras;
                posActual.selectArticuloCompras = x.GrupoArticulo;
                posActual.monedaSeleccionada = x.Moneda;
                posActual.servicio = x.TipoPosicion && x.TipoPosicion.Codigo;
                posActual.tipoImputacion = x.TipoImputacion && x.TipoImputacion.Codigo;
                posActual.rubroElectrico = x.CodigosProveedores.includes('ELECTRICO');
                posActual.rubroConsultoria = x.CodigosProveedores.includes('CONSULTORIA');
                posActual.rubroCivil = x.CodigosProveedores.includes('CIVIL');
                posActual.rubroIngenieria = x.CodigosProveedores.includes('INGENIERIA');
                posActual.rubroMecanico = x.CodigosProveedores.includes('MECANICO');

                posActual.proveedoresValidos = x.Proveedores.filter(p => p.TipoFiltroProveedorSolp.Codigo == 'VALIDO').map(p => p.RazonSocial);
                posActual.proveedoresNoSugeridos = x.Proveedores.filter(p => p.TipoFiltroProveedorSolp.Codigo == 'NOSUGERIDO').map(p => p.RazonSocial);
                posActual.proveedoresInvalidos = x.Proveedores.filter(p => p.TipoFiltroProveedorSolp.Codigo == 'INVALIDO').map(p => p.RazonSocial);



                if (x.Subposiciones) {
                    posActual.listadoSubPosiciones = [];
                    let i = 1;

                    x.Subposiciones.forEach(sp => {
                        let subpos = new SubPosicionViewModel(i);

                        subpos.id = sp.Codigo;
                        subpos.codigoServicio = sp.CodigoServicioSap;
                        subpos.tareaSubcontratarObj = sp.CodigoServicioSap;
                        subpos.tareaSubcontratar = sp.Tarea;
                        subpos.cuentaMayor = sp.CuentaMayor;
                        subpos.cuentaTd = sp.Cantidad;
                        subpos.unidadSeleccionada = sp.Unidad;
                        subpos.tipoImputacion = sp.TipoImputacionValor;
                        subpos.precioBruto = sp.PrecioBruto;
                        subpos.subPosicion = sp.Numero;

                        posActual.listadoSubPosiciones.push(subpos);
                        i++;
                    });
                }

                if (ultimaPos.Codigo != x.Codigo) {
                    solpActual.agregarNuevaPosicion();
                    posActual = solpActual.posicionActual;
                }
            });

            this.solpActual.setearPosicionPorDefecto();
        }

    }

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

    guardarCambios(mostrarPreview = false, enviarSap = false, guardarPorPaso = false) {
        try {

            if (guardarPorPaso = false) {
                this.blockUI.start('Guardando...');
                this.spinnerComponent.showIt();
            }

            this.solpActual.Finalizar = enviarSap;
            this.subscription = this.service.GuardarSolp(this.solpActual).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                        if (guardarPorPaso = false) {
                            this.blockUI.stop();
                        }
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                        if (guardarPorPaso = false) {
                            this.blockUI.stop();
                        }
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                        if (guardarPorPaso = false) {
                            this.blockUI.stop();
                        }
                    } else {
                        this.spinnerComponent.hideIt();
                        if (guardarPorPaso = false) {
                            this.blockUI.stop();
                        }

                        if (!mostrarPreview && !guardarPorPaso) {
                            this.messageService.add({ severity: 'success', detail: 'Los datos se guardaron correctamente' });
                        }
                        // this.floatMsgService.setSuccessMsg("Los datos se guardaron correctamente");

                        this.solpActual.id = result.Solp.Id;
                        this.solpActual.especificacionesViewModel.archivosAdjuntosNuevos.splice(0, this.solpActual.especificacionesViewModel.archivosAdjuntosNuevos.length);
                        this.solpActual.especificacionesViewModel.archivosGuardadosEspecificaciones = result.Solp.Adjuntos.map(x => {
                            return {
                                id: x.Id,
                                nombreArchivo: x.Nombre,
                                rutaDeAcceso: ''
                            }
                        });

                        this.cambiosGuardados = true;

                        if (mostrarPreview) {
                            if (result.Solp.Pdf) {
                                this.pdfPreview = "data:application/pdf;base64," + result.Solp.Pdf;
                                this.mostrarPreview = true;
                            } else {
                                this.messageService.add({ severity: 'error', detail: 'Hubo un error al generar el preview. Por favor contacte con el administrador de sistemas.' });
                            }
                        }

                        if (enviarSap) {

                            if (result.Mensaje == "OK") {
                                this.finalizarOk = true;
                                this.displaySAP = true;
                            }
                            else {
                                this.listadoErrores = result.Errores;
                                this.displayErrorSAP = true;
                            }

                            // if (result.Solp.NroSolp) {
                            //     this.finalizarOk = true;
                            // } else {
                            //     this.messageService.add({ severity: 'error', detail: 'Hubo un error al generar la SOLP en SAP, intente de nuevo mas tarde o comuniquese con el administrador' });
                            // }
                        }


                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.spinnerComponent.hideIt();
                    if (guardarPorPaso = false) {
                        this.blockUI.stop();
                    }
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.spinnerComponent.hideIt();
            if (guardarPorPaso = false) {
                this.blockUI.stop();
            }
            return false; //<-- Prevent Refresh
        }
    }

    actualizarPasoCompleto(paso: Paso) {
        if (paso) {
            switch (paso.Codigo) {
                case EnumPasoSolp.PliegoGeneracion1:
                    paso.Completo = this.listaStringCompleta([
                        this.solpActual.nombreDePedido,
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
                    console.log(this.solpActual)
                    paso.Completo = this.listaStringCompleta([
                        //this.solpActual.selectClaseDocumento,
                        this.solpActual.posicionActual.servicio,
                        this.solpActual.posicionActual.textoGenerico,
                        this.solpActual.posicionActual.fechaEntregaServicio,
                        this.solpActual.posicionActual.fechaDeLiberacion,
                        this.solpActual.posicionActual.selectCentroEntrega,
                        this.solpActual.posicionActual.selectAlmacenEntrega,
                        this.solpActual.posicionActual.calleEntrega,
                        this.solpActual.posicionActual.numeroEntrega,
                        this.solpActual.posicionActual.selectGrupoCompras,
                        this.solpActual.posicionActual.selectArticuloCompras,
                        this.solpActual.posicionActual.monedaSeleccionada
                    ]);

                    break;
                case EnumPasoSolp.SolpSubposiciones:
                    paso.Completo = this.listaStringCompleta([
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


    //evento que se activa cuando se deja uno de los paso de la solp
    //infomacionSolp : { codigo : string , esPasoInvalido : bool}
    actualizarEstadoSolp(infomacionSolp: any) {
        var pasoSolp = this.pasos.find(x => x.Codigo == infomacionSolp.codigo);
        pasoSolp.Completo = !infomacionSolp.esPasoInvalido;
    }

    // funcion para que cuando agregues una posicion, vuelva a la altura posiciones
    scrollTo(el: HTMLElement) {
        el.scrollIntoView();
    }

    agregarPosicion(el: HTMLElement) {
        this.cabecera.validarPosicionActual();
        this.solpActual.agregarNuevaPosicion();
        el.scrollIntoView();
    }

    getCombos() {
        try {
            this.subscription = this.service.getCombos().subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.combos = result;
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    preview() {
        this.guardarCambios(true);
    }

    salir() {
        this.navService.navegarSeccion('/compras');
    }


    generarZipPliego(idSolp) {
        this.blockUI.start('Generando ')
        this.service.descargarZipPliego(idSolp)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "application/octet-stream",
                        });

                        if (window.navigator.msSaveOrOpenBlob) {
                            // IE11
                            window.navigator.msSaveOrOpenBlob(
                                blob,
                                result.FileDownloadName
                            );
                        } else {
                            var url = window.URL.createObjectURL(blob);
                            var link = document.createElement("a");
                            document.body.appendChild(link);
                            link.href = url;
                            link.download = result.FileDownloadName;
                            link.click();
                            setTimeout(function () {
                                window.URL.revokeObjectURL(url);
                            }, 0);
                            this.blockUI.stop();
                            return false;
                        }
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    // this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
    }

    // Todos los Modal
    finalizar() {
        this.guardarCambios(false, true, false);
        this.displayFinalizar = false
        // ;
        //
    }

    // Abre el modal del boton finalizar
    showDialog() {
        this.displayFinalizar = true;
    }

    cancelarFinalizar() {
        this.displayFinalizar = false;
    }

    cancelarSolp() {
        this.confirmationService.confirm({
            key: 'cancelarSolp',
            message: '¿Está seguro que desea volver a la pantalla principal? No se conservaran los cambios no guardados.',
            accept: () => {
                this.salir();
            },
            reject: () => {
            }
        });
    }

    solpFinalizadaVolverHome() {
        this.confirmationService.confirm({
            key: 'solpFinalizadaVolverHome',
            message: '¿Desea volver a la pantalla principal?',
            accept: () => {
                this.salir();
            },
            reject: () => {
            }
        });
    }

    ultimoPasoSolp() {
        this.confirmationService.confirm({
            key: 'ultimoPasoSolp',
            message: 'Está a punto de enviar a SOLP sin documento a xxxx ¿Desea continuar?',
            accept: () => {

            },
            reject: () => {
                this.salir();
            }
        });

    }


}

