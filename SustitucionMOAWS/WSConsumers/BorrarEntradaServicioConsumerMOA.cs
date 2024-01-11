using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOARepositorio;
using SustitucionMOAWS.BorrarEntradaServicioNuevoWebServiceMOA;
using SustitucionMOAWS.BorrarEntradaServicioWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using System;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class BorrarEntradaServicioConsumerMOA : IBorrarEntradaServicioConsumerMOA
    {
        SI_MMRFC_BORRAR_HESClient service;
        //private const string COMP_CODE = "MOA";
        //private readonly IRepositorio repositorio;

        public BorrarEntradaServicioConsumerMOA()
        {
            service = new SI_MMRFC_BORRAR_HESClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            //this.repositorio = repositorio;
        }

        public string BorrarEntradaServicio(string nroES)
        {
            try
            {
                string ENTRYSHEET = nroES;
                BorrarEntradaServicioNuevoWebServiceMOA.BAPIRET2[] RETURN = new BorrarEntradaServicioNuevoWebServiceMOA.BAPIRET2[] { };

                service.SI_MMRFC_BORRAR_HES(nroES, ref RETURN);

                return Map(RETURN);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string Map(BorrarEntradaServicioNuevoWebServiceMOA.BAPIRET2[] RETURN)
        {
            //string result = RETURN[0].MESSAGE;
            string result = string.Join(Environment.NewLine, RETURN.Select(r => r.MESSAGE));
            //string[] messages = RETURN.Select(r => r.MESSAGE).ToArray();

            return result;
        }

    }

    public interface IBorrarEntradaServicioConsumerMOA
    {
    }
}
