CREATE PROCEDURE AGV_ERR_HISTORY_UPDATE
	--��ۼ��ެ�����錾
	@Agv_id				NVARCHAR(10)		--AGV����ID

AS
	
	DECLARE

		@err_code_ INT			--�װ����

	--�װ���ޏ�����
	SET @err_code_ = 0

	--AGV�ُ헚��ð��ق���AGV����ID����
	IF EXISTS (SELECT AGV_ID FROM AGV_ERR_HISTORY
				WHERE AGV_ID = @Agv_id)

		BEGIN	--<

		--AGV�ُ헚��ð���ں���UPDATE
		UPDATE AGV_ERR_HISTORY
			SET FINISH = 1,
				END_TIME = GETDATE(),
				LAST_TIME = GETDATE()
			WHERE FINISH = 0 AND
					AGV_ID = @Agv_id

		SET @err_code_ = @@ERROR

		END		-->

	ELSE

		BEGIN	--<

		SET @err_code_ = -1

		END		-->

	RETURN @err_code_