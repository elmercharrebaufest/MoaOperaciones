import { Component, EventEmitter, Input, Output } from '@angular/core';
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
import {FormBuilder, FormControl, Validators } from '@angular/forms';
import { EnumPasoSolp } from '../enum-paso-solp';

declare var $: any;

@Component({
    selector: 'cabecera',
    templateUrl: `cabecera.component.html`,
    styleUrls: ['../compras.component.css'],
})
export class CabeceraComponent extends ListBaseComponent {

    @Input('combos') 
    protected combos:any;

    @Input('model') 
    protected model:Solp;

    @Input('locale') 
    protected locale:any;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
         protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
          protected route: ActivatedRoute, private formBuilder: FormBuilder, protected router: Router, 
          private validadorPasoSolpService : ValidadorPasoSolpService, private confirmationService: ConfirmationService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

    }

    // Declaro las variables
    claseDocumento: SelectItem[];
    centroEntrega: SelectItem[];
    monedaCompras: SelectItem[];
    articuloCompras: SelectItem[];
    solicitanteCompras: SelectItem[];
    grupoCompras: SelectItem[];
    almacenEntrega: SelectItem[];
    posiciones: SelectItem[];
    resultadoProveedores: string[]; 
    fechaEntregaServicio: any;
    fechaDeLiberacion: any;
    hoy: Date = new Date();
    selectPosicion: any; 

    // solpActual: Solp;

    formularioPosicion: [FormGroup];
    formularioActual: FormGroup;

    proveedoresAutocomplete: any;
    camposObligatorios: any[] = [
        { campo: 'servicio',                    esObligatorio: true,    esFijo: true },
        { campo: 'centroDeCosto',               esObligatorio: false,   esFijo: true },
        { campo: 'ordenDeOt',                   esObligatorio: false,   esFijo: true },
        { campo: 'ordenDeInversion',            esObligatorio: false,   esFijo: true },
        { campo: 'siniestroBeneficio',          esObligatorio: false,   esFijo: true },
        { campo: 'tipoImputacion',              esObligatorio: true,    esFijo: true },
        { campo: 'textoGenerico',               esObligatorio: true,    esFijo: true },
        { campo: 'fechaEntregaServicio',        esObligatorio: false,    esFijo: false },
        { campo: 'fechaDeLiberacion',           esObligatorio: false,    esFijo: false },
        { campo: 'plazoDeEntrega',              esObligatorio: true,   esFijo: true },
        { campo: 'concluido',                   esObligatorio: false,   esFijo: true },
        { campo: 'indiceFijacion',              esObligatorio: false,   esFijo: true },
        { campo: 'selectCentroEntrega',         esObligatorio: false,    esFijo: false },
        { campo: 'nombreEntrega',               esObligatorio: false,   esFijo: true },
        { campo: 'codigoPostalEntrega',         esObligatorio: false,   esFijo: true },
        { campo: 'selectAlmacenEntrega',        esObligatorio: false,    esFijo: false },
        { campo: 'calleEntrega',                esObligatorio: true,    esFijo: true },
        { campo: 'paisEntrega',                 esObligatorio: false,   esFijo: true },
        { campo: 'numeroEntrega',               esObligatorio: true,    esFijo: true },
        { campo: 'selectGrupoCompras',          esObligatorio: false,    esFijo: false },
        { campo: 'selectArticuloCompras',       esObligatorio: true,    esFijo: true },
        { campo: 'selectSolicitanteCompras',    esObligatorio: true,    esFijo: true },
        { campo: 'necesidadCompras',            esObligatorio: false,   esFijo: true },
        { campo: 'rubroElectrico',              esObligatorio: false,   esFijo: true },
        { campo: 'rubroCivil',                  esObligatorio: false,   esFijo: true },
        { campo: 'rubroIngenieria',             esObligatorio: false,   esFijo: true },
        { campo: 'rubroMecanico',               esObligatorio: false,   esFijo: true },
        { campo: 'rubroConsultoria',            esObligatorio: false,   esFijo: true },
        { campo: 'proveedoresValidos',          esObligatorio: false,   esFijo: true },
        { campo: 'proveedoresInvalidos',        esObligatorio: false,   esFijo: true },
        { campo: 'proveedoresNoSugeridos',      esObligatorio: false,   esFijo: true },
        { campo: 'selectMonedaCompras',         esObligatorio: true,    esFijo: true },
    ];
    
    @Output() onEstCompleto = new EventEmitter<any>();

    
    // Funcion que crea el chips y setea el evento
    // onKeyUp(event: KeyboardEvent, texts: string[]) {
    //   if (event.key == "Enter") {
    //    let tokenInput = event.srcElement as any;
    //    if (tokenInput.value) {
    //     texts.push(tokenInput.value);
    //     tokenInput.value = "";
    //    }
    //   }
    // }  

    
    // onKeyUp(event: KeyboardEvent, texts: string[]) {
    //     debugger
    //     var charCode = event.which || event.keyCode;
        
    //     if (event.key == "Enter" || event.key == "Tab" ) {
    //       if(event.key == "Tab" && this.proveedoresAutocomplete){
    //         texts.push(this.proveedoresAutocomplete);
    //         this.proveedoresAutocomplete = "";
    //         let tokenInput = event.srcElement as any;
    //         tokenInput.value = "";
    //       } 
    //       else {
    //         let tokenInput = event.srcElement as any;
    //             if (tokenInput.value) {
    //                 texts.push(tokenInput.value);
    //                 tokenInput.value = "";
    //             }
    //         }    
    //     }
    // }

    // // Funcion que hace la lista para el autocomplete
    // search(event){
    //     let query = event.query;
    //     this.resultadoProveedores = [];
    // }


    setTabs() {
        this.setMenuSeccionTab("Cabecera", "Cabecera");
    }

    ngOnInit() {
        this.setTabs();

        this.claseDocumento = this.combos.ClaseDocumento;
        this.centroEntrega = this.combos.Centro;
        this.grupoCompras = this.combos.GrupoCompras;
        this.articuloCompras = this.combos.GrupoArticulo;
        this.monedaCompras = this.combos.Moneda;

        this.setControlesObligatorios(this.model.selectClaseDocumento || this.claseDocumento[0]);
        this.validadorPasoSolpService.formulario = this.formularioActual;
        
        if (this.model.cargoPasoCinco) {
            this.validadorPasoSolpService.aplicarValidaciones();
        }

        this.centroSeleccionado();

        this.model.cargoPasoCinco = true;

        // document.getElementById("proveedoresValidos").addEventListener('keydown', function (e) {
        //     if (e.which == 9) {
        //         e.preventDefault();
        //     }
        // });

        // document.getElementById("proveedoresInvalidos").addEventListener('keydown', function (e) {
        //     if (e.which == 9) {
        //         e.preventDefault();
        //     }
        // });

        // document.getElementById("proveedoresNoSugeridos").addEventListener('keydown', function (e) {
        //     if (e.which == 9) {
        //         e.preventDefault();
        //     }
        // });
        
 
    }

    setControlesObligatorios(claseDocumento){
        this.actualizarCamposObligatorios(claseDocumento);

        if(!this.formularioActual){
            this.formularioActual = this.formBuilder.group({});
            this.camposObligatorios.forEach(x=> {
                let control = x.esObligatorio ? new FormControl('', [Validators.required]) : new FormControl();
                this.formularioActual.addControl(x.campo, control);
            });

            return;
        }

        this.camposObligatorios.forEach(x=> {
            let formControl = this.formularioActual.controls[x.campo];
            formControl.clearValidators();
            if(x.esObligatorio){
                formControl.setValidators(Validators.required);
            }
        });
    }

    actualizarCamposObligatorios(claseDocumento){
        //reset de obligatorios configurables
        this.camposObligatorios.forEach(c => {
            if(!c.esFijo)
                c.esObligatorio = false;
        });

        let camposObligatorios = this.combos.CamposObligatoriosCabeceraSolp.filter(x=>x.ClaseDocumentoCodigo == claseDocumento.Codigo);

        camposObligatorios.forEach(c => {
            this.camposObligatorios.find(x=>x.campo == c.Codigo).esObligatorio = true;
        });
    }

    cambiarClaseDocumento(){
        this.setControlesObligatorios(this.model.selectClaseDocumento);
        this.validarPosicionActual();
    }

    mostrarError(nombreCampo: string): boolean {
        if (this.formularioActual && this.formularioActual.controls) {
            let campoObligatorio = this.camposObligatorios.find(x=>x.campo == nombreCampo);
            if(campoObligatorio){
                let control = this.formularioActual.controls[nombreCampo];
                return (control.invalid || (control.errors && control.errors.required))
                    && (control.dirty || control.touched)
            }
        }

        return false;
    }

    mostrarAsterisco(nombreCampo: string){
        return this.camposObligatorios.find(x=>x.campo == nombreCampo).esObligatorio ? '*' : '';
    }
    
    ngOnDestroy(){
        super.ngOnDestroy();
        this.validarPosicionActual();

        this.onEstCompleto.emit({codigo :EnumPasoSolp.SolpCabecera, esPasoInvalido : this.model.posicionesValidas()});
    }

    validarPosicionActual() {
        this.model.posicionActual.posicionValida = !this.validadorPasoSolpService.esPasoInvalido();
    }

    centroSeleccionado(){
        let direccionCentro: any;
        if (this.model.posicionActual.selectCentroEntrega == undefined) {
            this.almacenEntrega = [];
        }
        else {
            this.almacenEntrega = this.combos.Almacen.filter(x => x.IdPadre == this.model.posicionActual.selectCentroEntrega.Id);
            direccionCentro = this.combos.CentrosDireccion.find(x => x.CodigoSap == this.model.posicionActual.selectCentroEntrega.CodigoSap);
        }


        this.model.posicionActual.nombreEntrega =  this.model.posicionActual.nombreEntrega
            || (this.model.posicionActual.selectCentroEntrega == undefined ? "" :this.model.posicionActual.selectCentroEntrega.Descripcion);

        this.model.posicionActual.codigoPostalEntrega =  this.model.posicionActual.codigoPostalEntrega || (direccionCentro == undefined ? "" :direccionCentro.Cp);
        this.model.posicionActual.calleEntrega = this.model.posicionActual.calleEntrega || (direccionCentro == undefined ? "" :  direccionCentro.Direccion);
        this.model.posicionActual.numeroEntrega =  this.model.posicionActual.numeroEntrega || (direccionCentro == undefined ? "" :direccionCentro.Numero);
        this.model.posicionActual.paisEntrega =  this.model.posicionActual.paisEntrega || (direccionCentro == undefined ? "" : direccionCentro.Pais);
    }

    eliminarPosicion()
    {
        console.log("eliminar");
        this.confirmationService.confirm({
            message: '¿Está seguro que desea eliminar la posición?',
            accept: () => {
                this.model.eliminarPosicion()
            },
            reject: () => {
                
            }
        });

    }

    // agregarSolpCabecera(){
        
    // }

}
