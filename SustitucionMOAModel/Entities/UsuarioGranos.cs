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
        private UsuarioGranos() : base() { }

        public UsuarioGranos(string mail, string CUIT) : base(mail, CUIT) {
            TipoUsuario = TipoUsuario.GetTipoGranos();
        }

        public string Comercial { get; set; }
        public string RutaInformeComercialFirmado { get; set; }
        public string RutaConstanciaCBU { get; set; }
        public string RutaConstanciaCUIT { get; set; }
        public string RutaInscripcionIIBB { get; set; }
        public string RutaGananciasIVAIIBB { get; set; }
        public string RutaSIPER { get; set; }
        public string RutaDocumentacionEnBolsa { get; set; }

    }
}
