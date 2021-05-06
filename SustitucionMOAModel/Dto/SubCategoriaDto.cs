using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class SubCategoriaDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Nombre { get; set; }
        public int CategoriaId { get; set; }

        public SubCategoriaDto() { }

        public SubCategoriaDto(SubCategoria subcategoria)
        {
            Id = subcategoria.Id;
            Code = subcategoria.Code;
            Nombre = subcategoria.Nombre;
            CategoriaId = subcategoria.Categoria_Id;
        }
    }
}
