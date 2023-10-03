using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.ViewModel.Notificacion;
using System;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IComunicacionService
    {
        List<ComunicacionDto> ObtenerComunicacionesPorProveedor(string vendedor, string proveedor, string fechaInicio, string fechaFin);
        string   GrabarComunicacionComoLeida(ComunicacionListaIdDto comunicacionIds);
        string GrabarComunicacionComoNoLeida(ComunicacionListaIdDto comunicacionIds);
        String ProcesarCM05(string vendedor, string proveedor);
        String ProcesarCuentasHabilitadas(string vendedor, string proveedor);
        //string ObtenerLiquidacionesObservadas(string vendedor, string proveedor);
        string ProcesarLiquidacionesObservadas(string vendedor, string fechaInicio, string fechaFin);
    }
}
