CREATE PROCEDURE AGV_SYS_ORDER_INSERT
	--ÌßÛ¼°¼Þ¬ˆø”éŒ¾
	@Id					INT = 0 OUTPUT,		--ID(–ß‚è’l)
	@Agv_id				NVARCHAR(10),		--AGVŽ¯•ÊID
	@Order_type			INT = 0				--¼½ÃÑ“®ì

AS
	
	DECLARE

		@err_code_ INT			--´×°º°ÄÞ

	--´×°º°ÄÞ‰Šú‰»
	SET @err_code_ = 0

	--AGV¼½ÃÑ“®ìŽwŽ¦Ã°ÌÞÙÚº°ÄÞINSERT
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