using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.AzureAD.Model
{
    public class ObtenerUsuarioResponse
    {
        public List<UsuarioResponse> Value { get; set; }
    }

    public class UsuarioResponse
    {
        public string Id { get; set; }

        public string UserPrincipalName { get; set; }

        public List<string> BusinessPhones { get; set; }

        public string DisplayName { get; set; }

        public string GivenName { get; set; }

        public string JobTitle { get; set; }

        public string Mail { get; set; }

        public string MobilePhone { get; set; }

        public string OfficeLocation { get; set; }

        public string PreferredLanguage { get; set; }

        public string Surname { get; set; }
    }
}
