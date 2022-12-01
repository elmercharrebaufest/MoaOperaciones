CREATE TABLE [dbo].[OrdenDeCargaFasonViajes]
(
	[Id] [int] IDENTITY(1,1) NOT NULL, 
    [OrdenDeCargaFason_Id] BIGINT NOT NULL, 
    [FechaIngreso] DATETIME NOT NULL, 
    [FechaEgreso] DATETIME NOT NULL, 
    [Cantidad] INT NOT NULL, 
    [UniMedCant] VARCHAR(50) NOT NULL, 
    [NroRemito] VARCHAR(50) NOT NULL, 
    CONSTRAINT [PK_OrdenDeCargaFasonViajes] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
CONSTRAINT [FK_OrdenDeCargaFasonViajes_OrdenDeCargaFason] FOREIGN KEY (OrdenDeCargaFason_Id) REFERENCES [OrdenDeCargaFason]([Id]),
) ON [PRIMARY]
GO