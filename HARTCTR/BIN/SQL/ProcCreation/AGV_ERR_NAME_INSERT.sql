CREATE PROCEDURE AGV_ERR_NAME_INSERT
	--��ۼ��ެ�����錾
	@Err_code			INT,					--�ُ���
	@Err_name			NVARCHAR(45),			--�ُ햼��
	@Err_detail			NVARCHAR(200) = '',		--�ُ�ڍ�
	@Err_recovery		NVARCHAR(200) = ''		--�������@

AS
	
	DECLARE

		@err_code_ INT			--�װ����

	--�װ���ޏ�����
	SET @err_code_ = 0

	--AGV�ُ햼��ð��ق���ُ��ތ���
	IF NOT EXISTS (SELECT ERR_CODE FROM AGV_ERR_NAME
				WHERE ERR_CODE = @Err_code)

		BEGIN	--<
		--�ُ�ð���ں���INSERT
		INSERT INTO AGV_ERR_NAME
				(ERR_CODE,
				ERR_NAME,
				ERR_DETAIL,
				ERR_RECOVERY,
				MAKE_TIME)
				VALUES(
				@Err_code,
				@Err_name,
				@Err_detail,
				@Err_recovery,
				GETDATE())
				
		SET @err_code_ = @@ERROR
	
		END	-->

	ELSE

		BEGIN	--<
		--�ُ�ð���ں���UPDATE
		UPDATE AGV_ERR_NAME
			SET ERR_NAME = @Err_name,
				ERR_DETAIL = @Err_detail,
				ERR_RECOVERY = @Err_recovery
			WHERE ERR_CODE = @Err_code
				
		SET @err_code_ = @@ERROR
	
		END	-->

	RETURN @err_code_