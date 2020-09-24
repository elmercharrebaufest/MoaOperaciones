CREATE TABLE [dbo].[UsuarioRelacionConEmpleados](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Usuario_Id] [int] NOT NULL,
	[NombreProveedora] [varchar](200) NOT NULL,
	[CargoProveedora] [varchar](200) NOT NULL,
	[NombreMolinos] [varchar](200) NOT NULL,
	[Vinculo] [varchar](200) NOT NULL,
 CONSTRAINT [PK_dbo.UsuarioRelacionConEmpleados] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UsuarioRelacionConEmpleados]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsuarioRelacionConEmpleados_dbo.Usuario_Usuario_Id] FOREIGN KEY([Usuario_Id])
REFERENCES [dbo].[Usuario] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UsuarioRelacionConEmpleados] CHECK CONSTRAINT [FK_dbo.UsuarioRelacionConEmpleados_dbo.Usuario_Usuario_Id]
GO
