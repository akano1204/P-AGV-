CREATE PROCEDURE AGV_WORK_DAY_UPDATE
	--ÌßÛ¼°¼Þ¬øé¾
	@Make_date			CHAR(8),			--út
	@Agv_id				NVARCHAR(10),		--AGV¯ÊID
	@Run_seconds		INT = 0,			--sÔ(b)
	@Charge_seconds		INT = 0,			--[dÔ(b)
	@Work_seconds		INT = 0,			--ÀÔ(b)
	@Work_count			INT = 0,			--Àñ
	@Lift_count			INT = 0,			--¸~ñ
	@Run_distance		INT = 0,			--s£(cm)
	@Wait_seconds		INT = 0				--Ò¿Ô

AS
	
	DECLARE

		@err_code_			INT,			--´×°º°ÄÞ

		@make_date_			CHAR(8) = NULL,	--õp
		@run_seconds_		INT = 0,		--sÔ(b)·ª
		@charge_seconds_	INT = 0,		--[dÔ(b)·ª
		@work_seconds_		INT = 0,		--ÀÔ(b)·ª
		@work_count_		INT = 0,		--Àñ·ª
		@lift_count_		INT = 0,		--¸~ñ·ª
		@run_distance_		INT = 0			--s£(cm)·ª

	--´×°º°ÄÞú»
	SET @err_code_ = 0

	--Ä×Ý»Þ¸¼®ÝJn
	BEGIN TRANSACTION

	IF @err_code_ = 0

		BEGIN	--<

		IF EXISTS(SELECT AGV_ID FROM AGV_WORK_DAY
					WHERE MAKE_DATE = @Make_date AND
							AGV_ID = @Agv_id)

			BEGIN	--<

			--AGVÒ­\ÍÃ°ÌÞÙÚº°ÄÞõ
			SELECT @make_date_ = MAKE_DATE, 
					@run_seconds_ = RUN_SECONDS,
					@charge_seconds_ = CHARGE_SECONDS,
					@work_seconds_ = WORK_SECONDS,
					@work_count_ = WORK_COUNT,
					@lift_count_ = LIFT_COUNT,
					@run_distance_ = RUN_DISTANCE
			FROM AGV_WORK_DAY
			WHERE MAKE_DATE = @Make_date AND
					AGV_ID = @Agv_id

			END		-->

		--el·ªð¾¯Ä
		SET @run_seconds_ = @Run_seconds - @run_seconds_
		SET @charge_seconds_ = @Charge_seconds - @charge_seconds_
		SET @work_seconds_	= @Work_seconds - @work_seconds_
		SET @work_count_ = @Work_count - @work_count_
		SET @lift_count_ = @Lift_count - @lift_count_
		SET @run_distance_	=@Run_distance - @run_distance_

		END		-->

	IF @err_code_ = 0
	
		BEGIN	--<
		
		IF @make_date_ is NULL

			BEGIN	--<<
			--AGVÒ­\ÍÃ°ÌÞÙÚº°ÄÞINSERT
			INSERT INTO AGV_WORK_DAY
				(MAKE_DATE,
				AGV_ID,
				RUN_SECONDS,
				CHARGE_SECONDS,
				WORK_SECONDS,
				WORK_COUNT,
				LIFT_COUNT,
				RUN_DISTANCE,
				WAIT_SECONDS,
				MAKE_TIME,
				LAST_TIME)
				VALUES(
				@Make_date,
				@Agv_id,
				@Run_seconds,
				@Charge_seconds,
				@Work_seconds,
				@Work_count,
				@Lift_count,
				@Run_distance,
				@Wait_seconds,
				GETDATE(),
				GETDATE())
				
			SET @err_code_ = @@ERROR

			END		-->>

		ELSE

			BEGIN	--<<
			--AGVÒ­\ÍÃ°ÌÞÙÚº°ÄÞUPDATE
			UPDATE AGV_WORK_DAY
			SET RUN_SECONDS = @Run_seconds,
				CHARGE_SECONDS = @Charge_seconds,
				WORK_SECONDS = @Work_seconds,
				WORK_COUNT = @Work_count,
				LIFT_COUNT = @Lift_count,
				RUN_DISTANCE = @Run_distance,
				WAIT_SECONDS = @Wait_seconds,
				LAST_TIME = GETDATE()

			WHERE MAKE_DATE = @Make_date AND
					AGV_ID = @Agv_id

			SET @err_code_ = @@ERROR

			END		-->>

		END		-->

	IF @err_code_ = 0

		BEGIN	--<
		--AGVÒ­ÝÏÃ°ÌÞÙ©çAGV¯ÊIDõ
		IF NOT EXISTS (SELECT AGV_ID FROM AGV_WORK_TOTAL
					WHERE AGV_ID = @Agv_id)

			BEGIN	--<<
			--AGVÒ­ÝÏÃ°ÌÞÙÚº°ÄÞINSERT
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
				@run_seconds_,
				@charge_seconds_,
				@work_seconds_,
				@work_count_,
				@lift_count_,
				@run_distance_,
				0,
				0,
				NULL,
				NULL,
				NULL,
				GETDATE(),
				GETDATE())
				
			SET @err_code_ = @@ERROR

			END		-->>

		ELSE
			
			BEGIN	--<<

			--AGVÒ­ÝÏÃ°ÌÞÙÚº°ÄÞUPDATE
			UPDATE AGV_WORK_TOTAL
				SET RUN_SECONDS = RUN_SECONDS + @run_seconds_,
					CHARGE_SECONDS = CHARGE_SECONDS + @charge_seconds_,
					WORK_SECONDS = WORK_SECONDS + @work_seconds_,
					WORK_COUNT = WORK_COUNT + @work_count_,
					LIFT_COUNT = LIFT_COUNT + @lift_count_,
					RUN_DISTANCE = RUN_DISTANCE + @run_distance_,
					LAST_TIME = GETDATE()

				WHERE AGV_ID = @Agv_id
				
			SET @err_code_ = @@ERROR

			END		-->>

		END		-->

	IF @err_code_ = 0

		BEGIN	--<

			--Ä×Ý»Þ¸¼®ÝÌºÐ¯Ä
			COMMIT TRANSACTION

		END		-->

	ELSE

		BEGIN	--<

			--Ä×Ý»Þ¸¼®ÝÌÛ°ÙÊÞ¯¸
			ROLLBACK TRANSACTION

		END		-->

	RETURN @err_code_