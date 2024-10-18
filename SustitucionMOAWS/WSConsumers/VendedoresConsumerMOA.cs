using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.VendedoresWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class VendedoresConsumerMOA: IVendedoresConsumerMOA
    {
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
                ZMPES4340[] salidas = new ZMPES4340[] { };
                List<ZMPES4100> fechasSAP = new List<ZMPES4100>() { };
                foreach (FechaWS fecha in fechas)
                {
                    fechasSAP.Add(new ZMPES4100()
                    {
                        FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                        FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                    });
                }
                ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();
                ZMPES4910 error = service.SI_MPMF_MOAOP_VENDEDORES(proveedor, ref fechasSAPArray, ref salidas);
                VendedoresWSMOAResponse result = map(error, salidas);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private VendedoresWSMOAResponse map(ZMPES4910 error, ZMPES4340[] salidas)
        {
            VendedoresWSMOAResponse result = new VendedoresWSMOAResponse();
            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES4340 vendedor in salidas)
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
