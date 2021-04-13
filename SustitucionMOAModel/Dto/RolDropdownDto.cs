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
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Code { get; set; }

        public RolDropdownDto() { }
        public RolDropdownDto(Rol rol)
        {
            Id = rol.Id;
            Nombre = rol.Nombre;
            Code = rol.Codigo;
        }
    }
}
