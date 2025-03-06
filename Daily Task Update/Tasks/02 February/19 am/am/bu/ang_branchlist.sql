create or replace PROCEDURE PSM_AM_BRANCH_MASTER_3  (
    p_login         IN VARCHAR2,
    p_roleID        IN VARCHAR2,
    p_mark1         in VARCHAR2,
    p_mark2         in VARCHAR2,
    p_branch_cursor OUT SYS_REFCURSOR
) AS
BEGIN

    if p_mark1 is null then
            OPEN p_branch_cursor FOR
        SELECT *
        FROM BRANCH_MASTER BR
        where branch_tar_cat in (186, 615, 308)
        --(184, 185, 186, 187, 189, 231, 283, 308, 368, 369, 609, 610, 611, 615) -- branch_tar_cat - 186, 615, 308 corporate partner , -- only ang : 189, 231, 186
        and branch_code is not null 
        AND EXISTS( SELECT 1 FROM USERDETAILS_JI WHERE BRANCH_ID = BR.BRANCH_CODE AND LOGIN_ID = p_login AND ROLE_ID = p_roleID)


        ORDER BY BRANCH_NAME; 
    else
        OPEN p_branch_cursor FOR
        SELECT *
        FROM BRANCH_MASTER BR
        where branch_tar_cat in  (186, 615, 308)
        --(184, 185, 186, 187, 189, 231, 283, 308, 368, 369, 609, 610, 611, 615)
        and branch_code is not null 
        AND EXISTS( SELECT 1 FROM USERDETAILS_JI WHERE BRANCH_ID = BR.BRANCH_CODE AND LOGIN_ID = p_login AND ROLE_ID = p_roleID)

        ORDER BY BRANCH_NAME; 
    end if;
END PSM_AM_BRANCH_MASTER_3;