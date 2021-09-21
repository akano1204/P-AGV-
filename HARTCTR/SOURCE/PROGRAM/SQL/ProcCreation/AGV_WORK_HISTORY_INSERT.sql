CREATE PROCEDURE AGV_WORK_HISTORY_INSERT
	--ﾌﾟﾛｼｰｼﾞｬ引数宣言
	@Id					INT = 0 OUTPUT,		--ID(戻り値)
	@Order_no			INT = 0,			--動作指示親番
	@Order_sub_no		INT = 0,			--動作指示子番
	@From_qr			NVARCHAR(20),		--現在地QR
	@To_qr				NVARCHAR(20),		--目的地QR
	@Rack_id			NVARCHAR(20)		--ﾗｯｸID

AS
	
	DECLARE

		@err_code_		INT,				--ｴﾗｰｺｰﾄﾞ

		@agv_id_		NVARCHAR(10)		--AGV識別ID(AGV指示ﾃｰﾌﾞﾙより)

	--ｴﾗｰｺｰﾄﾞ初期化
	SET @err_code_ = 0

	--AGV識別ID初期化
	SET @agv_id_ = ''

	--AGV動作指示ﾃｰﾌﾞﾙからAGV識別ID検索
	IF EXISTS (SELECT AGV_ID
				FROM AGV_ORDER
				WHERE ORDER_NO = @Order_no AND
						ORDER_SUB_NO = @Order_sub_no)

		BEGIN	--<

		--AGV動作指示ﾃｰﾌﾞﾙからAGV識別ID検索
		SELECT @agv_id_ = AGV_ID
			FROM AGV_ORDER
			WHERE ORDER_NO = @Order_no AND
					ORDER_SUB_NO = @Order_sub_no

			--AGV搬送履歴ﾃｰﾌﾞﾙﾚｺｰﾄﾞINSERT
			INSERT INTO AGV_WORK_HISTORY
					(MAKE_DATE,
					ORDER_NO,
					ORDER_SUB_NO,
					AGV_ID,
					START_TIME,
					END_TIME,
					FROM_QR,
					TO_QR,
					RACK_ID,
					MAKE_TIME,
					LAST_TIME)
					VALUES(
					CONVERT(CHAR, GETDATE(), 112),
					@Order_no,
					@Order_sub_no,
					@agv_id_,
					GETDATE(),
					NULL,
					@From_qr,
					@To_qr,
					@Rack_id,
					GETDATE(),
					GETDATE())
			
			SET @Id = SCOPE_IDENTITY()

			SET @err_code_ = @@ERROR

		END		-->

	ELSE

		BEGIN	--<

		SET @err_code_ = -1

		END		-->

	RETURN @err_code_