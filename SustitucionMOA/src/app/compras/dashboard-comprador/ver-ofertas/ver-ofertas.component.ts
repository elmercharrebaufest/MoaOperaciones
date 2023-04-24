import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { UsuarioService } from '../../../usuario/usuario.service';
import { ComprasService } from '../../compras.service';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { Table } from 'primeng/table';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { PeticionDeOfertaDto } from '../../../modelos/peticion-de-oferta-model';
import { Solp } from '../../solp/solp';

@Component({
  selector: 'app-ver-ofertas',
  templateUrl: './ver-ofertas.component.html',
  styleUrls: ['../../compras.component.css',
    './ver-ofertas.component.css']
})
export class VerOfertasComponent extends ListBaseComponent implements OnInit {
  @BlockUI() blockUI: NgBlockUI;

  @ViewChild("tabla")
  protected tabla: Table;

  @Input()
  public peticion: PeticionDeOfertaDto;

  peticionOferta: PeticionDeOfertaDto;
  SolpDto: Solp;
  tablaOfertas: PeticionDeOfertaDto;
  frozenCols: any[];
  scrollableCols: any[];
  cols: any[];

  constructor(protected service: ComprasService, protected usuarioService: UsuarioService, protected navService: NavService, protected sessionDataService: SessionDataService,
    protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
  }

 


  ngOnInit() {
    if (this.route.params) {
      this.route.params.forEach((params: Params) => {
          let peticionOferta_Id = parseInt(params["id"]);
          this.verOfertas(peticionOferta_Id);
      })
    };

    if (this.tablaOfertas == null) {
      this.tablaOfertas = {
          Id: null,
          FechaEntregaFormateado: null,
          PlazoDeOferta: null,
          CUIT: null,
          Mail: null,
          Usuarios: new Array(),
          SolpDto: null,
          Selected: null
      };
    }

    this.frozenCols  = [
        { field: 'checkbox', header: '' },
        { field: 'posicion', header: 'POS.' },
        { field: 'codigo', header: 'CODIGO' },
        { field: 'descripcion', header: 'DESCRIPCION' },
        { field: 'textoDeSuministro', header: 'TEXTO DE SUMINISTRO' },
        { field: 'cantidadPendiente', header: 'CTD PENDIENTE' },
        { field: 'cantidad', header: 'CTD.' },
        { field: 'unidadMedida', header: 'UM' }
    ];

    this.scrollableCols = [
        { field: 'cantidadCotizada', header: 'Ctd. Cotizada' },
        { field: 'unidadCotizada', header: 'Um. Cotizada' },
        { field: 'precioUnidad', header: 'Precio unidad' },
        { field: 'precioTotal', header: 'Precio total' },
        { field: 'totalARP', header: 'Total ARP' },
        { field: 'fechaDeEntrega', header: 'Fecha de entrega' },
    ];

    this.cols = [
        { field: 'checkbox', header: '' },
        { field: 'posicion', header: 'POS.' },
        { field: 'codigo', header: 'CODIGO' },
        { field: 'descripcion', header: 'DESCRIPCION' },
        { field: 'textoDeSuministro', header: 'TEXTO DE SUMINISTRO' },
        { field: 'cantidadPendiente', header: 'CTD PENDIENTE' },
        { field: 'cantidad', header: 'CTD.' },
        { field: 'unidadMedida', header: 'UM' },
        { field: 'cantidadCotizada', header: 'Ctd. Cotizada' },
        { field: 'unidadCotizada', header: 'Um. Cotizada' },
        { field: 'precioUnidad', header: 'Precio unidad' },
        { field: 'precioTotal', header: 'Precio total' },
        { field: 'totalARP', header: 'Total ARP' },
        { field: 'fechaDeEntrega', header: 'Fecha de entrega' }
    ];

  }

  verOfertas(peticionOferta_Id) {
    try {
        this.blockUI.start('Cargando...');
        this.subscription = this.service.getListarOfertasComprador(peticionOferta_Id).subscribe(
            (result: any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    this.tablaOfertas = result.data;
                    console.log("ofertas", this.tablaOfertas)

                }
                this.blockUI.stop();
            },
            error => {
                this.floatMsgService.setErrorMsg(error.message);
                this.blockUI.stop();
            });
    } catch (e) {
        this.blockUI.stop();
        this.floatMsgService.setErrorMsg(e);
        return false; //<-- Prevent Refresh
    }
    return false; //<-- Prevent Refresh
  }

  descargarAdjuntosCotizacion(cotizacionId) {
    this.blockUI.start("Descargando...");
    this.service.DescargarAdjuntosCotizacion(cotizacionId)
        .subscribe(
            (result) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                }
                else {
                    var byteArray = new Uint8Array(result.FileContents);
                    var blob = new Blob([byteArray], {
                        type: "application/octet-stream",
                    });

                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(
                            blob,
                            result.FileDownloadName
                        );
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = result.FileDownloadName;
                        link.click();
                        setTimeout(function () {
                            window.URL.revokeObjectURL(url);
                        }, 0);
                        this.blockUI.stop();
                        return false;
                    }
                    this.blockUI.stop();
                }
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
                this.blockUI.stop();
            }
        )
  }
  
}
