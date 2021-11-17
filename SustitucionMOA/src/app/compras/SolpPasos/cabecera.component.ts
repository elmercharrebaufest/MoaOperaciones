import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ListBaseComponent } from './../../common/base-components/list-base-component'
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { ComprasService } from './../compras.service'
import { PosicionSolp, Solp } from './../Solp';
import { ConfirmationService, SelectItem } from 'primeng/api';
import { AbstractControl, FormGroup, ValidationErrors } from '@angular/forms';
import { ValidadorPasoSolpService } from '../validadorPasoSolpService';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { EnumPasoSolp } from '../enum-paso-solp';
import { Message } from 'primeng/components/common/api';
import { MessageService } from 'primeng/components/common/messageservice';

declare var $: any;

@Component({
    selector: 'cabecera',
    templateUrl: `cabecera.component.html`,
    styleUrls: ['../compras.component.css', './cabecera.component.css'],
})
export class CabeceraComponent extends ListBaseComponent implements OnDestroy {

    @Input('combos')
    protected combos: any;

    @Input('model')
    protected model: Solp;

    @Input('locale')
    protected locale: any;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, private formBuilder: FormBuilder, protected router: Router,
        private validadorPasoSolpService: ValidadorPasoSolpService, private confirmationService: ConfirmationService, private messageService: MessageService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

    }

    // Declaro las variables
    claseDocumento: SelectItem[];
    centroEntrega: SelectItem[];
    monedaCompras: SelectItem[];
    almacenEntrega: SelectItem[];
    posiciones: SelectItem[];
    resultadoProveedores: string[];
    hoy: Date = new Date();
    selectPosicion: any;

    formularioPosicion: [FormGroup];
    formularioActual: FormGroup;
    validFormEliminarPosicion = true;

    proveedoresAutocomplete: any;

    camposObligatorios: any[] = [
        { campo: 'servicio', esObligatorio: true, esFijo: true },
        { campo: 'centroDeCosto', esObligatorio: false, esFijo: true },
        { campo: 'ordenDeOt', esObligatorio: false, esFijo: true },
        { campo: 'ordenDeInversion', esObligatorio: false, esFijo: true },
        { campo: 'siniestroBeneficio', esObligatorio: false, esFijo: true },
        { campo: 'tipoImputacion', esObligatorio: true, esFijo: true },
        { campo: 'textoGenerico', esObligatorio: true, esFijo: true },
        { campo: 'selectCentroEntrega', esObligatorio: false, esFijo: false },
        { campo: 'nombreEntrega', esObligatorio: false, esFijo: true },
        { campo: 'codigoPostalEntrega', esObligatorio: false, esFijo: true },
        { campo: 'selectAlmacenEntrega', esObligatorio: false, esFijo: false },
        { campo: 'calleEntrega', esObligatorio: true, esFijo: true },
        { campo: 'paisEntrega', esObligatorio: false, esFijo: true },
        { campo: 'numeroEntrega', esObligatorio: true, esFijo: true },
        { campo: 'rubroElectrico', esObligatorio: false, esFijo: true },
        { campo: 'rubroCivil', esObligatorio: false, esFijo: true },
        { campo: 'rubroIngenieria', esObligatorio: false, esFijo: true },
        { campo: 'rubroMecanico', esObligatorio: false, esFijo: true },
        { campo: 'rubroConsultoria', esObligatorio: false, esFijo: true },
        { campo: 'proveedoresValidos', esObligatorio: false, esFijo: true },
        { campo: 'proveedoresInvalidos', esObligatorio: false, esFijo: true },
        { campo: 'proveedoresNoSugeridos', esObligatorio: false, esFijo: true },
        { campo: 'selectMonedaCompras', esObligatorio: true, esFijo: true },
    ];

    @Output() onEstCompleto = new EventEmitter<any>();

    mensajesEncabezado: Message[] = [];

    setTabs() {
        this.setMenuSeccionTab("Cabecera", "Cabecera");
    }


    ngOnInit() {
        this.setTabs();

        this.claseDocumento = this.combos.ClaseDocumento;
        this.centroEntrega = this.combos.Centro;
        this.monedaCompras = this.combos.Moneda;
        let claseDocumento = this.model.selectClaseDocumento!== undefined && this.model.selectClaseDocumento.Id>0 ? this.model.selectClaseDocumento : this.claseDocumento[0];
        this.setControlesObligatorios(claseDocumento);
        this.validadorPasoSolpService.formulario = this.formularioActual;

        if (this.model.cargoPasoCinco) {
            this.validadorPasoSolpService.aplicarValidaciones();
        }
        this.model.selectClaseDocumento = claseDocumento;

        this.centroSeleccionado();

        if (this.model.monedaPorDefecto && !this.model.posicionActual.monedaSeleccionada)
            this.model.posicionActual.monedaSeleccionada = this.combos.Moneda.find(x => x.Codigo == this.model.monedaPorDefecto)

        if (!this.model.posicionActual.selectSolicitanteCompras)
            this.model.posicionActual.selectSolicitanteCompras = this.model.fiscalContrato;

        this.model.cargoPasoCinco = true;

        if (this.model.vincularAPliego) {
            this.mensajesEncabezado.push({ severity: 'warn', summary: '', detail: 'No es posible editar esta pantalla desde la plataforma. Para editar dirijase a SAP' });
            this.formularioActual.disable();
        }
        else {
            this.mensajesEncabezado = [];
            this.formularioActual.enable();
        }
    }

    setControlesObligatorios(claseDocumento) {
        this.actualizarCamposObligatorios(claseDocumento);

        if (!this.formularioActual) {
            this.formularioActual = this.formBuilder.group({});
            this.camposObligatorios.forEach(x => {
                let control = x.esObligatorio ? new FormControl('', [Validators.required]) : new FormControl();
                this.formularioActual.addControl(x.campo, control);
            });

            return;
        }

        this.camposObligatorios.forEach(x => {
            let formControl = this.formularioActual.controls[x.campo];
            formControl.clearValidators();
            if (x.esObligatorio) {
                formControl.setValidators(Validators.required);
            }
        });
    }

    actualizarCamposObligatorios(claseDocumento) {
        //reset de obligatorios configurables
        this.camposObligatorios.forEach(c => {
            if (!c.esFijo)
                c.esObligatorio = false;
        });

        let camposObligatorios = this.combos.CamposObligatoriosCabeceraSolp.filter(x => x.ClaseDocumentoCodigo == claseDocumento.Codigo);
        camposObligatorios.forEach(c => {
            if (this.camposObligatorios.find(x => x.campo == c.Codigo) != null)
                this.camposObligatorios.find(x => x.campo == c.Codigo).esObligatorio = true;
        });
    }

    cambiarClaseDocumento() {
        this.setControlesObligatorios(this.model.selectClaseDocumento);
        this.validarPosicionActual();
    }

    mostrarError(nombreCampo: string): boolean {
        if (this.formularioActual && this.formularioActual.controls) {
            let campoObligatorio = this.camposObligatorios.find(x => x.campo == nombreCampo);
            if (campoObligatorio) {
                let control = this.formularioActual.controls[nombreCampo];
                return (control.invalid || (control.errors && control.errors.required))
                    && (control.dirty || control.touched)
            }
        }
        return false;
    }

    mostrarAsterisco(nombreCampo: string) {
        return this.camposObligatorios.find(x => x.campo == nombreCampo).esObligatorio ? '*' : '';
    }

    ngOnDestroy() {
        super.ngOnDestroy();
        this.validarPosicionActual();

        this.onEstCompleto.emit({ codigo: EnumPasoSolp.SolpCabecera, esPasoInvalido: this.model.posicionesValidas() });
    }

    validarPosicionActual() {
        this.validFormEliminarPosicion = this.model.posicionActual.estado;
        this.model.posicionActual.posicionValida = !this.validadorPasoSolpService.esPasoInvalido();
    }

    centroSeleccionado() {
        let direccionCentro: any;

        if (!this.model.posicionActual.selectCentroEntrega && this.model.centroPorDefecto) {
            this.model.posicionActual.selectCentroEntrega = this.combos.Centro.find(x => x.Codigo == this.model.centroPorDefecto);
        }

        if (this.model.posicionActual.selectCentroEntrega) {
            direccionCentro = this.combos.CentrosDireccion.find(x => x.CodigoSap == this.model.posicionActual.selectCentroEntrega.CodigoSap);
        }

        this.model.posicionActual.nombreEntrega = this.model.posicionActual.nombreEntrega
            || (this.model.posicionActual.selectCentroEntrega == undefined ? "" : this.model.posicionActual.selectCentroEntrega.Descripcion);

        if (direccionCentro !== undefined) {
            this.model.posicionActual.codigoPostalEntrega = direccionCentro.Cp;
            this.model.posicionActual.calleEntrega = direccionCentro.Direccion;
            this.model.posicionActual.numeroEntrega = direccionCentro.Numero;
            this.model.posicionActual.paisEntrega = direccionCentro.Pais;
        }
    }

    eliminarPosicion() {
        this.confirmationService.confirm({
            message: '¿Está seguro que desea eliminar la posición?',
            accept: () => {
                this.model.eliminarPosicion()
            },
            reject: () => {

            }
        });
    }

    recuperarPosicion()
    {
        this.model.recuperarPosicion()
    }
    
    buscarCombo(event, type) {
        switch (type) {
            case 'CENTRO':
                this.centroEntrega = this.combos.Centro.filter(x => x.CodigoDescripcion.toLowerCase().includes(event.query.toLowerCase()));
                break;
            case 'ALMACEN':
                if (this.model.posicionActual.selectCentroEntrega == undefined) {
                    this.almacenEntrega = [];
                }
                else {
                    this.almacenEntrega = this.combos.Almacen.filter(x => x.IdPadre == this.model.posicionActual.selectCentroEntrega.Id &&
                        x.CodigoDescripcion.toLowerCase().includes(event.query.toLowerCase()));
                }
                break;
            case 'MONEDA COMPRAS':
                this.monedaCompras = this.combos.Moneda.filter(x => x.CodigoDescripcion.toLowerCase().includes(event.query.toLowerCase()));
                break;
            default:
                break;
        }
    }

    agregarPosicion(el: HTMLElement) {
        this.validarPosicionActual();
        this.model.agregarNuevaPosicion();
        el.scrollIntoView();
    }

}
