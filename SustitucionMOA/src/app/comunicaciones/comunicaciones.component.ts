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
import { forEach } from '@angular/router/src/utils/collection';


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
  arrayFecha: any;
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
    this.arrayFecha = new Set<string>();
  
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



              //const arrayFiltrado = result.data.filter(element => element.ComunicacionTipo === 6);

              const agrupadoPorFecha = result.data.reduce((result, element) => {
                  const fechaCreacion = element.FechaCreacion;

                  // Verificar si ya existe una entrada en el resultado para esta fecha
                  if (!result[fechaCreacion]) {
                      result[fechaCreacion] = [];
                  }

                  // Agregar el elemento al grupo de la fecha correspondiente
                  result[fechaCreacion].push(element);

                  return result;
              }, {});

              this.communication = agrupadoPorFecha;

  
          //this.communication = result.data.filter((item: any) => {
          //  if (item.ComunicacionTipo === 6) {
          //    if (!this.arrayFecha.has(item.FechaCreacion)) {
                
          //      this.arrayFecha.add(item.FechaCreacion);
          //      return true; 
          //    }
          //    return false;
          //  } else {
          //    return true;
          //  }
          //});

              console.log(this.communication);
              console.log(this.arrayFecha);


              

         


              const auxComunications = Object.keys(this.communication);

              auxComunications.forEach((key) => {

                  let contar = true;
                  this.communication[key].forEach((item, i) => {
                      
                       
                      if (item.Leida === false) {
                        

                          if (item.ComunicacionTipo === 6 && item.FechaCreacion === key && contar) {
                              this.quantityCommunication++
                              contar = false;
                          }

                          if (item.ComunicacionTipo !== 6)
                               this.quantityCommunication++

                      }
                          
                  })  
                  
              });


              



          //    const unreadCommunications = this.communication.filter((item: any) =>
          //        item.Leida === false
          //    );

          //this.quantityCommunication = unreadCommunications.length;
              //this.layoutComponent.updateQuantity(23);

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

    // Verifica si tiene a.m o p.m la fecha
    //if (!dateString.includes("a. m.") && !dateString.includes("p. m.")) {
    //  // Obtener las horas del dateString
    //  const [hh] = dateString.split(":");
    //  const hour24 = parseInt(hh);
  
    //  // Determinar si es "AM" o "PM" y actualizar 'dateString'
    //  if (hour24 >= 12) {
    //    dateString += "p. m.";
    //  } else {
    //    dateString += "a. m.";
    //  }
    //}

    const part = dateString.split(" ");
    const date = part[0]; // "09/01/2023"
    const hour = part[1];  // "12:00:00"
    //const ampm = part[2]; // a.m p.m

    const [hh, mm, ss] = hour.split(":");

    let hour24 = hh;

    //if (ampm.toLowerCase() === "p.") {
    //  // Si es PM, agrega 12 a la hora (excepto a las 12 PM)
    //  if (hh !== "12") {
    //    hour24 = String(Number(hh) + 12);
    //  }
    //} else if (ampm.toLowerCase() === "a.") {
    //  // Si es AM y la hora es 12 AM, cambia la hora a 00
    //  if (hh === "12") {
    //    hour24 = "00";
    //  }
    //}

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
