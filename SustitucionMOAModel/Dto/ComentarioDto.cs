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
        public UsuarioDto Usuario { get; set; }
        public bool? Recordado { get; set; }
        public DateTime? FechaRecordado { get; set; }
        public IList<ArchivoDto> Archivos { get; set; }
        public IList<ComentarioRecordadoDto> ComentarioRecordados { get; set; }
        public int Consulta_Id { get; set; }
        public bool CreadorInterno { get; set; }

        public ComentarioDto() { }

        public ComentarioDto(Comentario comentario)
        {
            Id = comentario.Id;
            Detalle = comentario.Detalle;
            Fecha = comentario.Fecha;
            UsuarioId = comentario.Usuario_Id;
            if (comentario.Archivos != null)
            {
                Archivos = comentario.Archivos.Select(a => new ArchivoDto(a)).ToList();
            };
            Usuario = new UsuarioDto(comentario.Usuario);
        }
    }
}
