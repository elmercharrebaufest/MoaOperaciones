using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.Compras
{
    public class CertificacionDto
    {
        public string NombreDeArchivo { get; set; }
        public string NRO_OC { get; set; }
        public string NRO_Certificacion { get; set; }
        public float Importe { get; set; }
        public string Moneda { get; set; }
        public string FechaDeRegistro { get; set; }
        public int UsuarioId { get; set; }
        public int ProveedorId { get; set; }
        public int ArchivoId { get; set; }
    }
}