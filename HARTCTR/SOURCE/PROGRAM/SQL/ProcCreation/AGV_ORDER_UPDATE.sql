CREATE PROCEDURE AGV_ORDER_UPDATE
	--��ۼ��ެ�����錾
	@Finish				INT = 0,			--�����׸�
	@Response			INT = 0,			--�����׸�
	@Order_no			INT = 0,			--����w���e��
	@Order_sub_no		INT = 0,			--����w���q��
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

		@err_code_		INT					--�װ���
		
	--�װ���ޏ�����
	SET @err_code_ = 0

	--AGV����w��ð��ق���ID����
	IF EXISTS (SELECT ID FROM AGV_ORDER
				WHERE ORDER_NO = @Order_no AND
						ORDER_SUB_NO = @Order_sub_no)

		BEGIN	--<

		--AGV����w��ð���ں���UPDATE
		UPDATE AGV_ORDER
			SET FINISH = @Finish,
				RESPONSE = @Response,
				ORDER_OP1 = @Order_op1,
				ORDER_OP2 = @Order_op2,
				ORDER_OP3 = @Order_op3,
				ORDER_OP4 = @Order_op4,
				ORDER_OP5 = @Order_op5,
				O_INFO1 = @O_info1,
				O_INFO2 = @O_info2,
				O_INFO3 = @O_info3,
				O_INFO4 = @O_info4,
				O_INFO5 = @O_info5,
				LAST_TIME = GETDATE()
				WHERE ORDER_NO = @Order_no AND
						ORDER_SUB_NO = @Order_sub_no

		SET @err_code_ = @@ERROR

		END		-->

	ELSE

		BEGIN	--<

		SET @err_code_ = -1

		END		-->

	RETURN @err_code_