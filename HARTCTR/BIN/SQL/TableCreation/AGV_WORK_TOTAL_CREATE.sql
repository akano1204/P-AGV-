CREATE TABLE AGV_WORK_TOTAL(
	
	AGV_ID NVARCHAR(10) NOT NULL DEFAULT '',
	RUN_SECONDS INT NOT NULL DEFAULT 0,
	CHARGE_SECONDS INT NOT NULL DEFAULT 0,
	WORK_SECONDS INT NOT NULL DEFAULT 0,
	WORK_COUNT INT NOT NULL DEFAULT 0,
	LIFT_COUNT INT NOT NULL DEFAULT 0,
	RUN_DISTANCE INT NOT NULL DEFAULT 0,
	ALERT_LIFT_COUNT INT NOT NULL DEFAULT 0,
	ALERT_RUN_DISTANCE INT NOT NULL DEFAULT 0,
	REPAIR_TIME1 DATETIME DEFAULT NULL,
	REPAIR_TIME2 DATETIME DEFAULT NULL,
	REPAIR_TIME3 DATETIME DEFAULT NULL,
	MAKE_TIME DATETIME NOT NULL DEFAULT GETDATE(),
	LAST_TIME DATETIME NOT NULL DEFAULT GETDATE(),
	
	CONSTRAINT AGV_WORK_TOTAL_PK PRIMARY KEY(AGV_ID))