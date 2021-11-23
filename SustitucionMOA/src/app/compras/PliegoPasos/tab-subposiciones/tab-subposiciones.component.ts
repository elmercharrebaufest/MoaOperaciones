import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService, Message } from 'primeng/api';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { CambioContraseniaComponent } from '../../../usuario/cambio-contrasenia/usuario.cambio-contrasenia.component';
import { ComprasService } from '../../compras.service';
import { EnumColumnaSubPosicion } from '../../enum-columna-subPosiciones';
import { EnumTipoImputacion } from '../../enum-tipo-imputacion';
import { PosicionSolp, Solp } from '../../Solp';
import { ValidadorPasoSolpService } from '../../validadorPasoSolpService';
import { SubPosicionViewModel } from '../solapaSubposiciones/subPosicionViewModel';

@Component({
  selector: 'tab-subposiciones',
  templateUrl: './tab-subposiciones.component.html',
  styleUrls: ['../../compras.component.css']
})
export class TabSubposicionesComponent extends ListBaseComponent {

  @Input('model')
  protected model: Solp;

  @Input('locale')
  protected locale: any;

  @Input('combos')
  protected combos: any;

  @Input('listadoSubposiciones')
  protected listadoSubposiciones: Array<SubPosicionViewModel>;


  tituloColumnaTipoDeImputacion: string;
  listadoPosicionActul = Array<SubPosicionViewModel>();
  enumTipoImputacion: typeof EnumTipoImputacion = EnumTipoImputacion;
  enumColumnaSubPosicion: typeof EnumColumnaSubPosicion = EnumColumnaSubPosicion;
  total: number = 0;
  unidades: any[];

  tablaAFiltrar: any;
  autocomplete: any[];
  autocompleteServiciosSolp: any[];
  autocompletePaste: { Tabla: string, CodigoSap: string }[] = [];
  autocompleteServiciosSolpPaste: string[] = [];
  arraryErrores: any = new Array<{ id: number, text: string }>();

    // array de columnas en la grilla
    // se utiliza esta array para luego cargar las posiciones dinamicamente segun la informacion del clipboard
    columnasGrilla: any = [
      { nombre: "codigoServicio", tipo: "codigoServicioSolp" },
      { nombre: "tareaSubcontratar", tipo: "tarea" },
      { nombre: "cuentaTd", tipo: "numerico" },
      { nombre: "unidadMedida", tipo: "combo" },
      { nombre: "precioBruto", tipo: "decimal" },
      { nombre: "cuentaMayor", tipo: "codigoSap", tabla: "CuentasSolpSap" },
      { nombre: "tipoImputacion", tipo: "codigoSap" }];

  //variable para verificar si la posicion no fue dada de alta con los datos minimos
  posicionInvalida: boolean = false;


  constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
    protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, private formBuilder: FormBuilder, protected router: Router,
    private validadorPasoSolpService: ValidadorPasoSolpService, private confirmationService: ConfirmationService) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
  }

  camposObligatorios: any[] = [
    { campo: 'codigoServicio', esObligatorio: false, esFijo: true },
    { campo: 'tareaSubcontratar', esObligatorio: true, esFijo: true },
    { campo: 'cuentaTd', esObligatorio: true, esFijo: true },
    { campo: 'unidadMedida', esObligatorio: true, esFijo: true },
    { campo: 'precioBruto', esObligatorio: true, esFijo: true },
    { campo: 'cuentaMayor', esObligatorio: true, esFijo: true },
    { campo: 'tipoImputacion', esObligatorio: true, esFijo: true },
    { campo: 'servicio', esObligatorio: true, esFijo: true },
    { campo: 'centroDeCosto', esObligatorio: false, esFijo: true },
    { campo: 'ordenDeOt', esObligatorio: false, esFijo: true },
    { campo: 'ordenDeInversion', esObligatorio: false, esFijo: true },
    { campo: 'siniestroBeneficio', esObligatorio: false, esFijo: true },
    { campo: 'tipoImputacion', esObligatorio: true, esFijo: true }
  ];

  mensajesEncabezado: Message[] = [];

  validarErrorCustom(subposicion: any, valor: any, campoAValidar: string) {
    return ((this.camposObligatorios.find(x => x.campo == campoAValidar).esObligatorio) && valor.toString().length == 0);  
  }

  validarConNoNulo(subposicion: any, valor: any, campoAValidar: string){    
    if(subposicion.tareaSubcontratar === ""){
        return false
    }  
    
    return valor == null || 
        ((this.camposObligatorios.find(x => x.campo == campoAValidar).esObligatorio) 
        && valor.toString().length == 0);
  }
      
  validarPrecioBruto(subposicion: any, valor: any, campoAValidar: string){
    if(subposicion.tareaSubcontratar === ""){
        return false
    }  
    
    return valor == null ||
        ((this.camposObligatorios.find(x => x.campo == campoAValidar).esObligatorio) 
        && (valor.toString().length == 0 || valor === 0));
  }

  //Como el componente tiene inputs angular llama a esta funcion antes del onInit, entonces el ngOnInit no es necesario.
  ngOnChanges(){
    this.actualizarPosicion();
  }

  actualizarPosicion(){
    this.listadoPosicionActul = this.model.posicionActual.listadoSubPosiciones;

    if (this.listadoPosicionActul.length == 0) {
      this.nuevaPosicion(null);
    }

    this.validarDatosMinimosPosicionActual();
    this.calcularTotalSubPosicion();
    this.actualizarTipoDeImputacion();

    if (this.model.vincularAPliego) {
    this.mensajesEncabezado.push({ severity: 'warn', summary: '', detail: 'No es posible editar esta pantalla desde la plataforma. Para editar dirijase a SAP' });
    }
    else {
      this.mensajesEncabezado = [];
    }
  }


  validarDatosMinimosPosicionActual(): void {
    if ((this.model.posicionActual.textoGenerico == undefined || this.model.posicionActual.textoGenerico == "")
        || (this.model.posicionActual.tipoImputacion == undefined || this.model.posicionActual.tipoImputacion == "")) {
        this.posicionInvalida = true;
    } else
        this.posicionInvalida = false;
  }

  actualizarTipoDeImputacion(): void {
    switch (this.model.posicionActual.tipoImputacion) {
        case this.enumTipoImputacion.CentroDeCosto:
            this.tituloColumnaTipoDeImputacion = "Centro de costo";
            this.tablaAFiltrar = 'CecoSolpSap';
            break;
        case this.enumTipoImputacion.OrdenDeOt:
            this.tituloColumnaTipoDeImputacion = "Orden de OT";
            this.tablaAFiltrar = 'OrdenSolpSap';
            break;
        case this.enumTipoImputacion.OrdenInversion:
            this.tituloColumnaTipoDeImputacion = "Orden de inversión"
            this.tablaAFiltrar = 'OrdenSolpSap';
            break;
        case this.enumTipoImputacion.Siniestro:
            this.tituloColumnaTipoDeImputacion = "Siniestro / Centro de beneficio"
            this.tablaAFiltrar = 'CentroBeneficio';
            break;
    }
  }

  nuevaPosicion(rowSeleccionada: any): void {
    if (!this.model.vincularAPliego) {
        if (rowSeleccionada) {
            let ultimoRegistroEnListado = this.listadoPosicionActul[this.listadoPosicionActul.length - 1]
            if (ultimoRegistroEnListado.id == rowSeleccionada.data.id) {
                ultimoRegistroEnListado.seleccionado = true;
                this.listadoPosicionActul.push(new SubPosicionViewModel(ultimoRegistroEnListado.subPosicion + 1));
            }
        } else {
            this.listadoPosicionActul.push(new SubPosicionViewModel(1));
        }
    }
  } 

  eliminarSubposiciones(indice = -1): void {
    if (this.listadoPosicionActul.length > 0) {
        if (indice >= 0) {
            this.listadoPosicionActul.splice(indice, 1);
            this.reEnumerarSubposiciones(this.listadoPosicionActul);
        } else {
            this.listadoPosicionActul = [];
        }
        this.model.posicionActual.listadoSubPosiciones = this.listadoPosicionActul;
        this.calcularTotalSubPosicion();
    }

    if (this.listadoPosicionActul.length == 0) {
        this.nuevaPosicion(null);
        this.model.posicionActual.listadoSubPosiciones = this.listadoPosicionActul;
    }
  }

  reEnumerarSubposiciones(listadoSubposiciones: Array<SubPosicionViewModel>) {
      for (let i = 0; i < listadoSubposiciones.length; i++) {
          listadoSubposiciones[i].subPosicion = i + 1;
      }
  }

  eliminarSubPosicionIndividual(indice: number): void {
      this.confirmationService.confirm({
          message: '¿Está seguro que desea eliminar la subposición?',
          accept: () => {
              this.eliminarSubposiciones(indice);
          },
          reject: () => {

          }
      });
  }

  eliminarSubPosicion() {
      this.confirmationService.confirm({
          message: '¿Está seguro que desea eliminar todas las subposiciones?',
          accept: () => {
              this.eliminarSubposiciones();
          },
          reject: () => {

          }
      });
  }

  onPaste(evento: any, indexColumna: number, rowIndex: number, dt): void {
      let datos = evento.clipboardData.getData("text");
      if (!datos.includes("Recuperando datos")) {
          this.spinnerComponent.showIt();
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
          this.completarCodigosSapOnPaste();
          this.completarServicioSolpOnPaste();
          this.endEditCell(dt);
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
                  fila[columna.nombre] = Number.isNaN(valor) ? undefined : valor;
                  break;
              case "decimal":
                  let valorDecimal = Number.parseFloat(columnas[index]);
                  fila[columna.nombre] = Number.isNaN(valorDecimal) ? undefined : valorDecimal;
                  break;
              case "combo":
                  let seleccion = this.combos.Unidades.find(x => x.Codigo.toLowerCase() == columnas[index].toLowerCase()) || {};
                  fila.unidadSeleccionada = seleccion;
                  break;
              case "codigoSap":
                  var codigoSap = columnas[index].trim();

                  fila[columna.nombre] = { CodigoSap: codigoSap };

                  this.autocompletePaste.push({
                      CodigoSap: codigoSap,
                      Tabla: columna.tabla || this.tablaAFiltrar
                  });
                  break;
              case "codigoServicioSolp":
                  fila[columna.nombre] = { CodigoSap: columnas[index] };

                  this.autocompleteServiciosSolpPaste.push(columnas[index]);
                  break;
              case "tarea":
                  fila[columna.nombre] = columnas[index];
                  fila.tareaSubcontratarObj = { Descripcion: columnas[index] }
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
      this.total = 1;
      this.listadoPosicionActul.forEach(posicion => {
          this.total = this.total + (+posicion.precioBruto);
      });
  }

  buscarCombo(event, type) {
      switch (type) {
          case 'UNIDAD MEDIDA':
              this.unidades = this.combos.Unidades.filter(x => x.Descripcion.toLowerCase().includes(event.query.toLowerCase()));
              break;

          default:
              break;
      }
  }

//   cambiarSubPosicion(): void {
//     this.listadoPosicionActul = this.model.posicionActual.listadoSubPosiciones;
  
//     this.validarDatosMinimosPosicionActual();
//     this.actualizarTipoDeImputacion();
//     this.calcularTotalSubPosicion();
// }

 

  completarCodigosSapOnPaste() {
      try {
          this.subscription = this.service.obtenerDatosPorCodigosSap(this.autocompletePaste).subscribe(
              (result: any) => {
                  if (result.logout == true) {
                      this.sessionDataService.logout();
                  } else if (result.error != undefined && result.error != "") {
                      this.floatMsgService.setErrorMsg(result.error);
                  } else if (result.info != undefined) {
                      this.floatMsgService.setInfoMsg(result.info);
                  } else {
                      if (result) {
                          this.listadoPosicionActul.forEach(c => {
                              if (c.cuentaMayor && c.cuentaMayor.CodigoSap && !c.cuentaMayor.Codigo) {
                                  c.cuentaMayor = result.find(x => x.Tabla == 'CuentasSolpSap' && x.CodigoSap == c.cuentaMayor.CodigoSap);
                              }

                              if (c.tipoImputacion && c.tipoImputacion.CodigoSap && !c.tipoImputacion.Codigo) {
                                  c.tipoImputacion = result.find(x => x.Tabla == this.tablaAFiltrar && x.CodigoSap == c.tipoImputacion.CodigoSap);
                              }
                          });
                      }
                  }

                  this.spinnerComponent.hideIt();
              },
              error => {
                  this.floatMsgService.setErrorMsg(error.message);
                  this.spinnerComponent.hideIt();
              }
          );
      } catch (e) {
          this.floatMsgService.setErrorMsg(e);
          this.spinnerComponent.hideIt();
          return false; //<-- Prevent Refresh
      }

      return false; //<-- Prevent Refresh
  }

  completarServicioSolpOnPaste() {
      try {
          this.subscription = this.service.obtenerDatosPorCodigosSapServicioSolp(this.autocompleteServiciosSolpPaste).subscribe(
              (result: any) => {
                  if (result.logout == true) {
                      this.sessionDataService.logout();
                  } else if (result.error != undefined && result.error != "") {
                      this.floatMsgService.setErrorMsg(result.error);
                  } else if (result.info != undefined) {
                      this.floatMsgService.setInfoMsg(result.info);
                  } else {
                      if (result && result.length > 0) {
                          this.listadoPosicionActul.forEach(c => {
                          if (c.codigoServicio && c.codigoServicio.CodigoSap) {
                              var datos = result.find(x => x.Codigo == c.codigoServicio.CodigoSap);
                              if (datos) {
                                  c.codigoServicio = datos;
                                  c.tareaSubcontratar = c.codigoServicio.Descripcion;

                                  var unidadSeleccionadaAux = this.combos.Unidades.find(x => x.Descripcion == c.codigoServicio.UnidadMedidaBase);
                                  if (unidadSeleccionadaAux) {
                                      c.unidadSeleccionada = unidadSeleccionadaAux;
                                      c.unidadMedida = unidadSeleccionadaAux.Descripcion;
                                  }
                              }
                          }
                        });
                      }
                  }

                  this.spinnerComponent.hideIt();
              },
              error => {
                  this.floatMsgService.setErrorMsg(error.message);
                  this.spinnerComponent.hideIt();
              }
          );
      } catch (e) {
          this.floatMsgService.setErrorMsg(e);
          this.spinnerComponent.hideIt();
          return false; //<-- Prevent Refresh
      }

      return false; //<-- Prevent Refresh
  }

  autocompleteSap(event, tablaAFiltrar, soloDescripcion = false) {
      try {
          this.subscription = this.service.autocompleteSap(tablaAFiltrar || this.tablaAFiltrar, event.query.toLowerCase()).subscribe(
              (result: any) => {
                  if (result.logout == true) {
                      this.sessionDataService.logout();
                  } else if (result.error != undefined && result.error != "") {
                      this.floatMsgService.setErrorMsg(result.error);
                  } else if (result.info != undefined) {
                      this.floatMsgService.setInfoMsg(result.info);
                  } else {
                      this.autocomplete = soloDescripcion ? result.map(x => x.Descripcion.trim()) : result;
                  }
              },
              error => {
                  this.floatMsgService.setErrorMsg(error.message);
                  this.spinnerComponent.hideIt();
              }
          );
      } catch (e) {
          this.floatMsgService.setErrorMsg(e);
          return false; //<-- Prevent Refresh
      }

      return false; //<-- Prevent Refresh
  }

  autocompleteServicioSolp(event) {
      try {
          this.subscription = this.service.autocompleteServicioSolp(event.query.toLowerCase()).subscribe(
              (result: any) => {
                  if (result.logout == true) {
                      this.sessionDataService.logout();
                  } else if (result.error != undefined && result.error != "") {
                      this.floatMsgService.setErrorMsg(result.error);
                  } else if (result.info != undefined) {
                      this.floatMsgService.setInfoMsg(result.info);
                  } else {
                      this.autocompleteServiciosSolp = result;
                  }
              },
              error => {
                  this.floatMsgService.setErrorMsg(error.message);
                  this.spinnerComponent.hideIt();
              });
      } catch (e) {
          this.floatMsgService.setErrorMsg(e);
          return false; //<-- Prevent Refresh
      }

      return false; //<-- Prevent Refresh
  }

  onSelectServicio(posicion: SubPosicionViewModel, dt) {
      posicion.tareaSubcontratar = posicion.codigoServicio.Descripcion;
      posicion.tareaSubcontratarObj = { ...posicion.codigoServicio };

      var unidadSeleccionadaAux = this.combos.Unidades.find(x => x.Descripcion == posicion.codigoServicio.UnidadMedidaBase);

      if (unidadSeleccionadaAux) {
          posicion.unidadSeleccionada = unidadSeleccionadaAux;
          posicion.unidadMedida = unidadSeleccionadaAux.Descripcion;
      }

      this.endEditCell(dt);
  }

  onSelectTarea(posicion: SubPosicionViewModel, dt) {
      posicion.tareaSubcontratar = posicion.tareaSubcontratarObj.Descripcion;
      posicion.codigoServicio = { ...posicion.tareaSubcontratarObj };

      var unidadSeleccionadaAux = this.combos.Unidades.find(x => x.Descripcion == posicion.codigoServicio.UnidadMedidaBase);
      posicion.unidadSeleccionada = unidadSeleccionadaAux;
      posicion.unidadMedida = unidadSeleccionadaAux.Descripcion;
      this.endEditCell(dt);
  }

  onBlueTarea(event, posicion: SubPosicionViewModel) {
      posicion.tareaSubcontratar = event.target.value;
      posicion.tareaSubcontratarObj = { Descripcion: event.target.value }
  }

  endEditCell(dt) {
      dt.closeCellEdit();
  }


}
