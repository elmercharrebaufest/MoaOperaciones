import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { ActivatedRoute, Router } from '@angular/router';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { ComprasService } from '../../compras.service';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { AdjudicacionDto } from '../../../modelos/adjudicacion';
import { AdjuntosSolpDto } from '../adjuntos-solp-model';
import { ArchivoDto } from '../../../modelos/cotizacionDto';
import { Archivo } from '../../../common/models/archivo';

@Component({
  selector: 'app-adjuntos-solp',
  templateUrl: './adjuntos-solp.component.html',
  styleUrls: ['./adjuntos-solp.component.css']
})
export class AdjuntosSolpComponent extends ListBaseComponent implements OnInit {

  @Input() displayAdjuntosSolp: boolean;
  @Input() adjuntosSolpDto: AdjuntosSolpDto;
  @Input() nroSolp: string;

  @Output() cerrarModalAdjuntosEmitter = new EventEmitter();

  @BlockUI() blockUI: NgBlockUI;
  
  error: string;
  visualizarAlert = false;
  mostrarPreview: boolean;
  pdfPreview: any;



  constructor(protected service: ComprasService, protected navService: NavService,
    protected sessionDataService: SessionDataService, protected securityService: SecurityService,
    protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, protected router: Router) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
  }

  ngOnInit() {
  }

  onCerrarModalAdjuntos() {
    this.cerrarModalAdjuntosEmitter.next();
  }

  onHideAdjuntosDialog() {
      this.cerrarModalAdjuntosEmitter.next();
  }

  mostrarPliego(){
    if (this.adjuntosSolpDto.PDF) {
        this.pdfPreview = "data:application/pdf;base64," + this.adjuntosSolpDto.PDF;
        this.mostrarPreview = true;
    }
  }

  descargarArchivo(archivo: Archivo) {
 
    let archivoId: number = archivo.Id;
    let fileKey: string = archivo.FileKey
    let proveedorId: number = 0;
    let urlApi: string = "/api/compras/DescargarArchivo?archivoId=" + archivoId.toString();
    
    var param = btoa("fileKey=" + fileKey + "&mail=" + '' + "&archivoId=" + archivoId.toString() + "&proveedorId=" + '' + "&urlApi=" + urlApi.toString());

    var url = "/officetohtml/index.html?param=" + param;
    var link = document.createElement("a");
    document.body.appendChild(link);
    link.href = url;
    link.target = "_blank";
    link.click();
  }
}
