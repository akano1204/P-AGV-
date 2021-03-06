CREATE PROCEDURE AGV_ERR_HISTORY_UPDATE
	--ﾌﾟﾛｼｰｼﾞｬ引数宣言
	@Agv_id				NVARCHAR(10)		--AGV識別ID

AS
	
	DECLARE

		@err_code_ INT			--ｴﾗｰｺｰﾄﾞ

	--ｴﾗｰｺｰﾄﾞ初期化
	SET @err_code_ = 0

	--AGV異常履歴ﾃｰﾌﾞﾙからAGV識別ID検索
	IF EXISTS (SELECT AGV_ID FROM AGV_ERR_HISTORY
				WHERE AGV_ID = @Agv_id)

		BEGIN	--<

		--AGV異常履歴ﾃｰﾌﾞﾙﾚｺｰﾄﾞUPDATE
		UPDATE AGV_ERR_HISTORY
			SET FINISH = 1,
				END_TIME = GETDATE(),
				LAST_TIME = GETDATE()
			WHERE FINISH = 0 AND
					AGV_ID = @Agv_id

		SET @err_code_ = @@ERROR

		END		-->

	ELSE

		BEGIN	--<

		SET @err_code_ = -1

		END		-->

	RETURN @err_code_