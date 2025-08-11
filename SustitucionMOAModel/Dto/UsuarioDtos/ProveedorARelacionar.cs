using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.UsuarioDtos
{
    public class ProveedorARelacionar
    {
        public int Id { get; set; }
        
        public string CUIT { get; set; }
        
        public string RazonSocial { get; set; }
        
        public string CodigoProveedor { get; set; }
        
        public int IdTipoProveedor { get; set; }

        public string TipoProveedor { get; set; }
    }
}
