CREATE PROCEDURE AGV_WORK_HISTORY_INSERT
	--��ۼ��ެ�����錾
	@Id					INT = 0 OUTPUT,		--ID(�߂�l)
	@Order_no			INT = 0,			--����w���e��
	@Order_sub_no		INT = 0,			--����w���q��
	@From_qr			NVARCHAR(20),		--���ݒnQR
	@To_qr				NVARCHAR(20),		--�ړI�nQR
	@Rack_id			NVARCHAR(20)		--ׯ�ID

AS
	
	DECLARE

		@err_code_		INT,				--�װ����

		@agv_id_		NVARCHAR(10)		--AGV����ID(AGV�w��ð��ق��)

	--�װ���ޏ�����
	SET @err_code_ = 0

	--AGV����ID������
	SET @agv_id_ = ''

	--AGV����w��ð��ق���AGV����ID����
	IF EXISTS (SELECT AGV_ID
				FROM AGV_ORDER
				WHERE ORDER_NO = @Order_no AND
						ORDER_SUB_NO = @Order_sub_no)

		BEGIN	--<

		--AGV����w��ð��ق���AGV����ID����
		SELECT @agv_id_ = AGV_ID
			FROM AGV_ORDER
			WHERE ORDER_NO = @Order_no AND
					ORDER_SUB_NO = @Order_sub_no

			--AGV��������ð���ں���INSERT
			INSERT INTO AGV_WORK_HISTORY
					(MAKE_DATE,
					ORDER_NO,
					ORDER_SUB_NO,
					AGV_ID,
					START_TIME,
					END_TIME,
					FROM_QR,
					TO_QR,
					RACK_ID,
					MAKE_TIME,
					LAST_TIME)
					VALUES(
					CONVERT(CHAR, GETDATE(), 112),
					@Order_no,
					@Order_sub_no,
					@agv_id_,
					GETDATE(),
					NULL,
					@From_qr,
					@To_qr,
					@Rack_id,
					GETDATE(),
					GETDATE())
			
			SET @Id = SCOPE_IDENTITY()

			SET @err_code_ = @@ERROR

		END		-->

	ELSE

		BEGIN	--<

		SET @err_code_ = -1

		END		-->

	RETURN @err_code_