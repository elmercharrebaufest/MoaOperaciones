using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SustitucionMOAModel.Enums;

namespace SustitucionMOAModel.Dto
{
    public class ProveedorDto
    {
        public int Id { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string CodigoProveedor { get; set; }
        public string Mail { get; set; }
        public EstadoAprobacion EstadoAprobacion { get; set; }
        public string Observaciones { get; set; }

        public int? IdDataAgro { get; set; }
        public int? IdComercialDataAgro { get; set; }
        public string EstadoAprobacionDescripcion { get; set; }

        public virtual List<ProveedorHistorialAprobacionDto> HistorialAprobaciones { get; set; }
        public string Comercial { get; set; }
        public string SISAEstadoCuit { get; set; }
        public string EstadoSIPER { get; set; }
    }
}
