using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class InfoProveedorDataAgroDto
    {
        public string ProveedorCBU { get; set; }
        public string ProveedorClasificacion { get; set; }
        public string estadoSISA { get; set; }

        public override bool Equals(object obj)
        {
            return obj is InfoProveedorDataAgroDto dto &&
                   ProveedorCBU == dto.ProveedorCBU &&
                   ProveedorClasificacion == dto.ProveedorClasificacion &&
                   estadoSISA == dto.estadoSISA;
        }
    }
}
