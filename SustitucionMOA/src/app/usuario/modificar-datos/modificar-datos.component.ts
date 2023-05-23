import { Component, OnInit, ViewChild } from '@angular/core';
import { UsuarioService } from '../usuario.service';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';

@Component({
  selector: 'app-modificar-datos',
  templateUrl: './modificar-datos.component.html',
  styleUrls: ['./modificar-datos.component.css']
})
export class ModificarDatosComponent implements OnInit {

  tipoUsuario: any = [];
  modificarDatosForm: FormGroup;
  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;

  constructor(protected service: UsuarioService, private formBuilder: FormBuilder) {
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
      idTipoUsuario: [0],
      proveedores: this.formBuilder.array([]),
    });
  }
  public trackByFn(index: any, item: any) {
    return index;
  }
  private cargarFormDatosUsuario(usuario: any) {
    console.log('usuario--->>',usuario);
    this.modificarDatosForm.controls['id'].setValue(usuario.Id);
    this.modificarDatosForm.controls['mail'].setValue(usuario.Mail);
    this.modificarDatosForm.controls['cuit'].setValue(usuario.CUIT);
    this.modificarDatosForm.controls['idTipoUsuario'].setValue(usuario.TipoUsuario.Id);

    this.service.getProvedoresEmail(usuario.Mail).subscribe(listaProveedores => {
      this.cargarFormDatoProveedores(listaProveedores.data.proveedores);
    }, error => { }, () => { this.spinnerComponent.hideIt(); });

  }
  private cargarFormDatoProveedores(proveedores: any) {
    proveedores.forEach(proveedor => {
      this.proveedoresFormArray.push(this.inicializarFormProveedor(proveedor));
    });
  }

  public onModificarDatos(){
    this.spinnerComponent.showIt();
    let existeErrores: boolean = false;
    this.service.validaModificacionUsuario(this.modificarDatosForm.value).subscribe(data=>{
      if(data.length > 0) existeErrores= true;
    }, error => { }
     , () => { 
      this.spinnerComponent.hideIt(); 
    });
  }

  public inicializarFormProveedor(proveedor: any): FormGroup {
    if (proveedor != null) {
      return this.formBuilder.group({
        cuit: proveedor.CUIT,
        razonSocial: proveedor.RazonSocial,
        codigoProveedor: proveedor.CodigoProveedor,
        idTipoProveedor: proveedor.IdTipoProveedor
      })
    } else {
      return this.formBuilder.group({
        cuit: 0,
        razonSocial: [''],
        codigoProveedor: [0],
        idTipoProveedor: 0
      })
    }
  }

}
