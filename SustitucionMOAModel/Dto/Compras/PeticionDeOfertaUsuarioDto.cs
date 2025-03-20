using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class PeticionDeOfertaUsuarioDto
    {
        public int UsuarioId { get; set; }

        private string razonSocial;
        private bool razonSocialOverride;
        public string RazonSocial
        {
            get
            {
                if (razonSocialOverride) { return razonSocial; }
                return DatosProveedor?.RazonSocial ?? CuitRegistroUsuario;
            }
            set
            {
                razonSocial = value;
                razonSocialOverride = true;
            }
        }

        public int Id { get; set; }

        private string cuit;
        private bool cuitOverride;
        public string CUIT
        {
            get
            {
                if (cuitOverride) { return cuit; }
                return DatosProveedor?.CUIT ?? CuitRegistroUsuario;
            }
            set
            {
                cuit = value;
                cuitOverride = true;
            }
        }

        private string mail;
        private bool mailOverride;
        public string Mail
        {
            get
            {
                if (mailOverride) { return mail; }
                return DatosProveedor?.Mail ?? CuitRegistroUsuario;
            }
            set
            {
                mail = value;
                mailOverride = true;
            }
        }

        public bool? PropuestaTecnicaAprobada { get; set; }
        public bool? RealizoVisita { get; set; }
        public CotizacionDto Cotizacion { get; set; }
        public bool CircularSinLeer { get; set; }
        public IEnumerable<int> CircularesSinLeer { get; set; }
        public string EstadoVisita { get; set; }
        public string EstadoVisitaColor { get; set; }
        public string EstadoPropuestaTecnica { get; set; }
        public string EstadoPropuestaTecnicaColor { get; set; }
        public string CotizacionEstado { get; set; }
        public bool VerAdjudicar { get; set; }
        public bool VerImportes { get; set; }
        public bool EstaHabilitado { get; set; }

        private EstadoAprobacion? proveedorEstadoAprobacion;
        private bool proveedorEstadoAprobacionOverride;
        public EstadoAprobacion? ProveedorEstadoAprobacion
        {
            get
            {
                if (proveedorEstadoAprobacionOverride) { return proveedorEstadoAprobacion; }
                return DatosProveedor?.EstadoAprobacion;
            }
            set
            {
                proveedorEstadoAprobacion = value;
                proveedorEstadoAprobacionOverride = true;
            }
        }

        public string MensajeAdjudicar { get; set; }
        public bool ValidacionCircularSolicitante { get; set; }
        public string ObservacionNoCumple { get; set; }
        public DateTime PlazoDeOferta { get; set; }
        public DateTime PlazoDeOfertaOriginal { get; set; }
        public DateTime? PlazoDeOfertaCircular { get; set; }
        public DateTime? PlazoDeOfertaCierre { get; set; }
        public DateTime? FechaCircular { get; set; }

        private string codigoProveedor;
        private bool codigoProveedorOverride;
        public string CodigoProveedor
        {
            get
            {
                if (codigoProveedorOverride) { return codigoProveedor; }
                return DatosProveedor?.CodigoProveedor ?? "";
            }
            set
            {
                codigoProveedor = value;
                codigoProveedorOverride = true;
            }
        }

        public string THCategoria { get; set; }
        public bool? VisibleSolicitante { get; set; }
        public bool Deshabilitado { get; set; }
        public List<MonedaTotalDto> TotalesPorMoneda { get; set; }

        public Proveedor DatosProveedor { private get; set; }
        public string CuitRegistroUsuario { private get; set; }

        public DateTime GetFechaFinPlazo()
        {
            if (PlazoDeOfertaCierre == null && FechaCircular == null)
            {
                return PlazoDeOfertaOriginal;
            }
            else
            {
                if (PlazoDeOfertaCierre == null)
                {
                    return PlazoDeOfertaCircular.Value;
                }
                else
                {
                    if (FechaCircular == null)
                    {
                        return PlazoDeOfertaCierre.Value;
                    }
                    else
                    {
                        if (PlazoDeOfertaCierre.Value > FechaCircular.Value)
                        {
                            return PlazoDeOfertaCierre.Value;
                        }
                        else
                        {
                            return PlazoDeOfertaCircular.Value;
                        }
                    }
                }
            }
        }
    }
}
