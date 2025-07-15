create or replace PROCEDURE PSM_ANA_MergeAgent(
    p_main_agent_exist_code IN VARCHAR2,
    p_to_merge_agent_exist_codes IN VARCHAR2
) AS
    v_rm_code VARCHAR2(100);  
BEGIN
    -- Step 1: Get the rm_code of the main agent
    SELECT rm_code INTO v_rm_code
    FROM agent_master
    WHERE exist_code = p_main_agent_exist_code;

    -- Step 2: Update the rmcode in TRANSACTION_ST for all agents to merge
    UPDATE TRANSACTION_ST
    SET rmcode = v_rm_code
    WHERE rmcode IN (
        SELECT REGEXP_SUBSTR(p_to_merge_agent_exist_codes, '[^,]+', 1, LEVEL)
        FROM dual
        CONNECT BY REGEXP_SUBSTR(p_to_merge_agent_exist_codes, '[^,]+', 1, LEVEL) IS NOT NULL
    );

    -- Step 3: Delete all agents from agent_master for the exist codes to merge
    DELETE FROM agent_master
    WHERE exist_code IN (
        SELECT REGEXP_SUBSTR(p_to_merge_agent_exist_codes, '[^,]+', 1, LEVEL)
        FROM dual
        CONNECT BY REGEXP_SUBSTR(p_to_merge_agent_exist_codes, '[^,]+', 1, LEVEL) IS NOT NULL
    );

    -- Commit the transaction to save changes
    COMMIT;

EXCEPTION
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20001, 'No agent found with the specified exist code: ' || p_main_agent_exist_code);
    WHEN OTHERS THEN
        ROLLBACK; -- Rollback in case of an error
        RAISE_APPLICATION_ERROR(-20002, 'Error during merging agents: ' || SQLERRM);
END PSM_ANA_MergeAgent;
