CREATE PROCEDURE AGV_ERR_NAME_TRUNCATE

AS
	
	DECLARE

		@err_code_ INT			--´×°º°ÄÞ

	--´×°º°ÄÞ‰Šú‰»
	SET @err_code_ = 0

	--AGVˆÙí–¼ÌÃ°ÌÞÙ‚©‚ç‘SÚº°ÄÞœ‹Ž
	TRUNCATE TABLE AGV_ERR_NAME

	SET @err_code_ = @@ERROR

	RETURN @err_code_