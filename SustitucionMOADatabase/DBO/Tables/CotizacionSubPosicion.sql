CREATE TABLE [dbo].[CotizacionSubPosicion] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [CotizacionPosicion_Id] INT            NOT NULL,
    [SolpSubPosicion_Id] INT            NOT NULL,
    [Cantidad] INT            NULL DEFAULT 0,
    [UnidadDeMedida_Id] INT            NULL,   
    [Moneda_Id] INT            NULL,
    [Precio] DECIMAL(18, 6) NULL DEFAULT 0, 
    CONSTRAINT [PK_CotizacionSubPosicion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK.CotizacionSubPosicion_CotizacionPosicion_CotizacionPosicion_Id] FOREIGN KEY ([CotizacionPosicion_Id]) REFERENCES [CotizacionPosicion]([Id]),
    CONSTRAINT [FK.CotizacionSubPosicion_SolpSubPosicion_SolpSubPosicion_Id] FOREIGN KEY ([SolpSubPosicion_Id]) REFERENCES [SolpSubposicion]([Id]),
   CONSTRAINT [FK.CCotizacionSubPosicion_Unidad_UnidadDeMedida_Id] FOREIGN KEY ([UnidadDeMedida_Id]) REFERENCES [TablaSap]([Id]),

);


