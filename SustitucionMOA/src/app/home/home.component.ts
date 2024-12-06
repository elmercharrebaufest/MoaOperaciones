import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { HomeService } from './home.service';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
import { SecurityService } from './../common/services/SecurityService';
import { SessionDataService } from './../common/services/SessionDataService';
import { BaseComponent } from './../common/base-components/base-component';
import { ModalService } from './../common/services/ModalService';
import { CarouselNotificacionesComponent } from '../notificaciones/carousel-notificaciones/carousel-notificaciones.component';
@Component({
    selector: 'app-home',
    //template: '<h1>{{titulo}}</h1>'
    templateUrl: `home.component.html`,
    providers: [HomeService]
})
export class HomeComponent extends BaseComponent implements OnInit, OnDestroy {

    constructor(private service: HomeService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.checkPermisos();
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
        this.filtroFechaComponent = new FiltroFechaComponent();
    }

    @ViewChild(FiltroFechaComponent)
    protected filtroFechaComponent: FiltroFechaComponent;

    @ViewChild("msjHome")
    private mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    
    @ViewChild(CarouselNotificacionesComponent)
    protected carouselNotificaciones: CarouselNotificacionesComponent;


    visible: boolean = false;
    cuentasCorrientes: any = null;
    vigentes: string;
    fijaciones: string;
    ampliaciones: string;
    anulaciones: string;
    descargas: string;
    aplicaciones: string;
    aprobadas: string;
    registrado: string;
    pendientesRegistro: string;
    observadas: string;
    pagas: string;
    emitidos: string;
    iva: string;
    ganancias: string;
    iibb: string;
    hoy: string;  // Valores que se muestran en pantalla,
    ayer: string; // no tienen relacion con las fechas de filtrado
    mensajeCtaCte: string;
    itemsPerPage = "10";
    subscription: any;
    tituloArchivoPDF = "Documento"

    checkPermisos() {
        if (this.securityService.esGranosRedirect()) { 
            this.securityService.tienePermisoRedirect("CONSULTAR HOME");
        }
    }

    setTabs() {
        this.setMenuSeccionTab("home", "");
        this.sessionDataService.setGranosSelected("G");
        sessionStorage.setItem("granosSelected", "G");
    }

    ngOnInit() {
        this.setTabs();
        this.navService.setSeccionList([]);
        this.getData();
    }

    getData() {
        this.mensajeComponent.setMsgsEmpty();
        this.vaciarDatos();
        this.visible = false;
        this.spinnerComponent.showIt();
        this.mensajeCtaCte = undefined;
        this.unsubscribe();
        this.subscription = this.service.getHomeInfo(this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin).subscribe(
            (result:any) => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                }else if (result.error != undefined && result.error != "") {
                    this.visible = false;
                    this.mensajeComponent.setErrorMsg(result.error);

                } else if (result.info != undefined) {
                    this.visible = false;
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.visible = true;
                    if (result.data != null) {
                        if (result.data.resumen != null) {
                            this.setData(result.data.resumen);
                            if (result.data.msjCtaCte != null && result.data.msjCtaCte != "") {
                                this.mensajeCtaCte = result.data.msjCtaCte;
                                this.cuentasCorrientes = null;
                            }
                            else {
                                this.cuentasCorrientes = result.data.cuentasCorrientes;
                            }

                        }
                    }                    
                }
            },
            error => {
                this.visible = false;
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    descargaPDF(documento: string, ejercicio: string) {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.descargarDocumentoPDF(documento, ejercicio).subscribe(
            (result:any) => {
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
                        window.navigator.msSaveOrOpenBlob(blob, this.tituloArchivoPDF + documento + ".pdf");
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = this.tituloArchivoPDF + documento + ".pdf"
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                        return false;
                    }
                }
            },
            error => {
                this.floatMsgService.setErrorMsg(error.message);
            }
        );

        return false;
    }

    setData(resumen: any) {
        this.vigentes = this.getCantidad("Vigentes", resumen);
        this.fijaciones = this.getCantidad("Fijaciones", resumen);
        this.ampliaciones = this.getCantidad("Ampliaciones", resumen);
        this.anulaciones = this.getCantidad("Anulaciones", resumen);
        this.descargas = this.getCantidad("Descargas", resumen);
        this.aplicaciones = this.getCantidad("Aplicaciones", resumen);
        this.aprobadas = this.getCantidad("Aprobadas", resumen);
        this.registrado = this.getCantidad("Registrados", resumen);
        this.pendientesRegistro = this.getCantidad("Pendientes de registro", resumen);
        this.observadas = this.getCantidad("Observadas", resumen);
        this.pagas = this.getCantidad("Pagas", resumen);
        this.emitidos = this.getCantidad("Emitidos", resumen);
        this.iva = this.getCantidad("IVA", resumen);
        this.ganancias = this.getCantidad("Ganancias", resumen);
        this.iibb = this.getCantidad("IIBB", resumen);
    }

    getCantidad(tipo: string, datos: any) {
        var cantidad = "0";
        for (var i = 0; i < datos.length; i++)
        {
            if (datos[i].descripcion == tipo) {
                return datos[i].cantidad;
            }
        }
        return cantidad;
    }

    vaciarDatos() {
        this.cuentasCorrientes = null;
        this.vigentes = null;
        this.fijaciones = null;
        this.ampliaciones = null;
        this.anulaciones = null;
        this.descargas = null;
        this.aplicaciones = null;
        this.aprobadas = null;
        this.registrado = null;
        this.pendientesRegistro = null;
        this.observadas = null;
        this.emitidos = null;
        this.iva = null;
        this.ganancias = null;
        this.iibb = null;
    }

    isVisible(): boolean {
        return this.visible;
    }

    isVisibleCtaCte(): boolean {
        return this.cuentasCorrientes != null && this.cuentasCorrientes.length > 0;
    }

    showModalTableResponsive(cuentaCorriente: any) {
        this.modalService.openModalTableResponsive("Movimientos", [
            { etiqueta: "F. Emisión", valor: cuentaCorriente.docDate },
            { etiqueta: "F. Vto.", valor: cuentaCorriente.fecVto },
            { etiqueta: "Nº Cte.", valor: cuentaCorriente.docNo },
            { etiqueta: "Descripción", valor: cuentaCorriente.descripcion },
            { etiqueta: "Contrato", valor: cuentaCorriente.contrato },
            { etiqueta: "Moneda", valor: cuentaCorriente.moneda },
            { etiqueta: "TC", valor: cuentaCorriente.ukursString },
            { etiqueta: "Debe", valor: cuentaCorriente.debeString },
            { etiqueta: "Haber", valor: cuentaCorriente.haberString }
        ]);
        return false;
    }
}