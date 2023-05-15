using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Compras
{
    public class Material
    {
        public string CentroLogistico { get; set; }
        public string NroMaterial { get; set; }
        public string NombreDeMaterial { get; set; }
        public string GrupoArticulo { get; set; }
        public string TipoMaterial { get; set; }
        public string UnidadDeMedidaBase { get; set; }
        public string UnidadDeMedidaCompras { get; set; }
        public string UnidadDeMedidaSalida { get; set; }
        public string TipoValoracion { get; set; }
        public string ClaseDeValoracion { get; set; }
        public decimal PrecioDelMaterial { get; set; }
        public string GrupoCompras { get; set; }
        public decimal PlazoDeEntregaPrevisto { get; set; }
        public string CuentaDeMayor { get; set; }
        public string TextoAmpliado { get; set; }
    }
}
