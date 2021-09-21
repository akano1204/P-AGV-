CREATE PROCEDURE AGV_WORK_TOTAL_UPDATE
	--ÌßÛ¼°¼Ş¬ˆø”éŒ¾
	@Agv_id				NVARCHAR(10),		--AGV¯•ÊID
	@Run_seconds		INT = 0,			--‘–sŠÔ(•b)
	@Charge_seconds		INT = 0,			--[“dŠÔ(•b)
	@Work_seconds		INT = 0,			--”À‘—ŠÔ(•b)
	@Work_count			INT = 0,			--”À‘—‰ñ”
	@Lift_count			INT = 0,			--¸~‰ñ”
	@Run_distance		INT = 0,			--‘–s‹——£(cm)
	@Alert_lift_count	INT = 0,			--Œx•ñ¸~‰ñ”
	@Alert_run_distance	INT = 0,			--Œx•ñ‘–s‹——£(cm)
	@Repair_time1		DATETIME = NULL,	--ÒİÃŠÔ1
	@Repair_time2		DATETIME = NULL,	--ÒİÃŠÔ2
	@Repair_time3		DATETIME = NULL		--ÒİÃŠÔ3

AS
	
	DECLARE

		@err_code_ INT			--´×°º°ÄŞ

	--´×°º°ÄŞ‰Šú‰»
	SET @err_code_ = 0

	--AGV‰Ò“­—İÏÃ°ÌŞÙ‚©‚çAGV¯•ÊIDŒŸõ
	IF NOT EXISTS (SELECT AGV_ID FROM AGV_WORK_TOTAL
				WHERE AGV_ID = @Agv_id)

		BEGIN	--<
		--AGV‰Ò“­—İÏÃ°ÌŞÙÚº°ÄŞINSERT
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
		--AGV‰Ò“­—İÏÃ°ÌŞÙÚº°ÄŞUPDATE
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