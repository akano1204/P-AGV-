CREATE PROCEDURE AGV_WORK_HISTORY_UPDATE
	--ﾌﾟﾛｼｰｼﾞｬ引数宣言
	@Order_no			INT = 0,			--動作指示親番
	@Order_sub_no		INT = 0				--動作指示子番

AS
	
	DECLARE

		@err_code_ INT			--ｴﾗｰｺｰﾄﾞ

	--ｴﾗｰｺｰﾄﾞ初期化
	SET @err_code_ = 0

	--AGV搬送履歴ﾃｰﾌﾞﾙからAGV識別ID検索
	IF EXISTS (SELECT ID FROM AGV_WORK_HISTORY
				WHERE ORDER_NO = @Order_no AND
						ORDER_SUB_NO = @Order_sub_no)

		BEGIN	--<

		--AGV搬送履歴ﾃｰﾌﾞﾙﾚｺｰﾄﾞUPDATE
		UPDATE AGV_WORK_HISTORY
			SET END_TIME = GETDATE(),
				LAST_TIME = GETDATE()
			WHERE ORDER_NO = @Order_no AND
					ORDER_SUB_NO = @Order_sub_no

		SET @err_code_ = @@ERROR

		END		-->

	ELSE

		BEGIN	--<

		SET @err_code_ = -1

		END		-->

	RETURN @err_code_