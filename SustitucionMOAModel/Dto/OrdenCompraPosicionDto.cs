using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class OrdenCompraPosicionDto
    {
        public long OrdenCompraId { get; set; }
        public int PosicionId { get; set; }
        public string Material { get; set; }
        public string Descripcion { get; set; }
        public decimal CantidadPedida { get; set; }
        public string Campo_U { get; set; }
        public string Campo_T { get; set; }
        public DateTime FechaEntrega { get; set; }
        public decimal PrecioNeto { get; set; }
        public string Moneda { get; set; }
        public string Campo_por { get; set; }
        public string Campo_CPP { get; set; }
        public string Campo_Grupo_Art { get; set; }
        public string Campo_CE { get; set; }
        public string Almacen { get; set; }
        public string Solicitante { get; set; }
        public long Solped { get; set; }
        public List<OrdenCompraItemDto> Items { get; set; }
    }
}
