using Newtonsoft.Json;
using SustitucionMOAModel.Models.DataAgro;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
    public class ObtenerContratosDisponiblesResponse
    {
        //[JsonProperty("contratosDisponibles")]
        public List<ContratoOrdenFas> Contratos { get; set; }

        //[JsonProperty("info")]
        public string Info { get; set; }

        //[JsonProperty("error")]
        public string Error { get; set; }

        //[JsonProperty("logout")]
        public bool Logout { get; set; }
    }

    public class ContratoOrdenFas
    {
        public string NumeroContrato { get; set; }

        public MaterialDto Producto { get; set; }

        public string TipoContrato { get; set; }
        //public string ProductoId { get; set; }

        //public string ProductoDescripcion { get; set; }
    }
}