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
        this.categoriaDropdownComponent = new DropdownComponent();
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CONTACTO MAIL"); }

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

    setTabs() {
        this.setMenuSeccionTab("Faq", "Faq");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);
        this.displayListaPreguntas();
    }

    async displayListaPreguntas() {

        //WEB
        $('.categoria1MenuFaq').on('click', function(e){
            var $link = $(e.target);
            e.preventDefault();
            if(!$link.data('lockedAt') || +new Date() - $link.data('lockedAt') > 300) {
                $("#archive_posts").slideToggle();
            }
            $link.data('lockedAt', +new Date());
        });
        $('.faqCategoria1').on('click', function(o){
            var $ok = $(o.target);
            o.preventDefault();
            if(!$ok.data('lockedAt') || +new Date() - $ok.data('lockedAt') > 300) {
                $("#archive_posts").slideToggle();
            }
            $ok.data('lockedAt', +new Date());
        });

        $('.categoria2MenuFaq').on('click', function(e){
            var $link = $(e.target);
            e.preventDefault();
            if(!$link.data('lockedAt') || +new Date() - $link.data('lockedAt') > 300) {
                $("#archive_posts2").slideToggle();
            }
            $link.data('lockedAt', +new Date());
        });
        $('.faqCategoria2').on('click', function(o){
            var $ok = $(o.target);
            o.preventDefault();
            if(!$ok.data('lockedAt') || +new Date() - $ok.data('lockedAt') > 300) {
                $("#archive_posts2").slideToggle();
            }
            $ok.data('lockedAt', +new Date());
        });
        $('.categoria3MenuFaq').on('click', function(e){
            var $link = $(e.target);
            e.preventDefault();
            if(!$link.data('lockedAt') || +new Date() - $link.data('lockedAt') > 300) {
                $("#archive_posts3").slideToggle();
            }
            $link.data('lockedAt', +new Date());
        });
        $('.faqCategoria3').on('click', function(o){
            var $ok = $(o.target);
            o.preventDefault();
            if(!$ok.data('lockedAt') || +new Date() - $ok.data('lockedAt') > 300) {
                $("#archive_posts3").slideToggle();
            }
            $ok.data('lockedAt', +new Date());
        });
        $('.categoria4MenuFaq').on('click', function(e){
            var $link = $(e.target);
            e.preventDefault();
            if(!$link.data('lockedAt') || +new Date() - $link.data('lockedAt') > 300) {
                $("#archive_posts4").slideToggle();
            }
            $link.data('lockedAt', +new Date());
        });
        $('.faqCategoria4').on('click', function(o){
            var $ok = $(o.target);
            o.preventDefault();
            if(!$ok.data('lockedAt') || +new Date() - $ok.data('lockedAt') > 300) {
                $("#archive_posts4").slideToggle();
            }
            $ok.data('lockedAt', +new Date());
        });
        $('.categoria5MenuFaq').on('click', function(e){
            var $link = $(e.target);
            e.preventDefault();
            if(!$link.data('lockedAt') || +new Date() - $link.data('lockedAt') > 300) {
                $("#archive_posts5").slideToggle();
            }
            $link.data('lockedAt', +new Date());
        });
        $('.faqCategoria5').on('click', function(o){
            var $ok = $(o.target);
            o.preventDefault();
            if(!$ok.data('lockedAt') || +new Date() - $ok.data('lockedAt') > 300) {
                $("#archive_posts5").slideToggle();
            }
            $ok.data('lockedAt', +new Date());
        });
        $('.categoria6MenuFaq').on('click', function(e){
            var $link = $(e.target);
            e.preventDefault();
            if(!$link.data('lockedAt') || +new Date() - $link.data('lockedAt') > 300) {
                $("#archive_posts6").slideToggle();
            }
            $link.data('lockedAt', +new Date());
        });
        $('.faqCategoria6').on('click', function(o){
            var $ok = $(o.target);
            o.preventDefault();
            if(!$ok.data('lockedAt') || +new Date() - $ok.data('lockedAt') > 300) {
                $("#archive_posts6").slideToggle();
            }
            $ok.data('lockedAt', +new Date());
        });
        $('.categoria7MenuFaq').on('click', function(e){
            var $link = $(e.target);
            e.preventDefault();
            if(!$link.data('lockedAt') || +new Date() - $link.data('lockedAt') > 300) {
                $("#archive_posts7").slideToggle();
            }
            $link.data('lockedAt', +new Date());
        });
        $('.faqCategoria7').on('click', function(o){
            var $ok = $(o.target);
            o.preventDefault();
            if(!$ok.data('lockedAt') || +new Date() - $ok.data('lockedAt') > 300) {
                $("#archive_posts7").slideToggle();
            }
            $ok.data('lockedAt', +new Date());
        });
        $('.categoria8MenuFaq').on('click', function(e){
            var $link = $(e.target);
            e.preventDefault();
            if(!$link.data('lockedAt') || +new Date() - $link.data('lockedAt') > 300) {
                $("#archive_posts8").slideToggle();
            }
            $link.data('lockedAt', +new Date());
        });
        $('.faqCategoria8').on('click', function(o){
            var $ok = $(o.target);
            o.preventDefault();
            if(!$ok.data('lockedAt') || +new Date() - $ok.data('lockedAt') > 300) {
                $("#archive_posts8").slideToggle();
            }
            $ok.data('lockedAt', +new Date());
        });
        $('.categoria9MenuFaq').on('click', function(e){
            var $link = $(e.target);
            e.preventDefault();
            if(!$link.data('lockedAt') || +new Date() - $link.data('lockedAt') > 300) {
                $("#archive_posts9").slideToggle();
            }
            $link.data('lockedAt', +new Date());
        });
        $('.faqCategoria9').on('click', function(o){
            var $ok = $(o.target);
            o.preventDefault();
            if(!$ok.data('lockedAt') || +new Date() - $ok.data('lockedAt') > 300) {
                $("#archive_posts9").slideToggle();
            }
            $ok.data('lockedAt', +new Date());
        });
        $('.categoria10MenuFaq').on('click', function(e){
            var $link = $(e.target);
            e.preventDefault();
            if(!$link.data('lockedAt') || +new Date() - $link.data('lockedAt') > 300) {
                $("#archive_posts10").slideToggle();
            }
            $link.data('lockedAt', +new Date());
        });
        $('.faqCategoria10').on('click', function(o){
            var $ok = $(o.target);
            o.preventDefault();
            if(!$ok.data('lockedAt') || +new Date() - $ok.data('lockedAt') > 300) {
                $("#archive_posts10").slideToggle();
            }
            $ok.data('lockedAt', +new Date());
        });
        $('.categoria11MenuFaq').on('click', function(e){
            var $link = $(e.target);
            e.preventDefault();
            if(!$link.data('lockedAt') || +new Date() - $link.data('lockedAt') > 300) {
                $("#archive_posts11").slideToggle();
            }
            $link.data('lockedAt', +new Date());
        });
        $('.faqCategoria11').on('click', function(o){
            var $ok = $(o.target);
            o.preventDefault();
            if(!$ok.data('lockedAt') || +new Date() - $ok.data('lockedAt') > 300) {
                $("#archive_posts11").slideToggle();
            }
            $ok.data('lockedAt', +new Date());
        });
    }
    
}