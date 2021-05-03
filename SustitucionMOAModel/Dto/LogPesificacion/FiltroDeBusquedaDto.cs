using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.LogPesificacion
{
    public class FiltroDeBusquedaDto
    {
        public int IdUsuario { get; set; }
        public string Fecha { get; set; }

        public int? Contrato { get; set; }

        public int? Fijacion { get; set; }

        public string Mail { get; set; }

        public string Proveedor { get; set; }


        public DateTime FechaDT {
            get {
               return  string.IsNullOrEmpty( this.Fecha)? DateTime.MinValue : DateTime.Parse(this.Fecha);
            }
        }
    }
}
