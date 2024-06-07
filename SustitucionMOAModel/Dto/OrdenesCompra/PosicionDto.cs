using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    public class PosicionDto
    {   
        public int Id { get; set; }
        public long NumeroPosicion { get; set; }
        public int CotizacionPosicion_Id { get; set; }
        public decimal Cantidad { get; set; }
        public int SolpPosicion_Id { get; set; }
       // public List<SolpSubposicionDtoCopia> SubposicionesCompras { get; set; }
        public string CodigoMaterial { get; set; }
        public int? Indice { get; set; }
        public string Descripcion { get; set; }
        public string CentroComprasDescripcion { get; set; }
        public string TextoSuministro { get; set; }
        public string Modelo { get; set; }
        public string UnidadDescripcion { get; set; }
        public string MonedaDescripcion { get; set; }
        public decimal? PrecioUnidad { get; set; }
        public int? MonedaId { get; set; }
        public decimal? PrecioTotal { get; set; }
        public DateTime? FechaEntregaServicio { get; set; }
        public int? PlazoDeOferta { get; set; }
        public string MaterialComprasDescripcion { get; set; }
        public string MonedaCodigo { get; set; }
        public string CentroComprasCodigo { get; set; }
        public string MaterialTextoAmpliado { get; set; }
        public string UM { get; set; }
        public string GrupoArticulos { get; set; }
        public string Centro { get; set; }
        public string Almacen { get; set; }
        public string NumeroSolp { get; set; }
        public string Contrato { get; set; }
        public string Solicitante { get; set; }

        /// <summary>
        /// MMSN-491 - Cambiar separador
        /// </summary>
        public string PrecioUnidadString { get; set; }
        /// <summary>
        /// Lista de Ids de las entradas de servicio
        /// </summary>
        //public List<string> IdsEntradasDeServicio { get; set; }
        public List<ItemDto> Items { get; set; }

        /// <summary>
        /// referencia a la orden de compra a la que pertenece la posicion
        /// </summary>
        public string NroOrdenCompra { get; set; }

        public string NoMoreGR { get; set; }
        public bool Bloqueada { get; set; }
    }
}
