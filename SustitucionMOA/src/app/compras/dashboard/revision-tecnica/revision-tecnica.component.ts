import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ConfirmationService } from 'primeng/api';
import { isNullOrUndefined } from 'util';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { CircularDto } from '../../../modelos/circular-model';
import { PeticionDeOfertaDto, PeticionDeOfertaRevisionTecnicaDto, PeticionDeOfertaUsarioDto } from '../../../modelos/peticion-de-oferta-model';
import { ComprasService } from '../../compras.service';
import { PanelHorasComponent } from '../../panel-horas/panel-horas.component';

@Component({
    selector: 'app-revision-tecnica',
    templateUrl: './revision-tecnica.component.html',
    styleUrls: ['./revision-tecnica.component.css']
})
export class RevisionTecnicaComponent implements OnInit, OnChanges {

    @Input() displayRevisionTecnica: boolean;
    @Input() public peticion: PeticionDeOfertaDto;

    fechaDeEntrega: Date
    nroPeticion: any;
    displayOkRevision: boolean;
    error: string = "";
    visualizarAlert = false;
    @BlockUI() blockUI: NgBlockUI;
    @Output() cerrardisplayRevisionTecnicaEmitter = new EventEmitter();
    @Output() onCloseModalEmitter = new EventEmitter();
    @Output() descargarArchivoEmitter = new EventEmitter<{ archivoId: number }>();
    @Output() descargarAdjuntosCotizacionEmitter = new EventEmitter<{ cotizacionId: number }>();

    @ViewChild('panelHoras') panelHoras: PanelHorasComponent;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    }

    ngOnChanges(changes: SimpleChanges): void {
        this.iniciarDatosPeticion();
    }

    ngOnInit() {
        this.iniciarDatosPeticion();
    }

    autocompletarFechaDeEntrega() {
        if (this.peticion != null && this.peticion.FechaEntregaFormateado != "") {
            const date = new Date(this.peticion.FechaEntregaFormateado);
            this.fechaDeEntrega = date;
        }
    }

    onCloseModal() {
        this.onCloseModalEmitter.next();
        this.visualizarAlert = false;
        this.iniciarDatosPeticion();
    }

    iniciarDatosPeticion() {
        if (this.peticion == null || this.peticion == undefined) {
            this.peticion = {
                Id: null,
                FechaEntregaFormateado: null,
                PlazoDeOferta: null,
                CUIT: null,
                Mail: null,
                Usuarios: new Array(),
                SolpDto: null,
                Selected: null,
            } as unknown as PeticionDeOfertaDto
        } else {
            if (this.peticion.RevisionTecnica == null || this.peticion.RevisionTecnica == undefined) {
                this.peticion.RevisionTecnica = {
                    Id: 0,
                    Usuario_Id: 0,
                    RecotizacionEconomica: false,
                    ModificacionSolp: false,
                    ObservacionRecotizacion: "",
                    Finalizada: false
                };
            }
        }
    }

    salirRT() {
        this.visualizarAlert = false;
        this.iniciarDatosPeticion();
        this.cerrardisplayRevisionTecnicaEmitter.next();
        this.displayOkRevision = false;
    }

    validarPeticion() {
        this.visualizarAlert = false;
        if (this.peticion.Usuarios.find(x =>
            (x.ObservacionNoCumple == "" || isNullOrUndefined(x.ObservacionNoCumple))
            && x.PropuestaTecnicaAprobada == false)) {
            this.error = "El campo Observaciones es obligatorio";
            this.visualizarAlert = true;
            return true;
        }
    }

    validarPlazoDeOferta() {
        this.visualizarAlert = false;
        if (this.peticion.Estado != "Cerrado") {
            this.error = "No se puede finalizar la revisión técnica ya que el plazo de oferta no se encuentra vencido";
            this.visualizarAlert = true;
            return true;
        } else {
            this.error = ""; // Borra el mensaje de error si al menos uno está seleccionado
            this.visualizarAlert = false;
            return false;
        }
    }

    validarCotizacionFinalizada() {
        this.visualizarAlert = false;
        var todasCotizacionesFinalizadas: boolean = this.peticion.Usuarios.every(u =>
            u.Cotizacion && u.Cotizacion.CotizacionEstado_Id == 1
        );
        if (todasCotizacionesFinalizadas == false) {
            this.error = "No se puede finalizar la revisión técnica ya que hay cotizaciones sin finalizar";
            this.visualizarAlert = true;
            return true;
        }
    }

    checkRecotizacionEconomica() {
        const alMenosUnoSeleccionado = this.peticion.RevisionTecnica.RecotizacionEconomica !== null && this.peticion.RevisionTecnica.RecotizacionEconomica !== undefined;

        if (!alMenosUnoSeleccionado) {
            this.error = "Todos los checks deben estar seleccionados para finalizar la revisión técnica";
            this.visualizarAlert = true;
            return true;
        } else {
            if (this.peticion.RevisionTecnica.RecotizacionEconomica == false) {
                this.error = ""; // Borra el mensaje de error si al menos uno está seleccionado
                this.visualizarAlert = false;
                return false;
            } else {
                const alMenosUnCheckSeleccionado = this.peticion.RevisionTecnica.ModificacionSolp !== null && this.peticion.RevisionTecnica.ModificacionSolp !== undefined;
                if (!alMenosUnCheckSeleccionado) {
                    this.error = "Debe indicar si va a realizar modificaciones en la SOLP";
                    this.visualizarAlert = true;
                    return true;
                }

                if (this.peticion.Usuarios.find(x =>
                    (this.peticion.RevisionTecnica.ObservacionRecotizacion == "" || isNullOrUndefined(this.peticion.RevisionTecnica.ObservacionRecotizacion)))) {
                    this.error = "El campo Observación de recotizacion es obligatorio";
                    this.visualizarAlert = true;
                    return true;
                }
                
                this.error = ""; // Borra el mensaje de error si al menos uno está seleccionado
                this.visualizarAlert = false;
                return false;
            }
        }
    }

    checkVisitaTecnica() {
        if (this.peticion.TipoPosicionCodigo !== 'MATERIALES') {
            const alMenosUnoSeleccionado = this.peticion.Usuarios.every(x => x.RealizoVisita !== null && x.RealizoVisita !== undefined);

            if (this.peticion.TieneVisitaObraMasiva == true || this.peticion.TieneVisitaObraBool == true) {
                if (!alMenosUnoSeleccionado) {
                    this.error = "Todos los checks deben estar seleccionados para finalizar la revisión técnica";
                    this.visualizarAlert = true;
                    return true;
                } else {
                    this.error = ""; // Borra el mensaje de error si al menos uno está seleccionado
                    this.visualizarAlert = false;
                    return false;
                }
            }
        }
        // No realizas la validación si 'Visita Técnica' no está visible.
        return false;
    }

    checkPropuestaTecnica() {
        var faltaCheck = false;

        if (this.peticion.TipoPosicionCodigo != 'MATERIALES') {
            if (this.peticion.TieneVisitaObraMasiva == true || this.peticion.TieneVisitaObraBool == true) {
                faltaCheck = this.peticion.Usuarios.some(x => (x.PropuestaTecnicaAprobada == null || x.PropuestaTecnicaAprobada == undefined || x.RealizoVisita == null || x.RealizoVisita == undefined) && x.Cotizacion != null && x.Cotizacion.CotizacionEstado_Id == 1);
            } else {
                faltaCheck = this.peticion.Usuarios.some(x => ((x.PropuestaTecnicaAprobada == null || x.PropuestaTecnicaAprobada == undefined) && x.Cotizacion != null && x.Cotizacion.CotizacionEstado_Id == 1));
            }
        } else {
            faltaCheck = this.peticion.Usuarios.some(x => (x.PropuestaTecnicaAprobada == null || x.PropuestaTecnicaAprobada == undefined) && x.Cotizacion != null && x.Cotizacion.CotizacionEstado_Id == 1 &&
                x.Cotizacion.RespetaMateriales == false);
        }

        if (faltaCheck) {
            this.error = "Todos los checks deben estar seleccionados para finalizar la revisión técnica";
            this.visualizarAlert = true;
            return true;
        }
    }

    descargarArchivo(archivoId: number) {
        this.descargarArchivoEmitter.next({ archivoId: archivoId });
    }

    descargarAdjuntosCotizacion(cotizacionId: number) {
        this.descargarAdjuntosCotizacionEmitter.next({ cotizacionId: cotizacionId });
    }

    guardarRT() {
        this.grabarRevisionTecnica(false);
    }

    mostrarPanelHs(p) {
        if (!p.MostrarPanel) {
            p.MostrarPanel = true;
        } else {
            p.MostrarPanel = false;
        }
    }

    confirmarFinalizacion() {
        if (!this.checkVisitaTecnica()
            && !this.checkPropuestaTecnica()
            && !this.validarPeticion()
            && (!this.validarPlazoDeOferta() || !this.validarCotizacionFinalizada())
            && !this.checkRecotizacionEconomica()
        ) {
            this.confirmationService.confirm({
                key: 'finalizarRevision',
                message: 'Una vez finalizada la revisión técnica ya no podrá editarse. ¿Está seguro de que desea finalizarla?',
                accept: () => {
                    this.grabarRevisionTecnica(true);
                },
                reject: () => {
                }
            });
        }
    }

    grabarRevisionTecnica(finalizar: boolean) {
        this.blockUI.start('Grabando...');
        this.service.grabarRevisionTecnica(this.peticion.Usuarios, finalizar, this.peticion.RevisionTecnica)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.error = result.error;
                        this.visualizarAlert = true;
                    } else if (result.info != undefined) {
                        this.error = result.info;
                        this.visualizarAlert = true;
                    } else {
                        this.nroPeticion = result.data.IdEntidad;
                        this.displayOkRevision = finalizar;
                        if (!finalizar) this.onCloseModalEmitter.next();
                    }
                    this.blockUI.stop();
                },
                (error) => {
                    this.error = error.message;
                    this.visualizarAlert = true;
                    this.blockUI.stop();
                }
            )
    }

    revisionSinFinalizar() {
        return this.peticion.RevisionFinalizada != true;
    }
}
