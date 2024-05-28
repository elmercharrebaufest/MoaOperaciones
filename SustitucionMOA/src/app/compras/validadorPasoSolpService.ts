import { Injectable } from "@angular/core";
import { FormGroup } from "@angular/forms";


@Injectable()
export class ValidadorPasoSolpService
{
    _formulario: FormGroup;

    set formulario(form :FormGroup )
    {
      this._formulario = form;
    }
    
    //devuelve el valor si el formulario es valido
    esPasoInvalido(): boolean {
        if(this._formulario != undefined){

            return this._formulario.invalid;
        }
    }

    //aplica las validaciones para los controles 
    aplicarValidaciones(): void {
        Object.keys(this._formulario.controls).forEach(key => {
            let control = this._formulario.get(key);
            control.markAsDirty();
            control.updateValueAndValidity();
        });
    }

    // para cuando el control pierde el foco
    onBlurDirty(nombreControl : string)
    {
        let control = this._formulario.controls[nombreControl]
        if(control.value == undefined || control.value=="")
        {
            control.markAsDirty();
        }
    }

}