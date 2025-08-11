import { Component, OnInit, ViewChild, Output, EventEmitter, OnDestroy } from '@angular/core';
import { UsuarioService } from '../usuario.service';
import { FormGroup, FormBuilder, FormArray, FormControl, Validators } from '@angular/forms';
import { Subscription, forkJoin } from 'rxjs';
import { debounceTime, filter, map, } from 'rxjs/operators';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { MessageService } from 'primeng/api';
import { Proveedor } from '../../common/models/proveedor';
import { ProveedorARelacionar } from '../../common/models/usuario/proveedorARelacionar';

@Component({
    selector: 'app-modificar-datos',
    templateUrl: './modificar-datos.component.html',
    styleUrls: ['./modificar-datos.component.css'],
    providers: [MessageService]
})
export class ModificarDatosComponent implements OnInit, OnDestroy {

    tipoUsuario: any = [];
    existeProveedores: boolean = true;
    modificarDatosForm: FormGroup;
    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;
    @Output() cerrarModal = new EventEmitter();

    subscriptions = new Subscription();

    constructor(protected service: UsuarioService,
        private formBuilder: FormBuilder,
        private messageService: MessageService) {
        this.spinnerComponent = new SpinnerComponent();
        this.service.getUsuarioModificarDatos().subscribe(data => {
            this.modificarDatosForm = this.inicializarFormDatosUsuario();
            if (data != null && data > 0)
                this.cargarDatosUsuario(data.toString());
        });
    }

    ngOnInit() {
        this.cargarTipoUsuario();
        this.asignarFormSubscriptions()
    }

    get proveedoresFormArray(): FormArray {
        return this.modificarDatosForm.get("proveedores") as FormArray
    }

    private cargarTipoUsuario() {
        this.service.getTipoUsuario().subscribe(response => {
            this.tipoUsuario = response.data.tipoUsuario.filter(u => u.NombreCorto !== "A");
        });
    }
    private cargarDatosUsuario(id: string) {
        this.spinnerComponent.showIt();
        forkJoin([this.service.getUsuarioPorId(id)]).pipe(
            map(([datosUsuario]) => {
                this.cargarFormDatosUsuario(datosUsuario.data.usuario);
            })
        ).subscribe();
    }

    private inicializarFormDatosUsuario(): FormGroup {
        return this.formBuilder.group({
            id: [0],
            mail: [''],
            cuit: [''],
            usuarioModificacion: [''],
            idTipoUsuario: [0],
            organizacionDeCompra: [''],
            proveedores: this.formBuilder.array([]),
        });
    }
    public trackByFn(index: any, item: any) {
        return index;
    }
    private cargarFormDatosUsuario(usuario: any) {
        let usuarioModificacion: string = sessionStorage.getItem("username");
        this.modificarDatosForm.controls['id'].setValue(usuario.Id);
        this.modificarDatosForm.controls['mail'].setValue(usuario.Mail);
        this.modificarDatosForm.controls['cuit'].setValue(usuario.CUIT);
        this.modificarDatosForm.controls['usuarioModificacion'].setValue(usuarioModificacion);
        this.modificarDatosForm.controls['idTipoUsuario'].setValue(usuario.TipoUsuario.Id);
        this.modificarDatosForm.controls['organizacionDeCompra'].setValue(usuario.OrganizacionDeCompra);
        let listaProveedores: Proveedor[];
        this.existeProveedores = true;
        this.service.getProvedoresEmail(usuario.TipoUsuario.Id, usuario.Mail, usuario.CUIT).subscribe(proveedores => {
            listaProveedores = proveedores.data.proveedores;
        }, error => { }, () => {
            if (listaProveedores.length == 0) this.existeProveedores = false;
            this.spinnerComponent.hideIt();
            this.cargarFormDatoProveedores(listaProveedores);
        });

    }
    private cargarFormDatoProveedores(proveedores: Proveedor[]) {
        proveedores.forEach(proveedor => {
            this.proveedoresFormArray.push(this.inicializarFormProveedor(proveedor));
        });
    }
    private crearObjectoModificarUsuario() {
        let modificarDatos = this.modificarDatosForm;
        let formProveedores = this.modificarDatosForm.get('proveedores')['controls'];
        let proveedores = [];
        for (let index = 0; index < formProveedores.length; index++) {
            let proveedor = formProveedores[index].controls;
            proveedores.push({
                codigoProveedor: proveedor.codigoProveedor.value,
                cuit: proveedor.cuit.value,
                id: proveedor.id.value,
                idTipoProveedor: proveedor.idTipoProveedor.value,
                razonSocial: proveedor.razonSocial.value,
                esRevendedor: proveedor.esRevendedor.value
            });
        }
        let modificarUsuario = {
            id: modificarDatos.controls["id"].value,
            mail: modificarDatos.controls["mail"].value,
            cuit: modificarDatos.controls["cuit"].value,
            usuarioModificacion: modificarDatos.controls["usuarioModificacion"].value,
            idTipoUsuario: modificarDatos.controls["idTipoUsuario"].value,
            organizacionDeCompra: modificarDatos.controls["organizacionDeCompra"].value,
            proveedores: proveedores,
        }
        return modificarUsuario;
    }
    public onActualizaCuitProveedor() {
        const cuitUsuario = this.modificarDatosForm.controls.cuit.value;
        let formProveedores = this.modificarDatosForm.get('proveedores')['controls'];
        for (let index = 0; index < formProveedores.length; index++) {
            let proveedores = formProveedores[index].controls;
            proveedores.cuit.setValue(cuitUsuario);
        }
    }
    public onActualizaTipoProveedor() {
        const idTipoUsuario = this.modificarDatosForm.controls.idTipoUsuario.value;
        let formProveedores = this.modificarDatosForm.get('proveedores')['controls'];
        for (let index = 0; index < formProveedores.length; index++) {
            let proveedores = formProveedores[index].controls;
            proveedores.idTipoProveedor.setValue(idTipoUsuario);
        }
    }
    public onModificarDatos() {
        let mensaje = this.validarDatosUsuario();
        if (mensaje != '') {
            this.messageService.add({ key: 'toastPopupDetalles', severity: 'error', summary: 'Errores encontrados', detail: mensaje });
            return;
        }
        this.spinnerComponent.showIt();
        let existeErrores: boolean = false;
        let modificarUsuario = this.crearObjectoModificarUsuario();
        this.service.validarMailUsuario(modificarUsuario).subscribe(response => {
            if (response.data.validaciones.length > 0) {
                for (const data of response.data.validaciones)
                    mensaje += `${data}\n`;
                existeErrores = true;
            }
        }, error => { }
            , () => {
                this.spinnerComponent.hideIt();
                if (!existeErrores) {
                    this.guardarDatosUsuario();
                } else {
                    this.messageService.add({ key: 'toastPopupDetalles', severity: 'error', summary: 'Errores encontrados', detail: mensaje });
                }
            });
    }
    private guardarDatosUsuario() {
        this.spinnerComponent.showIt();
        let datosUsuario = this.crearObjectoModificarUsuario();
        this.service.modificarUsuario(datosUsuario).subscribe(data => {
        }, error => { }, () => {
            this.spinnerComponent.hideIt();
            this.service.setUsuarioRecargarLista(true);
            this.cerrarModal.emit(true);
            this.messageService.add({ key: 'toastPopupDetalles', severity: 'success', summary: 'Modificación de Usuario', detail: 'Se guardaron los cambios correctamente.' });
        });
    }

    private validarDatosUsuario() {
        let mensaje: string = '';
        let datosUsuario = this.crearObjectoModificarUsuario();
        if (datosUsuario.mail == '') mensaje += 'Ingrese el mail del usuario.\n'
        if (datosUsuario.idTipoUsuario == '' || datosUsuario.idTipoUsuario == '0') mensaje += 'Seleccione el tipo de usuario.\n';
        if (datosUsuario.cuit == '' || datosUsuario.cuit.length < 11) mensaje += 'El cuit del usuario no tiene el formato correcto.\n';
        if (mensaje == '') {
            datosUsuario.proveedores.forEach(proveedor => {
                if (proveedor.cuit == '' || proveedor.cuit.length < 11) mensaje += 'El cuit del proveedor no tiene el formato correcto.\n';
                if (proveedor.codigoProveedor == '' && proveedor.codigoProveedor == '0') mensaje += 'No se ha ingresado el código del proveedor.\n';
                if (proveedor.razonSocial == '') mensaje += 'No se ha ingresado la razón social.\n';
                if (mensaje != '') return;
            });
        }
        return mensaje;
    }
    public inicializarFormProveedor(proveedor: Proveedor): FormGroup {
        if (proveedor != null) {
            return this.formBuilder.group({
                id: proveedor.Id,
                cuit: { value: proveedor.CUIT, disabled: true },
                razonSocial: proveedor.RazonSocial,
                codigoProveedor: proveedor.CodigoProveedor,
                idTipoProveedor: { value: proveedor.IdTipoProveedor, disabled: true },
                esRevendedor: proveedor.EsRevendedor,
                organizacionDeCompra: proveedor.OrganizacionDeCompra
            })
        } else {
            return this.formBuilder.group({
                id: [0],
                cuit: 0,
                razonSocial: [''],
                codigoProveedor: [0],
                idTipoProveedor: 0,
                esRevendedor: false
            })
        }
    }
    @ViewChild("spinnerModalAsignar")
    protected spinnerModalAsignar: SpinnerComponent;
    asignandoCuit = false;
    ngOnDestroy(): void {
        this.subscriptions.unsubscribe()
    }

    formAsignarCUIT = new FormGroup({
        cuit: new FormControl(null, [Validators.required, Validators.min(11)]),
        razonSocial: new FormControl({ value: null, disabled: true }, [Validators.required]),
        codigoProveedor: new FormControl({ value: null, disabled: true }, [Validators.required]),
        tipoProveedorId: new FormControl({ value: null, disabled: false }, [Validators.required]),
    });

    proveedoresAsignarCuit: ProveedorARelacionar[] = [];
    proveedorAsignarCuitSeleccionado: ProveedorARelacionar;

    asignarFormSubscriptions() {
        const cuitControl = this.formAsignarCUIT.get("cuit");
        this.subscriptions.add(
            cuitControl.valueChanges.pipe(
                filter((_) => cuitControl.valid),
                debounceTime(500)).subscribe(cuit => {
                    this.obtenerProveedoresARelacionar(cuit);
                })
        )
    }

    obtenerProveedoresARelacionar(cuit: string) {
        this.spinnerModalAsignar.showIt();
        this.service.getProveedoresARelacionar(cuit).subscribe(
            (resp) => {
                this.spinnerModalAsignar.hideIt();
                if (resp.error || resp.info) {
                    this.messageService.add({
                        key: "toastAsignacion",
                        severity: resp.error ? 'error' : 'warn',
                        summary: "Validando CUIT",
                        detail: resp.error || resp.info
                    });
                }
                else {
                    if (resp.data) {
                        this.proveedoresAsignarCuit = resp.data;
                    }
                }
            });
    }

    onProveedorAsignarCuitChanged() {
        const ctrlRazonSocial = this.formAsignarCUIT.get("razonSocial");
        const ctrlCodigoProveedor = this.formAsignarCUIT.get("codigoProveedor");

        if (this.proveedorAsignarCuitSeleccionado) {
            if (ctrlRazonSocial) { ctrlRazonSocial.setValue(this.proveedorAsignarCuitSeleccionado.RazonSocial) };
            if (ctrlCodigoProveedor) { ctrlCodigoProveedor.setValue(this.proveedorAsignarCuitSeleccionado.CodigoProveedor) };
        }
        else {
            if (ctrlRazonSocial) { ctrlRazonSocial.setValue(null) };
            if (ctrlCodigoProveedor) { ctrlCodigoProveedor.setValue(null) };
        }
    }

    public onAsignarNuevaCuit() {
        if (this.asignandoCuit)
            return;

        this.asignandoCuit = true;
        this.spinnerModalAsignar.showIt();
        const { id, mail } = this.modificarDatosForm.getRawValue();
        const { cuit, codigoProveedor, tipoProveedorId, razonSocial } = {
            cuit: this.proveedorAsignarCuitSeleccionado.CUIT,
            codigoProveedor: this.proveedorAsignarCuitSeleccionado.CodigoProveedor,
            tipoProveedorId: this.proveedorAsignarCuitSeleccionado.IdTipoProveedor,
            razonSocial: this.proveedorAsignarCuitSeleccionado.RazonSocial
        };
        this.service.asignarNuevaCuit({
            idUsuario: id,
            mailUsuario: mail,
            cuitAAsignar: cuit,
            codigoProveedorAAsignar: codigoProveedor,
            tipoProveedorIdAAsignar: tipoProveedorId.toString(),
            razonSocialAAsignar: razonSocial
        }).subscribe(({ logout, error, info, data }) => {
            this.spinnerModalAsignar.hideIt();
            this.asignandoCuit = false;
            if (logout) {
            } else if (error || info) {
                this.messageService.add({
                    key: "toastAsignacion",
                    severity: error ? 'error' : 'warn',
                    summary: "Intentando Asignación",
                    detail: error || info
                })
            } else {
                this.messageService.add({
                    key: "toastAsignacion",
                    severity: "success",
                    summary: "Asignación exitosa",
                    detail: `Se asignó correctamente la CUIT: ${cuit} para el usuario ${mail}`
                })
                this.limpiarFormAsignar()
            }
        })
    }

    limpiarFormAsignar() {
        this.formAsignarCUIT.reset()
    }
}
