import { ModalNotificacionesComponent } from './../modal-notificaciones/modal-notificaciones.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../../common/base-components/base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { NotificacionesService } from '../notificaciones.service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-carousel-notificaciones',
  templateUrl: './carousel-notificaciones.component.html',
  styleUrls: ['./carousel-notificaciones.component.css']
})
export class CarouselNotificacionesComponent extends BaseComponent implements OnInit {

  @ViewChild("myModal") modal: ModalNotificacionesComponent;

  data: any;
  newsEnabled: number = 0;
  newCurrent = [];
  attachedCurrent = [];
  showNews: boolean = false;
  showNextButton: boolean = false;
  showPreviousButton: boolean = false;
  pointsArray: any[] = [];
  selectedPoint: number = 0;
  enableIndividualNews = false;
  notificacionLeida: boolean[] = [];
  nombre: string;
  notificacionModal: any;
  adjuntoModal = [];
  mostarModal: boolean = false;

  currentPage = 0;
  itemsPerPage = 3;
  pages: number = 0;
  contadorNotificaciones: number = 3;

  imagenBase64: string; 
  imagenSegura: SafeUrl;
  imagenPrevisualizacion: boolean = false;

  totalPages: number = 0;

  private timerInterval: any;

  constructor(protected service: NotificacionesService, protected navService: NavService,
    protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
    protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    private sanitizer: DomSanitizer) {
    super(navService, securytiService, floatMsgService, modalService);

    this.nombre = sessionStorage.getItem("nombre");

    sessionDataService.nombre$.subscribe(
      nombre => {
          this.nombre = nombre;
      });
  }

  ngOnInit(): void {
    this.navService.setSeccionList([]);
    this.getNotificaciones();
  }

  getNotificaciones() {
    this.data = null;
    this.newsEnabled = 0;
    this.imagenPrevisualizacion = false;

    try {
      this.unsubscribe();
        this.subscription = this.service.getNotificaciones().subscribe(
        (result:any) => {
          if (result.logout == true) {
            this.sessionDataService.logout();
          } else if (result.error != undefined && result.error != "") {
          } else if (result.info != undefined) {
          } else {
            this.data = result.data;
            if (this.data.length > 0) {
              this.showNews = true;
              this.notificacionLeida = [];
              this.data.forEach((notificacion: any) => {
                this.newCurrent.push(notificacion);

                if (notificacion.Leida == 1) {
                  this.notificacionLeida[notificacion.Id] = true;
                } else {
                  this.notificacionLeida[notificacion.Id] = false;
                }
                if(notificacion.Habilitada == true) {
                  this.newsEnabled++;
                }

                notificacion.ArchivosAdjuntos.forEach((adjunto: any) => {
                  this.attachedCurrent.push(adjunto);
                  
                  if(adjunto.AdjuntoTipo == 'previsualizacion') {
                    this.imagenPrevisualizacion = true;
                  }
                })     
              });
            }
          }
          this.actualizarNotificaciones();
          if(this.data.length > 3) {
            this.onMouseLeave();
          }
        },
        error => {
        }
      );
    } catch (e) {
      return false; //<-- Prevent Refresh
    }
    return false; //<-- Prevent Refresh
  }


  getNotificacion(notificacionId: number) {
    try {
      this.unsubscribe();
        this.subscription = this.service.getNotificacion(notificacionId).subscribe(
        (result:any) => {
          this.notificacionModal = result.data;
        },
        error => {
        }
      );
    } catch (e) {
      return false; //<-- Prevent Refresh
    }
    return false; //<-- Prevent Refresh
  }

  convertImage(base64:string): SafeUrl {
    this.imagenBase64 = this.decodeBase64Image(base64);
    this.imagenSegura = this.sanitizer.bypassSecurityTrustUrl(this.imagenBase64);
    return this.imagenSegura
  }

  decodeBase64Image(base64: string): string {
    return 'data:image/png;base64,' + base64;
  }

    actualizarNotificaciones() {        
    const inicio = this.currentPage * this.itemsPerPage;

    const fin = inicio + this.itemsPerPage;
        this.showPreviousButton = this.currentPage != 0;
        this.showNextButton = this.currentPage < this.totalPages - 1 || (this.itemsPerPage > 0 && this.currentPage == 0 && this.newsEnabled > 3);        
    this.newCurrent = this.data.slice(inicio, fin);

    this.calculatePoints();

    if (this.newCurrent.length == 1) {
      this.enableIndividualNews = true;
    }
    else this.enableIndividualNews = false
  }

    calculatePoints() {        
    this.totalPages = Math.ceil(this.newsEnabled / 3);
    this.pointsArray = new Array(this.totalPages).fill(null);
  }

  previousNews() {
    this.currentPage--;
    this.contadorNotificaciones = this.contadorNotificaciones - 3;
    this.selectedPoint--;
    this.actualizarNotificaciones();
  }
  
  nextNews() {      
    this.currentPage++;
    this.contadorNotificaciones = this.contadorNotificaciones + 3;
    this.selectedPoint++;
    this.actualizarNotificaciones();
  }

  get startIndex(): number {
    return (this.currentPage - 1) * this.itemsPerPage;
  }

  get endIndex(): number {
    return this.startIndex + this.itemsPerPage;
  }

  changePage(selectedPoint: number) {
    this.selectedPoint = selectedPoint
    this.currentPage = selectedPoint;
    this.actualizarNotificaciones();
  }

  openModalNovedad(notificacionId: number) {
    this.notificacionModal = null;
  
    try {
      this.service.postNotificacionLeida(notificacionId).subscribe(
        (result: any) => {
          if (result.logout == true) {
            this.sessionDataService.logout();
          } else if (result.error != undefined && result.error != "") {
          } else if (result.info != undefined) {
          } else {
            this.getNotificacion(notificacionId);
            this.mostarModal = true;
            setTimeout(() => {
              if (this.notificacionModal != null) {
                if (this.notificacionModal.Leida == 1) {
                  this.notificacionLeida[this.notificacionModal.Id] = true;
                } else {
                  this.notificacionLeida[this.notificacionModal.Id] = false;
                }
    
                this.notificacionModal.ArchivosAdjuntos.forEach((adjunto: any) => { 
                  if(adjunto.AdjuntoTipo != 'previsualizacion') {
                    this.adjuntoModal.push(adjunto);
                  }
                })  
                this.modal.mostrarModal();
              }    
            }, 500);
          }
          
        },
        error => {
        }
      );
    } catch (e) {
      return false; //<-- Prevent Refresh
    }
    return false; //<-- Prevent Refresh
  }

  onMouseEnter() {
    try {
      if (this.data.length > 0) 
      clearInterval(this.timerInterval);
    }
    catch {}
  }

  onMouseLeave() {
    clearInterval(this.timerInterval); 
    let previousPage;
    try {
      this.timerInterval = setInterval(() => {
        this.currentPage = (this.currentPage + 1) % this.totalPages;
        
        if (this.currentPage === 0 && previousPage !== 0) {
          this.contadorNotificaciones = 3;
        }
        else {
          this.contadorNotificaciones = this.contadorNotificaciones + 3;
        }
        previousPage = this.currentPage;
        this.selectedPoint = this.currentPage
        this.actualizarNotificaciones();
      }, 7000);
    }
    catch {}
  }
}
