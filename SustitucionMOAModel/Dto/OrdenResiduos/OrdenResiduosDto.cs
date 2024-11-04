using SustitucionMOAModel.Dto.Scato;
using SustitucionMOAModel.Models.DataAgro;

namespace SustitucionMOAModel.Dto.OrdenResiduos
{
    public class OrdenResiduosDto
    {
        public int Id { get; set; }

        public EstadoOrdenResiduosDto Estado { get; set; }

        public ProveedorDto Cliente { get; set; }

        public MaterialDto Producto { get; set; }

        public DestinoScato DestinoMercaderia { get; set; }

        public OrdenDeCarga.PlantaDto Planta { get; set; }

        public OrdenDeCarga.DomicilioDto Domicilio { get; set; }

        public AlmacenDto Almacen { get; set; }

        public string PatenteChasis { get; set; }

        public string PatenteAcoplado { get; set; }

        public string NombreChofer { get; set; }
        public string ApellidoChofer { get; set; }

        public string CUILChofer { get; set; }

        public string RazonSocialTransporte { get; set; }

        public string CUITTransporte { get; set; }

        public string Observaciones { get; set; }

        public string FechaCreacion { get; set; }

        public string FechaIngreso { get; set; }

        public string FechaEgreso { get; set; }

        public int? CantidadDeViajes { get; set; }

        public Entities.OrdenResiduos ToEntity()
        {
            return ToEntity(new Entities.OrdenResiduos());
        }

        public Entities.OrdenResiduos ToEntity(Entities.OrdenResiduos entity)
        {
            entity.Id = Id;
            entity.AlmacenId = Almacen.Id;
            entity.ChoferCuil = CUILChofer;
            entity.ChoferNombre = NombreChofer;
            entity.ChoferApellido = ApellidoChofer;
            entity.ClienteId = Cliente.Id;
            entity.DomicilioDescr = Domicilio?.Descripcion;
            entity.DomicilioOrden = Domicilio?.Orden;
            entity.DomicilioTipo = Domicilio?.Tipo.ToString();
            entity.EstadoId = Estado?.Id ?? 0;
            entity.KmsARecorrer = DestinoMercaderia?.KmsARecorrer;
            entity.LocalidadId = DestinoMercaderia?.LocalidadId;
            entity.LocalidadDescripcion = DestinoMercaderia?.LocalidadDescripcion;
            entity.ProvinciaId = DestinoMercaderia?.ProvinciaId;
            entity.ProvinciaDescripcion = DestinoMercaderia?.ProvinciaDescripcion;
            entity.MaterialId = Producto.MaterialId;
            entity.Observacion = Observaciones;
            entity.PatenteAcoplado = PatenteAcoplado;
            entity.PatenteChasis = PatenteChasis;
            entity.PlantaCodigo = Planta?.Codigo.ToString();
            entity.TransporteCuit = CUITTransporte;
            entity.TransporteRazonSocial = RazonSocialTransporte;

            return entity;
        }

        public OrdenResiduosDto FromEntity(Entities.OrdenResiduos entity)
        {
            Almacen = new AlmacenDto { Id = entity.AlmacenId, Nombre = entity.Almacen.Nombre };
            Cliente = new ProveedorDto(entity.Cliente, false);
            CUILChofer = entity.ChoferCuil;
            CUITTransporte = entity.TransporteCuit;
            Domicilio = new OrdenDeCarga.DomicilioDto
            {
                Descripcion = entity.DomicilioDescr,
                Orden = entity.DomicilioOrden ?? 0,
                Tipo = entity.DomicilioTipo != null && int.TryParse(entity.DomicilioTipo, out int t) ? t : 0
            };
            Estado = new EstadoOrdenResiduosDto
            {
                Id = entity.Estado.Id,
                Nombre = entity.Estado.Nombre,
                NombreExterno = entity.Estado.NombreExterno,
                Semaforo = entity.Estado.Semaforo
            };
            FechaCreacion = entity.FechaCreacion.ToString("dd/MM/yyyy");
            FechaEgreso = entity.FechaEgreso?.ToString("dd/MM/yyyy HH:mm");
            FechaIngreso = entity.FechaIngreso?.ToString("dd/MM/yyyy HH:mm");
            Id = (int)entity.Id;
            DestinoMercaderia = entity.LocalidadId == null ? null : new DestinoScato
            {
                LocalidadId = entity.LocalidadId.Value,
                LocalidadDescripcion = entity.LocalidadDescripcion,
                ProvinciaId = entity.ProvinciaId ?? 0,
                ProvinciaDescripcion = entity.ProvinciaDescripcion,
                KmsARecorrer = entity.KmsARecorrer
            };
            NombreChofer = entity.ChoferNombre;
            ApellidoChofer = entity.ChoferApellido;
            Observaciones = entity.Observacion;
            PatenteAcoplado = entity.PatenteAcoplado;
            PatenteChasis = entity.PatenteChasis;
            Planta = new OrdenDeCarga.PlantaDto { Codigo = entity.PlantaCodigo != null && int.TryParse(entity.PlantaCodigo, out int c) ? c : 0 };
            Producto = new MaterialDto
            {
                MaterialId = entity.Producto.Id,
                CodigoSap = entity.Producto.CodigoSap,
                Descripcion = entity.Producto.Nombre,
                Abreviacion = entity.Producto.Abreviacion,
                ValidaSisaRuca = entity.Producto.ValidaSisaRuca
            };
            RazonSocialTransporte = entity.TransporteRazonSocial;

            return this;
        }
    }
}
