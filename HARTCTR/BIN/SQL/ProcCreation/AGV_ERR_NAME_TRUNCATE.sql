CREATE PROCEDURE AGV_ERR_NAME_TRUNCATE

AS
	
	DECLARE

		@err_code_ INT			--�װ����

	--�װ���ޏ�����
	SET @err_code_ = 0

	--AGV�ُ햼��ð��ق���Sں��ޏ���
	TRUNCATE TABLE AGV_ERR_NAME

	SET @err_code_ = @@ERROR

	RETURN @err_code_