using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class OrdenDeCargaDto
    {
        public int Id { get; set; }

        public string CUITCliente { get; set; }

        public string DescripcionEstado { get; set; }

        public string ColorSemaforo { get; set; }

    }

    public class OrdenDeCargaDetalleDto
    {
        public int Id { get; set; }
        public string CUITCliente { get; set; }
        public string RazonSocialCliente { get; set; }
        public string DescripcionEstado { get; set; }
        public string ColorSemaforo { get; set; }

        public string Chofer { get; set; }

        public string FechaCarga { get; set; }

        public string Transporte { get; set; }

        public int Cantidad { get; set; }

        public string Observacion { get; set; }

        public string ContratoSAP { get; set; }

        public bool CorredorSeleccionado { get; set; }

        public string Corredor { get; set; }

        public bool TransporteExiste { get; set; }

        public string PatenteAcoplado { get; set; }

        public string ChasisAcoplado { get; set; }

        public bool AprobadoCredito { get; set; }

        public bool InformadaSAP { get; set; }

        public string FechaEntregaGenerada { get; set; }

    }
}
