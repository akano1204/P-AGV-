CREATE PROCEDURE AGV_ERR_HISTORY_UPDATE
	--ÌßÛ¼°¼Þ¬ˆø”éŒ¾
	@Agv_id				NVARCHAR(10)		--AGVŽ¯•ÊID

AS
	
	DECLARE

		@err_code_ INT			--´×°º°ÄÞ

	--´×°º°ÄÞ‰Šú‰»
	SET @err_code_ = 0

	--AGVˆÙí—š—ðÃ°ÌÞÙ‚©‚çAGVŽ¯•ÊIDŒŸõ
	IF EXISTS (SELECT AGV_ID FROM AGV_ERR_HISTORY
				WHERE AGV_ID = @Agv_id)

		BEGIN	--<

		--AGVˆÙí—š—ðÃ°ÌÞÙÚº°ÄÞUPDATE
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