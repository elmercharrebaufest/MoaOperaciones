using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Interfaces.Validadores;
using System;

namespace SustitucionMOAUtils.Validadores
{
    public class ValidadorPesificacion : IValidadorPesificacion
    {
        public ResultadoValidacionPesificacion IsValid(LogPesificacion entidad)
        {
            ResultadoValidacionPesificacion resultado = new ResultadoValidacionPesificacion();

            if (entidad.IdUsuario == 0)
                resultado.Errores.Add(new ValidationCustomException(String.Format(ErrorMsg.ErrorUserNoLogueado)));

            if (entidad.Fecha != null && entidad.Fecha == DateTime.MinValue)
                resultado.Errores.Add(new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Fecha")));

            if (string.IsNullOrEmpty(entidad.CodigoProveedor))
                resultado.Errores.Add(new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "CodigoDeProveedor")));


            if (entidad.EsCargaMasiva)
            {
                if (string.IsNullOrEmpty( entidad.RutaFisicaArchivo))
                    resultado.Errores.Add(new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "RutaFisicaArchivo")));
            }
            else
            {

                if (entidad.Fijacion.HasValue && entidad.Fijacion == 0)
                    resultado.Errores.Add(new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Fijacion")));

                if (entidad.Contrato.HasValue && entidad.Contrato == 0)
                    resultado.Errores.Add(new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Contrato")));

                if (entidad.CantidadKilos.HasValue && entidad.CantidadKilos == 0)
                    resultado.Errores.Add(new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Cantidad de kilos")));
            }

            return resultado;
        }
    }
}
