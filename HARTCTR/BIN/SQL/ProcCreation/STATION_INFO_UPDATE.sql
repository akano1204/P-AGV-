CREATE PROCEDURE STATION_INFO_UPDATE
	--ﾌﾟﾛｼｰｼﾞｬ引数宣言
	@St_id				NVARCHAR(20),			--ｽﾃｰｼｮﾝID
	@St_name			NVARCHAR(100),			--ｽﾃｰｼｮﾝ名
	@St_type			INT,					--ｽﾃｰｼｮﾝﾀｲﾌﾟ
	@St_attribute		INT						--ｽﾃｰｼｮﾝ属性

AS
	
	DECLARE

		@err_code_ INT			--ｴﾗｰｺｰﾄﾞ

	--ｴﾗｰｺｰﾄﾞ初期化
	SET @err_code_ = 0

	--AGVｽﾃｰｼｮﾝ情報ﾃｰﾌﾞﾙからｽﾃｰｼｮﾝID検索
	IF NOT EXISTS (SELECT ST_ID FROM STATION_INFO
				WHERE ST_ID = @St_id)

		BEGIN	--<
		--AGVｽﾃｰｼｮﾝ情報ﾃｰﾌﾞﾙﾚｺｰﾄﾞINSERT
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
		--AGVｽﾃｰｼｮﾝ情報ﾃｰﾌﾞﾙﾚｺｰﾄﾞUPDATE
		UPDATE STATION_INFO
			SET ST_NAME = @St_name,
				ST_TYPE = @St_type,
				ST_ATTRIBUTE = @St_attribute,
				LAST_TIME = GETDATE()
			WHERE ST_ID = @St_id
				
		SET @err_code_ = @@ERROR
	
		END	-->

	RETURN @err_code_