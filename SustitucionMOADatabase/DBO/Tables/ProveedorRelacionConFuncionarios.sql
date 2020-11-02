CREATE TABLE [dbo].[ProveedorRelacionConFuncionarios](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Proveedor_Id] [int] NOT NULL,
	[NombreFirma] [varchar](200) NOT NULL,
	[CargoFirma] [varchar](200) NOT NULL,
	[NombreFuncionario] [varchar](200) NOT NULL,
	[CargoFuncionario] [varchar](200) NOT NULL,
	[Vinculo] [varchar](200) NOT NULL,
 CONSTRAINT [PK_dbo.ProveedorRelacionConFuncionarios] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProveedorRelacionConFuncionarios]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProveedorRelacionConFuncionarios_dbo.Proveedor_Proveedor_Id] FOREIGN KEY([Proveedor_Id])
REFERENCES [dbo].[Proveedor] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ProveedorRelacionConFuncionarios] CHECK CONSTRAINT [FK_dbo.ProveedorRelacionConFuncionarios_dbo.Proveedor_Proveedor_Id]
GO
