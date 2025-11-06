using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class TrackingRequestDto
    {
        public string Ctg { get; set; }
        public string Patente { get; set; }
    }

    public class TrackingResponseDto
    {
        public bool Resultado { get; set; }
        public string Mensaje { get; set; }
        public TrackingDataDto Data { get; set; }
    }

    public class EstadoEtapasResponseDto
    {
        public bool Resultado { get; set; }
        public string Mensaje { get; set; }
        public EstadoEtapasDto Data { get; set; }
    }

    public class TrackingDataDto
    {
        public string Workflow { get; set; }
        public string Ctg { get; set; }
        public DateTime FechaHoraIngreso { get; set; }
        public string TitularCartaPorte { get; set; }
        public string RemitenteComercial { get; set; }
        public string RemitenteComercialVtaPrim { get; set; }
        public string Entregador { get; set; }
        public string Transportista { get; set; }
        public string Material { get; set; }
        public bool Rechazado { get; set; }
        public CamionDto Camion { get; set; }
        public ChoferDto Chofer { get; set; }
        public DatosAdicionalesDto DatosAdicionales { get; set; }
        public List<EtapaDto> Etapas { get; set; }
    }

    public class EstadoEtapasDto
    {
        public DatosAdicionalesDto DatosAdicionales { get; set; }
        public List<EtapaDto> Etapas { get; set; }
    }

    public class CamionDto
    {
        public string Patente { get; set; }
        public string PatenteAcoplado { get; set; }
    }

    public class ChoferDto
    {
        public string Cuil { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Extranjero { get; set; }
        public string NombreApellido { get; set; }
    }

    public class DatosAdicionalesDto
    {
        public string PreCaladoFila { get; set; }
        public string PostCaladoFila { get; set; }
        public string CaladoEstado { get; set; }
        public int? PesadaBruto { get; set; }
        public int? PesadaTara { get; set; }
        public int? PesadaDescargado { get; set; }
    }

    public class EtapaDto
    {
        public string Nombre { get; set; }
        public DateTime Fecha { get; set; }
        public string TiempoEstimado { get; set; }
        public string Estado { get; set; } // "completado" | "en-proceso" | "pendiente"
    }
}