CREATE PROCEDURE AGV_SYS_ORDER_UPDATE
	--ÌßÛ¼°¼Þ¬ˆø”éŒ¾
	@Id					INT = 0				--ID

AS
	
	DECLARE

		@err_code_ INT			--´×°º°ÄÞ

	--´×°º°ÄÞ‰Šú‰»
	SET @err_code_ = 0

	--AGV“®ìŽwŽ¦Ã°ÌÞÙ‚©‚çAGVŽ¯•ÊIDŒŸõ
	IF EXISTS (SELECT ID FROM AGV_SYS_ORDER
				WHERE ID = @Id)

		BEGIN	--<

		--AGV¼½ÃÑ“®ìŽwŽ¦Ã°ÌÞÙÚº°ÄÞUPDATE
		UPDATE AGV_SYS_ORDER
			SET FINISH = 1,
				LAST_TIME = GETDATE()
			WHERE ID = @Id

		SET @err_code_ = @@ERROR

		END		-->

	ELSE

		BEGIN	--<

		SET @err_code_ = -1

		END		-->

	RETURN @err_code_