CREATE TABLE PSM_PROCEDURE_LOG (
    LOG_ID           VARCHAR2(100)   NOT NULL,      -- Unique identifier for the log entry
    PROCEDURE_NAME   VARCHAR2(100)   NOT NULL,      -- Name of the procedure being executed
    START_TIME       TIMESTAMP,                     -- When the procedure started
    END_TIME         TIMESTAMP,                     -- When the procedure ended
    EVENT_TIME       TIMESTAMP,                     -- For intermediate events (optional)
    PARAMETERS       VARCHAR2(4000),                -- Input parameters to the procedure
    STATUS           VARCHAR2(20)    NOT NULL,      -- STARTED/COMPLETED/ERROR
    MESSAGE          VARCHAR2(4000),                -- Status or error message
    RECORD_COUNT     NUMBER,                       -- Number of records processed
    ERROR_CODE       VARCHAR2(100),                 -- Error code if applicable
    ERROR_STACK      CLOB,                         -- Full error stack trace
    SESSION_ID       NUMBER,                       -- Session ID that executed the procedure
    MODULE           VARCHAR2(100),                -- Application module
    ACTION           VARCHAR2(100),                -- Application action
    CLIENT_INFO      VARCHAR2(100),                -- Client information
    CONSTRAINT PSM_PROCEDURE_LOG_PK PRIMARY KEY (LOG_ID)
);

-- Index for performance
CREATE INDEX PSM_PROCEDURE_LOG_IDX1 ON PSM_PROCEDURE_LOG(PROCEDURE_NAME, START_TIME);
CREATE INDEX PSM_PROCEDURE_LOG_IDX2 ON PSM_PROCEDURE_LOG(STATUS, START_TIME);