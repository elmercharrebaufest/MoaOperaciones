import { DatePipe } from '@angular/common';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { BaseComponent } from '../common/base-components/base-component';
import { NavService } from '../common/services/NavService';
import { SecurityService } from '../common/services/SecurityService';
import { FloatMsgService } from '../common/services/FloatMsgService';
import { ModalService } from '../common/services/ModalService';
import { ComunicacionesService } from './comunicaciones.service';
import { SessionDataService } from '../common/services/SessionDataService';
import { LayoutComponent } from '../layout/layout.component';
import { Router } from '@angular/router';
import { UpdateComunicacionService } from '../common/services/UpdateComunicacionService';

@Component({
  selector: 'app-comunicaciones',
  templateUrl: './comunicaciones.component.html',
  styleUrls: ['./comunicaciones.component.css'],
  providers: [DatePipe]
})
export class ComunicacionesComponent extends BaseComponent implements OnInit {
  @Output() cerrarComunicaciones: EventEmitter<any> = new EventEmitter();
  communication: any;
  idProveedor: string;
  communicationRead: boolean[] = [];
  arrayVencidas: any;
  arrayProximasAVencer: any;
  contadorConsultas: number;
  quantityCommunication: number = 0;
  hasCommunications: boolean = false;
  showButtonMoreCommunications: boolean = false;
  ids: any;
  filter: string;

    startDate: string;
    endDate: string;

    // Define una variable para almacenar los IDs
    comunicacionTipoSeisIds: number[] = [];

    ExencionesVencidasIds: number[] = [];
    ExencionesAVencerIds: number[] = [];
    ConsultasIds: number[] = [];
    liquidacionesIds: number[] = [];

    subscriptionComunicaciones: any;
    rawCommunications: any;

  constructor(
    protected navService: NavService, 
    protected securytiService: SecurityService,
    protected floatMsgService: FloatMsgService, 
    protected modalService: ModalService,
    protected serviceComunicaciones: ComunicacionesService,
    private sessionDataService: SessionDataService,
    protected layoutComponent: LayoutComponent,
    private router: Router,
      private datePipe: DatePipe,
      private updateComunicacionService: UpdateComunicacionService,
    ) {
    super(navService, securytiService, floatMsgService, modalService);

    this.idProveedor = sessionStorage.getItem("proveedor");

    sessionDataService.proveedor$.subscribe(
      proveedor => {
          this.idProveedor = proveedor;
      });
      //Para recibir actualizacíones al componente
      this.updateComunicacionService.getAllCommunicationsObserv$.subscribe(res => {
          this.rawCommunications = res;
          this.formatComunicaciones();
      });

  }

  ngOnInit() {
      this.dateConvert();
      // This will get the communications list
      this.updateComunicacionService.updateCommunications(this.idProveedor, this.startDate, this.endDate);
      this.checkCommunications();
  }

    ngOnDestroy() {
        //this.subscription.unsubscribe();
        this.quantityCommunication = 0;
    }
  

    formatComunicaciones() {
        let result = this.rawCommunications;
        this.communicationRead = [];
        this.arrayVencidas = new Set();
        this.arrayProximasAVencer = new Set();
        this.contadorConsultas = 0;
        this.arrayVencidas = new Set();
        this.arrayProximasAVencer = new Set();

        try {
            if (result.data.length == 0) {
                this.hasCommunications = false;
                this.showButtonMoreCommunications = false;
                this.communication = [];
            } else {
                this.hasCommunications = true;
                if (result.data.length > 15) {
                    this.showButtonMoreCommunications = true;
                } else {
                    this.showButtonMoreCommunications = false;
                }
            }

            const agrupadoPorFecha = result.data.reduce((result, element) => {
                const fechaCreacion = element.FechaCreacion;

                if (!result[fechaCreacion]) {
                    result[fechaCreacion] = [];
                }

                result[fechaCreacion].push(element);

                return result;
            }, {});

            this.communication = agrupadoPorFecha;

            let filteredCommunication = {};

            //MMSN - 134: Ajuste - Notificaciones Apiladas
            let skippedItems = [];
            let stackedTypes = [1, 2, 6];

            class stackedComm {
                Id: number;
                Type: number;
            }

            let skippedItemsByType = [];


            for (const fecha in this.communication) {
                const items = this.communication[fecha];
                const filteredItems = [];
                const seenCategories = {};
                const categoryCounts = {};

                let tipo1Found = false;
                let tipo2Found = false;
                let tipo6Found = false;

                for (const item of items) {
                    if (item.ComunicacionTipo === 1 && !tipo1Found) {
                        filteredItems.push(item);
                        tipo1Found = true;
                    } else if (item.ComunicacionTipo === 2 && !tipo2Found) {
                        filteredItems.push(item);
                        tipo2Found = true;
                    } else if (item.ComunicacionTipo === 6 && !tipo6Found) {
                        filteredItems.push(item);
                        tipo6Found = true;
                    } else if (item.ComunicacionTipo !== 1 && item.ComunicacionTipo !== 2 && item.ComunicacionTipo !== 5 && item.ComunicacionTipo !== 6) {
                        filteredItems.push(item);
                    } else if (stackedTypes.includes(item.ComunicacionTipo)) {
                        const st = new stackedComm();
                        st.Id = item.Id;
                        st.Type = item.ComunicacionTipo;
                        skippedItemsByType.push(st);
                    }

                    // Filtrar por DescripcionCategoria
                    if (item.ComunicacionTipo === 5 && !seenCategories[item.DescripcionCategoria]) {
                        filteredItems.push(item);
                        categoryCounts[item.DescripcionCategoria] = 1;
                        seenCategories[item.DescripcionCategoria] = true;
                    }
                    else {
                        categoryCounts[item.DescripcionCategoria]++;
                        skippedItems.push(item);
                    }
                }
                for (const item of filteredItems) {
                    item.Cantidad = categoryCounts[item.DescripcionCategoria];

                    if (item.ComunicacionTipo === 5) {
                        item.OtherIds = [];
                        for (let skipped of skippedItems) {
                            if (skipped.DescripcionCategoria === item.DescripcionCategoria) {
                                item.OtherIds.push(skipped.Id)
                            }
                        }
                    }

                    if (stackedTypes.includes(item.ComunicacionTipo)) {
                        item.OtherIds = [];
                        for (let skipped of skippedItemsByType) {
                            if (skipped.Type === item.ComunicacionTipo && skipped.Id !== item.Id) {
                                item.OtherIds.push(skipped);
                            }
                        }
                    }
                }

                if (filteredItems.length > 0) {
                    filteredCommunication[fecha] = filteredItems;
                }
            }

            this.communication = filteredCommunication;

            result.data.forEach((item: any) => {
                if (item.ComunicacionTipo === 1 && item.DescripcionWeb !== null) {
                    this.arrayVencidas.add(item.DescripcionWeb);
                }

                if (item.ComunicacionTipo === 2 && item.DescripcionWeb !== null) {
                    this.arrayProximasAVencer.add(item.DescripcionWeb);
                }

                if (item.ComunicacionTipo === 5) {
                    this.contadorConsultas++;
                }
            });

            this.arrayVencidas = Array.from(this.arrayVencidas);

            this.arrayProximasAVencer = Array.from(this.arrayProximasAVencer);

            const auxComunications = Object.keys(this.communication);
            this.quantityCommunication = 0;

            let i = 0;
            while (i < auxComunications.length) {
                const key = auxComunications[i];
                let contarExencionesVencidas = true;
                let contarExencionesAVencer = true;
                let contarConsultas = true;
                let contarliquidaciones = true;

                let j = 0;
                while (j < this.communication[key].length) {
                    const item = this.communication[key][j];

                    if (!item.Leida) {
                        if (item.ComunicacionTipo === 1 && item.FechaCreacion === key && contarExencionesVencidas) {
                            this.quantityCommunication++;
                            contarExencionesVencidas = false;
                        }
                        if (item.ComunicacionTipo === 2 && item.FechaCreacion === key && contarExencionesAVencer) {
                            this.quantityCommunication++;
                            contarExencionesAVencer = false;
                        }
                        if (item.ComunicacionTipo === 5 && item.FechaCreacion === key && contarConsultas) {
                            this.quantityCommunication++;
                            contarConsultas = false;
                        }
                        if (item.ComunicacionTipo === 6 && item.FechaCreacion === key && contarliquidaciones) {
                            this.quantityCommunication++;
                            contarliquidaciones = false;
                        }
                        if (![1, 2, 5, 6].includes(item.ComunicacionTipo)) {
                            this.quantityCommunication++;
                        }
                    }

                    j++;
                }

                i++;
            }

            this.layoutComponent.updateQuantity(this.quantityCommunication);

        } catch (error) {
            return false;
        }
        return false;
    }


  
  dateConvert() {
    var today = new Date();

    this.endDate = today.toLocaleDateString('en-US', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    })

    today.setMonth(today.getMonth() - 2);

    this.startDate = today.toLocaleDateString('en-US', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    })

    this.startDate = this.datePipe.transform(this.startDate , 'yyyy-MM-dd');
    this.endDate = this.datePipe.transform(this.endDate , 'yyyy-MM-dd');

  }
  
  dateFormat(dateString: string): String {

    const part = dateString.split(" ");
    const date = part[0];
    const hour = part[1]; 

    const [hh, mm, ss] = hour.split(":");

    let hour24 = hh;

    // La hora en formato de 24 horas
    const hour24Format = `${hour24}:${mm}`;

    return hour24Format
  }

    //MMSN-134: Added DescripciónCategoria as parameter for Communication Methods & StackedTypes-->
    openCommunication(notificaciones, fechacreacion, tipoComunicacion, i, idNotificacion, descripcionCategoria) {
        const ids = [];
        let stackedTypes = [1, 2, 6];
        this.filter = "";
        let proveedorDescripcion = sessionStorage.getItem("nombre");
        //MMSN-463 - ProveedorId necesario para comparaciones en consultas.
        let proveedorId = this.idProveedor;

        notificaciones.forEach((notificacion) => {
            if (notificacion.Leida == false && notificacion.ComunicacionTipo === tipoComunicacion) {
                if (tipoComunicacion === 5 && notificacion.DescripcionCategoria === descripcionCategoria) {
                    if (notificacion.OtherIds !== undefined && notificacion.OtherIds.length > 0) {
                        for (let stacked of notificacion.OtherIds) {
                            ids.push(stacked);
                        }
                        ids.push(notificacion.Id)
                    }
                    else {
                        ids.push(notificacion.Id)
                    }
                }
                else if (tipoComunicacion === 5 && notificacion.DescripcionCategoria !== descripcionCategoria) {
                    //Do not notify if the type is 5 and the description doesn't match
                }
                else if (stackedTypes.includes(tipoComunicacion)) {
                    if (notificacion.OtherIds !== undefined && notificacion.OtherIds.length > 0) {
                        for (let stacked of notificacion.OtherIds) {
                            ids.push(stacked.Id);
                        }
                    }
                    ids.push(notificacion.Id);
                }
                else {
                    ids.push(notificacion.Id);
                }
            }
            if (notificacion.ComunicacionTipo === 5 && notificacion.Id === idNotificacion && notificacion.DescripcionCategoria === descripcionCategoria) {
                this.filter = notificacion.DescripcionCategoria;
            }
        });

        if (ids.length)
            this.serviceComunicaciones.postComunicacionLeida(ids).subscribe();
        setTimeout(() => {
            this.serviceComunicaciones.getComunicaciones(this.idProveedor, this.startDate, this.endDate);
        }, 500);
        this.redirect(notificaciones[i].ComunicacionTipo, this.filter, proveedorDescripcion, proveedorId);
        this.cerrarComunicaciones.emit();
    }


    //MMSN-134: Check if there is stacked notifications -->
    unreadCommunication(notificacion) {
        const ids = [];
        if (notificacion.Leida == true) {
            ids.push(notificacion.Id)
            if (notificacion.OtherIds !== undefined && notificacion.OtherIds.length > 0) {
                for (let stacked of notificacion.OtherIds) {
                    if (notificacion.ComunicacionTipo !== 5) {
                        ids.push(stacked.Id);
                    }
                    else {
                        ids.push(stacked);
                    }
                }
            }
            this.serviceComunicaciones.postComunicacionNoLeida(ids).subscribe();

            setTimeout(() => {
                this.serviceComunicaciones.getComunicaciones(this.idProveedor, this.startDate, this.endDate);
            }, 500);
        }
    }

    redirect(communicationType: number, filter: string, nombreProveedor: string, idProveedor: string) {

        nombreProveedor = decodeURIComponent(nombreProveedor); 

    switch (communicationType) {
      case 1:
        this.router.navigate(['/consulta/crear-consulta'], { queryParams: { filter : 'ExencionesVencidas' }});
        break;

      case 2:
        this.router.navigate(['/consulta/crear-consulta'], { queryParams: { filter : 'ExencionesProximasAVencer' }});
        break;

      case 3:
        this.router.navigate(['/consulta/crear-consulta'], { queryParams: { filter : 'CM05' }});
        break;

      case 4:
        this.router.navigate(['/consulta/crear-consulta'], { queryParams: { filter: 'Actualizacion-Impositiva' }});
        break;
      
      case 5:
            this.router.navigate(['/consulta/mis-consultas'], { queryParams: { filtrosActivados: true, filter: 'DOC', categoria: filter, proveedor: nombreProveedor, idProveedor: idProveedor}});
        break;

      case 6:
        this.router.navigate(['/liquidacion/observada'], { queryParams: { filter: 'UltimosDosMeses' } })
        break;

      default:
        break;
    }
  }

  private checkCommunications() {
    setTimeout(() => {
        this.updateComunicacionService.updateCommunications(this.idProveedor, this.startDate, this.endDate);
        this.formatComunicaciones();
      this.checkCommunications();
    }, 30000);
  }

    // Función auxiliar para obtener las claves del objeto
    objectKeys(obj: any) {
      var retu = Object.keys(obj)
    return retu;
  }


}
