import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FaqService } from './faq.service';
import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
import { ListBaseComponent } from './../common/base-components/list-base-component'
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SessionDataService } from './../common/services/SessionDataService';
import { SecurityService } from './../common/services/SecurityService';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { Seccion } from './../common/models/seccion';
import { ModalService } from './../common/services/ModalService';
import { ReCaptchaComponent } from 'angular2-recaptcha';
import { element } from '@angular/core/src/render3/instructions';
import { e } from '@angular/core/src/render3';


declare var $: any;

@Component({
    selector: 'faq',
    templateUrl: `faq.component.html`,
    providers: [FaqService]

})
export class FaqComponent extends ListBaseComponent {

    @ViewChild('dropdown_categoria')
    protected categoriaDropdownComponent: DropdownComponent;

    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    @ViewChild('fileInput')
    protected fileInput: ElementRef;

    @ViewChild('dtp_fecha_pago')
    protected fechaPagoDTP: ElementRef;

    @ViewChild('recaptchaComponent')
    protected captcha: ReCaptchaComponent;

    constructor(protected service: FaqService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.isGranosSelected = sessionStorage.getItem("granosSelected");
        sessionDataService.granosSelected$.subscribe(
        granosSelected => {
            this.isGranosSelected = granosSelected;
        });
        this.categoriaDropdownComponent = new DropdownComponent();
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CONTACTO MAIL"); }

    isGranosSelected: string;
    x = true;
    proveedor: string;
    nombre: string;
    email: string;
    telefono: string;
    categoriaOptions: Array<DropdownOption> = [];
    inscripcion: string;
    motivo: string;
    file: any;
    camposAdicionales: boolean = false;
    fechaPagoDP: any;
    visibleButton: boolean = true;
    categoriaSelected: any;
    captchaOk: any = null;
    granosFlag: string;

    setTabs() {
        this.setMenuSeccionTab("Faq", "Faq");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);
        this.displayListaPreguntas();
    }

    isAuthorized(permiso: string) {
        return this.securityService.tienePermiso(permiso);
    }

    isGranos() {
        this.isGranosSelected = sessionStorage.getItem("granosSelected");
        this.granosFlag = sessionStorage.getItem("granosFlag");
        if (this.granosFlag == "A" || this.isGranosSelected == "G" )
            return true;
    }

    isNoGranos() {
        this.isGranosSelected = sessionStorage.getItem("granosSelected");
        this.granosFlag = sessionStorage.getItem("granosFlag");
        if (this.isGranosSelected == "N" || this.granosFlag == "A")
            return true;
    }


    displayListaPreguntas() {
        $('#1heading').on('click', async function(){
            await $("#heading1").click();
        });
        $('.faqCategoria1').on('click', async function(){
            await $("#1collapse").collapse("toggle");
        });
        $('#2heading').on('click', async function(){
            await $(".preguntasRespuesta2").collapse("toggle");
        });
        $('.faqCategoria2').on('click', async function(){
            await $(".preguntasMenu2").collapse("toggle");
        }); 
        $('#3heading').on('click', async function(){
            await $(".faqCategoria3").click();         
        });
        $('.faqCategoria3').on('click', async function(){
            await $("#3collapse").collapse("toggle");
        }); 
        $('#4heading').on('click', async function(){
            await $("#heading4").click();         
        });
        $('#heading4').on('click', async function(){
            await $("#4collapse").collapse("toggle");
        }); 
        $('#5heading').on('click', async function(){
            await $("#heading5").click();         
        });
        $('#heading5').on('click', async function(){
            await $("#5collapse").collapse("toggle");
        }); 
        $('#6heading').on('click', async function(){
            await $("#heading6").click();         
        });
        $('#heading6').on('click', async function(){
            await $("#6collapse").collapse("toggle");
        }); 
        $('#7heading').on('click', async function(){
            await $("#heading7").click();         
        });
        $('#heading7').on('click', async function(){
            await $("#7collapse").collapse("toggle");
        }); 
        $('#8heading').on('click', async function(){
            await $("#heading8").click();         
        });
        $('#heading8').on('click', async function(){
            await $("#8collapse").collapse("toggle");
        }); 
        $('#9heading').on('click', async function(){
            await $("#heading9").click();         
        });
        $('#heading9').on('click', async function(){
            await $("#9collapse").collapse("toggle");
        }); 
        $('#10heading').on('click', async function(){
            await $("#heading10").click();         
        });
        $('#heading10').on('click', async function(){
            await $("#10collapse").collapse("toggle");
        }); 
        $('#11heading').on('click', async function(){
            await $("#heading11").click();         
        });
        $('#heading11').on('click', async function(){
            await $("#11collapse").collapse("toggle");
        }); 
        $('#12heading').on('click', async function(){
            await $("#heading12").click();         
        });
        $('#heading12').on('click', async function(){
            await $("#12collapse").collapse("toggle");
        }); 

        $('.panel-collapse').on('shown.bs.collapse', function (e) {
            var $panel = $(this).closest('.panel');
            $('html,body').animate({
                scrollTop: $panel.offset().top - 200
            }, 100); 
        }); 
    }
}