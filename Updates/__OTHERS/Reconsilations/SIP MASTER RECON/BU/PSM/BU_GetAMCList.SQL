CREATE OR REPLACE PROCEDURE WEALTHMAKER.GetAMCList (
    p_cursor OUT SYS_REFCURSOR  -- Output cursor for the result set
)
AS
BEGIN
    -- Fetch the combined result from MUT_FUND and ISS_MASTER (via other_product)
    OPEN p_cursor FOR
        SELECT MUT_NAME , MUT_CODE
        FROM MUT_FUND
        UNION ALL
        SELECT B.ISS_NAME AS MUT_NAME ,
        B.ISS_CODE AS MUT_CODE
        FROM ISS_MASTER B
        JOIN OTHER_PRODUCT A ON A.ISS_CODE = B.ISS_CODE
        WHERE B.PROD_CODE IN ('DT020', 'DT027','DT046')
        ORDER BY MUT_NAME;  -- Sorting by MUT_NAME (or ISS_NAME)
END GetAMCList;
/
