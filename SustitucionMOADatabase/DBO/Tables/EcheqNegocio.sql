CREATE TABLE [dbo].[EcheqNegocio] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [ProveedorId]            INT            NOT NULL,
    [UsuarioCreacionId]     INT            NOT NULL,
    [UsuarioModificacionId] INT            NULL,
    [FechaCreacion]          DATETIME2 (7)  NOT NULL,
    [FechaModificacion]      DATETIME2 (7)  NULL,
    [Contrato]              NVARCHAR(50)            NOT NULL,
    [Pedido]                NVARCHAR (50) NOT NULL,
    [Kilos]       DECIMAL(18, 2)            NOT NULL,
    [KilosPagados]     DECIMAL(18, 2)            NOT NULL,
    [Precio]           DECIMAL(18, 2)  NOT NULL,
    [Moneda]       NVARCHAR(50)  NOT NULL,
    [MaterialId]     INT  NOT NULL,
    [Fecha]         NVARCHAR(50)            NOT NULL,
    [MarcaCheque]            BIT   NOT NULL,
    [Clasificacion]      NVARCHAR(50)            NOT NULL,

    CONSTRAINT [PK_EcheqNegocio] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK.EcheqNegocio_Usuario_UsuarioCreacionId] FOREIGN KEY ([UsuarioCreacionId]) REFERENCES [Usuario]([Id]),
    CONSTRAINT [FK.EcheqNegocio_Usuario_UsuarioModificacionId] FOREIGN KEY ([UsuarioModificacionId]) REFERENCES [Usuario]([Id]),
    CONSTRAINT [FK.EcheqNegocio_MaterialFason_MaterialId] FOREIGN KEY ([MaterialId]) REFERENCES [MaterialFason]([Id]),
    CONSTRAINT [FK.EcheqNegocio_Proveedor_ProveedorId] FOREIGN KEY ([ProveedorId]) REFERENCES [Proveedor]([Id])




);


