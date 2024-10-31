using SustitucionMOAModel.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAModel.Util
{
    public static class PrefijoCondicionEspecial
    {
        public const string none = "";
        public const string TR = "TR-";
        public const string PA = "PA-";
        public const string UR = "UR-";
        public const string AD = "AD-";
        public const string AOR = "AOR-";
        public const string TUR = "TUR-";
        public const string SP = "SP-";
        public const string AJ = "AJ-";
        public const string PD = "PD-";

        public readonly static IReadOnlyCollection<string> PrefijosValidos = new List<string>
        {
            TR,
            PA,
            UR,
            AD,
            AOR,
            TUR,
            SP,
            AJ,
            PD,
        }
        .AsReadOnly();

        public static bool TienePrefijo(string nombre, out string prefijo)
        {
            prefijo = none;
            if (string.IsNullOrWhiteSpace(nombre)) { return false; }

            bool res = PrefijosValidos.Any(p => nombre.StartsWith(p));

            if (res)
            {
                prefijo = PrefijosValidos.First(p => nombre.StartsWith(p));
            }

            return res;
        }


        public static string ConfigurarPrefijos(this Solp solp)
        {
            var prefijo = none;

            if (solp.TrabajoYaHecho == true)
            {
                prefijo = TR;
            }

            if (solp.CondEspProveedorAsignado == true)
            {
                prefijo = PA;
            }

            if (solp.Urgencia == true)
            {
                prefijo = UR;
            }

            if (solp.Adicional == true || (solp.Adicional == true && solp.Urgencia == true))
            {
                prefijo = AD;
            }

            if (solp.TrabajoYaHecho == true && solp.Adicional == true)
            {
                prefijo = AOR;
            }

            if ((solp.TrabajoYaHecho == true && solp.Urgencia == true) || (solp.TrabajoYaHecho == true && solp.Urgencia == true && solp.Adicional == true))
            {
                prefijo = TUR;
            }

            if (solp.TrabajoYaHecho == true && solp.THServicioPermanente == true && solp.Adicional != true && solp.Urgencia != true)
            {
                prefijo = SP;
            }

            if (solp.TrabajoYaHecho == true && solp.THAjustePolinomica == true && solp.Adicional != true && solp.Urgencia != true)
            {
                prefijo = AJ;
            }

            if (solp.TrabajoYaHecho == true && solp.THProveedorDirecto == true && solp.Adicional != true && solp.Urgencia != true)
            {
                prefijo = PD;
            }

            return prefijo;

        }
    }
}
