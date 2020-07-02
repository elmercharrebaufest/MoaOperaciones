import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { CartaPorteFormularioService } from './../carta-porte2.service';
import { FiltroFechaComponent } from './../../common/view-child/filtro-fecha/filtro-fecha.component';
import { ListBaseComponent } from './../../common/base-components/list-base-component'
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { DropdownComponent, DropdownOption } from './../../common/view-child/dropdown/dropdown.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { Formulario } from './carta-porte.formulario';
import { Seccion } from './../../common/models/Seccion';
import { ModalService } from './../../common/services/ModalService';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
declare var $: any;

@Component({
    selector: 'my-app',
    templateUrl: `./app/carta-porte/formulario/carta-porte.formulario.component.html?v=${new Date().getTime()}`,
    providers: [CartaPorteFormularioService]

})
export class CartaPorteFormularioComponent extends ListBaseComponent {

    @ViewChild("dropdown_pages")
    protected InputPagesComponent: DropdownComponent;

    @ViewChild("sp_imp_temp")
    protected spinnerSmallImpTempComponent: SpinnerSmallComponent;

    @ViewChild("sp_imp_blanco")
    protected spinnerSmallImpBlancoComponent: SpinnerSmallComponent;

    @ViewChild("sp_ctg_buscar")
    protected spinnerSmallCTGBuscarComponent: SpinnerSmallComponent;
    
    constructor(protected service: CartaPorteFormularioService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }
    
    checkPermisos() { this.securityService.tienePermisoRedirect("CREAR FORMULARIO CCPP"); }

    formulario = new Formulario();
    cosechaOptions: any;
    localidadOptions: any[];
    provinciaOptions: any;
    destinoOptions: any;
    destinatarioOptions: any;
    localidadCTG = "";
    localidadDestinoCTG = "";
    ccppPDF: any;
    pageSelected = 1;

    setTabs() {
        this.setMenuSeccionTab("carta-porte", "Formulario");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        var secciones = [new Seccion('/carta-porte/descarga', 'carta-porte', 'Descargas'), new Seccion('/carta-porte/aplicacion', 'carta-porte', 'Aplicaciones')];
        if (this.isAuthorized("CREAR FORMULARIO CCPP"))
            secciones.push(new Seccion('/carta-porte/formulario', 'carta-porte', 'Formulario'));
        this.navService.setSeccionList(secciones);
        this.formulario.cargaPesadaDestino = false;
        this.formulario.declaracionCalidad = false;
        this.formulario.conforme = false;
        this.formulario.condicional = false;
        this.getDataDropdown();
    }

    ngAfterViewInit(): void {
        $(document).on("mouseover", '.form_datetime_carga', function () {
            $('.form_datetime_carga').datetimepicker({
                format: 'yyyy-mm-dd',
                language: 'es',
                weekStart: 1,
                todayBtn: 1,
                autoclose: 1,
                todayHighlight: 1,
                startView: 2,
                forceParse: 0,
                showMeridian: 1,
                pickTime: false,
                minView: 2,
                maxView: 4
            })
        });   
    }

    getDataDropdown() {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        try {
            this.subscription = this.service.getFormularioDropdowns().subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.cosechaOptions = result.cosechas;
                        this.destinoOptions = result.destinos;
                        this.destinatarioOptions = result.destinatarios;
                        this.localidadOptions = result.localidades;
                        this.provinciaOptions = result.provincias;
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

    observableSource(keyword: any) {
        let filteredList = this.localidadOptions.filter(el => el.label.toLowerCase().indexOf(keyword.toLowerCase()) !== -1);
        return Observable.of(filteredList);
    }

    getDataCTG(){
        this.unsubscribe();
        this.floatMsgService.setMsgsEmpty();
        try {
            this.spinnerSmallCTGBuscarComponent.showIt();
            this.subscription = this.service.getDataCTG(this.formulario.nroCTG).subscribe(
                result => {
                    this.spinnerSmallCTGBuscarComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.formulario.fechaCarga = result.data.fechaCarga;
                        this.formulario.nroRenspa = result.data.renspa;
                        //Intermediarios
                        this.formulario.intermediario = result.data.intervinientes.interm;
                        this.formulario.cuitIntermediario = result.data.intervinientes.intermCuit;
                        this.formulario.remitenteComercial = result.data.intervinientes.remCom;
                        this.formulario.cuitRemitenteComercial = result.data.intervinientes.remComCuit;
                        this.formulario.corredorComprador = result.data.intervinientes.corrComp;
                        this.formulario.cuitCorredorComprador = result.data.intervinientes.corrCompCuit;
                        this.formulario.mercadoATermino = result.data.intervinientes.mercadot;
                        this.formulario.cuitMercadoATermino = result.data.intervinientes.mercodatCuit;
                        this.formulario.corredorVendedor = result.data.intervinientes.corrVend;
                        this.formulario.cuitCorredorVendedor = result.data.intervinientes.corrVendCuit;
                        this.formulario.remitenteComercial = result.data.intervinientes.remCom;
                        this.formulario.cuitRemitenteComercial = result.data.intervinientes.remComCuit;
                        this.formulario.destinatario = result.data.intervinientes.destinatario;
                        this.formulario.cuitDestinatario = result.data.intervinientes.destinatarioCuit;
                        this.formulario.destino = result.data.intervinientes.destino;
                        this.formulario.cuitDestino = result.data.intervinientes.destinoCuit;
                        this.formulario.intermediarioFlete = result.data.intervinientes.interFlete;
                        this.formulario.cuitIntermediarioFlete = result.data.intervinientes.interFleteCuit;
                        this.formulario.transportista = result.data.intervinientes.transpor;
                        this.formulario.cuitTransportista = result.data.intervinientes.transporCuit;
                        this.formulario.chofer = result.data.intervinientes.chofer;
                        this.formulario.cuitCuilChofer = result.data.intervinientes.choferCuit;
                        //Granos
                        this.formulario.granoEspecie = result.data.granos.grano;
                        this.formulario.cosecha = result.data.granos.cosecha;
                        this.formulario.localidad = result.data.granos.localOrig;
                        this.formulario.provincia = result.data.granos.provOrig;
                        this.formulario.tipo = result.data.granos.tipo;
                        this.formulario.nroContrato = result.data.granos.contrato;
                        this.formulario.cargaPesadaDestino = result.data.granos.pesadaDest;
                        this.formulario.kilosEstimados = result.data.granos.kilosEst;
                        this.formulario.declaracionCalidad = result.data.granos.decCalidad;
                        this.formulario.conforme = result.data.granos.conforme;
                        this.formulario.condicional = result.data.granos.condicional;
                        this.formulario.pesoBruto = result.data.granos.bruto;
                        this.formulario.pesoNeto = result.data.granos.neto;
                        this.formulario.pesoTara = result.data.granos.tara;
                        this.formulario.observaciones = result.data.granos.observa;
                        this.formulario.procedenciaMercaderia = result.data.granos.procedencia;
                        this.formulario.establecimiento = result.data.granos.establecim;
                        //Destino
                        this.formulario.direccionDestino = result.data.destino.direccion;
                        this.formulario.localidadDestino = result.data.destino.localDest;
                        this.formulario.provinciaDestino = result.data.destino.provDest;
                        //Transporte
                        this.formulario.camion1 = result.data.transporte.camion;
                        this.formulario.tarifaReferencia = result.data.transporte.tarifaRef;
                        this.formulario.kmRecorrer = result.data.transporte.kmRecorrer;
                        this.formulario.pagadorFlete = result.data.transporte.pagaFlete;
                        this.formulario.fletePagado = result.data.transporte.fletePag;
                        this.formulario.fletePagar = result.data.transporte.fleteAPag;
                        this.formulario.acoplado = result.data.transporte.acoplado;
                        this.formulario.tarifa = result.data.transporte.tarifa;
                    }
                },
                error => {
                    this.spinnerSmallCTGBuscarComponent.hideIt();
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.spinnerSmallCTGBuscarComponent.hideIt();
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    } 

    generalFormatter(data: any): string {
        return `${data['label']}`;
    }

    cargarArchivo(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            this.ccppPDF = fileList[0];
        }
    }

    getCompletedPDFTemplate(fechaCarga: string) {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        try {
            this.spinnerSmallImpTempComponent.showIt();
            this.getSpecialInputs(fechaCarga);
            this.subscription = this.service.getCompletedPDFTemplate(this.formulario, this.ccppPDF, this.pageSelected).subscribe(
                result => {
                    this.spinnerSmallImpTempComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        var byteArray = new Uint8Array(result.data);
                        var blob = new Blob([byteArray], { type: 'application/pdf' });
                        if (window.navigator.msSaveOrOpenBlob) {
                            // IE11
                            window.navigator.msSaveOrOpenBlob(blob, "CCPPPrint.pdf");
                        } else {
                            var objectURL = URL.createObjectURL(blob);

                            if ($('#print_page')) $('#print_page').remove();

                            var iframe = document.createElement('iframe');
                            iframe.id = 'print_page';
                            iframe.className = 'sample-iframe';
                            iframe.src = objectURL;
                            document.getElementById("pdf-print").appendChild(iframe);
                            //document.body.appendChild(iframe);
                            iframe.focus();
                            iframe.contentWindow.print();
                       }
                    }
                },
                error => {
                    this.spinnerSmallImpTempComponent.hideIt();
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.spinnerSmallImpTempComponent.hideIt();
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    getTemplate(fechaCarga: string) {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        try {
            this.spinnerSmallImpBlancoComponent.showIt();
            this.getSpecialInputs(fechaCarga);
            this.subscription = this.service.getTemplate(this.formulario).subscribe(
                result => {
                    this.spinnerSmallImpBlancoComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        var bytes = new Uint8Array(result.data);
                        var blob = new Blob([bytes], {
                            type: 'application/pdf'
                        });

                        if (window.navigator.msSaveOrOpenBlob) {
                            // IE11
                            window.navigator.msSaveOrOpenBlob(blob, "CCPPPrint.pdf");
                        } else {
                            var objectURL = window.URL.createObjectURL(blob);

                            if ($('#print_page')) $('#print_page').remove();

                            var iframe = document.createElement('iframe');
                            iframe.id = 'print_page';
                            iframe.className = 'sample-iframe';
                            iframe.src = objectURL;
                            document.getElementById("pdf-print").appendChild(iframe);
                            //document.body.appendChild(iframe);

                            iframe.focus();

                            iframe.contentWindow.print();
                        }
                    }
                },
                error => {
                    this.spinnerSmallImpBlancoComponent.hideIt();
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.spinnerSmallImpBlancoComponent.hideIt();
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    setInputPages(event: any) {
        this.pageSelected = event;
    }

    destinatarioSelected(value: any) {
        this.formulario.destinatario = value.label;
    }

    destinoSelected(value: any) {
        this.formulario.destino = value.label
    }

    cosechaSelected(value: any) {
        this.formulario.cosecha = value.label
    }

    provinciaSelected(value: any) {
        this.formulario.provincia = value.label
    }

    localidadSelected(value: any) {
        this.formulario.localidad = value.label
    }

    provinciaDestinoSelected(value: any) {
        this.formulario.provinciaDestino = value.label
    }

    localidadDestinoSelected(value: any) {
        this.formulario.localidadDestino = value.label
    }

    getSpecialInputs(fechaCarga: string) {
        try { this.formulario.fechaCarga = fechaCarga.split(" ")[0] } catch{ }
    }
}