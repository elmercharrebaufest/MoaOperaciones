using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services.Email.Dto
{
    public class MailPeticionDeOfertaRequest
    {
        public PeticionDeOferta PeticionDeOferta { get; set; }

        public List<string> Destinatarios { get; set; } = new List<string>();

        public List<FileDto> ListaArchivosParaMailPO { get; set; } = new List<FileDto>();

        public bool EsProveedor { get; set; } = true;

        public bool EsEdicionPO { get; set; } = false;

        public Dictionary<string, byte[]> ArchivosAdjuntos { get; set; } = new Dictionary<string, byte[]>();

        public List<string> Proveedores { get; set; } = null;
    }
}
