using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class ComentarioDto
    {
        public int Id { get; set; }
        public string Detalle { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioId { get; set; }
        public IList<ArchivoDto> Archivos { get; set; }

        public ComentarioDto(Comentario comentario)
        {
            Id = comentario.Id;
            Detalle = comentario.Detalle;
            Fecha = comentario.Fecha;
            UsuarioId = comentario.Usuario_Id;
            //Archivos = comentario.Archivos.Select(a => new ArchivoDto(a)).ToList();
        }
    }
}
