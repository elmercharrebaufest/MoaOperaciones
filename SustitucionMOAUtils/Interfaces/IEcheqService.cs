using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEcheqService
    {
        List<EcheqNegocioDto> ObtenerPendientePago(string proveedor, string fechaInicio, string fechaFin, string contrato);
        string MarcarContrato(EcheqRequestModel request);
        string DesmarcarContrato(EcheqRequestModel request);
        string MarcarDocumento(EcheqRequestModel request);
        string DesmarcarDocumento(EcheqRequestModel request);
        string AgregarApertura(EcheqRequestModel request);
        List<ConfiguracionDto> ObtenerConfiguracion();
        List<EcheqReporteDto> ObtenerDatosReporte(string fechaInicio, string fechaFin, string mailUsuario, string codigoProveedor);
    }
}
