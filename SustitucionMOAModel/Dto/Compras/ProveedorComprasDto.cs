using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class ProveedorComprasDto
    {
        public int Proveedor_Id { get; set; }//Tabla proveedor
        public int Usuario_Id { get; set; }//Tabla usuario
        public string Mail { get; set; }
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public string CodigoProveedor { get; set; }
        public ProveedorComprasDto() { }

    }
}
