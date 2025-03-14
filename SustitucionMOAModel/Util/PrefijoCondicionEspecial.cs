using SustitucionMOAModel.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAModel.Util
{
    public static class PrefijoCondicionEspecial
    {
        private const string none = "";

        /// <summary>
        /// Trabajo ya hecho
        /// </summary>
        private const string TR = "TR-";

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
        /// Trabajo ya hecho y Urgencia
        /// </summary>
        private const string TUR = "TUR-";

        /// <summary>
        /// Trabajo ya hecho y Servicio permanente. Sin Adicional ni Urgencia
        /// </summary>
        private const string SP = "SP-";

        /// <summary>
        /// Trabajo ya hecho y Ajuste polinómica. Sin Adicional ni Urgencia
        /// </summary>
        private const string AJ = "AJ-";

        /// <summary>
        /// Trabajo ya hecho y Proveedor directo. Sin Adicional ni Urgencia
        /// </summary>
        private const string PD = "PD-";

        /// <summary>
        /// Con presupuesto
        /// </summary>
        private const string CPD = "CPD-";

        /// <summary>
        /// Con presupuesto + adicional
        /// </summary>
        private const string CPOR = "CPOR-";

        /// <summary>
        /// Con presupuesto + urgencia
        /// </summary>
        private const string CPU = "CPU-";

        /// <summary>
        /// Con presupuesto + adicional + urgencia
        /// </summary>
        private const string CPAU = "CPAU-";

        private readonly static IReadOnlyCollection<string> PrefijosValidos = new List<string>
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
            CPD,
            CPOR,
            CPU,
            CPAU
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

            if (solp.TrabajoYaHecho == true)
            {
                if (solp.Adicional == true)
                {
                    prefijo = AOR;
                }

                if (solp.Urgencia == true || (solp.Urgencia == true && solp.Adicional == true))
                {
                    prefijo = TUR;
                }

                if (solp.THServicioPermanente == true && solp.Adicional != true && solp.Urgencia != true)
                {
                    prefijo = SP;
                }

                if (solp.THAjustePolinomica == true && solp.Adicional != true && solp.Urgencia != true)
                {
                    prefijo = AJ;
                }

                if (solp.THProveedorDirecto == true && solp.Adicional != true && solp.Urgencia != true)
                {
                    prefijo = PD;
                }
            }
            else
            {
                if (solp.ConPresupuesto)
                {
                    prefijo = CPD;

                    if (solp.Adicional == true && solp.Urgencia == true)
                    {
                        prefijo = CPAU;
                    }
                    else
                    {
                        if (solp.Adicional == true)
                        {
                            prefijo = CPOR;
                        }
                        if (solp.Urgencia == true)
                        {
                            prefijo = CPU;
                        }
                    }
                }
            }

            return prefijo;
        }
    }
}
