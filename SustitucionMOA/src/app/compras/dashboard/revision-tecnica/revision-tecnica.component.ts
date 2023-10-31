import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbAlert } from '@ng-bootstrap/ng-bootstrap';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ConfirmationService } from 'primeng/api';
import { isNullOrUndefined } from 'util';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { CircularDto } from '../../../modelos/circular-model';
import { PeticionDeOfertaDto, PeticionDeOfertaUsarioDto } from '../../../modelos/peticion-de-oferta-model';
import { ComprasService } from '../../compras.service';
import { PanelHorasComponent } from '../../panel-horas/panel-horas.component';

@Component({
    selector: 'app-revision-tecnica',
    templateUrl: './revision-tecnica.component.html',
    styleUrls: ['./revision-tecnica.component.css']
})
export class RevisionTecnicaComponent implements OnInit, OnChanges {


    @Input()
    displayRevisionTecnica: boolean;

    @Input()
    public peticion: PeticionDeOfertaDto;
    fechaDeEntrega: Date
    plazoDeOferta: Date
    public circular: CircularDto;
    @Output() cerrardisplayRevisionTecnicaEmitter = new EventEmitter();
    val1: string = "No";
    val2: string;
    ObservacionNoCumple: any;
    visualizarFechas: boolean;
    plazoDias: string;
    selectedProv: number[] = []
    nroPeticion: any;
    displayOkPeticion: boolean;
    subscription: any;
    @BlockUI() blockUI: NgBlockUI;
    error: string = "";
    visualizarAlert = false;
    @Output() descargarArchivoEmitter = new EventEmitter<{ archivoId: number }>();
    @Output() descargarAdjuntosCotizacionEmitter = new EventEmitter<{ cotizacionId: number }>();
    @Output() grabarRevisionTecnicaEmitter = new EventEmitter<{ finalizar: boolean }>();

    @ViewChild('panelHoras') panelHoras: PanelHorasComponent;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    }
    ngOnChanges(changes: SimpleChanges): void {

    }

    ngOnInit() {
        if (this.peticion == null) {
            this.peticion = {
                Id: null,
                FechaEntregaFormateado: null,
                PlazoDeOferta: null,
                CUIT: null,
                Mail: null,
                Usuarios: new Array(),
                SolpDto: null,
                Selected: null
            };
        }
    }

    autocompletarFechaDeEntrega() {
        if (this.peticion != null && this.peticion.FechaEntregaFormateado != "") {
            const date = new Date(this.peticion.FechaEntregaFormateado);
            this.fechaDeEntrega = date;
        }
    }

    onCerrarPeticion() {
        this.visualizarAlert = false;
        this.iniciarModalPeticion();
        this.cerrardisplayRevisionTecnicaEmitter.next();
    }

    estaSeleccionado(seleccion) {

    }

    private armarPeticion() {

    }


    iniciarModalPeticion() {

    }

    salir() {
        this.onCerrarPeticion();
        this.displayOkPeticion = false;
    }

    validarPeticion() {
        this.visualizarAlert = false;
        if (this.peticion.Usuarios.find(x =>
            (x.ObservacionNoCumple == "" || isNullOrUndefined(x.ObservacionNoCumple))
            && x.PropuestaTecnicaAprobada == false)) {
            this.error = "El campo Observacion es obligatorio";
            this.visualizarAlert = true;
            return true;
        }
    }

    checkVisitaTecnica() {
        if (this.peticion.TipoPosicionCodigo !== 'MATERIALES') {
            const alMenosUnoSeleccionado = this.peticion.Usuarios.every(x => x.RealizoVisita !== null && x.RealizoVisita !== undefined);

            if (this.peticion.TieneVisitaObraMasiva == true || this.peticion.TieneVisitaObraBool == true) {
                if (!alMenosUnoSeleccionado) {
                    this.error = "Todos los checks deben estar seleccionados para finalizar la revision tecnica";
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
                x.Cotizacion.RespetaMateriales == false );
        }

        if (faltaCheck) {
            this.error = "Todos los checks deben estar seleccionados para finalizar la revision tecnica";
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
        this.grabarRevisionTecnicaEmitter.next({ finalizar: false });
    }

    mostrarPanelHs(p) {
        if (!p.MostrarPanel) {
            p.MostrarPanel = true;
        } else {
            p.MostrarPanel = false;

        }
    }

    confirmarFinalizacion() {
        if (!this.checkVisitaTecnica() && !this.checkPropuestaTecnica() && !this.validarPeticion()) {
            this.confirmationService.confirm({
                key: 'finalizarRevision',
                message: 'Una vez finalizada la revisión técnica ya no podrá editarse. Esta seguro que deseea cerrar la revisión tecnica?',
                accept: () => {

                    this.grabarRevisionTecnicaEmitter.next({ finalizar: true });
                },
                reject: () => {
                }
            });

        }
    }
}
