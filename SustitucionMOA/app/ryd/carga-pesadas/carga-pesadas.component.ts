import { Component, OnInit, ViewChild, OnDestroy  } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { RYDService } from './../ryd.service';
import { RYDBaseComponent } from './../ryd.component';
import { Pesada, CargaPesadas } from './../ryd';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { DropdownComponent, DropdownOption } from './../../common/view-child/dropdown/dropdown.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { Formatter } from './../../common/formatter/Formatter';
import { ModalService } from './../../common/services/ModalService';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

import { Subject } from "rxjs/Subject";
import "rxjs/add/operator/takeUntil";

declare var $: any;

@Component({
    selector: 'carga-pesadas',
    templateUrl: `./app/ryd/carga-pesadas/carga-pesadas.component.html?v=${new Date().getTime()}`,
    providers: [RYDService]
})

export class CargaPesadasComponent extends RYDBaseComponent implements OnDestroy  {

    @ViewChild("dropdown_balanza")
    protected InputBalanzaComponent: DropdownComponent;

    @ViewChild("dropdown_commodity")
    protected InputCommoditiesComponent: DropdownComponent;

    @ViewChild("dropdown_exportador")
    protected InputExportadoresComponent: DropdownComponent;

    @ViewChild("topMensaje")
    protected mensajeTopComponent: MensajeComponent;

    @ViewChild("bottomMensaje")
    protected mensajeBottomComponent: MensajeComponent;

    @ViewChild("topSpinner")
    protected spinnerTopComponent: SpinnerComponent;

    @ViewChild("bottomSpinner")
    protected spinnerBottomComponent: SpinnerComponent;

    constructor(protected service: RYDService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.InputBalanzaComponent = new DropdownComponent();
        this.InputCommoditiesComponent = new DropdownComponent();
        this.InputExportadoresComponent = new DropdownComponent();
        this.mensajeTopComponent = new MensajeComponent();
        this.mensajeBottomComponent = new MensajeComponent();
        this.spinnerTopComponent = new SpinnerComponent();
        this.spinnerBottomComponent = new SpinnerComponent();
    }

    public cargaPesadas: CargaPesadas;
    balanzaOptions: any;
    bodegaOptions: any;
    bodegaInput: any;
    commodityOptions: any;
    destinoOptions: any;
    destinoInput: any;
    exportadorOptions: any;
    vaporOptions: any;
    vaporInput: any;
    habilitarCargaPesadas: boolean = false;
    numeroPesada: number = 1;
    fechaPesada: string = "";
    pesoTara: number = 0;
    pesoBruto: number = 0;
    pesoNeto: number = 0;
    pesadas: Array<Pesada> = [];;
    subscription: any;
    itemsPerPage = 10; 

    checkPermisos() { this.securityService.tienePermisoRedirect("REGISTRAR PESADA"); }

    setTabs() {
        this.setMenuSeccionTab("ryd", "Carga de Pesadas");
    }

    ngOnInit() {
        super.ngOnInit();
        this.initCargaPesada();
        this.getDataInputs();

    }

    ngAfterViewInit(): void {
        $('.form_datetime').datetimepicker({
            format:"dd/mm/yyyy HH:ii",
            language: 'es',
            weekStart: 1,
            todayBtn: 1,
            autoclose: 1,
            todayHighlight: 1,
            startView: 2,
            forceParse: 0,
            showMeridian: 1,
            minuteStep: 1,
            useCurrent: true
        });

        $(document).on("focus", ".form_datetimePesadas", function () { // <- Esto es porque el campo se encuentra oculto al iniciar.
            $(this).datetimepicker({
                format: "dd/mm/yyyy HH:ii",
                language: 'es',
                weekStart: 1,
                todayBtn: 1,
                autoclose: 1,
                todayHighlight: 1,
                startView: 2,
                forceParse: 0,
                showMeridian: 1,
                minuteStep: 1,
                useCurrent: true
            });
        });
    }

    getDataInputs() {
        this.setAllMsgsEmpty();
        this.subscriptionDropDowns = this.service.getInputsCargaPesadas().subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                }else if (result.error != undefined && result.error != "") {
                    this.mensajeTopComponent.setErrorMsg(result.error);
                } else {
                    this.balanzaOptions = result.data.balanzas;
                    this.bodegaOptions = result.data.bodegas;
                    this.commodityOptions = result.data.commodities;
                    this.destinoOptions = result.data.destinos;
                    this.exportadorOptions = result.data.exportadores;
                    this.vaporOptions = result.data.vapores;
                }
            },
            error => {
                this.mensajeTopComponent.setErrorMsg(error.message);
            }

        );
    }

    private validarEncabezado(fecha: string) {
        try {
            this.setAllMsgsEmpty();
            this.validateInput(this.cargaPesadas.balanza, "Balanza");
            this.cargaPesadas.bodega = this.validateInput(this.bodegaInput, "Bodega");
            this.cargaPesadas.commodity = this.validateInput(this.cargaPesadas.commodity, "Commodity");
            this.cargaPesadas.destino = this.validateInput(this.destinoInput, "Destino");
            this.cargaPesadas.exportador = this.validateInput(this.cargaPesadas.exportador, "Exportador");
            this.cargaPesadas.vapor = this.validateInput(this.vaporInput, "Vapor");
            this.cargaPesadas.pesoAcumulado = 0;
            this.cargaPesadas.fecha = Formatter.parseFecha(fecha);
            this.habilitarCargaPesadas = !this.habilitarCargaPesadas;

        } catch (err) {
            this.mensajeTopComponent.setErrorMsg(err.message);

        }
        return false; // <-- Prevent refresh.
    }

    private agregarPesada(fecha: string) {
        try {
            this.setAllMsgsEmpty();
            this.showAllSpinners();
            var pesada = new Pesada();
            pesada.numeroPesada = this.numeroPesada;
            pesada.fecha = Formatter.parseFecha(fecha);
            pesada.pesoBruto = this.pesoBruto;
            pesada.pesoNeto = this.pesoNeto;
            pesada.pesoTara = this.pesoTara;
            this.cargaPesadas.pesadas = [pesada];
            this.unsubscribe();
            this.subscription = this.service.postRegistrarPesadas(this.cargaPesadas.balanza,
                this.cargaPesadas.fecha,
                this.cargaPesadas.bodega,
                this.cargaPesadas.commodity,
                this.cargaPesadas.destino,
                this.cargaPesadas.exportador,
                this.cargaPesadas.vapor,
                this.cargaPesadas.pesoProgramado,
                this.cargaPesadas.pesoAcumulado,
                this.numeroPesada,
                pesada.fecha,
                this.pesoTara,
                this.pesoBruto).subscribe(
                result => {
                    this.hideAllSpinners();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }else if (result.error != null) {
                        this.setAllErrorMsgs(result.error);
                    } else {
                        this.pesadas.push(pesada);
                        this.numeroPesada++;
                        this.cargaPesadas.pesoAcumulado = +this.cargaPesadas.pesoAcumulado + +pesada.pesoNeto;
                        this.vaciarPesada();
                    }
                },
                error => {
                    this.hideAllSpinners();
                    this.setAllErrorMsgs(error.message);
                }
            );
        } catch {

        }
        return false; // <-- Prevent refresh.
    }

    private finalizarPesadas() {
        this.setAllMsgsEmpty();
        this.showAllSpinners();
        this.unsubscribe();
        this.subscription = this.service.postFinalizarCargaPesadas(this.cargaPesadas.balanza, this.cargaPesadas.fecha).subscribe(
            result => {
                this.hideAllSpinners();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                }else if (result.error != undefined && result.error != "") {
                    this.setAllErrorMsgs(result.error);
                } else if (result.info != undefined) {
                    this.setAllInfoMsgs(result.info);
                } else {
                    this.mensajeTopComponent.setSuccessMsg(result.data);
                    this.emptyAll();
                    this.InputBalanzaComponent.setSelectItem("");
                    this.cargaPesadas.balanza = "";
                }
            },
            error => {
                this.hideAllSpinners();
                this.setAllErrorMsgs(error.message);
            }

        )
        return false; // <-- Prevent refresh.
    }

    private verificarBalanzaEnProceso() {
        this.spinnerTopComponent.showIt();
        this.setAllMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.verificarBalanzaEnProceso(this.cargaPesadas.balanza).subscribe(
            result => {
                this.spinnerTopComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                }else if (result.error != undefined && result.error != "") {
                    this.mensajeTopComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeTopComponent.setInfoMsg(result.info);
                } else {
                    this.setDatosCarga(result.data);
                } 
            },
            error => {
                this.spinnerTopComponent.hideIt();
                this.mensajeTopComponent.setErrorMsg(error.message);
            }

        )
        return false;
    }

    private initCargaPesada() {
        this.cargaPesadas = new CargaPesadas();
        this.cargaPesadas.pesoProgramado = 0;
        this.cargaPesadas.pesoAcumulado = 0;
        this.cargaPesadas.fecha = "";
        this.cargaPesadas.pesadas = [];
    }

    private emptyAll() {
        this.vaciarPesada();
        this.setDatosCarga(null);

    }

    private vaciarPesada() {
        this.pesoBruto = 0;
        this.pesoNeto = 0;
        this.pesoTara = 0;
    }
    
    private validateInput(value: any, nombreInput: string) : string {
        if (value == "" || value == undefined) {
            throw Error("El valor del campo " + nombreInput + " no es válido.");
        } else if (value.value != undefined) {
            return value.value
        } else { return value }
    }

    private showAllSpinners() {
        this.spinnerTopComponent.showIt();
        this.spinnerBottomComponent.showIt();
    }

    private hideAllSpinners() {
        this.spinnerTopComponent.hideIt();
        this.spinnerBottomComponent.hideIt();
    }

    private setAllMsgsEmpty() {
        this.mensajeTopComponent.setMsgsEmpty();
        this.mensajeBottomComponent.setMsgsEmpty();
    }

    private setAllErrorMsgs(msg: string) {
        this.mensajeTopComponent.setErrorMsg(msg);
        this.mensajeBottomComponent.setErrorMsg(msg);
    }

    private setAllInfoMsgs(msg: string) {
        this.mensajeTopComponent.setInfoMsg(msg);
        this.mensajeBottomComponent.setInfoMsg(msg);
    }

    private setAllSuccessMsgs(msg: string) {
        this.mensajeTopComponent.setSuccessMsg(msg);
        this.mensajeBottomComponent.setSuccessMsg(msg);
    }

    private setInputBalanza(value: string) {
        this.cargaPesadas.balanza = value;
        this.verificarBalanzaEnProceso();
    }

    private setInputCommodity(value: string) {
        this.cargaPesadas.commodity = value;
    }

    private setInputExportador(value: string) {
        this.cargaPesadas.exportador = value;
    }

    generalFormatter(data: any): string {
        return `${data['label']}`;
    }

    private calcularPesoNeto() {
        this.pesoNeto = this.pesoBruto - this.pesoTara;
    }

    private setDatosCarga(carga: CargaPesadas) {
        if (carga != undefined) {
            this.cargaPesadas.pesadas = carga.pesadas;
            this.cargaPesadas.fecha = carga.fecha;
            this.cargaPesadas.hora = carga.hora;
            this.cargaPesadas.commodity = carga.commodity;
            this.cargaPesadas.exportador = carga.exportador;
            this.cargaPesadas.pesoProgramado = carga.pesoProgramado;
            this.cargaPesadas.pesoAcumulado = carga.pesoAcumulado;
            this.bodegaInput = this.setSelectItem(carga.bodega, this.bodegaOptions);
            this.cargaPesadas.bodega = carga.bodega;
            this.InputCommoditiesComponent.setSelectItem(carga.commodity);
            this.destinoInput = this.setSelectItem(carga.destino, this.destinoOptions);
            this.cargaPesadas.destino = carga.destino;
            this.InputExportadoresComponent.setSelectItem(carga.exportador);
            this.vaporInput = this.setSelectItem(carga.vapor, this.vaporOptions);
            this.cargaPesadas.vapor = carga.vapor;
            this.pesadas = carga.pesadas;
            this.numeroPesada = carga.pesadas.length + 1;
            this.habilitarCargaPesadas = true;
        } else {
            this.bodegaInput = null;
            this.cargaPesadas.bodega = "";
            this.InputCommoditiesComponent.setSelectItem("");
            this.destinoInput = null;
            this.cargaPesadas.destino = "";
            this.InputExportadoresComponent.setSelectItem("");
            this.vaporInput = null;
            this.cargaPesadas.vapor = "";
            this.cargaPesadas.commodity = "";
            this.cargaPesadas.exportador = "";
            this.cargaPesadas.fecha = "";
            this.cargaPesadas.pesoAcumulado = 0;
            this.cargaPesadas.pesoProgramado = 0;
            this.pesadas = [];
            this.numeroPesada = 1;
            this.habilitarCargaPesadas = false;
        }
    }

    private setSelectItem(value: string, options: any[]) : any {
        if (options != null) {
            for (let option of options) {
                if (option.value == value) {
                    return option;
                }
            }
        }
    }
}