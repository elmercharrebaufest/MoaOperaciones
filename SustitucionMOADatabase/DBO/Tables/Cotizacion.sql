CREATE TABLE [dbo].[Cotizacion] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [UsuarioCreador_Id] INT            NOT NULL,
    [CotizacionEstado_Id] INT            NOT NULL,
    [PeticionDeOfertaUsuario_Id] INT NOT NULL,
    [FechaCreacion]          DATETIME2 (7)  NOT NULL,
    [RespetaMateriales] bit  NULL, 
    [RespetaServicios] bit  NULL, 
    [ObservacionTecnica] [nvarchar](max)  NULL,	 
    [ObservacionEconomica] [nvarchar](max)  NULL,	
    [Revision] INT            NOT NULL,
    [PorcentajeDeHoras] INT NULL, 
    CONSTRAINT [PK_Cotizacion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK.Cotizacion_Cotizacion_CotizacionEstado_Id] FOREIGN KEY ([CotizacionEstado_Id]) REFERENCES [CotizacionEstado]([Id]),
    CONSTRAINT [FK.Cotizacion_Cotizacion_UsuarioCreador_Id] FOREIGN KEY ([UsuarioCreador_Id]) REFERENCES [Usuario]([Id]),
    CONSTRAINT [FK.Cotizacion_Cotizacion_PeticionDeOfertaUsuario_Id] FOREIGN KEY ([PeticionDeOfertaUsuario_Id]) REFERENCES [PeticionDeOfertaUsuario]([Id]),

);


