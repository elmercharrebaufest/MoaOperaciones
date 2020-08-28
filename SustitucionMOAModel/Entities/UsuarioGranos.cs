using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    [Table("UsuarioGranos")]
    public class UsuarioGranos : Usuario
    {
        public string Comercial { get; set; }
        public string RutaInformeComercialFirmado { get; set; }
        public string RutaConstanciaCBU { get; set; }
        public string RutaConstanciaCBUMercaderia { get; set; }
        public string RutaConstanciaCUIT { get; set; }
        public string RutaInscripcionIIBB { get; set; }

        public string RutaCertificadoExclusionIVA { get; set; }
        public string RutaCertificadoExclusionIIBB { get; set; }
        public string RutaCertificadoExclusionSUSS { get; set; }

        public string RutaCertificadoExclusionGanancias { get; set; }

        public string RutaSIPER { get; set; }
        public string RutaDocumentacionEnBolsa { get; set; }

    }
}
