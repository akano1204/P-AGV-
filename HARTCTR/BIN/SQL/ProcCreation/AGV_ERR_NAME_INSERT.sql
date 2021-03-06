CREATE PROCEDURE AGV_ERR_NAME_INSERT
	--ﾌﾟﾛｼｰｼﾞｬ引数宣言
	@Err_code			INT,					--異常ｺｰﾄﾞ
	@Err_name			NVARCHAR(45),			--異常名称
	@Err_detail			NVARCHAR(200) = '',		--異常詳細
	@Err_recovery		NVARCHAR(200) = ''		--復旧方法

AS
	
	DECLARE

		@err_code_ INT			--ｴﾗｰｺｰﾄﾞ

	--ｴﾗｰｺｰﾄﾞ初期化
	SET @err_code_ = 0

	--AGV異常名称ﾃｰﾌﾞﾙから異常ｺｰﾄﾞ検索
	IF NOT EXISTS (SELECT ERR_CODE FROM AGV_ERR_NAME
				WHERE ERR_CODE = @Err_code)

		BEGIN	--<
		--異常ﾃｰﾌﾞﾙﾚｺｰﾄﾞINSERT
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
		--異常ﾃｰﾌﾞﾙﾚｺｰﾄﾞUPDATE
		UPDATE AGV_ERR_NAME
			SET ERR_NAME = @Err_name,
				ERR_DETAIL = @Err_detail,
				ERR_RECOVERY = @Err_recovery
			WHERE ERR_CODE = @Err_code
				
		SET @err_code_ = @@ERROR
	
		END	-->

	RETURN @err_code_