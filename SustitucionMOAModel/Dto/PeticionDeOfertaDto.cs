using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class PeticionDeOfertaDto
    {
        public int Id { get; set; }
        public int Solp_Id { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime PlazoDeOferta { get; set; }
        public string Observaciones { get; set; }
        public string PlazoDeOfertaFormateado { get { return PlazoDeOferta.ToString("dd/MM/yyyy"); } }
        public string Estado { get { return PlazoDeOferta >= DateTime.Now.Date ? "Abierto" : "Cerrado"; } }
        public string EstadoColor { get { return PlazoDeOferta >= DateTime.Now.Date ? "Green" : "Red"; } }
    }
}
