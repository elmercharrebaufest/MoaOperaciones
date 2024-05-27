using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class HistorialDeFechaDto
    {
        public List<SolpDto> ListaSolp { get; set; }
        public string FechaCreacionPOFormateada { get; set; }
        public List<List<string>> Cuerpo { get; set; }
    }

    public class HistorialPorProveedorDto
    {
        public string RazonSocial { get; set; }
        public List<DateTime> FechasCirculares { get; set; }
        public List<DateTime> FechasCotizaciones { get; set; }
        public List<DateTime> FechaOrdenDeCompraCreacion { get; set; }
        public IEnumerable<DateTime> FechaOrdenDeCompraLiberacion { get; set; }

        public List<Usuario> UsuariosCirculares { get; set; }
    }
}
