using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                this.Id = materialSolp.Id;
                this.Codigo = materialSolp.Codigo;
                this.CodigoSap = materialSolp.CodigoSap;
                this.Descripcion = materialSolp.Descripcion;
                this.TipoMaterial = materialSolp.TipoMaterial;
                this.TipoValoracion = materialSolp.TipoValoracion;
                this.PrecioMaterial = materialSolp.PrecioMaterial;
                this.Estado = materialSolp.Estado;
                this.GrupoArticulo = new TablaSapDto(materialSolp.GrupoArticulo);
                this.CentroLogistico = new TablaSapDto(materialSolp.CentroLogistico);
                this.UnidadMedidaBase = new TablaSapDto(materialSolp.UnidadMedidaBase);
                this.UnidadMedidaCompras = new TablaSapDto(materialSolp.UnidadMedidaCompras);
                this.UnidadMedidaSalida = new TablaSapDto(materialSolp.UnidadMedidaSalida);
                this.GrupoCompras = new TablaSapDto(materialSolp.GrupoCompras);
                this.CuentaMayor = new TablaSapDto(materialSolp.CuentaMayor);
            }
        }

        public MaterialSolpDto() { }
    }
}
