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
        ResultadoGenerico DesmarcarContrato(EcheqRequestModel request);
        ResultadoGenerico MarcarDocumento(EcheqRequestModel request);
        ResultadoGenerico DesmarcarDocumento(EcheqRequestModel request);
    }
}
