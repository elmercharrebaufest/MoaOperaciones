using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WebApiMap.CNRT
{
    public class EquiposResponse
    {
        public string Result { get; set; }

        public int Status { get; set; }

        public Equipo Data { get; set; }

        public object UserMessage { get; set; }

        public object Actions { get; set; }

        public TipoVehiculoCNRT? TipoVehiculoCNRTSegunCategoriaEscalado
        {
            get
            {
                switch (Data.CategoriaEscalado)
                {
                    case null:
                    case "A":
                    case "B":
                        return TipoVehiculoCNRT.CamionBitren;
                    case "C":
                        return TipoVehiculoCNRT.CamionC;
                    case "D":
                        return TipoVehiculoCNRT.CamionD;
                    case "E":
                        return TipoVehiculoCNRT.CamionE;
                    default:
                        return null;
                }
            }
        }
    }

    public enum TipoVehiculoCNRT
    {
        CamionBitren,
        CamionC,
        CamionD,
        CamionE
    }
}
