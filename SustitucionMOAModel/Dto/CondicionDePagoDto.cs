using SustitucionMOAModel.Entities;
using System.Collections.Generic;
using System.IO;

namespace SustitucionMOAModel.Dto
{
    public class CondicionDePagoDto
    {       
        
        public string CondicionDeImportacionCodigo { get; set; }
        public string CondicionDeImportacionComplemento { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int? Id { get; set; }
        public string CodigoDescripcion { get; set; }
    }

    public class CondicionDeImportacionDto
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int? Id { get; set; }
        public string CodigoDescripcion { get; set; }
    }
}
