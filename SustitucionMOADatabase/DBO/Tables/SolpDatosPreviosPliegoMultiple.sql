

CREATE TABLE [dbo].[SolpDatosPreviosPliegoMultiple](
	[Solp_Id] [int] NOT NULL,
	[Pliego_Id] [int] NULL,
	[EstadoDocumento_Id] [int] NULL,
	[TipoSolp_Id] [int] NULL,
 CONSTRAINT [PK_dbo.SolpDatosPreviosPliegoMultiple] PRIMARY KEY CLUSTERED 
(
	[Solp_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
CONSTRAINT [FK_SolpDatosPreviosPliegoMultiple_Solp] FOREIGN KEY ([Solp_Id]) REFERENCES [dbo].[Solp] ([Id]) ON DELETE CASCADE,
CONSTRAINT [FK_SolpDatosPreviosPliegoMultiple_Pliego] FOREIGN KEY ([Pliego_Id]) REFERENCES [dbo].[Pliego] ([Id]),
) ON [PRIMARY]
GO

