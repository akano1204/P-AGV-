CREATE PROCEDURE AGV_STATE_UPDATE
	--��ۼ��ެ�����錾
	@Agv_id				NVARCHAR(10),		--AGV����ID
	@Command_time		DATETIME,			--�ŏI����ގ�M����
	@Action				CHAR(1) = '',		--�^�q���
	@Order_no			INT = 0,			--����w���e��
	@Order_sub_no		INT = 0,			--����w���q��
	@Battery			INT = 0,			--�[�d�c��
	@Battery_state		INT = 0,			--�ޯ�ذ���
	@Action_mode		CHAR(1) = '',		--����Ӱ��
	@Map				CHAR(1) = '',		--����ϯ��
	@Location_x			INT = 0,			--���ݒnX���W
	@Location_y			INT = 0,			--���ݒnY���W
	@Rack_id			NVARCHAR(20),		--ׯ�ID
	@Err_code			INT = 0				--�ُ���

AS
	
	DECLARE

		@err_code_ INT			--�װ����

	--�װ���ޏ�����
	SET @err_code_ = 0

	--AGV���ð��ق���AGV����ID����
	IF NOT EXISTS (SELECT AGV_ID FROM AGV_STATE
				WHERE AGV_ID = @Agv_id)

		BEGIN	--<
		--AGV���ð���ں���INSERT
		INSERT INTO AGV_STATE
				(AGV_ID,
				COMMAND_TIME,
				ACTION,
				ORDER_NO,
				ORDER_SUB_NO,
				BATTERY,
				BATTERY_STATE,
				ACTION_MODE,
				MAP,
				LOCATION_X,
				LOCATION_Y,
				RACK_ID,
				ERR_CODE,
				LAST_TIME)
				VALUES(
				@Agv_id,
				@Command_time,
				@Action,
				@Order_no,
				@Order_sub_no,
				@Battery,
				@Battery_state,
				@Action_mode,
				@Map,
				@Location_x,
				@Location_y,
				@Rack_id,
				@Err_code,
				GETDATE())
				
		SET @err_code_ = @@ERROR
	
		END	-->

	ELSE

		BEGIN	--<
		--AGV���ð���ں���UPDATE
		UPDATE AGV_STATE
			SET COMMAND_TIME = @Command_time,
				ACTION = @Action,
				ORDER_NO = @Order_no,
				ORDER_SUB_NO = @Order_sub_no,
				BATTERY = @Battery,
				BATTERY_STATE = @Battery_state,
				ACTION_MODE = @Action_mode,
				MAP = @Map,
				LOCATION_X = @Location_x,
				LOCATION_Y = @Location_y,
				RACK_ID = @Rack_id,
				ERR_CODE = @Err_code,
				LAST_TIME = GETDATE()
			WHERE AGV_ID = @Agv_id
				
		SET @err_code_ = @@ERROR
	
		END	-->

	RETURN @err_code_