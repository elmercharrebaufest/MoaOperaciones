using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class CategoriaDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Nombre { get; set; }
        public bool CamposAdicionales { get; set; }

        public CategoriaDto(Categoria categoria)
        {
            Id = categoria.Id;
            Code = categoria.Code;
            Nombre = categoria.Nombre;
            CamposAdicionales = categoria.CamposAdicionales;
        }
    }
}
