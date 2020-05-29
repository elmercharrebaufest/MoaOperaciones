using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Proforma
{
    public class ProcedenciaFleteWSMOAResponse
    {
        public List<ProcedenciaFleteView> procedenciasFlete { get; set; }
        public string error { get; set; }

        public ProcedenciaFleteWSMOAResponse()
        {
            this.procedenciasFlete = new List<ProcedenciaFleteView>() { };
        }
    }

    public class ProcedenciaFleteExcelWSMOAResponse
    {
        public List<ProcedenciaFlete> procedenciasFlete { get; set; }
        public string error { get; set; }

        public ProcedenciaFleteExcelWSMOAResponse()
        {
            this.procedenciasFlete = new List<ProcedenciaFlete>() { };
        }
    }
}
