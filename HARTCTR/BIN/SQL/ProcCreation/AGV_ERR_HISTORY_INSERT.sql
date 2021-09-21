CREATE PROCEDURE AGV_ERR_HISTORY_INSERT
	--ÌßÛ¼°¼Þ¬ˆø”éŒ¾
	@Agv_id				NVARCHAR(10),		--AGVŽ¯•ÊID
	@Err_code			INT = 0,			--ˆÙíº°ÄÞ
	@Floor_qr			NVARCHAR(20)		--ÅI°QR

AS
	
	DECLARE

		@err_code_ INT			--´×°º°ÄÞ

	--´×°º°ÄÞ‰Šú‰»
	SET @err_code_ = 0

	--AGVˆÙí—š—ðÃ°ÌÞÙÚº°ÄÞINSERT
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