using SustitucionMOAModel.Models.ViewModel;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOAModel.Models.WSMapMOA.ReporteContrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.Compras
{
    public class IngresanteInfoEditableDto
    { 
        public int ID { get; set; }
        public string ColumnaEditar { get; set; }
        public string NuevoValor { get; set; }
    }
}
