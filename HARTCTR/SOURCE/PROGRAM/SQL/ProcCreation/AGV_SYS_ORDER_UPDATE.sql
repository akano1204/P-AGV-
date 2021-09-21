CREATE PROCEDURE AGV_SYS_ORDER_UPDATE
	--��ۼ��ެ�����錾
	@Id					INT = 0				--ID

AS
	
	DECLARE

		@err_code_ INT			--�װ����

	--�װ���ޏ�����
	SET @err_code_ = 0

	--AGV����w��ð��ق���AGV����ID����
	IF EXISTS (SELECT ID FROM AGV_SYS_ORDER
				WHERE ID = @Id)

		BEGIN	--<

		--AGV���ѓ���w��ð���ں���UPDATE
		UPDATE AGV_SYS_ORDER
			SET FINISH = 1,
				LAST_TIME = GETDATE()
			WHERE ID = @Id

		SET @err_code_ = @@ERROR

		END		-->

	ELSE

		BEGIN	--<

		SET @err_code_ = -1

		END		-->

	RETURN @err_code_