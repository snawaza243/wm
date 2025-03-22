CREATE OR REPLACE PROCEDURE MERGE_INVESTORS(
    P_MSFGMAIN_CODE         IN VARCHAR2,
    P_MSFGMERGEDINVESTORS   IN TABLE_OF_VARCHAR2
) AS
    V_NEW_INV_CODE          VARCHAR2(50);
    V_MCOUNT                NUMBER := 0;
    V_FLAG                  BOOLEAN := TRUE;
    V_BRANCH_CD             VARCHAR2(50);
    V_RM_CD                 VARCHAR2(50);
    V_FAM_HEAD              VARCHAR2(50);
    V_MEMBERS1              VARCHAR2(50);
    V_MEMBERS2              VARCHAR2(50);
    V_MEMBERS3              VARCHAR2(50);
BEGIN
    -- Initialize variables
    V_BRANCH_CD := '';
    V_RM_CD := '';
    
    -- Begin transaction
    BEGIN
        -- Loop through each merged investor code
        FOR I IN 1..P_MSFGMERGEDINVESTORS.COUNT LOOP
            -- Find RM and Branch Code (Assuming this is done by a function)
            -- Find_RM(v_branch_cd, v_Rm_cd);
            
            -- Process each investor
            FOR REC IN (SELECT INV_CODE, INVESTOR_NAME 
                        FROM INVESTOR_MASTER 
                        WHERE SOURCE_ID = P_MSFGMERGEDINVESTORS(I)) LOOP
                
                -- Check if investor exists in the main code
                BEGIN
                    SELECT INV_CODE INTO V_NEW_INV_CODE
                    FROM INVESTOR_MASTER
                    WHERE SOURCE_ID = P_MSFGMAIN_CODE
                    AND UPPER(REPLACE(REPLACE(INVESTOR_NAME, '.', ''), ' ', '')) LIKE '%' || UPPER(REPLACE(REPLACE(REC.INVESTOR_NAME, '.', ''), ' ', '')) || '%'
                    AND INSTR(UPPER(INVESTOR_NAME), 'HUF') = 0;
                EXCEPTION
                    WHEN NO_DATA_FOUND THEN
                        V_MCOUNT := V_MCOUNT + 1;
                        IF V_MCOUNT >= 999 THEN
                            V_NEW_INV_CODE := P_MSFGMAIN_CODE || LPAD(V_MCOUNT, 5, '0');
                        ELSE
                            V_NEW_INV_CODE := P_MSFGMAIN_CODE || LPAD(V_MCOUNT, 3, '0');
                        END IF;
                        
                        -- Update investor_master
                        UPDATE INVESTOR_MASTER
                        SET SOURCE_ID = P_MSFGMAIN_CODE,
                            BRANCH_CODE = V_BRANCH_CD,
                            RM_CODE = V_RM_CD,
                            INV_CODE = V_NEW_INV_CODE
                        WHERE INV_CODE = REC.INV_CODE;
                END;
                
                -- Update related tables
                UPDATE FP_INVESTOR SET FAMILYHEAD_CODE = V_NEW_INV_CODE
                WHERE FAMILYHEAD_CODE = REC.INV_CODE;
                
                UPDATE FP_INVESTOR
                SET FAM_MEM1 = REPLACE(FAM_MEM1, REC.INV_CODE, V_NEW_INV_CODE)
                WHERE FAMILYHEAD_CODE LIKE SUBSTR(REC.INV_CODE, 1, 8) || '%'
                OR FAMILYHEAD_CODE LIKE SUBSTR(V_NEW_INV_CODE, 1, 8) || '%';
                
                
                UPDATE FP_INVESTOR
                SET FAM_MEM1 = REPLACE(FAM_MEM1, REC.INV_CODE, V_NEW_INV_CODE)
                WHERE FAMILYHEAD_CODE LIKE SUBSTR(REC.INV_CODE, 1, 8) || '%'
                OR FAMILYHEAD_CODE LIKE SUBSTR(V_NEW_INV_CODE, 1, 8) || '%';
                
                
                UPDATE FP_INVESTOR
                SET FAM_MEM1 = REPLACE(FAM_MEM1, REC.INV_CODE, V_NEW_INV_CODE)
                WHERE FAMILYHEAD_CODE LIKE SUBSTR(REC.INV_CODE, 1, 8) || '%'
                OR FAMILYHEAD_CODE LIKE SUBSTR(V_NEW_INV_CODE, 1, 8) || '%';
                
                -- Repeat for fam_mem2 and fam_mem3
                
                -- Update transaction tables
                UPDATE TRANSACTION_ST
                SET CLIENT_CODE = V_NEW_INV_CODE,
                    BRANCH_CODE = V_BRANCH_CD,
                    SOURCE_CODE = P_MSFGMAIN_CODE,
                    RMCODE = V_RM_CD,
                    MODIFY_TALISMA = SYSDATE
                WHERE CLIENT_CODE = REC.INV_CODE;
                
                -- Repeat for other transaction tables...
                
                -- Insert into history table
                INSERT INTO INV_DEL_HIST_AGENT_MERGE (INV_CODE, NEW_INV_CODE, UPDATEON, UPDATEDBY)
                VALUES (REC.INV_CODE, V_NEW_INV_CODE, SYSDATE, USER);
                
                -- Delete old records if necessary
                IF V_NEW_INV_CODE IS NOT NULL THEN
                    INSERT INTO CLIENT_INV_MERGE_LOG VALUES (V_NEW_INV_CODE, REC.INV_CODE, USER, SYSDATE);
                    DELETE FROM INVESTOR_MASTER WHERE INV_CODE = REC.INV_CODE;
                END IF;
            END LOOP;
            
            -- Update agent_master
            UPDATE AGENT_MASTER
            SET SOURCEID = V_BRANCH_CD,
                RM_CODE = V_RM_CD,
                MODIFY_DATE = SYSDATE
            WHERE AGENT_CODE = P_MSFGMAIN_CODE;
            
            -- Insert into agent_del
            INSERT INTO AGENT_DEL
            SELECT * FROM AGENT_MASTER WHERE AGENT_CODE = P_MSFGMERGEDINVESTORS(I);
            
            -- Delete from agent_master
            DELETE FROM AGENT_MASTER WHERE AGENT_CODE = P_MSFGMERGEDINVESTORS(I);
            
            -- Insert into history table
            INSERT INTO AGENT_DEL_HIST_AGENT_MERGE (AGENT_CODE, NEW_AGENT_CODE, UPDATEON, UPDATEDBY)
            VALUES (P_MSFGMERGEDINVESTORS(I), P_MSFGMAIN_CODE, SYSDATE, USER);
        END LOOP;
        
        -- Commit transaction
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            -- Rollback transaction on error
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20001, 'Error occurred: ' || SQLERRM);
    END;
END MERGE_INVESTORS;
/