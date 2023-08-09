import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbAlert } from '@ng-bootstrap/ng-bootstrap';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ConfirmationService } from 'primeng/api';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { CircularDto } from '../../../modelos/circular-model';
import { PeticionDeOfertaDto } from '../../../modelos/peticion-de-oferta-model';
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
    Observacion: any;
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
    @Output() grabarRevisionTecnicaEmitter = new EventEmitter();

    @ViewChild('panelHoras') panelHoras: PanelHorasComponent;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    }
    ngOnChanges(changes: SimpleChanges): void {

    }

    ngOnInit() {
        // console.log("init revision tecnica");
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
        // console.log("peticion", this.peticion);
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
        // if(this.Observacion == "" || this.Observacion == undefined){
        //     this.error = "El campo Observacion es obligatorio";
        //    return this.visualizarAlert = true;
        // }       
    }

    descargarArchivo(archivoId: number) {
        this.descargarArchivoEmitter.next({ archivoId: archivoId });
    }

    descargarAdjuntosCotizacion(cotizacionId: number) {
        this.descargarAdjuntosCotizacionEmitter.next({ cotizacionId: cotizacionId });
    }

    enviar() {
        this.grabarRevisionTecnicaEmitter.next();
    }

    mostrarPanelHs(p){
        if(!p.MostrarPanel){
          p.MostrarPanel = true;
        } else {
          p.MostrarPanel = false;
    
        } 
      }
}
