CREATE PROCEDURE AGV_ORDER_UPDATE_FINISH
	--ﾌﾟﾛｼｰｼﾞｬ引数宣言
	@Order_no			INT = 0,			--動作指示親番
	@Order_sub_no		INT = 0,			--動作指示子番
	@Response			INT = 0				--応答ﾌﾗｸﾞ

AS
	
	DECLARE

		@err_code_		INT					--ｴﾗｰｺｰﾄ
		
	--ｴﾗｰｺｰﾄﾞ初期化
	SET @err_code_ = 0

	--AGV動作指示ﾃｰﾌﾞﾙからID検索
	IF EXISTS (SELECT ID FROM AGV_ORDER
				WHERE ORDER_NO = @Order_no AND
						ORDER_SUB_NO = @Order_sub_no)

		BEGIN	--<

		--AGV動作指示ﾃｰﾌﾞﾙﾚｺｰﾄﾞUPDATE
		UPDATE AGV_ORDER
			SET FINISH = 1,
				RESPONSE = @Response,
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