using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.VendedoresWebServiceMOA;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class VendedoresConsumerMOA: IVendedoresConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];
        private readonly SI_MPMF_MOAOP_VENDEDORESClient service;
        public VendedoresConsumerMOA()
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MPMF_MOAOP_VENDEDORES&amp;interfaceNamespace=urn%3AOPERACIONES";
            service = new SI_MPMF_MOAOP_VENDEDORESClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }


        public VendedoresWSMOAResponse Request(string proveedor, List<FechaWS> fechas)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;


                    List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100> fechasSAP = new List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100>() { };
                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4340[] salidas = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4340[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();
                    var request = new Z_MPMF_MOAOP_VENDEDORES()
                    {
                        PE_PROVEEDOR = proveedor,
                        T_FECHA_IN = fechasSAPArray,
                        T_SALIDA = salidas
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_VENDEDORES request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_VENDEDORES(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_VENDEDORES response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response);
                }
                else
                {
                    VendedoresWebServiceMOA.ZMPES4340[] salidas = new VendedoresWebServiceMOA.ZMPES4340[] { };
                    List<VendedoresWebServiceMOA.ZMPES4100> fechasSAP = new List<VendedoresWebServiceMOA.ZMPES4100>() { };
                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new VendedoresWebServiceMOA.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }
                    VendedoresWebServiceMOA.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();
                    VendedoresWebServiceMOA.ZMPES4910 error = service.SI_MPMF_MOAOP_VENDEDORES(proveedor, ref fechasSAPArray, ref salidas);
                    VendedoresWSMOAResponse result = Map(error, salidas);
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private VendedoresWSMOAResponse MapSinPI(Z_MPMF_MOAOP_VENDEDORESResponse response)
        {
            VendedoresWSMOAResponse result = new VendedoresWSMOAResponse();
            if (response.MENSAJE_ERROR != null)
            {
                result.error.codigo = response.MENSAJE_ERROR.CODIGO;
                result.error.descripcion = response.MENSAJE_ERROR.DESCRIPCION;
                result.error.tipo = response.MENSAJE_ERROR.TIPO;
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4340 vendedor in response.T_SALIDA)
            {
                result.vendedores.Add(new Vendedor()
                {
                    descVendedor = vendedor.DESC_VEND,
                    estado = vendedor.ESTADO,
                    estadoMoa = "Habilitado",
                    fecha = vendedor.FECHA,
                    idVendedor = vendedor.ID_VENDEDOR,
                    cuit = vendedor.CUIT_VEND
                });
            }
            return result;
        }

        private VendedoresWSMOAResponse Map(VendedoresWebServiceMOA.ZMPES4910 error, VendedoresWebServiceMOA.ZMPES4340[] salidas)
        {
            VendedoresWSMOAResponse result = new VendedoresWSMOAResponse();
            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (VendedoresWebServiceMOA.ZMPES4340 vendedor in salidas)
            {
                result.vendedores.Add(new Vendedor()
                {
                    descVendedor = vendedor.DESC_VEND,
                    estado = vendedor.ESTADO,
                    estadoMoa = "Habilitado",
                    fecha = vendedor.FECHA,
                    idVendedor = vendedor.ID_VENDEDOR,
                    cuit = vendedor.CUIT_VEND
                });
            }
            return result;
        }
    }

    public interface IVendedoresConsumerMOA
    {
        VendedoresWSMOAResponse Request(string proveedor, List<FechaWS> fechas);

    }
}
