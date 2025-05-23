using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerAdjuntosSOLPEDWebServiceMOA;
using SustitucionMOAWS.ObtenerMaterialesSolpWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerMaterialesSolpConsumerMOA : IObtenerMaterialesSolpConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];


        public ObtenerMaterialesSolpConsumerMOA()
        {

        }

        public MaterialWSMOAResponse request(List<string> CentroCodigo, string NombreDeMaterial)
        {
            try
            {

                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    
                    byte IM_MAX = Convert.ToByte(0);
                    var result = new MaterialWSMOAResponse();
                    result.Materiales = new List<Material>();
                    foreach (var centros in CentroCodigo)
                    {
                        var IM_PlantList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5820>();
                        IM_PlantList.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5820 { SIGN = "I", OPTION = "EQ", LOW = centros });
                        WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5820[] IM_PLANT = IM_PlantList.ToArray();

                        var request = new Z_MMRFC_OBTENER_MATERIALES()
                        {
                            IM_MATERIAL = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5800[] { },
                            IM_MATL_DESC = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5810[] {},
                            IM_MATL_GROUP = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5830[]{},
                            IM_MAX = Convert.ToByte(IM_MAX),
                            IM_PLANT = IM_PLANT
                        };
                        Log.Info($"SAP sin PI Z_MMRFC_OBTENER_MATERIALES request");
                        Log.Info(request.ToXml());
                        var response = agent.Z_MMRFC_OBTENER_MATERIALES(request);
                        Log.Info($"SAP sin PI Z_MMRFC_OBTENER_MATERIALES response");
                        Log.Info(response.ToXml());

                        var resultado = MapSinPI(response);
                        result.Materiales.AddRange(resultado.Materiales);
                    }

                    return result;

                }
                else
                {
                    SI_MMRFC_OBTENER_MATERIALESClient service;
                    var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_OBTENER_MATERIALES&amp;interfaceNamespace=urn%3AOPERACIONES";
                    service = new SI_MMRFC_OBTENER_MATERIALESClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    ObtenerMaterialesSolpWebServiceMOA.ZMPES5800[] IM_MATERIAL   = new ObtenerMaterialesSolpWebServiceMOA.ZMPES5800[] { };
                    ObtenerMaterialesSolpWebServiceMOA.ZMPES5810[] IM_MATL_DESC  = new ObtenerMaterialesSolpWebServiceMOA.ZMPES5810[] { };
                    ObtenerMaterialesSolpWebServiceMOA.ZMPES5830[] IM_MATL_GROUP = new ObtenerMaterialesSolpWebServiceMOA.ZMPES5830[] { };

                    //ZMPES5820[] IM_PLANT = new List<ZMPES5820>{ new ZMPES5820 { SIGN = "I", OPTION = "EQ", LOW = CentroCodigo },  }.ToArray();

                    byte IM_MAX = Convert.ToByte(0);
                    var result = new MaterialWSMOAResponse();
                    result.Materiales = new List<Material>();
                    foreach (var centros in CentroCodigo)
                    {
                        var IM_PlantList = new List<ObtenerMaterialesSolpWebServiceMOA.ZMPES5820>();
                        IM_PlantList.Add(new ObtenerMaterialesSolpWebServiceMOA.ZMPES5820 { SIGN = "I", OPTION = "EQ", LOW = centros });
                        ObtenerMaterialesSolpWebServiceMOA.ZMPES5820[] IM_PLANT = IM_PlantList.ToArray();
                        string error = service.SI_MMRFC_OBTENER_MATERIALES(IM_MATERIAL, IM_MATL_DESC, IM_MATL_GROUP, IM_MAX, IM_PLANT, out ObtenerMaterialesSolpWebServiceMOA.ZMPES5840[] EX_MATERIAL, out ObtenerMaterialesSolpWebServiceMOA.BAPIRETURN[] EX_RETURN);
                        var resultado = map(error, EX_MATERIAL, EX_RETURN);
                        result.Materiales.AddRange(resultado.Materiales);
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private MaterialWSMOAResponse MapSinPI(Z_MMRFC_OBTENER_MATERIALESResponse response)
        {
            MaterialWSMOAResponse result = new MaterialWSMOAResponse();
            result.Materiales = new List<Material> { };

            if (response.EX_EXITO == "200")
            {
                foreach (WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5840 materialSolp in response.EX_MATERIAL)
                {
                    result.Materiales.Add(new Material()
                    {
                        CentroLogistico = materialSolp.PLANT,
                        NroMaterial = materialSolp.MATERIAL,
                        NombreDeMaterial = materialSolp.MATL_DESC,
                        GrupoArticulo = materialSolp.MATL_GROUP,
                        TipoMaterial = materialSolp.MATL_TYPE,
                        UnidadDeMedidaBase = materialSolp.BASE_UOM,
                        UnidadDeMedidaCompras = materialSolp.PURC_UOM,
                        UnidadDeMedidaSalida = materialSolp.AUSM_UOM,
                        TipoValoracion = materialSolp.TIPO_VALOR,
                        ClaseDeValoracion = materialSolp.VAL_TYPE,
                        PrecioDelMaterial = materialSolp.MATL_PRICE,
                        GrupoCompras = materialSolp.PUR_GROUP,
                        PlazoDeEntregaPrevisto = materialSolp.PLND_DELRY,
                        CuentaDeMayor = materialSolp.GL_ACCOUNT,
                        //PermiteComprarContraStock = materialSolp.PERMITE_STOCK,
                        TextoAmpliado = materialSolp.TEXTO_COMPRAS,

                    });
                }
            }

            result.error = response.EX_EXITO;

            return result;
        }
    

        protected virtual MaterialWSMOAResponse map(string error, ObtenerMaterialesSolpWebServiceMOA.ZMPES5840[] EX_MATERIAL, ObtenerMaterialesSolpWebServiceMOA.BAPIRETURN[] IM_RETURN)
        {
            MaterialWSMOAResponse result = new MaterialWSMOAResponse();
            result.Materiales = new List<Material> { };

            if (error == "200")
            {
                foreach (ObtenerMaterialesSolpWebServiceMOA.ZMPES5840 materialSolp in EX_MATERIAL)
                {
                    result.Materiales.Add(new Material()
                    {
                        CentroLogistico = materialSolp.PLANT,
                        NroMaterial = materialSolp.MATERIAL,
                        NombreDeMaterial = materialSolp.MATL_DESC,
                        GrupoArticulo = materialSolp.MATL_GROUP,
                        TipoMaterial = materialSolp.MATL_TYPE,
                        UnidadDeMedidaBase = materialSolp.BASE_UOM,
                        UnidadDeMedidaCompras = materialSolp.PURC_UOM,
                        UnidadDeMedidaSalida = materialSolp.AUSM_UOM,
                        TipoValoracion = materialSolp.TIPO_VALOR,
                        ClaseDeValoracion = materialSolp.VAL_TYPE,
                        PrecioDelMaterial = materialSolp.MATL_PRICE,
                        GrupoCompras = materialSolp.PUR_GROUP,
                        PlazoDeEntregaPrevisto = materialSolp.PLND_DELRY,
                        CuentaDeMayor = materialSolp.GL_ACCOUNT,
                        //PermiteComprarContraStock = materialSolp.PERMITE_STOCK,
                        TextoAmpliado = materialSolp.TEXTO_COMPRAS,

                    });
                }
            }

            result.error = error;

            return result;
        }
    }
}


//PLANT          Centro Logístico
//MATERIAL       Número de Material
//MATL_DESC      Texto Breve o Nombre de Material
//MATL_GROUP     Grupo de Articulo
//MATL_TYPE      Tipo de Material
//BASE_UOM       Unidad de Medida Base
//PURC_UOM       Unidad de Medida de Compras
//AUSM_UOM       Unidad de Medida de Salida
//TIPO_VALOR     Tipo Valoración
//VAL_TYPE       Clase de Valoración
//MATL_PRICE     Precio del Material
//PUR_GROUP      Grupo de Compras
//PLND_DELRY     Plazo de entrega previsto
//GL_ACCOUNT     Cuenta de Mayor
//PERMITE_STOCK  Determina si permite comprar contra Stock