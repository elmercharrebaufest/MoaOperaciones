CREATE TABLE [dbo].[CotizacionHistorial] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [Cotizacion_Id]   INT NOT  NULL,
    [Log]          varchar(MAX) NOT  NULL,
    [FechaFinalizacion]          DATETIME2  NOT NULL,
    [Usuario_Id] INT            NOT NULL
    CONSTRAINT [PK_CotizacionHistorial] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK.CotizacionHistorial_Cotizacion_CotizacionId] FOREIGN KEY ([Cotizacion_Id]) REFERENCES [Cotizacion]([Id]),
    CONSTRAINT [FK.CotizacionHistorial_Usuario_UsuarioId] FOREIGN KEY ([Usuario_Id]) REFERENCES [Usuario]([Id]),
);
