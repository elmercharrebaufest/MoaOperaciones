import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { AdjudicacionDto, AdjudicacionEdicionDto } from '../../../../modelos/adjudicacion';
import { ComprasService } from '../../../compras.service';
import { NavService } from '../../../../common/services/NavService';
import { SecurityService } from '../../../../common/services/SecurityService';
import { FloatMsgService } from '../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../common/services/ModalService';
import { SessionDataService } from '../../../../common/services/SessionDataService';
import { UsuarioService } from '../../../../usuario/usuario.service';
import { ListBaseComponent } from '../../../../common/base-components/list-base-component';
import { SelectItem } from 'ng2-select';

@Component({
    selector: 'app-editar-orden-de-compra',
    templateUrl: './editar-orden-de-compra.component.html',
    styleUrls: ['./editar-orden-de-compra.component.css']
})
export class EditarOrdenDeCompraComponent extends ListBaseComponent implements OnInit {

    @Input()
    displayEditarOc: boolean;

    @Input()
    public ordenDeCompra: AdjudicacionEdicionDto = {} as AdjudicacionEdicionDto;
    combos: any;
    monedaCompras: SelectItem[];
    regiones: SelectItem[];
    condicionesPago: any[] = [];
    condicionesImportacion: any[] = [];

    @Input('locale') es: any;

    @Input() mensaje: string = "";
    @Output() cerrarEditarOcEmitter = new EventEmitter();
    @Output() guardarEditarOcEmitter = new EventEmitter<AdjudicacionEdicionDto>();
    displayTextos: boolean;
    camposDeshabilitados: boolean = false;
    monedaDeshabilitada: boolean = false;

    constructor(protected service: ComprasService, protected usuarioService: UsuarioService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    ngOnInit() {
        this.getCombos();
        this.es = {
            firstDayOfWeek: 1,
            dayNames: ["domingo", "lunes", "martes", "miércoles", "jueves", "viernes", "sábado"],
            dayNamesShort: ["dom", "lun", "mar", "mié", "jue", "vie", "sáb"],
            dayNamesMin: ["D", "L", "M", "X", "J", "V", "S"],
            monthNames: ["enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"],
            monthNamesShort: ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"],
            today: 'Hoy',
            clear: 'Borrar'
        }
    }

    onCerrarEditarOc() {
        this.mensaje = "";
        this.camposDeshabilitados = false;
        this.monedaDeshabilitada = false;
        this.cerrarEditarOcEmitter.next();
    }

    onGuardarEditarOc() {
        this.mensaje = "";
        this.guardarEditarOcEmitter.emit(this.ordenDeCompra);
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
                        this.setComboMoneda();
                        this.setComboRegion();
                        this.setComboCondicionesDePago();
                        this.setComboCondicionesDeImportacion();
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

    public setComboMoneda(): void {
        if (this.combos != undefined) {
            this.monedaCompras = this.combos.Moneda;
        }
    }

    public setComboRegion(): void {
        if (this.combos != undefined) {
            this.regiones = this.combos.Regiones;

        }
    }

    public setComboCondicionesDePago(): void {
        if (this.combos != undefined) {
            this.condicionesPago = this.combos.CondicionesDePago;
            this.condicionesPago = [{ Id: "", CodigoDescripcion: "Seleccione una condicion" }, ...this.condicionesPago];
        }
    }

    public setComboCondicionesDeImportacion(): void {
        if (this.combos != undefined) {
            this.condicionesImportacion = this.combos.CondicionesDeImportacion;
            this.condicionesImportacion = [{ Id: "", CodigoDescripcion: "Seleccione una condicion" }, ...this.condicionesImportacion];

        }
    }

    onEditarCelda(cotizacion: any, campo: string, valorInicial: any) {
        if (cotizacion[campo] === valorInicial) {
            cotizacion[campo] = ''; // Limpia el valor si es igual al valorInicial
        }
    }

    onReestablecerValor(cotizacion: any, campo: string) {
        if (cotizacion[campo] === '' || cotizacion[campo] === null) {
            cotizacion[campo] = 0; // Restablece a cero si está en blanco
        }
    }


    eliminarRow(index: number) {
        if (this.ordenDeCompra.AdjudicacionPosiciones != null) {
            this.ordenDeCompra.AdjudicacionPosiciones[index].Eliminado = true;
            this.monedaDeshabilitada = true;
        }
    }

    eliminarRowServicio(index: number) {
        if (this.ordenDeCompra.AdjudicacionPosiciones != null) {
            this.ordenDeCompra.AdjudicacionPosiciones[index].Eliminado = true;
            this.ordenDeCompra.AdjudicacionPosiciones[index].SubposicionesCompras.forEach(subpo => {
                subpo.Eliminado = true;
            });
            this.monedaDeshabilitada = true;
        }
    }

    eliminarRowSub(posicion: any, subposicion: number) {
        if (posicion != null && posicion.SubposicionesCompras != null) {
            posicion.SubposicionesCompras[subposicion].Eliminado = true;
            const todasSubposicionesEliminadas = posicion.SubposicionesCompras.every(subposicion => subposicion.Eliminado);
            if (todasSubposicionesEliminadas) {
                posicion.Eliminado = true;
            }
            this.monedaDeshabilitada = true;
        }
    }

    validarPorcentaje(entrada: string, porcentaje: number) {
        this.ordenDeCompra.CondicionDePago[entrada] = Math.min(100, Math.max(0, porcentaje));
    }

    public onSelectMoneda(ordenDeCompra: any, event) {
        ordenDeCompra.MonedaCodigo = event.value.Codigo;
        ordenDeCompra.MonedaId = event.value.Id;
    }

    public onSelectRegion(posiciones: any, event) {
        posiciones.RegionCodigo = event.value.CodigoSap;
        posiciones.RegionCodigo = event.value.CodigoSap;
        posiciones.RegionId = event.value.Id;
    }

    public onSelectCondicionesPago(ordenDeCompra: any, event) {
        ordenDeCompra.CondicionDePagoCodigo = event.value.Codigo;
        ordenDeCompra.CondicionDePagoId = event.value.Id;
    }

    public onSelectCondicionesImportacion(ordenDeCompra: any, event) {
        ordenDeCompra.CondicionDeImportacionCodigo = event.value.Codigo;
        ordenDeCompra.CondicionDeImportacionId = event.value.Id;
    }

    abrilModalTextos() {
        this.displayTextos = true;
    }

    cerrarModalTextos() {
        this.displayTextos = false;
    }

    aceptarModalTextos() {
        this.displayTextos = false;
    }

    calcularPrecioTotalPosiciones(posicion: any) {
        if (posicion) {
            if (posicion.SubposicionesCompras != null) {
                const totalSubposiciones = posicion.SubposicionesCompras.reduce(
                    (total, subposicion) => total + (subposicion.Cantidad * subposicion.PrecioBruto),
                    0
                );
                posicion.PrecioTotal = totalSubposiciones;
            }
        }
    }

    public onChangeMoneda(ordenDeCompra: any, event) {
        if (ordenDeCompra.Moneda_Id == event.value.Id) {
            this.camposDeshabilitados = false;
        } else {
            this.camposDeshabilitados = true;
        }
    }

    public onChangeCampos() {
        this.monedaDeshabilitada = true;
    }

}
