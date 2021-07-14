import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ListBaseComponent } from './../../../common/base-components/list-base-component'
import { SessionDataService } from './../../../common/services/SessionDataService';
import { SecurityService } from './../../../common/services/SecurityService';
import { NavService } from './../../../common/services/NavService';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { ComprasService } from '../../compras.service';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { ModalService } from './../../../common/services/ModalService';
import { Solp } from '../../Solp';
import { SubPosicionViewModel } from './subPosicionViewModel';
import { EnumTipoImputacion } from '../../enum-tipo-imputacion'
import { EnumColumnaSubPosicion } from '../../enum-columna-subPosiciones'
import { ConfirmationService } from 'primeng/api';


@Component({
    selector: 'subPosicion',
    templateUrl: `subPosicion.component.html`,
    styleUrls: ['../../compras.component.css'],
})
export class SubPosicionComponent extends ListBaseComponent {

    @Input('model')
    protected model: Solp;

    @Input('locale')
    protected locale: any;

    @Input('combos') 
    protected combos:any;

    tituloColumnaTipoDeImputacion: string;
    listadoPosicionActul = Array<SubPosicionViewModel>();
    enumTipoImputacion: typeof EnumTipoImputacion = EnumTipoImputacion;
    enumColumnaSubPosicion: typeof EnumColumnaSubPosicion = EnumColumnaSubPosicion;
    total: number = 0;

    // array de columnas en la grilla
    // se utiliza esta array para luego cargar las posiciones dinamicamente segun la informacion del clipboard
    columnasGrilla: any = [{ nombre: "codigoServicio", tipo: "numerico" }, { nombre: "tareaSubcontratar", tipo: "string" }, { nombre: "cuentaTd", tipo: "numerico" }, { nombre: "unidadMedida", tipo: "combo" }, { nombre: "precioBruto", tipo: "decimal" }, { nombre: "cuentaMayor", tipo: "numerico" }, { nombre: "tipoImputacion", tipo: "numerico" }];

    //variable para verificar si la posicion no fue dada de alta con los datos minimos
    posicionInvalida: boolean = false;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, private formBuilder: FormBuilder, protected router: Router,  private confirmationService: ConfirmationService
    ) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

    }

    setTabs() {
        this.setMenuSeccionTab("Sub Posiciones", "Sub Posiciones");
    }

    ngOnInit() {
        this.setTabs();

        let primeraPosicion = this.model.posiciones[0]
        this.listadoPosicionActul = primeraPosicion.listadoSubPosiciones;
        this.model.posicionActual = primeraPosicion;

        this.validarDatosMinimosPosicionActual();
        this.calcularTotalSubPosicion();
        this.actualizarTipoDeImputacion();
    }

    validarDatosMinimosPosicionActual(): void {
        if ((this.model.posicionActual.textoGenerico == undefined || this.model.posicionActual.textoGenerico == "")
            && (this.model.posicionActual.tipoImputacion == undefined || this.model.posicionActual.tipoImputacion == "")) {
            this.posicionInvalida = true;
        }
        else
            this.posicionInvalida = false;
    }

    cambiarSubPosicion(): void {
        this.listadoPosicionActul = this.model.posicionActual.listadoSubPosiciones;
        this.validarDatosMinimosPosicionActual();
        this.actualizarTipoDeImputacion();
        this.calcularTotalSubPosicion();
    }

    actualizarTipoDeImputacion(): void {
        switch (this.model.posicionActual.tipoImputacion) {
            case this.enumTipoImputacion.CentroDeCosto:
                this.tituloColumnaTipoDeImputacion = "Centro de costo"
                break;
            case this.enumTipoImputacion.OrdenDeOt:
                this.tituloColumnaTipoDeImputacion = "Orden de OT"
                break;
            case this.enumTipoImputacion.OrdenInversion:
                this.tituloColumnaTipoDeImputacion = "Orden de inversión"
                break;
            case this.enumTipoImputacion.Siniestro:
                this.tituloColumnaTipoDeImputacion = "Siniestro / Centro de beneficio"
                break;
        }
    }

    nuevaPosicion(rowSeleccionada: any): void {
        let ultimoRegistroEnListado = this.listadoPosicionActul[this.listadoPosicionActul.length - 1]
        if (ultimoRegistroEnListado.id == rowSeleccionada.data.id) {
            ultimoRegistroEnListado.seleccionado = true;
            this.listadoPosicionActul.push(new SubPosicionViewModel(this.listadoPosicionActul.length));
        }
    }

    eliminarSubposiciones(): void {
        if (this.listadoPosicionActul.length > 1) {
            let subPosicionesAgregadas = this.listadoPosicionActul.filter(x => !x.eliminar);
            subPosicionesAgregadas.forEach((element, index, array) => {
                element.subPosicion = index;
            });
            this.listadoPosicionActul = subPosicionesAgregadas;
            this.calcularTotalSubPosicion();
        }
    }

    eliminarSubPosicion()
    {
        this.confirmationService.confirm({
            message: '¿Está seguro que desea eliminar la subposición?',
            accept: () => {
                this.eliminarSubposiciones()
            },
            reject: () => {
                
            }
        });
    }

    onPaste(evento: any, indexColumna: number, rowIndex: number): void {

        let datos = evento.clipboardData.getData("text");
        if (!datos.includes("Recuperando datos")) {
            //separo la informacion por filas 
            let filas = datos.split("\n");
            filas.forEach(element => {
                evento.preventDefault();
                //separo la informacion por columnas 
                let columnas = element.split("\t")
                this.SetValuesForColumns(indexColumna, columnas, rowIndex, this.listadoPosicionActul);
                rowIndex++;
            });
            this.calcularTotalSubPosicion();
        }
    }

    /**
    Metodo Auxuliar para cargar una fila dinamicamente
        indexColumn : posicion de la columna en la grilla coincide con el array columnasGrilla
    columnas : array de valores del clipboard que se obtiene de cada columna
    */
    SetValuesForColumns(indexColumn: number, columnas: any, rowIndex: number, listado: SubPosicionViewModel[]): void {
        let esEdicion = false;

        //piso las filas que tengan datos y si no tengo mas filas creo nuevas
        let tamañoArray = this.listadoPosicionActul.length;
        let fila: SubPosicionViewModel;
        if (rowIndex < tamañoArray) {
            fila = listado[rowIndex];
            esEdicion = true;
        } else {
            fila = new SubPosicionViewModel(this.listadoPosicionActul.length);
        }

        for (let index = 0; index < columnas.length && index < this.columnasGrilla.length; index++) {
            let columna = this.columnasGrilla[indexColumn + index]
            switch (columna.tipo) {
                case "numerico":
                    let valor = Number.parseInt(columnas[index]);
                    fila[columna.nombre] = Number.isNaN(valor) ?  undefined:valor;
                    break;
                case "decimal":
                    let valorDecimal = Number.parseFloat(columnas[index]);
                    fila[columna.nombre] = Number.isNaN(valorDecimal) ?  undefined:valorDecimal;
                    break;
                case "combo":
                    let seleccion = this.combos.Unidades.find( x => x.Codigo.toLowerCase() == columnas[index].toLowerCase()) || {};
                    fila.unidadSeleccionada = seleccion;
                    break;
                default:
                    fila[columna.nombre] = columnas[index];
                    break;
            }
        }

        if (!esEdicion) {
            listado.push(fila);
            listado.push(new SubPosicionViewModel(this.listadoPosicionActul.length));
        }
    }

    calcularTotalSubPosicion() {
        this.total = 0;
        this.listadoPosicionActul.forEach(posicion => {
            this.total = this.total + (+posicion.precioBruto);
        });
    }
}
