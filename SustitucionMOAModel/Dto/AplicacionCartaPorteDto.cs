
using SustitucionMOAModel.Enums.SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities = SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class AplicacionCartaPorteDto
    {
        public int Id { get; set; }
        public string MailUsuario { get; set; }
        public string RazonSocial { get; set; }
        public string RazonSocialCuit { get; set; }
        public string ColorEstado { get; set; }
        public string LabelEstado { get; set; }
        public string Contrato { get; set; }
        public string CartaPorte { get; set; }
        public int Kilogramos { get; set; }
        public string Error { get; set; }
        public EstadoAplicacionCartaPorte Estado { get; set; }
        public string FechaAlta { get; set; }
        public string FechaActualizacion { get; set; }


        public AplicacionCartaPorteDto(Entities.AplicacionCartaPorte aplicacionCCPP)
        {
            var colorEstado = aplicacionCCPP.Estado.ObtenerSemaforo();
            var labelEstado = aplicacionCCPP.Estado.ToFriendlyString();
            var razonSocial = aplicacionCCPP.Proveedor?.RazonSocial ?? string.Empty;

            this.MailUsuario = aplicacionCCPP.Usuario?.Mail ?? string.Empty;
            this.RazonSocial = razonSocial;
            this.RazonSocialCuit = $"{razonSocial} - {aplicacionCCPP.Proveedor?.CUIT}";
            this.ColorEstado = colorEstado;
            this.LabelEstado = labelEstado;
            this.FechaActualizacion = aplicacionCCPP.FechaAlta != null ? aplicacionCCPP.FechaActualizacion?.ToString("dd/MM/yyyy") : string.Empty;
            this.FechaAlta = aplicacionCCPP.FechaAlta.ToString("dd/MM/yyyy");
            this.Contrato = aplicacionCCPP.Contrato;
            this.CartaPorte = aplicacionCCPP.CartaPorte;
            this.Kilogramos = aplicacionCCPP.Kilogramos;
            this.Error = aplicacionCCPP.Error;
            this.Estado = aplicacionCCPP.Estado;
            this.Id = aplicacionCCPP.Id;
        }
    }
    public class AplicacionCartaPorteFiltrosDto
    {

        public List<DropdownOption> FiltroEstados;
        public List<DropdownOption> FiltroClientes;
        public AplicacionCartaPorteFiltrosDto(List<AplicacionCartaPorteDto> aplicaciones)
        {
            this.FiltroEstados = aplicaciones
                .GroupBy(apl => apl.Estado)
                .Select(x => new DropdownOption
                    {
                        value = x.Key.ToString(), label = $"{x.Key.ToFriendlyString()} ({x.Count()})" 
                    })
                .ToList();
            this.FiltroClientes = aplicaciones.GroupBy(apl => apl.RazonSocialCuit).Select(x => new DropdownOption { value = x.Key, label = $"{x.Key} ({x.Count()})" }).ToList();
            AgregarOpciontodos();
        }
        void AgregarOpciontodos()
        {
            this.FiltroClientes.Insert(0, new DropdownOption { value = "", label = "Todos" });
            this.FiltroEstados.Insert(0, new DropdownOption { value = "", label = "Todos" });
        }
    }
}
