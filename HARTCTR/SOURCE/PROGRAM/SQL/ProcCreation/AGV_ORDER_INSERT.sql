CREATE PROCEDURE AGV_ORDER_INSERT
	--��ۼ��ެ�����錾
	@Id					INT = 0 OUTPUT,		--ID(�߂�l)
	@Order_no			INT = 0 OUTPUT,		--����w���e��(0:�̔ԁA�߂�l, 1�`:�q�Ԃ̂ݍ̔�)
	@Order_sub_no		INT = 0 OUTPUT,		--����w���q��(�߂�l)
	@Order_mark			INT = 0,			--����w���ŏIϰ�
	@Agv_id				NVARCHAR(10),		--AGV����ID
	@St_to				NVARCHAR(20),		--�ړIST
	@To_qr				NVARCHAR(20),		--�ړIQR
	@St_action			INT = 0,			--�ړI�n����
	@Rack_angle			INT = 0,			--ׯ������ʎw��
	@Sensor				INT = 0,			--�ݻp-�����
	@Music				INT = 0,			--Э��ޯ������
	@Order_op1			INT = 0,			--����OP1
	@Order_op2			INT = 0,			--����OP2
	@Order_op3			INT = 0,			--����OP3
	@Order_op4			INT = 0,			--����OP4
	@Order_op5			INT = 0,			--����OP5
	@O_info1			NVARCHAR(50),		--������01
	@O_info2			NVARCHAR(50),		--������02
	@O_info3			NVARCHAR(50),		--������03
	@O_info4			NVARCHAR(50),		--������04
	@O_info5			NVARCHAR(50)		--������05

AS
	
	DECLARE

		@err_code_		INT,				--�װ���
		
		@order_no_		INT,				--����w���e��(�Q�Ɨp)
		@order_sub_no_	INT					--����w���q��(�Q�Ɨp)

	--�װ���ޏ�����
	SET @err_code_ = 0

	--�w���ԍ�������
	SET @order_no_ = 0
	SET @order_sub_no_ = 0

	--�r��ۯ��̂�����ݻ޸��݊J�n
	BEGIN TRANSACTION

	IF @err_code_ = 0

		BEGIN	--<

		--��ݻ޸��ݏI���܂Ŕr�����b�N
		SELECT * FROM AGV_ORDER
			WITH (TABLOCKX)

		SET @err_code_ = @@ERROR

		END		-->

	IF @err_code_ = 0

		--�̔Ԃ��s��
		BEGIN	--<

		--�e�Ԏw�莞
		IF 0 < @Order_no

			BEGIN	--<<

			SELECT @order_sub_no_ = ISNULL(MAX(ORDER_SUB_NO), 0), 
					@Agv_id = ISNULL(MIN(AGV_ID), '')
				FROM AGV_ORDER
				WHERE ORDER_NO = @Order_no
				GROUP BY ORDER_NO

			IF @order_sub_no_ = 0

				BEGIN	--<<<

				SET @err_code_ = -1

				END		-->>>

			ELSE

				BEGIN	--<<<

				--�q�Ԃ��̔�
				SET @Order_sub_no = @order_sub_no_ + 1

				SET @err_code_ = @@ERROR

				END		-->>>
				
			END		-->>

		ELSE

			BEGIN	--<<

			SELECT @order_no_ = ISNULL(MAX(ORDER_NO), 0) FROM AGV_ORDER

			--�e�Ԃ��̔�
			SET @Order_no = @order_no_ + 1
			SET @Order_sub_no = 1

			SET @err_code_ = @@ERROR

			END		-->>

		END		-->

	IF @err_code_ = 0

		BEGIN	--<

		--AGV����w��ð���ں���INSERT
		INSERT INTO AGV_ORDER
				(FINISH,
				RESPONSE,
				ORDER_NO,
				ORDER_SUB_NO,
				ORDER_MARK,
				AGV_ID,
				ST_TO,
				TO_QR,
				ST_ACTION,
				RACK_ANGLE,
				SENSOR,
				MUSIC,
				ORDER_OP1,
				ORDER_OP2,
				ORDER_OP3,
				ORDER_OP4,
				ORDER_OP5,
				O_INFO1,
				O_INFO2,
				O_INFO3,
				O_INFO4,
				O_INFO5,
				MAKE_TIME,
				LAST_TIME)
				VALUES(
				0,
				0,
				@Order_no,
				@Order_sub_no,
				@Order_mark,
				@Agv_id,
				@St_to,
				@To_qr,
				@St_action,
				@Rack_angle,
				@Sensor,
				@Music,
				@Order_op1,
				@Order_op2,
				@Order_op3,
				@Order_op4,
				@Order_op5,
				@O_info1,
				@O_info2,
				@O_info3,
				@O_info4,
				@O_info5,
				GETDATE(),
				GETDATE())

		SET @Id = SCOPE_IDENTITY()
				
		SET @err_code_ = @@ERROR

		END		-->

	IF @err_code_ = 0

		BEGIN	--<

			--��ݻ޸��݂̺Я�
			COMMIT TRANSACTION

		END	-->

	ELSE

		BEGIN	--<

			--��ݻ޸��݂�۰��ޯ�
			ROLLBACK TRANSACTION

		END	-->

	RETURN @err_code_