using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Flete
{
    public class Viaje
    {
        public DateTime fechaCCPPDate { get; set; }
        public string fechaCCPP { get; set; }
        public string nroProforma { get; set; }
        public string ccpp { get; set; }
        public string patente { get; set; }
        public string kgString { get; set; }
        public decimal kg { get; set; }
        public string descMat { get; set; }
        public string origen { get; set; }
        public string destino { get; set; }
        public string tarifaString { get; set; }
        public decimal tarifa { get; set; }
        public string peajeString { get; set; }
        public decimal peaje { get; set; }
        public string playaString { get; set; }
        public decimal playa { get; set; }
        public string importeString { get; set; }
        public decimal importe { get; set; }
        public string status { get; set; }
        public string factura { get; set; }
    }

    public class ViajeExcel
    {
        public string fechaCCPP { get; set; }
        public string nroProforma { get; set; }
        public string ccpp { get; set; }
        public string patente { get; set; }
        public decimal kg { get; set; }
        public string descMat { get; set; }
        public string origen { get; set; }
        public string destino { get; set; }
        public decimal tarifa { get; set; }
        public decimal peaje { get; set; }
        public decimal playa { get; set; }
        public decimal importe { get; set; }
        public string status { get; set; }
        public string factura { get; set; }
    }

    public class ViajeAgrupado
    {
        public string proforma { get; set; }
        public string proveedor { get; set; }
        public string proveedorId { get; set; }
        public string regionId { get; set; }
        public string region { get; set; }
        public string fecha { get; set; }
        public string totalKg { get; set; }
        public string totalImporteString { get; set; }
        public decimal totalImporte { get; set; }
        public string factura { get; set; }
        public string fechaEmision { get; set; }
        public DateTime fechaEmisionDate { get; set; }
        public List<Viaje> viajeItem { get; set; }
    }

    public class ViajeAgrupadoExcel
    {
        public string proforma { get; set; }
        public string proveedor { get; set; }
        public string proveedorId { get; set; }
        public string regionId { get; set; }
        public string region { get; set; }
        public string fecha { get; set; }
        public string totalKg { get; set; }
        public decimal totalImporte { get; set; }
        public string factura { get; set; }
        public string fechaEmision { get; set; }
        public List<ViajeExcel> viajeItem { get; set; }
    }
}
