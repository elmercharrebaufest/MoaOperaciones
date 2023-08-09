using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Dto;

namespace SustitucionMOAModel.Models.WSMapMOA.Compras
{
    public class MaterialWSMOAResponse
    {
        public string error { get; set; }
        public List<Material> Materiales { get; set; }

    }
}
