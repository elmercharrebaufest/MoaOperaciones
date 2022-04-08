CREATE PROCEDURE [dbo].[ActualizarToneladasAprobadasCampo] @IdCampo INT, @IdTSA INT, @Cuit NVARCHAR(15), @ToneladasAprobadas FLOAT, @MotivoRechazo NVARCHAR(500)
AS
BEGIN
	DECLARE @LogId BIGINT
	DECLARE @Log varchar(MAX)=''
	INSERT INTO LogActualizarToneladasAprobadasCampo values (GETDATE(),@IdCampo,@IdTSA,@Cuit,@ToneladasAprobadas,@MotivoRechazo,'')
	set @LogId = @@IDENTITY

SET NOCOUNT ON
BEGIN TRY
	BEGIN TRAN
	DECLARE @CampoCosechaId INT
	DECLARE @CampoSustentableId INT

	SELECT @CampoSustentableId = Id FROM dbo.CampoSustentable WHERE IdScato = @IdTSA AND Id = @IdCampo
	set @Log = @Log+'(1)@CampoSustentableId = ' +LTRIM(ISNULL( @CampoSustentableId,''))  + ' - '
	--Encontramos un Campo Sustenable ya creado y con Id de TSA/SCATO asignado asi que vamos a actualizar las toneladas de este
	IF @CampoSustentableId IS NOT NULL
	BEGIN
		SELECT @CampoCosechaId=cc.Id
		FROM dbo.Proveedor p
		INNER JOIN dbo.CampoProveedor cp ON p.Id = cp.Proveedor_Id
		INNER JOIN dbo.CampoCosecha cc ON cp.CampoCosecha_Id = cc.Id
		INNER JOIN dbo.Cosecha c ON cc.Cosecha_Id = c.Id
		WHERE CP.CUIT = @Cuit AND GETDATE() BETWEEN c.Inicio AND c.Fin AND cc.CampoSustentable_Id = @CampoSustentableId

		set @Log = @Log+'(2)@CampoCosechaId = ' + LTRIM(ISNULL( @CampoCosechaId,''))  + ' - '

	END
	ELSE
	BEGIN
		SELECT @CampoSustentableId = Id FROM dbo.CampoSustentable WHERE IdScato = @IdTSA
			
		set @Log = @Log+'(3)@CampoSustentableId = ' + LTRIM(ISNULL( @CampoSustentableId,''))  + ' - '

		--Encontramos un Campo Sustentable con el Id de TSA/SCATO asignado pero que no corresponde con el ID de Moa Operaciones recibido
		IF @CampoSustentableId IS NOT NULL
		BEGIN
			SELECT @CampoCosechaId = cc.Id FROM dbo.CampoCosecha cc
			INNER JOIN dbo.Cosecha c on c.Id = cc.Cosecha_Id
			WHERE cc.CampoSustentable_Id = @CampoSustentableId AND GETDATE() BETWEEN c.Inicio AND c.Fin

			set @Log = @Log+'(3)@CampoSustentableId = ' +  LTRIM(ISNULL(@CampoSustentableId,''))  + ' - '

			IF @CampoCosechaId IS NOT NULL
			BEGIN
				IF NOT EXISTS(SELECT 1 FROM dbo.CampoProveedor cp INNER JOIN dbo.Proveedor p ON cp.Proveedor_Id = p.Id  WHERE cp.CampoCosecha_Id = @CampoCosechaId AND CP.CUIT = @Cuit)
				BEGIN
					UPDATE cp
					SET CampoCosecha_Id = @CampoCosechaId
					FROM dbo.CampoProveedor cp
					INNER JOIN dbo.CampoCosecha cc ON CC.Id = cp.CampoCosecha_Id
					INNER JOIN dbo.Cosecha c on c.Id = cc.Cosecha_Id
					INNER JOIN dbo.Proveedor p ON p.Id = cp.Proveedor_Id
					WHERE CP.CUIT = @Cuit AND GETDATE() BETWEEN c.Inicio AND c.Fin AND cc.CampoSustentable_Id = @IdCampo

					set @Log = @Log+'(4)update CampoProveedor set CampoCosecha_Id= ' +  LTRIM(ISNULL(@CampoCosechaId,''))  + ' - '

				END
			END
			--No existe todavía relacion entre el campo que debemos asignar y la cosecha. Lo creamos ahora
			ELSE
			BEGIN
				DECLARE @NuevoCC table (Id int)

				INSERT INTO dbo.CampoCosecha(CampoSustentable_Id, Cosecha_Id)
				OUTPUT inserted.Id INTO @NuevoCC
				VALUES (@CampoSustentableId, (SELECT Id FROM dbo.Cosecha WHERE GETDATE() BETWEEN Inicio AND Fin))

				SELECT @CampoCosechaId = Id FROM @NuevoCC

				set @Log = @Log+'(5)INSERT INTO dbo.CampoCosecha ' +  LTRIM(ISNULL(@CampoCosechaId,''))  + ' - '

			END

			--Cleanup del registro que se creo en un principio de campo sustentable que ya no se necesita
			DELETE FROM dbo.CampoCosecha WHERE CampoSustentable_Id = @IdCampo
			DELETE FROM dbo.CampoSustentable WHERE Id = @IdCampo

			set @Log = @Log+'(6)DELETE FROM dbo.CampoCosecha WHERE CampoSustentable_Id =  ' +  LTRIM(ISNULL(@IdCampo,''))  + ' - '
			set @Log = @Log+'(7)DELETE FROM dbo.CampoSustentable WHERE Id = ' +  LTRIM(ISNULL(@IdCampo,''))  + ' - '

		END
		--Nuevo Id de TSA para asignar al nuevo campo
		ELSE
		BEGIN
			UPDATE dbo.CampoSustentable
			SET IdScato = @IdTSA
			WHERE Id = @IdCampo
			
			set @Log = @Log+'(8)UPDATE dbo.CampoSustentable SET IdScato = '+ LTRIM(ISNULL(@IdTSA,''))+' WHERE Id = ' + LTRIM(ISNULL(@IdCampo ,'')) + ' - '

			SELECT @CampoCosechaId = cc.Id FROM dbo.CampoCosecha cc
			INNER JOIN dbo.Cosecha c on c.Id = cc.Cosecha_Id
			WHERE cc.CampoSustentable_Id = @IdCampo AND GETDATE() BETWEEN c.Inicio AND c.Fin

			set @Log = @Log+'(9)SELECT @CampoCosechaId = cc.Id FROM dbo.CampoCosecha cc ' +  LTRIM(ISNULL(@CampoCosechaId,''))  + ' - '

		END
	END
	
	set @Log = @Log+'(10)IF @CampoCosechaId IS NULL OR @CampoCosechaId = 0 ' +  LTRIM(ISNULL(@CampoCosechaId,'')) + ' - '

	IF @CampoCosechaId IS NULL OR @CampoCosechaId = 0
	BEGIN
		ROLLBACK TRAN
		update LogActualizarToneladasAprobadasCampo set [Log] = @Log where Id = @LogId
		RETURN -1
	END

    DECLARE @ToneladasAprobadasActuales INT = 0, @StockDisponible DECIMAL

    SELECT 
        @ToneladasAprobadasActuales = ToneladasAprobadas,
        @StockDisponible = StockDisponible
    FROM 
        dbo.CampoCosecha
    WHERE 
        Id = @CampoCosechaId

	set @Log = @Log+'(11)@ToneladasAprobadasActuales ' +  LTRIM(ISNULL(@ToneladasAprobadasActuales,''))  + ' - '
	set @Log = @Log+'(12)@StockDisponible ' +  LTRIM(ISNULL(@StockDisponible,''))  + ' - '

    DECLARE @Actualizar BIT = 1

    IF @ToneladasAprobadasActuales > 0 AND @ToneladasAprobadas > 0 AND @ToneladasAprobadas <> @ToneladasAprobadasActuales
    BEGIN 
        If @StockDisponible > @ToneladasAprobadas
        BEGIN 
            IF NOT EXISTS( SELECT TOP 1 1 FROM dbo.ConflictoCampoSustentable WHERE IdCampo = @IdCampo AND IdTSA = @IdTSA AND CUIT = @CUIT AND ToneladasInformadas = @ToneladasAprobadas)
            BEGIN
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

				set @Log = @Log+'(13)INSERT INTO dbo.ConflictoCampoSustentable - '

            END
        END
    END
	
	set @Log = @Log+'(13)@Actualizar ' +   LTRIM(@Actualizar)  + ' - ' 

    IF @Actualizar = 1 
    BEGIN 
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
			update LogActualizarToneladasAprobadasCampo set [Log] = @Log where Id = @LogId
            RETURN -2
        END
	END

	update LogActualizarToneladasAprobadasCampo set [Log] = @Log where Id = @LogId
    COMMIT TRAN
    RETURN 1
    
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	update LogActualizarToneladasAprobadasCampo set [Log] = @Log + ' ' +ERROR_MESSAGE() +' ' + ERROR_LINE() where Id = @LogId
	RETURN -99
END CATCH

END
GO
