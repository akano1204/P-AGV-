CREATE PROCEDURE AGV_ORDER_UPDATE_FINISH
	--��ۼ��ެ�����錾
	@Order_no			INT = 0,			--����w���e��
	@Order_sub_no		INT = 0,			--����w���q��
	@Response			INT = 0				--�����׸�

AS
	
	DECLARE

		@err_code_		INT					--�װ���
		
	--�װ���ޏ�����
	SET @err_code_ = 0

	--AGV����w��ð��ق���ID����
	IF EXISTS (SELECT ID FROM AGV_ORDER
				WHERE ORDER_NO = @Order_no AND
						ORDER_SUB_NO = @Order_sub_no)

		BEGIN	--<

		--AGV����w��ð���ں���UPDATE
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