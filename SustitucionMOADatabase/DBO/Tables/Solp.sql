CREATE TABLE [dbo].[Solp] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [UsuarioCreacion_Id]     INT            NOT NULL,
    [UsuarioModificacion_Id] INT            NULL,
    [FechaCreacion]          DATETIME2 (7)  NOT NULL,
    [FechaModificacion]      DATETIME2 (7)  NULL,
    [Pliego_Id]              INT            NULL,
    [ClaseDocumento_Id]      INT            NULL,
    [NroSolp]                NVARCHAR (MAX) NULL,
    [EstadoSolpSap_Id]       INT            NULL,
    [EstadoDocumento_Id]     INT            NULL,
    [FechaBorrado]           DATETIME2 (7)  NULL,
    [FechaCreacionSap]       DATETIME2 (7)  NULL,
    [FechaLiberacionSap]     DATETIME2 (7)  NULL,
    [TipoSolp_Id]            INT            NULL,
    [PasoCompletado]         INT            NULL,
    [EstadoPasos]            VARCHAR (20)   NULL,
    [UsuarioCompras_Id]      INT            NULL,
    [TipoSolpSap] INT NULL, 
    [EmailLinkToken] UNIQUEIDENTIFIER NULL, 
    [TrabajoYaHecho] BIT NULL, 
    [ProveedorAsignado_Id] INT NULL, 
    [Adicional] BIT NULL, 
    [NroOrdenDeCompraAdicional] NCHAR(10) NULL, 
    CONSTRAINT [PK_dbo.Solp] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Solp_dbo.UsuarioCompras_Id] FOREIGN KEY ([UsuarioCompras_Id]) REFERENCES [dbo].[UsuarioCompras] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Solp_Pliego] FOREIGN KEY ([Pliego_Id]) REFERENCES [dbo].[Pliego] ([Id]),
    CONSTRAINT [FK_Solp_TablaEstado_EstadoDocumento] FOREIGN KEY ([EstadoDocumento_Id]) REFERENCES [dbo].[TablaEstado] ([Id]),
    CONSTRAINT [FK_Solp_TablaGeneral_TipoSolp] FOREIGN KEY ([TipoSolp_Id]) REFERENCES [dbo].[TablaGeneral] ([Id]),
    CONSTRAINT [FK_Solp_TablaSap_ClaseDocumento] FOREIGN KEY ([ClaseDocumento_Id]) REFERENCES [dbo].[TablaSap] ([Id]),
    CONSTRAINT [FK_Solp_TablaSap_EstadoSolpSap] FOREIGN KEY ([EstadoSolpSap_Id]) REFERENCES [dbo].[TablaSap] ([Id]),
    CONSTRAINT [FK_Solp_Usuario_ProveedorAsignado] FOREIGN KEY ([ProveedorAsignado_Id]) REFERENCES [dbo].[Usuario] ([Id])


);


