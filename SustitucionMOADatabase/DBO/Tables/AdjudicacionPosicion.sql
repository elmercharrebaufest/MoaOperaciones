CREATE TABLE [dbo].[AdjudicacionPosicion] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [Adjudicacion_Id]   int not NULL,
    [SolpPosicion_Id]          int not   NULL,   
    [CotizacionPosicion_Id]          int not   NULL, 
    [Cantidad] DECIMAL(18, 2)            NOT NULL,
    [Texto]  [nvarchar](max)  NULL,
    [Monto] DECIMAL(18, 2) NULL DEFAULT 0, 
    [PlazoDeEntrega] DATETIME NULL, 
    CONSTRAINT [PK_AdjudicacionPosicion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK.AdjudicacionPosicion_Adjudicacion_AdjudicacionId] FOREIGN KEY ([Adjudicacion_Id]) REFERENCES [Adjudicacion]([Id]),
    CONSTRAINT [FK.AdjudicacionPosicion_SolpPosicion_SolpPosicionId] FOREIGN KEY ([SolpPosicion_Id]) REFERENCES [SolpPosicion]([Id]),
    CONSTRAINT [FK.AdjudicacionPosicion_CotizacionPosicion_CotizacionPosicionId] FOREIGN KEY ([CotizacionPosicion_Id]) REFERENCES [CotizacionPosicion]([Id]),
);


