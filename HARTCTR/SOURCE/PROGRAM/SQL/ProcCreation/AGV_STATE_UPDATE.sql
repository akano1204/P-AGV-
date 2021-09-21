CREATE PROCEDURE AGV_STATE_UPDATE
	--ﾌﾟﾛｼｰｼﾞｬ引数宣言
	@Agv_id				NVARCHAR(10),		--AGV識別ID
	@Command_time		DATETIME,			--最終ｺﾏﾝﾄﾞ受信時間
	@Action				CHAR(1) = '',		--運航状態
	@Order_no			INT = 0,			--動作指示親番
	@Order_sub_no		INT = 0,			--動作指示子番
	@Battery			INT = 0,			--充電残量
	@Battery_state		INT = 0,			--ﾊﾞｯﾃﾘｰ状態
	@Action_mode		CHAR(1) = '',		--動作ﾓｰﾄﾞ
	@Map				CHAR(1) = '',		--現在ﾏｯﾌﾟ
	@Location_x			INT = 0,			--現在地X座標
	@Location_y			INT = 0,			--現在地Y座標
	@Rack_id			NVARCHAR(20),		--ﾗｯｸID
	@Err_code			INT = 0				--異常ｺｰﾄﾞ

AS
	
	DECLARE

		@err_code_ INT			--ｴﾗｰｺｰﾄﾞ

	--ｴﾗｰｺｰﾄﾞ初期化
	SET @err_code_ = 0

	--AGV状態ﾃｰﾌﾞﾙからAGV識別ID検索
	IF NOT EXISTS (SELECT AGV_ID FROM AGV_STATE
				WHERE AGV_ID = @Agv_id)

		BEGIN	--<
		--AGV状態ﾃｰﾌﾞﾙﾚｺｰﾄﾞINSERT
		INSERT INTO AGV_STATE
				(AGV_ID,
				COMMAND_TIME,
				ACTION,
				ORDER_NO,
				ORDER_SUB_NO,
				BATTERY,
				BATTERY_STATE,
				ACTION_MODE,
				MAP,
				LOCATION_X,
				LOCATION_Y,
				RACK_ID,
				ERR_CODE,
				LAST_TIME)
				VALUES(
				@Agv_id,
				@Command_time,
				@Action,
				@Order_no,
				@Order_sub_no,
				@Battery,
				@Battery_state,
				@Action_mode,
				@Map,
				@Location_x,
				@Location_y,
				@Rack_id,
				@Err_code,
				GETDATE())
				
		SET @err_code_ = @@ERROR
	
		END	-->

	ELSE

		BEGIN	--<
		--AGV状態ﾃｰﾌﾞﾙﾚｺｰﾄﾞUPDATE
		UPDATE AGV_STATE
			SET COMMAND_TIME = @Command_time,
				ACTION = @Action,
				ORDER_NO = @Order_no,
				ORDER_SUB_NO = @Order_sub_no,
				BATTERY = @Battery,
				BATTERY_STATE = @Battery_state,
				ACTION_MODE = @Action_mode,
				MAP = @Map,
				LOCATION_X = @Location_x,
				LOCATION_Y = @Location_y,
				RACK_ID = @Rack_id,
				ERR_CODE = @Err_code,
				LAST_TIME = GETDATE()
			WHERE AGV_ID = @Agv_id
				
		SET @err_code_ = @@ERROR
	
		END	-->

	RETURN @err_code_