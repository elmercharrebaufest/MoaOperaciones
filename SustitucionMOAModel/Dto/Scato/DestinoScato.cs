using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.Scato
{
    public class DestinoScato
    {
        public int LocalidadId { get; set; }

        public string LocalidadDescripcion { get; set; }

        public int ProvinciaId { get; set; }

        public string ProvinciaDescripcion { get; set; }

        public string KmsARecorrer { get; set; }
    }
}
