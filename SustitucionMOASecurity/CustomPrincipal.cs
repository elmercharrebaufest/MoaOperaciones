using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models;

namespace SustitucionMOASecurity
{
    class CustomPrincipal : IPrincipal
    {
        private Usuario Usuario;

        public CustomPrincipal(Usuario usuario)
        {

            if (usuario != null)
            {
                this.Usuario = usuario;
                this.Identity = new GenericIdentity(usuario.username);
            }
        }


        public IIdentity Identity
        {
            get;
            set;
        }

        public bool IsInRole(string permisosString)
        {
            if (string.IsNullOrEmpty(permisosString))
                return true;
            else
            {
                if (this.Usuario == null)
                {
                    return false;
                }

                var permisos = permisosString.Split(new char[] { ',' });

                ICollection<string> permisosUsuario = this.Usuario.permisos;

                return permisos.Any(r => permisosUsuario.Any( pu => pu == r));

            }
        }
    }
}
