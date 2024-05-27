CREATE TABLE [dbo].[Usuario](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Mail] [nvarchar](200) NULL,
	[CUITRegistro] [nvarchar](50) NULL,
	[Habilitado] [bit] NOT NULL,
	[TipoUsuario_Id] [int] NULL,
	[UltimoLogin] SMALLDATETIME NULL, 
	[SeccionesVisitadas] NVARCHAR(MAX) NOT NULL DEFAULT '', 
	[AceptoTyC] BIT NOT NULL DEFAULT 0, 
	[AceptoTyCFecha] DATETIME,
	[ApiKey] NVARCHAR(100) NULL
	CONSTRAINT [PK_dbo.Usuario] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
	[UsuarioSap] NVARCHAR(20) NULL DEFAULT '', 
	[OrganizacionDeCompra] NVARCHAR(20) NULL, 
    [Suplente] NVARCHAR(MAX) NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Usuario]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Usuario_dbo.TipoUsuario_TipoUsuario_Id] FOREIGN KEY([TipoUsuario_Id])
REFERENCES [dbo].[TipoUsuario] ([Id])
GO

ALTER TABLE [dbo].[Usuario] CHECK CONSTRAINT [FK_dbo.Usuario_dbo.TipoUsuario_TipoUsuario_Id]
GO

