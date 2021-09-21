CREATE PROCEDURE AGV_ORDER_INSERT
	--ﾌﾟﾛｼｰｼﾞｬ引数宣言
	@Id					INT = 0 OUTPUT,		--ID(戻り値)
	@Order_no			INT = 0 OUTPUT,		--動作指示親番(0:採番、戻り値, 1～:子番のみ採番)
	@Order_sub_no		INT = 0 OUTPUT,		--動作指示子番(戻り値)
	@Order_mark			INT = 0,			--動作指示最終ﾏｰｸ
	@Agv_id				NVARCHAR(10),		--AGV識別ID
	@St_to				NVARCHAR(20),		--目的ST
	@To_qr				NVARCHAR(20),		--目的QR
	@St_action			INT = 0,			--目的地動作
	@Rack_angle			INT = 0,			--ﾗｯｸ到着面指定
	@Sensor				INT = 0,			--ｾﾝｻp-ﾊﾟﾀｰﾝ
	@Music				INT = 0,			--ﾐｭｰｼﾞｯｸﾊﾟﾀｰﾝ
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

		@err_code_		INT,				--ｴﾗｰｺｰﾄ
		
		@order_no_		INT,				--動作指示親番(参照用)
		@order_sub_no_	INT					--動作指示子番(参照用)

	--ｴﾗｰｺｰﾄﾞ初期化
	SET @err_code_ = 0

	--指示番号初期化
	SET @order_no_ = 0
	SET @order_sub_no_ = 0

	--排他ﾛｯｸのためﾄﾗﾝｻﾞｸｼｮﾝ開始
	BEGIN TRANSACTION

	IF @err_code_ = 0

		BEGIN	--<

		--ﾄﾗﾝｻﾞｸｼｮﾝ終了まで排他ロック
		SELECT * FROM AGV_ORDER
			WITH (TABLOCKX)

		SET @err_code_ = @@ERROR

		END		-->

	IF @err_code_ = 0

		--採番を行う
		BEGIN	--<

		--親番指定時
		IF 0 < @Order_no

			BEGIN	--<<

			SELECT @order_sub_no_ = ISNULL(MAX(ORDER_SUB_NO), 0), 
					@Agv_id = ISNULL(MIN(AGV_ID), '')
				FROM AGV_ORDER
				WHERE ORDER_NO = @Order_no
				GROUP BY ORDER_NO

			IF @order_sub_no_ = 0

				BEGIN	--<<<

				SET @err_code_ = -1

				END		-->>>

			ELSE

				BEGIN	--<<<

				--子番を採番
				SET @Order_sub_no = @order_sub_no_ + 1

				SET @err_code_ = @@ERROR

				END		-->>>
				
			END		-->>

		ELSE

			BEGIN	--<<

			SELECT @order_no_ = ISNULL(MAX(ORDER_NO), 0) FROM AGV_ORDER

			--親番を採番
			SET @Order_no = @order_no_ + 1
			SET @Order_sub_no = 1

			SET @err_code_ = @@ERROR

			END		-->>

		END		-->

	IF @err_code_ = 0

		BEGIN	--<

		--AGV動作指示ﾃｰﾌﾞﾙﾚｺｰﾄﾞINSERT
		INSERT INTO AGV_ORDER
				(FINISH,
				RESPONSE,
				ORDER_NO,
				ORDER_SUB_NO,
				ORDER_MARK,
				AGV_ID,
				ST_TO,
				TO_QR,
				ST_ACTION,
				RACK_ANGLE,
				SENSOR,
				MUSIC,
				ORDER_OP1,
				ORDER_OP2,
				ORDER_OP3,
				ORDER_OP4,
				ORDER_OP5,
				O_INFO1,
				O_INFO2,
				O_INFO3,
				O_INFO4,
				O_INFO5,
				MAKE_TIME,
				LAST_TIME)
				VALUES(
				0,
				0,
				@Order_no,
				@Order_sub_no,
				@Order_mark,
				@Agv_id,
				@St_to,
				@To_qr,
				@St_action,
				@Rack_angle,
				@Sensor,
				@Music,
				@Order_op1,
				@Order_op2,
				@Order_op3,
				@Order_op4,
				@Order_op5,
				@O_info1,
				@O_info2,
				@O_info3,
				@O_info4,
				@O_info5,
				GETDATE(),
				GETDATE())

		SET @Id = SCOPE_IDENTITY()
				
		SET @err_code_ = @@ERROR

		END		-->

	IF @err_code_ = 0

		BEGIN	--<

			--ﾄﾗﾝｻﾞｸｼｮﾝのｺﾐｯﾄ
			COMMIT TRANSACTION

		END	-->

	ELSE

		BEGIN	--<

			--ﾄﾗﾝｻﾞｸｼｮﾝのﾛｰﾙﾊﾞｯｸ
			ROLLBACK TRANSACTION

		END	-->

	RETURN @err_code_