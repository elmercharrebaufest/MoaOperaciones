using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.AltaEmpresa
{
    public class ExistenciaEmpresaResponse
    {
        public string Mensaje { get; set; }

        public string Error { get; set; }

        public List<EmpresaExistenciaVerificada> EmpresasExistentes { get; set; } = new List<EmpresaExistenciaVerificada>();
    }

    public class EmpresaExistenciaVerificada
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Mail { get; set; }
    }
}
