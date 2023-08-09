CREATE PROCEDURE [dbo].[ActualizarToneladasAprobadasCampo] @IdCampo INT, @IdTSA INT, @Cuit NVARCHAR(15), @ToneladasAprobadas FLOAT, @MotivoRechazo NVARCHAR(500), @CosechaNombre nvarchar(40)
AS   
BEGIN  
 DECLARE @LogId BIGINT  
 DECLARE @Log varchar(MAX)=''  
 INSERT INTO LogActualizarToneladasAprobadasCampo values (GETDATE(),@IdCampo,@IdTSA,@Cuit,@ToneladasAprobadas,@MotivoRechazo,'',@CosechaNombre)  
 set @LogId = @@IDENTITY  
  
SET NOCOUNT ON  
BEGIN TRY  
 BEGIN TRAN  
 DECLARE @CampoCosechaId INT  
 DECLARE @CampoSustentableId INT  
  
  UPDATE dbo.CampoSustentable SET IdScato = @IdTSA WHERE Id = @IdCampo  
  
 SELECT @CampoSustentableId = Id FROM dbo.CampoSustentable WHERE Id = @IdCampo  

 SELECT @CampoCosechaId = CC.Id
 FROM CampoCosecha CC inner join
    Cosecha C on CC.Cosecha_Id = C.Id
 WHERE CC.CampoSustentable_Id = @CampoSustentableId and C.Nombre = @CosechaNombre
 
 IF @@ROWCOUNT <> 1  
    BEGIN  
  print('error  SELECT @CampoCosechaId = Id FROM dbo.CampoCosecha WHERE CampoSustentable_Id = @CampoSustentableId')  
        ROLLBACK TRAN  
        RETURN -2  
    END  
  
  
 IF @CampoCosechaId IS NULL OR @CampoCosechaId = 0  
 BEGIN  
  print('error  IF @CampoCosechaId IS NULL OR @CampoCosechaId = 0')  
  ROLLBACK TRAN  
  RETURN -1  
 END  
 print('10')  
    DECLARE @ToneladasAprobadasActuales INT = 0, @StockDisponible DECIMAL  
  
    SELECT   
        @ToneladasAprobadasActuales = ToneladasAprobadas,  
        @StockDisponible = StockDisponible  
    FROM   
        dbo.CampoCosecha  
    WHERE   
        Id = @CampoCosechaId  
  
  
    DECLARE @Actualizar BIT = 1  
  
    IF @ToneladasAprobadasActuales > 0 AND @ToneladasAprobadas > 0 AND @ToneladasAprobadas <> @ToneladasAprobadasActuales  
    BEGIN   
 print('11')  
        If @StockDisponible > @ToneladasAprobadas  
        BEGIN   
  print('12')  
            IF NOT EXISTS( SELECT TOP 1 1 FROM dbo.ConflictoCampoSustentable WHERE IdCampo = @IdCampo AND IdTSA = @IdTSA AND CUIT = @CUIT AND ToneladasInformadas = @ToneladasAprobadas)  
            BEGIN  
   print('13')  
                Set @Actualizar = 0  
                INSERT INTO dbo.ConflictoCampoSustentable  
                (  
                    [IdCampo]   
                    ,[IdTSA]  
                    ,[IdCampoSustentable]   
                    ,[ToneladasActuales]  
                    ,[ToneladasInformadas]   
                    ,[StockDisponible]  
                    ,[CUIT]  
                    ,[MotivoRechazo]  
                    ,[FechaConflicto]  
                    ,[Notificado]  
                )  
                VALUES   
                (  
                    @IdCampo,  
                    @IdTSA,  
                    @CampoSustentableId,  
                    @ToneladasAprobadasActuales,  
                    @ToneladasAprobadas,  
                    @StockDisponible,  
                    @Cuit,  
                    @MotivoRechazo,  
                    GETDATE(),  
                    0  
                )  
  
            END  
        END  
    END  
  
    IF @Actualizar = 1   
    BEGIN   
 print('14')  
        UPDATE   
            dbo.CampoCosecha  
        SET   
            ToneladasAprobadas = ISNULL(@ToneladasAprobadas,0),  
            MotivoRechazo = ISNULL(@MotivoRechazo, '')  
        WHERE   
            Id = @CampoCosechaId  
  
        IF @@ROWCOUNT <> 1  
        BEGIN  
            ROLLBACK TRAN  
            RETURN -2  
        END  
 END  
  
    COMMIT TRAN  
    RETURN 1  
      
END TRY  
BEGIN CATCH  
print('error')  
print (ERROR_MESSAGE())  
print (@LogId)  
 ROLLBACK TRAN  
update LogActualizarToneladasAprobadasCampo set Log=ERROR_MESSAGE() where id = @LogId  
 RETURN -99  
END CATCH  
  
END  