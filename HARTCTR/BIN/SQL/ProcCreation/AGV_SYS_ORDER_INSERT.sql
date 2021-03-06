CREATE PROCEDURE AGV_SYS_ORDER_INSERT
	--ﾌﾟﾛｼｰｼﾞｬ引数宣言
	@Id					INT = 0 OUTPUT,		--ID(戻り値)
	@Agv_id				NVARCHAR(10),		--AGV識別ID
	@Order_type			INT = 0				--ｼｽﾃﾑ動作

AS
	
	DECLARE

		@err_code_ INT			--ｴﾗｰｺｰﾄﾞ

	--ｴﾗｰｺｰﾄﾞ初期化
	SET @err_code_ = 0

	--AGVｼｽﾃﾑ動作指示ﾃｰﾌﾞﾙﾚｺｰﾄﾞINSERT
	INSERT INTO AGV_SYS_ORDER
			(FINISH,
			AGV_ID,
			ORDER_TYPE,
			MAKE_TIME,
			LAST_TIME)
			VALUES(
			0,
			@Agv_id,
			@Order_type,
			GETDATE(),
			GETDATE())

	SET @Id = SCOPE_IDENTITY()
			
	SET @err_code_ = @@ERROR
	
	RETURN @err_code_