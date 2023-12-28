CREATE TABLE [dbo].[LiberadorSapSolp] (
    [Id]                 INT IDENTITY (1, 1) NOT NULL,
    [Solp_Id]            INT NOT NULL, 
    [LiberadorSap_Id]    INT NOT NULL, 
    CONSTRAINT [PK_dbo.LiberadorSapSolp] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Solp] FOREIGN KEY ([Solp_Id]) REFERENCES [dbo].[Solp] ([Id]),
    CONSTRAINT [FK_LiberadorSap] FOREIGN KEY ([LiberadorSap_Id]) REFERENCES [dbo].[LiberadorSap] ([Id])
);