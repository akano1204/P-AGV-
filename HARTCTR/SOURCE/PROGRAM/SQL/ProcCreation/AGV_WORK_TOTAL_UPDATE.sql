CREATE PROCEDURE AGV_WORK_TOTAL_UPDATE
	--��ۼ��ެ�����錾
	@Agv_id				NVARCHAR(10),		--AGV����ID
	@Run_seconds		INT = 0,			--���s����(�b)
	@Charge_seconds		INT = 0,			--�[�d����(�b)
	@Work_seconds		INT = 0,			--��������(�b)
	@Work_count			INT = 0,			--������
	@Lift_count			INT = 0,			--���~��
	@Run_distance		INT = 0,			--���s����(cm)
	@Alert_lift_count	INT = 0,			--�x�񏸍~��
	@Alert_run_distance	INT = 0,			--�x�񑖍s����(cm)
	@Repair_time1		DATETIME = NULL,	--��Î���1
	@Repair_time2		DATETIME = NULL,	--��Î���2
	@Repair_time3		DATETIME = NULL		--��Î���3

AS
	
	DECLARE

		@err_code_ INT			--�װ����

	--�װ���ޏ�����
	SET @err_code_ = 0

	--AGV�ғ��ݐ�ð��ق���AGV����ID����
	IF NOT EXISTS (SELECT AGV_ID FROM AGV_WORK_TOTAL
				WHERE AGV_ID = @Agv_id)

		BEGIN	--<
		--AGV�ғ��ݐ�ð���ں���INSERT
		INSERT INTO AGV_WORK_TOTAL
				(AGV_ID,
				RUN_SECONDS,
				CHARGE_SECONDS,
				WORK_SECONDS,
				WORK_COUNT,
				LIFT_COUNT,
				RUN_DISTANCE,
				ALERT_LIFT_COUNT,
				ALERT_RUN_DISTANCE,
				REPAIR_TIME1,
				REPAIR_TIME2,
				REPAIR_TIME3,
				MAKE_TIME,
				LAST_TIME)
				VALUES(
				@Agv_id,
				@Run_seconds,
				@Charge_seconds,
				@Work_seconds,
				@Work_count,
				@Lift_count,
				@Run_distance,
				@Alert_lift_count,
				@Alert_run_distance,
				@Repair_time1,
				@Repair_time2,
				@Repair_time3,
				GETDATE(),
				GETDATE())
				
		SET @err_code_ = @@ERROR
	
		END	-->

	ELSE

		BEGIN	--<
		--AGV�ғ��ݐ�ð���ں���UPDATE
		UPDATE AGV_WORK_TOTAL
			SET RUN_SECONDS = @Run_seconds,
				CHARGE_SECONDS = @Charge_seconds,
				WORK_SECONDS = @Work_seconds,
				WORK_COUNT = @Work_count,
				LIFT_COUNT = @Lift_count,
				RUN_DISTANCE = @Run_distance,
				ALERT_LIFT_COUNT = @Alert_lift_count,
				ALERT_RUN_DISTANCE = @Alert_run_distance,
				REPAIR_TIME1 = @Repair_time1,
				REPAIR_TIME2 = @Repair_time2,
				REPAIR_TIME3 = @Repair_time3,
				LAST_TIME = GETDATE()

			WHERE AGV_ID = @Agv_id
				
		SET @err_code_ = @@ERROR
	
		END	-->

	RETURN @err_code_