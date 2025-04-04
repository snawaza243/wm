insert into agent_master (agent_name) values('dum1');

select 
        loggeduserid,               PAIDFLAG,               AGENT_NAME,               SOURCEID,               RM_CODE,              ADDRESS1,             ADDRESS2,             
        CITY_ID,                    LOCATION_ID,            MOBILE,                   PINCODE,                FAX,                  CONTACTPER,           EMAIL,   
        TDS  ,                      SUB_BROKER_TYPE ,       category_id,              CPEMAILID ,             TIMEST ,              PHONE     ,           REMARK     ,            
        master_ana,                 ONLINE_SUBSCIPTION ,    ONLINE_BLOCK_AGENT,       BLOCK_AGENT,            online_block_remark,  offline_block_remark, ANA_AUDITDATE,
        ANA_AUDIT,                  PAYMENTMODEID,          ACCTYPEID,                ACCNO,                  AFFECTEDFROM,         BANKID,               CITY_NAME,            
        BANK_BRANCH_NAME /*BANK_BRANCHID*/,              SMS_FLAG,               GSTIN_NO,                 DOB,                    AGENT_TYPE,           PAN,                  DIST,             
        AADHAR_CARD_NO,             POSP_MARKING ,          POSP_TYPE ,               POSP_NO_LI,             POSP_NO_GI,           POSP_CERTIFIED_ON_LI,  POSP_VALID_TILL_LI,          
        POSP_CERTIFIED_ON_GI,       POSP_VALID_TILL_GI,     VERIFIED_STATUS ,         /*NEFT_BANK_NAME,         BANK_BRANCH_NAME,     IFSC_CODE,            NAME_IN_BANK,*/           
        AMFICERT,                   AMFIEXTYPEID,           AMFIID,                   
        AGENT_MASTER.r_address1,  AGENT_MASTER.r_address2,  AGENT_MASTER.r_state_name, AGENT_MASTER.r_city_name, AGENT_MASTER.r_pincode

from agent_master where AGENT_CODE = '30058709';



SELECT 
        ANA_AUDIT,                  PAYMENTMODEID,          ACCTYPEID,                ACCNO,                  AFFECTEDFROM,         BANKID,               CITY_NAME  , BANK_BRANCH_NAME          
from agent_master where AGENT_CODE = '30058709';










DELETE FROM AGENT_MASTER where agent_name = 'TEST2';