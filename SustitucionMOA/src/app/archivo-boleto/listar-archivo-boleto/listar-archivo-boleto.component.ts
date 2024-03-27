import { Component, OnInit, ViewChild } from "@angular/core";
import { NavService } from "../../common/services/NavService";
import { SecurityService } from "../../common/services/SecurityService";
import { FloatMsgService } from "../../common/services/FloatMsgService";
import { ModalService } from "../../common/services/ModalService";
import { ArchivoBoletoService } from "../archivo-boleto.service";
import { BlockUI, NgBlockUI } from "ng-block-ui";
import { SessionDataService } from "../../common/services/SessionDataService";
import { MensajeComponent } from "../../common/view-child/mensaje/mensaje.component";
import { ArchivoBoletoDto } from "../../common/models/archivo-boleto/archivoBoletoDto";
import { finalize } from "rxjs/operators";
import { NGXLogger } from "ngx-logger";
import { FiltroFechaComponent } from "../../common/view-child/filtro-fecha/filtro-fecha.component";
import { Subscription } from "rxjs";
import { ListBaseComponent } from "../../common/base-components/list-base-component";

@Component({
    templateUrl: './listar-archivo-boleto.component.html',
    styleUrls: ['./listar-archivo-boleto.component.css']
})
export class ListarArchivoBoletoComponent
    extends ListBaseComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;
    @ViewChild('mensajePrincipal')
    mensajeComponent: MensajeComponent
    @ViewChild('mensajeCarga')
    mensajeCargaComponent: MensajeComponent

    @ViewChild('filtroFecha')
    protected filtroFechaComponent: FiltroFechaComponent;

    listaArchivos?: ArchivoBoletoDto[] | null = [];
    listaArchivosFiltrados?: ArchivoBoletoDto[] | null = [];

    archivoASubir: File | null = null;
    inputArchivo = null;

    subscriptionLista = new Subscription();

    filtroNombreArchivo = "";
    verModalCargaArchivo = false;

    filtroEstados = null;
    opcionesEstado = [
        { label: 'Pendiente', value: 'Pendiente' },
        { label: 'Aprobado', value: 'Aprobado' },
        { label: 'Rechazado', value: 'Rechazado' },
    ]


    constructor(
        private ngxLogger: NGXLogger,
        protected service: ArchivoBoletoService,
        protected navService: NavService,
        protected securityService: SecurityService,
        protected sessionDataService: SessionDataService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService,
    ) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService)
    }

    ngOnInit(): void {
        this.cargarLista();
        this.setEmptyNavBar()
    }


    cargarLista() {
        this.blockUI.start("Cargando lista")
        try {
            const { fecha_inicio, fecha_fin } = this.filtroFechaComponent;
            this.subscriptionLista.unsubscribe();
            this.subscriptionLista = this.service
                .listarArchivosBoleto(fecha_inicio, fecha_fin)
                .pipe(finalize(() => this.blockUI.stop()))
                .subscribe(res => {
                    const data = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent)
                    if (data) {
                        this.mensajeComponent.setMsgsEmpty();
                    }
                    this.listaArchivos = data;
                })
        } catch (error) {
            this.blockUI.stop()
            this.ngxLogger.error(error)
            this.mensajeComponent.setErrorMsg("Ha ocurrido un error.")
        }
    }


    subirArchivo() {
        if (this.archivoASubir) {
            this.blockUI.start("Cargando archivo")
            try {
                this.service.crearArchivoBoleto(
                    this.archivoASubir
                )
                    .pipe(finalize(() => this.blockUI.stop()))
                    .subscribe(res => {
                        const archivoSubido = this.manejarApiResponse(res, this.sessionDataService, this.mensajeCargaComponent)
                        if (archivoSubido) {
                            this.listaArchivos.push(archivoSubido)
                            this.verModalCargaArchivo = false;
                        }
                    })
            } catch (error) {
                this.blockUI.stop()
                this.ngxLogger.error(error)
                this.mensajeCargaComponent.setErrorMsg("Ha ocurrido un error.")
            }
        }
    }

    setArchivo(event: Event) {
        const target = event.target as HTMLInputElement;
        if (target.files) {
            this.archivoASubir = target.files[0]
        }
    }

    onCierreModal() {
        this.archivoASubir = null;
        this.inputArchivo = null;
        this.mensajeCargaComponent.setMsgsEmpty();
    }

}