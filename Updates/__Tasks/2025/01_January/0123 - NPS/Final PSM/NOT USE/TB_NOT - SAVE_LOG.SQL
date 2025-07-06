create or replace PROCEDURE PSM_NPS_SAVE_LOGIN_ACTION (
    p_user_id      IN VARCHAR2,
    p_action       IN VARCHAR2,
    p_source       IN VARCHAR2,
    p_timestamp    IN DATE
) AS
BEGIN
    INSERT INTO user_action_log (user_id, action, source, timestamp)
    VALUES (p_user_id, p_action, p_source, p_timestamp);

    COMMIT; -- Commit the transaction if needed
EXCEPTION
    WHEN OTHERS THEN
        -- Handle exceptions if necessary
        RAISE; -- Re-raise the exception for the calling application to handle
END PSM_NPS_SAVE_LOGIN_ACTION;