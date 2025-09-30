using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerFuenteAprovisionamientoWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerFuenteAprovisionamientoConsumerMOA : IObtenerFuenteAprovisionamientoConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];

        public ObtenerFuenteAprovisionamientoConsumerMOA()
        {

        }

        public FuenteAprovisionamientoWSMOAResponse request(string fechaEntregaPosicion, string numeroMaterial, string centro)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    string IM_DELIV_DATE = fechaEntregaPosicion;
                    string IM_MATERIAL = numeroMaterial;
                    string IM_PLANT = centro;
                    string IM_VENDOR = "";

                    var request = new Z_MMRFC_OBTENER_FUENTE_APROV()
                    {
                        IM_DELIV_DATE = IM_DELIV_DATE,
                        IM_MATERIAL = IM_MATERIAL,
                        IM_PLANT = IM_PLANT,
                        IM_VENDOR = IM_VENDOR
                    };
                    Log.Info($"SAP sin PI Z_MMRFC_OBTENER_FUENTE_APROV request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MMRFC_OBTENER_FUENTE_APROV(request);
                    Log.Info($"SAP sin PI Z_MMRFC_OBTENER_FUENTE_APROV response");
                    //Log.Info(response.ToXml());

                    return MapSinPI(response);
                }
                else
                {
                    SI_MMRFC_OBTENER_FUENTE_APROVClient fuenteAprov;
                    fuenteAprov = new SI_MMRFC_OBTENER_FUENTE_APROVClient();
                    fuenteAprov.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    fuenteAprov.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    string IM_DELIV_DATE = fechaEntregaPosicion;
                    string IM_MATERIAL = numeroMaterial;
                    string IM_PLANT = centro;
                    string IM_VENDOR = "";
                    ObtenerFuenteAprovisionamientoWebServiceMOA.ZMPES5850[] EX_FUENTE = new ObtenerFuenteAprovisionamientoWebServiceMOA.ZMPES5850[] { };
                    ObtenerFuenteAprovisionamientoWebServiceMOA.BAPIRETURN[] EX_RETURN = new ObtenerFuenteAprovisionamientoWebServiceMOA.BAPIRETURN[] { };

                    string resultado = fuenteAprov.SI_MMRFC_OBTENER_FUENTE_APROV(IM_DELIV_DATE, IM_MATERIAL, IM_PLANT, IM_VENDOR, out EX_FUENTE, out EX_RETURN);

                    return map(resultado, EX_FUENTE, EX_RETURN);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected virtual FuenteAprovisionamientoWSMOAResponse MapSinPI(Z_MMRFC_OBTENER_FUENTE_APROVResponse response)
        {
            FuenteAprovisionamientoWSMOAResponse result = new FuenteAprovisionamientoWSMOAResponse();
            result.ContratosAprovisionamiento = new List<FuenteAprovisionamiento> { };


            foreach (var contrato in response.EX_FUENTE.Where(a => !string.IsNullOrEmpty(a.AGREEMENT)).ToList())
            {
                result.ContratosAprovisionamiento.Add(new FuenteAprovisionamiento()
                {
                    ProveedorFijo = contrato.FIXED_VEND, //FIXED_VEND FLIEF   Proveedor fijo
                    NombreProveedor = contrato.NAM_VENDOR, //NAM_VENDOR LFA1-NAME1 Nombre del Proveedor
                    CentroAprovisionamiento = contrato.SUPPL_PLNT, //SUPPL_PLNT BEWRK   Centro desde el cual se aprovisiona el material
                    NumeroContratoSuperior = contrato.AGREEMENT, //AGREEMENT KONNR   Número del contrato superior
                    NumeroPosicionContratoSuperior = contrato.AGMT_ITEM, //AGMT_ITEM KTPNR   Número de posición del contrato superior
                    NumeroRegistroInfoCompras = contrato.INFO_REC, //INFO_REC INFNR   Número del registro info de compras
                    TipoDocumentoCompras = contrato.DOC_CAT, //DOC_CAT BSTYP   Tipo de documento de compras
                    OrganizacionCompras = contrato.PURCH_ORG, //PURCH_ORG   EKORG Organización de compras
                    UnidadMedida = contrato.PO_UNIT, //PO_UNIT BSTME   Unidad de medida de pedido
                    TipoPosicionDocumento = contrato.ITEM_CAT, //ITEM_CAT    PSTYP Tipo de posición del documento de compras
                    NumeroMaterial = contrato.MATERIAL, //MATERIAL MATNR18 Número de material (18 caracteres)
                    TipoPosicionDocumentoCompras = contrato.ITEM_CAT_EXT ///ITEM_CAT_EXT EPSTP   Tipo de posición del documento de compras

                });
            }


            result.error = response.EX_EXITO;

            return result;
        }

        protected virtual FuenteAprovisionamientoWSMOAResponse map(string resultado, ObtenerFuenteAprovisionamientoWebServiceMOA.ZMPES5850[] EX_FUENTE, ObtenerFuenteAprovisionamientoWebServiceMOA.BAPIRETURN[] IM_RETURN)
        {
            FuenteAprovisionamientoWSMOAResponse result = new FuenteAprovisionamientoWSMOAResponse();
            result.ContratosAprovisionamiento = new List<FuenteAprovisionamiento> { };


            foreach (var contrato in EX_FUENTE.Where(a => !string.IsNullOrEmpty(a.AGREEMENT)).ToList())
            {
                result.ContratosAprovisionamiento.Add(new FuenteAprovisionamiento()
                {
                    ProveedorFijo = contrato.FIXED_VEND, //FIXED_VEND FLIEF   Proveedor fijo
                    NombreProveedor = contrato.NAM_VENDOR, //NAM_VENDOR LFA1-NAME1 Nombre del Proveedor
                    CentroAprovisionamiento = contrato.SUPPL_PLNT, //SUPPL_PLNT BEWRK   Centro desde el cual se aprovisiona el material
                    NumeroContratoSuperior = contrato.AGREEMENT, //AGREEMENT KONNR   Número del contrato superior
                    NumeroPosicionContratoSuperior = contrato.AGMT_ITEM, //AGMT_ITEM KTPNR   Número de posición del contrato superior
                    NumeroRegistroInfoCompras = contrato.INFO_REC, //INFO_REC INFNR   Número del registro info de compras
                    TipoDocumentoCompras = contrato.DOC_CAT, //DOC_CAT BSTYP   Tipo de documento de compras
                    OrganizacionCompras = contrato.PURCH_ORG, //PURCH_ORG   EKORG Organización de compras
                    UnidadMedida = contrato.PO_UNIT, //PO_UNIT BSTME   Unidad de medida de pedido
                    TipoPosicionDocumento = contrato.ITEM_CAT, //ITEM_CAT    PSTYP Tipo de posición del documento de compras
                    NumeroMaterial = contrato.MATERIAL, //MATERIAL MATNR18 Número de material (18 caracteres)
                    TipoPosicionDocumentoCompras = contrato.ITEM_CAT_EXT ///ITEM_CAT_EXT EPSTP   Tipo de posición del documento de compras

                });
            }


            result.error = resultado;

            return result;
        }
    }

    public interface IObtenerFuenteAprovisionamientoConsumerMOA
    {
        FuenteAprovisionamientoWSMOAResponse request(string fechaEntregaPosicion, string numeroMaterial, string centro);

    }
}

