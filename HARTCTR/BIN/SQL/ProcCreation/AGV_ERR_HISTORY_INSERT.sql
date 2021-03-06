CREATE PROCEDURE AGV_ERR_HISTORY_INSERT
	--ﾌﾟﾛｼｰｼﾞｬ引数宣言
	@Agv_id				NVARCHAR(10),		--AGV識別ID
	@Err_code			INT = 0,			--異常ｺｰﾄﾞ
	@Floor_qr			NVARCHAR(20)		--最終床QR

AS
	
	DECLARE

		@err_code_ INT			--ｴﾗｰｺｰﾄﾞ

	--ｴﾗｰｺｰﾄﾞ初期化
	SET @err_code_ = 0

	--AGV異常履歴ﾃｰﾌﾞﾙﾚｺｰﾄﾞINSERT
	INSERT INTO AGV_ERR_HISTORY
			(FINISH,
			MAKE_DATE,
			AGV_ID,
			ERR_CODE,
			FLOOR_QR,
			START_TIME,
			END_TIME,
			MAKE_TIME,
			LAST_TIME)
			VALUES(
			0,
			CONVERT(CHAR, GETDATE(), 112),
			@Agv_id,
			@Err_code,
			@Floor_qr,
			GETDATE(),
			NULL,
			GETDATE(),
			GETDATE())
			
	SET @err_code_ = @@ERROR
	
	RETURN @err_code_