using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class Aprobaciones
    {
        /*
    Definiciones
    [ID] [int] NOT NULL,
	[Ingresante_CDS] [nvarchar](50) NULL, --Email usuario ingresante
	[NRO_ES_LOCAL] [int] NULL, --"T_0000000001 -> Nro entrada Servicio temporal - string" -> Multiples selecciones -> mismo T_
	[NRO_ES_SAP] [int] NULL, --"1001123 -> Nro hoja de aceptación SAP (Nro ES creada en SAP)"
	[Fecha_Documento] [date] NULL, --"Fecha documento del modal de alta (en params)"
	[Fecha_Contabilizacion] [date] NULL,--"Fecha contabilización (en params)"
	[Referencia] [nvarchar](50) NULL,--Referencia/remito(en params)
	[Cantidad] [int] NULL, -- Agregar a params : Cantidad DEL ITEM -> (itemSelected[0].Cantidad?) -> Si hay mas de una linea, registrar POR ITEM
	[Descripcion_ES] [nvarchar](50) NULL,--Descripción proveniente del front (en params)
	[Importe] [float] NULL, --Linea Item -> Importe a certificar (en params)
	[Estado_certificacion] [nvarchar](50) NULL, -- Aprobado/Pendiente Aprobación/Rechazado Ej: Aprobación automatica = aprobado
	[Aprobada_automaticamente] [nvarchar](10) NULL, -- Pasar a tipo bit - 0 = No / 1 = Si
	[Aprobador_CDS] [nvarchar](50) NULL, --  Aprobacion automatica = email usuario ingresante / Aprobación manual = email usuario aprobador
	[Fiscal_SOLPED] [nvarchar](50) NULL, -- Email del fiscal: Misma lógica que Validación, Fiscal del contrato o Responsable trabajo (emails) o Solicitante de las posiciones -> Buscar usuario en DB -> Email del solicitante
	[Suplente] [nvarchar](50) NULL, -- Email -> del valor asignado a la tabla Usuarios / Columna Suplente del email del fiscal de la solped
	--ATENCION !!
	--NUEVO
	[Area_Fiscal] [nvarchar](100) NULL, -- Cargar del valor asignado a la tabla Usuarios / Columna Area  del email del fiscal de la solped
	--NUEVO
	[Fecha_Carga_ES] [date] NULL, -- DateTime.Today() -> Fecha de carga de ES, sea temporal o no.
	-- FIN NUEVOS 22/03
	[Fecha_aprobacion] [date] NULL, -- Aprobacion automatica: Fecha de apruebación automaticamente - Pendiente aprobación: Al momento de generar la ES Temporal : valor nulo, se modifica al darle aprobación manual.
	[Fecha_rechazo] [date] NULL, -- Aprobación automatica: Nulo - Pendiente aprobación: Al momento de generar la ES temporal: Nulo
	[Motivo_rechazo] [nvarchar](50) NULL, -- Nulo hasta que se cargue de pantalla
	[Notificaciones_enviadas] [nvarchar](10) NULL, -- Pasar a tipo bit - 0 = no / 1 = Si - Nulo al momento del alta
	[NRO_OC] [nchar](10) NULL, -- Orden de compra asociada (en params)
	[NRO_POS] [nchar](10) NULL, -- Posicion asociada (en params)
	[Nro_linea] [nchar](10) NULL, -- Linea asociada [item] (en params)
	[Nro_servicio] [nchar](10) NULL, -- Service (en params)
	[Texto_breve_servicio] [nchar](10) NULL, -- Short Text (en params)
	[UM] [nchar](10) NULL, -- Agregar a params : UM desde el front
	[Monto] [nvarchar](10) NULL, -- Pasar a tipo float - En FE: Item -> Importe O Monto total en el modal de alta ES
	[Cantidad_real] [nvarchar](10) Por ahora en Stand by - A definir
	[Cantidad_a_certificar] [nvarchar](10) NULL, --Cantidad proveniente del front (en params)
	[Porcentaje_a_certificar] [nchar](10) NULL, -- Agregar a params : Porcentaje proveniente del front 
	[Monto_a_certificar] [nchar](10) NULL, -- Agregar a params: Monto a certificar DE CADA ES A APROBAR
	[Monto_total] [nchar](10) NULL, -- Agregar a params: Suma de los montos a certificar de cada ES A APROBAR
         */

        [Key]
        public int ID { get; set; }

        public string Ingresante_CDS { get; set; }

        public string NRO_ES_LOCAL { get; set; }

        public int? NRO_ES_SAP { get; set; }

        public DateTime? Fecha_Documento { get; set; }

        public DateTime? Fecha_Contabilizacion { get; set; }

        public string Referencia { get; set; }

        public double? Cantidad { get; set; }

        public string Descripcion_ES { get; set; }

        public double? Importe { get; set; }

        public string Estado_certificacion { get; set; }

        public bool? Aprobada_automaticamente { get; set; }

        public string Aprobador_CDS { get; set; }

        public string Fiscal_SOLPED { get; set; }

        public string Suplente { get; set; }

        public string Area_Fiscal { get; set; }

        public DateTime? Fecha_Carga_ES { get; set; }

        public DateTime? Fecha_aprobacion { get; set; }

        public DateTime? Fecha_rechazo { get; set; }

        public string Motivo_rechazo { get; set; }

        public bool? Notificaciones_enviadas { get; set; }

        public string NRO_OC { get; set; }

        public string NRO_POS { get; set; }

        public string Nro_linea { get; set; }

        public string Nro_servicio { get; set; }

        public string Texto_breve_servicio { get; set; }

        public string UM { get; set; }

        public double? Monto { get; set; }

        public double? Cantidad_Anterior { get; set; }

        public string Cantidad_a_certificar { get; set; }

        public string Porcentaje_a_certificar { get; set; }

        public double? Monto_a_certificar { get; set; }

        public double? Monto_total { get; set; }

        public string Planned_package { get; set; }

        public string Planned_line { get; set; }
        public string Proveedor { get; set; }
    }
}