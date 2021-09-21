CREATE PROCEDURE STATION_INFO_UPDATE
	--��ۼ��ެ�����錾
	@St_id				NVARCHAR(20),			--�ð���ID
	@St_name			NVARCHAR(100),			--�ð��ݖ�
	@St_type			INT,					--�ð�������
	@St_attribute		INT						--�ð��ݑ���

AS
	
	DECLARE

		@err_code_ INT			--�װ����

	--�װ���ޏ�����
	SET @err_code_ = 0

	--AGV�ð��ݏ��ð��ق���ð���ID����
	IF NOT EXISTS (SELECT ST_ID FROM STATION_INFO
				WHERE ST_ID = @St_id)

		BEGIN	--<
		--AGV�ð��ݏ��ð���ں���INSERT
		INSERT INTO STATION_INFO
				(ST_ID,
				ST_NAME,
				ST_TYPE,
				ST_ATTRIBUTE,
				MAKE_TIME,
				LAST_TIME)
				VALUES(
				@St_id,
				@St_name,
				@St_type,
				@St_attribute,
				GETDATE(),
				GETDATE())
				
		SET @err_code_ = @@ERROR
	
		END	-->

	ELSE

		BEGIN	--<
		--AGV�ð��ݏ��ð���ں���UPDATE
		UPDATE STATION_INFO
			SET ST_NAME = @St_name,
				ST_TYPE = @St_type,
				ST_ATTRIBUTE = @St_attribute,
				LAST_TIME = GETDATE()
			WHERE ST_ID = @St_id
				
		SET @err_code_ = @@ERROR
	
		END	-->

	RETURN @err_code_