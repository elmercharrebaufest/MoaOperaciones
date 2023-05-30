import { Component, OnInit, ViewChild, Output, EventEmitter } from '@angular/core';
import { UsuarioService } from '../usuario.service';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { MessageService} from 'primeng/api';

@Component({
  selector: 'app-modificar-datos',
  templateUrl: './modificar-datos.component.html',
  styleUrls: ['./modificar-datos.component.css'],
  providers: [MessageService]
})
export class ModificarDatosComponent implements OnInit {

  tipoUsuario: any = [];
  existeProveedores: boolean = true;
  modificarDatosForm: FormGroup;
  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;
  @Output() cerrarModal = new EventEmitter();

  constructor(protected service: UsuarioService, 
              private formBuilder: FormBuilder,
              private messageService: MessageService) {
    this.spinnerComponent = new SpinnerComponent();
    this.service.UsuarioModificarDatos.subscribe(data => {
      this.modificarDatosForm = this.inicializarFormDatosUsuario();
      if (data != null)
        this.cargarDatosUsuario(data);
    });
  }

  ngOnInit() {
    this.cargarTipoUsuario();
  }

  get proveedoresFormArray(): FormArray {
    return this.modificarDatosForm.get("proveedores") as FormArray
  }

  private cargarTipoUsuario() {
    this.service.getTipoUsuario().subscribe(response => {
      this.tipoUsuario = response.data.tipoUsuario;
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
    let listaProveedores = null;
    this.existeProveedores = true;
    this.service.getProvedoresEmail(usuario.TipoUsuario.Id,usuario.Mail, usuario.CUIT).subscribe(proveedores => {
      listaProveedores = proveedores.data.proveedores;
    }, error => { }, () => { 
      if (listaProveedores.length == 0) this.existeProveedores = false;
      this.spinnerComponent.hideIt(); 
      this.cargarFormDatoProveedores(listaProveedores);
    });

  }
  private cargarFormDatoProveedores(proveedores: any) {
    proveedores.forEach(proveedor => {
      this.proveedoresFormArray.push(this.inicializarFormProveedor(proveedor));
    });
  }
  private crearObjectoModificarUsuario(){
    let modificarDatos = this.modificarDatosForm;
    let formProveedores = this.modificarDatosForm.get('proveedores')['controls'];
    let proveedores = [];
    for (let index = 0; index < formProveedores.length; index++) {
      let proveedor = formProveedores[index].controls;
      proveedores.push({
        codigoProveedor : proveedor.codigoProveedor.value,
        cuit : proveedor.cuit.value,
        id : proveedor.id.value,
        idTipoProveedor : proveedor.idTipoProveedor.value,
        razonSocial : proveedor.razonSocial.value,
      });
    }
    let modificarUsuario = {
      id: modificarDatos.controls["id"].value,
      mail: modificarDatos.controls["mail"].value,
      cuit: modificarDatos.controls["cuit"].value,
      usuarioModificacion: modificarDatos.controls["usuarioModificacion"].value,
      idTipoUsuario: modificarDatos.controls["idTipoUsuario"].value,
      proveedores: proveedores,
    }
    return modificarUsuario;
  }
  public onActualizaCuitProveedor(){
    const cuitUsuario = this.modificarDatosForm.controls.cuit.value;
    let formProveedores = this.modificarDatosForm.get('proveedores')['controls'];
    for (let index = 0; index < formProveedores.length; index++) {
      let proveedores = formProveedores[index].controls;
      proveedores.cuit.setValue(cuitUsuario);
    }
  }
  public onActualizaTipoProveedor(){
    const idTipoUsuario = this.modificarDatosForm.controls.idTipoUsuario.value;
    let formProveedores = this.modificarDatosForm.get('proveedores')['controls'];
    for (let index = 0; index < formProveedores.length; index++) {
      let proveedores = formProveedores[index].controls;
      proveedores.idTipoProveedor.setValue(idTipoUsuario);
    }
  }
  public onModificarDatos(){
    let mensaje = this.validarDatosUsuario();
    if (mensaje != ''){
      this.messageService.add({ key: 'toastPopupDetalles', severity: 'error', summary: 'Errores encontrados', detail: mensaje });
      return;
    }
    this.spinnerComponent.showIt();
    let existeErrores: boolean = false;
    let modificarUsuario = this.crearObjectoModificarUsuario();
    this.service.validarMailUsuario(modificarUsuario).subscribe(response=>{
      if(response.data.validaciones.length > 0) {
        for(const data of response.data.validaciones)
          mensaje += `${data}\n`;
        existeErrores= true;
      }
    }, error => { }
     , () => { 
      this.spinnerComponent.hideIt(); 
      if (!existeErrores){
        this.guardarDatosUsuario();
      }else{
        this.messageService.add({ key: 'toastPopupDetalles', severity: 'error', summary: 'Errores encontrados', detail: mensaje });
      }
    });
  }
  private guardarDatosUsuario(){
    this.spinnerComponent.showIt();
    let datosUsuario = this.crearObjectoModificarUsuario();
    this.service.modificarUsuario(datosUsuario).subscribe(data=>{
    }, error=>{}, ()=>{
      this.spinnerComponent.hideIt(); 
      this.service.UsuarioRecargarLista = true;
      this.cerrarModal.emit(true);
      this.messageService.add({ key: 'toastPopupDetalles', severity: 'success', summary: 'Modificación de Usuario', detail: 'Se guardaron los cambios correctamente.' });
    });
  }

  private validarDatosUsuario(){
    let mensaje: string = '';
    let datosUsuario = this.crearObjectoModificarUsuario();
    if(datosUsuario.mail == '') mensaje += 'Ingrese el mail del usuario.\n'
    if(datosUsuario.idTipoUsuario == '' || datosUsuario.idTipoUsuario == '0') mensaje += 'Seleccione el tipo de usuario.\n';
    if(datosUsuario.cuit == '' || datosUsuario.cuit.length < 11) mensaje += 'El cuit del usuario no tiene el formato correcto.\n';
    if (mensaje ==''){
      datosUsuario.proveedores.forEach(proveedor=>{
        if(proveedor.cuit == '' || proveedor.cuit.length < 11) mensaje += 'El cuit del proveedor no tiene el formato correcto.\n';
        if(proveedor.codigoProveedor == '' && proveedor.codigoProveedor == '0') mensaje += 'No se ha ingresado el codigo del proveedor.\n';
        if(proveedor.razonSocial == '') mensaje += 'No se ha ingresado la razón social.\n';
        if(mensaje !='') return;
      });
    }
    return mensaje;
  }
  public inicializarFormProveedor(proveedor: any): FormGroup {
    if (proveedor != null) {
      return this.formBuilder.group({
        id: proveedor.Id,
        cuit: { value: proveedor.CUIT,disabled: true },
        razonSocial: proveedor.RazonSocial,
        codigoProveedor: proveedor.CodigoProveedor,
        idTipoProveedor: { value: proveedor.IdTipoProveedor,disabled: true }
      })
    } else {
      return this.formBuilder.group({
        id: [0],
        cuit: 0,
        razonSocial: [''],
        codigoProveedor: [0],
        idTipoProveedor: 0
      })
    }
  }

}
