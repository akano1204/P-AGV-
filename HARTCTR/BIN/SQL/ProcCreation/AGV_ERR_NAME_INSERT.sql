CREATE PROCEDURE AGV_ERR_NAME_INSERT
	--ÌßÛ¼°¼Þ¬ˆø”éŒ¾
	@Err_code			INT,					--ˆÙíº°ÄÞ
	@Err_name			NVARCHAR(45),			--ˆÙí–¼Ì
	@Err_detail			NVARCHAR(200) = '',		--ˆÙíÚ×
	@Err_recovery		NVARCHAR(200) = ''		--•œ‹Œ•û–@

AS
	
	DECLARE

		@err_code_ INT			--´×°º°ÄÞ

	--´×°º°ÄÞ‰Šú‰»
	SET @err_code_ = 0

	--AGVˆÙí–¼ÌÃ°ÌÞÙ‚©‚çˆÙíº°ÄÞŒŸõ
	IF NOT EXISTS (SELECT ERR_CODE FROM AGV_ERR_NAME
				WHERE ERR_CODE = @Err_code)

		BEGIN	--<
		--ˆÙíÃ°ÌÞÙÚº°ÄÞINSERT
		INSERT INTO AGV_ERR_NAME
				(ERR_CODE,
				ERR_NAME,
				ERR_DETAIL,
				ERR_RECOVERY,
				MAKE_TIME)
				VALUES(
				@Err_code,
				@Err_name,
				@Err_detail,
				@Err_recovery,
				GETDATE())
				
		SET @err_code_ = @@ERROR
	
		END	-->

	ELSE

		BEGIN	--<
		--ˆÙíÃ°ÌÞÙÚº°ÄÞUPDATE
		UPDATE AGV_ERR_NAME
			SET ERR_NAME = @Err_name,
				ERR_DETAIL = @Err_detail,
				ERR_RECOVERY = @Err_recovery
			WHERE ERR_CODE = @Err_code
				
		SET @err_code_ = @@ERROR
	
		END	-->

	RETURN @err_code_