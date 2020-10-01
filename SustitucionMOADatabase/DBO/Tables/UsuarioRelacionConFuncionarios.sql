CREATE TABLE [dbo].[UsuarioRelacionConFuncionarios](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Usuario_Id] [int] NOT NULL,
	[NombreFirma] [varchar](200) NOT NULL,
	[CargoFirma] [varchar](200) NOT NULL,
	[NombreFuncionario] [varchar](200) NOT NULL,
	[CargoFuncionario] [varchar](200) NOT NULL,
	[Vinculo] [varchar](200) NOT NULL,
 CONSTRAINT [PK_dbo.UsuarioRelacionConFuncionarios] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UsuarioRelacionConFuncionarios]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsuarioRelacionConFuncionarios_dbo.Usuario_Usuario_Id] FOREIGN KEY([Usuario_Id])
REFERENCES [dbo].[Usuario] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UsuarioRelacionConFuncionarios] CHECK CONSTRAINT [FK_dbo.UsuarioRelacionConFuncionarios_dbo.Usuario_Usuario_Id]
GO
