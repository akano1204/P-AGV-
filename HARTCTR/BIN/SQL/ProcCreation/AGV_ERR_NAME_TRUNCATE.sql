CREATE PROCEDURE AGV_ERR_NAME_TRUNCATE

AS
	
	DECLARE

		@err_code_ INT			--ｴﾗｰｺｰﾄﾞ

	--ｴﾗｰｺｰﾄﾞ初期化
	SET @err_code_ = 0

	--AGV異常名称ﾃｰﾌﾞﾙから全ﾚｺｰﾄﾞ除去
	TRUNCATE TABLE AGV_ERR_NAME

	SET @err_code_ = @@ERROR

	RETURN @err_code_