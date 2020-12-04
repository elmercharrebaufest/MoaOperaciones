CREATE TABLE [dbo].[ProveedorRelacionConEmpleados](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Proveedor_Id] [int] NOT NULL,
	[NombreProveedora] [varchar](200) NOT NULL,
	[CargoProveedora] [varchar](200) NOT NULL,
	[NombreMolinos] [varchar](200) NOT NULL,
	[Vinculo] [varchar](200) NOT NULL,
 CONSTRAINT [PK_dbo.ProveedorRelacionConEmpleados] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProveedorRelacionConEmpleados]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProveedorRelacionConEmpleados_dbo.Proveedor_Proveedor_Id] FOREIGN KEY([Proveedor_Id])
REFERENCES [dbo].[Proveedor] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ProveedorRelacionConEmpleados] CHECK CONSTRAINT [FK_dbo.ProveedorRelacionConEmpleados_dbo.Proveedor_Proveedor_Id]
GO
