
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import * as uuid from 'uuid';
import ImageResize from 'quill-image-resize-module';
import Quill from 'quill';
Quill.register('modules/imageResize', ImageResize);

import { ListBaseComponent } from '../../../../common/base-components/list-base-component'
import { SessionDataService } from '../../../../common/services/SessionDataService';
import { SecurityService } from '../../../../common/services/SecurityService';
import { NavService } from '../../../../common/services/NavService';
import { FloatMsgService } from '../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../common/services/ModalService';
import { ComprasService } from '../../../compras.service'
import { Solp } from '../../solp';
import { ValidadorPasoSolpService } from '../../../validadorPasoSolpService';
import { EnumPasoSolp } from '../../../enum-paso-solp';

declare var $: any;

@Component({
    selector: 'generacion2',
    templateUrl: `generacion2.component.html`,
    styleUrls: ['../../../compras.component.css'],
})
export class Generacion2Component extends ListBaseComponent {

    @Input('model')
    protected model: Solp;

    @Input('locale')
    protected locale: any;

    modulesEditor = {};

    //validaciones
    formulario2: FormGroup;

    camposObligatorios: any[] = [
        { campo: 'supervisorSector', esObligatorio: true },
        { campo: 'supervisorTrabajo', esObligatorio: true }
    ];

    @Output() onEstCompleto = new EventEmitter<any>();

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService,
        protected modalService: ModalService, protected route: ActivatedRoute,
        protected router: Router, private formBuilder: FormBuilder,
        private validadorPasoSolpService: ValidadorPasoSolpService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.modulesEditor = {
            imageResize: true
        }
    }



    solpPaso2Result: any;
    fechaEntrega: any;
    horaEntrega: any;
    visitaDeObraHora: any;
    visitaDeObraFecha: any;
    fechaLimiteHora: any;
    fechaLimiteFecha: any;
    hoy: Date = new Date();
    resultadoSupervisorSector: string[];
    resultadoSupervisorTrabajo: string[];



    //variables auxiliares de text rich
    posicionDeInicioInsert: number = 0;


    parsearFecha() {
        this.fechaEntrega = (<HTMLInputElement>document.querySelectorAll('[fechaInicioInput]')[0]).value;
        if (this.fechaEntrega != '' && this.fechaEntrega != null && this.horaEntrega != '' && this.horaEntrega != null) {
            var dateParts = this.fechaEntrega.split("-");
            this.model.fechaDeEntregaDeOfertasFecha = new Date(+dateParts[0], +dateParts[1] - 1, +dateParts[2], this.horaEntrega);
        }
    }

    agregarNuevaVisita() {
        this.model.listaVisitas.push(
            {
                id: uuid.v4(),
                visitaDeObraFecha: new Date(),
                visitaDeObraHora: new Date(1, 1, 1, 10, 0, 0, 0)
            }
        )
    };

    eliminarVisita(id) {
        this.model.listaVisitas = this.model.listaVisitas.filter(x => x.id != id);

        if (this.model.listaVisitas.length == 0) {
            this.agregarNuevaVisita();
        }

    }


    setTabs() {
        this.setMenuSeccionTab("Generacion2", "Generacion2");
    }

    ngOnInit() {
        this.setTabs();

        //declaro las validaciones para los campos
        if (this.model.tipoSolp == "SIN_PLIEGO") {
            this.formulario2 = this.formBuilder.group({
                supervisorTrabajo: new FormControl('', Validators.required),
                supervisorSector: new FormControl('', Validators.required),
                visitaDeObra: [{ value: true, disabled: true }, [Validators.required]],
                visitaDeObraMasiva: [{ value: true, disabled: true }, [Validators.required]],
                obradores: [{ value: true, disabled: true }, [Validators.required]],
                modoElevacion: [{ value: true, disabled: true }, [Validators.required]],
                andamio: [{ value: true, disabled: true }, [Validators.required]],
                tecnicoSeguridad: [{ value: true, disabled: true }, [Validators.required]],
                grillaPersonal: [{ value: true, disabled: true }, [Validators.required]],
                fabricacionTallerExterno: [{ value: true, disabled: true }, [Validators.required]],
                descripcionTecnica: [{ value: true, disabled: true }, [Validators.required]],
                entregaDocumentacion: [{ value: true, disabled: true }, [Validators.required]],
                fechaLimiteFecha: [{ value: true, disabled: true }, [Validators.required]],
                fechaLimiteHora: [{ value: true, disabled: true }, [Validators.required]]
            });
        } else {
            this.formulario2 = this.formBuilder.group({
                supervisorTrabajo: new FormControl('', Validators.required),
                supervisorSector: new FormControl('', Validators.required),
                visitaDeObra: [{ value: true, disabled: false }, []],
                visitaDeObraMasiva: [{ value: true, disabled: false }, []],
                obradores: [{ value: true, disabled: false }, []],
                modoElevacion: [{ value: true, disabled: false }, []],
                andamio: [{ value: true, disabled: false }, []],
                tecnicoSeguridad: [{ value: true, disabled: false }, []],
                grillaPersonal: [{ value: true, disabled: false }, []],
                fabricacionTallerExterno: [{ value: true, disabled: false }, []],
                descripcionTecnica: [{ value: true, disabled: false }, []],
                entregaDocumentacion: [{ value: true, disabled: false }, []],
            });
        }


        this.validadorPasoSolpService.formulario = this.formulario2;
        if (this.model.cargoPasoDos) {
            this.validadorPasoSolpService.aplicarValidaciones();
        }

        this.model.cargoPasoDos = true;

        if (this.model.supervisorSector[0] == '') {
            this.model.supervisorSector = [];
        }
        if (this.model.supervisorTrabajo[0] == '') {
            this.model.supervisorTrabajo = [];
        }
    }

    selectionChange(event) {
        if (event.range && this.model.observacionesGeneracion) {
            this.posicionDeInicioInsert = this.ObtenerPosicionInsert(event.range.index, this.model.observacionesGeneracion);
        }
    }

    fileChange(file) {
        if (this.posicionDeInicioInsert != undefined && this.model.observacionesGeneracion.length > this.posicionDeInicioInsert) {
            var textoInicial = this.model.observacionesGeneracion.substring(0, this.posicionDeInicioInsert + 1);
            var textoFinal = this.model.observacionesGeneracion.substring(this.posicionDeInicioInsert + 1, this.model.observacionesGeneracion.length);
            this.model.observacionesGeneracion = textoInicial + '<img src=' + file + '>' + textoFinal;
            this.posicionDeInicioInsert = undefined;
        }
        else {
            this.model.observacionesGeneracion = '<img src=' + file + '>';
        }
    }

    ObtenerPosicionInsert(posicion: number, texto: string) {
        let contar = false;
        for (var i = 0; i < texto.length; i++) {
            var letra = texto[i];
            if (letra == "<") {
                contar = false;
                continue
            }
            else if (letra == ">") {
                contar = true;
                continue;
            }

            if (contar) {
                posicion--;

            }

            if (posicion == 0)
                return i;
        }
    }

    mostrarError(nombreCampo: string): boolean {
        if (this.formulario2 && this.formulario2.controls) {
            return (this.formulario2.controls[nombreCampo].invalid || (this.formulario2.controls[nombreCampo].errors && this.formulario2.controls[nombreCampo].errors.required))
                && (this.formulario2.controls[nombreCampo].dirty || this.formulario2.controls[nombreCampo].touched)
        }
        return false;
    }

    mostrarValidacion(campoAValidar, vacio) {
        let camposVacios = this.camposObligatorios.find(x => x.campo == campoAValidar && x.esObligatorio);
        return (camposVacios != null && vacio == 0);
    }

    ngOnDestroy() {
        super.ngOnDestroy();
        this.onEstCompleto.emit({ codigo: EnumPasoSolp.PliegoGeneracion2, esPasoInvalido: this.validadorPasoSolpService.esPasoInvalido() });
    }

    onBlur(control: string) {
        this.validadorPasoSolpService.onBlurDirty(control);
    }

    onRadioButtonChange(visita: string) {
        if (visita == "visitaDeObra") {
            if (this.model.visitaDeObra != true) {
                this.model.visitaDeObra = true;
                this.model.visitaDeObraMasiva = false;
                this.model.listaVisitas = [];
                this.agregarNuevaVisita();
            }
            else {
                this.model.visitaDeObra = false;
                this.model.listaVisitas = [];
            }
        } else if (visita == "visitaDeObraMasiva") {
            if (this.model.visitaDeObraMasiva != true) {
                this.model.visitaDeObraMasiva = true;
                this.model.visitaDeObra = false;
                if (this.model.listaVisitas.length == 0) {
                    this.agregarNuevaVisita();
                }
            }
            else {
                this.model.visitaDeObraMasiva = false;
                this.model.listaVisitas = [];
            }
        }
    }

}