using SustitucionMOAModel.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAModel.Util
{
    public static class PrefijoCondicionEspecial
    {
        private const string none = "";

        /// <summary>
        /// Trabajo ya hecho. Sin certificación automática
        /// </summary>
        private const string TR = "TR-";

        /// <summary>
        /// Trabajo ya hecho. Con certificación automática
        /// </summary>
        private const string TRC = "TRC-";

        /// <summary>
        /// Proveedor asignado
        /// </summary>
        private const string PA = "PA-";

        /// <summary>
        /// Urgencia
        /// </summary>
        private const string UR = "UR-";

        /// <summary>
        /// Adicional
        /// </summary>
        private const string AD = "AD-";

        /// <summary>
        /// Trabajo ya hecho y Adicional
        /// </summary>
        private const string AOR = "AOR-";

        /// <summary>
        /// Trabajo ya hecho y Adicional. Con Certificación automática
        /// </summary>
        private const string AORC = "AORC-";

        /// <summary>
        /// Trabajo ya hecho y Urgencia
        /// </summary>
        private const string TUR = "TUR-";

        /// <summary>
        /// Trabajo ya hecho y Urgencia. Con Certificación automática
        /// </summary>
        private const string TURC = "TURC-";

        /// <summary>
        /// Trabajo ya hecho y Servicio permanente. Sin Adicional ni Urgencia
        /// </summary>
        private const string SP = "SP-";

        /// <summary>
        /// Trabajo ya hecho y Servicio permanente. Sin Adicional ni Urgencia. Con Certificación automática
        /// </summary>
        private const string SPC = "SPC-";

        /// <summary>
        /// Trabajo ya hecho y Ajuste polinómica. Sin Adicional ni Urgencia
        /// </summary>
        private const string AJ = "AJ-";

        /// <summary>
        /// Trabajo ya hecho y Ajuste polinómica. Sin Adicional ni Urgencia. Con Certificación automática
        /// </summary>
        private const string AJC = "AJC-";

        /// <summary>
        /// Trabajo ya hecho y Proveedor directo. Sin Adicional ni Urgencia
        /// </summary>
        private const string PD = "PD-";

        /// <summary>
        /// Trabajo ya hecho y Proveedor directo. Sin Adicional ni Urgencia. Con Certificación automática
        /// </summary>
        private const string PDC = "PDC-";

        private readonly static IReadOnlyCollection<string> PrefijosValidos = new List<string>
        {
            TR,
            TRC,
            PA,
            UR,
            AD,
            AOR,
            AORC,
            TUR,
            TURC,
            SP,
            SPC,
            AJ,
            AJC,
            PD,
            PDC
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
                prefijo = solp.CertificacionAutomatica ? TRC : TR;
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

            if (solp.TrabajoYaHecho == true)
            {
                if (solp.Adicional == true)
                {
                    prefijo = solp.CertificacionAutomatica ? AORC : AOR;
                }

                if (solp.Urgencia == true || (solp.Urgencia == true && solp.Adicional == true))
                {
                    prefijo = solp.CertificacionAutomatica ? TURC : TUR;
                }

                if (solp.THServicioPermanente == true && solp.Adicional != true && solp.Urgencia != true)
                {
                    prefijo = solp.CertificacionAutomatica ? SPC : SP;
                }

                if (solp.THAjustePolinomica == true && solp.Adicional != true && solp.Urgencia != true)
                {
                    prefijo = solp.CertificacionAutomatica ? AJC : AJ;
                }

                if (solp.THProveedorDirecto == true && solp.Adicional != true && solp.Urgencia != true)
                {
                    prefijo = solp.CertificacionAutomatica ? PDC : PD;
                }
            }

            return prefijo;
        }
    }
}
