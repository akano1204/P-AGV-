CREATE PROCEDURE STATION_INFO_UPDATE
	--ÌßÛ¼°¼Þ¬ˆø”éŒ¾
	@St_id				NVARCHAR(20),			--½Ã°¼®ÝID
	@St_name			NVARCHAR(100),			--½Ã°¼®Ý–¼
	@St_type			INT,					--½Ã°¼®ÝÀ²Ìß
	@St_attribute		INT						--½Ã°¼®Ý‘®«

AS
	
	DECLARE

		@err_code_ INT			--´×°º°ÄÞ

	--´×°º°ÄÞ‰Šú‰»
	SET @err_code_ = 0

	--AGV½Ã°¼®Ýî•ñÃ°ÌÞÙ‚©‚ç½Ã°¼®ÝIDŒŸõ
	IF NOT EXISTS (SELECT ST_ID FROM STATION_INFO
				WHERE ST_ID = @St_id)

		BEGIN	--<
		--AGV½Ã°¼®Ýî•ñÃ°ÌÞÙÚº°ÄÞINSERT
		INSERT INTO STATION_INFO
				(ST_ID,
				ST_NAME,
				ST_TYPE,
				ST_ATTRIBUTE,
				MAKE_TIME,
				LAST_TIME)
				VALUES(
				@St_id,
				@St_name,
				@St_type,
				@St_attribute,
				GETDATE(),
				GETDATE())
				
		SET @err_code_ = @@ERROR
	
		END	-->

	ELSE

		BEGIN	--<
		--AGV½Ã°¼®Ýî•ñÃ°ÌÞÙÚº°ÄÞUPDATE
		UPDATE STATION_INFO
			SET ST_NAME = @St_name,
				ST_TYPE = @St_type,
				ST_ATTRIBUTE = @St_attribute,
				LAST_TIME = GETDATE()
			WHERE ST_ID = @St_id
				
		SET @err_code_ = @@ERROR
	
		END	-->

	RETURN @err_code_