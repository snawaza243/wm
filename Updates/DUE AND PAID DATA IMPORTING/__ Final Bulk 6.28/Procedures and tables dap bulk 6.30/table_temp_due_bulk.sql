DROP TABLE WEALTHMAKER.PSM_DAP_TEMP_DUE1 CASCADE CONSTRAINTS;

CREATE TABLE WEALTHMAKER.PSM_DAP_TEMP_DUE1
(
  POLICY_NO       VARCHAR2(100 BYTE),
  COMPANY_CD      VARCHAR2(100 BYTE),
  STATUS_CD       VARCHAR2(100 BYTE),
  LOCATION        VARCHAR2(100 BYTE),
  CL_NAME         VARCHAR2(100 BYTE),
  PREM_AMT        VARCHAR2(100 BYTE),
  PAY_MODE        VARCHAR2(100 BYTE),
  SA              VARCHAR2(100 BYTE),
  DUE_DATE        VARCHAR2(100 BYTE),
  CL_ADD1         VARCHAR2(100 BYTE),
  CL_ADD2         VARCHAR2(100 BYTE),
  CL_ADD3         VARCHAR2(100 BYTE),
  CL_ADD4         VARCHAR2(100 BYTE),
  CL_ADD5         VARCHAR2(100 BYTE),
  CL_CITY         VARCHAR2(100 BYTE),
  CL_PIN          VARCHAR2(100 BYTE),
  CL_PHONE1       VARCHAR2(100 BYTE),
  CL_PHONE2       VARCHAR2(100 BYTE),
  CL_MOBILE       VARCHAR2(100 BYTE),
  PLAN_NAME       VARCHAR2(100 BYTE),
  DOC             VARCHAR2(100 BYTE),
  PREM_FREQ       VARCHAR2(100 BYTE),
  PLY_TERM        VARCHAR2(100 BYTE),
  PPT             VARCHAR2(100 BYTE),
  NET_AMOUNT      VARCHAR2(100 BYTE),
  MON_NO          VARCHAR2(100 BYTE),
  YEAR_NO         VARCHAR2(100 BYTE),
  IMPORTDATATYPE  VARCHAR2(100 BYTE),
  NEWINSERT       VARCHAR2(100 BYTE)
)
TABLESPACE USERS
PCTUSED    0
PCTFREE    10
INITRANS   1
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
LOGGING 
NOCOMPRESS 
NOCACHE
MONITORING;
