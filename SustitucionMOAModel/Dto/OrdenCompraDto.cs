using SustitucionMOAModel.Dto.OrdenesCompra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class OrdenCompraDto
    {
        public long Id { get; set; }
        public string ProveedorNombre { get; set; }
        public string Fecha { get; set; }
        public string Descripcion { get; set; }
        public decimal MontoTotal { get; set; }
        public List<PosicionDto> Posiciones { get; set; }
        public string ProveedorNumero { get; set; }
        public string MonedaDescripcion { get; set; }
        public string SUBJ_TO_R { get; set; }
        //public List<OrdenCompraPosicionDto> Posiciones { get; set; }
        //public List<PosicionDto> PosicionesTmp2 { get; set; } = new List<PosicionDto>();
    }
}
