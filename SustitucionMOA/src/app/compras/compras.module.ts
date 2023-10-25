import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { CalendarModule } from 'primeng/calendar';
import { DialogModule } from 'primeng/dialog';
import { EditorModule } from 'primeng/editor';
import { SidebarModule } from 'primeng/sidebar';
import { CheckboxModule } from 'primeng/checkbox';
import { KeyFilterModule } from 'primeng/keyfilter';
import { FileUploadModule } from 'primeng/fileupload';
import { RadioButtonModule } from 'primeng/radiobutton';
import { DropdownModule } from 'primeng/dropdown';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { ToastModule } from 'primeng/toast';
import { ChipsModule } from 'primeng/chips';
import { TableModule } from 'primeng/table';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService, SortEvent } from 'primeng/api';
import { MultiSelectModule } from 'primeng/multiselect';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { PanelModule } from 'primeng/panel';
import { InputSwitchModule } from 'primeng/inputswitch';
import { TabMenuModule } from 'primeng/tabmenu';
import { QuillModule } from 'ngx-quill'

import { SharedModule } from '../common/shared.module';
import { ComprasRoutingModule } from './compras-routing.module';
import { ComprasService } from './compras.service';
import { Generacion2Component } from './solp/steps/generacion2/generacion2.component';
import { Generacion1Component } from './solp/steps/generacion1/generacion1.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { SolpComponent } from './solp/solp.component';
import { PliegoPreviewComponent } from './preview/pliego.preview.component';
import { DragAndDropDirective } from '../directivas/drag-and-drop.directive';
import { EspecificacionesComponent } from './solp/steps/especificaciones/especificaciones.component'
import { CabeceraComponent } from './solp/steps/posicion/cabecera.component';
import { ValidadorPasoSolpService } from './validadorPasoSolpService';
import { TabFechasComponent } from './solp/steps/posicion/tab-fechas/tab-fechas.component';
import { TabDatosPosicionComponent } from './solp/steps/posicion/tab-datos-posicion/tab-datos-posicion.component';
import { TabDireccionEntregaComponent } from './solp/steps/posicion/tab-direccion-entrega/tab-direccion-entrega.component';
import { TabImputacionesComponent } from './solp/steps/posicion/tab-imputaciones/tab-imputaciones.component';
import { TabProveedoresComponent } from './solp/steps/posicion/tab-proveedores/tab-proveedores.component';
import { TabSubposicionComponent } from './solp/steps/posicion/tab-subposicion/tab-subposicion.component';
import { StepperActionsComponent } from './solp/stepper-actions/stepper-actions.component';
import { FinalizarSolpComponent } from './solp/finalizar/finalizar-solp.component';
import { ContratoMarcoComponent } from './solp/steps/posicion/contrato-marco/contrato-marco.component';
import { ObtenerContratoMarcoComponent } from './solp/steps/posicion/obtener-contrato-marco/obtener-contrato-marco.component';
import { PaginatorModule } from 'primeng/paginator';
import { TabViewModule } from 'primeng/tabview';
import {TooltipModule} from 'primeng/tooltip';
import { ListadoDashboardCompradorComponent } from './dashboard-comprador/listado-dashboard-comprador/listado-dashboard-comprador.component';
import { CircularComponent } from './dashboard-comprador/circular/circular.component';
import { LegajoComponent } from './dashboard-comprador/legajo/legajo.component';
import { NgxMaskModule } from "ngx-mask";
import { NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';

import { ProveedorPeticionComponent } from './dashboard-comprador/proveedor-peticion/proveedor-peticion.component';
import { AltaProveedorComponent } from './dashboard-comprador/alta-proveedor/alta-proveedor.component';
import { ListadoDashboardProveedorComponent } from './dashboard-proveedor/listado-dashboard-proveedor/listado-dashboard-proveedor.component';
import { VerOfertasComponent } from './dashboard-comprador/ver-ofertas/ver-ofertas.component';
import { RevisionTecnicaComponent } from './dashboard/revision-tecnica/revision-tecnica.component';

import { PeticionDeOfertaFormularioComponent } from './peticion-de-oferta-formulario/peticion-de-oferta-formulario.component';
import { CotizacionFormularioComponent } from './dashboard-proveedor/cotizacion-formulario/cotizacion-formulario.component';
import { CotizacionComponent } from './solp/steps/cotizacion/cotizacion.component';
import { CotizacionMaterialComponent } from './dashboard-proveedor/cotizacion-formulario/cotizacion-material/cotizacion-material.component';
import { CotizacionServicioComponent } from './dashboard-proveedor/cotizacion-formulario/cotizacion-servicio/cotizacion-servicio.component';
import { PanelHorasComponent } from './panel-horas/panel-horas.component';
import { OrdenDeCompraDetalleComponent } from './dashboard-comprador/orden-de-compra-detalle/ordenDeCompraDetalle.component';
import { TextosAdjudicarComponent } from './dashboard-comprador/ver-ofertas/textos-adjudicar/textos-adjudicar.component';

import {AccordionModule} from 'primeng/accordion';
import { RegistroInfoComponent } from './peticion-de-oferta-formulario/registro-info/registro-info/registro-info.component';
import { CerrarCotizacionComponent } from './dashboard-comprador/cerrar-cotizacion/cerrar-cotizacion.component';
import { PlazoDeOfertaComponent } from './dashboard-proveedor/cotizacion-formulario/cotizacion-material/plazo-de-oferta/plazo-de-oferta.component';
import { VisalizarPrecioComponent } from './dashboard-comprador/ver-ofertas/visualizar-precio/visualizar-precio.component';
import { ReporteOcComponent } from './reporte-oc/reporte-oc.component';

@NgModule({
    imports: [
        CommonModule,
        ComprasRoutingModule,
        FormsModule,
        SharedModule,
        InputTextModule,
        CalendarModule,
        DialogModule,
        EditorModule,
        SidebarModule,
        CheckboxModule,
        KeyFilterModule,
        FileUploadModule,
        QuillModule.forRoot(),
        MessagesModule,
        MessageModule,
        ToastModule,
        RadioButtonModule,
        DropdownModule,
        AutoCompleteModule,
        ReactiveFormsModule,
        ChipsModule,
        MultiSelectModule,
        OverlayPanelModule,
        TableModule,
        ConfirmDialogModule,
        PanelModule,
        InputSwitchModule,
        TabMenuModule,
        PaginatorModule,
        TooltipModule,
        TabViewModule,
        NgbAlertModule,
        NgxMaskModule,
        AccordionModule
    ],
    declarations: [
        SolpComponent,
        CotizacionComponent,
        DashboardComponent,
        Generacion1Component,
        Generacion2Component,
        PliegoPreviewComponent,
        EspecificacionesComponent,
        DragAndDropDirective,
        CabeceraComponent,
        TabFechasComponent,
        TabDatosPosicionComponent,
        TabDireccionEntregaComponent,
        TabProveedoresComponent,
        TabSubposicionComponent,
        TabImputacionesComponent,
        StepperActionsComponent,
        FinalizarSolpComponent,
        ContratoMarcoComponent,
        ObtenerContratoMarcoComponent,
        ListadoDashboardCompradorComponent,
        PeticionDeOfertaFormularioComponent,
        LegajoComponent,
        CircularComponent,
        AltaProveedorComponent,
        ProveedorPeticionComponent,
        ListadoDashboardProveedorComponent,
        VerOfertasComponent,
        RevisionTecnicaComponent,
        PeticionDeOfertaFormularioComponent,
        CotizacionFormularioComponent,
        CotizacionMaterialComponent,
        CotizacionServicioComponent,
        PanelHorasComponent,
        OrdenDeCompraDetalleComponent,
        TextosAdjudicarComponent,
        RegistroInfoComponent,
        CerrarCotizacionComponent,
        PlazoDeOfertaComponent,
        ReporteOcComponent,
        VisalizarPrecioComponent
    ],
    providers: [
        ComprasService,
        ValidadorPasoSolpService,
        ConfirmationService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ComprasModule { }
