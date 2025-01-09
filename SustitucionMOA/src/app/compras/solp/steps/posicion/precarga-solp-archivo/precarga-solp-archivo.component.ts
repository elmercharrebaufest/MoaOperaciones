import { Component, Input, OnInit, Output, EventEmitter } from "@angular/core";
import { ComprasService } from "../../../../compras.service";
import { SessionDataService } from "../../../../../common/services/SessionDataService";
import { SolpPosicionPrecargada } from "../../../../../modelos/compras/PrecargaSolp/solpPosicionPrecargada";

@Component({
    selector: 'app-precarga-solp-archivo',
    templateUrl: './precarga-solp-archivo.component.html'
})
export class PrecargaSolpArchivoComponent implements OnInit {

    @Input()
    displayPrecargaSolp: boolean;

    @Input()
    tipoSolpId: number;

    @Output()
    guardarPrecargaEmitter = new EventEmitter<SolpPosicionPrecargada[]>();

    @Output()
    cerrarPrecargaEmitter = new EventEmitter();

    estaProcesando: boolean = false;
    listaErrores: string[] = [];
    posicionesCargadas: SolpPosicionPrecargada[] = [];

    constructor(private service: ComprasService, protected sessionDataService: SessionDataService) {
    }

    ngOnInit() {
    }

    onCancelar() {
        this.limpiarDatos();
        // this.displayPrecargaSolp = false;
        this.cerrarPrecargaEmitter.next();
    }

    onArchivoCargado(event: any) {
        this.limpiarDatos();
        let archivos: FileList = event.target.files;
        if (archivos.length > 0) {
            this.estaProcesando = true;
            let archivo = archivos[0];
            this.service.procesarPrecargaSolp(archivo, this.tipoSolpId)
                .subscribe(
                    (result) => {
                        if (result.logout) {
                            this.sessionDataService.logout();
                        }
                        else if (result.error) {
                            console.error(result.error);
                        }
                        else if (result.info) {
                            console.info(result.info);
                        }
                        else if (result.data) {
                            let response = result.data;
                            if (response.ErroresValidacion && response.ErroresValidacion.length > 0) {
                                this.listaErrores = response.ErroresValidacion;
                            }
                            else {
                                this.posicionesCargadas = response.Posiciones || [];
                            }
                        }
                        this.estaProcesando = false;
                    },
                    (error) => {
                        console.error("Error en el procesamiento del archivo de precarga solp", error);
                        this.estaProcesando = false;
                    }
                );
        }
    }

    onGuardarPrecarga() {
        if (this.posicionesCargadas && this.posicionesCargadas.length > 0) {
            this.guardarPrecargaEmitter.next(this.posicionesCargadas);
        }
        this.displayPrecargaSolp = false;
        this.limpiarDatos();
    }

    limpiarDatos() {
        this.estaProcesando = false;
        this.posicionesCargadas = [];
        this.listaErrores = [];
    }
}