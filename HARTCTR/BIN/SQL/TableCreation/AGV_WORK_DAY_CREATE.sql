CREATE TABLE AGV_WORK_DAY(
	
	MAKE_DATE CHAR(8) NOT NULL DEFAULT '',
	AGV_ID NVARCHAR(10) NOT NULL DEFAULT '',
	RUN_SECONDS INT NOT NULL DEFAULT 0,
	CHARGE_SECONDS INT NOT NULL DEFAULT 0,
	WORK_SECONDS INT NOT NULL DEFAULT 0,
	WORK_COUNT INT NOT NULL DEFAULT 0,
	LIFT_COUNT INT NOT NULL DEFAULT 0,
	RUN_DISTANCE INT NOT NULL DEFAULT 0,
	WAIT_SECONDS INT NOT NULL DEFAULT 0,
	MAKE_TIME DATETIME NOT NULL DEFAULT GETDATE(),
	LAST_TIME DATETIME NOT NULL DEFAULT GETDATE(),
	
	CONSTRAINT AGV_WORK_DAY_PK PRIMARY KEY(MAKE_DATE, AGV_ID))