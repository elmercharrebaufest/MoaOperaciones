using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.DataAgroServices;
using System;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class DataAgroConsumer
    {
        private DataAgroServicesClient service = new DataAgroServicesClient();

        public DataAgroConsumer()
        {
            service.ClientCredentials.UserName.UserName = string.Concat(DataAgroWSCredential.getDominio(), @"\", DataAgroWSCredential.getUserName());
            service.ClientCredentials.UserName.Password = DataAgroWSCredential.getPassword();
            //service.ClientCredentials.UserName. = DataAgroWSCredential.getDominio();
        }


        public ResultadoValidarProveedorComercial ValidarCUIT(string CUIT, bool? corredor = false)
        {
            try
            {
                return service.ValidarProveedorComercial(CUIT, corredor);
            }
            //Significa que no estamos conectados
            catch (Exception)
            {
                return null;
            }
        }

        public bool ProveedorApocrifo(string CUIT)
        {
            try
            {
                return service.ProveedorApocrifo(CUIT);
            }
            //Significa que no estamos conectados
            catch (Exception)
            {
                return false;
            }
        }

        public decimal TraerTipoDeCambio()
        {
            return service.TraerTipoDeCambio(DateTime.Now.Date, "", "M");
        }

        public SustitucionMOAWS.DataAgroServices.ResultadoAltaCampoSustentable AltaCampoSustentable(SustitucionMOAModel.Entities.CampoProveedor campo, string kmz)
        {

            return service.AltaCampoSustentable(new CampoDetalleTerceroDto
            {
                ProveedorCUIT = campo.CUIT,
                Campania = campo.CampoCosecha.Cosecha.Nombre,
                LocalidadId = campo.CampoCosecha.Campo.Localidad_Id,
                Latitud = campo.Latitud,
                Longitud = campo.Longitud,
                KMZnombre = campo.Archivo.FileKey,
                KMZfileBase64 = kmz.Substring(400),
                Nombre = campo.CampoCosecha.Campo.Nombre,
                ToneladasAprobadas = Convert.ToDecimal(campo.CampoCosecha.ToneladasAprobadas),
                HectareasTotales = Convert.ToDecimal(campo.HectareasTotales),
                HectareasCultivables = Convert.ToDecimal(campo.HectareasSoja),
                Id = campo.CampoCosecha.Id,
                Estado = campo.Proveedor.EstadoAprobacion.ToString()
            });
        }

        public ResultEstadoProveedores ObtenerEstadoProveedores(string[] cuits)
        {
            var result = service.ObtenerEstadoProveedores(cuits);
            foreach (var item in result.Contactos.Where(a => a.OperaConMATBA))
            {
                item.EstadoHomeDescripcion = "No Habilitado";
            }
            return result;

        }

    }

}
