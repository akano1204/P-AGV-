CREATE PROCEDURE AGV_WORK_HISTORY_UPDATE
	--��ۼ��ެ�����錾
	@Order_no			INT = 0,			--����w���e��
	@Order_sub_no		INT = 0				--����w���q��

AS
	
	DECLARE

		@err_code_ INT			--�װ����

	--�װ���ޏ�����
	SET @err_code_ = 0

	--AGV��������ð��ق���AGV����ID����
	IF EXISTS (SELECT ID FROM AGV_WORK_HISTORY
				WHERE ORDER_NO = @Order_no AND
						ORDER_SUB_NO = @Order_sub_no)

		BEGIN	--<

		--AGV��������ð���ں���UPDATE
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