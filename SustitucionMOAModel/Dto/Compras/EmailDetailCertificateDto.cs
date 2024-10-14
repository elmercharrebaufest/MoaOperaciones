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
    public class EmailDetailCertificateDto
    {
        public string Destinatario { get; set; }
        public string MotivoRechazo { get; set; } = null;
        public string Proveedor { get; set; }
        public string NumeroCertificacion { get; set; }
        public string FechaCertificacion { get; set; }
        public string Descripcion { get; set; }
        public string MontoTotal { get; set; }
        public string GeneradoPor { get; set; } = null;
        public string NroOC { get; set; }
        public string Moneda { get; set; }
        public string NumeroPosicion { get; set; } = null;
        public string Aprobador { get; set; } = null;

        public List<ServiceDetailDto> DetalleServicio { get; set; }
    }

    public class ServiceDetailDto
    {
        public string Descripcion { get; set; }
        public string Cantidad { get; set; }
        public string UM { get; set; }
        public string Porcentaje { get; set; }
        public string Monto { get; set; }
        
    }
}
