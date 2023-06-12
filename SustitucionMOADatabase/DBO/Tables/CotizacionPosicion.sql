CREATE TABLE [dbo].[CotizacionPosicion] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [Cotizacion_Id] INT            NOT NULL,
    [PeticionDeOfertaSolpPosicion_Id] INT            NOT NULL,
    [Cantidad] INT            NULL DEFAULT 0,
    [UnidadDeMedida_Id] INT            NULL,   
    [Moneda_Id] INT            NULL,
    [Precio] DECIMAL(18, 6) NULL DEFAULT 0, 
    [FechaDeEntrega]          DATETIME2 (7)  NULL,
    [NoDisponible] BIT NULL, 
    CONSTRAINT [PK_CotizacionPosicion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK.CotizacionPosicion_Cotizacion_Cotizacion_Id] FOREIGN KEY ([Cotizacion_Id]) REFERENCES [Cotizacion]([Id]),
    CONSTRAINT [FK.CotizacionPosicion_PeticionDeOfertaSolpPosicion_PeticionDeOfertaSolpPosicion_Id] FOREIGN KEY ([PeticionDeOfertaSolpPosicion_Id]) REFERENCES [PeticionDeOfertaSolpPosicion]([Id]),
    CONSTRAINT [FK.CotizacionPosicion_Unidad_UnidadDeMedida_Id] FOREIGN KEY ([UnidadDeMedida_Id]) REFERENCES [TablaSap]([Id]),
    CONSTRAINT [FK.CotizacionPosicion_Moneda_Moneda_Id] FOREIGN KEY ([Moneda_Id]) REFERENCES [TablaSap]([Id]),

);


