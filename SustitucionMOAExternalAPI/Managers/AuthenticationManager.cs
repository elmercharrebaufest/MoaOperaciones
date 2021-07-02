using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SustitucionMOAExternalAPI.Managers
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly IExternalApiService _service;
        public AuthenticationManager(IExternalApiService service)
        {
            this._service = service;
        }
        public List<string> ObtenerPermisos(string apikey)
        {
            var roles = _service.GetRolesApiKey(apikey);

            return roles.SelectMany(x => x.PermisosAsociados).Select(x => x.Permiso).Distinct().ToList();
        }

        public string ObtenerUsuario(string apikey)
        {
            return _service.GetUsuarioApiKey(apikey);
        }
    }
}