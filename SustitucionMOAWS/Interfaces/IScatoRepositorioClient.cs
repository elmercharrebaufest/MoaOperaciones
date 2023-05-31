using SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
using SustitucionMOAWS.ScatoComandosWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.Interfaces
{
    public interface IScatoRepositorioClient
    {
        ConsultaListado<Planta> ObtenerPlantas(string cuitDestino);
        ConsultaListado<Domicilio> ObtenerDomicilios(string cuitDestino);
        ConsultaListado<ChoferDto> ObtenerChoferPorCuil(string cuilChofer);
    }
}
