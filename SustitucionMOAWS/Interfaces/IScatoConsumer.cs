using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAWS.ScatoWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.Interfaces
{
    public interface IScatoConsumer
    {
        List<CartaPorteFoto> ObtenerFotoCartaPorte(string cartaPorteId);
        List<CartaPorteFoto> ObtenerFotoCartasPorte(List<string> cartaPorteIds);
        List<LocalidadDto> ObtenerLocalidades();
        List<ProvinciaDto> ObtenerProvincias();
        List<KmPorProveedorDto> BuscarDestinos(string cuit);
        bool CuilChoferExiste(string cuil);


    }
}
