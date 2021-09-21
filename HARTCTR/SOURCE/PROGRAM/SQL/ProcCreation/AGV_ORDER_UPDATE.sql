CREATE PROCEDURE AGV_ORDER_UPDATE
	--ﾌﾟﾛｼｰｼﾞｬ引数宣言
	@Finish				INT = 0,			--完了ﾌﾗｸﾞ
	@Response			INT = 0,			--応答ﾌﾗｸﾞ
	@Order_no			INT = 0,			--動作指示親番
	@Order_sub_no		INT = 0,			--動作指示子番
	@Order_op1			INT = 0,			--動作OP1
	@Order_op2			INT = 0,			--動作OP2
	@Order_op3			INT = 0,			--動作OP3
	@Order_op4			INT = 0,			--動作OP4
	@Order_op5			INT = 0,			--動作OP5
	@O_info1			NVARCHAR(50),		--動作情報01
	@O_info2			NVARCHAR(50),		--動作情報02
	@O_info3			NVARCHAR(50),		--動作情報03
	@O_info4			NVARCHAR(50),		--動作情報04
	@O_info5			NVARCHAR(50)		--動作情報05

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
			SET FINISH = @Finish,
				RESPONSE = @Response,
				ORDER_OP1 = @Order_op1,
				ORDER_OP2 = @Order_op2,
				ORDER_OP3 = @Order_op3,
				ORDER_OP4 = @Order_op4,
				ORDER_OP5 = @Order_op5,
				O_INFO1 = @O_info1,
				O_INFO2 = @O_info2,
				O_INFO3 = @O_info3,
				O_INFO4 = @O_info4,
				O_INFO5 = @O_info5,
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