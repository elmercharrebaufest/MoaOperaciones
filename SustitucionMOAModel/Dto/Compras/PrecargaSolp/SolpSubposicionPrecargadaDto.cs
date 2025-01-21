using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.Compras.PrecargaSolp
{
    public class SolpSubposicionPrecargadaDto
    {
        public int Numero { get; set; }
        public string Tarea { get; set; }
        public string Codigo { get; set; }
        public decimal? Cantidad { get; set; }
        public int? UnidadId { get; set; }
        public TablaSapDto CuentaMayor { get; set; }
        public TablaSapDto Unidad { get; set; }

        public ServicioSolpDto ServicioCatalogado { get; set; }
    }
}
