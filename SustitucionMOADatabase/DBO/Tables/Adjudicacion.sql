CREATE TABLE [dbo].[Adjudicacion] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [Cotizacion_Id]   int not NULL,
    [Solp_Id]          int not   NULL,
    [NumeroOrdenDeCompra]          varchar(20) NOT  NULL,
    [FechaCreacion]          DATETIME2 (7)  NOT NULL,
    [UsuarioCreador_Id] INT            NOT NULL,
    [Moneda_Id] INT            NOT NULL,
    [MontoTotal] DECIMAL(18, 6) NULL, 
    [TextoDeCabecera]          NVARCHAR(MAX) NULL,
    [CondicionesDeEntrega]          NVARCHAR(MAX) NULL,
    [CondicionesDePago]          NVARCHAR(MAX) NULL,
    [Garantias]          NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_Adjudicacion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK.Adjudicacion_Usuario_UsuarioCreadorId] FOREIGN KEY ([UsuarioCreador_Id]) REFERENCES [Usuario]([Id]),
    CONSTRAINT [FK.Adjudicacion_Usuario_CotizacionId] FOREIGN KEY ([Cotizacion_Id]) REFERENCES [Cotizacion]([Id]),
    CONSTRAINT [FK.Adjudicacion_Usuario_SolpId] FOREIGN KEY ([Solp_Id]) REFERENCES [Solp]([Id]),
    CONSTRAINT [FK.Adjudicacion_Adjudicacion_MonedaId] FOREIGN KEY (Moneda_Id) REFERENCES [TablaSap]([Id]),

);


