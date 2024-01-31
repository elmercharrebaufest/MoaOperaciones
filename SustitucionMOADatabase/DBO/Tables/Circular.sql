CREATE TABLE [dbo].[Circular] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [UsuarioCreador_Id] INT            NOT NULL,
    [RequiereCambioDeFechas]   BIT      NULL,
    [FechaCreacion]          DATETIME2 (7)  NOT NULL,
    [PlazoDeOferta]          DATETIME2 (7)   NULL,
    [FechaDeEntrega]          DATETIME2 (7)   NULL,
    [Observaciones]              NVARCHAR(MAX)            NOT NULL
    CONSTRAINT [PK_Circular] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK.Circular_Usuario_UsuarioCreadorId] FOREIGN KEY ([UsuarioCreador_Id]) REFERENCES [Usuario]([Id])

);


