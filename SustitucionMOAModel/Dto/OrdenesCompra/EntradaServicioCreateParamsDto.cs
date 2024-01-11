using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    public class EntradaServicioCreateParamsDto
    {
        public EntrySheetHeaderSection EntrySheetHeader { get; set; }
        public EntrySheetServiceSection EntrySheetServices { get; set; }
    }


    public class EntrySheetHeaderSection
    {
        public string PaqueteNumero { get; set; }
        public string Descripcion { get; set; }
        public string OrdenCompraNumero { get; set; }
        public string OrdenCompraPosicionNumero { get; set; }
        public string DocumentoReferenciaNumero { get; set; }
        public string FechaDocumento { get; set; }
        public string FechaContabilizacion { get; set; }
        public bool GrabarAceptada { get; set; }
    }


    public class EntrySheetServiceSection
    {
        public List<EntrySheetServiceItemSection> Items { get; set; }
    }


    public class EntrySheetServiceItemSection
    {
        public string PackageNumber { get; set; }
        public string LineNumber { get; set; }
        public string OutlineIndicator { get; set; }
        public string SubPackageNumber { get; set; }
        public string ExternalLineNumber { get; set; }
        public string Service { get; set; }
        public string Quantity { get; set; }
        public decimal GrossPrice { get; set; }
        public string ShortText { get; set; }
        public string PlannedPackage { get; set; }
        public string PlannedLine { get; set; }
    }
}
