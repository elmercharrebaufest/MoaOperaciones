using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class OrdenDeCargaFason
    {
        [Key]
        public long Id { get; set; }
        public EstadoOrdenDeCargaFason Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaRetiro { get; set; }
        public int Cantidad { get; set; }
        public string PatenteChasis { get; set; }
        public string PatenteAcoplado { get; set; }
        public string NombreChofer { get; set; }
        public string CUILChofer { get; set; }
        public string RazonSocialTransporte { get; set; }
        public string CUITTransporte { get; set; }
        public string Observacion { get; set; }
        public int Cliente_Id { get; set; }
        [ForeignKey("Cliente_Id")]
        public virtual Proveedor Cliente { get; set; }
        public int Producto_Id { get; set; }
        [ForeignKey("Producto_Id")]
        public virtual Material Producto { get; set; }
        public bool TransporteExiste { get; set; }

        public int LocalidadId { get; set; }

        public string LocalidadDescripcion { get; set; }

        public int? CorredorId { get; set; }

        [ForeignKey("CorredorId")]
        public virtual Proveedor Corredor { get; set; }


        public int? CantidadEntregada { get; set; }
        public DateTime? FechaEgreso { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string NroRemito { get; set; }
        public string UniMedCant { get; set; }
        public string KmARecorrer { get; set; }
        public bool FleteMOA { get; set; }
        public bool Reventa { get; set; }
        public OrdenDeCargaFason() { }
        public OrdenDeCargaFason(CrearOrdenDeCargaFasonRequest request)
        {
            Cantidad = request.Cantidad;
            Cliente_Id = request.Cliente;
            CorredorId = request.CorredorId;
            CUILChofer = request.CUILChofer;
            CUITTransporte = request.CUITTransporte;
            LocalidadId = request.Destino.LocalidadId;
            LocalidadDescripcion = request.Destino.LocalidadDescripcion;
            FechaCreacion = DateTime.Now;
            FechaRetiro = request.FechaRetiro;
            NombreChofer = request.NombreChofer;
            Observacion = request.Observacion;
            PatenteAcoplado = request.PatenteAcoplado;
            PatenteChasis = request.PatenteChasis;
            Producto_Id = request.Producto_Id;
            RazonSocialTransporte = request.RazonSocialTransporte;
            KmARecorrer = request.Destino.KmARecorrer;
            FleteMOA = request.FleteMOA;
            Reventa = request.Reventa;
        }
        public override bool Equals(object obj)
        {
            return obj is OrdenDeCargaFason carga &&
                Id == carga.Id &&
                Estado == carga.Estado &&
                FechaCreacion == carga.FechaCreacion &&
                FechaRetiro == carga.FechaRetiro &&
                Cantidad == carga.Cantidad &&
                PatenteChasis == carga.PatenteChasis &&
                PatenteAcoplado == carga.PatenteAcoplado &&
                NombreChofer == carga.NombreChofer &&
                CUILChofer == carga.CUILChofer &&
                RazonSocialTransporte == carga.RazonSocialTransporte &&
                CUITTransporte == carga.CUITTransporte &&
                LocalidadId == carga.LocalidadId &&
                LocalidadDescripcion == carga.LocalidadDescripcion &&
                Observacion == carga.Observacion &&
                Cliente_Id == carga.Cliente_Id &&
                Producto_Id == carga.Producto_Id &&
                TransporteExiste == carga.TransporteExiste &&
                CorredorId == carga.CorredorId &&
                KmARecorrer == carga.KmARecorrer &&
                FleteMOA == carga.FleteMOA &&
                Reventa == carga.Reventa;
        }

        public override int GetHashCode()
        {
            int hashCode = -1110409952;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();

            hashCode = hashCode * -1521134295 + FechaCreacion.GetHashCode();
            hashCode = hashCode * -1521134295 + FechaRetiro.GetHashCode();
            hashCode = hashCode * -1521134295 + Cantidad.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PatenteChasis);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PatenteAcoplado);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NombreChofer);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CUILChofer);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RazonSocialTransporte);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CUITTransporte);
            hashCode = hashCode * -1521134295 + LocalidadId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocalidadDescripcion);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Observacion);
            hashCode = hashCode * -1521134295 + Cliente_Id.GetHashCode();
            hashCode = hashCode * -1521134295 + Producto_Id.GetHashCode();
            hashCode = hashCode * -1521134295 + CorredorId.GetHashCode();
            return hashCode;
        }
    }
}
