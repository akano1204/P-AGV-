CREATE PROCEDURE AUTORATOR_STATE_UPDATE
	--��ۼ��ެ�����錾
	@Autorator_id		INT = 0,					--���ڰ������ID
	@Command_time		DATETIME = NULL,		--�ŏI����ގ�M����
	@Total_state		INT = 0,				--���
	@Err_code			INT = 0,				--�ُ���
	@Floor				INT = 0,				--��د�ސ��݊K
	@In_floor			INT = 0,				--�����K
	@In_state			INT = 0,				--�������
	@Out_floor			INT = 0,				--���o�K
	@Out_state			INT = 0					--���o���

AS
	
	DECLARE

		@err_code_ INT			--�װ����

	--�װ���ޏ�����
	SET @err_code_ = 0

	--�����@���ð��ق��絰�ڰ������ID����
	IF NOT EXISTS (SELECT AUTORATOR_ID FROM AUTORATOR_STATE
				WHERE AUTORATOR_ID = @Autorator_id)

		BEGIN	--<
		--�����@���ð���ں���INSERT
		INSERT INTO AUTORATOR_STATE
				(COMMAND_TIME,
				TOTAL_STATE,
				ERR_CODE,
				FLOOR,
				IN_FLOOR,
				IN_STATE,
				OUT_FLOOR,
				OUT_STATE,
				MAKE_TIME,
				LAST_TIME)
				VALUES(
				@Command_time,
				@Total_state,
				@Err_code,
				@Floor,
				@In_floor,
				@In_state,
				@Out_floor,
				@Out_state,
				GETDATE(),
				GETDATE())
				
		SET @err_code_ = @@ERROR
	
		END	-->

	ELSE

		BEGIN	--<
		--�����@���ð���ں���UPDATE
		UPDATE AUTORATOR_STATE
			SET COMMAND_TIME = @Command_time,
				TOTAL_STATE = @Total_state,
				ERR_CODE = @Err_code,
				FLOOR = @Floor,
				IN_FLOOR = @In_floor,
				IN_STATE = @In_state,
				OUT_FLOOR = @Out_floor,
				OUT_STATE = @Out_state,
				MAKE_TIME = GETDATE(),
				LAST_TIME = GETDATE()
			WHERE AUTORATOR_ID = @Autorator_id
				
		SET @err_code_ = @@ERROR
	
		END	-->

	RETURN @err_code_