using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class OrdenCompraItemDto
    {
        public int ItemId { get; set; }
        public int? PosicionId { get; set; }
        public string Descripcion { get; set; }
        public string Campo_I { get; set; }
        public int? ServicioNumero { get; set; }
        public decimal? Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public decimal? PrecioBruto { get; set; }
        public decimal Monto { get; set; }
        public string Toler { get; set; }
        public List<OrdenCompraEntradaServicioDto> EntradaServicio { get; set; }
        public string PCKG_NO { get; set; }
        public string SUBPCKG_NO { get; set; }
    }
}
