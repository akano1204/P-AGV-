CREATE PROCEDURE AGV_SYS_ORDER_UPDATE
	--ﾌﾟﾛｼｰｼﾞｬ引数宣言
	@Id					INT = 0				--ID

AS
	
	DECLARE

		@err_code_ INT			--ｴﾗｰｺｰﾄﾞ

	--ｴﾗｰｺｰﾄﾞ初期化
	SET @err_code_ = 0

	--AGV動作指示ﾃｰﾌﾞﾙからAGV識別ID検索
	IF EXISTS (SELECT ID FROM AGV_SYS_ORDER
				WHERE ID = @Id)

		BEGIN	--<

		--AGVｼｽﾃﾑ動作指示ﾃｰﾌﾞﾙﾚｺｰﾄﾞUPDATE
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