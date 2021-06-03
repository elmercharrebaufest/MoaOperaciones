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
import { SelectItem } from 'primeng/api';
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
    
    

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, private formBuilder: FormBuilder, protected router: Router, private validadorPasoSolpService : ValidadorPasoSolpService) {
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

    formularioPosicion: [FormGroup];
    formularioActual: FormGroup;
    
    @Output() onEstCompleto = new EventEmitter<any>();

    
    // Funcion que crea el chips y setea el evento
    onKeyUp(event: KeyboardEvent, texts: string[]) {
      if (event.key == "Enter") {
       let tokenInput = event.srcElement as any;
       if (tokenInput.value) {
        texts.push(tokenInput.value);
        tokenInput.value = "";
       }
      }
    }  

    // Funcion que hace la lista para el autocomplete
    search(event){
        let query = event.query;
        this.resultadoProveedores = [];
    }


    setTabs() {
        this.setMenuSeccionTab("Cabecera", "Cabecera");
    }

    ngOnInit() {
        this.setTabs();
        console.log(this.combos);

        this.claseDocumento = this.combos.ClaseDocumento;

        this.centroEntrega = this.combos.Centro;

        //this.almacenEntrega = this.combos.Almacen;

        this.grupoCompras = this.combos.GrupoCompras;

        this.articuloCompras = this.combos.GrupoArticulo;

        this.monedaCompras = this.combos.Moneda;

        this.formularioActual = this.formBuilder.group({
            servicio: new FormControl('', [Validators.required]),
            centroDeCosto: new FormControl('', [Validators.required]),
            ordenDeOt: new FormControl('', [Validators.required]),
            ordenDeInversion: new FormControl('', [Validators.required]),
            siniestroBeneficio: new FormControl('', [Validators.required]),
            tipoImputacion: new FormControl('', [Validators.required]),
            textoGenerico: new FormControl('', [Validators.required]),
            fechaEntregaServicio: new FormControl('', [Validators.required]),
            fechaDeLiberacion: new FormControl('', [Validators.required]),
            plazoDeEntrega: new FormControl(),
            concluido: new FormControl(),
            indiceFijacion: new FormControl(),
            selectCentroEntrega: new FormControl('', [Validators.required]),
            nombreEntrega: new FormControl(),
            codigoPostalEntrega: new FormControl(),
            selectAlmacenEntrega: new FormControl('', [Validators.required]),
            calleEntrega: new FormControl('', [Validators.required]),
            paisEntrega: new FormControl(),
            numeroEntrega: new FormControl('', [Validators.required]),
            selectGrupoCompras: new FormControl('', [Validators.required]),
            selectArticuloCompras: new FormControl('', [Validators.required]),
            selectSolicitanteCompras: new FormControl('', [Validators.required]),
            necesidadCompras: new FormControl('', [Validators.required]),
            rubroElectrico: new FormControl(),
            rubroCivil: new FormControl(),
            rubroMecanico: new FormControl(),
            rubroIngenieria: new FormControl(),
            rubroConsultoria: new FormControl(),
            proveedoresValidos: new FormControl(),
            proveedoresInvalidos: new FormControl(),
            proveedoresNoSugeridos: new FormControl(),
            selectMonedaCompras: new FormControl('', [Validators.required])

        });

        this.formularioPosicion = [this.formularioActual]

        this.validadorPasoSolpService.formulario = this.formularioActual;

        if (this.model.cargoPasoCinco) {
            this.validadorPasoSolpService.aplicarValidaciones();
        }

        this.model.cargoPasoCinco = true;
        
 
    }

    nuevaPosicion(){
        this.formularioPosicion.push(this.formBuilder.group({
            servicio: new FormControl('', [Validators.required]),
            centroDeCosto: new FormControl('', [Validators.required]),
            ordenDeOt: new FormControl('', [Validators.required]),
            ordenDeInversion: new FormControl('', [Validators.required]),
            siniestroBeneficio: new FormControl('', [Validators.required]),
            tipoImputacion: new FormControl('', [Validators.required]),
            textoGenerico: new FormControl('', [Validators.required]),
            fechaEntregaServicio: new FormControl('', [Validators.required]),
            fechaDeLiberacion: new FormControl('', [Validators.required]),
            plazoDeEntrega: new FormControl(),
            concluido: new FormControl(),
            indiceFijacion: new FormControl(),
            selectCentroEntrega: new FormControl('', [Validators.required]),
            nombreEntrega: new FormControl(),
            codigoPostalEntrega: new FormControl(),
            selectAlmacenEntrega: new FormControl('', [Validators.required]),
            calleEntrega: new FormControl('', [Validators.required]),
            paisEntrega: new FormControl(),
            numeroEntrega: new FormControl('', [Validators.required]),
            selectGrupoCompras: new FormControl('', [Validators.required]),
            selectArticuloCompras: new FormControl('', [Validators.required]),
            selectSolicitanteCompras: new FormControl('', [Validators.required]),
            necesidadCompras: new FormControl('', [Validators.required]),
            rubroElectrico: new FormControl(),
            rubroCivil: new FormControl(),
            rubroMecanico: new FormControl(),
            rubroIngenieria: new FormControl(),
            rubroConsultoria: new FormControl(),
            proveedoresValidos: new FormControl(),
            proveedoresInvalidos: new FormControl(),
            proveedoresNoSugeridos: new FormControl(),
            selectMonedaCompras: new FormControl('', [Validators.required])

            
        }));
    }


    mostrarError(nombreCampo: string): boolean {
        if (this.formularioActual && this.formularioActual.controls) {
            return (this.formularioActual.controls[nombreCampo].invalid || (this.formularioActual.controls[nombreCampo].errors && this.formularioActual.controls[nombreCampo].errors.required))
                && (this.formularioActual.controls[nombreCampo].dirty || this.formularioActual.controls[nombreCampo].touched)
        }

        return false;
    }


    
    ngOnDestroy(){
        super.ngOnDestroy();
        this.onEstCompleto.emit({codigo :EnumPasoSolp.SolpCabecera, esPasoInvalido : this.validadorPasoSolpService.esPasoInvalido()});
    }

    centroSeleccionado(){
        this.almacenEntrega = this.combos.Almacen.filter(x=> x.IdPadre == this.model.posicionActual.selectCentroEntrega.Id);
        //TODO: completar campos de direccion segun this.combos.CentrosDireccion
        let direccionCentro = this.combos.CentrosDireccion.find(x=> x.CodigoSap == this.model.posicionActual.selectCentroEntrega.CodigoSap);
        this.model.posicionActual.nombreEntrega = this.model.posicionActual.nombreEntrega || this.model.posicionActual.selectCentroEntrega.Descripcion;
        this.model.posicionActual.codigoPostalEntrega = this.model.posicionActual.codigoPostalEntrega || direccionCentro.Cp;
        this.model.posicionActual.calleEntrega = this.model.posicionActual.calleEntrega || direccionCentro.Direccion;
        this.model.posicionActual.numeroEntrega = this.model.posicionActual.numeroEntrega || direccionCentro.Numero;
        this.model.posicionActual.paisEntrega = this.model.posicionActual.paisEntrega || direccionCentro.Pais;


    }

}
