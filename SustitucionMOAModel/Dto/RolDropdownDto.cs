using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class RolDropdownDto
    {
        public int value { get; set; }

        public string label { get; set; }


        public RolDropdownDto(Rol rol)
        {
            value = rol.Id;
            label = rol.Nombre;
        }
    }
}
