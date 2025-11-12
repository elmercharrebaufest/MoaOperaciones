using System;

namespace SustitucionMOAModel.Dto.CampoSustentable
{
    public class SP_CampoProveedorListadoDto
    {
        public int IdScato { get; set; }
        public string NombreCosecha { get; set; }
        public double? HectareasSoja { get; set; }
        public double? HectareasTotales { get; set; }
        public string NombreCampo { get; set; }
        public double? ToneladasAprobadas { get; set; }
        public int CampoCosecha_Id { get; set; }
        public int IdProveedor { get; set; }
        public string CodigoProveedor { get; set; }
        public string RazonSocialProveedor { get; set; }
        public string CUITProveedor { get; set; }
        public string RazonSocialCampoProveedor { get; set; }
        public int CosechaId { get; set; }
        public string MotivoRechazo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string TipoNormativa { get; set; }
        public int TipoNormativaId { get; set; }
        public bool? Validado { get; set; }
        public int? ValidadoPor { get; set; }
        public int? EvidenciaEPA_Id { get; set; }
        public string CUITCampoProveedor { get; set; }
        public string Renspa { get; set; }
    }

}
