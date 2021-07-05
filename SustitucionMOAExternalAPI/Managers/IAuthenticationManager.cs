using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SustitucionMOAExternalAPI.Managers
{
    public interface IAuthenticationManager
    {
        List<string> ObtenerPermisos(string apikey);
        string ObtenerUsuario(string apikey);
    }
}