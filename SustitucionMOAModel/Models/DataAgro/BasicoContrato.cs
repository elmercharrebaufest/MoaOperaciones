using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DataAgro
{
    public class BasicoContrato
    {
        public string ChequeElectronicoValor;

        public int Id { get; set; }
        public string Cuit { get; set; }
        public int ContratoId { get; set; }
        public int MaterialId { get; set; }
        public int? NivelTarifaId { get; set; }
        public int TipoNegocioId { get; set; }
        public double Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string PrecioPlazo { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public int? CampanaId { get; set; }
        public DateTime? FechaDesde { get; set; }
        public string FechaDesdeFormateado { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string FechaHastaFormateado { get; set; }
        public int ProveedorId { get; set; }
        public string MonedaId { get; set; }
        public string Moneda { get; set; }
        public DateTime? Fecha { get; set; }
        public string FechaFormateado { get; set; }
        public string Hora { get; set; }

        public string NivelTarifa { get; set; }
        public decimal? TarifaFlete { get; set; }
        public int GrupoCompra { get; set; }
        public string GrupoCompraDescripcion { get; set; }
        public int? ComercialId { get; set; }
        public int? ComercialCreadorId { get; set; }
        public int? ProvinciaId { get; set; }
        public int? LocalidadId { get; set; }
        public bool? Base { get; set; }
        public decimal? Importe_Sustentable { get; set; }
        public string MonedaId_Sustentable { get; set; }
        public string Moneda_Sustentable { get; set; }
        public DateTime? FechaDesde_Sustentable { get; set; }
        public string FechaDesde_SustentableFormateado { get; set; }
        public DateTime? FechaHasta_Sustentable { get; set; }
        public string FechaHasta_SustentableFormateado { get; set; }
        public DateTime? Fecha_Dolarizado { get; set; }
        public string Fecha_DolarizadoFormateado { get; set; }
        public int? Dias_Pesificado { get; set; }
        public bool? NoInformaSIO { get; set; }
        public bool? TrigoEspecial { get; set; }
        public int? Estado { get; set; }
        public string UsuarioId { get; set; }
        public string ContratoSAP { get; set; }
        public double? Ampliaciones { get; set; }
        public string TipoNegocio { get; set; }
        public string Proveedor { get; set; }
        public string Corredor { get; set; }
        public string CUITCorredor { get; set; }
        public string Comercial { get; set; }
        public string ComercialCreador { get; set; }
        public string Material { get; set; }

        public string Campania { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        public string Estado_Contrato { get; set; }
        //public int? Cantidad_F { get; set; }
        //public decimal? Precio_F { get; set; }
        //public string Proveedor_F { get; set; }
        //public string Fecha_F { get; set; }
        //public string Material_F { get; set; }
        //public string MonedaId_F { get; set; }
        //public string Moneda_F { get; set; }
        //public int? Ampliaciones_F { get; set; }
        public DateTime Fecha_Order { get; set; }
        public int Estado_Order { get; set; }
        public string Observacion { get; set; }
        //public string Observacion_F { get; set; }

        public int? FijacionDePrecioContratoId { get; set; }
        public bool? Sustentable { get; set; }
        public bool? Dolarizado { get; set; }
        public bool? Pesificado { get; set; }
        public string Negocio { get; set; }

        public int? ClasificacionId { get; set; }
        public string ClasificacionDescripcion { get; set; }
        public int? DestinoId { get; set; }
        public string DestinoDescripcion { get; set; }
        public int? CantidadCamiones { get; set; }
        public bool? Consignatario { get; set; }
        public bool? PlanCanje { get; set; }
        public int? CondicionFijacion { get; set; }
        public bool? CD { get; set; }
        public bool? Warrant { get; set; }
        public bool? PagoDirectoVendedor { get; set; }
        public string CalidadDescripcion { get; set; }
        public bool? EstablecimientoPropio { get; set; }
        public int? BoletoId { get; set; }
        public int? BolsaId { get; set; }
        public string BoletoDescripcion { get; set; }
        public string BolsaDescripcion { get; set; }
        public DateTime? DesdeFijacion { get; set; }
        public string DesdeFijacionFormateado { get; set; }
        public DateTime? HastaFijacion { get; set; }
        public string HastaFijacionFormateado { get; set; }
        public string CondicionFijacionDescripcion { get; set; }
        public List<DescuentoBonificacionDto> Descuentos { get; set; }
        public List<CalidadDto> Calidades { get; set; }
        public bool? MercsDeposito { get; set; }
        public int CorredorId { get; set; }
        public DatosFijacionDeContratoDto DatosFijacion { get; set; }
        public decimal? PorcentajeComision { get; set; }
        public string ContratoCorredor { get; set; }
        public string ContratoVendedor { get; set; }
        public bool? SelCargoMOA { get; set; }
        public bool? SelCargoVendedor { get; set; }
        public bool? Madre { get; set; }
        public bool? EsFason { get; set; }
        public string ContratoMadre { get; set; }
        public string Posicion { get; set; }
        public string TipoFason { get; set; }
        public int TipoFasonId { get; set; }
        public int FasonId { get; set; }
        public string Operador { get; set; }
        public int OperadorId { get; set; }
        public int AgenteId { get; set; }
        public List<AperturaPrecioDto> AperturaPrecios { get; set; }
        public List<PrecioPactadosDto> PreciosPactados { get; set; }
        public decimal? PrecioNeto { get; set; }
        public bool? Pizarra { get; set; }
        public int? StandardCalidadId { get; set; }
        public string StandardDeCalidadDescripcion { get; set; }
        public bool? PagoDiferido { get; set; }
        public int? PagoDiferidoTerceroId { get; set; }
        public int? ZonaId { get; set; }
        public string ZonaDescripcion { get; set; }
        public int? AcuerdoId { get; set; }
        public decimal? ImporteFinanciero { get; set; }
        public decimal? ImporteRedespacho { get; set; }
        public decimal? ImporteComision { get; set; }
        public decimal? ImporteBonificacion { get; set; }
        public decimal? PorcentajeBonificacion { get; set; }
        public bool? Compensacion { get; set; }
        public int? Acuerdo { get; set; }
        public string Rechazo { get; set; }
        public int? ComercialZonaId { get; set; }
        public string ComercialZonaDescripcion { get; set; }
        public bool OcultarEnTablero { get; set; }
        public string FechaCiertaFormateado { get; set; }
        public DateTime? FechaCierta { get; set; }
        public string MonedaBonificacion { get; set; }
        public string MesPosicion { get; set; }
        public int? CampanaMaterialId { get; set; }
        public int? ContratoAcuerdoId { get; set; }
        public decimal? PorcentajeDePago { get; set; }
        public int? TipoAgenteCompraId { get; set; }
        public string CaratulaExtension { get; set; }
        public string CaratulaMAT { get; set; }
        public decimal? PrecioAjusteComision { get; set; }
        public string MonedaAjusteComisionId { get; set; }
        public DateTime? FechaOperacion { get; set; }
        public string FechaOperacionFormateado { get; set; }
        public string MotivoOperacionAnterior { get; set; }
        public string UsuarioConfirmador { get; set; }
        public DateTime? FechaConfirmacion { get; set; }
        public int CantidadMaximaCupo { get; set; }
        public bool? ChequeElectronico { get; set; }
        public bool? DolarizadoExpress { get; set; }
        public string DolarizadoExpressValor { get; set; }
        public string PagoCBU { get; set; }
        public string DolarizadoValor { get; set; }
        public bool? CalidadTercero { get; set; }
        public bool? DolarizadoTercero { get; set; }
        public bool? PagoDiferidoTercero { get; set; }
        public string ObservacionTercero { get; set; }
        public bool? Canje { get; set; }
        public string MonedaCanjeId { get; set; }
        public decimal? Monto { get; set; }
        public string Insumo { get; set; }
        public bool PrestamoDevolucion { get; set; }
        public int PlantaDestinoId { get; set; }
        public string PlantaDestinoDescripcion { get; set; }
        public bool? DolarizadoCorredor { get; set; }
        public bool? SustentableTercero { get; set; }
        public bool? Venta { get; set; }
        public bool? Anticipo { get; set; }
        public bool? Cesion { get; set; }
        public string ClasificacionContrato { get; set; }
        public string TipoAgenteCompra { get; set; }
    }
    public class DescuentoBonificacionDto
    {
        public int Id { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public decimal Importe { get; set; }
        public string MonedaId { get; set; }
        public string Moneda { get; set; }
        public decimal Porcentaje { get; set; }
        public string TipoDBDesc { get; set; }
        public int TipoDBId { get; set; }
        public string TipoPeriodoDBDesc { get; set; }
        public int TipoPeriodoDBId { get; set; }
        public int ContratoId { get; set; }

    }
    public class CalidadDto
    {
        public int Id { get; set; }
        public int CalidadEspecialId { get; set; }
        public string CalidadEspecialDesc { get; set; }
        public decimal Valor { get; set; }
        public int? ContratoId { get; set; }
        public int? AcuerdoId { get; set; }
        public decimal? PorcentajeDesde { get; set; }
        public decimal? PorcentajeHasta { get; set; }

    }
    public partial class DatosFijacionDeContratoDto
    {
        public string ContratoId { get; set; }
        public string KilosAplicados { get; set; }
        public string KilosPendiente { get; set; }
        public string KilosContrato { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public string Filtro { get; set; }
        public string DesdeEntrega { get; set; }
        public string HastaEntrega { get; set; }
        public string Posicion { get; set; }
        public bool? Calidad { get; set; }
        public string Campana { get; set; }
        public bool? PagoDiferido { get; set; }
        public int? Centro { get; set; }
        public string CentroDescripcion { get; set; }
        public string ARecibirSinPrecio { get; set; }
        public string RecibidoSinFijar { get; set; }
        public string Color { get; set; }
        public decimal ImporteAPrecio { get; set; }
        public decimal ImporteSobrePrecio { get; set; }
        public string MonedaAPrecio { get; set; }
        public string MonedaSobrePrecio { get; set; }
        public decimal PorcentajeAPrecio { get; set; }
        public decimal PorcentajeSobrePrecio { get; set; }
        public string CondicionFijacionCod { get; set; }
        public string CondicionFijacionDescripcion { get; set; }
        public string CondicionPagoCod { get; set; }
        public string CondicionPagoDescripcion { get; set; }
        public bool? ChequeElectronico { get; set; }

        public List<CalidadDto> Calidades { get; set; }
        public int CampanaId { get; set; }
        public string Clasificacion { get; set; }
        public bool Cesion { get; set; }
        public bool Anticipo { get; set; }
        public string FijacionSap { get; set; }
    }
    public class AperturaPrecioDto
    {
        public int Id { get; set; }
        public int? contratoId { get; set; }
        public int? FijacionId { get; set; }
        public int ConceptoAperturaPrecioId { get; set; }
        public decimal Importe { get; set; }
        public decimal Porcentaje { get; set; }
        public string MonedaId { get; set; }
        public string ConceptoAperturaPrecio { get; set; }
        public string Moneda { get; set; }

    }
    public class PrecioPactadosDto
    {
        public int Id { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public decimal Precio { get; set; }
        public string MonedaPactadoId { get; set; }
        public string MonedaPactadoDesc { get; set; }
        public decimal? ImportePactado { get; set; }
        public string MonedaImportePactadoId { get; set; }
        public string MonedaImportePactadoDesc { get; set; }
        public decimal? Porcentaje { get; set; }
        public int ContratoId { get; set; }

    }
}
