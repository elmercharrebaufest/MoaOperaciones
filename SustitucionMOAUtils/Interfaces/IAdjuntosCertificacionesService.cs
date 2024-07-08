using SustitucionMOAModel.Dto.Compras;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAdjuntosCertificacionesService
    {
        Task<List<string>> AdjuntarAsync(HttpFileCollectionBase files, string name);
        Task<List<ESAdjuntosDto>> GetAdjuntos(string idESTemporal);
    }
}
