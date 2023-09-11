using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class UsuarioModificacionDto
    {
        public int Id { get; set; }
        public string Cuit { get; set; }
        public string Mail { get; set; }
        public string UsuarioModificacion { get; set; }
        public int IdTipoUsuario { get; set; }
        public List<ProveedoresModificacionDto> Proveedores { get; set; }
    }
    public class ProveedoresModificacionDto
    {
        public int Id { get; set; }
        public string Cuit { get; set; }
        public string RazonSocial     {get; set;}
        public string CodigoProveedor {get; set;}
        public int IdTipoProveedor {get; set;}
        public bool EsRevendedor { get; set; }
    }
}
