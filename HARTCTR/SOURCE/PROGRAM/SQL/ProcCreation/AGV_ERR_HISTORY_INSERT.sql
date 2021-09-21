CREATE PROCEDURE AGV_ERR_HISTORY_INSERT
	--��ۼ��ެ�����錾
	@Agv_id				NVARCHAR(10),		--AGV����ID
	@Err_code			INT = 0,			--�ُ���
	@Floor_qr			NVARCHAR(20)		--�ŏI��QR

AS
	
	DECLARE

		@err_code_ INT			--�װ����

	--�װ���ޏ�����
	SET @err_code_ = 0

	--AGV�ُ헚��ð���ں���INSERT
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