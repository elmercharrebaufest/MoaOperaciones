using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerMaterialesSolpWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerMaterialesSolpConsumerMOA : IObtenerMaterialesSolpConsumerMOA
    {
        SI_MMRFC_OBTENER_MATERIALESClient service;

        public ObtenerMaterialesSolpConsumerMOA()
        {
            service = new SI_MMRFC_OBTENER_MATERIALESClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public MaterialWSMOAResponse request(List<string> CentroCodigo, string NombreDeMaterial)
        {
            try
            {
                ZMPES5800[] IM_MATERIAL = new ZMPES5800[] { };
                ZMPES5810[] IM_MATL_DESC = new ZMPES5810[] { };
                ZMPES5830[] IM_MATL_GROUP = new ZMPES5830[] { };

                //ZMPES5820[] IM_PLANT = new List<ZMPES5820>{ new ZMPES5820 { SIGN = "I", OPTION = "EQ", LOW = CentroCodigo },  }.ToArray();

                byte IM_MAX = Convert.ToByte(0);
                var result = new MaterialWSMOAResponse();
                result.Materiales = new List<Material>();
                foreach (var centros in CentroCodigo)
                {
                    var IM_PlantList = new List<ZMPES5820>();
                    IM_PlantList.Add(new ZMPES5820 { SIGN = "I", OPTION = "EQ", LOW = centros });
                    ZMPES5820[] IM_PLANT = IM_PlantList.ToArray();
                    string error = service.SI_MMRFC_OBTENER_MATERIALES(IM_MATERIAL, IM_MATL_DESC, IM_MATL_GROUP, IM_MAX, IM_PLANT, out ZMPES5840[] EX_MATERIAL, out BAPIRETURN[] EX_RETURN);
                    var resultado = map(error, EX_MATERIAL, EX_RETURN);
                    result.Materiales.AddRange(resultado.Materiales);
                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected virtual MaterialWSMOAResponse map(string error, ZMPES5840[] EX_MATERIAL, BAPIRETURN[] IM_RETURN)
        {
            MaterialWSMOAResponse result = new MaterialWSMOAResponse();
            result.Materiales = new List<Material> { };

            if (error == "200")
            {
                foreach (ZMPES5840 materialSolp in EX_MATERIAL)
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