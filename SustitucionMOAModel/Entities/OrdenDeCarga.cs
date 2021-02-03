using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class OrdenDeCarga
    {
        [Key]
        public int Id { get; set; }

        public int ClienteId { get; set; }

        public DateTime FechaCarga { get; set; }

        public int CUITCliente { get; set; }

        public string NombreChofer { get; set; }

        public string ApellidoChofer { get; set; }

        public int CUITChofer { get; set; }

        public int CUITTransporte { get; set; }

        public string RazonSocialTransporte { get; set; }

        public string Producto { get; set; }

        public int Cantidad { get; set; }

        public string Observacion { get; set; }

        public EstadoOrdenDeCarga Estado { get; set; }

        public string ContratoSAP { get; set; }

        public bool CorredorSeleccionado { get; set; }

        public bool TransporteExiste { get; set; }
    }
}
