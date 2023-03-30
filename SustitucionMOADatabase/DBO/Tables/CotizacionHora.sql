CREATE TABLE [dbo].[CotizacionHora] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [Cotizacion_Id] INT            NOT NULL,
    [Categoria]              NVARCHAR(MAX)            NOT NULL,
    [CantidadPersonas] INT            NOT NULL,
    [HorasNormales] INT            NOT NULL,
    [HorasNocturnas] INT            NOT NULL,
    [HorasExtras] INT            NOT NULL,
    [Gremio]              NVARCHAR(MAX)            NOT NULL
    CONSTRAINT [PK_CotizacionHora] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK.CotizacionHora_Cotizacion_Cotizacion_Id] FOREIGN KEY ([Cotizacion_Id]) REFERENCES [Cotizacion]([Id]),

);


