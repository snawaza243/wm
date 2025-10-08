CREATE OR REPLACE PROCEDURE PSM_LOG_PERMISSION(
    PX_LOGID    IN VARCHAR2,
    PX_ROLEID   IN VARCHAR2,
    PX_FOR      IN VARCHAR2 DEFAULT NULL,
    PX_CURSOR   OUT SYS_REFCURSOR
)AS 

BEGIN

    BEGIN -- ROLEPERMISSION

        IF PX_FOR = 'ROLEPERMISSION' THEN
            
            select * from user_master 
            where login_id=PX_LOGID 
            and login_pass= V_PASS
            and status='1' 
            AND LOGINVARIFY(frmLoginMaster)=1;
        END IF;
    END;





END;