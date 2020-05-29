using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Formulario
{
    public class CartaPorteFormularioDropdownsWSMOAResponse
    {
        public List<FormularioDropdownElement> cosechas { get; set; }
        public List<FormularioDropdownElement> destinatarios { get; set; }
        public List<FormularioDestinoElement> destinos { get; set; }
        public List<FormularioDropdownElement> localidades { get; set; }
        public List<FormularioDropdownElement> provincias { get; set; }

        public CartaPorteFormularioDropdownsWSMOAResponse() {
            this.cosechas = new List<FormularioDropdownElement>() { };
            this.destinatarios = new List<FormularioDropdownElement>() { };
            this.destinos = new List<FormularioDestinoElement>() { };
            this.localidades = new List<FormularioDropdownElement>() { };
            this.provincias = new List<FormularioDropdownElement>() { };
        }
    }
}
