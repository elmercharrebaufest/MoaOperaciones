CREATE TABLE [dbo].[CampoCosechaProveedor](
	[CampoCosecha_Id] [int] NOT NULL,
	[Proveedor_Id] [int] NOT NULL,
	CONSTRAINT [PK_dbo.CampoCosechaProveedor] PRIMARY KEY CLUSTERED ([CampoCosecha_Id], [Proveedor_Id])

	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],

	CONSTRAINT [FK_CampoCosechaProveedor_CampoCosecha] FOREIGN KEY ([CampoCosecha_Id]) REFERENCES [CampoCosecha]([ID]) ON DELETE CASCADE,
	CONSTRAINT [FK_CampoCosechaProveedor_Proveedor] FOREIGN KEY ([Proveedor_Id]) REFERENCES [Proveedor]([ID]) ON DELETE CASCADE

) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX IX_CampoCosechaProveedor_CampoCosecha_Id ON [dbo].[CampoCosechaProveedor]([CampoCosecha_Id]);

GO

CREATE NONCLUSTERED INDEX IX_CampoCosechaProveedor_Proveedor_Id ON [dbo].[CampoCosechaProveedor]([Proveedor_Id]);

GO
