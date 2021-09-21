CREATE PROCEDURE AUTORATOR_STATE_UPDATE
	--ﾌﾟﾛｼｰｼﾞｬ引数宣言
	@Autorator_id		INT = 0,					--ｵｰﾄﾚｰﾀｰ識別ID
	@Command_time		DATETIME = NULL,		--最終ｺﾏﾝﾄﾞ受信時間
	@Total_state		INT = 0,				--状態
	@Err_code			INT = 0,				--異常ｺｰﾄﾞ
	@Floor				INT = 0,				--ｷｬﾘｯｼﾞ潜在階
	@In_floor			INT = 0,				--搬入階
	@In_state			INT = 0,				--搬入状態
	@Out_floor			INT = 0,				--搬出階
	@Out_state			INT = 0					--搬出状態

AS
	
	DECLARE

		@err_code_ INT			--ｴﾗｰｺｰﾄﾞ

	--ｴﾗｰｺｰﾄﾞ初期化
	SET @err_code_ = 0

	--垂直機情報ﾃｰﾌﾞﾙからｵｰﾄﾚｰﾀｰ識別ID検索
	IF NOT EXISTS (SELECT AUTORATOR_ID FROM AUTORATOR_STATE
				WHERE AUTORATOR_ID = @Autorator_id)

		BEGIN	--<
		--垂直機情報ﾃｰﾌﾞﾙﾚｺｰﾄﾞINSERT
		INSERT INTO AUTORATOR_STATE
				(COMMAND_TIME,
				TOTAL_STATE,
				ERR_CODE,
				FLOOR,
				IN_FLOOR,
				IN_STATE,
				OUT_FLOOR,
				OUT_STATE,
				MAKE_TIME,
				LAST_TIME)
				VALUES(
				@Command_time,
				@Total_state,
				@Err_code,
				@Floor,
				@In_floor,
				@In_state,
				@Out_floor,
				@Out_state,
				GETDATE(),
				GETDATE())
				
		SET @err_code_ = @@ERROR
	
		END	-->

	ELSE

		BEGIN	--<
		--垂直機情報ﾃｰﾌﾞﾙﾚｺｰﾄﾞUPDATE
		UPDATE AUTORATOR_STATE
			SET COMMAND_TIME = @Command_time,
				TOTAL_STATE = @Total_state,
				ERR_CODE = @Err_code,
				FLOOR = @Floor,
				IN_FLOOR = @In_floor,
				IN_STATE = @In_state,
				OUT_FLOOR = @Out_floor,
				OUT_STATE = @Out_state,
				MAKE_TIME = GETDATE(),
				LAST_TIME = GETDATE()
			WHERE AUTORATOR_ID = @Autorator_id
				
		SET @err_code_ = @@ERROR
	
		END	-->

	RETURN @err_code_