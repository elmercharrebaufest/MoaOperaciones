using SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.Interfaces
{
    public interface IScatoRepositorioClient
    {
        //Task<ConsultaListado<Planta>> ObtenerPlantasAsync(string cuitDestino);
        ConsultaListado<Planta> ObtenerPlantas(string cuitDestino);
        ConsultaListado<Domicilio> ObtenerDomicilios(string cuitDestino);
    }
}
