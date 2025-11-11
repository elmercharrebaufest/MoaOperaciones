using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerCuentasSolpWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerCuentasSolpConsumerMOA : IObtenerCuentasSolpConsumerMOA
    {
        private const string COMP_CODE = "MOA";
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];


        public ObtenerCuentasSolpConsumerMOA()
        {

        }

        public object request()
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    string IM_COMP_CODE = COMP_CODE;
                    string IM_GL_ACCOUNT = "";
                    var request = new Z_MMRFC_OBTENER_CUENTAS()
                    {
                        IM_COMP_CODE = IM_COMP_CODE,
                        IM_GL_ACCOUNT = IM_GL_ACCOUNT
                    };
                    Log.Info($"SAP sin PI Z_MMRFC_OBTENER_CUENTAS request");
                    Log.Info(request.ToXml());

                    var response = agent.Z_MMRFC_OBTENER_CUENTAS(request);

                    SapLogHelper.LogResponse(response.ToXml(), "Z_MMRFC_OBTENER_CUENTAS");

                    return MapSinPI(response);
                }
                else
                {
                    SI_MMRFC_OBTENER_CUENTASClient service;
                    var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_OBTENER_CUENTAS&amp;interfaceNamespace=urn%3AOPERACIONES";

                    service = new SI_MMRFC_OBTENER_CUENTASClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    string IM_COMP_CODE = COMP_CODE;
                    string IM_GL_ACCOUNT = "";
                    ObtenerCuentasSolpWebServiceMOA.ZMPES5760[] EX_GL_ACCOUNT_LIST = new ObtenerCuentasSolpWebServiceMOA.ZMPES5760[] { };
                    ObtenerCuentasSolpWebServiceMOA.BAPIRETURN[] EX_RETURN = new ObtenerCuentasSolpWebServiceMOA.BAPIRETURN[] { };

                    string error = service.SI_MMRFC_OBTENER_CUENTAS(IM_COMP_CODE, IM_GL_ACCOUNT, out EX_GL_ACCOUNT_LIST, out EX_RETURN);
                    return Map(error, EX_GL_ACCOUNT_LIST, EX_RETURN);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private object MapSinPI(Z_MMRFC_OBTENER_CUENTASResponse response)
        {
            CuentaWSMOAResponse result = new CuentaWSMOAResponse();
            result.Cuentas = new List<Cuenta> { };

            if (response.EX_EXITO == "200")
            {
                foreach (WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5760 cuentaSolp in response.EX_GL_ACCOUNT_LIST)
                {
                    result.Cuentas.Add(new Cuenta()
                    {
                        Codigo = cuentaSolp.GL_ACCOUNT,
                        Descripcion = cuentaSolp.SHORT_TEXT
                    });
                }
                result.error = response.EX_EXITO;
            }

            return result;
        }

        protected virtual object Map(string error, ObtenerCuentasSolpWebServiceMOA.ZMPES5760[] EX_GL_ACCOUNT_LIST, ObtenerCuentasSolpWebServiceMOA.BAPIRETURN[] EX_RETURN)
        {
            CuentaWSMOAResponse result = new CuentaWSMOAResponse();
            result.Cuentas = new List<Cuenta> { };

            if (error == "200")
            {
                foreach (ObtenerCuentasSolpWebServiceMOA.ZMPES5760 cuentaSolp in EX_GL_ACCOUNT_LIST)
                {
                    result.Cuentas.Add(new Cuenta()
                    {
                        Codigo = cuentaSolp.GL_ACCOUNT,
                        Descripcion = cuentaSolp.SHORT_TEXT
                    });
                }
                result.error = error;
            }

            return result;
        }
    }
}
