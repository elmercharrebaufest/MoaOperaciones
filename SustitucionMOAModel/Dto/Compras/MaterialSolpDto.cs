using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class MaterialSolpDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string CodigoSap { get; set; }
        public string Descripcion { get; set; }
        public string TipoMaterial { get; set; }
        public string TipoValoracion { get; set; }
        public decimal? PrecioMaterial { get; set; }
        public bool Estado { get; set; }
        public TablaSapDto GrupoArticulo { get; set; }
        public TablaSapDto CentroLogistico { get; set; }
        public TablaSapDto UnidadMedidaBase { get; set; }
        public TablaSapDto UnidadMedidaCompras { get; set; }
        public TablaSapDto UnidadMedidaSalida { get; set; }
        public TablaSapDto GrupoCompras { get; set; }
        public TablaSapDto CuentaMayor { get; set; }

        public MaterialSolpDto(MaterialSolp materialSolp)
        {
            if (materialSolp != null)
            {
                Id = materialSolp.Id;
                Codigo = materialSolp.Codigo;
                CodigoSap = materialSolp.CodigoSap;
                Descripcion = materialSolp.Descripcion;
                TipoMaterial = materialSolp.TipoMaterial;
                TipoValoracion = materialSolp.TipoValoracion;
                PrecioMaterial = materialSolp.PrecioMaterial;
                Estado = materialSolp.Estado;
                GrupoArticulo = new TablaSapDto(materialSolp.GrupoArticulo);
                CentroLogistico = new TablaSapDto(materialSolp.CentroLogistico);
                UnidadMedidaBase = new TablaSapDto(materialSolp.UnidadMedidaBase);
                UnidadMedidaCompras = new TablaSapDto(materialSolp.UnidadMedidaCompras);
                UnidadMedidaSalida = new TablaSapDto(materialSolp.UnidadMedidaSalida);
                GrupoCompras = new TablaSapDto(materialSolp.GrupoCompras);
                CuentaMayor = new TablaSapDto(materialSolp.CuentaMayor);
            }
        }

        public MaterialSolpDto() { }
    }
}
