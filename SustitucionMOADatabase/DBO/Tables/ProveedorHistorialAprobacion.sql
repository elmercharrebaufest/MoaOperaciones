CREATE TABLE [dbo].[ProveedorHistorialAprobacion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Proveedor_Id] [int]  NOT NULL,
	[Usuario_Id] [int]  NOT NULL,
	[EstadoAprobacion][int]  NOT NULL,
	[Observacion] varchar(max) NULL,
	[Fecha] DATETIME NOT NULL DEFAULT (getdate()), 
    CONSTRAINT [PK_dbo.ProveedorHistorialAprobacion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProveedorHistorialAprobacion]  WITH CHECK ADD  CONSTRAINT [FK_ProveedorHistorialAprobacion_Proveedor] FOREIGN KEY([Proveedor_Id])
REFERENCES [dbo].[Proveedor] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ProveedorHistorialAprobacion]  WITH CHECK ADD  CONSTRAINT [FK_ProveedorHistorialAprobacion_Usuario] FOREIGN KEY([Usuario_Id])
REFERENCES [dbo].[Usuario] ([Id])
ON DELETE CASCADE
GO


