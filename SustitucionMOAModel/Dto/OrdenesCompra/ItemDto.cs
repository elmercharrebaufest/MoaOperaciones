using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    public class ItemDto
    {
        public string Id { get; set; }
        public string LINE_NO { get; set; }
        public int? PosicionId { get; set; }
        public int NumeroLinea { get; set; }
        public decimal? Cantidad { get; set; }
        public string Descripcion { get; set; }
        public string Campo_I { get; set; }
        public long? ServicioNumero { get; set; }
        public string UnidadMedida { get; set; }
        public decimal? PrecioBruto { get; set; }
        public decimal Monto { get; set; }
        public string Toler { get; set; }
        public string ItemNumero { get; set; }
        public string SUBPCKG_NO { get; set; }
        public string UM { get; set; }
        public decimal? Importe { get; set; }

        /// <summary>
        /// POHEADER.CURRENCY 
        /// </summary>
        public string Moneda { get; set; }

        /// <summary>
        /// MMSN-460 - Suma de las cantidades de las ES ingresadas por OC
        /// </summary>
        public decimal? CantidadReal { get; set; }

        /// <summary>
        /// MMSN-460 - % del item = cantidadReal x 100 / cantidad
        /// </summary>
        public string Porcentaje { get; set; }

        /// <summary>
        /// MMSN-460 - Mismo solicitante para el item que la posición.
        /// </summary>
        public string Solicitante { get; set; }

        /// <summary>
        /// MMSN-491 - Cambiar separador
        /// </summary>
        public string ImporteString { get; set; }
        
        
        public List<EntradaServicioDto> EntradasServicio { get; set; }

        /// <summary>
        /// referencia a la orden de compra a la que pertenece el item
        /// </summary>
        public string NroOrdenCompra { get; set; }

        /// <summary>
        /// referencia a la posicion de la orden de compra a la que pertenece el item
        /// </summary>
        public string NroPosicion { get; set; }

    }
}
