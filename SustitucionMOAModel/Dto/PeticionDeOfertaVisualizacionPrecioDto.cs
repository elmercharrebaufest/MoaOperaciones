using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class PeticionDeOfertaVisualizacionPrecioDto
    {
        public int Id { get; set; }
        public int PeticionDeOferta_Id { get; set; }
        public int Archivo_Id { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Observacion { get; set; }

        public List<ArchivoDto> Adjuntos { get; set; }
    }
}
