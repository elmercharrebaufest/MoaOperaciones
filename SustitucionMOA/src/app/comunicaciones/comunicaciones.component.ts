import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '../common/base-components/base-component';
import { NavService } from '../common/services/NavService';
import { SecurityService } from '../common/services/SecurityService';
import { FloatMsgService } from '../common/services/FloatMsgService';
import { ModalService } from '../common/services/ModalService';
import { ComunicacionesService } from './comunicaciones.service';
import { SessionDataService } from '../common/services/SessionDataService';
import { LayoutComponent } from '../layout/layout.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-comunicaciones',
  templateUrl: './comunicaciones.component.html',
  styleUrls: ['./comunicaciones.component.css'],
  providers: [DatePipe]
})
export class ComunicacionesComponent extends BaseComponent implements OnInit {
  
  communication: any;
  idProveedor: string;
  communicationRead: boolean[] = [];
  arrayVencidas: any;
  arrayProximasAVencer: any;
  quantityCommunication: number = 0;
  hasCommunications: boolean = false;
  showButtonMoreCommunications: boolean = false;

  startDate: String;
  endDate: String;

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
    ) {
    super(navService, securytiService, floatMsgService, modalService);

    this.idProveedor = sessionStorage.getItem("proveedor");

    sessionDataService.proveedor$.subscribe(
      proveedor => {
          this.idProveedor = proveedor;
      });
  }

  ngOnInit() {
    this.dateConvert();
    this.getComunicaciones(this.idProveedor, this.startDate, this.endDate);
    this.checkCommunications();
  }

  getComunicaciones(idProveedor, start_date, end_date) {
    this.communicationRead = [];
    this.quantityCommunication = 0;
    this.arrayVencidas = new Set();
    this.arrayProximasAVencer = new Set();
  
    try {
      this.unsubscribe();
      this.subscription = this.serviceComunicaciones.getComunicaciones(idProveedor, start_date, end_date).subscribe(
        (result: any) => {
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

          const filteredCommunication = {};

          for (const fecha in this.communication) {
            const items = this.communication[fecha];
            const filteredItems = [];
          
            let tipo1Found = false;
            let tipo2Found = false;
          
            for (const item of items) {
              if (item.ComunicacionTipo === 1 && !tipo1Found) {
                filteredItems.push(item);
                tipo1Found = true;
              } else if (item.ComunicacionTipo === 2 && !tipo2Found) {
                filteredItems.push(item);
                tipo2Found = true;
              } else if (item.ComunicacionTipo !== 1 && item.ComunicacionTipo !== 2) {
                filteredItems.push(item);
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
          });
          
          this.arrayVencidas = Array.from(this.arrayVencidas);

          result.data.forEach((item: any) => {
            if (item.ComunicacionTipo === 2 && item.DescripcionWeb !== null) {
              this.arrayProximasAVencer.add(item.DescripcionWeb);
            }
          });
          
          this.arrayProximasAVencer = Array.from(this.arrayProximasAVencer);

          const auxComunications = Object.keys(this.communication);

          auxComunications.forEach((key) => {
            let contar = true;
            this.communication[key].forEach((item, i) => {
              if (item.Leida === false) {
                if (item.ComunicacionTipo === (1 || 2 || 6) && item.FechaCreacion === key && contar) {
                  this.quantityCommunication++
                  contar = false;
                }
                if (item.ComunicacionTipo !== (1 || 2 || 6))
                  this.quantityCommunication++
                }
            })  
          });

          this.layoutComponent.updateQuantity(this.quantityCommunication);
        }
      );
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

  openCommunication(notificaciones, fechacreacion, tipoComunicacion,i) {
    const ids = [];

    notificaciones.forEach((notificacion) => {
      if (notificacion.Leida == false && notificacion.ComunicacionTipo === tipoComunicacion)
        ids.push(notificacion.Id);
    }); 

    if (ids.length)
      this.serviceComunicaciones.postComunicacionLeida(ids).subscribe();
    setTimeout(() => {
      this.getComunicaciones(this.idProveedor, this.startDate, this.endDate);
    }, 500);
  
    this.redirect(notificaciones[i].ComunicacionTipo);
  }

  unreadCommunication(notificacion) {
    if (notificacion.Leida == true) {
      this.serviceComunicaciones.postComunicacionNoLeida(notificacion.Id).subscribe();

      setTimeout(() => {
        this.getComunicaciones(this.idProveedor, this.startDate, this.endDate);
      }, 500);
    }
  }

  redirect(communicationType: number) {
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

      case 6:
        this.router.navigate(['/liquidacion/observada'], { queryParams: { filter: 'UltimosDosMeses' } })
        break;

      default:
        break;
    }
  }

  private checkCommunications() {
    setTimeout(() => {
      this.getComunicaciones(this.idProveedor, this.startDate, this.endDate);
      this.checkCommunications();
    }, 15000);
  }

    // Función auxiliar para obtener las claves del objeto
    objectKeys(obj: any) {
      var retu = Object.keys(obj)
    return retu;
  }
}
