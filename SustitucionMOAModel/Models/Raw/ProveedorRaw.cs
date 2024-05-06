using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.Raw
{
    public class ProveedorRaw
    {
        public int Id { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string CodigoProveedor { get; set; }
        public string CodigoCorredor { get; set; }
        public string RazonSocialCorredor { get; set; }
        public int IdTipoUsuario { get; set; }
        public ProveedorRaw(Proveedor p)
        {
            Id = p.Id;
            CUIT = p.CUIT?? "-";
            RazonSocial = p.RazonSocial?? "-";
            CodigoProveedor = p.CodigoProveedor?? "-";
            CodigoCorredor = p.ProveedorCorredor != null? p.ProveedorCorredor.CodigoProveedor : null;
            RazonSocialCorredor = p.ProveedorCorredor != null? p.ProveedorCorredor.RazonSocial : null;
            IdTipoUsuario = p.TipoProveedor.Id;
        }
    }
}
