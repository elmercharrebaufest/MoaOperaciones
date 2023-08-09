using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Compras
{
    public class FuenteAprovisionamiento
    {
        //Estruct.transfer.p.búsqueda fte.aprovision.
        public string ProveedorFijo { get; set; } //FIXED_VEND FLIEF   Proveedor fijo
        public string NombreProveedor { get; set; } //NAM_VENDOR LFA1-NAME1 Nombre del Proveedor
        public string CentroAprovisionamiento { get; set; } //SUPPL_PLNT BEWRK   Centro desde el cual se aprovisiona el material
        public string NumeroContratoSuperior { get; set; } //AGREEMENT KONNR   Número del contrato superior
        public string NumeroPosicionContratoSuperior { get; set; } //AGMT_ITEM KTPNR   Número de posición del contrato superior
        public string NumeroRegistroInfoCompras { get; set; } //INFO_REC INFNR   Número del registro info de compras
        public string TipoDocumentoCompras { get; set; } //DOC_CAT BSTYP   Tipo de documento de compras
        public string OrganizacionCompras { get; set; } //PURCH_ORG   EKORG Organización de compras
        public string UnidadMedida { get; set; } //PO_UNIT BSTME   Unidad de medida de pedido
        public string TipoPosicionDocumento { get; set; } //ITEM_CAT    PSTYP Tipo de posición del documento de compras
        public string NumeroMaterial { get; set; } //MATERIAL MATNR18 Número de material (18 caracteres)
        public string TipoPosicionDocumentoCompras { get; set; } ///ITEM_CAT_EXT EPSTP   Tipo de posición del documento de compras
    }
}
