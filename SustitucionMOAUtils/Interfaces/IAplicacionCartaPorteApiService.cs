using SustitucionMOAModel.Dto.AplicacionCartaPorte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAplicacionCartaPorteApiService
    {
        List<AplicacionCartaPorteApiDto> ObtenerAplicacionesAProcesar();

        void ActualizarEstadoAplicacion(int id, int nuevoEstado, string error);
    }
}
