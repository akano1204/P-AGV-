CREATE PROCEDURE AGV_SYS_ORDER_INSERT
	--��ۼ��ެ�����錾
	@Id					INT = 0 OUTPUT,		--ID(�߂�l)
	@Agv_id				NVARCHAR(10),		--AGV����ID
	@Order_type			INT = 0				--���ѓ���

AS
	
	DECLARE

		@err_code_ INT			--�װ����

	--�װ���ޏ�����
	SET @err_code_ = 0

	--AGV���ѓ���w��ð���ں���INSERT
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